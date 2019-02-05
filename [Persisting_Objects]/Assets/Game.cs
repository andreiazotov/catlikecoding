using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public Transform prefab;
    public KeyCode createKey = KeyCode.C;
    public KeyCode newGameKey = KeyCode.N;
    public KeyCode saveKey = KeyCode.S;
    public KeyCode loadKey = KeyCode.L;

    private List<Transform> _objects;
    private string _savePath;

    private void Awake()
    {
        _objects = new List<Transform>();
        _savePath = Path.Combine(Application.persistentDataPath, "saveFile");
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
            Save();
        }
        else if (Input.GetKeyDown(loadKey))
        {
            Load();
        }
    }

    private void CreateObject()
    {
        var t = Instantiate(prefab);
        t.localPosition = Random.insideUnitSphere * 5.0f;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(0.1f, 1.0f);
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

    private void Save()
    {
        using (var writer = new BinaryWriter(File.Open(_savePath, FileMode.Create)))
        {
            writer.Write(_objects.Count);
            for (int i = 0; i < _objects.Count; i++)
            {
                var t = _objects[i];
                writer.Write(t.localPosition.x);
                writer.Write(t.localPosition.y);
                writer.Write(t.localPosition.z);
            }
        }
    }

    private void Load()
    {
        BeginNewGame();
        using (var reader = new BinaryReader(File.Open(_savePath, FileMode.Open)))
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var p = new Vector3();
                p.x = reader.ReadSingle();
                p.y = reader.ReadSingle();
                p.z = reader.ReadSingle();
                var t = Instantiate(prefab);
                t.localPosition = p;
                _objects.Add(t);
            }
        }
    }
}
