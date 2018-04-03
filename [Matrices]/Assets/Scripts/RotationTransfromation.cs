using UnityEngine;

public class RotationTransfromation : Transfromation
{
    public Vector3 rotation;

    public override Vector3 Apply(Vector3 point)
    {
        float radZ = this.rotation.z * Mathf.Deg2Rad;
        float sinZ = Mathf.Sin(radZ);
        float cosZ = Mathf.Cos(radZ);

        return new Vector3(
            point.x * cosZ - point.y * sinZ,
            point.x * sinZ + point.y * cosZ,
            point.z
        );
    }
}
