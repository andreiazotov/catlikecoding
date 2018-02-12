using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Grid : MonoBehaviour
{
    public int xSize;
    public int ySize;

    private Vector3[] _vertices;

    private void Awake()
    {
        this.StartCoroutine(this.Generate());
    }

    private IEnumerator Generate()
    {
        this._vertices = new Vector3[(this.xSize + 1) * (this.ySize + 1)];
        int index = 0;
        for (int y = 0; y <= this.ySize; y++)
        {
            for (int x = 0; x <= this.xSize; x++)
            {
                this._vertices[index++] = new Vector2(x, y);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (this._vertices == null)
        {
            return;
        }
        Gizmos.color = Color.black;
        foreach (var v in this._vertices)
        {
            Gizmos.DrawSphere(v, 0.1f);
        }
    }
}
