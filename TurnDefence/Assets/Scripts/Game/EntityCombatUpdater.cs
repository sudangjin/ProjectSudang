using UnityEngine;

public class EntityCombatUpdater : MonoBehaviour
{
    private IProjectileShooter shooter;

    private void Awake()
    {
        shooter = GetComponent<IProjectileShooter>();
    }

    public void Update()
    {
        shooter?.TryShoot();
    }
}