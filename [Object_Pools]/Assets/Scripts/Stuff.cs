using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Stuff : PooledObject
{
    public Rigidbody Body { get; private set; }
    private MeshRenderer[] _meshRenderers;

    private void Awake()
    {
        this.Body = this.GetComponent<Rigidbody>();
        this._meshRenderers = this.GetComponentsInChildren<MeshRenderer>();
    }

    public void SetMaterial(Material m)
    {
        for (int i = 0; i < this._meshRenderers.Length; i++)
        {
            this._meshRenderers[i].material = m;
        }
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
            this.ReturnToPool();
        }
    }
}
