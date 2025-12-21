using UnityEngine;

public enum ShapeBehaviorType
{
    Move,
    Rotate,
    Oscillate,
    Satellite,
}

public static class ShapeBehaviorTypeMethods
{
    public static ShapeBehavior GetInstance(this ShapeBehaviorType type)
    {
        switch (type)
        {
            case ShapeBehaviorType.Move:
                return ShapeBehaviorPool<ShapeBehaviorMove>.Get();
            case ShapeBehaviorType.Rotate:
                return ShapeBehaviorPool<ShapeBehaviorRotate>.Get();
            case ShapeBehaviorType.Oscillate:
                return ShapeBehaviorPool<ShapeBehaviorOscillate>.Get();
            case ShapeBehaviorType.Satellite:
                return ShapeBehaviorPool<ShapeBehaviorSatellite>.Get();
        }
        Debug.Log("Forgot to support " + type);
        return null;
    }
}

public abstract class ShapeBehavior
#if UNITY_EDITOR
	: ScriptableObject
#endif
{
#if UNITY_EDITOR
    public bool IsReclaimed { get; set; }
#endif
    public abstract ShapeBehaviorType BehaviorType { get; }
    public abstract bool GameUpdate(Shape shape);
    public abstract void Save(GameDataWriter writer);
    public abstract void Load(GameDataReader reader);
    public abstract void Recycle();
    public virtual void ResolveShapeInstances() { }

#if UNITY_EDITOR
    void OnEnable()
    {
        if (IsReclaimed)
        {
            Recycle();
        }
    }
#endif
}
