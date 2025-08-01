using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ExpOrbComponent : MonoBehaviour
{
    [SerializeField] private SpriteRenderer shineSprite;
    private SpriteRenderer spriteRenderer;

    public int Value { get; private set; }

    private Transform target;
    private bool isCollecting = false;

    private Tweener colorTween;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(int value, int grade)
    {
        Value = value;
        isCollecting = false;
        spriteRenderer.color = GameSessionManager.Instance.Config.expColors[grade - 1];
        StartShine();
    }

    public void Collect(Transform player)
    {
        if (isCollecting) return;
        target = player;
        isCollecting = true;

        if (colorTween != null && colorTween.IsActive())
            colorTween.Kill();

        StartCoroutine(MoveToPlayer());
    }

    private void StartShine()
    {
        if (shineSprite == null) return;

        if (colorTween != null && colorTween.IsActive())
            colorTween.Kill();

        colorTween = spriteRenderer
            .DOFade(0.5f, 0.8f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private IEnumerator MoveToPlayer()
    {
        Vector3 start = transform.position;
        Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            Vector3 curve = Vector3.Lerp(start, target.position, t);
            curve += randomOffset * (1f - t);
            transform.position = curve;
            yield return null;
        }

        Destroy(gameObject);
    }
}
