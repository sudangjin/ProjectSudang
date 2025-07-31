using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance { get; private set; }

    private Dictionary<GameObject, Stack<GameObject>> pools = new();
    private Dictionary<GameObject, Transform> prefabPools = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public GameObject Create(GameObject prefab, Transform parent)
    {
        if (!pools.TryGetValue(prefab, out var stack))
        {
            stack = new Stack<GameObject>();
            pools[prefab] = stack;
        }

        GameObject obj;
        if (stack.Count > 0)
        {
            obj = stack.Pop();
            obj.SetActive(true);
        }
        else
        {
            obj = Instantiate(prefab);
        }

        obj.transform.SetParent(parent, false);
        return obj;
    }

    public void Release(GameObject prefab, GameObject obj, Transform parent)
    {
        if (prefab == null)
        {
            Debug.LogError($"[ObjectPooler] Prefab reference missing for {obj.name}");
            Destroy(obj);
            return;
        }

        if (!prefabPools.TryGetValue(prefab, out var poolTransform))
        {
            string poolName = prefab.name + "_Pool";
            GameObject poolObj = new GameObject(poolName);
            poolObj.transform.SetParent(parent, false);
            prefabPools[prefab] = poolObj.transform;
        }

        obj.SetActive(false);
        obj.transform.SetParent(prefabPools[prefab], false);

        if (!pools.TryGetValue(prefab, out var stack))
        {
            stack = new Stack<GameObject>();
            pools[prefab] = stack;
        }
        stack.Push(obj);
    }

    public void ClearPool(GameObject prefab)
    {
        if (pools.TryGetValue(prefab, out var stack))
        {
            foreach (var obj in stack)
                Destroy(obj);
            stack.Clear();
        }

        if (prefabPools.TryGetValue(prefab, out var container))
        {
            Destroy(container.gameObject);
            prefabPools.Remove(prefab);
        }

        pools.Remove(prefab);
    }

    public void ClearAllPools()
    {
        foreach (var stack in pools.Values)
        {
            foreach (var obj in stack)
                Destroy(obj);
        }
        pools.Clear();

        foreach (var parent in prefabPools.Values)
            Destroy(parent.gameObject);

        prefabPools.Clear();
    }
}
