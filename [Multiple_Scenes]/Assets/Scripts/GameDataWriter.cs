using System.IO;
using UnityEngine;

public class GameDataWriter
{
    private BinaryWriter _writer;

    public GameDataWriter(BinaryWriter writer) {
        _writer = writer;
    }

    public void Write(float value) {
        _writer.Write(value);
    }

    public void Write(int value) {
        _writer.Write(value);
    }

    public void Write(Quaternion value) {
        _writer.Write(value.x);
        _writer.Write(value.y);
        _writer.Write(value.z);
        _writer.Write(value.w);
    }

    public void Write(Vector3 value) {
        _writer.Write(value.x);
        _writer.Write(value.y);
        _writer.Write(value.z);
    }

    public void Write(Color color) {
        _writer.Write(color.r);
        _writer.Write(color.g);
        _writer.Write(color.b);
        _writer.Write(color.a);
    }
}
