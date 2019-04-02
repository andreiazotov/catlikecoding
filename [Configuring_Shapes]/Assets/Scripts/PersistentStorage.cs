using System.IO;
using UnityEngine;

public class PersistentStorage : MonoBehaviour
{
    private string _savePath;

    private void Awake() {
        _savePath = Path.Combine(Application.persistentDataPath, "saveFile");
    }

    public void Save(PersistableObject o, int version) {
        using (var writer = new BinaryWriter(File.Open(_savePath, FileMode.Create))) {
            writer.Write(-version);
            o.Save(new GameDataWriter(writer));
        }
    }

    public void Load(PersistableObject o) {
        var data = File.ReadAllBytes(_savePath);
        var reader = new BinaryReader(new MemoryStream(data));
        o.Load(new GameDataReader(reader, -reader.ReadInt32()));
    }
}
