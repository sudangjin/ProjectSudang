using UnityEngine;
using System.Collections;
using DG.Tweening;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerView : MonoBehaviour
{
    [SerializeField] private Transform unit;
    [SerializeField] private Transform dirStick;

    [SerializeField] private UIGauge hpGauge;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public PlayerController Controller { get; private set; }

    private bool currentFacingLeft = false;
    private Coroutine flashCoroutine;
    private MaterialPropertyBlock propertyBlock;
    private static readonly int ColorID = Shader.PropertyToID("_Color");

    private void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
    }

    public void Init(PlayerController controller)
    {
        Controller = controller;
        if (Controller == null) return;

        if (hpGauge != null)
        {
            int max = Controller.GetMaxHP();
            int current = Controller.GetCurrentHP();
            hpGauge.Init();
            hpGauge.UpdateValue(current, max);
            hpGauge.SetProgress(current, max);
        }

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
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
        if (dirStick == null) return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float targetZ = angle - 90f;

        if (instant)
        {
            dirStick.rotation = Quaternion.Euler(0f, 0f, targetZ);
            onComplete?.Invoke();
        }
        else
        {
            var upgradeStat = UpgradeManager.Instance.AddedUpdateStat;
            dirStick
                .DORotate(new Vector3(0f, 0f, targetZ), Controller.Character.TurnSpeed * upgradeStat.MultipleTurnSpeed)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => onComplete?.Invoke());
        }
    }

    public void PlayHitEffect()
    {
        if (spriteRenderer == null) return;

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashWhite());
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

    public void UpdateHPGauge(int current, int max)
    {
        if (hpGauge == null) return;

        hpGauge.Init();
        hpGauge.UpdateValue(current, max);
        hpGauge.SetProgress(current, max);
    }

    public void Die()
    {
        Debug.Log("Player died.");
    }

    public void PlayLevelUpEffect()
    {
        Debug.Log("Level Up!");
    }

    public void ShowDamage(int damage, bool isHeal)
    {
        var prefab = PrefabPreLoader.Instance.GetPrefab(PrefabType.DAMAGE_TEXT);
        if (prefab == null) return;

        Vector3 spawnPos = transform.position + new Vector3(Random.Range(-0.3f, 0.3f), 0.8f, 0);

        GameObject obj = ObjectPooler.Instance.Create(prefab, spawnPos, SceneHierarchy.Instance.damageTextParent);
        var dmg = obj.GetComponent<DamageText>();
        var color = isHeal ? GameSessionManager.Instance.Config.playerHeal : GameSessionManager.Instance.Config.playerHit;
        dmg.Show(damage, color, spawnPos);
    }
}
