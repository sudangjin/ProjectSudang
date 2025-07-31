using UnityEngine;

public class MonsterModel
{
    public float MoveSpeed { get; private set; }
    public float AttackRange { get; private set; }
    public int MaxHP { get; private set; }
    public int CurrentHP { get; private set; }
    public int Damage { get; private set; }
    public float AttackSpeed { get; private set; }
    public int EXP { get; private set; }
    public long Score { get; private set; }
    public MonsterData.MovementType MoveType { get; private set; }
    public int ProjectileID { get; private set; }
    public bool IsBoss { get; private set; }
    public bool IsDead => CurrentHP <= 0;

    public MonsterModel(float moveSpeed, float attackRange, int maxHP, int damage, float attackSpeed, int exp, long score, MonsterData.MovementType moveType, int projectileID, bool isBoss)
    {
        MoveSpeed = moveSpeed;
        AttackRange = attackRange;
        MaxHP = maxHP;
        CurrentHP = maxHP;
        Damage = damage;
        AttackSpeed = attackSpeed;
        EXP = exp;
        Score = score;
        MoveType = moveType;
        ProjectileID = projectileID;
        IsBoss = isBoss;
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
