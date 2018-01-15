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
        Ripple
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
        for (int i = 0, z = 0; z < this.resolution; z++)
        {
            position.z = (z + 0.5f) * step - 1.0f;
            for (int x = 0; x < this.resolution; x++, i++)
            {
                var point = Instantiate(this.pointPrefab);
                position.x = (x + 0.5f) * step - 1.0f;
                point.localPosition = position;
                point.localScale = scale;
                point.SetParent(this.transform, false);
                this._points[i] = point;
            }
        }
    }

    private void Update()
    {
        float t = Time.time;
        var f = s_functions[(int)this.function];

        for (int i = 0; i < this._points.Length; i++)
        {
            var point = this._points[i];
            var position = point.localPosition;
            position.y = f(position.x, position.z, t);
            point.localPosition = position;
        }
    }

    /*
     * Because static methods aren't associated with object instances,
     * the compiled code doesn't have to keep track of which object
     * invoking the method on. This means that static method invocations
     * are a bit faster, but it's usually not significant enough.
    */
    private static float SineFunction(float x, float z, float t)
    {
        return Mathf.Sin(pi * (x + t));
    }

    private static float MultiSineFunction(float x, float z, float t)
    {
        float y = Mathf.Sin(pi * (x + t));
        y += Mathf.Sin(2.0f * pi * (x + 2.0f * t)) / 2.0f;
        y *= 2.0f / 3.0f;
        return y;
    }

    private static float Sine2DFunction(float x, float z, float t)
    {
        float y = Mathf.Sin(pi * (x + t));
        y += Mathf.Sin(pi * (z + t));
        y *= 0.5f; // multiplication instructions are quicker than division 
        return y;
    }

    private static float MultiSine2DFunction(float x, float z, float t)
    {
        float y = 4f * Mathf.Sin(pi * (x + z + t * 0.5f));
        y += Mathf.Sin(pi * (x + t));
        y += Mathf.Sin(2f * pi * (z + 2.0f * t)) * 0.5f;
        y *= 1.0f / 5.5f;
        return y;
    }

    private static float Ripple(float x, float z, float t)
    {
        float d = Mathf.Sqrt(x * x + z * z);
        float y = Mathf.Sin(pi * (4.0f * d - t));
        y /= 1.0f + 10.0f * d;
        return y;
    }
}
