using UnityEngine;

public class SplineWalker : MonoBehaviour
{
    public BezierSpline spline;
    public float duration;
    public bool lookForward;
    public SplineWalkerMode mode;

    private bool _goingForward = true;
    private float _progress;

    private void Update()
    {
        if (this._goingForward)
        {
            this._progress += Time.deltaTime / this.duration;
            if (this._progress > 1f)
            {
                if (mode == SplineWalkerMode.Once)
                {
                    this._progress = 1f;
                }
                else if (mode == SplineWalkerMode.Loop)
                {
                    this._progress -= 1f;
                }
                else
                {
                    this._progress = 2f - this._progress;
                    this._goingForward = false;
                }
            }
        }
        else
        {
            this._progress -= Time.deltaTime / this.duration;
            if (this._progress < 0f)
            {
                this._progress = -this._progress;
                this._goingForward = true;
            }
        }

        var position = this.spline.GetPoint(this._progress);
        this.transform.localPosition = position;
        if (this.lookForward)
        {
            this.transform.LookAt(position + this.spline.GetDirection(this._progress));
        }
    }
}