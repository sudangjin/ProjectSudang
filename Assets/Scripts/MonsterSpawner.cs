using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab;
    public Transform player;
    public float spawnInterval = 2f;
    public float spawnRadius = 10f;

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnMonster();
            timer = 0f;
        }
    }

    void SpawnMonster()
    {
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        Vector2 spawnPos = (Vector2)player.position + randomDir * spawnRadius;

        GameObject monster = Instantiate(monsterPrefab, spawnPos, Quaternion.identity);
        monster.GetComponent<MonsterMover>().Initialize(player);
    }
}
