using UnityEngine;

public class PlayerModel
{
    public int MaxHP { get; private set; }
    public int CurrentHP { get; private set; }

    public bool IsDead => CurrentHP <= 0;

    public PlayerModel(int maxHP)
    {
        MaxHP = maxHP;
        CurrentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        CurrentHP = Mathf.Max(CurrentHP - damage, 0);
    }

    public void Heal(int amount)
    {
        CurrentHP = Mathf.Min(CurrentHP + amount, MaxHP);
    }
}