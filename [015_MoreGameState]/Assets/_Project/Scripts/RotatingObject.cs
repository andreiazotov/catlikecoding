using UnityEngine;

public class RotatingObject : PersistableObject
{
    [SerializeField]
    private Vector3 _angularVelocity;

    void Update()
    {
        transform.Rotate(_angularVelocity * Time.deltaTime);
    }
}