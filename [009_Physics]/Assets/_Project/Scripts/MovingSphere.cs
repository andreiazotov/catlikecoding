using UnityEngine;

public class MovingSphere : MonoBehaviour
{
    [SerializeField, Range(0.0f, 100.0f)]
    private float _maxSpeed = 10.0f;

    [SerializeField, Range(0.0f, 100.0f)]
    private float _maxAcceleration = 10.0f;

    [SerializeField, Range(0.0f, 100.0f)]
    private float _maxAirAcceleration = 1.0f;

    [SerializeField, Range(0.0f, 10.0f)]
    private float _jumpHeight = 2.0f;

    [SerializeField, Range(0, 5)]
    private int _maxAirJumps = 0;

    [SerializeField, Range(0.0f, 90.0f)]
    private float _maxGroundAngle = 25.0f;

    private int _groundContactCount;
    private int _jumpPhase;
    private bool _desiredJump;
    private float _minGroundDotProduct;
    private Rigidbody _body;
    private Vector3 _velocity;
    private Vector3 _desiredVelocity;
    private Vector3 _contactNormal;

    private bool _OnGround => _groundContactCount > 0;

    private void Awake()
    {
        _body = GetComponent<Rigidbody>();
        OnValidate();
    }

    void Update()
    {
        Vector2 input;
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");
        _desiredVelocity = new Vector3(input.x, 0.0f, input.y) * _maxSpeed;
        _desiredJump |= Input.GetButtonDown("Jump");
    }

    // The physics engine uses a fixed time step, regardless of the frame rate.
    // Although we already have given control over the sphere to PhysX we still
    // influence its velocity. For best results we should adjust velocity in
    // lockstep with the fixed time step. We do that by splitting our Update
    // method in two parts. The part where we check for input and set the desired
    // velocity can remain in Update, while the adjustment of the velocity should
    // move to a new FixedUpdate method. To make that work we have to store the
    // desired velocity in a field.
    // When FixedUpdate gets invoked Time.deltaTime is equal to Time.fixedDeltaTime.
    private void FixedUpdate()
    {
        UpdateState();
        AdjustVelocity();
        if (_desiredJump)
        {
            _desiredJump = false;
            Jump();
        }
        _body.linearVelocity = _velocity;
        ClearState();
    }

    private void ClearState()
    {
        _groundContactCount = 0;
        _contactNormal = Vector3.zero;
    }

    private void UpdateState()
    {
        _velocity = _body.linearVelocity;
        if (_OnGround)
        {
            _jumpPhase = 0;
            if (_groundContactCount > 1)
            {
                _contactNormal.Normalize();
            }
        }
        else
        {
            _contactNormal = Vector3.up;
        }
    }

    private void Jump()
    {
        if (_OnGround || _jumpPhase < _maxAirJumps)
        {
            _jumpPhase++;
            float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * _jumpHeight);
            float alignedSpeed = Vector3.Dot(_velocity, _contactNormal);
            if (alignedSpeed > 0f)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0.0f);
            }
            _velocity += _contactNormal * jumpSpeed;
        }
    }

    private Vector3 ProjectOnContactPlane(Vector3 vector)
    {
        return vector - _contactNormal * Vector3.Dot(vector, _contactNormal);
    }

    void AdjustVelocity()
    {
        Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
        Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;

        float currentX = Vector3.Dot(_velocity, xAxis);
        float currentZ = Vector3.Dot(_velocity, zAxis);

        float acceleration = _OnGround ? _maxAcceleration : _maxAirAcceleration;
        float maxSpeedChange = acceleration * Time.deltaTime;

        float newX = Mathf.MoveTowards(currentX, _desiredVelocity.x, maxSpeedChange);
        float newZ = Mathf.MoveTowards(currentZ, _desiredVelocity.z, maxSpeedChange);

        _velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
    }

    private void OnCollisionStay(Collision collision)
    {
        EvaluateCollision(collision);
    }

    private void OnCollisionEnter(Collision collision)
    {
        EvaluateCollision(collision);
    }

    private void EvaluateCollision(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            if (normal.y >= _minGroundDotProduct)
            {
                _groundContactCount++;
                _contactNormal += normal;
            }
        }
    }

    private void OnValidate()
    {
        _minGroundDotProduct = Mathf.Cos(_maxGroundAngle * Mathf.Deg2Rad);
    }
}
