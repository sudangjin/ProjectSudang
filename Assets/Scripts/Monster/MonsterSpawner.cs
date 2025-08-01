using UnityEngine;
using System.Collections.Generic;

public class MonsterSpawner : MonoBehaviour
{
    public static MonsterSpawner Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SpawnMonster(Transform player, MonsterData monster)
    {
        var config = GameSessionManager.Instance.Config;

        var spawnDirs = GameSessionManager.Instance.SpawnDirections;

        Vector2 direction = spawnDirs[Random.Range(0, spawnDirs.Count)];
        Vector2 offset = new Vector2(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
        Vector2 spawnPos = (Vector2)player.position + direction * config.spawnRadius + offset;

        GameObject prefab = Resources.Load<GameObject>($"Prefabs/Monster/{monster.PrefabName}");
        if (prefab == null) return;

        GameObject monsterObj = ObjectPooler.Instance.Create(prefab, SceneHierarchy.Instance.monstersParent);
        monsterObj.transform.position = spawnPos;

        var controller = monsterObj.GetComponent<MonsterController>();
        controller.PrefabReference = prefab;
        controller.Initialize(player, monster);
    }
}
