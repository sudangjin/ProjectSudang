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

    [Header("게임 참조")]
    [SerializeField] private StageController stageController;
    [SerializeField] private Transform player;

    public GameState State { get; private set; } = GameState.None;

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

    private void InitializeGame()
    {
        State = GameState.Playing;

        if (stageController != null && player != null)
        {
            stageController.StartStage(player);
        }
        else
        {
            Debug.LogWarning("StageController 또는 Player가 연결되지 않았습니다.");
        }
    }

    public void GameOver()
    {
        if (State == GameState.GameOver) return;

        State = GameState.GameOver;
        Debug.Log("Game Over");
    }

    public void Victory()
    {
        if (State == GameState.Victory) return;

        State = GameState.Victory;
        Debug.Log("Victory!");
    }
}
