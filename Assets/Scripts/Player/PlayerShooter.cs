using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(EntityCombatUpdater))]
public class PlayerShooter : MonoBehaviour, IProjectileShooter
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;

    private float fireTimer = 0f;

    public void TryShoot()
    {
        var updateStat = UpgradeManager.Instance.AddedUpdateStat;
        if (updateStat == null) return;

        var weapon = UpgradeManager.Instance.WeaponData;
        var attackSpeed = weapon.AttackSpeed * updateStat.MultiplePlayerAttackSpeed;

        fireTimer += Time.deltaTime;
        if (fireTimer >= 1 / attackSpeed)
        {
            StartCoroutine(FireProjectile());
            fireTimer = 0f;
        }
    }

    private IEnumerator FireProjectile()
    {
        var updateStat = UpgradeManager.Instance.AddedUpdateStat;
        var weapon = UpgradeManager.Instance.WeaponData;

        var burstCount = updateStat.AddShotSameDir + 1;
        for (int shot = 0; shot < burstCount; shot++)
        {
            if (updateStat.AddShotBehindDir)
            {
                Vector2 fireDir = Player.Instance.GetAimDirection();
                ProjectileFactory.Spawn(
                    speed: weapon.Speed * updateStat.MultplePlayerProjectileSpeed,
                    lifeTime: weapon.LifeTime,
                    firePosition: firePoint.position,
                    direction: -fireDir,
                    damage: weapon.Damage + updateStat.AddPlayerDamage,
                    prefabName: weapon.PrefabName,
                    LayerMask.GetMask("Monster")
                );
            }

            if (updateStat.AddShotRandomDir > 0)
            {
                var dirs = Player.Instance.GetMultiAimDirections();

                IEnumerable<Vector2> pickedList = null;

                if (updateStat.NormalizeShotDir)
                {
                    pickedList = dirs.GetRange(2, 1 + updateStat.AddShotRandomDir);
                }
                else
                {
                    pickedList = dirs.OrderBy(x => Random.value).Take(updateStat.AddShotRandomDir + 1);
                }

                foreach (var dir in pickedList)
                {
                    ProjectileFactory.Spawn(
                        speed: weapon.Speed * updateStat.MultplePlayerProjectileSpeed,
                        lifeTime: weapon.LifeTime,
                        firePosition: firePoint.position,
                        direction: dir,
                        damage: weapon.Damage + updateStat.AddPlayerDamage,
                        prefabName: weapon.PrefabName,
                        LayerMask.GetMask("Monster")
                    );
                }
            }
            else
            {
                Vector2 fireDir = Player.Instance.GetAimDirection();
                ProjectileFactory.Spawn(
                    speed: weapon.Speed * updateStat.MultplePlayerProjectileSpeed,
                    lifeTime: weapon.LifeTime,
                    firePosition: firePoint.position,
                    direction: fireDir,
                    damage: weapon.Damage + updateStat.AddPlayerDamage,
                    prefabName: weapon.PrefabName,
                    LayerMask.GetMask("Monster")
                );
            }

            yield return new WaitForSeconds(0.1f / updateStat.MultiplePlayerAttackSpeed);
        }
    }
}
