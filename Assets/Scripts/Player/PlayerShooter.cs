using UnityEngine;

[RequireComponent(typeof(EntityCombatUpdater))]
public class PlayerShooter : MonoBehaviour, IProjectileShooter
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;

    private float fireTimer = 0f;

    public void TryShoot()
    {
        var weaponStat = UpgradeManager.Instance.WeaponStat;
        if (weaponStat == null) return;

        fireTimer += Time.deltaTime;
        if (fireTimer >= 1 / weaponStat.AttackSpeed)
        {
            FireProjectile();
            fireTimer = 0f;
        }
    }

    private void FireProjectile()
    {
        var weaponStat = UpgradeManager.Instance.WeaponStat;
        Vector2 fireDir = Player.Instance.GetAimDirection();
        ProjectileFactory.Spawn(
            speed: weaponStat.Speed,
            lifeTime: weaponStat.LifeTime,
            firePosition: firePoint.position,
            direction: fireDir,
            damage: weaponStat.Damage,
            prefabName: weaponStat.PrefabName,
            LayerMask.GetMask("Monster")
        );
    }
}
