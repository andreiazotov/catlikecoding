using static UnityEngine.Mathf; // That makes all constant and static members of the type usable without explicitly mentioning the type itself.

public static class FunctionLibrary
{
    public enum FuncName
    {
        Wave = 0,
        MultiWave,
        Ripple,
    }

    public delegate float Func(float x, float z, float t);

    private static Func[] s_funcs = { Wave, MultiWave, Ripple};

    public static Func GetFunction(FuncName name)
    {
        return s_funcs[(int)name];
    }

    public static float Wave(float x, float z, float t)
    {
        return Sin(PI * (x + z + t));
    }

    public static float MultiWave(float x, float z, float t)
    {
        // Division requires a bit more work than multiplication, so it's a rule of thumb to prefer multiplication over division.
        var y = Sin(PI * (x + 0.5f + t));
        y += 0.5f * Sin(2.0f * PI * (z + t));
        y += Sin(PI * (x + z + 0.25f * t));
        return y * (2.0f / 3.0f);
    }

    public static float Ripple(float x, float z, float t)
    {
        float d = Sqrt(x * x + z * z);
        float y = Sin(PI * (4.0f * d - t));
        return y / (1.0f + 10.0f * d);
    }
}
