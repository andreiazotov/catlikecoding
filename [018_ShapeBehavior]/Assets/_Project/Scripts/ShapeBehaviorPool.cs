using UnityEngine;
using System.Collections.Generic;

public static class ShapeBehaviorPool<T> where T : ShapeBehavior, new()
{
    private static readonly Stack<T> stack = new();

    public static T Get()
    {
        if (stack.Count > 0)
        {
            T behavior = stack.Pop();
#if UNITY_EDITOR
            behavior.IsReclaimed = false;
#endif
            return behavior;
        }
#if UNITY_EDITOR
        return ScriptableObject.CreateInstance<T>();
#else
		return new T();
#endif
    }

    public static void Reclaim(T behavior)
    {
#if UNITY_EDITOR
        behavior.IsReclaimed = true;
#endif
        stack.Push(behavior);
    }
}
