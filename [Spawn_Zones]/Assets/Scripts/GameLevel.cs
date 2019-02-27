using UnityEngine;

public class GameLevel : MonoBehaviour
{
    [SerializeField]
    private SpawnZone _spawnZone;

    void Start() {
        Game.Instance.SpawnZoneOfLevel = _spawnZone;
    }
}