using UnityEngine;

public class Shape : PersistableObject
{
    public Vector3 AngularVelocity { get; set; }
    public Vector3 Velocity { get; set; }

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

    public void GameUpdate()
    {
        // reorder operands for better performance
        // transform.Rotate(Vector3.forward * 50f * Time.deltaTime); -> transform.Rotate(50f * Time.deltaTime * Vector3.forward);
        transform.Rotate(AngularVelocity * Time.deltaTime);
        transform.localPosition += Velocity * Time.deltaTime;
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
        writer.Write(AngularVelocity);
        writer.Write(Velocity);
    }

    public override void Load(GameDataReader reader)
    {
        base.Load(reader);
        SetColor(reader.Version > 0 ? reader.ReadColor() : Color.white);
        AngularVelocity = reader.Version >= 4 ? reader.ReadVector3() : Vector3.zero;
        Velocity = reader.Version >= 4 ? reader.ReadVector3() : Vector3.zero;
    }
}