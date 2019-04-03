using UnityEngine;

public abstract class SpawnZone : PersistableObject
{
    [SerializeField]
    private SpawnConfiguration _spawnConfig;

    public abstract Vector3 SpawnPoint { get; }

    public virtual void ConifgureSpawn(Shape shape) {
        var t = shape.transform;
        t.localPosition = SpawnPoint;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * _spawnConfig.scale.RandomValueInRange;
        shape.SetColor(Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.25f, 1f, 1f, 1f));

        Vector3 direction;
        if (_spawnConfig.movementDirection == SpawnConfiguration.SpawnMovementDirection.Forward) {
            direction = transform.forward;
        } else if (_spawnConfig.movementDirection == SpawnConfiguration.SpawnMovementDirection.Outward) {
            direction = (t.localPosition - transform.localPosition).normalized;
        } else if (_spawnConfig.movementDirection == SpawnConfiguration.SpawnMovementDirection.Upward) {
            direction = transform.up;
        } else {
            direction = Random.onUnitSphere;
        }
        shape.Velocity = direction * _spawnConfig.speed.RandomValueInRange;
        shape.AngularVelocity = Random.onUnitSphere * _spawnConfig.angularSpeed.RandomValueInRange;
    }

    [System.Serializable]
    public struct SpawnConfiguration
    {
        public enum SpawnMovementDirection
        {
            Forward,
            Upward,
            Outward,
            Random
        }

        public SpawnMovementDirection movementDirection;
        public FloatRange speed;
        public FloatRange angularSpeed;
        public FloatRange scale;
    }
}
