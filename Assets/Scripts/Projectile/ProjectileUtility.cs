using UnityEngine;

public static class ProjectileUtility
{
    public static void Fire(
        GameObject projectilePrefab,
        Vector3 firePosition,
        Vector2 direction,
        float speed,
        float lifetime,
        int damage,
        int targetLayerMask)
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("ProjectileUtility.Fire: projectilePrefab is null");
            return;
        }

        GameObject projectileObj = Object.Instantiate(projectilePrefab, firePosition, Quaternion.identity);
        ProjectileView view = projectileObj.GetComponent<ProjectileView>();

        if (view == null)
        {
            Debug.LogWarning("ProjectileUtility.Fire: ProjectileView component missing on projectile prefab");
            return;
        }

        new ProjectileController(view, direction.normalized, speed, lifetime, damage, targetLayerMask);
    }
}
