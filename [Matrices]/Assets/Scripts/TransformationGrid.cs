using UnityEngine;

public class TransformationGrid : MonoBehaviour
{
    public Transform prefab;
    public int gridResolution = 10;

    private Transform[] _grid;

    private void Awake()
    {
        this._grid = new Transform[this.gridResolution * this.gridResolution * this.gridResolution];

        for (int i = 0, z = 0; z < this.gridResolution; z++)
        {
            for (int y = 0; y < this.gridResolution; y++)
            {
                for (int x = 0; x < this.gridResolution; x++, i++)
                {
                    this._grid[i] = this.CreateGridPoint(x, y, z);
                }
            }
        }
    }

    private Transform CreateGridPoint(int x, int y, int z)
    {
        var point = Instantiate(this.prefab);
        point.localPosition = this.GetCoordinates(x, y, z);
        point.GetComponent<MeshRenderer>().material.color = new Color(
            (float)x / this.gridResolution,
            (float)y / this.gridResolution,
            (float)z / this.gridResolution
        );
        return point;
    }

    private Vector3 GetCoordinates(int x, int y, int z)
    {
        return new Vector3(
            x - (this.gridResolution - 1) * 0.5f,
            y - (this.gridResolution - 1) * 0.5f,
            z - (this.gridResolution - 1) * 0.5f
        );
    }
}
