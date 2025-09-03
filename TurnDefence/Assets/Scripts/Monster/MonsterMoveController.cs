using UnityEngine;

public class MonsterMoveController : MonoBehaviour
{
    private IMonsterMoveBehavior behavior;

    public void SetupBehavior(MovementType type, MonsterController controller, Transform target, MonsterView view)
    {
        foreach (var b in GetComponents<IMonsterMoveBehavior>())
        {
            if (b is Component c) Destroy(c);
        }

        switch (type)
        {
            default:
            case MovementType.LINE:
                behavior = gameObject.AddComponent<MonsterMove_Line>();
                break;
            case MovementType.JUMP:
                behavior = gameObject.AddComponent<MonsterMove_Jump>();
                break;
            case MovementType.AROUND:
                behavior = gameObject.AddComponent<MonsterMove_Around>();
                break;
            case MovementType.CURVE:
                behavior = gameObject.AddComponent<MonsterMove_Curve>();
                break;
        }

        behavior.Init(controller, target, view);
    }

    public void Tick()
    {
        behavior?.Tick();
    }
}
