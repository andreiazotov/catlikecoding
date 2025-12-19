using UnityEngine;

public sealed class ShapeBehaviorOscillate : ShapeBehavior
{
    public Vector3 Offset { get; set; }
    public float Frequency { get; set; }

    private float _previousOscillation;

    public override ShapeBehaviorType BehaviorType => ShapeBehaviorType.Oscillate;

    public override void GameUpdate(Shape shape)
    {
        float oscillation = Mathf.Sin(2.0f * Mathf.PI * Frequency * Time.time * shape.Age);
        shape.transform.localPosition += (oscillation - _previousOscillation) * Offset;
        _previousOscillation = oscillation;
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(Offset);
        writer.Write(Frequency);
        writer.Write(_previousOscillation);
    }

    public override void Load(GameDataReader reader)
    {
        Offset = reader.ReadVector3();
        Frequency = reader.ReadFloat();
        _previousOscillation = reader.ReadFloat();
    }

    public override void Recycle()
    {
        _previousOscillation = 0.0f;
        ShapeBehaviorPool<ShapeBehaviorOscillate>.Reclaim(this);
    }
}