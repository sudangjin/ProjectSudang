using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIWaveInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI waveText;

    private void OnEnable()
    {
        GameEvent.Subscribe(EventKeys.GameWaveChanged, OnWaveUpdated);
    }

    private void OnDisable()
    {
        GameEvent.Unsubscribe(EventKeys.GameWaveChanged, OnWaveUpdated);
    }

    private void OnWaveUpdated(object data)
    {
        waveText.text = $"Wave {(int)data}";
    }
}
