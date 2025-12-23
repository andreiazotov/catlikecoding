using UnityEngine;

public class GameLevel : PersistableObject
{
    [SerializeField]
    private int _populationLimit;

    [SerializeField]
    private SpawnZone _spawnZone;

    [UnityEngine.Serialization.FormerlySerializedAs("persistentObjects")]
    [SerializeField]
    private GameLevelObject[] _levelObjects;

    public static GameLevel Current { get; private set; }

    public int PopulationLimit => _populationLimit;

    public void SpawnShape()
    {
        _spawnZone.SpawnShape();
    }

    private void OnEnable()
    {
        Current = this;
        _levelObjects ??= new GameLevelObject[0];
    }

    public void GameUpdate()
    {
        for (int i = 0; i < _levelObjects.Length; i++)
        {
            _levelObjects[i].GameUpdate();
        }
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(_levelObjects.Length);
        for (int i = 0; i < _levelObjects.Length; i++)
        {
            _levelObjects[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int savedCount = reader.ReadInt();
        for (int i = 0; i < savedCount; i++)
        {
            _levelObjects[i].Load(reader);
        }
    }
}