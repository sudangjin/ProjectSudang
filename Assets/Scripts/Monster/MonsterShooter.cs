using UnityEngine;

[RequireComponent(typeof(EntityCombatUpdater))]
public class MonsterShooter : MonoBehaviour, IProjectileShooter
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 3f;
    [SerializeField] private float projectileLifeTime = 3f;
    [SerializeField] private int projectileDamage = 1;
    [SerializeField] private float fireCooldown = 1.5f;

    private float fireCooldownTimer = 0f;
    private MonsterController controller;

    private void Awake()
    {
        controller = GetComponent<MonsterController>();
        if (controller == null)
            Debug.LogWarning("MonsterShooter: MonsterController가 없습니다.");
    }

    public void TryShoot()
    {
        if (controller == null || controller.IsDead)
            return;

        fireCooldownTimer += Time.deltaTime;

        if (fireCooldownTimer < fireCooldown)
            return;

        if (!IsTargetInAttackRange())
            return;

        FireAtTarget();
        fireCooldownTimer = 0f;
    }

    private bool IsTargetInAttackRange()
    {
        var player = Player.Instance;
        if (player == null || controller == null) return false;

        float distance = Vector3.Distance(transform.position, player.transform.position);
        return distance <= controller.AttackRange;
    }

    private void FireAtTarget()
    {
        var player = Player.Instance;
        if (player == null || projectilePrefab == null) return;

        Vector3 shootOrigin = firePoint != null ? firePoint.position : transform.position;
        Vector2 direction = (player.transform.position - shootOrigin).normalized;

        ProjectileUtility.Fire(
            projectilePrefab,
            shootOrigin,
            direction,
            projectileSpeed,
            projectileLifeTime,
            projectileDamage,
            LayerMask.GetMask("Player")
        );
    }
}
