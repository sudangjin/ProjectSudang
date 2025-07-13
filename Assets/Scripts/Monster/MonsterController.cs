using UnityEngine;

[RequireComponent(typeof(MonsterView))]
public class MonsterController : MonoBehaviour
{
    private MonsterModel model;
    private MonsterView view;
    private Transform target;

    public void Initialize(Transform targetTransform, float moveSpeed, float attackRange, int hp)
    {
        target = targetTransform;
        model = new MonsterModel(moveSpeed, attackRange, hp);
        view = GetComponent<MonsterView>();
        view.Init();
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

        if (model.IsDead)
        {
            view.Die();
        }
    }
}
