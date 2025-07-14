using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MonsterView : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private MonsterController controller;

    public void Init(MonsterController monsterController)
    {
        controller = monsterController;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Move(Vector2 direction, float speed)
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    public void PlayHitEffect()
    {
        //spriteRenderer.color = Color.red;
        CancelInvoke(nameof(RestoreColor));
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
