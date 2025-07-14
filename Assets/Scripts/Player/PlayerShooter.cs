using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;

    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private float bulletLifeTime = 5f;
    [SerializeField] private int bulletDamage = 1;

    [SerializeField] private float fireInterval = 0.5f;

    private float fireTimer = 0f;
    private PlayerAimer aimer;

    void Start()
    {
        aimer = GetComponent<PlayerAimer>();
    }

    void Update()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireInterval)
        {
            FireBullet();
            fireTimer = 0f;
        }
    }

    private void FireBullet()
    {
        if (bulletPrefab == null || firePoint == null || aimer == null)
            return;

        Vector2 fireDir = aimer.GetAimDirection();
        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        BulletView view = bulletObj.GetComponent<BulletView>();
        new BulletController(view, fireDir, bulletSpeed, bulletLifeTime, bulletDamage);
    }
}
