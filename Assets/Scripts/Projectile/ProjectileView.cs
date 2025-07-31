using UnityEngine;

public class ProjectileView : MonoBehaviour
{
    private ProjectileController controller;

    public void Init(ProjectileController controller)
    {
        this.controller = controller;
        RotateToDirection(controller.Direction);
    }

    public void ReleaseToPool(GameObject prefab)
    {
        ObjectPooler.Instance.Release(prefab, gameObject, SceneHierarchy.Instance.projectilesParent);
    }

    private void RotateToDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!controller.IsTargetLayer(collision.gameObject)) return;

        if (collision.TryGetComponent(out IHittable target))
            controller.OnHit(target);
    }
}
