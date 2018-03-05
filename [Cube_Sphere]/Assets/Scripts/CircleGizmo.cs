using UnityEngine;

public class CircleGizmo : MonoBehaviour
{
    public int resolution = 10;

    private void OnDrawGizmosSelected()
    {
        float step = 2.0f / this.resolution;
        for (int i = 0; i < this.resolution + 1; i++)
        {
            this.ShowPoint(i * step - 1.0f, -1.0f);
            this.ShowPoint(i * step - 1.0f, 1.0f);
        }

        for (int i = 0; i < this.resolution + 1; i++)
        {
            this.ShowPoint(-1.0f, i * step - 1.0f);
            this.ShowPoint(1.0f, i * step - 1.0f);
        }
    }

    private void ShowPoint(float x, float y)
    {
        var square = new Vector2(x, y);
        var circle = new Vector2(square.x * Mathf.Sqrt(1f - square.y * square.y * 0.5f), square.y * Mathf.Sqrt(1f - square.x * square.x * 0.5f));

        Gizmos.color = Color.black;
        Gizmos.DrawSphere(square, 0.025f);

        Gizmos.color = Color.white;
        Gizmos.DrawSphere(circle, 0.025f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(square, circle);

        Gizmos.color = Color.gray;
        Gizmos.DrawLine(circle, Vector2.zero);
    }
}
