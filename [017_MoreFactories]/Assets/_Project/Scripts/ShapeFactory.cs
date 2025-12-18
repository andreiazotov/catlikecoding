using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class ShapeFactory : ScriptableObject
{
    [SerializeField]
    private Shape[] _prefabs;

    [SerializeField]
    private Material[] _materials;

    [SerializeField]
    private bool _recycle;

    public int FactoryId
    {
        get
        {
            return _factoryId;
        }
        set
        {
            if (_factoryId == int.MinValue && value != int.MinValue)
            {
                _factoryId = value;
            }
            else
            {
                Debug.Log("Not allowed to change factoryId.");
            }
        }
    }

    [System.NonSerialized]
    private int _factoryId = int.MinValue;

    private List<Shape>[] _pools;

    private Scene _poolScene;

    private void CreatePools()
    {
        _pools = new List<Shape>[_prefabs.Length];
        for (int i = 0; i < _pools.Length; i++)
        {
            _pools[i] = new List<Shape>();
        }

        if (Application.isEditor)
        {
            _poolScene = SceneManager.GetSceneByName(name);
            if (_poolScene.isLoaded)
            {
                var rootObjects = _poolScene.GetRootGameObjects();
                for (int i = 0; i < rootObjects.Length; i++)
                {
                    Shape pooledShape = rootObjects[i].GetComponent<Shape>();
                    if (!pooledShape.gameObject.activeSelf)
                    {
                        _pools[pooledShape.ShapeId].Add(pooledShape);
                    }
                }
                return;
            }
        }
        _poolScene = SceneManager.CreateScene(name);
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
                shape.OriginFactory = this;
                shape.ShapeId = shapeId;
                SceneManager.MoveGameObjectToScene(shape.gameObject, _poolScene);
            }
        }
        else
        {
            shape = Instantiate(_prefabs[shapeId]);
            shape.OriginFactory = this;
            shape.ShapeId = shapeId;
            SceneManager.MoveGameObjectToScene(shape.gameObject, _poolScene);
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
        if (shapeToRecycle.OriginFactory != this)
        {
            Debug.LogError("Tried to reclaim shape with wrong factory.");
            return;
        }

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