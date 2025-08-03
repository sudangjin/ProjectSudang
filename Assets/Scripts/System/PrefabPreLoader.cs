using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabPreLoader : MonoBehaviour
{
    public static PrefabPreLoader Instance { get; private set; }

    private IReadOnlyDictionary<PrefabType, GameObject> preparePrefabMap = null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;


        Dictionary<PrefabType, GameObject> prefabDict = new Dictionary<PrefabType, GameObject>();
        prefabDict.Add(PrefabType.EXP_ORB, Resources.Load<GameObject>("Prefabs/EXP"));
        preparePrefabMap = prefabDict;
    }

    public GameObject GetPrefab(PrefabType type)
    {
        return preparePrefabMap.TryGetValue(type, out GameObject prefab) ? prefab : null;
    }
}
