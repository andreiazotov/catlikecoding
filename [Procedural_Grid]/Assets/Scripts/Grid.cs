using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Grid : MonoBehaviour
{
    public int xSize;
    public int ySize;

    private Vector3[] _vertices;
    private Mesh _mesh;

    private void Awake()
    {
        this.Generate();
    }

    private void Generate()
    {
        this._vertices = new Vector3[(this.xSize + 1) * (this.ySize + 1)];
        var uv = new Vector2[this._vertices.Length];
        var tangents = new Vector4[this._vertices.Length];
        var tangent = new Vector4(1.0f, 0.0f, 0.0f, -1.0f);
        this._mesh = new Mesh();
        this.GetComponent<MeshFilter>().mesh = this._mesh;
        this._mesh.name = "Procedural Grid";
        int index = 0;
        for (int y = 0; y <= this.ySize; y++)
        {
            for (int x = 0; x <= this.xSize; x++)
            {
                this._vertices[index] = new Vector2(x, y);
                uv[index] = new Vector2((float)x / this.xSize, (float)y / this.ySize);
                tangents[index] = tangent;
                index++;
            }
        }
        this._mesh.vertices = this._vertices;
        this._mesh.uv = uv;
        this._mesh.tangents = tangents;

        int[] triangles = new int[this.xSize * this.ySize * 6];

        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + this.xSize + 1;
                triangles[ti + 5] = vi + this.xSize + 2;
            }
        }

        this._mesh.triangles = triangles;
        this._mesh.RecalculateNormals();
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
