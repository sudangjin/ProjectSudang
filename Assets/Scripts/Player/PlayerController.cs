using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerView))]
public class PlayerController : MonoBehaviour, IHittable
{
    public static PlayerController Instance { get; private set; }

    private PlayerModel model;
    private PlayerView view;

    [SerializeField] private int maxHP = 100;

    [SerializeField] private KeyCode rotateLeftKey = KeyCode.LeftArrow;
    [SerializeField] private KeyCode rotateRightKey = KeyCode.RightArrow;
    [SerializeField] private float rotateRepeatDelay = 0.2f;

    private int nowDir = 0;
    private IReadOnlyList<Vector2> directions;
    private bool isRotating = false;

    private float rotateTimer = 0f;
    private bool isHoldingLeft = false;
    private bool isHoldingRight = false;

    public int Level => model.Level;
    public int CurrentExp => model.CurrentExp;
    public int ExpToNextLevel => model.ExpToNextLevel;
    public int CurrentDirectionIndex => nowDir;
    public Transform GetTransform() => transform;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        model = new PlayerModel(GameManager.Instance.Config.playerMaxHP);
        view = GetComponent<PlayerView>();
        view.Init(this);
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
        if (isRotating || directions.Count == 0)
            return;

        int total = directions.Count;
        int prev = nowDir;

        // 한 번 누름 처리
        if (Input.GetKeyDown(rotateRightKey))
        {
            isHoldingRight = true;
            isHoldingLeft = false;
            rotateTimer = 0f;
            nowDir = (nowDir + 1) % total;
        }
        else if (Input.GetKeyDown(rotateLeftKey))
        {
            isHoldingLeft = true;
            isHoldingRight = false;
            rotateTimer = 0f;
            nowDir = (nowDir - 1 + total) % total;
        }

        // 지속 입력 처리
        if (Input.GetKey(rotateRightKey))
        {
            isHoldingRight = true;
            rotateTimer += Time.deltaTime;

            if (rotateTimer >= rotateRepeatDelay)
            {
                rotateTimer = 0f;
                nowDir = (nowDir + 1) % total;
            }
        }
        else if (Input.GetKey(rotateLeftKey))
        {
            isHoldingLeft = true;
            rotateTimer += Time.deltaTime;

            if (rotateTimer >= rotateRepeatDelay)
            {
                rotateTimer = 0f;
                nowDir = (nowDir - 1 + total) % total;
            }
        }
        else
        {
            // 키를 떼면 초기화
            if (isHoldingLeft || isHoldingRight)
            {
                isHoldingLeft = false;
                isHoldingRight = false;
                rotateTimer = 0f;
            }
        }

        // 방향이 바뀐 경우 처리
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
        view.UpdateHPGauge(model.CurrentHP, model.MaxHP);

        if (model.IsDead)
        {
            view.Die();
            GameManager.Instance.GameOver();
        }
    }

    public void Heal(int amount)
    {
        model.Heal(amount);
        view.UpdateHPGauge(model.CurrentHP, model.MaxHP);
    }

    public int GetCurrentHP() => model.CurrentHP;
    public int GetMaxHP() => model.MaxHP;
}
public static class Player
{
    public static PlayerController Instance => PlayerController.Instance;
}