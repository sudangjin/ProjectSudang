using System.Threading;
using UnityEngine;

public static class ProjectileUtility
{
    public static void Fire(
        ProjectileData projectile,
        Vector3 firePosition,
        Vector2 direction,
        int damage,
        int targetLayerMask)
    {
        if (projectile == null)
        {
            Debug.LogWarning("ProjectileUtility.Fire: projectilePrefab is null");
            return;
        }

        string path = $"Prefabs/Projectile/{projectile.PrefabName}";
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/Projectile/{projectile.PrefabName}");
        if (prefab == null)
        {
            Debug.LogError($"[Spawner] Monster prefab '{projectile.PrefabName}' not found!");
            return;
        }

        GameObject projectileObj = ObjectPooler.Instance.Create(prefab, SceneHierarchy.Instance.projectilesParent);

        projectileObj.transform.position = firePosition;
        projectileObj.transform.rotation = Quaternion.identity;

        ProjectileView view = projectileObj.GetComponent<ProjectileView>();

        if (view == null)
        {
            Debug.LogWarning("ProjectileUtility.Fire: ProjectileView component missing on projectile prefab");
            return;
        }

        new ProjectileController(view, projectile, direction.normalized, damage, targetLayerMask, projectileObj);
    }
}
