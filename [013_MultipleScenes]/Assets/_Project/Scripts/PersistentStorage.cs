using System.IO;
using UnityEngine;

public class PersistentStorage : MonoBehaviour
{

    private string _savePath;

    void Awake()
    {
        _savePath = Path.Combine(Application.persistentDataPath, "saveFile");
    }

    public void Save(PersistableObject o, int version)
    {
        using var writer = new BinaryWriter(File.Open(_savePath, FileMode.Create));
        writer.Write(-version);
        o.Save(new GameDataWriter(writer));
    }

    public void Load(PersistableObject o)
    {
        using var reader = new BinaryReader(File.Open(_savePath, FileMode.Open));
        o.Load(new GameDataReader(reader, -reader.ReadInt32()));
    }
}