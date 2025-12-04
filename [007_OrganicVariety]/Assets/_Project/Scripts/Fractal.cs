using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
// Burst is specifically optimized to work with
// Unity's Mathematics library, which is designed
// with vectorization in mind.
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using static Unity.Mathematics.math;
using float4x4 = Unity.Mathematics.float4x4;
using quaternion = Unity.Mathematics.quaternion;

public class Fractal : MonoBehaviour
{
    [SerializeField, Range(2, 8)]
    private int _depth = 4;

    [SerializeField]
    private Mesh _mesh;

    [SerializeField]
    private Material _material;

    [SerializeField]
    private Gradient _gradient;

    [SerializeField, Range(0.0f, 90.0f)]
    private float _maxSagAngleA = 15.0f;

    [SerializeField, Range(0.0f, 90.0f)]
    private float _maxSagAngleB = 25.0f;

    [SerializeField, Range(0.0f, 90.0f)]
    private float _spinVelocityA = 20.0f;

    [SerializeField, Range(0.0f, 90.0f)]
    private float _spinVelocityB = 25.0f;

    [SerializeField, Range(0.0f, 1.0f)]
    private float _reverseSpinChance = 0.25f;

    public int Depth { get { return _depth; } set { _depth = value; } }

    // Jobs cannot work with objects, only simple values and struct types are allowed.
    // It's still possible to use arrays, but we have to convert them to the generic
    // NativeArray type. This is a struct that contains a pointer to native machine memory,
    // which exists outside the regular managed memory heap used by our C# code.
    // So it is sidestepping the default memory management overhead.
    // The only difference is that we're now using native arrays instead of managed C# arrays.
    // This might perform worse, because accessing native arrays from managed C# code has a little extra overhead.
    // This overhead won't exist once we use a Burst-compiled job.
    private NativeArray<FractalPart>[] _parts;
    private NativeArray<float4x4>[] _matrices;
    private ComputeBuffer[] _matricesBuffers;
    private Vector4[] _sequenceNumbers;

    private static readonly Vector3[] s_directions =
    {
        Vector3.up,
        Vector3.right,
        Vector3.left,
        Vector3.forward,
        Vector3.back
    };

    private static readonly Quaternion[] s_rotations =
    {
        Quaternion.identity,
        Quaternion.Euler(0f, 0f, -90f), Quaternion.Euler(0f, 0f, 90f),
        Quaternion.Euler(90f, 0f, 0f), Quaternion.Euler(-90f, 0f, 0f)
    };

    private static readonly float3[] s_directionsMath =
    {
        up(),
        right(),
        left(),
        forward(),
        back()
    };

    private static readonly quaternion[] s_rotationsMath =
    {
        quaternion.identity,
        quaternion.RotateZ(-0.5f * PI),
        quaternion.RotateZ(0.5f * PI),
        quaternion.RotateX(0.5f * PI),
        quaternion.RotateX(-0.5f * PI)
    };

    private static readonly int s_matricesId = Shader.PropertyToID("_Matrices");
    private static readonly int s_baseColorId = Shader.PropertyToID("_Base_Color");
    private static readonly int s_sequenceNumbersId = Shader.PropertyToID("_SequenceNumbers");

    private static MaterialPropertyBlock s_propertyBlock;

    private void OnEnable()
    {
        _parts = new NativeArray<FractalPart>[_depth];
        _matrices = new NativeArray<float4x4>[_depth];
        _matricesBuffers = new ComputeBuffer[_depth];
        _sequenceNumbers = new Vector4[_depth];
        for (int i = 0, length = 1; i < _parts.Length; i++, length *= 5)
        {
            // The first argument is the size of the array.
            // The second argument indicates how long the native array
            // is expected to exist. As we keep using the same arrays every frame
            // we'll have to use Allocator.Persistent.
            _parts[i] = new NativeArray<FractalPart>(length, Allocator.Persistent);
            _matrices[i] = new NativeArray<float4x4>(length, Allocator.Persistent);
            _matricesBuffers[i] = new ComputeBuffer(length, 16 * 4);
            _sequenceNumbers[i] = new Vector4(UnityEngine.Random.value, UnityEngine.Random.value);
        }

        _parts[0][0] = CreatePart(0);
        for (int li = 1; li < _parts.Length; li++)
        {
            var levelParts = _parts[li];
            for (int fpi = 0; fpi < levelParts.Length; fpi += 5)
            {
                for (int ci = 0; ci < 5; ci++)
                {
                    levelParts[fpi + ci] = CreatePart(ci);
                }
            }
        }

        s_propertyBlock ??= new MaterialPropertyBlock();
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        FractalPart rootPart = _parts[0][0];
        rootPart.SpinAngle += rootPart.SpinVelocity * deltaTime;
        rootPart.WorldRotation = mul(transform.rotation, mul(rootPart.Rotation, quaternion.RotateY(rootPart.SpinAngle)));
        rootPart.WorldPosition = transform.position;
        _parts[0][0] = rootPart;
        float objectScale = transform.lossyScale.x;
        _matrices[0][0] = Matrix4x4.TRS(rootPart.WorldPosition, rootPart.WorldRotation, float3(objectScale));
        float scale = objectScale;
        JobHandle jobHandle = default;
        for (int li = 1; li < _parts.Length; li++)
        {
            scale *= 0.5f;
            jobHandle = new UpdateFractalLevelJob
            {
                DeltaTime = deltaTime,
                Scale = scale,
                Parents = _parts[li - 1],
                Parts = _parts[li],
                Matrices = _matrices[li]
            }.ScheduleParallel(_parts[li].Length, 5, jobHandle);
        }
        jobHandle.Complete();

        var bounds = new Bounds(rootPart.WorldPosition, 3.0f * objectScale * Vector3.one);
        for (int i = 0; i < _matricesBuffers.Length; i++)
        {
            ComputeBuffer buffer = _matricesBuffers[i];
            buffer.SetData(_matrices[i]);
            s_propertyBlock.SetColor(s_baseColorId, _gradient.Evaluate(i / (_matricesBuffers.Length - 1.0f)));
            s_propertyBlock.SetBuffer(s_matricesId, buffer);
            s_propertyBlock.SetVector(s_sequenceNumbersId, _sequenceNumbers[i]);
            _material.SetBuffer(s_matricesId, buffer);
            Graphics.DrawMeshInstancedProcedural(_mesh, 0, _material, bounds, buffer.count, s_propertyBlock);
        }
    }

    private FractalPart CreatePart(int childIndex) => new()
    {
        MaxSagAngle = radians(UnityEngine.Random.Range(_maxSagAngleA, _maxSagAngleB)),
        SpinVelocity = (UnityEngine.Random.value < _reverseSpinChance ? -1.0f : 1.0f) * radians(UnityEngine.Random.Range(_spinVelocityA, _spinVelocityB)),
        Rotation = s_rotationsMath[childIndex],
    };

    private void OnDisable()
    {
        for (int i = 0; i < _matricesBuffers.Length; i++)
        {
            _matricesBuffers[i].Release();
            _parts[i].Dispose();
            _matrices[i].Dispose();
        }
        _parts = null;
        _matrices = null;
        _matricesBuffers = null;
        _sequenceNumbers = null;
    }

    private void OnValidate()
    {
        if (_parts != null && enabled)
        {
            OnDisable();
            OnEnable();
        }
    }

    public struct FractalPart
    {
        public quaternion Rotation;
        public float3 WorldPosition;
        public quaternion WorldRotation;
        public float SpinAngle;
        public float MaxSagAngle;
        public float SpinVelocity;
    }

    // If multiple processes are modifying the same data in parallel then it becomes
    // arbitrary which does what first. If two processes set the same array element
    // the last one wins. If one process gets the same element that another sets,
    // it either gets the old or the new value. The final result depends on the exact timing,
    // over which we have no control, which can lead to inconsistent behavior that is very hard
    // to detect and fix. These phenomena are known as race conditions.
    // The ReadOnly attribute indicates that this data remains constant during the execution
    // of the job, which means that processes can safely read from it in parallel
    // because the result will always be the same.
    // The compiler enforces that the job doesn't write to ReadOnly data and doesn't
    // read from WriteOnly data.If we accidentally do so anyway the compiler will
    // let us know that we made a semantic mistake.
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    struct UpdateFractalLevelJob : IJobFor
    {
        public float DeltaTime;
        public float Scale;

        [ReadOnly]
        public NativeArray<FractalPart> Parents;

        public NativeArray<FractalPart> Parts;

        [WriteOnly]
        public NativeArray<float4x4> Matrices;

        public void Execute(int i)
        {
            FractalPart parent = Parents[i / 5];
            FractalPart part = Parts[i];
            part.SpinAngle += part.SpinVelocity * DeltaTime;

            float3 upAxis = mul(mul(parent.WorldRotation, part.Rotation), up());
            float3 sagAxis = cross(up(), upAxis);
            sagAxis = normalize(sagAxis);

            float sagMagnitude = length(sagAxis);
            quaternion baseRotation;
            if (sagMagnitude > 0f)
            {
                sagAxis /= sagMagnitude;
                quaternion sagRotation = quaternion.AxisAngle(sagAxis, part.MaxSagAngle * sagMagnitude);
                baseRotation = mul(sagRotation, parent.WorldRotation);
            }
            else
            {
                baseRotation = parent.WorldRotation;
            }

            part.WorldRotation = mul(baseRotation, mul(part.Rotation, quaternion.RotateY(part.SpinAngle)));
            part.WorldPosition = parent.WorldPosition + mul(part.WorldRotation, float3(0f, 1.5f * Scale, 0f));
            Parts[i] = part;
            Matrices[i] = Matrix4x4.TRS(part.WorldPosition, part.WorldRotation,float3(Scale));
        }
    }
}
