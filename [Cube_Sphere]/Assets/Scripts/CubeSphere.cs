using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CubeSphere : MonoBehaviour
{
    public int gridSize;
    public float radius = 1.0f;

    private Mesh _mesh;
    private Vector3[] _vertices;
    private Vector3[] _normals;
    private Color32[] _cubeUV;

    private void Awake()
    {
        this.Generate();
    }

    private void Generate()
    {
        this._mesh = new Mesh();
        this.GetComponent<MeshFilter>().mesh = this._mesh;
        this._mesh.name = "Procedural Sphere";
        this.CreateVertices();
        this.CreateTriangles();
        this.CreateColliders();
    }

    private void CreateVertices()
    {
        int cornerVertices = 8;
        int edgeVertices = (this.gridSize + this.gridSize + this.gridSize - 3) * 4;
        int faceVertices = ((this.gridSize - 1) * (this.gridSize - 1) + (this.gridSize - 1) * (this.gridSize - 1) + (this.gridSize - 1) * (this.gridSize - 1)) * 2;

        this._vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];
        this._normals = new Vector3[this._vertices.Length];
        this._cubeUV = new Color32[this._vertices.Length];

        int v = 0;
        for (int y = 0; y <= this.gridSize; y++)
        {
            for (int x = 0; x <= this.gridSize; x++)
            {
                this.SetVertex(v++, x, y, 0);
            }
            for (int z = 1; z <= this.gridSize; z++)
            {
                this.SetVertex(v++, this.gridSize, y, z);
            }
            for (int x = this.gridSize - 1; x >= 0; x--)
            {
                this.SetVertex(v++, x, y, this.gridSize);
            }
            for (int z = this.gridSize - 1; z > 0; z--)
            {
                this.SetVertex(v++, 0, y, z);
            }
        }

        for (int z = 1; z < this.gridSize; z++)
        {
            for (int x = 1; x < this.gridSize; x++)
            {
                this.SetVertex(v++, x, this.gridSize, z);
            }
        }
        for (int z = 1; z < this.gridSize; z++)
        {
            for (int x = 1; x < this.gridSize; x++)
            {
                this.SetVertex(v++, x, 0, z);
            }
        }

        this._mesh.vertices = this._vertices;
        this._mesh.normals = this._normals;
        this._mesh.colors32 = this._cubeUV;
    }

    private void SetVertex(int i, int x, int y, int z)
    {
        var v = new Vector3(x, y, z) * 2f / this.gridSize - Vector3.one;
        float x2 = v.x * v.x;
        float y2 = v.y * v.y;
        float z2 = v.z * v.z;
        Vector3 s;
        s.x = v.x * Mathf.Sqrt(1f - y2 / 2f - z2 / 2f + y2 * z2 / 3f);
        s.y = v.y * Mathf.Sqrt(1f - x2 / 2f - z2 / 2f + x2 * z2 / 3f);
        s.z = v.z * Mathf.Sqrt(1f - x2 / 2f - y2 / 2f + x2 * y2 / 3f);
        this._normals[i] = v.normalized;
        this._vertices[i] = this._normals[i] * this.radius;
        this._cubeUV[i] = new Color32((byte)x, (byte)y, (byte)z, 0);
    }

    private void CreateTriangles()
    {
        var trianglesZ = new int[(gridSize * gridSize) * 12];
        var trianglesX = new int[(gridSize * gridSize) * 12];
        var trianglesY = new int[(gridSize * gridSize) * 12];

        int ring = (this.gridSize + this.gridSize) * 2;

        int tX = 0;
        int tY = 0;
        int tZ = 0;
        int v = 0;

        for (int y = 0; y < gridSize; y++, v++)
        {
            for (int q = 0; q < gridSize; q++, v++)
            {
                tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
            }
            for (int q = 0; q < gridSize; q++, v++)
            {
                tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);
            }
            for (int q = 0; q < gridSize; q++, v++)
            {
                tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
            }
            for (int q = 0; q < gridSize - 1; q++, v++)
            {
                tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);
            }
            tX = SetQuad(trianglesX, tX, v, v - ring + 1, v + ring, v + 1);
        }

        tY = this.CreateTopFace(trianglesY, tY, ring);
        tY = this.CreateBottomFace(trianglesY, tY, ring);
        this._mesh.subMeshCount = 3;
        this._mesh.SetTriangles(trianglesX, 0);
        this._mesh.SetTriangles(trianglesY, 1);
        this._mesh.SetTriangles(trianglesZ, 2);
    }

    private void OnDrawGizmos()
    {
        if (this._vertices == null)
        {
            return;
        }

        for (int i = 0; i < this._vertices.Length; i++)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(this._vertices[i], 0.1f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(this._vertices[i], this._normals[i]);
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
        int v = ring * this.gridSize;
        for (int x = 0; x < this.gridSize - 1; x++, v++)
        {
            t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + ring);
        }
        t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + 2);

        int vMin = ring * (this.gridSize + 1) - 1;
        int vMid = vMin + 1;
        int vMax = v + 2;

        for (int z = 1; z < this.gridSize - 1; z++, vMin--, vMid++, vMax++)
        {
            t = SetQuad(triangles, t, vMin, vMid, vMin - 1, vMid + this.gridSize - 1);
            for (int x = 1; x < this.gridSize - 1; x++, vMid++)
            {
                t = SetQuad(triangles, t, vMid, vMid + 1, vMid + this.gridSize - 1, vMid + this.gridSize);
            }
            t = SetQuad(triangles, t, vMid, vMax, vMid + this.gridSize - 1, vMax + 1);
        }

        int vTop = vMin - 2;
        t = SetQuad(triangles, t, vMin, vMid, vTop + 1, vTop);
        for (int x = 1; x < this.gridSize - 1; x++, vTop--, vMid++)
        {
            t = SetQuad(triangles, t, vMid, vMid + 1, vTop, vTop - 1);
        }
        t = SetQuad(triangles, t, vMid, vTop - 2, vTop, vTop - 1);

        return t;
    }

    private int CreateBottomFace(int[] triangles, int t, int ring)
    {
        int v = 1;
        int vMid = this._vertices.Length - (this.gridSize - 1) * (this.gridSize - 1);
        t = SetQuad(triangles, t, ring - 1, vMid, 0, 1);
        for (int x = 1; x < this.gridSize - 1; x++, v++, vMid++)
        {
            t = SetQuad(triangles, t, vMid, vMid + 1, v, v + 1);
        }
        t = SetQuad(triangles, t, vMid, v + 2, v, v + 1);

        int vMin = ring - 2;
        vMid -= this.gridSize - 2;
        int vMax = v + 2;

        for (int z = 1; z < this.gridSize - 1; z++, vMin--, vMid++, vMax++)
        {
            t = SetQuad(triangles, t, vMin, vMid + this.gridSize - 1, vMin + 1, vMid);
            for (int x = 1; x < gridSize - 1; x++, vMid++)
            {
                t = SetQuad(
                    triangles, t,
                    vMid + this.gridSize - 1, vMid + this.gridSize, vMid, vMid + 1);
            }
            t = SetQuad(triangles, t, vMid + this.gridSize - 1, vMax + 1, vMid, vMax);
        }

        int vTop = vMin - 1;
        t = SetQuad(triangles, t, vTop + 1, vTop, vTop + 2, vMid);
        for (int x = 1; x < this.gridSize - 1; x++, vTop--, vMid++)
        {
            t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vMid + 1);
        }
        t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vTop - 2);

        return t;
    }

    private void CreateColliders()
    {
        this.gameObject.AddComponent<SphereCollider>();
    }
}
