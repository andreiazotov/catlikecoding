using UnityEngine;

public class CameraTransformation : Transformation
{
    public float focalLength = 1.0f;

    public override Matrix4x4 Matrix
    {
        get
        {
            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetRow(0, new Vector4(this.focalLength, 0f, 0f, 0f));
            matrix.SetRow(1, new Vector4(0f, this.focalLength, 0f, 0f));
            matrix.SetRow(2, new Vector4(0f, 0f, 1f, 0f));
            matrix.SetRow(3, new Vector4(0f, 0f, 0f, 1f));
            return matrix;
        }
    }
}
