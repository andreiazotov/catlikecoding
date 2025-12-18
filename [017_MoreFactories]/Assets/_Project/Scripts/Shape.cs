using UnityEngine;

public class Shape : PersistableObject
{
    [SerializeField]
    private MeshRenderer[] _meshRenderers;

    public Vector3 AngularVelocity { get; set; }
    public Vector3 Velocity { get; set; }

    public int MaterialId { get; private set; }

    public ShapeFactory OriginFactory
    {
        get
        {
            return _originFactory;
        }
        set
        {
            if (_originFactory == null)
            {
                _originFactory = value;
            }
            else
            {
                Debug.LogError("Not allowed to change origin factory.");
            }
        }
    }

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

    public int ColorCount
    {
        get
        {
            return _colors.Length;
        }
    }

    private int _shapeId;
    private Color[] _colors;
    private ShapeFactory _originFactory;

    private void Awake()
    {
        _colors = new Color[_meshRenderers.Length];
    }

    public void Recycle()
    {
        OriginFactory.Reclaim(this);
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
        for (int i = 0; i < _meshRenderers.Length; i++)
        {
            _meshRenderers[i].material = material;
        }
        MaterialId = materialId;
    }

    public void SetColor(Color color)
    {
        for (int i = 0; i < _meshRenderers.Length; i++)
        {
            _colors[i] = color;
            _meshRenderers[i].material.color = color;
        }
    }

    public void SetColor(Color color, int index)
    {
        _colors[index] = color;
        _meshRenderers[index].material.color = color;
    }

    public override void Save(GameDataWriter writer)
    {
        base.Save(writer);
        writer.Write(_colors.Length);
        for (int i = 0; i < _colors.Length; i++)
        {
            writer.Write(_colors[i]);
        }
        writer.Write(AngularVelocity);
        writer.Write(Velocity);
    }

    public override void Load(GameDataReader reader)
    {
        base.Load(reader);
        if (reader.Version >= 5)
        {
            LoadColors(reader);
        }
        else
        {
            SetColor(reader.Version > 0 ? reader.ReadColor() : Color.white);
        }
        AngularVelocity = reader.Version >= 4 ? reader.ReadVector3() : Vector3.zero;
        Velocity = reader.Version >= 4 ? reader.ReadVector3() : Vector3.zero;
    }

    private void LoadColors(GameDataReader reader)
    {
        int count = reader.ReadInt();
        int max = count <= _colors.Length ? count : _colors.Length;
        int i = 0;
        for (; i < max; i++)
        {
            SetColor(reader.ReadColor(), i);
        }
        if (count > _colors.Length)
        {
            for (; i < count; i++)
            {
                reader.ReadColor();
            }
        }
        else if (count < _colors.Length)
        {
            for (; i < _colors.Length; i++)
            {
                SetColor(Color.white, i);
            }
        }
    }
}