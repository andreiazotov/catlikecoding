using UnityEngine;

public sealed class ShapeBehaviorLifecycle : ShapeBehavior
{
    private float _adultDuration;
    private float _dyingDuration;
    private float _dyingAge;

    public override ShapeBehaviorType BehaviorType => ShapeBehaviorType.Lifecycle;

    public void Initialize(Shape shape, float growingDuration, float adultDuration, float dyingDuration)
    {
        _adultDuration = adultDuration;
        _dyingDuration = dyingDuration;
        _dyingAge = growingDuration + adultDuration;

        if (growingDuration > 0f)
        {
            shape.AddBehavior<ShapeBehaviorGrow>().Initialize(shape, growingDuration);
        }
    }

    public override bool GameUpdate(Shape shape)
    {
        if(shape.Age >= _dyingAge)
        {
            if (_dyingDuration <= 0.0f)
            {
                shape.Die();
                return true;
            }
            if (!shape.IsMarkedAsDying)
            {
                shape.AddBehavior<ShapeBehaviorDie>().Initialize(shape, _dyingDuration + _dyingAge - shape.Age);
            }
            return false;
        }
        return true;
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(_adultDuration);
        writer.Write(_dyingDuration);
        writer.Write(_dyingAge);
    }

    public override void Load(GameDataReader reader)
    {
        _adultDuration = reader.ReadFloat();
        _dyingDuration = reader.ReadFloat();
        _dyingAge = reader.ReadFloat();
    }

    public override void Recycle()
    {
        ShapeBehaviorPool<ShapeBehaviorLifecycle>.Reclaim(this);
    }
}
