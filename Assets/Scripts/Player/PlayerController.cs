using UnityEngine;

[RequireComponent(typeof(PlayerView))]
public class PlayerController : MonoBehaviour
{
    private PlayerModel model;
    private PlayerView view;

    [SerializeField] private int maxHP = 100;

    private void Awake()
    {
        model = new PlayerModel(maxHP);
        view = GetComponent<PlayerView>();
        view.Init();
    }

    public void TakeDamage(int damage)
    {
        if (model.IsDead) return;

        model.TakeDamage(damage);
        view.PlayHitEffect();

        if (model.IsDead)
        {
            view.Die();
            GameManager.Instance.GameOver();
        }
    }

    public void Heal(int amount)
    {
        model.Heal(amount);
    }

    public int GetCurrentHP() => model.CurrentHP;
    public int GetMaxHP() => model.MaxHP;
}
