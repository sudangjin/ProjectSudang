using System.Collections.Generic;
using UnityEngine;

public class ProjectileUpdater : MonoBehaviour
{
    public static ProjectileUpdater Instance { get; private set; }

    private readonly List<ProjectileController> projectiles = new();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void Register(ProjectileController projectile)
    {
        projectiles.Add(projectile);
    }

    public void Unregister(ProjectileController projectile)
    {
        projectiles.Remove(projectile);
    }

    public void Update()
    {
        for (int i = projectiles.Count - 1; i >= 0; i--)
        {
            projectiles[i].Update();
        }
    }
}
