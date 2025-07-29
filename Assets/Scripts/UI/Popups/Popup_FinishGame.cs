using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Popup_FinishGame : PopupBase
{
    [SerializeField] private TextMeshProUGUI txtScore;

    private Action callback = null;

    public void Init(Action okCallback)
    {
        callback = okCallback;
    }

    public void OnExitSession()
    {
        callback?.Invoke();
        Close();
    }
}
