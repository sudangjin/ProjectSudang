using UnityEngine;

public class MonsterMove_Curve : MonoBehaviour, IMonsterMoveBehavior
{
    private MonsterController controller;
    private Transform target;
    private MonsterView view;

    [SerializeField] private float curveFactor = 0.5f;
    [SerializeField] private float minLateral = 0.5f;
    [SerializeField] private float maxLateral = 3.0f;
    [SerializeField] private bool clockwise = false;
    [SerializeField] private float stopEpsilon = 0.02f;

    public void Init(MonsterController controller, Transform target, MonsterView view)
    {
        this.controller = controller;
        this.target = target;
        this.view = view;
    }

    public void Tick()
    {
        if (controller == null || controller.IsDead || target == null) return;

        Vector2 pos = controller.transform.position;
        Vector2 tpos = target.position;
        Vector2 toTarget = tpos - pos;
        float dist = toTarget.magnitude;

        if (dist <= controller.AttackRange - stopEpsilon)
        {
            view.UpdateMoveState(false);
            return;
        }

        Vector2 dir = toTarget / Mathf.Max(dist, 0.0001f);
        Vector2 perp = clockwise ? new Vector2(-dir.y, dir.x) : new Vector2(dir.y, -dir.x);

        float lateral = Mathf.Clamp(dist * curveFactor, minLateral, maxLateral);
        Vector2 aim = tpos + perp * lateral;

        Vector2 moveDir = ((Vector2)aim - pos).normalized;
        controller.transform.position += (Vector3)(moveDir * controller.ModelMoveSpeed() * Time.deltaTime);

        view.UpdateMoveState(true);
    }
}
