using System.Collections;
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

    private Coroutine autoHealCoroutine = null; 

    public int Level => model.Level;
    public int CurrentExp => model.CurrentExp;
    public int ExpToNextLevel => model.ExpToNextLevel;
    public int CurrentDirectionIndex => nowDir;

    public CharacterData Character { get; private set; }

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
        Character = CharacterData.Get(characterID);
        if (model == null)
            model = new PlayerModel(Character.HP);
        else
            model.Reset(Character.HP);

        if (view == null)
            view = GetComponent<PlayerView>();

        view.Init(this);
        ResetState();

        GameSessionManager.Instance.UpdateSpawnDirections(Character.Direction);
        var directions = GameSessionManager.Instance.SpawnDirections;
        view.SetArrowRotation(directions[nowDir], instant: true);
        view.UpdateFacingByDirection(directions[nowDir]);

        UpgradeManager.Instance.ApplyUpgrade(Character.StartUpgradeID);

        autoHealCoroutine = StartCoroutine(AutoHealRoutine());
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

        if (autoHealCoroutine != null)
        {
            StopCoroutine(autoHealCoroutine);
            autoHealCoroutine = null;
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

        int turnAmount = 1;

        if (Input.GetKeyDown(rotateRightKey))
        {
            isHoldingRight = true;
            isHoldingLeft = false;
            rotateTimer = 0f;

            float wrongTurnProb = UpgradeManager.Instance.AddedUpdateStat.ProbabilityWrongTurn;
            turnAmount = (Random.value < wrongTurnProb) ? 1 : 2;
            nowDir = (nowDir + turnAmount) % total;
        }
        else if (Input.GetKeyDown(rotateLeftKey))
        {
            isHoldingLeft = true;
            isHoldingRight = false;
            rotateTimer = 0f;

            float wrongTurnProb = UpgradeManager.Instance.AddedUpdateStat.ProbabilityWrongTurn;
            turnAmount = (Random.value < wrongTurnProb) ? 1 : 2;
            nowDir = (nowDir - turnAmount + total) % total;
        }

        if (Input.GetKey(rotateRightKey))
        {
            isHoldingRight = true;
            rotateTimer += Time.deltaTime;
            if (rotateTimer >= rotateRepeatDelay)
            {
                rotateTimer = 0f;

                float wrongTurnProb = UpgradeManager.Instance.AddedUpdateStat.ProbabilityWrongTurn;
                turnAmount = (Random.value < wrongTurnProb) ? 1 : 2;
                nowDir = (nowDir + turnAmount) % total;
            }
        }
        else if (Input.GetKey(rotateLeftKey))
        {
            isHoldingLeft = true;
            rotateTimer += Time.deltaTime;
            if (rotateTimer >= rotateRepeatDelay)
            {
                rotateTimer = 0f;

                float wrongTurnProb = UpgradeManager.Instance.AddedUpdateStat.ProbabilityWrongTurn;
                turnAmount = (Random.value < wrongTurnProb) ? 1 : 2;
                nowDir = (nowDir - turnAmount + total) % total;
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

    public List<Vector2> GetMultiAimDirections()
    {
        var directions = GameSessionManager.Instance.SpawnDirections;
        List<Vector2> result = new List<Vector2>();

        if (directions == null || directions.Count == 0)
            return result;

        int total = directions.Count;

        for (int i = 3; i >= 1; i--)
        {
            int prevIndex = (nowDir - i + total) % total;
            result.Add(directions[prevIndex]);
        }

        result.Add(directions[nowDir]);

        for (int i = 1; i <= 3; i++)
        {
            int nextIndex = (nowDir + i) % total;
            result.Add(directions[nextIndex]);
        }

        return result;
    }

    public float GetAimAngle()
    {
        Vector2 dir = GetAimDirection();
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }

    public int GainExp(int amount)
    {
        int prevLevel = model.Level;
        int remaining = model.AddExp(amount);

        if (model.Level != prevLevel)
        {
            view.PlayLevelUpEffect();
            GameEvent.Publish(EventKeys.PlayerLevelChanged, model.Level);
        }

        GameEvent.Publish(EventKeys.PlayerExpChanged, (model.CurrentExp, model.ExpToNextLevel));
        return remaining;
    }

    public void TakeDamage(int damage)
    {
        if (model.IsDead) return;

        model.TakeDamage(damage);
        view.PlayHitEffect();
        view.ShowDamage(damage, false);
        view.UpdateHPGauge(model.CurrentHP, model.MaxHP);

        if (model.IsDead)
        {
            view.Die();
            GameSessionManager.Instance.GameOver();
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;

        amount =(int)(amount * UpgradeManager.Instance.AddedUpdateStat.MultipleHealAmount);
        var healAmount = model.Heal(amount);
        if (healAmount > 0)
        { 
            view.ShowDamage(healAmount, true);
        }
        view.UpdateHPGauge(model.CurrentHP, model.MaxHP);
    }

    public void AddHP(int value)
    {
        model.AddHP(Character.HP + value);
        view.UpdateHPGauge(model.CurrentHP, model.MaxHP);
    }

    public int GetCurrentHP() => model.CurrentHP;
    public int GetMaxHP() => model.MaxHP;

    private IEnumerator AutoHealRoutine()
    {
        var upgradeStat = UpgradeManager.Instance.AddedUpdateStat;
        while (true)
        {
            if (model.IsDead) yield break;

            if (upgradeStat.AddHealPerSecond > 0)
            {
                Heal(upgradeStat.AddHealPerSecond);
            }

            yield return new WaitForSeconds(1f);
        }
    }
}

public static class Player
{
    public static PlayerController Instance => PlayerController.Instance;
}
