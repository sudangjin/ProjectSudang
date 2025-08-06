using System.Collections.Generic;

public class UpgradeData : BaseData
{
    public UpgradeType Type { get; private set; }
    public int RequireWeaponID { get; private set; }
    public IdValueCollection RequireUpgrades { get; private set; }
    public int MaxLevel { get; private set; }
    public float Param1 { get; private set; }
    public float Param2 { get; private set; }
    public float AddScore { get; private set; }
    public bool IsNegative { get; private set; }


    public UpgradeData(int id, string name, string desc, UpgradeType type, int requireWeaponID, string requireUpgradeRaw, int maxLevel, float param1, float param2, float addScore, bool isNegative)
        : base(id, name, desc)
    {
        Type = type;
        RequireWeaponID = requireWeaponID;
        RequireUpgrades = new IdValueCollection(requireUpgradeRaw);

        MaxLevel = maxLevel;
        Param1 = param1;
        Param2 = param2;
        AddScore = addScore;
        IsNegative = isNegative;
    }

    public static UpgradeData Get(int dataID)
    {
        return DataManager.Instance.GetUpgradeData(dataID);
    }
}
