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

    public MonsterModel(MonsterData monster, UpgradeStat upgradeStat)
    {
        var nowWave = GameSessionManager.Instance.Wave;
        var config = GameSessionManager.Instance.Config;

        var monsterHP = monster.HP * (1f + ((nowWave - 1) * config.monsterHPPerWave));
        var monsterDamage = monster.Damage * (1f + ((nowWave - 1) * config.monsterDamagePerWave));
        var monsterScore = monster.Score * (1f + ((nowWave - 1) * config.monsterScorePerWave));

        ID = monster.ID;
        MoveSpeed = monster.MoveSpeed * upgradeStat.MultipleEnemyMovementSpeed;
        AttackRange = monster.AttackRange * upgradeStat.MultipleEnemyAttackRange;
        MaxHP = (int)(monsterHP * upgradeStat.MultipleEnemyHP);
        CurrentHP = MaxHP;
        Damage = (int)(monsterDamage * upgradeStat.MultipleEnemyDamage);
        AttackSpeed = monster.AttackSpeed * upgradeStat.MultipleEnemyAttackSpeed;
        EXP = monster.EXP;
        Score = (long)(monsterScore * UpgradeManager.Instance.AddScore);
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
