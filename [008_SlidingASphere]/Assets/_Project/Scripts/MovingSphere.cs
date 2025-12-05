using UnityEngine;

public class MovingSphere : MonoBehaviour
{
    [SerializeField, Range(0.0f, 100.0f)]
    private float _maxSpeed = 10.0f;

    [SerializeField, Range(0.0f, 100.0f)]
    private float _maxAcceleration = 10.0f;

    [SerializeField]
    private Rect _allowedArea = new(-5.0f, -5.0f, 10.0f, 10.0f);

    [SerializeField, Range(0.0f, 1.0f)]
    private float bounciness = 0.5f;

    private Vector3 _velocity;

    void Update()
    {
        Vector2 input;
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");
        var desiredVelocity = new Vector3(input.x, 0.0f, input.y) * _maxSpeed;
        var maxSpeedChange = _maxAcceleration * Time.deltaTime;
        _velocity.x = Mathf.MoveTowards(_velocity.x, desiredVelocity.x, maxSpeedChange);
        _velocity.z = Mathf.MoveTowards(_velocity.z, desiredVelocity.z, maxSpeedChange);
        var displacement = _velocity * Time.deltaTime;
        Vector3 newPosition = transform.localPosition + displacement;

        if (newPosition.x < _allowedArea.xMin)
        {
            newPosition.x = _allowedArea.xMin;
            _velocity.x = -_velocity.x * bounciness;
        }
        else if (newPosition.x > _allowedArea.xMax)
        {
            newPosition.x = _allowedArea.xMax;
            _velocity.x = -_velocity.x * bounciness;
        }

        if (newPosition.z < _allowedArea.yMin)
        {
            newPosition.z = _allowedArea.yMin;
            _velocity.z = -_velocity.z * bounciness;
        }
        else if (newPosition.z > _allowedArea.yMax)
        {
            newPosition.z = _allowedArea.yMax;
            _velocity.z = -_velocity.z * bounciness;
        }

        transform.localPosition = newPosition;
    }
}
