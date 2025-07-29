using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSessionManager : MonoBehaviour
{
    public static GameSessionManager Instance { get; private set; }

    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private StageController stageController;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private InGameHUDManager ingameHUDManager;

    private long score = 0;
    private int wave = 0;
    private int killedEnemies = 0;
    private int totalEnemies = 0;

    public GameConfig Config => gameConfig;
    public GameState State { get; private set; } = GameState.WaitingWave;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Init()
    {
        score = 0;
        wave = 0;
        State = GameState.WaitingWave;

        if (stageController != null && playerController != null)
        {
            playerController.Init(gameConfig.playerMaxHP);
            stageController.StartStage(playerController.transform);
        }
        else
        {
            Debug.LogWarning("StageController 또는 PlayerController가 연결되지 않았습니다.");
        }

        ingameHUDManager.Init();

        StartCoroutine(WaveLoop());
    }

    private IEnumerator WaveLoop()
    {
        while (true)
        {
            wave++;
            killedEnemies = 0;
            totalEnemies = Config.baseEnemyCount + (wave - 1) * Config.enemyIncreasePerWave;

            State = GameState.WaitingWave;
            float targetTime = Time.time + Config.waitTime;
            GameEvent.Publish(EventKeys.GameStateChanged, new GameStatePayload(GameState.WaitingWave, targetTime: targetTime));

            yield return new WaitForSeconds(Config.waitTime);

            State = GameState.InWave;
            GameEvent.Publish(EventKeys.GameStateChanged, new GameStatePayload(GameState.InWave, current: killedEnemies, max: totalEnemies));

            stageController.StartWave(totalEnemies, Config.spawnInterval);

            while (killedEnemies < totalEnemies && State != GameState.GameOver)
                yield return null;
        }
    }


    public void OnEnemyKilled()
    {
        if (State != GameState.InWave) return;

        killedEnemies++;
        GameEvent.Publish(EventKeys.GameStateChanged, new GameStatePayload(GameState.InWave, current: killedEnemies, max: totalEnemies));
    }

    public void GameOver()
    {
        if (State == GameState.GameOver) return;

        State = GameState.GameOver;
        GameEvent.Publish(EventKeys.GameStateChanged, new GameStatePayload(GameState.GameOver));

        PopupManager.Instance.Open<Popup_FinishGame>().Init(() => {
            StartCoroutine(ExitGame());
        });
    }

    private IEnumerator ExitGame()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync("Lobby");
        while (!op.isDone)
            yield return null;
    }

    public void AddScore(int amount)
    {
        score += amount;
        GameEvent.Publish(EventKeys.GameScoreChanged, score);
    }
}
