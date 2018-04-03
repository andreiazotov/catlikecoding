using UnityEngine;

public class ScaleTransfromation : Transfromation
{
    public Vector3 scale;

    public override Vector3 Apply(Vector3 point)
    {
        return new Vector3(point.x * this.scale.x, point.y * this.scale.y, point.z * this.scale.z);
    }
}
