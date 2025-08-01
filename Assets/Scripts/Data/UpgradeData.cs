using System.Collections.Generic;

public class UpgradeData : BaseData
{
    public UpgradeType Type { get; private set; }
    public int RequireWeaponID { get; private set; }
    public IdValueCollection RequireUpgrades { get; private set; }
    public int MaxLevel { get; private set; }
    public float Value { get; private set; }

    public UpgradeData(int id, string name, string desc, UpgradeType type, int requireWeaponID, string requireUpgradeRaw, int maxLevel, float value)
        : base(id, name, desc)
    {
        Type = type;
        RequireWeaponID = requireWeaponID;
        RequireUpgrades = new IdValueCollection(requireUpgradeRaw);

        MaxLevel = maxLevel;
        Value = value;
    }

    public static UpgradeData Get(int dataID)
    {
        return DataManager.Instance.GetUpgradeData(dataID);
    }
}
