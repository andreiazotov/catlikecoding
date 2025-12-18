using UnityEngine;

public class GameLevel : PersistableObject
{
    [SerializeField]
    private SpawnZone _spawnZone;

    [SerializeField]
    private PersistableObject[] _persistentObjects;

    public static GameLevel Current { get; private set; }

    public void ConfigureSpawn(Shape shape)
    {
        _spawnZone.ConfigureSpawn(shape);
    }

    private void OnEnable()
    {
        Current = this;
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(_persistentObjects.Length);
        for (int i = 0; i < _persistentObjects.Length; i++)
        {
            _persistentObjects[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int savedCount = reader.ReadInt();
        for (int i = 0; i < savedCount; i++)
        {
            _persistentObjects[i].Load(reader);
        }
    }
}