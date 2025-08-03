using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NoticeLabel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textLabel;

    private Coroutine waitClose = null;

    public void ShowLabel(string text, Color color)
    {
        if (textLabel == null)
        {
            Debug.LogError("TextMeshProUGUI 컴포넌트가 할당되지 않았습니다.");
            return;
        }

        if (waitClose != null)
        {
            StopCoroutine(waitClose);
            waitClose = null;
        }

        textLabel.text = text;
        textLabel.color = color;

        textLabel.gameObject.SetActive(true);

        Sequence sequence = DOTween.Sequence();
        sequence
            .OnStart(() => {
                textLabel.rectTransform.anchoredPosition = new Vector2(0f, -30f); // 시작 위치 설정
                textLabel.DOFade(0f, 0f); // 투명도 초기화
            })
            .Append(textLabel.rectTransform.DOAnchorPosY(0f, 0.3f))
            .Join(textLabel.DOFade(1f, 0.3f))
            .OnComplete(() => {
                waitClose = StartCoroutine(WaitClose(1.5f));
            })
            .SetUpdate(true)
            .Play();
    }

    public IEnumerator WaitClose(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);
        textLabel.gameObject.SetActive(false);
    }
}
