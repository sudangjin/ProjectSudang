using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Game/GameConfig", order = 1)]
public class GameConfig : ScriptableObject
{
    [Header("Player")]
    public int playerMaxHP = 100;

    [Header("Spawn")]
    public int spawnDirections = 12;

    [Header("Monster")]
    public GameObject monsterPrefab;
    public float spawnInterval = 2f;
    public float spawnRadius = 8f;
    public float monsterMoveSpeed = 2f;
    public float monsterAttackRange = 1.5f;
    public int monsterHP = 3;
}
