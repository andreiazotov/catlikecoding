using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Cube : MonoBehaviour
{
    public int xSize;
    public int ySize;
    public int zSize;
    public int roundness;

    private Vector3[] _vertices;
    private Mesh _mesh;
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
        this._mesh.name = "Procedural Cube";
        this.CreateVertices();
        this.CreateTriangles();
        this.CreateColliders();
    }

    private void CreateVertices()
    {
        int cornerVertices = 8;
        int edgeVertices = (this.xSize + this.ySize + this.zSize - 3) * 4;
        int faceVertices = ((this.xSize - 1) * (this.ySize - 1) + (this.xSize - 1) * (this.zSize - 1) + (this.ySize - 1) * (this.zSize - 1)) * 2;

        this._vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];
        this._normals = new Vector3[this._vertices.Length];
        this._cubeUV = new Color32[this._vertices.Length];

        int v = 0;
        for (int y = 0; y <= this.ySize; y++)
        {
            for (int x = 0; x <= this.xSize; x++)
            {
                this.SetVertex(v++, x, y, 0);
            }
            for (int z = 1; z <= this.zSize; z++)
            {
                this.SetVertex(v++, this.xSize, y, z);
            }
            for (int x = this.xSize - 1; x >= 0; x--)
            {
                this.SetVertex(v++, x, y, this.zSize);
            }
            for (int z = this.zSize - 1; z > 0; z--)
            {
                this.SetVertex(v++, 0, y, z);
            }
        }

        for (int z = 1; z < this.zSize; z++)
        {
            for (int x = 1; x < this.xSize; x++)
            {
                this.SetVertex(v++, x, this.ySize, z);
            }
        }
        for (int z = 1; z < this.zSize; z++)
        {
            for (int x = 1; x < this.xSize; x++)
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
        var inner = this._vertices[i] = new Vector3(x, y, z);

        if (x < this.roundness)
        {
            inner.x = roundness;
        }
        else if (x > this.xSize - this.roundness)
        {
            inner.x = this.xSize - roundness;
        }

        if (y < this.roundness)
        {
            inner.y = this.roundness;
        }
        else if (y > this.ySize - this.roundness)
        {
            inner.y = this.ySize - this.roundness;
        }

        if (z < this.roundness)
        {
            inner.z = this.roundness;
        }
        else if (z > this.zSize - this.roundness)
        {
            inner.z = this.zSize - this.roundness;
        }

        this._normals[i] = (this._vertices[i] - inner).normalized;
        this._vertices[i] = inner + this._normals[i] * this.roundness;
        this._cubeUV[i] = new Color32((byte)x, (byte)y, (byte)z, 0);
    }

    private void CreateTriangles()
    {
        var trianglesZ = new int[(xSize * ySize) * 12];
        var trianglesX = new int[(ySize * zSize) * 12];
        var trianglesY = new int[(xSize * zSize) * 12];

        int ring = (this.xSize + this.zSize) * 2;

        int tX = 0;
        int tY = 0;
        int tZ = 0;
        int v = 0;

        for (int y = 0; y < ySize; y++, v++)
        {
            for (int q = 0; q < xSize; q++, v++)
            {
                tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
            }
            for (int q = 0; q < zSize; q++, v++)
            {
                tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);
            }
            for (int q = 0; q < xSize; q++, v++)
            {
                tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
            }
            for (int q = 0; q < zSize - 1; q++, v++)
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

    private void CreateColliders()
    {
        this.AddBoxCollider(this.xSize, this.ySize - this.roundness * 2, this.zSize - this.roundness * 2);
        this.AddBoxCollider(this.xSize - this.roundness * 2, this.ySize, this.zSize - this.roundness * 2);
        this.AddBoxCollider(this.xSize - this.roundness * 2, this.ySize - this.roundness * 2, this.zSize);

        var min = Vector3.one * this.roundness;
        var half = new Vector3(this.xSize, this.ySize, this.zSize) * 0.5f;
        var max = new Vector3(this.xSize, this.ySize, this.zSize) - min;

        this.AddCapsuleCollider(0, half.x, min.y, min.z);
        this.AddCapsuleCollider(0, half.x, min.y, max.z);
        this.AddCapsuleCollider(0, half.x, max.y, min.z);
        this.AddCapsuleCollider(0, half.x, max.y, max.z);

        this.AddCapsuleCollider(1, min.x, half.y, min.z);
        this.AddCapsuleCollider(1, min.x, half.y, max.z);
        this.AddCapsuleCollider(1, max.x, half.y, min.z);
        this.AddCapsuleCollider(1, max.x, half.y, max.z);

        this.AddCapsuleCollider(2, min.x, min.y, half.z);
        this.AddCapsuleCollider(2, min.x, max.y, half.z);
        this.AddCapsuleCollider(2, max.x, min.y, half.z);
        this.AddCapsuleCollider(2, max.x, max.y, half.z);
    }

    private void AddBoxCollider(float x, float y, float z)
    {
        var col = this.gameObject.AddComponent<BoxCollider>();
        col.size = new Vector3(x, y, z);
    }

    private void AddCapsuleCollider(int direction, float x, float y, float z)
    {
        var col = gameObject.AddComponent<CapsuleCollider>();
        col.center = new Vector3(x, y, z);
        col.direction = direction;
        col.radius = this.roundness;
        col.height = col.center[direction] * 2f;
    }
}
