using UnityEngine;

public class PooledObject : MonoBehaviour
{
    public ObjectPool pool { get; set; }

    [System.NonSerialized]
    private ObjectPool _poolInstanceForPrefab;

    public void ReturnToPool()
    {
        if (this.pool)
        {
            this.pool.AddObject(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public T GetPooledInstance<T>() where T : PooledObject
    {
        if (!this._poolInstanceForPrefab)
        {
            this._poolInstanceForPrefab = ObjectPool.GetPool(this);
        }
        return (T)this._poolInstanceForPrefab.GetObject();
    }
}
