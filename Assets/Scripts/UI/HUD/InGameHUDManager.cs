using UnityEngine;

public class InGameHUDManager : MonoBehaviour
{
    public static InGameHUDManager Instance { get; private set; }

    [SerializeField] private UIPlayerLevelInfo uiPlayerLevelInfo;
    [SerializeField] private UIProgressInfo uIProgressInfo;
    [SerializeField] private UIScoreInfo uIScoreInfo;

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
        uIProgressInfo.Init();
        uIScoreInfo.Init();
    }

    public void OnClickPause() => PopupManager.Instance.Open<Popup_Pause>();
}
