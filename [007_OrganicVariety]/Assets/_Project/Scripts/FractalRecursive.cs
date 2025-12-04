// The recursive hierarchy of our fractal with all of its independently-moving
// parts is something that Unity struggles with. It has to update the parts in isolation,
// calculate their object-to-world conversion matrices, then cull them, and finally render
// them either with GPU instancing or with the SRP batcher. As we know exactly how our fractal
// works we could use a more efficient strategy than Unity's general-purpose approach.
// We might be able to improve performance by simplifying the hierarchy, getting rid of
// its recursive nature.

using UnityEngine;

public class FractalRecursive : MonoBehaviour
{
    [SerializeField, Range(1, 8)]
    private int _depth = 4;

    public int Depth { get { return _depth; } set { _depth = value; } }

    private void Start()
    {
        name = $"Fractal {Depth}";

        if (Depth <= 1)
        {
            return;
        }
        var A = CreateChild(Vector3.up,      Quaternion.identity);
        var B = CreateChild(Vector3.right,   Quaternion.Euler( 0.0f,  0.0f, -90.0f));
        var C = CreateChild(Vector3.left,    Quaternion.Euler( 0.0f,  0.0f,  90.0f));
        var D = CreateChild(Vector3.forward, Quaternion.Euler( 90.0f, 0.0f,  0.0f ));
        var E = CreateChild(Vector3.back,    Quaternion.Euler(-90.0f, 0.0f,  0.0f ));

        A.transform.SetParent(transform, false);
        B.transform.SetParent(transform, false);
        C.transform.SetParent(transform, false);
        D.transform.SetParent(transform, false);
        E.transform.SetParent(transform, false);
    }

    private FractalRecursive CreateChild(Vector3 direction, Quaternion rotation)
    {
        var child = Instantiate(this);
        child.Depth = Depth - 1;
        child.transform.SetLocalPositionAndRotation(direction * 0.75f, rotation);
        child.transform.localScale = Vector3.one * 0.5f;
        return child;
    }

    private void Update()
    {
        transform.Rotate(0.0f, 22.5f * Time.unscaledDeltaTime, 0.0f);
    }
}
