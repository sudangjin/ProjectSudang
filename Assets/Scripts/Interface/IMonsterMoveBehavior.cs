using UnityEngine;

public interface IMonsterMoveBehavior
{
    void Init(MonsterController controller, Transform target, MonsterView view);
    void Tick();
}