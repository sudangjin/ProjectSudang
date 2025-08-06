using UnityEngine;
using System.Collections;
using DG.Tweening;
using System;

public class ExpOrbComponent : MonoBehaviour
{
    private ParticleSystem particle;

    public int Value { get; private set; }

    private Transform target;
    private bool isCollecting = false;

    private Tweener colorTween;

    private Action finishCallback = null;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    public void Init(int value, int grade)
    {
        Value = value;
        isCollecting = false;

        var color = GameSessionManager.Instance.Config.expColors[grade - 1];
        var main = particle.main;
        main.startColor = color;

        var colorOverLifetime = particle.colorOverLifetime;
        if (colorOverLifetime.enabled)
        {
            Gradient grad = new Gradient();

            grad.SetKeys(
                new GradientColorKey[] {
                new GradientColorKey(color, 0f),
                new GradientColorKey(Color.white, 1f)
                },
                new GradientAlphaKey[] {
                new GradientAlphaKey(1f, 0.5f),
                new GradientAlphaKey(0.5f, 1f)
                }
            );

            colorOverLifetime.color = new ParticleSystem.MinMaxGradient(grad);
        }
    }

    public void Collect(Transform player, Action onFinish)
    {
        if (isCollecting) return;
        target = player;
        isCollecting = true;
        finishCallback = onFinish;

        if (colorTween != null && colorTween.IsActive())
            colorTween.Kill();

        StartCoroutine(MoveToPlayer());
    }


    private IEnumerator MoveToPlayer()
    {
        Vector3 start = transform.position;
        Vector3 randomOffset = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            Vector3 curve = Vector3.Lerp(start, target.position, t);
            curve += randomOffset * (1f - t);
            transform.position = curve;
            yield return null;
        }

        finishCallback?.Invoke();
    }
}
