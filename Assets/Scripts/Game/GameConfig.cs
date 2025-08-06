using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class MonsterSpawnEntry
{
    public int grade;
    public float ratio;
}

[CreateAssetMenu(fileName = "GameConfig", menuName = "Game/GameConfig", order = 1)]
public class GameConfig : ScriptableObject
{
    [Header("Spawn")]
    public float waitTime = 3f;
    public int baseEnemyCount = 20;
    public int enemyIncreasePerWave = 2;
    public int maxEnemyCount = 60;

    [Header("Monster")]
    public float spawnInterval = 2f;
    public float spawnRadius = 8f;

    [Header("Monster Spawn Ratio")]
    public List<MonsterSpawnEntry> monsterSpawnRatios = new List<MonsterSpawnEntry>();

    [Header("Experience Orbs")]
    public int expOrbMaxCount = 100;
    public List<Color> expColors = new List<Color>();

    [Header("Upgrade")]
    public List<float> hideCardUpgradeCountRatio;

    [Header("DamageText")]
    public Color monsterHit;
    public Color playerHit;
    public Color playerHeal; 
    
    [Header("TimeColor")]
    public Gradient colorOverTime;
}
