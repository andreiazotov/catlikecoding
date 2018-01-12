using UnityEngine;

public class Graph : MonoBehaviour
{
    public Transform pointPrefab;

    [Range(10, 100)]
    public int resolution = 10;

    private Transform[] _points;

    private void Awake()
    {
        float step = 2.0f / this.resolution;
        var scale = Vector3.one * step;
        Vector3 position;
        position.y = 0.0f;
        position.z = 0.0f;

        this._points = new Transform[this.resolution];
        for (int i = 0; i < this.resolution; i++)
        {
            var point = Instantiate(this.pointPrefab);
            position.x = ((i + 0.5f) * step - 1.0f);
            point.localPosition = position;
            point.localScale = scale;
            point.SetParent(this.transform, false);

            this._points[i] = point;
        }
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < this._points.Length; i++)
        {
            var point = this._points[i];
            var position = point.localPosition;
            position.y = Mathf.Sin(Mathf.PI * (position.x + Time.time));
            point.localPosition = position;
        }
    }
}
