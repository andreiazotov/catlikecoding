using UnityEngine;

public class Graph : MonoBehaviour
{
    public Transform pointPrefab;

    [Range(10, 100)]
    public int resolution = 10;

    /*
     * This makes it possible to control the function via the graph's
     * inspector, also while we're in play mode.
    */
    public GraphFunctionName function;

    private Transform[] _points;
    private const float pi = Mathf.PI;

    private static GraphFunction[] s_functions = {
        SineFunction,
        MultiSineFunction,
        Sine2DFunction,
        MultiSine2DFunction,
        Ripple,
        Cylinder,
        Sphere,
        Torus
    };

    private void Awake()
    {
        /* It is possible change the code and the function will change
         * along with it, even while the Unity editor
         * is in play mode. Execution will be paused, the current game
         * state saved, then the scripts are compiled again, and finally
         * the game state is reloaded and play resumes. Not everything
         * survives a recompile while in play mode, but graph does.
         * It will switch to animating the new function, without being
         * aware that something changed.
        */

        float step = 2.0f / this.resolution;
        var scale = Vector3.one * step;
        Vector3 position;
        position.y = 0.0f;
        position.z = 0.0f;

        this._points = new Transform[this.resolution * this.resolution];
        for (int i = 0; i < this._points.Length; i++)
        {
            var point = Instantiate(this.pointPrefab);
            point.localScale = scale;
            point.SetParent(this.transform, false);
            this._points[i] = point;
        }
    }

    private void Update()
    {
        float t = Time.time;
        var f = s_functions[(int)this.function];

        float step = 2.0f / this.resolution;
        for (int i = 0, z = 0; z < this.resolution; z++)
        {
            float v = (z + 0.5f) * step - 1.0f;
            for (int x = 0; x < this.resolution; x++, i++)
            {
                float u = (x + 0.5f) * step - 1f;
                this._points[i].localPosition = f(u, v, t);
            }
        }
    }

    /*
     * Because static methods aren't associated with object instances,
     * the compiled code doesn't have to keep track of which object
     * invoking the method on. This means that static method invocations
     * are a bit faster, but it's usually not significant enough.
    */
    private static Vector3 SineFunction(float x, float z, float t)
    {
        Vector3 p;
        p.x = x;
        p.y = Mathf.Sin(pi * (x + t));
        p.z = z;
        return p;
    }

    private static Vector3 MultiSineFunction(float x, float z, float t)
    {
        Vector3 p;
        p.x = x;
        p.y = Mathf.Sin(pi * (x + t));
        p.y += Mathf.Sin(2.0f * pi * (x + 2.0f * t)) / 2.0f;
        p.y *= 2.0f / 3.0f;
        p.z = z;
        return p;
    }

    private static Vector3 Sine2DFunction(float x, float z, float t)
    {
        Vector3 p;
        p.x = x;
        p.y = Mathf.Sin(pi * (x + t));
        p.y += Mathf.Sin(pi * (z + t));
        p.y *= 0.5f; // multiplication instructions are quicker than division 
        p.z = z;
        return p;
    }

    private static Vector3 MultiSine2DFunction(float x, float z, float t)
    {
        Vector3 p;
        p.x = x;
        p.y = 4.0f * Mathf.Sin(pi * (x + z + t / 2.0f));
        p.y += Mathf.Sin(pi * (x + t));
        p.y += Mathf.Sin(2.0f * pi * (z + 2.0f * t)) * 0.5f;
        p.y *= 1.0f / 5.5f;
        p.z = z;
        return p;
    }

    private static Vector3 Ripple(float x, float z, float t)
    {
        Vector3 p;
        float d = Mathf.Sqrt(x * x + z * z);
        p.x = x;
        p.y = Mathf.Sin(pi * (4.0f * d - t));
        p.y /= 1.0f + 10.0f * d;
        p.z = z;
        return p;
    }

    private static Vector3 Cylinder(float u, float v, float t)
    {
        float r = 0.8f + Mathf.Sin(pi * (6.0f * u + 2.0f * v + t)) * 0.2f;
        Vector3 p;
        p.x = r * Mathf.Sin(pi * u);
        p.y = v;
        p.z = r * Mathf.Cos(pi * u);
        return p;
    }

    static Vector3 Sphere(float u, float v, float t)
    {
        Vector3 p;
        float r = 0.8f + Mathf.Sin(pi * (6f * u + t)) * 0.1f;
        r += Mathf.Sin(pi * (4f * v + t)) * 0.1f;
        float s = r * Mathf.Cos(pi * 0.5f * v);
        p.x = s * Mathf.Sin(pi * u);
        p.y = r * Mathf.Sin(pi * 0.5f * v);
        p.z = s * Mathf.Cos(pi * u);
        return p;
    }

    static Vector3 Torus(float u, float v, float t)
    {
        Vector3 p;
        float r1 = 0.65f + Mathf.Sin(pi * (6.0f * u + t)) * 0.1f;
        float r2 = 0.2f + Mathf.Sin(pi * (4.0f * v + t)) * 0.05f;
        float s = r2 * Mathf.Cos(pi * v) + r1;
        p.x = s * Mathf.Sin(pi * u);
        p.y = r2 * Mathf.Sin(pi * v);
        p.z = s * Mathf.Cos(pi * u);
        return p;
    }
}
