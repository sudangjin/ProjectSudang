using UnityEngine;
using System.Collections;
using TMPro;

public class MonsterView : MonoBehaviour
{
    private MonsterController controller;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private UIGauge hpGauge;
    [SerializeField] private TextMeshProUGUI textPowerMultiplier;

    private Transform playerTransform;
    private MaterialPropertyBlock propertyBlock;
    private static readonly int ColorID = Shader.PropertyToID("_Color");
    private Coroutine flashCoroutine;

    private void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
    }

    public void Init(MonsterController monsterController, float enemyPowerMultiplier)
    {
        controller = monsterController;

        if (hpGauge != null)
        {
            hpGauge.Init();
            hpGauge.SetVisibility(controller.CurrentHP < controller.MaxHP);
        }

        if (Player.Instance != null)
            playerTransform = Player.Instance.transform;

        if (textPowerMultiplier != null)
        { 
            textPowerMultiplier.text = enemyPowerMultiplier > 1f ? 
                $"x{enemyPowerMultiplier:F1}" : 
                string.Empty;
        }
    }

    public void Update()
    {
        if (Camera.main != null)
            UpdateHPBarFacing(Camera.main.transform);

        if (playerTransform != null)
            FlipByPlayerPosition(playerTransform.position);
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
        hpGauge.UpdateValue(current, max);
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
        if (col != null) col.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;
    }

    public void OnDeathAnimationEnd()
    {
        if (controller != null)
            ObjectPooler.Instance.Release(controller.PrefabReference, gameObject, SceneHierarchy.Instance.monstersParent);
    }

    public void UpdateHPBarFacing(Transform cameraTransform)
    {
        if (hpGauge != null)
            hpGauge.FaceToCamera(cameraTransform);
    }

    public void UpdateMoveState(bool isMove)
    {
        animator.SetBool("IsMove", isMove);
    }

    public void PlayAttackMotion()
    {
        animator.SetTrigger("Attack");
    }

    private void FlipByPlayerPosition(Vector3 playerPosition)
    {
        if (spriteRenderer == null) return;
        bool isRightSide = transform.position.x > playerPosition.x;
        spriteRenderer.flipX = isRightSide;
    }

    public void ShowDamage(int damage)
    {
        var prefab = PrefabPreLoader.Instance.GetPrefab(PrefabType.DAMAGE_TEXT);
        if (prefab == null) return;

        Vector3 spawnPos = transform.position + new Vector3(Random.Range(-0.3f, 0.3f), 0.8f, 0);
        
        GameObject obj = ObjectPooler.Instance.Create(prefab, spawnPos, SceneHierarchy.Instance.damageTextParent);
        var dmg = obj.GetComponent<DamageText>();
        dmg.Show(damage, GameSessionManager.Instance.Config.monsterHit, spawnPos);
    }
}
