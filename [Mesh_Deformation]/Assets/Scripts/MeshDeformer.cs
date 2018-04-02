using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshDeformer : MonoBehaviour
{
    private Mesh _deformingMesh;
    private Vector3[] _originalVertices;
    private Vector3[] _displacedVertices;
    private Vector3[] _vertexVelocities;
    private float _uniformScale = 1.0f;

    public float springForce = 20.0f;
    public float damping = 5.0f;

    private void Start()
    {
        this._deformingMesh = this.GetComponent<MeshFilter>().mesh;
        this._originalVertices = this._deformingMesh.vertices;
        this._displacedVertices = new Vector3[this._originalVertices.Length];
        this._vertexVelocities = new Vector3[this._originalVertices.Length];

        for (int i = 0; i < this._originalVertices.Length; i++)
        {
            this._displacedVertices[i] = this._originalVertices[i];
        }
    }

    private void Update()
    {
        this._uniformScale = this.transform.localScale.x;
        for (int i = 0; i < this._displacedVertices.Length; i++)
        {
            this.UpdateVertex(i);
        }
        this._deformingMesh.vertices = this._displacedVertices;
        this._deformingMesh.RecalculateNormals();
    }

    private void UpdateVertex(int i)
    {
        Vector3 velocity = this._vertexVelocities[i];
        Vector3 displacement = this._displacedVertices[i] - this._originalVertices[i];
        displacement *= this._uniformScale;
        velocity -= displacement * this.springForce * Time.deltaTime;
        velocity *= 1f - this.damping * Time.deltaTime;
        this._vertexVelocities[i] = velocity;
        this._displacedVertices[i] += velocity * (Time.deltaTime / this._uniformScale);
    }

    public void AddDeformingForce(Vector3 point, float force)
    {
        point = transform.InverseTransformPoint(point);
        for (int i = 0; i < this._displacedVertices.Length; i++)
        {
            this.AddForceToVertex(i, point, force);
        }
    }

    private void AddForceToVertex(int i, Vector3 point, float force)
    {
        Vector3 pointToVertex = this._displacedVertices[i] - point;
        pointToVertex *= this._uniformScale;
        float attenuatedForce = force / (1.0f + pointToVertex.sqrMagnitude);
        float velocity = attenuatedForce * Time.deltaTime;
        this._vertexVelocities[i] += pointToVertex.normalized * velocity;
    }
}
