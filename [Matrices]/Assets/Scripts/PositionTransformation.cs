using UnityEngine;

public class PositionTransformation : Transformation
{
    public Vector3 position;

    public override Matrix4x4 Matrix
    {
        get
        {
            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetRow(0, new Vector4(1f, 0f, 0f, this.position.x));
            matrix.SetRow(1, new Vector4(0f, 1f, 0f, this.position.y));
            matrix.SetRow(2, new Vector4(0f, 0f, 1f, this.position.z));
            matrix.SetRow(3, new Vector4(0f, 0f, 0f, 1f));
            return matrix;
        }
    }
}
