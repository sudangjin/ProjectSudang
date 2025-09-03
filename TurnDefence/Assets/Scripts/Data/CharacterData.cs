public class CharacterData : BaseData
{
    public int StartWeaponID { get; private set; }
    public int StartUpgradeID { get; private set; }
    public int HP { get; private set; }
    public int AutoHeal { get; private set; }
    public float TurnSpeed { get; private set; }
    public int Direction { get; private set; }

    public CharacterData(int id, string name, string desc, int startWeaponID, int startUpgradeID, int hp, int autoHeal, float turnSpeed, int direction)
        : base(id, name, desc)
    {
        StartWeaponID = startWeaponID;
        StartUpgradeID = startUpgradeID;
        HP = hp;
        AutoHeal = autoHeal;
        TurnSpeed = turnSpeed;
        Direction = direction;
    }

    public static CharacterData Get(int dataID)
    {
        return DataManager.Instance.GetCharacterData(dataID);
    }
}
