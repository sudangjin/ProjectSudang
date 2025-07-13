using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private GameObject monsterPrefab;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float spawnRadius = 8f;

    [Header("Params")]
    [SerializeField] private float defaultMoveSpeed = 2f;
    [SerializeField] private float defaultAttackRange = 1.5f;
    [SerializeField] private int defaultHP = 20;

    private Transform player;
    private float timer;
    private bool isSpawning;

    public void StartSpawn(Transform playerTransform)
    {
        player = playerTransform;
        isSpawning = true;
        timer = 0f;
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

    private void SpawnMonster()
    {
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        Vector2 spawnPos = (Vector2)player.position + randomDir * spawnRadius;

        GameObject monsterObj = Instantiate(monsterPrefab, spawnPos, Quaternion.identity);

        MonsterController controller = monsterObj.GetComponent<MonsterController>();
        controller.Initialize(player, defaultMoveSpeed, defaultAttackRange, defaultHP);
    }

    public bool AllMonstersDefeated()
    {
        return GameObject.FindGameObjectsWithTag("Monster").Length == 0;
    }
}
