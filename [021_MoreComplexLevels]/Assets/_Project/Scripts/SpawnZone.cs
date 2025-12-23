using UnityEngine;

public abstract class SpawnZone : GameLevelObject
{
    public enum SpawnMovementDirection
    {
        Forward,
        Upward,
        Outward,
        Random
    }

    [SerializeField, Range(0.0f, 50.0f)]
    private float _spawnSpeed;

    [SerializeField]
    private SpawnConfiguration _spawnConfig;

    public abstract Vector3 SpawnPoint
    {
        get;
    }

    private float _spawnProgress;

    public override void GameUpdate()
    {
        _spawnProgress += Time.deltaTime * _spawnSpeed;
        while (_spawnProgress >= 1.0f)
        {
            _spawnProgress -= 1.0f;
            SpawnShape();
        }
    }

    public virtual void SpawnShape()
    {
        int factoryIndex = Random.Range(0, _spawnConfig.factories.Length);
        Shape shape = _spawnConfig.factories[factoryIndex].GetRandom();
        Transform t = shape.transform;
        t.SetLocalPositionAndRotation(SpawnPoint, Random.rotation);
        t.localScale = Vector3.one * _spawnConfig.scale.RandomValueInRange;
        SetupColor(shape);

        float angularSpeed = _spawnConfig.angularSpeed.RandomValueInRange;
        if (angularSpeed != 0.0f)
        {
            var rotation = shape.AddBehavior<ShapeBehaviorRotate>();
            rotation.AngularVelocity = Random.onUnitSphere * angularSpeed;
        }
        float speed = _spawnConfig.speed.RandomValueInRange;
        if (speed != 0.0f)
        {
            var movement = shape.AddBehavior<ShapeBehaviorMove>();
            movement.Velocity = GetDirectionVector(_spawnConfig.movementDirection, t) * speed;
        }
        SetupOscillation(shape);
        Vector3 lifecycleDurations = _spawnConfig.lifecycle.RandomDurations;
        int satelliteCount = _spawnConfig.satellite.amount.RandomValueInRange;
        for (int i = 0; i < satelliteCount; i++)
        {
            CreateSatelliteFor(shape, _spawnConfig.satellite.uniformLifecycles ?  lifecycleDurations : _spawnConfig.lifecycle.RandomDurations);
        }
        SetupLifecycle(shape, lifecycleDurations);
    }

    private void CreateSatelliteFor(Shape focalShape, Vector3 lifecycleDurations)
    {
        int factoryIndex = Random.Range(0, _spawnConfig.factories.Length);
        Shape shape = _spawnConfig.factories[factoryIndex].GetRandom();
        Transform t = shape.transform;
        t.localRotation = Random.rotation;
        t.localScale = focalShape.transform.localScale * _spawnConfig.satellite.relativeScale.RandomValueInRange; ;
        SetupColor(shape);
        shape.AddBehavior<ShapeBehaviorSatellite>().Initialize(shape, focalShape, _spawnConfig.satellite.orbitRadius.RandomValueInRange, _spawnConfig.satellite.orbitFrequency.RandomValueInRange);
        SetupLifecycle(shape, lifecycleDurations);
    }

    private void SetupColor(Shape shape)
    {
        if (_spawnConfig.uniformColor)
        {
            shape.SetColor(_spawnConfig.color.RandomInRange);
        }
        else
        {
            for (int i = 0; i < shape.ColorCount; i++)
            {
                shape.SetColor(_spawnConfig.color.RandomInRange, i);
            }
        }
    }

    private void SetupOscillation(Shape shape)
    {
        float amplitude = _spawnConfig.oscillationAmplitude.RandomValueInRange;
        float frequency = _spawnConfig.oscillationFrequency.RandomValueInRange;
        if (amplitude == 0f || frequency == 0f)
        {
            return;
        }
        var oscillation = shape.AddBehavior<ShapeBehaviorOscillate>();
        oscillation.Offset = GetDirectionVector(_spawnConfig.oscillationDirection, shape.transform) * amplitude;
        oscillation.Frequency = frequency;
    }

    private Vector3 GetDirectionVector(SpawnConfiguration.MovementDirection direction, Transform t)
    {
        switch (direction)
        {
            case SpawnConfiguration.MovementDirection.Upward:
                return transform.up;
            case SpawnConfiguration.MovementDirection.Outward:
                return (t.localPosition - transform.position).normalized;
            case SpawnConfiguration.MovementDirection.Random:
                return Random.onUnitSphere;
            default:
                return transform.forward;
        }
    }

    private void SetupLifecycle(Shape shape, Vector3 durations)
    {
        if (durations.x > 0.0f)
        {
            if (durations.y > 0.0f || durations.z > 0.0f)
            {
                shape.AddBehavior<ShapeBehaviorLifecycle>().Initialize(shape, durations.x, durations.y, durations.z);
            }
            else
            {
                shape.AddBehavior<ShapeBehaviorGrow>().Initialize(shape, durations.x);
            }
        }
        else if (durations.y > 0.0f)
        {
            shape.AddBehavior<ShapeBehaviorLifecycle>().Initialize(shape, durations.x, durations.y, durations.z);
        }
        else if (durations.z > 0.0f)
        {
            shape.AddBehavior<ShapeBehaviorDie>().Initialize(shape, durations.z);
        }
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(_spawnProgress);
    }

    public override void Load(GameDataReader reader)
    {
        _spawnProgress = reader.ReadFloat();
    }
}

[System.Serializable]
public struct FloatRange
{
    public float min;
    public float max;

    public float RandomValueInRange
    {
        get
        {
            return Random.Range(min, max);
        }
    }
}

[System.Serializable]
public struct IntRange
{
    public int min;
    public int max;

    public int RandomValueInRange
    {
        get
        {
            return Random.Range(min, max + 1);
        }
    }
}

[System.Serializable]
public struct SpawnConfiguration
{
    public enum MovementDirection
    {
        Forward,
        Upward,
        Outward,
        Random
    }
    public ShapeFactory[] factories;
    public MovementDirection movementDirection;
    public FloatRange speed;
    public FloatRange angularSpeed;
    public FloatRange scale;
    public ColorRangeHSV color;
    public bool uniformColor;
    public MovementDirection oscillationDirection;
    public FloatRange oscillationAmplitude;
    public FloatRange oscillationFrequency;
    public SatelliteConfiguration satellite;
    public LifecycleConfiguration lifecycle;
}

[System.Serializable]
public struct ColorRangeHSV
{
    [FloatRangeSlider(0.0f, 1.0f)]
    public FloatRange hue;

    [FloatRangeSlider(0.0f, 1.0f)]
    public FloatRange saturation;

    [FloatRangeSlider(0.0f, 1.0f)]
    public FloatRange value;

    public readonly Color RandomInRange => Random.ColorHSV(hue.min, hue.max, saturation.min, saturation.max, value.min, value.max, 1.0f, 1.0f);
}

[System.Serializable]
public struct SatelliteConfiguration
{
    public IntRange amount;
    [FloatRangeSlider(0.1f, 1.0f)]
    public FloatRange relativeScale;
    public FloatRange orbitRadius;
    public FloatRange orbitFrequency;
    public bool uniformLifecycles;
}

[System.Serializable]
public struct LifecycleConfiguration
{
    [FloatRangeSlider(0.0f, 2.0f)]
    public FloatRange growingDuration;

    [FloatRangeSlider(0.0f, 100.0f)]
    public FloatRange adultDuration;

    [FloatRangeSlider(0.0f, 2.0f)]
    public FloatRange dyingDuration;

    public Vector3 RandomDurations
    {
        get
        {
            return new Vector3(growingDuration.RandomValueInRange, adultDuration.RandomValueInRange, dyingDuration.RandomValueInRange);
        }
    }
}