using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardCell : MonoBehaviour
{
    [SerializeField] private RectTransform frontCardRect;
    [SerializeField] private RectTransform backCardRect;

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descText;

    [SerializeField] private TextMeshProUGUI multiplierText;

    [SerializeField] private GameObject negativeMark;

    [SerializeField] private EventTrigger eventTrigger;

    private float openTime = 0.2f;
    public float OpenTime => openTime * 2;

    private float closeTime = 0.1f;
    public float CloseTime => closeTime * 2;

    public UpgradeData UpgradeData { get; private set; }
    public int UpgradeCount { get; private set; }
    private Action<UpgradeData> onSelected = null;

    public bool IsOpen { get; private set; }

    public void Init(UpgradeData upgrade, int upgradeCount, bool isHideMode, Action<UpgradeData> onSelected)
    {
        UpgradeData = upgrade;
        UpgradeCount = upgradeCount;

        nameText.text = upgrade.Name;
        descText.text = UpgradeManager.Instance.GetDesc(upgrade.ID, true);
        multiplierText.text = UpgradeCount > 1 ? $"x{UpgradeCount}" : string.Empty;
        negativeMark.SetActive(upgrade.IsNegative);

        SetCardActive(false);

        if (eventTrigger != null)
            eventTrigger.enabled = false;

        this.onSelected = onSelected;

    }

    public void EnableInteraction()
    {
        if (eventTrigger != null)
            eventTrigger.enabled = true;
    }

    public void DisableInteraction()
    {
        if (eventTrigger != null)
            eventTrigger.enabled = false;
    }

    public void SetCardActive(bool isOpen)
    {
        frontCardRect.localScale = isOpen ? Vector3.one : new Vector3(0f, 1f, 1f);
        backCardRect.localScale = isOpen ? new Vector3(0f, 1f, 1f) : Vector3.one;
        IsOpen = isOpen;
    }

    public void OpenCard()
    {
        if (IsOpen) return;

        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(backCardRect.DOScaleX(0f, openTime).SetEase(Ease.InBack))
            .Append(frontCardRect.DOScale(1f, openTime).SetEase(Ease.OutBack))
            .OnComplete(()=> { IsOpen = true; })
            .SetUpdate(true)
            .Play();
    }

    public void CloseCard()
    {
        if (!IsOpen) return;

        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(frontCardRect.DOScaleX(0f, closeTime).SetEase(Ease.InBack))
            .Append(backCardRect.DOScale(1f, closeTime).SetEase(Ease.OutBack))
            .OnComplete(() => { IsOpen = false; })
            .SetUpdate(true)
            .Play();
    }

    public void OnSelect()
    {
        onSelected?.Invoke(UpgradeData);
    }
}
