using UnityEngine;

[RequireComponent(typeof(EntityCombatUpdater))]
public class PlayerShooter : MonoBehaviour, IProjectileShooter
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;

    [SerializeField] private float fireInterval = 0.5f;

    private float fireTimer = 0f;

    public void TryShoot()
    {
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireInterval)
        {
            FireProjectile();
            fireTimer = 0f;
        }
    }

    private void FireProjectile()
    {
        var projectile = ProjectileData.Get(6);
        Vector2 fireDir = Player.Instance.GetAimDirection();
        ProjectileUtility.Fire(
            projectile,
            firePoint.position,
            fireDir,
            1,
            LayerMask.GetMask("Monster")
        );
    }
}
