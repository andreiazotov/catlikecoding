using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Stuff : MonoBehaviour
{
    public Rigidbody body { get; private set; }

    private void Awake()
    {
        this.body = this.GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        /*
         * Comparing tags is quick and our kill zone
         * doesn't need to do anything, so it doesn't
         * need a component of its own.
        */
        if (other.CompareTag("KillZone"))
        {
            Destroy(this.gameObject);
        }
    }
}
