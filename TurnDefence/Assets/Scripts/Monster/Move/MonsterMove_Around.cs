using UnityEngine;

public class MonsterMove_Around : MonoBehaviour, IMonsterMoveBehavior
{
    private MonsterController controller;
    private Transform target;
    private MonsterView view;

    [SerializeField] private float radiusMultiplier = 0.9f;
    [SerializeField] private float approachEpsilon = 0.05f;
    [SerializeField] private bool clockwise = false;

    private enum State { Approach, Orbit }
    private State state = State.Approach;

    private float desiredRadius;
    private float angleDeg;

    public void Init(MonsterController controller, Transform target, MonsterView view)
    {
        this.controller = controller;
        this.target = target;
        this.view = view;

        desiredRadius = controller.AttackRange * radiusMultiplier;

        Vector2 fromTarget = controller.transform.position - target.position;
        if (fromTarget.sqrMagnitude < 0.0001f)
            fromTarget = Vector2.right;

        angleDeg = Mathf.Atan2(fromTarget.y, fromTarget.x) * Mathf.Rad2Deg;
        state = State.Approach;
    }

    public void Tick()
    {
        if (controller == null || controller.IsDead || target == null) return;

        switch (state)
        {
            case State.Approach:
                TickApproach();
                break;
            case State.Orbit:
                TickOrbit();
                break;
        }
    }

    private void TickApproach()
    {
        Vector2 toTarget = (Vector2)(target.position - controller.transform.position);
        float dist = toTarget.magnitude;

        if (Mathf.Abs(dist - desiredRadius) > approachEpsilon)
        {
            float dirSign = (dist > desiredRadius) ? +1f : -1f;
            Vector2 radialDir = (dist > 0.001f) ? (toTarget / dist) : Vector2.right;
            Vector2 step = radialDir * dirSign * controller.ModelMoveSpeed() * Time.deltaTime;

            float nextDist = (toTarget - step).magnitude;
            if (Mathf.Abs(nextDist - desiredRadius) > Mathf.Abs(dist - desiredRadius))
            {
                Vector2 fromTarget = (Vector2)(controller.transform.position - target.position);
                Vector2 snapDir = (fromTarget.sqrMagnitude > 0.0001f) ? fromTarget.normalized : Vector2.right;
                controller.transform.position = target.position + (Vector3)(snapDir * desiredRadius);
            }
            else
            {
                controller.transform.position += (Vector3)step;
            }

            view.UpdateMoveState(true);
            Vector2 fromTarget2 = controller.transform.position - target.position;
            angleDeg = Mathf.Atan2(fromTarget2.y, fromTarget2.x) * Mathf.Rad2Deg;
            return;
        }

        state = State.Orbit;
        view.UpdateMoveState(false);
    }

    private void TickOrbit()
    {
        Vector3 center = target.position;
        float moveSpeed = controller.ModelMoveSpeed();
        float dir = clockwise ? -1f : +1f;

        // 이동속도 기반 각속도 (rad/sec)
        float angleDeltaRad = (moveSpeed * Time.deltaTime) / desiredRadius;
        angleDeg += dir * angleDeltaRad * Mathf.Rad2Deg;

        float rad = angleDeg * Mathf.Deg2Rad;
        Vector2 orbitPos = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * desiredRadius;
        controller.transform.position = center + (Vector3)orbitPos;

        view.UpdateMoveState(true);
    }
}
