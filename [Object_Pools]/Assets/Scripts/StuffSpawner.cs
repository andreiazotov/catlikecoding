using UnityEngine;

[System.Serializable]
public struct FloatRange
{
    public float min;
    public float max;

    public float RandomInRange
    {
        get { return Random.Range(this.min, this.max); }
    }
}

public class StuffSpawner : MonoBehaviour
{
    public FloatRange timeBetweenSpawns;
    public FloatRange scale;
    public FloatRange randomVelocity;
    public FloatRange angularVelocity;
    public Stuff[] stuffPrefabs;
    public float velocity;
    public Material stuffMaterial;

    private float _currentSpawnDelay;
    private float _timeSinceLastSpawn;

    private void FixedUpdate()
    {
        this._timeSinceLastSpawn += Time.fixedDeltaTime;

        if (this._timeSinceLastSpawn >= this._currentSpawnDelay)
        {
            this._timeSinceLastSpawn -= this._currentSpawnDelay;
            this._currentSpawnDelay = this.timeBetweenSpawns.RandomInRange;
            this.SpawnStuff();
        }
    }

    private void SpawnStuff()
    {
        var obj = this.stuffPrefabs[Random.Range(0, this.stuffPrefabs.Length)];
        var spawn = obj.GetPooledInstance<Stuff>();
        spawn.transform.localPosition = this.transform.position;
        spawn.transform.localScale = Vector3.one * this.scale.RandomInRange;
        spawn.transform.localRotation = Random.rotation;
        spawn.body.velocity = transform.up * this.velocity + Random.onUnitSphere * this.randomVelocity.RandomInRange;
        spawn.body.angularVelocity = Random.onUnitSphere * this.angularVelocity.RandomInRange;
        spawn.SetMaterial(this.stuffMaterial);
    }
}
