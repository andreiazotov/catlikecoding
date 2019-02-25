using System.IO;
using UnityEngine;

public class PersistentStorage : MonoBehaviour
{
    private string _savePath;

    private void Awake() {
        _savePath = Path.Combine(Application.persistentDataPath, "saveFile");
    }

    public void Save(Shape o, int version) {
        using (var writer = new BinaryWriter(File.Open(_savePath, FileMode.Create))) {
            writer.Write(-version);
            o.Save(new GameDataWriter(writer));
        }
    }

    public void Load(Shape o) {
        using (var reader = new BinaryReader(File.Open(_savePath, FileMode.Open))) {
            o.Load(new GameDataReader(reader, -reader.ReadInt32()));
        }
    }
}
