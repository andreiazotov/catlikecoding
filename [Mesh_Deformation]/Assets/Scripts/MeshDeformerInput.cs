using UnityEngine;

public class MeshDeformerInput : MonoBehaviour
{
    public float force = 10.0f;
    public float forceOffset = 0.1f;

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            this.HandleInput();
        }
    }

    private void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        /*
         * Physics.Raycast is a static method for casting rays into the 3D scene.
         * It has various variants. The simplest version has a ray parameter and
         * returns whether it hit something. The versions that we are using has
         * an additional parameter. It is an output parameter of type RaycastHit.
         * This is a struct that contains information about what was hit and the
         * contact point.
         */
        if (Physics.Raycast(inputRay, out hit))
        {
            MeshDeformer deformer = hit.collider.GetComponent<MeshDeformer>();
            if (deformer)
            {
                Vector3 point = hit.point;
                deformer.AddDeformingForce(point, this.force);
            }
        }
    }
}
