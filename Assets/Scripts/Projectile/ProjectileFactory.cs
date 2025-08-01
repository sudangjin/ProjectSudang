using UnityEngine;

public static class ProjectileFactory
{
    public static void Spawn(float speed, float lifeTime, Vector3 firePosition, Vector2 direction, int damage, string prefabName, int targetLayerMask)
    {
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/Projectile/{prefabName}");
        if (prefab == null) return;

        GameObject projectileObj = ObjectPooler.Instance.Create(prefab, SceneHierarchy.Instance.projectilesParent);
        projectileObj.transform.position = firePosition;
        projectileObj.transform.rotation = Quaternion.identity;

        ProjectileView view = projectileObj.GetComponent<ProjectileView>();
        if (view == null) return;

        new ProjectileController(view, speed, lifeTime, direction, damage, targetLayerMask, prefab);
    }
}
