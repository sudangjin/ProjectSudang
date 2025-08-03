using System.Data;
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
        Transform targetTransform, MonsterData monster)
    {
        target = targetTransform;

        var upgradeStat = UpgradeManager.Instance.AddedUpdateStat;
        model = new MonsterModel(monster, upgradeStat);

        view.Init(this);
        gameObject.SetActive(true);

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = true;
    }

    public void Update()
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
