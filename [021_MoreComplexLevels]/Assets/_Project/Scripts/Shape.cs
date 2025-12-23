using System.Collections.Generic;
using UnityEngine;

public class Shape : PersistableObject
{
    [SerializeField]
    private MeshRenderer[] _meshRenderers;

    public int MaterialId { get; private set; }
    public float Age { get; private set; }
    public int InstanceId { get; private set; }
    public int SaveIndex { get; set; }

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

    public bool IsMarkedAsDying
    {
        get
        {
            return Game.Instance.IsMarkedAsDying(this);
        }
    }

    private int _shapeId;
    private Color[] _colors;
    private ShapeFactory _originFactory;
    private List<ShapeBehavior> _behaviors = new();

    private void Awake()
    {
        _colors = new Color[_meshRenderers.Length];
    }

    public T AddBehavior<T>() where T : ShapeBehavior, new()
    {
        T behavior = ShapeBehaviorPool<T>.Get();
        _behaviors.Add(behavior);
        return behavior;
    }

    public void Recycle()
    {
        Age = 0.0f;
        InstanceId += 1;
        for (int i = 0; i < _behaviors.Count; i++)
        {
            _behaviors[i].Recycle();
        }
        _behaviors.Clear();
        OriginFactory.Reclaim(this);
    }

    public void Die()
    {
        Game.Instance.Kill(this);
    }

    public void MarkAsDying()
    {
        Game.Instance.MarkAsDying(this);
    }

    public void GameUpdate()
    {
        // reorder operands for better performance
        // transform.Rotate(Vector3.forward * 50f * Time.deltaTime); -> transform.Rotate(50f * Time.deltaTime * Vector3.forward);
        Age += Time.deltaTime;
        for (int i = 0; i < _behaviors.Count; i++)
        {
            if (!_behaviors[i].GameUpdate(this))
            {
                _behaviors[i].Recycle();
                _behaviors.RemoveAt(i--);
            }
        }
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

    public void ResolveShapeInstances()
    {
        for (int i = 0; i < _behaviors.Count; i++)
        {
            _behaviors[i].ResolveShapeInstances();
        }
    }

    public override void Save(GameDataWriter writer)
    {
        base.Save(writer);
        writer.Write(_colors.Length);
        for (int i = 0; i < _colors.Length; i++)
        {
            writer.Write(_colors[i]);
        }
        writer.Write(Age);
        writer.Write(_behaviors.Count);
        for (int i = 0; i < _behaviors.Count; i++)
        {
            writer.Write((int)_behaviors[i].BehaviorType);
            _behaviors[i].Save(writer);
        }
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
        if (reader.Version >= 6)
        {
            Age = reader.ReadFloat();
            int behaviorCount = reader.ReadInt();
            for (int i = 0; i < behaviorCount; i++)
            {
                ShapeBehavior behavior = ((ShapeBehaviorType)reader.ReadInt()).GetInstance();
                _behaviors.Add(behavior);
                behavior.Load(reader);
            }
        }
        else if (reader.Version >= 4)
        {
            AddBehavior<ShapeBehaviorRotate>().AngularVelocity = reader.ReadVector3();
            AddBehavior<ShapeBehaviorMove>().Velocity = reader.ReadVector3();
        }
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

[System.Serializable]
public struct ShapeInstance
{
    public Shape Shape { get; private set; }
    private int _instanceIdOrSaveIndex;

    public ShapeInstance(int saveIndex)
    {
        Shape = null;
        _instanceIdOrSaveIndex = saveIndex;
    }

    public ShapeInstance(Shape shape)
    {
        Shape = shape;
        _instanceIdOrSaveIndex = shape.InstanceId;
    }

    public void Resolve()
    {
        if (_instanceIdOrSaveIndex >= 0)
        {
            Shape = Game.Instance.GetShape(_instanceIdOrSaveIndex);
            _instanceIdOrSaveIndex = Shape.InstanceId;
        }
    }

    public bool IsValid
    {
        get
        {
            return Shape && _instanceIdOrSaveIndex == Shape.InstanceId;
        }
    }

    public static implicit operator ShapeInstance(Shape shape)
    {
        return new ShapeInstance(shape);
    }
}