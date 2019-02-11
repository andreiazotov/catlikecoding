using UnityEngine;

[CreateAssetMenu]
public class ShapeFactory : ScriptableObject
{
    [SerializeField]
    private Shape[] _prefabs;

    [SerializeField]
    private Material[] _materials;

    public Shape Get(int shapeId = 0, int materialId = 0) {
        var instance = Instantiate(_prefabs[shapeId]);
        instance.ShapeId = shapeId;
        instance.SetMaterial(_materials[materialId], materialId);
        return instance;
    }

    public Shape GetRandom() {
        return Get(Random.Range(0, _prefabs.Length), Random.Range(0, _materials.Length));
    }
}