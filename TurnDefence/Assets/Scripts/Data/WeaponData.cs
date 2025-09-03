public class WeaponData : BaseData
{
    public WeaponType Type { get; private set; }
    public int Damage { get; private set; }
    public float Speed { get; private set; }
    public float LifeTime { get; private set; }
    public float AttackSpeed { get; private set; }
    public string PrefabName { get; private set; }

    public WeaponData(int id, string name, string desc, WeaponType type, int damage, float speed, float lifeTime, float attackSpeed, string prefabName)
        : base(id, name, desc)
    {
        Type = type;
        Damage = damage;
        Speed = speed;
        LifeTime = lifeTime;
        AttackSpeed = attackSpeed;
        PrefabName = prefabName;
    }

    public static WeaponData Get(int dataID)
    {
        return DataManager.Instance.GetWeaponData(dataID);
    }
}
