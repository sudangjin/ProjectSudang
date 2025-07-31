using UnityEngine;

[RequireComponent(typeof(EntityCombatUpdater))]
public class MonsterShooter : MonoBehaviour, IProjectileShooter
{
    [SerializeField] private Transform firePoint;

    private float fireCooldownTimer = 0f;
    private MonsterController controller;

    private void Awake()
    {
        controller = GetComponent<MonsterController>();
    }

    public void TryShoot()
    {
        if (controller == null || controller.IsDead)
            return;

        fireCooldownTimer += Time.deltaTime;

        if (fireCooldownTimer < controller.AttackSpeed)
            return;

        if (!IsTargetInAttackRange())
            return;

        FireAtTarget();
        fireCooldownTimer = 0f;
    }

    private bool IsTargetInAttackRange()
    {
        var player = Player.Instance;
        if (player == null) return false;

        float distance = Vector3.Distance(transform.position, player.transform.position);
        return distance <= controller.AttackRange;
    }

    private void FireAtTarget()
    {
        var player = Player.Instance;
        if (player == null) return;

        var projectileData = ProjectileData.Get(controller.ProjectileID);
        if (projectileData == null) return;

        Vector3 shootOrigin = firePoint != null ? firePoint.position : transform.position;
        Vector2 direction = (player.transform.position - shootOrigin).normalized;

        ProjectileFactory.Spawn(projectileData, shootOrigin, direction, controller.Damage, LayerMask.GetMask("Player"));
        controller.PlayAttackMotion();
    }
}
