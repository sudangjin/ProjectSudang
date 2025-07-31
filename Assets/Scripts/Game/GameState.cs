public enum GameState
{
    WaitingWave,
    InWave,
    GameOver
}
public class GameStatePayload
{
    public GameState State;
    public int Current;
    public int Max;
    public float TargetTime;

    public GameStatePayload(GameState state, int current = 0, int max = 0, float targetTime = 0f)
    {
        State = state;
        Current = current;
        Max = max;
        TargetTime = targetTime;
    }
}
