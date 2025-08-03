using UnityEngine;

public class PlayerModel
{
    public int MaxHP { get; private set; }
    public int CurrentHP { get; private set; }

    public int Level { get; private set; }
    public int CurrentExp { get; private set; }
    public int ExpToNextLevel => Level * 3;

    public bool IsDead => CurrentHP <= 0;

    public PlayerModel(int maxHP)
    {
        Reset(maxHP);
    }

    public void Reset(int maxHP)
    {
        MaxHP = maxHP;
        CurrentHP = maxHP;
        Level = 1;
        CurrentExp = 0;
    }

    public void TakeDamage(int damage)
    {
        CurrentHP = Mathf.Max(CurrentHP - damage, 0);
    }

    public int Heal(int amount)
    {
        var healAmount = Mathf.Min(amount, MaxHP - CurrentHP);
        CurrentHP += healAmount;

        return healAmount;
    }

    public void AddHP(int value)
    {
        var reduceHP = MaxHP - CurrentHP;

        MaxHP = value;
        CurrentHP = value - reduceHP;
    }

    public int AddExp(int amount)
    {
        if (amount <= 0) return 0;

        int need = ExpToNextLevel - CurrentExp;
        int toAdd = Mathf.Min(amount, need);

        CurrentExp += toAdd;
        amount -= toAdd;

        if (CurrentExp >= ExpToNextLevel)
        {
            CurrentExp = 0;
            Level++;
        }

        return amount;
    }
}
