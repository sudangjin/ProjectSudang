using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MonsterView : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public void Init()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Move(Vector2 direction, float speed)
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    public void UpdateSortingOrder()
    {
        spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }

    public void PlayHitEffect()
    {
        spriteRenderer.color = Color.red;
        Invoke(nameof(RestoreColor), 0.1f);
    }

    private void RestoreColor()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
