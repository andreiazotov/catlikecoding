using UnityEngine;

public sealed class ShapeBehaviorDie : ShapeBehavior
{
    private Vector3 _originalScale;
    private float _duration;
    private float _dyingAge;

    public override ShapeBehaviorType BehaviorType => ShapeBehaviorType.Die;

    public void Initialize(Shape shape, float duration)
    {
        _originalScale = shape.transform.localScale;
        _duration = duration;
        _dyingAge = shape.Age;
        shape.MarkAsDying();
    }

    public override bool GameUpdate(Shape shape)
    {
        float dyingDuration = shape.Age - _dyingAge;
        if (dyingDuration < _duration)
        {
            float s = 1.0f - dyingDuration / _duration;
            s = (3.0f - 2.0f * s) * s * s;
            shape.transform.localScale = s * _originalScale;
            return true;
        }
        shape.Die();
        return true;
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(_originalScale);
        writer.Write(_duration);
        writer.Write(_dyingAge);
    }

    public override void Load(GameDataReader reader)
    {
        _originalScale = reader.ReadVector3();
        _duration = reader.ReadFloat();
        _dyingAge = reader.ReadFloat();
    }

    public override void Recycle()
    {
        ShapeBehaviorPool<ShapeBehaviorDie>.Reclaim(this);
    }
}
