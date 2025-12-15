using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.ProBuilder;

public class Game : PersistableObject
{
    private const int SAVE_VERSION = 1;

    [SerializeField]
    private ShapeFactory _factory;

    [SerializeField]
    private PersistentStorage _storage;

    [SerializeField]
    private KeyCode _keyCreateObj = KeyCode.C;

    [SerializeField]
    private KeyCode _keyDestroObj = KeyCode.X;

    [SerializeField]
    private KeyCode _keyNewGame = KeyCode.N;

    [SerializeField]
    private KeyCode _keySave = KeyCode.S;

    [SerializeField]
    private KeyCode _keyLoad = KeyCode.L;

    private List<Shape> _objects;

    private void Awake()
    {
        _objects = new();
    }

    private void Update()
    {
        if (Input.GetKeyUp(_keyCreateObj))
        {
            CreateObject();
        }
        else if (Input.GetKeyUp(_keyDestroObj))
        {
            DestroyObject();
        }
        else if (Input.GetKeyUp(_keyNewGame))
        {
            BeginNewGame();
        }
        else if (Input.GetKeyUp(_keySave))
        {
            _storage.Save(this, SAVE_VERSION);
        }
        else if (Input.GetKeyUp(_keyLoad))
        {
            BeginNewGame();
            _storage.Load(this);
        }
    }

    private void CreateObject()
    {
        var o = _factory.GetRandom();
        Transform t = o.transform;
        t.SetLocalPositionAndRotation(Random.insideUnitSphere * 5.0f, Random.rotation);
        t.localScale = Vector3.one * Random.Range(0.1f, 1.0f);
        o.SetColor(Random.ColorHSV(0.0f, 1.0f, 0.5f, 1.0f, 0.25f, 1.0f, 1.0f, 1.0f));
        _objects.Add(o);
    }

    private void DestroyObject()
    {
        if (_objects.Count > 0)
        {
            int index = Random.Range(0, _objects.Count);
            _factory.Reclaim(_objects[index]);
            int lastIndex = _objects.Count - 1;
            _objects[index] = _objects[lastIndex];
            _objects.RemoveAt(lastIndex);
        }
    }

    private void BeginNewGame()
    {
        if (_objects != null && _objects.Count > 0)
        {
            for (int i = 0; i < _objects.Count; i++)
            {
                _factory.Reclaim(_objects[i]);
            }
            _objects.Clear();
        }
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(_objects.Count);
        for (int i = 0; i < _objects.Count; i++)
        {
            writer.Write(_objects[i].ShapeId);
            writer.Write(_objects[i].MaterialId);
            _objects[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int version = reader.Version;
        if (version > SAVE_VERSION)
        {
            Debug.LogError("Unsupported future save version " + version);
            return;
        }
        int count = version <= 0 ? -version : reader.ReadInt();
        for (int i = 0; i < count; i++)
        {
            int shapeId = version > 0 ? reader.ReadInt() : 0;
            int materialId = version > 0 ? reader.ReadInt() : 0;
            var o = _factory.Get(shapeId, materialId);
            o.Load(reader);
            _objects.Add(o);
        }
    }
}
