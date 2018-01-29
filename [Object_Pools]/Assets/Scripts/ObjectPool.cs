using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private PooledObject _prefab;
    private List<PooledObject> _availableObjects = new List<PooledObject>();

    public PooledObject GetObject()
    {
        PooledObject obj;
        int lastAvailableIndex = this._availableObjects.Count - 1;
        if (lastAvailableIndex >= 0)
        {
            obj = this._availableObjects[lastAvailableIndex];
            this._availableObjects.RemoveAt(lastAvailableIndex);
            obj.gameObject.SetActive(true);
        }
        else
        {
            obj = Instantiate(this._prefab);
            obj.transform.SetParent(transform, false);
            obj.pool = this;
        }
        return obj;
    }

    public void AddObject(PooledObject o)
    {
        o.gameObject.SetActive(false);
        this._availableObjects.Add(o);
    }

    public static ObjectPool GetPool(PooledObject prefab)
    {
        GameObject obj;
        ObjectPool pool;
        if (Application.isEditor)
        {
            obj = GameObject.Find(prefab.name + " Pool");
            if (obj)
            {
                pool = obj.GetComponent<ObjectPool>();
                if (pool)
                {
                    return pool;
                }
            }
        }
        obj = new GameObject(prefab.name + " Pool");
        pool = obj.AddComponent<ObjectPool>();
        pool._prefab = prefab;
        return pool;
    }
}
