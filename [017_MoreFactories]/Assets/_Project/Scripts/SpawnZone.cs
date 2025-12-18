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
        shape.AngularVelocity = Random.onUnitSphere * _spawnConfig.angularSpeed.RandomValueInRange;

        Vector3 direction;
        switch (_spawnConfig.movementDirection)
        {
            case SpawnConfiguration.MovementDirection.Upward:
                direction = transform.up;
                break;
            case SpawnConfiguration.MovementDirection.Outward:
                direction = (t.localPosition - transform.position).normalized;
                break;
            case SpawnConfiguration.MovementDirection.Random:
                direction = Random.onUnitSphere;
                break;
            default:
                direction = transform.forward;
                break;
        }
        shape.Velocity = direction * _spawnConfig.speed.RandomValueInRange;
        return shape;
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