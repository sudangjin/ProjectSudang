using System.Collections.Generic;
using UnityEngine;

public class BulletUpdater : MonoBehaviour
{
    public static BulletUpdater Instance { get; private set; }

    private readonly List<BulletController> bullets = new();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void Register(BulletController bullet)
    {
        bullets.Add(bullet);
    }

    public void Unregister(BulletController bullet)
    {
        bullets.Remove(bullet);
    }

    void Update()
    {
        for (int i = bullets.Count - 1; i >= 0; i--)
        {
            bullets[i].Update();
        }
    }
}
