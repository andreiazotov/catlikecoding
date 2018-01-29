using UnityEngine;

public class StuffSpawner : MonoBehaviour
{
    public float timeBetweenSpawns;
    public Stuff[] stuffPrefabs;
    public float velocity;

    private float _timeSinceLastSpawn;

    private void FixedUpdate()
    {
        this._timeSinceLastSpawn += Time.fixedDeltaTime;

        if (this._timeSinceLastSpawn >= this.timeBetweenSpawns)
        {
            this._timeSinceLastSpawn -= this.timeBetweenSpawns;
            this.SpawnStuff();
        }
    }

    private void SpawnStuff()
    {
        var obj = this.stuffPrefabs[Random.Range(0, this.stuffPrefabs.Length)];
        var spawn = Instantiate(obj);
        spawn.transform.localPosition = this.transform.position;
        spawn.body.velocity = transform.up * this.velocity;
    }
}
