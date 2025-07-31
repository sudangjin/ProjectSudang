using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIScoreInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    public void Init()
    {
        UpdateScore(0);
    }

    private void OnEnable()
    {
        GameEvent.Subscribe(EventKeys.GameScoreChanged, OnGameStateUpdate);
    }

    private void OnDisable()
    {
        GameEvent.Unsubscribe(EventKeys.GameScoreChanged, OnGameStateUpdate);
    }


    public void OnGameStateUpdate(object data)
    {
        UpdateScore((long)data);
    }

    public void UpdateScore(long score)
    { 
        scoreText.text = score.ToMoneyFormat();
    }
}
