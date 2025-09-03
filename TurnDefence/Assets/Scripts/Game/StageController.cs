using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageController : MonoBehaviour
{
    [SerializeField] private MonsterSpawner spawner;

    private Transform player;

    public void StartStage(Transform playerTransform)
    {
        player = playerTransform;
    }

    public void StartWave(int waveIndex, int enemyCount, float spawnInterval, int mapID, int enemyPowerMultiplier)
    {
        StartCoroutine(SpawnWave(waveIndex, enemyCount, spawnInterval, mapID, enemyPowerMultiplier));
    }

    private IEnumerator SpawnWave(int waveIndex, int enemyCount, float spawnInterval, int mapID, int enemyPowerMultiplier)
    {
        var ratios = GameSessionManager.Instance.Config.monsterSpawnRatios;
        float totalWeight = ratios.Sum(r => r.ratio);

        Queue<MonsterData> spawnQueue = new Queue<MonsterData>();

        for (int i = 0; i < enemyCount; i++)
        {
            if ((waveIndex % 5 == 0) && (i == enemyCount - 1))
            {
                var bosses = DataManager.Instance.GetBossesForMap(mapID);
                if (bosses.Count > 0)
                {
                    spawnQueue.Enqueue(bosses[Random.Range(0, bosses.Count)]);
                    continue;
                }
            }

            float roll = Random.Range(0, totalWeight);
            float cumulative = 0;
            int selectedGrade = ratios[0].grade;

            foreach (var entry in ratios)
            {
                cumulative += entry.ratio;
                if (roll < cumulative)
                {
                    selectedGrade = entry.grade;
                    break;
                }
            }

            var monstersByGrade = DataManager.Instance.GetMonstersByGradeForMap(mapID);
            if (monstersByGrade.TryGetValue(selectedGrade, out var monsterList) && monsterList.Count > 0)
                spawnQueue.Enqueue(monsterList[Random.Range(0, monsterList.Count)]);
        }

        while (spawnQueue.Count > 0)
        {
            var monster = spawnQueue.Dequeue();
            spawner.SpawnMonster(player, monster, enemyPowerMultiplier);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void OnPlayerDead()
    {
        GameSessionManager.Instance.GameOver();
    }
}
