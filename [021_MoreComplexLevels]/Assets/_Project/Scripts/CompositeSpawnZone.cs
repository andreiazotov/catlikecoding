using UnityEngine;

public class CompositeSpawnZone : SpawnZone
{
    [SerializeField]
    private bool _overrideConfig;

    [SerializeField]
    private bool _sequential;

    [SerializeField]
    private SpawnZone[] _spawnZones;

    private int _nextSequentialIndex;

    public override Vector3 SpawnPoint
    {
        get
        {
            int index;
            if (_sequential )
            {
                index = _nextSequentialIndex++;
                if (_nextSequentialIndex >= _spawnZones.Length)
                {
                    _nextSequentialIndex = 0;
                }
            }
            else
            {
                index = Random.Range(0, _spawnZones.Length);
            }
            return _spawnZones[index].SpawnPoint;
        }
    }

    public override void SpawnShape()
    {
        if (_overrideConfig)
        {
            base.SpawnShape();
        }
        else
        {
            int index;
            if (_sequential)
            {
                index = _nextSequentialIndex++;
                if (_nextSequentialIndex >= _spawnZones.Length)
                {
                    _nextSequentialIndex = 0;
                }
            }
            else
            {
                index = Random.Range(0, _spawnZones.Length);
            }
            _spawnZones[index].SpawnShape();
        }
    }

    public override void Save(GameDataWriter writer)
    {
        base.Save(writer);
        writer.Write(_nextSequentialIndex);
    }

    public override void Load(GameDataReader reader)
    {
        if (reader.Version >= 7)
        {
            base.Load(reader);
        }
        _nextSequentialIndex = reader.ReadInt();
    }
}