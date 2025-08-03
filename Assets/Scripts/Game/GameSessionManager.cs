using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSessionManager : MonoBehaviour
{
    public static GameSessionManager Instance { get; private set; }

    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private StageController stageController;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private InGameHUDManager ingameHUDManager;

    public long Score { get; private set; }

    public int Wave { get; private set; }
    private int killedEnemies = 0;
    private int totalEnemies = 0;
    private int pendingExp = 0;

    public GameConfig Config => gameConfig;
    public GameState State { get; private set; } = GameState.WaitingWave;

    private List<ExpOrbComponent> expOrbs = new List<ExpOrbComponent>();
    private List<Vector2> spawnDirs = new List<Vector2>();
    public IReadOnlyList<Vector2> SpawnDirections => spawnDirs;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Init(int characterID)
    {
        Score = 0;
        Wave = 0;
        pendingExp = 0;
        State = GameState.WaitingWave;
        expOrbs.Clear();

        UpgradeManager.Instance.ResetAll();

        if (stageController != null && playerController != null)
        {
            var characterData = CharacterData.Get(characterID);
            UpgradeManager.Instance.Init(WeaponData.Get(characterData.StartWeaponID));

            playerController.Init(characterID);
            stageController.StartStage(playerController.transform);
        }

        ingameHUDManager.Init();
        StartCoroutine(WaveLoop());
    }

    private IEnumerator WaveLoop()
    {
        int mapID = 1;

        while (true)
        {
            if (State == GameState.GameOver)
                yield break;

            Wave++;
            killedEnemies = 0;

            var upgradeStat = UpgradeManager.Instance.AddedUpdateStat;

            totalEnemies = Config.baseEnemyCount + (Wave - 1) * (Config.enemyIncreasePerWave + (upgradeStat.AddEnemyCountByWave * (Wave - upgradeStat.StartAddEnemyCountByWave)));
            totalEnemies = (int)(totalEnemies * upgradeStat.MultipleEnemyCount);

            State = GameState.WaitingWave;
            float targetTime = Time.time + Config.waitTime;
            GameEvent.Publish(EventKeys.GameStateChanged, new GameStatePayload(GameState.WaitingWave, targetTime: targetTime));

            yield return new WaitForSeconds(Config.waitTime);

            State = GameState.InWave;
            GameEvent.Publish(EventKeys.GameStateChanged, new GameStatePayload(GameState.InWave, current: killedEnemies, max: totalEnemies));

            stageController.StartWave(Wave, totalEnemies, Config.spawnInterval * upgradeStat.MultipleAppearEnemyTime, mapID);

            while (killedEnemies < totalEnemies && State != GameState.GameOver)
                yield return null;

            if (State != GameState.GameOver)
            {
                yield return StartCoroutine(CollectAllExp());
                yield return StartCoroutine(ApplyPendingExpRoutine());
            }
        }
    }

    public void OnEnemyKilled()
    {
        if (State != GameState.InWave) return;
        killedEnemies++;
        GameEvent.Publish(EventKeys.GameStateChanged, new GameStatePayload(GameState.InWave, current: killedEnemies, max: totalEnemies));
    }

    public void AddScore(long amount)
    {
        Score += (amount * Wave);
        GameEvent.Publish(EventKeys.GameScoreChanged, Score);
    }

    public void AddPendingExp(int value)
    {
        pendingExp += value;
    }

    private IEnumerator ApplyPendingExpRoutine()
    {
        if (pendingExp <= 0) yield break;

        int expToProcess = pendingExp;
        pendingExp = 0;

        while (expToProcess > 0)
        {
            int prevLevel = Player.Instance.Level;
            expToProcess = Player.Instance.GainExp(expToProcess);

            if (Player.Instance.Level > prevLevel)
            {
                bool popupClosed = false;
                PopupManager.Instance.Open<Popup_SelectCard>().Init(() => popupClosed = true);
                yield return new WaitUntil(() => popupClosed);
            }
            else
            {
                break;
            }
        }
    }

    public void RegisterExpOrb(ExpOrbComponent orb)
    {
        expOrbs.Add(orb);
    }

    private IEnumerator CollectAllExp()
    {
        if (expOrbs.Count == 0) yield break;

        int remaining = expOrbs.Count;
        bool completed = false;

        foreach (var orb in expOrbs)
        {
            if (orb != null)
            {
                orb.Collect(playerController.transform, () => {
                    remaining--;
                    if (remaining <= 0) completed = true;

                    GameObject prefab = PrefabPreLoader.Instance.GetPrefab(PrefabType.EXP_ORB);
                    if (prefab == null) return;

                    ObjectPooler.Instance.Release(prefab, orb.gameObject, SceneHierarchy.Instance.expParent);
                });
            }
        }
        expOrbs.Clear();

        yield return new WaitUntil(() => completed);
    }

    public void UpdateSpawnDirections(int count)
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
}
