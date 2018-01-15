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

    private static GraphFunction[] s_functions = {
        SineFunction,
        MultiSineFunction
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

        this._points = new Transform[this.resolution];
        for (int i = 0; i < this.resolution; i++)
        {
            var point = Instantiate(this.pointPrefab);
            position.x = ((i + 0.5f) * step - 1.0f);
            point.localPosition = position;
            point.localScale = scale;
            point.SetParent(this.transform, false);

            this._points[i] = point;
        }
    }

    private void FixedUpdate()
    {
        float t = Time.time;
        var f = s_functions[(int)this.function];

        for (int i = 0; i < this._points.Length; i++)
        {
            var point = this._points[i];
            var position = point.localPosition;
            position.y = f(position.x, t);
            point.localPosition = position;
        }
    }

    /*
     * Because static methods aren't associated with object instances,
     * the compiled code doesn't have to keep track of which object
     * invoking the method on. This means that static method invocations
     * are a bit faster, but it's usually not significant enough.
    */
    private static float SineFunction(float x, float t)
    {
        return Mathf.Sin(Mathf.PI * (x + t));
    }

    private static float MultiSineFunction(float x, float t)
    {
        float y = Mathf.Sin(Mathf.PI * (x + t));
        y += Mathf.Sin(2.0f * Mathf.PI * (x + 2.0f * t)) / 2.0f;
        y *= 2.0f / 3.0f;
        return y;
    }
}
