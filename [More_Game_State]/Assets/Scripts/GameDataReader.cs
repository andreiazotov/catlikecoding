using System.IO;
using UnityEngine;

public class GameDataReader
{
    public int Version { get; }

    private BinaryReader _reader;

    public GameDataReader(BinaryReader reader, int version) {
        _reader = reader;
        Version = version;
    }

    public float ReadFloat() {
        return _reader.ReadSingle();
    }

    public int ReadInt() {
        return _reader.ReadInt32();
    }

    public Quaternion ReadQuaternion() {
        Quaternion value;
        value.x = _reader.ReadSingle();
        value.y = _reader.ReadSingle();
        value.z = _reader.ReadSingle();
        value.w = _reader.ReadSingle();
        return value;
    }

    public Vector3 ReadVector3() {
        Vector3 value;
        value.x = _reader.ReadSingle();
        value.y = _reader.ReadSingle();
        value.z = _reader.ReadSingle();
        return value;
    }

    public Color ReadColor() {
        Color color;
        color.r = _reader.ReadSingle();
        color.g = _reader.ReadSingle();
        color.b = _reader.ReadSingle();
        color.a = _reader.ReadSingle();
        return color;
    }

    public Random.State ReadRandomState() {
        return JsonUtility.FromJson<Random.State>(_reader.ReadString());
    }
}
