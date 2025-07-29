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

    public void Heal(int amount)
    {
        CurrentHP = Mathf.Min(CurrentHP + amount, MaxHP);
    }

    public bool AddExp(int amount)
    {
        if (amount <= 0) return false;

        CurrentExp += amount;
        bool leveledUp = false;

        while (CurrentExp >= ExpToNextLevel)
        {
            CurrentExp -= ExpToNextLevel;
            Level++;
            leveledUp = true;
        }

        return leveledUp;
    }
}
