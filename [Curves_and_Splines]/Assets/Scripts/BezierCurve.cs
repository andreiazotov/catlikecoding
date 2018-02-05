using UnityEngine;

public class BezierCurve : MonoBehaviour
{
    public Vector3[] points;

    public void Reset()
    {
        points = new Vector3[]
        {
            new Vector3(1.0f, 0.0f, 0.0f),
            new Vector3(2.0f, 0.0f, 0.0f),
            new Vector3(3.0f, 0.0f, 0.0f),
            new Vector3(4.0f, 0.0f, 0.0f),
        };
    }

    public Vector3 GetPoint(float t)
    {
        return this.transform.TransformPoint(Bezier.GetPoint(this.points[0], this.points[1], this.points[2], this.points[3], t));
    }

    public Vector3 GetVelocity(float t)
    {
        return this.transform.TransformPoint(Bezier.GetFirstDerivative(this.points[0], this.points[1], this.points[2], this.points[3], t)) - this.transform.position;
    }

    public Vector3 GetDirection(float t)
    {
        return this.GetVelocity(t).normalized;
    }
}
