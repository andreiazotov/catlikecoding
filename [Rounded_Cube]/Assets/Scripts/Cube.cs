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
        this.Generate();
    }

    private void Generate()
    {
        this._mesh = new Mesh();
        this.GetComponent<MeshFilter>().mesh = this._mesh;
        this._mesh.name = "Procedural Cube";
        this.CreateVertices();
        this.CreateTriangles();
    }

    private void CreateVertices()
    {
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
            }
            for (int z = 1; z <= this.zSize; z++)
            {
                this._vertices[v++] = new Vector3(this.xSize, y, z);
            }
            for (int x = this.xSize - 1; x >= 0; x--)
            {
                this._vertices[v++] = new Vector3(x, y, this.zSize);
            }
            for (int z = this.zSize - 1; z > 0; z--)
            {
                this._vertices[v++] = new Vector3(0, y, z);
            }
        }

        for (int z = 1; z < this.zSize; z++)
        {
            for (int x = 1; x < this.xSize; x++)
            {
                this._vertices[v++] = new Vector3(x, this.ySize, z);
            }
        }
        for (int z = 1; z < this.zSize; z++)
        {
            for (int x = 1; x < this.xSize; x++)
            {
                this._vertices[v++] = new Vector3(x, 0, z);
            }
        }

        this._mesh.vertices = this._vertices;
    }
    private void CreateTriangles()
    {
        int quads = (this.xSize * this.ySize + this.xSize * this.zSize + this.ySize * this.zSize) * 2;
        var triangles = new int[quads * 6];
        int ring = (this.xSize + this.zSize) * 2;

        int v = 0;
        int t = 0;

        for (int y = 0; y < ySize; y++, v++)
        {
            for (int q = 0; q < ring - 1; q++, v++)
            {
                t = SetQuad(triangles, t, v, v + 1, v + ring, v + ring + 1);
            }
            t = SetQuad(triangles, t, v, v - ring + 1, v + ring, v + 1);
        }

        t = this.CreateTopFace(triangles, t, ring);
        t = this.CreateBottomFace(triangles, t, ring);
        this._mesh.triangles = triangles;
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

    private static int SetQuad(int[] triangles, int i, int v00, int v10, int v01, int v11)
    {
        triangles[i] = v00;
        triangles[i + 1] = v01;
        triangles[i + 2] = v10;
        triangles[i + 3] = v10;
        triangles[i + 4] = v01;
        triangles[i + 5] = v11;
        return i + 6;
    }

    private int CreateTopFace(int[] triangles, int t, int ring)
    {
        int v = ring * this.ySize;
        for (int x = 0; x < this.xSize - 1; x++, v++)
        {
            t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + ring);
        }
        t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + 2);

        int vMin = ring * (this.ySize + 1) - 1;
        int vMid = vMin + 1;
        int vMax = v + 2;

        for (int z = 1; z < this.zSize - 1; z++, vMin--, vMid++, vMax++)
        {
            t = SetQuad(triangles, t, vMin, vMid, vMin - 1, vMid + this.xSize - 1);
            for (int x = 1; x < this.xSize - 1; x++, vMid++)
            {
                t = SetQuad(triangles, t, vMid, vMid + 1, vMid + this.xSize - 1, vMid + this.xSize);
            }
            t = SetQuad(triangles, t, vMid, vMax, vMid + this.xSize - 1, vMax + 1);
        }

        int vTop = vMin - 2;
        t = SetQuad(triangles, t, vMin, vMid, vTop + 1, vTop);
        for (int x = 1; x < this.xSize - 1; x++, vTop--, vMid++)
        {
            t = SetQuad(triangles, t, vMid, vMid + 1, vTop, vTop - 1);
        }
        t = SetQuad(triangles, t, vMid, vTop - 2, vTop, vTop - 1);

        return t;
    }

    private int CreateBottomFace(int[] triangles, int t, int ring)
    {
        int v = 1;
        int vMid = this._vertices.Length - (this.xSize - 1) * (this.zSize - 1);
        t = SetQuad(triangles, t, ring - 1, vMid, 0, 1);
        for (int x = 1; x < this.xSize - 1; x++, v++, vMid++)
        {
            t = SetQuad(triangles, t, vMid, vMid + 1, v, v + 1);
        }
        t = SetQuad(triangles, t, vMid, v + 2, v, v + 1);

        int vMin = ring - 2;
        vMid -= this.xSize - 2;
        int vMax = v + 2;

        for (int z = 1; z < this.zSize - 1; z++, vMin--, vMid++, vMax++)
        {
            t = SetQuad(triangles, t, vMin, vMid + this.xSize - 1, vMin + 1, vMid);
            for (int x = 1; x < xSize - 1; x++, vMid++)
            {
                t = SetQuad(
                    triangles, t,
                    vMid + this.xSize - 1, vMid + this.xSize, vMid, vMid + 1);
            }
            t = SetQuad(triangles, t, vMid + this.xSize - 1, vMax + 1, vMid, vMax);
        }

        int vTop = vMin - 1;
        t = SetQuad(triangles, t, vTop + 1, vTop, vTop + 2, vMid);
        for (int x = 1; x < this.xSize - 1; x++, vTop--, vMid++)
        {
            t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vMid + 1);
        }
        t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vTop - 2);

        return t;
    }
}
