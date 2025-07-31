using UnityEngine;

[RequireComponent(typeof(MonsterView))]
public class MonsterController : MonoBehaviour, IHittable
{
    private MonsterModel model;
    private MonsterView view;
    private Transform target;

    public GameObject PrefabReference { get; set; }

    public bool IsDead => model.IsDead;
    public float AttackRange => model.AttackRange;
    public int CurrentHP => model.CurrentHP;
    public int MaxHP => model.MaxHP;
    public int ProjectileID => model.ProjectileID;
    public int Damage => model.Damage;
    public float AttackSpeed => model.AttackSpeed;
    public Transform GetTransform() => transform;

    private void Awake()
    {
        view = GetComponent<MonsterView>();
    }

    public void Initialize(
        Transform targetTransform,
        float moveSpeed,
        float attackRange,
        int hp,
        int damage,
        float attackSpeed,
        int exp,
        long score,
        MonsterData.MovementType moveType,
        int projectileID,
        bool isBoss)
    {
        target = targetTransform;

        model = new MonsterModel(
            moveSpeed: moveSpeed,
            attackRange: attackRange,
            maxHP: hp,
            damage: damage,
            attackSpeed: attackSpeed,
            exp: exp,
            score: score,
            moveType: moveType,
            projectileID: projectileID,
            isBoss: isBoss
            );

        view.Init(this);
        gameObject.SetActive(true);

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
            transform.position += (Vector3)(direction * model.MoveSpeed * Time.deltaTime);
            view.UpdateMoveState(true);
        }
        else
        {
            view.UpdateMoveState(false);
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
            Player.Instance.GainExp(model.EXP);
            GameSessionManager.Instance.AddScore(model.Score);
            GameSessionManager.Instance.OnEnemyKilled();
            view.Die();
        }
    }

    public void PlayAttackMotion()
    {
        view.PlayAttackMotion();
    }
}
