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