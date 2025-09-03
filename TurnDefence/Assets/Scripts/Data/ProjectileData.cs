public class ProjectileData : BaseData
{
    public float Speed { get; private set; }
    public float LifeTime { get; private set; }
    public string PrefabName { get; private set; }

    public ProjectileData(int id, string name, string desc, float speed, float lifeTime, string prefabName)
        : base(id, name, desc)
    {
        Speed = speed;
        LifeTime = lifeTime;
        PrefabName = prefabName;
    }

    public static ProjectileData Get(int dataID)
    {
        return DataManager.Instance.GetProjectileData(dataID);
    }
}
