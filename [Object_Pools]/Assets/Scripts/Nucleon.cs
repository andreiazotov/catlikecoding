using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Nucleon : MonoBehaviour
{
    public float attractionForce;

    private Rigidbody _body;

    private void Awake()
    {
        this._body = this.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        this._body.AddForce(this.transform.localPosition * -this.attractionForce);
    }
}
