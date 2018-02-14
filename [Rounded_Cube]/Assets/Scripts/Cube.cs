using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Cube : MonoBehaviour
{
    public int xSize;
    public int ySize;
    public int zSize;

    private Vector3[] _vertices;
    private Mesh _mesh;

    private void Awake()
    {
        this.StartCoroutine(this.Generate());
    }

    private IEnumerator Generate()
    {
        this._mesh = new Mesh();
        this._mesh.name = "Procedural Cube";

        var wait = new WaitForSeconds(0.05f);

        int cornerVertices = 8;
        int edgeVertices = (this.xSize + this.ySize + this.zSize - 3) * 4;
        int faceVertices = ((this.xSize - 1) * (this.ySize - 1) + (this.xSize - 1) * (this.zSize - 1) + (this.ySize - 1) * (this.zSize - 1)) * 2;

        this._vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];

        int v = 0;
        for (int y = 0; y <= this.ySize; y++)
        {
            for (int x = 0; x <= this.xSize; x++)
            {
                this._vertices[v++] = new Vector3(x, y, 0);
                yield return wait;
            }
            for (int z = 1; z <= this.zSize; z++)
            {
                this._vertices[v++] = new Vector3(this.xSize, y, z);
                yield return wait;
            }
            for (int x = this.xSize - 1; x >= 0; x--)
            {
                this._vertices[v++] = new Vector3(x, y, this.zSize);
                yield return wait;
            }
            for (int z = this.zSize - 1; z > 0; z--)
            {
                this._vertices[v++] = new Vector3(0, y, z);
                yield return wait;
            }
        }

        for (int z = 1; z < this.zSize; z++)
        {
            for (int x = 1; x < this.xSize; x++)
            {
                this._vertices[v++] = new Vector3(x, this.ySize, z);
                yield return wait;
            }
        }
        for (int z = 1; z < this.zSize; z++)
        {
            for (int x = 1; x < this.xSize; x++)
            {
                this._vertices[v++] = new Vector3(x, 0, z);
                yield return wait;
            }
        }

        yield return wait;
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
