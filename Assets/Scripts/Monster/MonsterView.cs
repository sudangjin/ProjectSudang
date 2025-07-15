using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MonsterView : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private MonsterController controller;

    [Header("UI")]
    [SerializeField] private UIGauge hpGauge;

    public void Init(MonsterController monsterController)
    {
        controller = monsterController;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (hpGauge != null)
        {
            hpGauge.Init(controller.GetMaxHP());

            bool isFullHP = controller.GetCurrentHP() >= controller.GetMaxHP();
            hpGauge.SetVisibility(!isFullHP);
        }
    }

    public void Move(Vector2 direction, float speed)
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    public void PlayHitEffect()
    {
        CancelInvoke(nameof(RestoreColor));
        Invoke(nameof(RestoreColor), 0.1f);

        if (hpGauge != null)
        {
            hpGauge.UpdateValue(controller.GetCurrentHP());
            hpGauge.SetVisibility(true);
        }
    }

    private void RestoreColor()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;
    }

    public void Die()
    {
        if (hpGauge != null)
            hpGauge.SetVisibility(false);

        Destroy(gameObject);
    }

    public void UpdateHPBarFacing(Transform cameraTransform)
    {
        if (hpGauge != null)
            hpGauge.FaceToCamera(cameraTransform);
    }
}
