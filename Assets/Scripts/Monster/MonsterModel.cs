using UnityEngine;

public class MonsterModel
{
    public float MoveSpeed { get; private set; }
    public float AttackRange { get; private set; }

    public int MaxHP { get; private set; }
    public int CurrentHP { get; private set; }

    public bool IsDead => CurrentHP <= 0;

    public MonsterModel(float moveSpeed, float attackRange, int maxHP)
    {
        MoveSpeed = moveSpeed;
        AttackRange = attackRange;
        MaxHP = maxHP;
        CurrentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        if (IsDead) return;

        CurrentHP = Mathf.Max(CurrentHP - damage, 0);
    }

    public void Heal(int amount)
    {
        if (IsDead) return;

        CurrentHP = Mathf.Min(CurrentHP + amount, MaxHP);
    }
}
