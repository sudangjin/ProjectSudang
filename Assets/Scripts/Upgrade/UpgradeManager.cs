using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    private Dictionary<int, int> upgradeLevels = new Dictionary<int, int>();
    public UpgradeStat AddedUpdateStat { get; private set; }
    public WeaponData WeaponData { get; private set; }

    public int RerollDice { get; private set; }

    public float AddScore { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Init(WeaponData weaponData)
    {
        ResetAll();
        WeaponData = weaponData;
        AddedUpdateStat = new UpgradeStat();
    }

    public int GetUpgradeLevel(int upgradeID)
    {
        if (upgradeLevels.TryGetValue(upgradeID, out int level))
            return level;
        return 0;
    }

    public void ApplyUpgrade(int upgradeID)
    {
        if (!upgradeLevels.ContainsKey(upgradeID))
            upgradeLevels[upgradeID] = 0;
        upgradeLevels[upgradeID]++;

        var upgrade = UpgradeData.Get(upgradeID);
        AddScore += upgrade.AddScore;
        UpdateUpgradeStat(upgrade, upgradeLevels[upgradeID], GameSessionManager.Instance.Wave);
    }

    public void ResetUpgrades()
    {
        upgradeLevels.Clear();
    }

    public Dictionary<UpgradeData, int> GetSelectableUpgrade(int count, bool containNegative)
    {
        List<UpgradeData> availableUpgrades = new List<UpgradeData>();
        foreach (var upgrade in DataManager.Instance.GetAllUpgrades())
        {
            if (!containNegative && upgrade.IsNegative) continue;
            if (GetUpgradeLevel(upgrade.ID) >= upgrade.MaxLevel) continue;
            if (upgrade.RequireWeaponID > 0 && upgrade.RequireWeaponID != WeaponData.ID) continue;
            if (!UpgradeUtility.CanUnlockUpgrade(upgrade, GetUpgradeLevel)) continue;

            availableUpgrades.Add(upgrade);
        }

        var pickedList = availableUpgrades.OrderBy(x => Random.value).Take(count);
        var randomMap = new Dictionary<UpgradeData, int>();
        foreach (var upgrade in pickedList)
        {
            int nowLevel = GetUpgradeLevel(upgrade.ID);
            int maxLevel = upgrade.MaxLevel;

            var ratios = GameSessionManager.Instance.Config.hideCardUpgradeCountRatio;
            int upgradeCount = Mathf.Min(
                UpgradeUtility.GetRandomUpgradeCount(ratios, containNegative),
                maxLevel - nowLevel
            );
            randomMap[upgrade] = upgradeCount;
        }

        return randomMap;
    }

    public bool UseRerollDice()
    {
        if (RerollDice <= 0) return false;
        RerollDice--;
        return true;
    }

    private void UpdateUpgradeStat(UpgradeData upgrade, int level, int wave)
    {
        switch (upgrade.Type)
        {
            case UpgradeType.POWER_UP:
                {
                    var value = upgrade.Value * level;
                    AddedUpdateStat.UpdatePlayerDamage((int)value);
                    break;
                }
            case UpgradeType.PROJECTILE_SPEED_UP:
                {
                    var value = upgrade.Value * level;
                    AddedUpdateStat.UpdatePlayerProjectileSpeed(value);
                    break;
                }
            case UpgradeType.MULTI_SHOT_SAME_DIRECTION:
                {
                    var value = upgrade.Value * level;
                    AddedUpdateStat.UpdateShotSameDir((int)value);
                    break;
                }
            case UpgradeType.MULTI_SHOT_RANDOM_DIRECTION:
                {
                    var value = upgrade.Value * level;
                    AddedUpdateStat.UpdateShotRandomDir((int)value);
                    break;
                }
            case UpgradeType.MULTI_SHOT_BEHIND_DIRECTION:
                {
                    AddedUpdateStat.UpdateShotBehindDir(level > 0);
                    break;
                }
            case UpgradeType.RELOAD_TIME_UP:
                {
                    var value = upgrade.Value * level;
                    AddedUpdateStat.UpdateReloadTime(value);
                    break;
                }
            case UpgradeType.ROTATION_SPEED_UP:
                {
                    var value = upgrade.Value * level;
                    AddedUpdateStat.UpdateTurnSpeed(value);
                    break;
                }
            case UpgradeType.NORMALIZE_RANDOM_SHOT_DIRECTION:
                {
                    AddedUpdateStat.UpdateNormalizeShotDir(level > 0);
                    break;
                }
            case UpgradeType.HEAL_HP_PER_SECOND:
                {
                    var value = upgrade.Value * level;
                    AddedUpdateStat.UpdateHealPerSecond((int)value);
                    break;
                }
            case UpgradeType.HEAL_HP_ON_KILL:
                {
                    var value = upgrade.Value * level;
                    AddedUpdateStat.UpdateHealOnKill((int)value);
                    break;
                }
            case UpgradeType.HEAL_UP:
                {
                    var value = upgrade.Value * level;
                    AddedUpdateStat.UpdateHealAmount(value);
                    break;
                }
            case UpgradeType.HP_UP:
                {
                    var value = upgrade.Value * level;
                    AddedUpdateStat.UpdateHP((int)value);

                    Player.Instance.AddHP(AddedUpdateStat.AddHP);
                    break;
                }
            case UpgradeType.ENEMY_SPAWN_UP:
                {
                    var value = upgrade.Value * level;
                    AddedUpdateStat.UpdateAppearEnemyTime(value);
                    break;
                }
            case UpgradeType.ENEMY_PER_WAVE_UP:
                {
                    var value = upgrade.Value * level;
                    AddedUpdateStat.UpdateEnemyCountByWave((int)value, wave);
                    break;
                }
            case UpgradeType.ENEMY_COUNT_UP:
                {
                    var value = upgrade.Value * level;
                    AddedUpdateStat.UpdateMultipleEnemyCount(value);
                    break;
                }
            case UpgradeType.NEGATIVE_ENEMY_POWER_UP:
                {
                    var value = upgrade.Value * level;
                    AddedUpdateStat.UpdateEnemyDamage(value);
                    break;
                }
            case UpgradeType.NEGATIVE_ENEMY_MOVEMENT_SPEED_UP:
                {
                    var value = upgrade.Value * level;
                    AddedUpdateStat.UpdateEnemyMovementSpeed(value);
                    break;
                }
            case UpgradeType.NEGATIVE_WRONG_TURN:
                {
                    var value = upgrade.Value * level;
                    AddedUpdateStat.UpdateProbabilityWrongTurn(value);
                    break;
                }
            case UpgradeType.NEGATIVE_ENEMY_HP_UP:
                {
                    var value = upgrade.Value * level;
                    AddedUpdateStat.UpdateEnemyHP(value);
                    break;
                }
            case UpgradeType.NEGATIVE_ENEMY_ATTACK_SPEED_UP:
                {
                    var value = upgrade.Value * level;
                    AddedUpdateStat.UpdateEnemyAttackSpeed(value);
                    break;
                }
            case UpgradeType.NEGATIVE_ENEMY_ATTACK_RANGE_UP:
                {
                    var value = upgrade.Value * level;
                    AddedUpdateStat.UpdateEnemyAttackRange(value);
                    break;
                }
        }
    }

    public void ResetAll()
    {
        upgradeLevels.Clear();
        AddedUpdateStat = null;
        RerollDice = 2;
    }
}
