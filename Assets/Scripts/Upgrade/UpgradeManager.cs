using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    private Dictionary<int, int> upgradeLevels = new Dictionary<int, int>();
    public WeaponStat WeaponStat { get; private set; }

    public int RerollDice { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void InitWeaponStat(int weaponID)
    {
        ResetAll();
        WeaponStat = new WeaponStat(weaponID);
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
            if (upgrade.RequireWeaponID > 0 && upgrade.RequireWeaponID != WeaponStat.ID) continue;
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

    public void ResetAll()
    {
        upgradeLevels.Clear();
        WeaponStat = null;
        RerollDice = 2;
    }
}
