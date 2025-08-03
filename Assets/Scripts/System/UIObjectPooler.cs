using System.Collections.Generic;
using UnityEngine;

public class UIObjectPooler : MonoBehaviour
{
    [SerializeField] private GameObject cellPrefab;

    private readonly List<GameObject> pooledObjects = new List<GameObject>();
    private Transform parentTransform;

    private void Awake()
    {
        if (cellPrefab != null)
        {
            cellPrefab.SetActive(false);
            parentTransform = cellPrefab.transform.parent;
        }
    }

    public GameObject Get()
    {
        foreach (var obj in pooledObjects)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        GameObject newObj = Instantiate(cellPrefab, parentTransform);
        newObj.SetActive(true);
        pooledObjects.Add(newObj);
        return newObj;
    }

    public T Get<T>() where T : Component
    {
        GameObject obj = Get();
        return obj.GetComponent<T>();
    }

    public void Release(GameObject obj)
    {
        if (obj == null) return;
        obj.SetActive(false);
    }

    public void ReleaseAll()
    {
        foreach (var obj in pooledObjects)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }
}
