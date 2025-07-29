using System.Collections;
using UnityEngine;

public class StageController : MonoBehaviour
{
    public MonsterSpawner spawner;
    private Transform player;

    public void StartStage(Transform playerTransform)
    {
        player = playerTransform;
    }

    public void StartWave(int enemyCount, float spawnInterval)
    {
        StartCoroutine(SpawnWave(enemyCount, spawnInterval));
    }

    private IEnumerator SpawnWave(int enemyCount, float spawnInterval)
    {
        for (int i = 0; i < enemyCount; i++)
        {
            spawner.SpawnMonster(player);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void OnPlayerDead()
    {
        GameSessionManager.Instance.GameOver();
    }
}
