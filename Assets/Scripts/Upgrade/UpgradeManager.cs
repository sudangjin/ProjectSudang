using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponStat
{
    public int ID { get; private set; }
    public int Damage { get; private set; }
    private int baseDamage;
    public float Speed { get; private set; }
    private float baseSpeed;
    public float LifeTime => baseLifeTime;
    private float baseLifeTime;
    public float AttackSpeed { get; private set; }
    private float baseAttackSpeed;

    public string PrefabName { get; private set; }

    private WeaponData weaponData;

    public WeaponStat(int id)
    {
        ID = id;
        weaponData = WeaponData.Get(id);

        baseDamage = weaponData.Damage;
        baseSpeed = weaponData.Speed;
        baseLifeTime = weaponData.LifeTime;
        baseAttackSpeed = weaponData.AttackSpeed;

        PrefabName = weaponData.PrefabName;

        UpdateDamage(0f);
        UpdateSpeed(0f);
        UpdateAttackSpeed(0f);
    }

    public void UpdateDamage(float upgradeDamage)
    {
        Damage = (int)(baseDamage * (1f + upgradeDamage));
    }

    public void UpdateSpeed(float upgradeSpeed)
    {
        Speed = baseSpeed * (1f + upgradeSpeed);
    }

    public void UpdateAttackSpeed(float upgradeAttackSpeed)
    {
        AttackSpeed = baseAttackSpeed * (1f + upgradeAttackSpeed);
    }
}

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    private Dictionary<int, int> upgradeLevels = new Dictionary<int, int>();
    public WeaponStat WeaponStat { get; private set; }

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

    public bool CanUnlockUpgrade(UpgradeData upgrade)
    {
        foreach (var req in upgrade.RequireUpgrades.Data)
        {
            int currentLevel = GetUpgradeLevel(req.Key);
            if (currentLevel < req.Value)
                return false;
        }
        return true;
    }

    public List<UpgradeData> GetSelectableUpgrade(int count)
    {
        List<UpgradeData> availableUpgrades = new List<UpgradeData>();
        foreach (var upgrade in DataManager.Instance.GetAllUpgrades())
        {
            if (GetUpgradeLevel(upgrade.ID) >= upgrade.MaxLevel) continue;
            if (upgrade.RequireWeaponID > 0 && upgrade.RequireWeaponID != WeaponStat.ID) continue;
            if (!CanUnlockUpgrade(upgrade)) continue;

            availableUpgrades.Add(upgrade);
        }

        return availableUpgrades.OrderBy(x => Random.value).Take(count).ToList();
    }

    public void ResetAll()
    {
        upgradeLevels.Clear();
        WeaponStat = null;
    }
}
