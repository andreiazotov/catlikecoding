using UnityEngine;

public class StuffSpawnerRing : MonoBehaviour
{
    public int numberOfSpawner;
    public float radius;
    public float tiltAngle;
    public StuffSpawner spawnerPrefab;
    public Material[] stuffMaterials;

    private void Awake()
    {
        for (int i = 0; i < this.numberOfSpawner; i++)
        {
            this.CreateSpawner(i);
        }
    }

    private void CreateSpawner(int index)
    {
        var rotater = new GameObject("Rotater").transform;
        rotater.SetParent(this.transform, false);
        rotater.localRotation = Quaternion.Euler(0f, index * 360f / this.numberOfSpawner, 0f);

        var spawner = Instantiate(this.spawnerPrefab);
        spawner.transform.SetParent(rotater, false);
        spawner.transform.localPosition = new Vector3(0.0f, 0.0f, this.radius);
        spawner.transform.localRotation = Quaternion.Euler(this.tiltAngle, 0.0f, 0.0f);
        spawner.stuffMaterial = this.stuffMaterials[index % this.stuffMaterials.Length];
    }
}
