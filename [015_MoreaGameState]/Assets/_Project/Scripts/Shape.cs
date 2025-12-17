using UnityEngine;

public class Shape : PersistableObject
{
    public int MaterialId { get; private set; }

    public int ShapeId
    {
        get
        {
            return _shapeId;
        }
        set
        {
            _shapeId = value;
        }
    }

    private int _shapeId;
    private Color _color;
    private MeshRenderer _meshRenderer;

    void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetMaterial(Material material, int materialId)
    {
        _meshRenderer.material = material;
        MaterialId = materialId;
    }

    public void SetColor(Color color)
    {
        _color = color;
        _meshRenderer.material.color = _color;
    }

    public override void Save(GameDataWriter writer)
    {
        base.Save(writer);
        writer.Write(_color);
    }

    public override void Load(GameDataReader reader)
    {
        base.Load(reader);
        SetColor(reader.Version > 0 ? reader.ReadColor() : Color.white);
    }
}