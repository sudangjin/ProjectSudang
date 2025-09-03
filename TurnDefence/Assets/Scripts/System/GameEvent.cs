using System;
using System.Collections.Generic;

public static class GameEvent
{
    private static readonly Dictionary<string, Action<object>> eventTable = new();

    public static void Subscribe(string key, Action<object> callback)
    {
        if (!eventTable.ContainsKey(key))
            eventTable[key] = delegate { };

        eventTable[key] += callback;
    }

    public static void Unsubscribe(string key, Action<object> callback)
    {
        if (eventTable.ContainsKey(key))
            eventTable[key] -= callback;
    }

    public static void Publish(string key, object data = null)
    {
        if (eventTable.TryGetValue(key, out var callback))
            callback?.Invoke(data);
    }

    public static void Clear()
    {
        eventTable.Clear();
    }
}
