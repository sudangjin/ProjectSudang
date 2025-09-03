public class MonsterData : BaseData
{
    public int HP { get; private set; }
    public int HPIncreasePerWave { get; private set; }
    public int Damage { get; private set; }
    public int DamageIncreasePerWave { get; private set; }  
    public float MoveSpeed { get; private set; }
    public float AttackSpeed { get; private set; }
    public float AttackRange { get; private set; }
    public int EXP { get; private set; }
    public long Score { get; private set; }
    public long ScoreIncreasePerWave { get; private set; }
    public string PrefabName { get; private set; }
    public int MapID { get; private set; }
    public int Grade { get; private set; }
    public bool IsBoss { get; private set; }
    public MovementType MoveType { get; private set; }
    public int ProjectileID { get; private set; }

    public MonsterData(int id,
        string name,
        string desc,
        int hp, int hpIncreasePerWave, 
        int damage, int damageIncreasePerWave,
        float moveSpeed,
        float attackSpeed,
        float attackRange,
        int exp,
        long score, long scoreIncreasePerWave,
        string prefabName,
        int mapID,
        int grade, 
        bool isBoss,
        MovementType moveType, 
        int projectileID)
        : base(id, name, desc)
    {
        HP = hp;
        HPIncreasePerWave = hpIncreasePerWave;
        Damage = damage;
        HPIncreasePerWave = hpIncreasePerWave;
        MoveSpeed = moveSpeed;
        AttackSpeed = attackSpeed;
        AttackRange = attackRange;
        EXP = exp;
        Score = score;
        ScoreIncreasePerWave = scoreIncreasePerWave;
        PrefabName = prefabName;
        MapID = mapID;
        Grade = grade;
        IsBoss = isBoss;
        MoveType = moveType;
        ProjectileID = projectileID;
    }

    public static MonsterData Get(int dataID)
    {
        return DataManager.Instance.GetMonsterData(dataID);
    }
}
