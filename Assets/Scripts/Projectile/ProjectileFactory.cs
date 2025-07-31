using UnityEngine;

public static class ProjectileFactory
{
    public static void Spawn(ProjectileData projectile, Vector3 firePosition, Vector2 direction, int damage, int targetLayerMask)
    {
        if (projectile == null) return;

        GameObject prefab = Resources.Load<GameObject>($"Prefabs/Projectile/{projectile.PrefabName}");
        if (prefab == null) return;

        GameObject projectileObj = ObjectPooler.Instance.Create(prefab, SceneHierarchy.Instance.projectilesParent);
        projectileObj.transform.position = firePosition;
        projectileObj.transform.rotation = Quaternion.identity;

        ProjectileView view = projectileObj.GetComponent<ProjectileView>();
        if (view == null) return;

        new ProjectileController(view, projectile, direction, damage, targetLayerMask, prefab);
    }
}
