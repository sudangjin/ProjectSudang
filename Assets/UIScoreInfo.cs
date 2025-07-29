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

    public void UpdateScore(int score)
    { 
        scoreText.text = score.ToMoneyFormat();
    }
}
