using UnityEngine;

public sealed class ShapeBehaviorMove : ShapeBehavior
{
    public Vector3 Velocity { get; set; }
    public override ShapeBehaviorType BehaviorType => ShapeBehaviorType.Move;

    public override bool GameUpdate(Shape shape)
    {
        shape.transform.localPosition += Velocity * Time.deltaTime;
        return true;
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(Velocity);
    }

    public override void Load(GameDataReader reader)
    {
        Velocity = reader.ReadVector3();
    }

    public override void Recycle()
    {
        ShapeBehaviorPool<ShapeBehaviorMove>.Reclaim(this);
    }
}
