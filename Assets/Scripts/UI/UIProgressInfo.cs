using System.Collections;
using TMPro;
using UnityEngine;

public class UIProgressInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI progressText;

    private Coroutine timerRoutine;

    public void Init()
    {
        progressText.text = string.Empty;
    }

    private void OnEnable()
    {
        GameEvent.Subscribe(EventKeys.GameStateChanged, OnGameStateUpdate);
    }

    private void OnDisable()
    {
        GameEvent.Unsubscribe(EventKeys.GameStateChanged, OnGameStateUpdate);
    }

    private void OnGameStateUpdate(object data)
    {
        var payload = (GameStatePayload)data;

        if (payload.State == GameState.WaitingWave)
        {
            if (timerRoutine != null) StopCoroutine(timerRoutine);
            timerRoutine = StartCoroutine(UpdateTimer(payload.TargetTime));
        }
        else if (payload.State == GameState.InWave)
        {
            SetProgress(payload.Current, payload.Max);
        }
        else if (payload.State == GameState.GameOver)
        {
            progressText.text = "Game Over!";
            if (timerRoutine != null) StopCoroutine(timerRoutine);
        }
    }

    private IEnumerator UpdateTimer(float targetTime)
    {
        while (Time.time < targetTime)
        {
            float remain = targetTime - Time.time;
            SetTimer(remain);
            yield return null;
        }
        SetTimer(0);
    }

    public void SetTimer(float time)
    {
        progressText.text = $"{time:F1}";
    }

    public void SetProgress(int current, int max)
    {
        progressText.text = $"{current} / {max}";
    }
}
