using System.Data;
using UnityEngine;

[RequireComponent(typeof(MonsterView))]
[RequireComponent(typeof(MonsterMoveController))]
public class MonsterController : MonoBehaviour, IHittable
{
    private MonsterModel model;
    private MonsterView view;
    private Transform target;
    private MonsterMoveController moveController; // ★ 추가

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
        moveController = GetComponent<MonsterMoveController>(); // 있을 수도/없을 수도
    }

    // 외부(Behavior)에서 안전히 접근하도록 래퍼 제공
    public float ModelMoveSpeed() => model != null ? model.MoveSpeed : 0f;
    public float RetreatSpeedMultiplier() => 1f; // 필요시 조정

    public void Initialize(Transform targetTransform, MonsterData monster, int enemyPowerMultiplier)
    {
        target = targetTransform;

        var upgradeStat = UpgradeManager.Instance.AddedUpdateStat;
        model = new MonsterModel(monster, upgradeStat, enemyPowerMultiplier);

        view.Init(this, enemyPowerMultiplier);
        gameObject.SetActive(true);

        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

        var rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = true;

        // ★ MonsterMoveController가 있으면 이동 전략 세팅
        if (moveController != null)
        {
            moveController.SetupBehavior(monster.MoveType, this, target, view);
        }
    }

    public void Update()
    {
        if (target == null || model.IsDead) return;

        if (moveController != null)
        {
            // ★ 전략에 위임
            moveController.Tick();
            return;
        }

        // ★ (호환용) 기본 직선 접근 로직
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
        view.ShowDamage(damage);
        view.UpdateHPGauge(model.CurrentHP, model.MaxHP);

        var upgradeStat = UpgradeManager.Instance.AddedUpdateStat;
        if (model.IsDead)
        {
            DropExpOrb();
            GameSessionManager.Instance.AddPendingExp(model.EXP);
            GameSessionManager.Instance.AddScore(model.Score);
            Player.Instance.Heal(upgradeStat.AddHealOnKill);
            GameSessionManager.Instance.OnEnemyKilled();

            view.Die();
        }
    }

    private void DropExpOrb()
    {
        GameObject prefab = PrefabPreLoader.Instance.GetPrefab(PrefabType.EXP_ORB);
        if (prefab == null) return;

        GameObject orbObj = ObjectPooler.Instance.Create(prefab, transform.position, SceneHierarchy.Instance.expParent);
        var orb = orbObj.GetComponent<ExpOrbComponent>();
        if (orb != null)
        {
            orb.Init(model.EXP, model.Grade);
            GameSessionManager.Instance.RegisterExpOrb(orb);
        }
    }

    public void PlayAttackMotion()
    {
        view.PlayAttackMotion();
    }
}
