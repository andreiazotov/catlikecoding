using UnityEngine;

public class PositionTransfromation : Transfromation
{
    public Vector3 position;

    public override Vector3 Apply(Vector3 point)
    {
        return point + this.position;
    }
}
