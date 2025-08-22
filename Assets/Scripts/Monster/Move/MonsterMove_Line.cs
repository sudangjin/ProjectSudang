using UnityEngine;

public class MonsterMove_Line : MonoBehaviour, IMonsterMoveBehavior
{
    private MonsterController controller;
    private Transform target;
    private MonsterView view;

    public void Init(MonsterController controller, Transform target, MonsterView view)
    {
        this.controller = controller;
        this.target = target;
        this.view = view;
    }

    public void Tick()
    {
        if (controller == null || controller.IsDead || target == null) return;

        float distance = Vector2.Distance(controller.transform.position, target.position);
        if (distance > controller.AttackRange)
        {
            Vector2 dir = (target.position - controller.transform.position).normalized;
            controller.transform.position += (Vector3)(dir * controller.ModelMoveSpeed() * Time.deltaTime);
            view.UpdateMoveState(true);
        }
        else
        {
            view.UpdateMoveState(false);
        }
    }
}
