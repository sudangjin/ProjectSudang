using UnityEngine;
using System.Collections.Generic;

public class MonsterSpawner : MonoBehaviour
{
    public static MonsterSpawner Instance { get; private set; }

    [SerializeField] private float spawnRadius = 8f;
    [SerializeField] private int spawnDirections = 12;

    private List<Vector2> spawnDirs = new();
    public IReadOnlyList<Vector2> SpawnDirections => spawnDirs;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        UpdateSpawnDirections(GameSessionManager.Instance.Config.spawnDirections);
    }

    public void SpawnMonster(Transform player, MonsterData monster)
    {
        var config = GameSessionManager.Instance.Config;

        Vector2 direction = spawnDirs[Random.Range(0, spawnDirs.Count)];
        Vector2 offset = new Vector2(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
        Vector2 spawnPos = (Vector2)player.position + direction * config.spawnRadius + offset;

        GameObject prefab = Resources.Load<GameObject>($"Prefabs/Monster/{monster.PrefabName}");
        if (prefab == null) return;

        GameObject monsterObj = ObjectPooler.Instance.Create(prefab, SceneHierarchy.Instance.monstersParent);
        monsterObj.transform.position = spawnPos;

        var controller = monsterObj.GetComponent<MonsterController>();
        controller.PrefabReference = prefab;
        controller.Initialize(
            player,
            monster.MoveSpeed,
            monster.AttackRange,
            monster.HP,
            monster.Damage,
            monster.AttackSpeed,
            monster.EXP,
            monster.Score,
            monster.MoveType,
            monster.ProjectileID,
            monster.IsBoss
        );
    }

    private void UpdateSpawnDirections(int count)
    {
        spawnDirs.Clear();
        for (int i = 0; i < count; i++)
        {
            float angle = 180f - (360f / count * i);
            float rad = angle * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
            spawnDirs.Add(dir);
        }
    }
}
