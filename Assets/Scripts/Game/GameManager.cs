using UnityEngine;

public enum GameState
{
    None,
    Init,
    Playing,
    GameOver,
    Victory
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState State { get; private set; } = GameState.None;

    public Transform player;
    public StageController stageController;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitializeGame();
    }

    public void InitializeGame()
    {
        State = GameState.Playing;

        // 스테이지 시작
        stageController.StartStage(player);
    }

    public void GameOver()
    {
        if (State == GameState.GameOver) return;

        State = GameState.GameOver;
        Debug.Log("Game Over!");
        // TODO: UI 구현
    }

    public void Victory()
    {
        if (State == GameState.Victory) return;

        State = GameState.Victory;
        Debug.Log("Stage Cleared!");
        // TODO: UI 구현
    }
}
