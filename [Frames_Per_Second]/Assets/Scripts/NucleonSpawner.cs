using UnityEngine;

public class NucleonSpawner : MonoBehaviour
{
    public float timeBetweenSpawns;
    public float spawnDistance;
    public Nucleon[] nucleonPrefabs;

    private float _timeSinceLastSpawn;

    private void FixedUpdate()
    {
        this._timeSinceLastSpawn += Time.fixedDeltaTime;

        if (this._timeSinceLastSpawn >= this.timeBetweenSpawns)
        {
            this._timeSinceLastSpawn -= this.timeBetweenSpawns;
            this.SpawnNucleon();
        }
    }

    private void SpawnNucleon()
    {
        var obj = this.nucleonPrefabs[Random.Range(0, this.nucleonPrefabs.Length)];
        var spawn = Instantiate(obj);
        spawn.transform.localPosition = Random.onUnitSphere * this.spawnDistance;
    }
}
