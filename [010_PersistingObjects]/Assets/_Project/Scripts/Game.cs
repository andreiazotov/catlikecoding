using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Game : PersistableObject
{
    [SerializeField]
    private PersistableObject _prefab;

    [SerializeField]
    private PersistentStorage _storage;

    [SerializeField]
    private KeyCode _keyCreateObj = KeyCode.C;

    [SerializeField]
    private KeyCode _keyNewGame = KeyCode.N;

    [SerializeField]
    private KeyCode _keySave = KeyCode.S;

    [SerializeField]
    private KeyCode _keyLoad = KeyCode.L;

    private List<PersistableObject> _objects;

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
        else if (Input.GetKeyUp(_keyNewGame))
        {
            BeginNewGame();
        }
        else if (Input.GetKeyUp(_keySave))
        {
            _storage.Save(this);
        }
        else if (Input.GetKeyUp(_keyLoad))
        {
            BeginNewGame();
            _storage.Load(this);
        }
    }

    private void CreateObject()
    {
        var o = Instantiate(_prefab);
        Transform t = o.transform;
        t.localPosition = Random.insideUnitSphere * 5.0f;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(0.1f, 1.0f);
        _objects.Add(o);
    }

    private void BeginNewGame()
    {
        if (_objects != null && _objects.Count > 0)
        {
            for (int i = 0; i < _objects.Count; i++)
            {
                Destroy(_objects[i].gameObject);
            }
            _objects.Clear();
        }
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(_objects.Count);
        for (int i = 0; i < _objects.Count; i++)
        {
            _objects[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int count = reader.ReadInt();
        for (int i = 0; i < count; i++)
        {
            PersistableObject o = Instantiate(_prefab);
            o.Load(reader);
            _objects.Add(o);
        }
    }
}
