using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu]
public class ShapeFactory : ScriptableObject
{
    [SerializeField]
    private Shape[] _prefabs;

    [SerializeField]
    private Material[] _materials;

    [SerializeField]
    private bool _recycle;

    private List<Shape>[] _pools;

    private void CreatePools()
    {
        _pools = new List<Shape>[_prefabs.Length];
        for (int i = 0; i < _pools.Length; i++)
        {
            _pools[i] = new List<Shape>();
        }
    }

    public Shape Get(int shapeId = 0, int materialId = 0)
    {
        Shape shape;
        if (_recycle)
        {
            if (_pools == null)
            {
                CreatePools();
            }
            var pool = _pools[shapeId];
            int lastIndex = pool.Count - 1;
            if (lastIndex >= 0)
            {
                shape = pool[lastIndex];
                shape.gameObject.SetActive(true);
                pool.RemoveAt(lastIndex);
            }
            else
            {
                shape = Instantiate(_prefabs[shapeId]);
                shape.ShapeId = shapeId;
            }
        }
        else
        {
            shape = Instantiate(_prefabs[shapeId]);
            shape.ShapeId = shapeId;
        }
        shape.SetMaterial(_materials[materialId], materialId);
        return shape;
    }

    public Shape GetRandom()
    {
        return Get(Random.Range(0, _prefabs.Length), Random.Range(0, _materials.Length));
    }

    public void Reclaim(Shape shapeToRecycle)
    {
        if (_recycle)
        {
            if (_pools == null)
            {
                CreatePools();
            }
            _pools[shapeToRecycle.ShapeId].Add(shapeToRecycle);
            shapeToRecycle.gameObject.SetActive(false);
        }
        else
        {
            Destroy(shapeToRecycle.gameObject);
        }
    }
}