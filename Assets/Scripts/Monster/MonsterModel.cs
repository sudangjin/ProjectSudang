using UnityEngine;

public class MonsterModel
{
    public int ID { get; private set; }
    public float MoveSpeed { get; private set; }
    public float AttackRange { get; private set; }
    public int MaxHP { get; private set; }
    public int CurrentHP { get; private set; }
    public int Damage { get; private set; }
    public float AttackSpeed { get; private set; }
    public int EXP { get; private set; }
    public long Score { get; private set; }
    public MovementType MoveType { get; private set; }
    public int ProjectileID { get; private set; }
    public int Grade { get; private set; }
    public bool IsBoss { get; private set; }
    public bool IsDead => CurrentHP <= 0;

    public MonsterModel(MonsterData monster)
    {
        ID = monster.ID;
        MoveSpeed = monster.MoveSpeed;
        AttackRange = monster.AttackRange;
        MaxHP = monster.HP;
        CurrentHP = monster.HP;
        Damage = monster.Damage;
        AttackSpeed = monster.AttackSpeed;
        EXP = monster.EXP;
        Score = monster.Score;
        MoveType = monster.MoveType;
        ProjectileID = monster.ProjectileID;
        Grade = monster.Grade;
        IsBoss = monster.IsBoss;
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
