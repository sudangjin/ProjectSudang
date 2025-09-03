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
        AddScore = 1f;
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
            if (upgrade.MaxLevel > 0 && GetUpgradeLevel(upgrade.ID) >= upgrade.MaxLevel) continue;
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
                    var value = upgrade.Param1 * level;
                    AddedUpdateStat.UpdatePlayerDamage((int)value);
                    break;
                }
            case UpgradeType.PROJECTILE_SPEED_UP:
                {
                    var value = upgrade.Param1 * level;
                    AddedUpdateStat.UpdatePlayerProjectileSpeed(value);
                    break;
                }
            case UpgradeType.MULTI_SHOT_SAME_DIRECTION:
                {
                    var value = upgrade.Param1 * level;
                    var panelty = upgrade.Param2;
                    AddedUpdateStat.UpdateShotSameDir((int)value, panelty);
                    break;
                }
            case UpgradeType.MULTI_SHOT_RANDOM_DIRECTION:
                {
                    var value = upgrade.Param1 * level;
                    var panelty = upgrade.Param2;
                    AddedUpdateStat.UpdateShotRandomDir((int)value, panelty);
                    break;
                }
            case UpgradeType.MULTI_SHOT_BEHIND_DIRECTION:
                {
                    AddedUpdateStat.UpdateShotBehindDir(level > 0);
                    break;
                }
            case UpgradeType.RELOAD_TIME_UP:
                {
                    var value = upgrade.Param1 * level;
                    AddedUpdateStat.UpdateReloadTime(value);
                    break;
                }
            case UpgradeType.ROTATION_SPEED_UP:
                {
                    var value = upgrade.Param1 * level;
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
                    var value = upgrade.Param1 * level;
                    AddedUpdateStat.UpdateHealPerSecond((int)value);
                    break;
                }
            case UpgradeType.HEAL_HP_ON_KILL:
                {
                    var value = upgrade.Param1 * level;
                    AddedUpdateStat.UpdateHealOnKill((int)value);
                    break;
                }
            case UpgradeType.HEAL_UP:
                {
                    var value = upgrade.Param1 * level;
                    AddedUpdateStat.UpdateHealAmount(value);
                    break;
                }
            case UpgradeType.HP_UP:
                {
                    var value = upgrade.Param1 * level;
                    AddedUpdateStat.UpdateHP((int)value);

                    Player.Instance.AddHP(AddedUpdateStat.AddHP);
                    break;
                }
            case UpgradeType.ENEMY_SPAWN_UP:
                {
                    var value = upgrade.Param1 * level;
                    AddedUpdateStat.UpdateAppearEnemyTime(value);
                    break;
                }
            case UpgradeType.ENEMY_PER_WAVE_UP:
                {
                    var value = upgrade.Param1 * level;
                    AddedUpdateStat.UpdateEnemyCountByWave((int)value, wave);
                    break;
                }
            case UpgradeType.ENEMY_COUNT_UP:
                {
                    var value = upgrade.Param1 * level;
                    AddedUpdateStat.UpdateMultipleEnemyCount(value);
                    break;
                }
            case UpgradeType.NEGATIVE_ENEMY_POWER_UP:
                {
                    var value = upgrade.Param1 * level;
                    AddedUpdateStat.UpdateEnemyDamage(value);
                    break;
                }
            case UpgradeType.NEGATIVE_ENEMY_MOVEMENT_SPEED_UP:
                {
                    var value = upgrade.Param1 * level;
                    AddedUpdateStat.UpdateEnemyMovementSpeed(value);
                    break;
                }
            case UpgradeType.NEGATIVE_WRONG_TURN:
                {
                    var value = upgrade.Param1 * level;
                    AddedUpdateStat.UpdateProbabilityWrongTurn(value);
                    break;
                }
            case UpgradeType.NEGATIVE_ENEMY_HP_UP:
                {
                    var value = upgrade.Param1 * level;
                    AddedUpdateStat.UpdateEnemyHP(value);
                    break;
                }
            case UpgradeType.NEGATIVE_ENEMY_ATTACK_SPEED_UP:
                {
                    var value = upgrade.Param1 * level;
                    AddedUpdateStat.UpdateEnemyAttackSpeed(value);
                    break;
                }
            case UpgradeType.NEGATIVE_ENEMY_ATTACK_RANGE_UP:
                {
                    var value = upgrade.Param1 * level;
                    AddedUpdateStat.UpdateEnemyAttackRange(value);
                    break;
                }
        }
    }

    public string GetDesc(int upgradeID, bool isNextLevel)
    {
        var upgrade = UpgradeData.Get(upgradeID);
        var level = GetUpgradeLevel(upgradeID);
        if (isNextLevel) level++;

        switch (upgrade.Type)
        {
            case UpgradeType.POWER_UP:
            case UpgradeType.HEAL_HP_PER_SECOND:
            case UpgradeType.HEAL_HP_ON_KILL:
            case UpgradeType.HP_UP:
            case UpgradeType.ENEMY_PER_WAVE_UP:
                {
                    return upgrade.Desc.SFormat(upgrade.Param1 * level);
                }
            case UpgradeType.PROJECTILE_SPEED_UP:
            case UpgradeType.RELOAD_TIME_UP:
            case UpgradeType.ROTATION_SPEED_UP:
            case UpgradeType.ENEMY_SPAWN_UP:
            case UpgradeType.ENEMY_COUNT_UP:
            case UpgradeType.HEAL_UP:
            case UpgradeType.NEGATIVE_ENEMY_POWER_UP:
            case UpgradeType.NEGATIVE_ENEMY_MOVEMENT_SPEED_UP:
            case UpgradeType.NEGATIVE_WRONG_TURN:
            case UpgradeType.NEGATIVE_ENEMY_HP_UP:
            case UpgradeType.NEGATIVE_ENEMY_ATTACK_SPEED_UP:
            case UpgradeType.NEGATIVE_ENEMY_ATTACK_RANGE_UP:
                {
                    return upgrade.Desc.SFormat(upgrade.Param1 * level * 100);
                }
            case UpgradeType.MULTI_SHOT_SAME_DIRECTION:
            case UpgradeType.MULTI_SHOT_RANDOM_DIRECTION:
                {
                    return upgrade.Desc.SFormat(upgrade.Param1 * level, upgrade.Param2 * level * 100);
                }
            case UpgradeType.MULTI_SHOT_BEHIND_DIRECTION:
            case UpgradeType.NORMALIZE_RANDOM_SHOT_DIRECTION:
                {
                    return upgrade.Desc;
                }
        }

        return null;
    }

    public void ResetAll()
    {
        upgradeLevels.Clear();
        AddedUpdateStat = null;
        RerollDice = 2;
    }
}
