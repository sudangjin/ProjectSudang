using UnityEngine;

public class MonsterMove_Jump : MonoBehaviour, IMonsterMoveBehavior
{
    private MonsterController controller;
    private Transform target;
    private MonsterView view;

    [SerializeField] private float approachBuffer = 0.05f;
    [SerializeField] private float retreatDistance = 4f;
    [SerializeField] private float retreatDuration = 0.5f;
    [SerializeField] private float reengageDelay = 0.2f;

    private enum State { Approach, AttackWindow, Retreat, Cooldown }
    private State state = State.Approach;
    private float stateTimer = 0f;
    private Vector2 lastAttackDir = Vector2.zero;
    private Vector3 retreatStartPos;
    private Vector3 retreatEndPos;

    public void Init(MonsterController controller, Transform target, MonsterView view)
    {
        this.controller = controller;
        this.target = target;
        this.view = view;
        state = State.Approach;
        stateTimer = 0f;
        lastAttackDir = Vector2.zero;
    }

    public void Tick()
    {
        if (controller == null || controller.IsDead || target == null) return;

        stateTimer += Time.deltaTime;

        switch (state)
        {
            case State.Approach:
                DoApproach();
                break;

            case State.AttackWindow:
                view.UpdateMoveState(false);

                SetupRetreat();
                state = State.Retreat;
                stateTimer = 0f;
                break;

            case State.Retreat:
                DoRetreat();
                break;

            case State.Cooldown:
                view.UpdateMoveState(false);
                if (stateTimer >= reengageDelay)
                {
                    state = State.Approach;
                    stateTimer = 0f;
                }
                break;
        }
    }

    private void DoApproach()
    {
        float distance = Vector2.Distance(controller.transform.position, target.position);
        if (distance > controller.AttackRange - approachBuffer)
        {
            Vector2 dir = (target.position - controller.transform.position).normalized;
            controller.transform.position += (Vector3)(dir * controller.ModelMoveSpeed() * Time.deltaTime);
            view.UpdateMoveState(true);
        }
        else
        {
            lastAttackDir = (target.position - controller.transform.position).normalized;
            controller.PlayAttackMotion();
            state = State.AttackWindow;
            stateTimer = 0f;
        }
    }

    private void SetupRetreat()
    {
        retreatStartPos = controller.transform.position;
        Vector3 retreatDir = (lastAttackDir == Vector2.zero) ? Vector3.zero : -(Vector3)lastAttackDir;
        float mult = controller.RetreatSpeedMultiplier();
        retreatEndPos = retreatStartPos + (retreatDir.normalized * retreatDistance * mult);
    }

    private void DoRetreat()
    {
        if (retreatDuration <= 0f)
        {
            controller.transform.position = retreatEndPos;
            state = State.Cooldown;
            stateTimer = 0f;
            return;
        }

        float t = Mathf.Clamp01(stateTimer / retreatDuration);
        controller.transform.position = Vector3.Lerp(retreatStartPos, retreatEndPos, t);
        view.UpdateMoveState(true);

        if (t >= 1f)
        {
            state = State.Cooldown;
            stateTimer = 0f;
        }
    }
}
