using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerView : MonoBehaviour
{
    [SerializeField] private Transform unit;
    [SerializeField] private Transform arrow;
    [SerializeField] private float rotateDuration = 0.15f;

    private bool currentFacingLeft = false;

    public void Init()
    {
    }

    public void UpdateFacingByDirection(Vector2 dir)
    {
        if (unit == null) return;

        bool faceLeft = dir.x < 0f;
        if (faceLeft == currentFacingLeft) return;

        currentFacingLeft = faceLeft;

        Vector3 unitScale = unit.localScale;
        unitScale.x = Mathf.Abs(unitScale.x) * (faceLeft ? 1f : -1f);
        unit.localScale = unitScale;
    }

    public void SetArrowRotation(Vector2 direction, bool instant = false, System.Action onComplete = null)
    {
        if (arrow == null) return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float targetZ = angle - 90f;

        if (instant)
        {
            arrow.rotation = Quaternion.Euler(0f, 0f, targetZ);
            onComplete?.Invoke();
        }
        else
        {
            arrow
                .DORotate(new Vector3(0f, 0f, targetZ), rotateDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => onComplete?.Invoke());
        }
    }

    public void PlayHitEffect()
    {
        // ¿¹½Ã
        Debug.Log("Hit!");
    }

    public void PlayLevelUpEffect()
    {
        Debug.Log("Level Up!");
    }

    public void Die()
    {
        Debug.Log("Player died.");
    }
}
