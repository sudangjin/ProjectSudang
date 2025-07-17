using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerView))]
public class PlayerController : MonoBehaviour
{
    private PlayerModel model;
    private PlayerView view;

    [Header("Stats")]
    [SerializeField] private int maxHP = 100;

    [Header("Aiming")]
    [SerializeField] private KeyCode rotateLeftKey = KeyCode.LeftArrow;
    [SerializeField] private KeyCode rotateRightKey = KeyCode.RightArrow;

    private int nowDir = 0;
    private IReadOnlyList<Vector2> directions;
    private bool isRotating = false;

    public int GetLevel() => model.Level;
    public int GetCurrentExp() => model.CurrentExp;
    public int GetExpToNextLevel() => model.ExpToNextLevel;
    public int GetCurrentDirectionIndex() => nowDir;

    private void Awake()
    {
        model = new PlayerModel(maxHP);
        view = GetComponent<PlayerView>();
        view.Init();
    }

    private void Start()
    {
        directions = MonsterSpawner.Instance?.SpawnDirections;
        if (directions == null || directions.Count == 0)
        {
            Debug.LogError("SpawnDirections가 비어있습니다.");
            directions = new List<Vector2> { Vector2.left };
        }

        view.SetArrowRotation(directions[nowDir], instant: true);
        view.UpdateFacingByDirection(directions[nowDir]);
    }

    private void Update()
    {
        HandleAiming();
    }

    private void HandleAiming()
    {
        if (isRotating || directions.Count == 0) return;

        int total = directions.Count;
        int prev = nowDir;

        if (Input.GetKeyDown(rotateRightKey))
        {
            nowDir = (nowDir + 1) % total;
        }
        else if (Input.GetKeyDown(rotateLeftKey))
        {
            nowDir = (nowDir - 1 + total) % total;
        }

        if (prev != nowDir)
        {
            isRotating = true;
            view.SetArrowRotation(directions[nowDir], instant: false, onComplete: () => {
                isRotating = false;
            });

            view.UpdateFacingByDirection(directions[nowDir]);
        }
    }

    public bool CanFire()
    {
        return !isRotating;
    }

    public Vector2 GetAimDirection()
    {
        return directions[nowDir];
    }

    public float GetAimAngle()
    {
        Vector2 dir = GetAimDirection();
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
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
