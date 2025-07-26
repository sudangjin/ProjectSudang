using UnityEngine;
using System.Collections.Generic;

public class MonsterSpawner : MonoBehaviour
{
    public static MonsterSpawner Instance { get; private set; }

    [Header("Spawn")]
    [SerializeField] private GameObject monsterPrefab;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float spawnRadius = 8f;
    [SerializeField] private int spawnDirections = 12;

    [Header("Params")]
    [SerializeField] private float defaultMoveSpeed = 2f;
    [SerializeField] private float defaultAttackRange = 1.5f;
    [SerializeField] private int defaultHP = 3;

    private Transform player;
    private float timer;
    private bool isSpawning;
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
        UpdateSpawnDirections(GameManager.Instance.Config.spawnDirections);
    }

    public void StartSpawn(Transform playerTransform)
    {
        player = playerTransform;
        isSpawning = true;
        timer = 0f;
        UpdateSpawnDirections(GameManager.Instance.Config.spawnDirections);
    }

    public void StopSpawn()
    {
        isSpawning = false;
    }

    private void Update()
    {
        if (!isSpawning || player == null) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnMonster();
        }
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

    private void SpawnMonster()
    {
        var config = GameManager.Instance.Config;

        Vector2 direction = spawnDirs[Random.Range(0, spawnDirs.Count)];
        Vector2 offset = new Vector2(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
        Vector2 spawnPos = (Vector2)player.position + direction * config.spawnRadius + offset;

        GameObject monsterObj = ObjectPooler.Instance.Create(config.monsterPrefab, SceneHierarchy.Instance.monstersParent);
        monsterObj.transform.position = spawnPos;

        var controller = monsterObj.GetComponent<MonsterController>();
        controller.Initialize(player, config.monsterMoveSpeed, config.monsterAttackRange, config.monsterHP, config.monsterPrefab);
    }
}
