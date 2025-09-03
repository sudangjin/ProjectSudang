using UnityEngine;

public class InGameHUDManager : MonoBehaviour
{
    public static InGameHUDManager Instance { get; private set; }

    [SerializeField] private UIPlayerLevelInfo uiPlayerLevelInfo;
    [SerializeField] private UIProgressInfo uiProgressInfo;
    [SerializeField] private UIScoreInfo uiScoreInfo;
    [SerializeField] private UIWaveInfo uiWave;

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
        uiPlayerLevelInfo.Init();
        uiProgressInfo.Init();
        uiScoreInfo.Init();
    }

    public void OnClickPause() => PopupManager.Instance.Open<Popup_Pause>();
}
