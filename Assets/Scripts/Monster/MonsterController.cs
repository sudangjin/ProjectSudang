using UnityEngine;

[RequireComponent(typeof(MonsterView))]
public class MonsterController : MonoBehaviour, IHittable
{
    private MonsterModel model;
    private MonsterView view;
    private Transform target;

    public GameObject OriginalPrefab { get; private set; } // 풀 반환 시 필요

    public bool IsDead => model.IsDead;
    public float AttackRange => model.AttackRange;
    public int CurrentHP => model.CurrentHP;
    public int MaxHP => model.MaxHP;
    public Transform GetTransform() => transform;

    public void Initialize(Transform targetTransform, float moveSpeed, float attackRange, int hp, GameObject prefabRef)
    {
        target = targetTransform;
        model = new MonsterModel(moveSpeed, attackRange, hp);

        view = GetComponent<MonsterView>();
        view.Init(this);

        OriginalPrefab = prefabRef;
        gameObject.SetActive(true);

        // 사망 후 비활성화된 Collider/Rigidbody 복원
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = true;
    }

    private void Update()
    {
        if (target == null || model.IsDead) return;

        float distance = Vector2.Distance(transform.position, target.position);

        if (distance > model.AttackRange)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            view.Move(direction, model.MoveSpeed);
        }
    }

    public void TakeDamage(int damage)
    {
        if (model.IsDead) return;

        model.TakeDamage(damage);
        view.PlayHitEffect();
        view.UpdateHPGauge(model.CurrentHP, model.MaxHP);

        if (model.IsDead)
        {
            Player.Instance.GainExp(1);
            GameSessionManager.Instance.AddScore(10);
            GameSessionManager.Instance.OnEnemyKilled();

            view.Die();
        }
    }
}
