using UnityEngine;

public class RotatingObject : GameLevelObject
{
    [SerializeField]
    private Vector3 _angularVelocity;

    public override void GameUpdate()
    {
        transform.Rotate(_angularVelocity * Time.deltaTime);
    } 
}