using UnityEngine;

public class BulletController
{
    public Vector2 Direction { get; private set; }
    public float Speed { get; private set; }
    public float LifeTime { get; private set; }
    public float Damage { get; private set; }

    private BulletView view;
    private float timer;
    private Vector3 startPosition;
    private float maxDistance = 20f; // 추가: 최대 이동 거리
    private bool isDestroyed = false;

    public BulletController(BulletView view, Vector2 direction, float speed, float lifeTime, float damage)
    {
        this.view = view;
        Direction = direction.normalized;
        Speed = speed;
        LifeTime = lifeTime;
        Damage = damage;

        startPosition = view.transform.position;
        this.view.Init(this);
        BulletUpdater.Instance.Register(this); // 매 프레임 호출 등록
    }

    public void Update()
    {
        if (isDestroyed) return;

        timer += Time.deltaTime;
        float distance = Vector3.Distance(startPosition, view.transform.position);

        if (timer >= LifeTime || distance >= maxDistance)
        {
            DestroyBullet();
            return;
        }

        view.Move(Direction, Speed);
    }

    public void OnHit(MonsterController monster)
    {
        if (isDestroyed) return;

        monster.TakeDamage((int)Damage);
        DestroyBullet();
    }

    private void DestroyBullet()
    {
        isDestroyed = true;
        BulletUpdater.Instance.Unregister(this);
        view.DestroySelf();
    }
}
