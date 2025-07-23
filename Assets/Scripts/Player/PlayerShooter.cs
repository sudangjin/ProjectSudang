using UnityEngine;

[RequireComponent(typeof(EntityCombatUpdater))]
public class PlayerShooter : MonoBehaviour, IProjectileShooter
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;

    [SerializeField] private float projectileSpeed = 5f;
    [SerializeField] private float projectileLifeTime = 5f;
    [SerializeField] private int projectileDamage = 1;
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
        Vector2 fireDir = Player.Instance.GetAimDirection();
        ProjectileUtility.Fire(
            projectilePrefab,
            firePoint.position,
            fireDir,
            projectileSpeed,
            projectileLifeTime,
            projectileDamage,
            LayerMask.GetMask("Monster")
        );
    }
}
