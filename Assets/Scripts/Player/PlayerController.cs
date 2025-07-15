using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerView))]
public class PlayerController : MonoBehaviour
{
    private PlayerModel model;
    private PlayerView view;

    [SerializeField] private int maxHP = 100;
    [SerializeField] private UIPlayerLevelInfo levelInfo;

    public int GetLevel() => model.Level;

    public int GetCurrentExp() => model.CurrentExp;

    public int GetExpToNextLevel() => model.ExpToNextLevel;

    private void Awake()
    {
        model = new PlayerModel(maxHP);
        view = GetComponent<PlayerView>();
        view.Init();
    }

    public void GainExp(int amount)
    {
        int prevLevel = model.Level;
        bool leveledUp = model.AddExp(amount);

        if (leveledUp && model.Level != prevLevel)
        {
            view.PlayLevelUpEffect();
            GameEvent.Publish(EventKeys.PlayerLevelChanged, model.Level);
        }

        GameEvent.Publish(EventKeys.PlayerExpChanged, (model.CurrentExp, model.ExpToNextLevel));
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

    public void Heal(int amount) => model.Heal(amount);

    public int GetCurrentHP() => model.CurrentHP;
    public int GetMaxHP() => model.MaxHP;
}
