using UnityEngine;

public class StageController : MonoBehaviour
{
    public MonsterSpawner spawner;

    public void StartStage(Transform player)
    {
        spawner.StartSpawn(player);
    }

    public void OnFinishStage()
    {
        GameManager.Instance.Victory();
    }

    public void OnPlayerDead()
    {
        GameManager.Instance.GameOver();
    }
}
