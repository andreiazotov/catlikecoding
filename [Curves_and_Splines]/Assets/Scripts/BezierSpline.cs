using UnityEngine;
using System;

public class BezierSpline : MonoBehaviour
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

    public void AddCurve()
    {
        var point = this.points[this.points.Length - 1];
        Array.Resize(ref this.points, this.points.Length + 3);
        point.x += 1.0f;
        this.points[points.Length - 3] = point;
        point.x += 1.0f;
        this.points[points.Length - 2] = point;
        point.x += 1.0f;
        this.points[points.Length - 1] = point;
    }

    public Vector3 GetPoint(float t)
    {
        int i;
        if (t >= 1f)
        {
            t = 1f;
            i = points.Length - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }
        return this.transform.TransformPoint(Bezier.GetPoint(this.points[i], this.points[i + 1], this.points[i + 2], this.points[i + 3], t));
    }

    public Vector3 GetVelocity(float t)
    {
        int i;
        if (t >= 1f)
        {
            t = 1f;
            i = points.Length - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }
        return this.transform.TransformPoint(Bezier.GetFirstDerivative(this.points[i], this.points[i + 1], this.points[i + 2], this.points[i + 3], t)) - this.transform.position;
    }

    public Vector3 GetDirection(float t)
    {
        return this.GetVelocity(t).normalized;
    }

    public int CurveCount
    {
        get { return (this.points.Length - 1) / 3; }
    }
}
