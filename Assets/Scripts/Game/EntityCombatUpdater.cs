using UnityEngine;

public class EntityCombatUpdater : MonoBehaviour
{
    private IProjectileShooter shooter;

    private void Awake()
    {
        shooter = GetComponent<IProjectileShooter>();
    }

    private void Update()
    {
        shooter?.TryShoot();
    }
}