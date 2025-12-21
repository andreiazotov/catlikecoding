using UnityEngine;

public sealed class ShapeBehaviorRotate : ShapeBehavior
{
    public Vector3 AngularVelocity { get; set; }
    public override ShapeBehaviorType BehaviorType => ShapeBehaviorType.Rotate;

    public override bool GameUpdate(Shape shape)
    {
        shape.transform.Rotate(AngularVelocity * Time.deltaTime);
        return true;
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(AngularVelocity);
    }

    public override void Load(GameDataReader reader)
    {
        AngularVelocity = reader.ReadVector3();
    }

    public override void Recycle()
    {
        ShapeBehaviorPool<ShapeBehaviorRotate>.Reclaim(this);
    }
}
