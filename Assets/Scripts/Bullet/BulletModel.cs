public class BulletModel
{
    public float Speed { get; private set; }
    public float LifeTime { get; private set; }
    public int Damage { get; private set; }

    public BulletModel(float speed, float lifeTime, int damage)
    {
        Speed = speed;
        LifeTime = lifeTime;
        Damage = damage;
    }
}
