using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Popup_SelectCard : PopupBase
{
    [SerializeField] private UIObjectPooler pooler;
    [SerializeField] private TextMeshProUGUI rerollCountText;
    [SerializeField] private TextMeshProUGUI hideRerollCountText;

    [SerializeField] private GameObject rerollButtons;
    [SerializeField] private GameObject closeButton;

    private int hideRerollDice;
    private List<CardCell> cardCells = new List<CardCell>();
    private bool interactionEnabled = false;
    private Action onPopupClosed;

    public void Init(Action onClosed)
    {
        onPopupClosed = onClosed;
        hideRerollDice = 1;
        SetCard(false);

        rerollCountText.text = UpgradeManager.Instance.RerollDice.ToString();
        hideRerollCountText.text = hideRerollDice.ToString();

        rerollButtons.SetActive(true);
        closeButton.SetActive(false);
    }

    private void SetCard(bool isHideMode)
    {
        var upgradeMap = UpgradeManager.Instance.GetSelectableUpgrade(3, isHideMode);

        pooler.ReleaseAll();
        cardCells.Clear();
        Queue<CardCell> cardQueue = new Queue<CardCell>();

        foreach (var upgradePair in upgradeMap)
        {
            var cell = pooler.Get<CardCell>();
            cell.Init(upgradePair.Key, upgradePair.Value, isHideMode, OnSelect);
            cardQueue.Enqueue(cell);
            cardCells.Add(cell);
        }

        interactionEnabled = false;

        if (!isHideMode)
            StartCoroutine(FlipCardsSequentially(cardQueue));
        else
            EnableCardInteractions();
    }

    private IEnumerator FlipCardsSequentially(Queue<CardCell> cardQueue)
    {
        while (cardQueue.Count > 0)
        {
            var cell = cardQueue.Dequeue();
            cell.OpenCard();
            yield return new WaitForSecondsRealtime(cell.OpenTime);
        }

        EnableCardInteractions();
    }

    private void EnableCardInteractions()
    {
        foreach (var cell in cardCells)
            cell.EnableInteraction();

        interactionEnabled = true;
    }

    private void DisableCardInteractions()
    {
        foreach (var cell in cardCells)
            cell.DisableInteraction();

        interactionEnabled = false;
    }

    private void ShowCloseButton(bool isShow)
    {
        closeButton.SetActive(true);
    }

    private void ShowRerollButton(bool isShow)
    {
        rerollButtons.SetActive(isShow);
    }

    private IEnumerator CloseOtherCardsAndExit(UpgradeData selected)
    {
        foreach (var cell in cardCells)
        {
            var isSelected = cell.UpgradeData.ID == selected.ID;

            if (isSelected) cell.OpenCard();
            else cell.CloseCard();
        }

        yield return new WaitForSecondsRealtime(cardCells[0].CloseTime);

        ShowCloseButton(true);
    }

    public void OnReroll()
    {
        if (UpgradeManager.Instance.UseRerollDice())
        {
            SetCard(false);

            rerollCountText.text = UpgradeManager.Instance.RerollDice.ToString();
        }
    }

    public void OnRerollHideMode()
    {
        if (hideRerollDice > 0)
        {
            hideRerollDice--;
            SetCard(true);

            hideRerollCountText.text = hideRerollDice.ToString();
        }
    }

    private void OnSelect(UpgradeData upgrade)
    {
        if (!interactionEnabled)
            return;

        UpgradeManager.Instance.ApplyUpgrade(upgrade.ID);

        DisableCardInteractions();
        ShowRerollButton(false);
        StartCoroutine(CloseOtherCardsAndExit(upgrade));
    }

    public override void Close()
    {
        base.Close();

        onPopupClosed?.Invoke();
    }
}
