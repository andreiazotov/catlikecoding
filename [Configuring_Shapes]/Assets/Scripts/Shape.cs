using UnityEngine;

public class Shape : PersistableObject
{
    private static int s_coorPropertyId = Shader.PropertyToID("_Color");
    private static MaterialPropertyBlock s_sharedPropertyBlock;

    public int ShapeId {
        get {
            return _shapeId;
        }
        set {
            if (_shapeId == int.MinValue && value != int.MinValue) {
                _shapeId = value;
            } else {
                Debug.LogError("Cannot assign value!");
            }
        }
    }

    public int MaterialId { get; private set; }
    public Vector3 AngularVelocity { get; set; }
    public Vector3 Velocity { get; set; }

    private int _shapeId = int.MinValue;
    private Color _color;
    private MeshRenderer _meshRenderer;

    private void Awake() {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetMaterial(Material material, int materialId) {
        _meshRenderer.material = material;
        MaterialId = materialId;
    }

    public void SetColor(Color color) {
        _color = color;
        if (s_sharedPropertyBlock == null) {
            s_sharedPropertyBlock = new MaterialPropertyBlock();
        }
        s_sharedPropertyBlock.SetColor(s_coorPropertyId, color);
        _meshRenderer.SetPropertyBlock(s_sharedPropertyBlock);
    }

    public override void Save(GameDataWriter writer) {
        base.Save(writer);
        writer.Write(_color);
        writer.Write(AngularVelocity);
        writer.Write(Velocity);
    }

    public override void Load(GameDataReader reader) {
        base.Load(reader);
        SetColor(reader.Version > 0 ? reader.ReadColor() : Color.white);
        AngularVelocity = reader.Version >= 4 ? reader.ReadVector3() : Vector3.zero;
        Velocity = reader.Version >= 4 ? reader.ReadVector3() : Vector3.zero;
    }

    public void GameUpdate() {
        transform.Rotate(AngularVelocity * Time.deltaTime);
        transform.localPosition += Velocity * Time.deltaTime;
    }
}
