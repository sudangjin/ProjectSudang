using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;

public class DataManager
{
    private static DataManager _instance;
    public static DataManager Instance => _instance ??= new DataManager();

    private bool initialized = false;

    private readonly Dictionary<int, MonsterData> monsterDataDic = new();
    private readonly Dictionary<int, ProjectileData> projectileDataDic = new();
    private readonly Dictionary<int, UpgradeData> upgradeDataDic = new();
    private readonly Dictionary<int, WeaponData> weaponDataDic = new();
    private readonly Dictionary<int, CharacterData> characterDataDic = new();

    private readonly Dictionary<int, MapMonsterGroup> mapMonsterGroups = new();

    private readonly List<IDictionary> allDataDictionaries;

    public IEnumerable<MonsterData> GetAllMonsters() => monsterDataDic.Values;
    public IEnumerable<ProjectileData> GetAllProjectiles() => projectileDataDic.Values;
    public IEnumerable<UpgradeData> GetAllUpgrades() => upgradeDataDic.Values;
    public IEnumerable<WeaponData> GetAllWeapons() => weaponDataDic.Values;
    public IEnumerable<CharacterData> GetAllCharacters() => characterDataDic.Values;

    private DataManager()
    {
        allDataDictionaries = new List<IDictionary>
        {
            monsterDataDic,
            projectileDataDic,
            upgradeDataDic,
            weaponDataDic,
            characterDataDic
        };
    }

    public IEnumerator InitAsync(System.Action<float> onProgress)
    {
        if (initialized)
        {
            onProgress?.Invoke(1f);
            yield break;
        }
        initialized = true;

        yield return LoadCsvDataAsync(
            fileName: "Monster",
            createData: (values, columns) => new MonsterData(
                id: values.GetInt(columns, "ID"),
                name: values.GetString(columns, "Name"),
                desc: values.GetString(columns, "Desc"),
                hp: values.GetInt(columns, "HP"),
                damage: values.GetInt(columns, "Damage"),
                moveSpeed: values.GetFloat(columns, "MoveSpeed"),
                attackSpeed: values.GetFloat(columns, "AttackSpeed"),
                attackRange: values.GetFloat(columns, "AttackRange"),
                exp: values.GetInt(columns, "EXP"),
                score: values.GetLong(columns, "Score"),
                prefabName: values.GetString(columns, "PrefabName"),
                mapID: values.GetInt(columns, "MapID"),
                grade: values.GetInt(columns, "Grade"),
                isBoss: values.GetBool(columns, "IsBoss"),
                moveType: values.GetEnum<MovementType>(columns, "MoveType"),
                projectileID: values.GetInt(columns, "ProjectileID")
            ),
            getKey: data => data.ID,
            targetDict: monsterDataDic,
            onProgress: onProgress
        );

        yield return LoadCsvDataAsync(
            fileName: "Projectile",
            createData: (values, columns) => new ProjectileData(
                id: values.GetInt(columns, "ID"),
                name: values.GetString(columns, "Name"),
                desc: values.GetString(columns, "Desc"),
                speed: values.GetFloat(columns, "Speed"),
                lifeTime: values.GetFloat(columns, "LifeTime"),
                prefabName: values.GetString(columns, "PrefabName")
            ),
            getKey: data => data.ID,
            targetDict: projectileDataDic,
            onProgress: onProgress
        );

        yield return LoadCsvDataAsync(
            fileName: "Upgrade",
            createData: (values, columns) => new UpgradeData(
                id: values.GetInt(columns, "ID"),
                name: values.GetString(columns, "Name"),
                desc: values.GetString(columns, "Desc"),
                type: values.GetEnum<UpgradeType>(columns, "Type"),
                requireWeaponID: values.GetInt(columns, "RequireWeaponID"),
                requireUpgradeRaw: values.GetString(columns, "RequireUpgrade"),
                maxLevel: values.GetInt(columns, "MaxLevel"),
                value: values.GetFloat(columns, "Value"),
                addScore: values.GetFloat(columns, "AddScore"),
                isNegative: values.GetBool(columns, "IsNegative")
            ),
            getKey: data => data.ID,
            targetDict: upgradeDataDic,
            onProgress: onProgress
        );

        yield return LoadCsvDataAsync(
            fileName: "Weapon",
            createData: (values, columns) => new WeaponData(
                id: values.GetInt(columns, "ID"),
                name: values.GetString(columns, "Name"),
                desc: values.GetString(columns, "Desc"),
                type: values.GetEnum<WeaponType>(columns, "Type"),
                damage: values.GetInt(columns, "Damage"),
                speed: values.GetFloat(columns, "Speed"),
                lifeTime: values.GetFloat(columns, "LifeTime"),
                attackSpeed: values.GetFloat(columns, "AttackSpeed"),
                prefabName: values.GetString(columns, "PrefabName")
            ),
            getKey: data => data.ID,
            targetDict: weaponDataDic,
            onProgress: onProgress
        );

        yield return LoadCsvDataAsync(
            fileName: "Character",
            createData: (values, columns) => new CharacterData(
                id: values.GetInt(columns, "ID"),
                name: values.GetString(columns, "Name"),
                desc: values.GetString(columns, "Desc"),
                startWeaponID: values.GetInt(columns, "StartWeaponID"),
                startUpgradeID: values.GetInt(columns, "StartUpgradeID"),
                hp: values.GetInt(columns, "HP"),
                autoHeal: values.GetInt(columns, "AutoHeal"),
                turnSpeed: values.GetFloat(columns, "TurnSpeed"),
                direction: values.GetInt(columns, "Direction")
            ),
            getKey: data => data.ID,
            targetDict: characterDataDic,
            onProgress: onProgress
        );

        BuildMapMonsterGroups();

        Debug.Log($"[DataManager] Loaded {monsterDataDic.Count} monsters and {projectileDataDic.Count} projectiles. (Async)");
    }

    private IEnumerator LoadCsvDataAsync<T>(
        string fileName,
        System.Func<string[], Dictionary<string, int>, T> createData,
        System.Func<T, int> getKey,
        Dictionary<int, T> targetDict,
        System.Action<float> onProgress)
    {
        TextAsset csv = Resources.Load<TextAsset>($"Data/{fileName}");
        if (csv == null)
        {
            Debug.LogError($"[DataManager] {fileName}.csv not found in Resources/Data/");
            yield break;
        }

        string[] lines = csv.text.Split('\n');
        if (lines.Length <= 1)
            yield break;

        string[] headers = lines[0].Trim().Split(',');
        Dictionary<string, int> columnIndex = new();
        for (int i = 0; i < headers.Length; i++)
            columnIndex[headers[i]] = i;

        int total = lines.Length - 1;

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] values = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
            for (int v = 0; v < values.Length; v++)
                values[v] = values[v].Trim().Trim('"');

            T data = createData(values, columnIndex);
            int key = getKey(data);

            targetDict[key] = data;

            onProgress?.Invoke((float)i / total);
            yield return null;
        }
    }

    private void BuildMapMonsterGroups()
    {
        mapMonsterGroups.Clear();

        foreach (var monster in monsterDataDic.Values)
        {
            if (!mapMonsterGroups.TryGetValue(monster.MapID, out var group))
            {
                group = new MapMonsterGroup();
                mapMonsterGroups[monster.MapID] = group;
            }

            if (monster.IsBoss)
            {
                group.Bosses.Add(monster);
            }
            else
            {
                if (!group.MonstersByGrade.TryGetValue(monster.Grade, out var list))
                {
                    list = new List<MonsterData>();
                    group.MonstersByGrade[monster.Grade] = list;
                }
                list.Add(monster);
            }
        }
    }

    public List<MonsterData> GetBossesForMap(int mapID)
    {
        if (mapMonsterGroups.TryGetValue(mapID, out var group))
            return group.Bosses;
        return new List<MonsterData>();
    }

    public Dictionary<int, List<MonsterData>> GetMonstersByGradeForMap(int mapID)
    {
        if (mapMonsterGroups.TryGetValue(mapID, out var group))
            return group.MonstersByGrade;
        return new Dictionary<int, List<MonsterData>>();
    }

    public MonsterData GetMonsterData(int id)
    {
        if (monsterDataDic.TryGetValue(id, out var data))
            return data;
        Debug.LogWarning($"[DataManager] Monster ID {id} not found.");
        return null;
    }

    public ProjectileData GetProjectileData(int id)
    {
        if (projectileDataDic.TryGetValue(id, out var data))
            return data;
        Debug.LogWarning($"[DataManager] Projectile ID {id} not found.");
        return null;
    }

    public UpgradeData GetUpgradeData(int id)
    {
        if (upgradeDataDic.TryGetValue(id, out var data))
            return data;
        Debug.LogWarning($"[DataManager] Upgrade ID {id} not found.");
        return null;
    }

    public WeaponData GetWeaponData(int id)
    {
        if (weaponDataDic.TryGetValue(id, out var data))
            return data;
        Debug.LogWarning($"[DataManager] Weapon ID {id} not found.");
        return null;
    }

    public CharacterData GetCharacterData(int id)
    {
        if (characterDataDic.TryGetValue(id, out var data))
            return data;
        Debug.LogWarning($"[DataManager] Character ID {id} not found.");
        return null;
    }

    public void ForceReload()
    {
        initialized = false;
        foreach (var dict in allDataDictionaries)
            dict.Clear();
        mapMonsterGroups.Clear();
    }
}

public class MapMonsterGroup
{
    public Dictionary<int, List<MonsterData>> MonstersByGrade = new();
    public List<MonsterData> Bosses = new();
}
