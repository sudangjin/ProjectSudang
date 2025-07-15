using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIPlayerLevelInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private UIGauge expGauge;
    private void Start()
    {
        var player = GameManager.Instance.Player; // È¤Àº PlayerController.Instance

        SetLevel(player.GetLevel());

        var current = player.GetCurrentExp();
        var max = player.GetExpToNextLevel();
        SetExpGauge(current, max);
    }

    private void OnEnable()
    {
        GameEvent.Subscribe(EventKeys.PlayerLevelChanged, OnLevelChanged);
        GameEvent.Subscribe(EventKeys.PlayerExpChanged, OnExpChanged);
    }

    private void OnDisable()
    {
        GameEvent.Unsubscribe(EventKeys.PlayerLevelChanged, OnLevelChanged);
        GameEvent.Unsubscribe(EventKeys.PlayerExpChanged, OnExpChanged);
    }

    private void OnLevelChanged(object level)
    {
        SetLevel((int)level);
    }

    private void OnExpChanged(object data)
    {
        var (current, max) = ((int, int))data;
        SetExpGauge(current, max);
    }

    public void SetLevel(int level)
    {
        levelText.text = $"LV.{level}";
    }

    public void SetExpGauge(int current, int max)
    {
        expGauge.Init(max);
        expGauge.UpdateValue(current);
    }

    public void SetVisible(bool isVisible)
    {
        gameObject.SetActive(isVisible);
    }
}
