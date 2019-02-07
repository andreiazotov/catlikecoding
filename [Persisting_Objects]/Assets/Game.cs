using System.Collections.Generic;
using UnityEngine;

public class Game : PersistableObject
{
    public PersistableObject prefab;
    public PersistentStorage storage;
    public KeyCode createKey = KeyCode.C;
    public KeyCode newGameKey = KeyCode.N;
    public KeyCode saveKey = KeyCode.S;
    public KeyCode loadKey = KeyCode.L;

    private List<PersistableObject> _objects;

    private void Awake()
    {
        _objects = new List<PersistableObject>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(createKey))
        {
            CreateObject();
        }
        else if (Input.GetKeyDown(newGameKey))
        {
            BeginNewGame();
        }
        else if (Input.GetKeyDown(saveKey))
        {
            storage.Save(this);
        }
        else if (Input.GetKeyDown(loadKey))
        {
            BeginNewGame();
            storage.Load(this);
        }
    }

    private void CreateObject()
    {
        var t = Instantiate(prefab);
        t.transform.localPosition = Random.insideUnitSphere * 5.0f;
        t.transform.localRotation = Random.rotation;
        t.transform.localScale = Vector3.one * Random.Range(0.1f, 1.0f);
        _objects.Add(t);
    }

    private void BeginNewGame()
    {
        for (int i = 0; i < _objects.Count; i++)
        {
            Destroy(_objects[i].gameObject);
        }
        _objects.Clear();
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
            var o = Instantiate(prefab);
            o.Load(reader);
            _objects.Add(o);
        }
    }
}
