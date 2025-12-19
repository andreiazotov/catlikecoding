using UnityEngine;
using static SpawnZone;

public abstract class SpawnZone : PersistableObject
{
    public enum SpawnMovementDirection
    {
        Forward,
        Upward,
        Outward,
        Random
    }

    [SerializeField]
    private SpawnConfiguration _spawnConfig;

    public abstract Vector3 SpawnPoint
    {
        get;
    }

    public virtual Shape SpawnShape()
    {
        int factoryIndex = Random.Range(0, _spawnConfig.factories.Length);
        Shape shape = _spawnConfig.factories[factoryIndex].GetRandom();
        Transform t = shape.transform;
        t.SetLocalPositionAndRotation(SpawnPoint, Random.rotation);
        t.localScale = Vector3.one * _spawnConfig.scale.RandomValueInRange;
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
        return shape;
    }

    void SetupOscillation(Shape shape)
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

    Vector3 GetDirectionVector(SpawnConfiguration.MovementDirection direction, Transform t)
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