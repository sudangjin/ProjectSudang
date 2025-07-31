using UnityEngine;

public class ProjectileController
{
    public Vector2 Direction { get; private set; }
    public float Speed { get; private set; }
    public float LifeTime { get; private set; }
    public float Damage { get; private set; }
    public GameObject PrefabReference { get; set; }

    private ProjectileView view;
    private float timer;
    private Vector3 startPosition;
    private float maxDistance = 20f;
    private bool isDestroyed = false;
    private LayerMask targetMask;

    public ProjectileController(ProjectileView view, ProjectileData projectile, Vector2 direction, float damage, LayerMask targetMask, GameObject prefabRef)
    {
        this.view = view;
        Direction = direction.normalized;
        Speed = projectile.Speed;
        LifeTime = projectile.LifeTime;
        Damage = damage;
        this.targetMask = targetMask;
        PrefabReference = prefabRef;

        startPosition = view.transform.position;
        this.view.Init(this);
        ProjectileUpdater.Instance.Register(this);
    }

    public void Update()
    {
        if (isDestroyed) return;

        timer += Time.deltaTime;
        float distance = Vector3.Distance(startPosition, view.transform.position);

        if (timer >= LifeTime || distance >= maxDistance)
        {
            DestroyProjectile();
            return;
        }

        view.transform.position += (Vector3)(Direction * Speed * Time.deltaTime);
    }

    public void OnHit(IHittable target)
    {
        if (isDestroyed) return;
        target.TakeDamage((int)Damage);
        DestroyProjectile();
    }

    private void DestroyProjectile()
    {
        if (isDestroyed) return;
        isDestroyed = true;

        ProjectileUpdater.Instance.Unregister(this);
        view.ReleaseToPool(PrefabReference);
    }

    public bool IsTargetLayer(GameObject obj)
    {
        return ((1 << obj.layer) & targetMask) != 0;
    }
}
