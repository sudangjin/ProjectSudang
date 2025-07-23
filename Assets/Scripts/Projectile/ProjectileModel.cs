public class ProjectileModel
{
    public float Speed { get; private set; }
    public float LifeTime { get; private set; }
    public int Damage { get; private set; }

    public ProjectileModel(float speed, float lifeTime, int damage)
    {
        Speed = speed;
        LifeTime = lifeTime;
        Damage = damage;
    }
}
