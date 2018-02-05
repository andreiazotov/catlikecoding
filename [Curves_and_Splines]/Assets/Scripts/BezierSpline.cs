using UnityEngine;
using System;

public class BezierSpline : MonoBehaviour
{
    [SerializeField]
    private Vector3[] points;

    [SerializeField]
    private BezierControlPointMode[] modes;

    [SerializeField]
    private bool loop;

    public bool Loop
    {
        get
        {
            return loop;
        }
        set
        {
            loop = value;
            if (value == true)
            {
                this.modes[this.modes.Length - 1] = this.modes[0];
                this.SetControlPoint(0, this.points[0]);
            }
        }
    }

    public void Reset()
    {
        this.points = new Vector3[]
        {
            new Vector3(1.0f, 0.0f, 0.0f),
            new Vector3(2.0f, 0.0f, 0.0f),
            new Vector3(3.0f, 0.0f, 0.0f),
            new Vector3(4.0f, 0.0f, 0.0f),
        };

        this.modes = new BezierControlPointMode[] {
            BezierControlPointMode.Free,
            BezierControlPointMode.Free
        };

    }

    public int ControlPointCount
    {
        get
        {
            return this.points.Length;
        }
    }

    public Vector3 GetControlPoint(int index)
    {
        return this.points[index];
    }

    public void SetControlPoint(int index, Vector3 point)
    {
        if (index % 3 == 0)
        {
            Vector3 delta = point - this.points[index];
            if (loop)
            {
                if (index == 0)
                {
                    this.points[1] += delta;
                    this.points[this.points.Length - 2] += delta;
                    this.points[this.points.Length - 1] = point;
                }
                else if (index == this.points.Length - 1)
                {
                    this.points[0] = point;
                    this.points[1] += delta;
                    this.points[index - 1] += delta;
                }
                else
                {
                    this.points[index - 1] += delta;
                    this.points[index + 1] += delta;
                }
            }
            else
            {
                if (index > 0)
                {
                    this.points[index - 1] += delta;
                }
                if (index + 1 < points.Length)
                {
                    this.points[index + 1] += delta;
                }
            }
        }
        this.points[index] = point;
        this.EnforceMode(index);
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

        Array.Resize(ref this.modes, this.modes.Length + 1);
        this.modes[this.modes.Length - 1] = this.modes[this.modes.Length - 2];

        this.EnforceMode(this.points.Length - 4);

        if (loop)
        {
            this.points[points.Length - 1] = this.points[0];
            this.modes[modes.Length - 1] = this.modes[0];
            EnforceMode(0);
        }
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

    public BezierControlPointMode GetControlPointMode(int index)
    {
        return this.modes[(index + 1) / 3];
    }

    public void SetControlPointMode(int index, BezierControlPointMode mode)
    {
        int modeIndex = (index + 1) / 3;
        this.modes[modeIndex] = mode;
        if (loop)
        {
            if (modeIndex == 0)
            {
                this.modes[this.modes.Length - 1] = mode;
            }
            else if (modeIndex == this.modes.Length - 1)
            {
                this.modes[0] = mode;
            }
        }
        this.EnforceMode(index);
    }

    private void EnforceMode(int index)
    {
        int modeIndex = (index + 1) / 3;
        var mode = this.modes[modeIndex];
        if (mode == BezierControlPointMode.Free || !this.loop && (modeIndex == 0 || modeIndex == this.modes.Length - 1))
        {
            return;
        }

        int middleIndex = modeIndex * 3;
        int fixedIndex, enforcedIndex;
        if (index <= middleIndex)
        {
            fixedIndex = middleIndex - 1;
            if (fixedIndex < 0)
            {
                fixedIndex = this.points.Length - 2;
            }
            enforcedIndex = middleIndex + 1;
            if (enforcedIndex >= this.points.Length)
            {
                enforcedIndex = 1;
            }
        }
        else
        {
            fixedIndex = middleIndex + 1;
            if (fixedIndex >= this.points.Length)
            {
                fixedIndex = 1;
            }
            enforcedIndex = middleIndex - 1;
            if (enforcedIndex < 0)
            {
                enforcedIndex = this.points.Length - 2;
            }
        }

        Vector3 middle = this.points[middleIndex];
        Vector3 enforcedTangent = middle - this.points[fixedIndex];
        if (mode == BezierControlPointMode.Aligned)
        {
            enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, this.points[enforcedIndex]);
        }
        this.points[enforcedIndex] = middle + enforcedTangent;
    }
}
