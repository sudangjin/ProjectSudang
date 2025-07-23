using UnityEngine;
using System.Collections;

public class MonsterView : MonoBehaviour
{
    private MonsterController controller;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private UIGauge hpGauge;

    private Transform playerTransform;

    private MaterialPropertyBlock propertyBlock;
    private static readonly int ColorID = Shader.PropertyToID("_Color");

    private Coroutine flashCoroutine;

    private void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
    }

    public void Init(MonsterController monsterController)
    {
        controller = monsterController;

        if (hpGauge != null)
        {
            hpGauge.Init(controller.MaxHP);
            hpGauge.SetVisibility(controller.CurrentHP < controller.MaxHP);
        }

        if (Player.Instance != null)
        {
            playerTransform = Player.Instance.transform;
        }
    }

    private void Update()
    {
        if (Camera.main != null)
            UpdateHPBarFacing(Camera.main.transform);

        if (playerTransform != null)
            FlipByPlayerPosition(playerTransform.position);
    }

    public void Move(Vector2 direction, float speed)
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    public void PlayHitEffect()
    {
        if (spriteRenderer == null) return;

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashWhite());
    }

    public void UpdateHPGauge(int current, int max)
    {
        if (hpGauge == null) return;

        hpGauge.UpdateValue(current);
        hpGauge.SetVisibility(current < max);
    }

    private IEnumerator FlashWhite()
    {
        spriteRenderer.GetPropertyBlock(propertyBlock);
        Color originalColor = spriteRenderer.color;

        propertyBlock.SetColor(ColorID, Color.white * 2f);
        spriteRenderer.SetPropertyBlock(propertyBlock);

        yield return new WaitForSeconds(0.1f);

        propertyBlock.SetColor(ColorID, originalColor);
        spriteRenderer.SetPropertyBlock(propertyBlock);
    }

    public void Die()
    {
        animator.SetTrigger("Death");

        if (hpGauge != null)
            hpGauge.SetVisibility(false);

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) Destroy(col);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) Destroy(rb);
    }

    public void OnDeathAnimationEnd()
    {
        Destroy(gameObject);
    }

    public void UpdateHPBarFacing(Transform cameraTransform)
    {
        if (hpGauge != null)
            hpGauge.FaceToCamera(cameraTransform);
    }

    private void FlipByPlayerPosition(Vector3 playerPosition)
    {
        if (spriteRenderer == null) return;

        bool isRightSide = transform.position.x > playerPosition.x;
        spriteRenderer.flipX = isRightSide;
    }
}
