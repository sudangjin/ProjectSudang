using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerView))]
public class PlayerController : MonoBehaviour, IHittable
{
    public static PlayerController Instance { get; private set; }

    private PlayerModel model;
    private PlayerView view;

    [SerializeField] private KeyCode rotateLeftKey = KeyCode.LeftArrow;
    [SerializeField] private KeyCode rotateRightKey = KeyCode.RightArrow;
    [SerializeField] private float rotateRepeatDelay = 0.2f;

    private int nowDir = 0;
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
    }

    public void Init(int characterID)
    {
        CharacterData character = CharacterData.Get(characterID);
        if (model == null)
            model = new PlayerModel(character.HP);
        else
            model.Reset(character.HP);

        if (view == null)
            view = GetComponent<PlayerView>();

        view.Init(this);
        ResetState();

        GameSessionManager.Instance.UpdateSpawnDirections(character.Direction);
        var directions = GameSessionManager.Instance.SpawnDirections;
        view.SetArrowRotation(directions[nowDir], instant: true);
        view.UpdateFacingByDirection(directions[nowDir]);

        UpgradeManager.Instance.ApplyUpgrade(character.StartUpgradeID);
    }

    public void ResetState()
    {
        ResetAimState();
        view.UpdateHPGauge(model.CurrentHP, model.MaxHP);

        var directions = GameSessionManager.Instance.SpawnDirections;
        if (directions != null && directions.Count > 0)
        {
            view.SetArrowRotation(directions[nowDir], instant: true);
            view.UpdateFacingByDirection(directions[nowDir]);
        }
    }

    public void Dispose()
    {
        ResetAimState();
        if (view != null)
            view.StopAllCoroutines();
        model = null;
        gameObject.SetActive(false);
    }

    public void Update()
    {
        HandleAiming();
    }

    private void HandleAiming()
    {
        var directions = GameSessionManager.Instance.SpawnDirections;
        if (isRotating || directions == null || directions.Count == 0)
            return;

        int total = directions.Count;
        int prev = nowDir;

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
            if (isHoldingLeft || isHoldingRight)
            {
                isHoldingLeft = false;
                isHoldingRight = false;
                rotateTimer = 0f;
            }
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

    private void ResetAimState()
    {
        nowDir = 0;
        isRotating = false;
        isHoldingLeft = false;
        isHoldingRight = false;
        rotateTimer = 0f;
    }

    public bool CanFire() => !isRotating;
    public Vector2 GetAimDirection() => GameSessionManager.Instance.SpawnDirections[nowDir];
    public float GetAimAngle()
    {
        Vector2 dir = GetAimDirection();
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }

    public bool GainExp(int amount)
    {
        int prevLevel = model.Level;
        bool leveledUp = model.AddExp(amount);

        if (leveledUp && model.Level != prevLevel)
        {
            view.PlayLevelUpEffect();
            GameEvent.Publish(EventKeys.PlayerLevelChanged, model.Level);
        }

        GameEvent.Publish(EventKeys.PlayerExpChanged, (model.CurrentExp, model.ExpToNextLevel));
        return leveledUp;
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
            GameSessionManager.Instance.GameOver();
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
