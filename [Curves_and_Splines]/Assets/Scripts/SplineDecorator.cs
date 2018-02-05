using UnityEngine;

public class SplineDecorator : MonoBehaviour
{
    public BezierSpline spline;
    public int frequency;
    public bool lookForward;
    public Transform[] items;

    private void Awake()
    {
        if (this.frequency <= 0 || this.items == null || this.items.Length == 0)
        {
            return;
        }
        float stepSize = this.frequency * this.items.Length;
        if (this.spline.Loop || stepSize == 1)
        {
            stepSize = 1f / stepSize;
        }
        else
        {
            stepSize = 1f / (stepSize - 1);
        }
        for (int p = 0, f = 0; f < this.frequency; f++)
        {
            for (int i = 0; i < this.items.Length; i++, p++)
            {
                Transform item = Instantiate(this.items[i]) as Transform;
                Vector3 position = this.spline.GetPoint(p * stepSize);
                item.transform.localPosition = position;
                if (this.lookForward)
                {
                    item.transform.LookAt(position + this.spline.GetDirection(p * stepSize));
                }
                item.transform.parent = this.transform;
            }
        }
    }
}