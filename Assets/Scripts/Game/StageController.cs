using UnityEngine;

public class StageController : MonoBehaviour
{
    public MonsterSpawner spawner;

    public void StartStage(Transform player)
    {
        spawner.StartSpawn(player);
    }

    public void OnMonsterCleared()
    {
        if (spawner.AllMonstersDefeated())
        {
            GameManager.Instance.Victory();
        }
    }

    public void OnPlayerDead()
    {
        GameManager.Instance.GameOver();
    }
}
