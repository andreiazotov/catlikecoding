using UnityEngine;

public class CompositeSpawnZone : SpawnZone
{
    [SerializeField]
    private SpawnZone[] _spawnZones;

    public override Vector3 SpawnPoint
    {
        get
        {
            int index = Random.Range(0, _spawnZones.Length);
            return _spawnZones[index].SpawnPoint;
        }
    }
}