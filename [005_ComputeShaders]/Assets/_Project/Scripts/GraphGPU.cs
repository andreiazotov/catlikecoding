using UnityEngine;

public class GraphGPU : MonoBehaviour
{
    public enum TransitionMode
    {
        Cycle,
        Random,
    }

    [SerializeField]
    private ComputeShader _computeShader;

    [SerializeField]
    private Material _material;

    [SerializeField]
    private Mesh _mesh;

    [SerializeField, Range(10, 1000)]
    private int _pointsCounter = 10;

    [SerializeField]
    private FunctionLibrary.FuncName _function;

    [SerializeField]
    private TransitionMode _transitionMode;

    [SerializeField, Min(0.0f)]
    private float _functionDuration = 1.0f;

    private float _duration = 0.0f;

    private ComputeBuffer _positionBuffer;

    private static readonly int positionsId = Shader.PropertyToID("_Positions");
    private static readonly int resolutionId = Shader.PropertyToID("_Resolution");
    private static readonly int stepId = Shader.PropertyToID("_Step");
    private static readonly int timeId = Shader.PropertyToID("_Time");

    private void OnEnable()
    {
        _positionBuffer = new ComputeBuffer(_pointsCounter * _pointsCounter, 3 * 4);
    }

    private void Update()
    {
        UpdateFunctionOnGPU();
    }

    private void UpdateFunctionOnGPU()
    {
        float step = 2.0f / _pointsCounter;
        int groups = Mathf.CeilToInt(_pointsCounter / 8.0f);
        _computeShader.SetInt(resolutionId, _pointsCounter);
        _computeShader.SetFloat(stepId, step);
        _computeShader.SetFloat(timeId, Time.time);
        _computeShader.SetBuffer(0, positionsId, _positionBuffer);
        _computeShader.Dispatch(0, groups, groups, 1);

        // Because this way of drawing doesn't use game objects Unity doesn't know
        // where in the scene the drawing happens. We have to indicate this by providing
        // a bounding box as an additional argument. This is an axis-aligned box that
        // indicates the spatial bounds of whatever we're drawing. Unity uses this to determine
        // whether the drawing can be skipped, because it ends up outside the field of view of the camera.
        // This is known as frustum culling. So instead of evaluating the bounds per point it now
        // happens for the entire graph at once. This is fine for our graph,
        // as the idea is that we view it in its entirety.
        _material.SetBuffer(positionsId, _positionBuffer);
        _material.SetFloat(stepId, step);
        var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / _pointsCounter));
        Graphics.DrawMeshInstancedProcedural(_mesh, 0, _material, bounds, _positionBuffer.count);
    }

    private void ResolveNextFunction()
    {
        _function = _transitionMode == TransitionMode.Cycle ? FunctionLibrary.GetNextFuncName(_function) : FunctionLibrary.GetRandomFuncName(_function);
    }

    void OnDisable()
    {
        // It will get released eventually if nothing holds a reference to the object,
        // when the garbage collector reclaims it. But when this happens is arbitrary.
        // It's best to release it explicitly as soon as possible, to avoid clogging memory.
        _positionBuffer.Release();
        _positionBuffer = null;
    }
}
