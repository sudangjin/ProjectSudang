using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

public class LoadedDataViewer : EditorWindow
{
    [MenuItem("Tools/View Loaded Data")]
    public static void OpenWindow()
    {
        GetWindow<LoadedDataViewer>("Loaded Data");
    }

    private Vector2 scroll;

    private bool showMonsters = true;
    private bool showProjectiles = true;
    private bool showUpgrades = true;
    private bool showWeapons = true;
    private bool showCharacters = true;

    private Dictionary<int, bool> monsterFoldouts = new();
    private Dictionary<int, bool> projectileFoldouts = new();
    private Dictionary<int, bool> upgradeFoldouts = new();
    private Dictionary<int, bool> weaponFoldouts = new();
    private Dictionary<int, bool> characterFoldouts = new();

    private void OnGUI()
    {
        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("플레이 모드에서만 데이터를 볼 수 있습니다.", MessageType.Info);
            return;
        }

        var dm = DataManager.Instance;
        if (dm == null)
        {
            EditorGUILayout.HelpBox("DataManager 인스턴스를 찾을 수 없습니다.", MessageType.Warning);
            return;
        }

        scroll = EditorGUILayout.BeginScrollView(scroll);

        // Monsters
        showMonsters = EditorGUILayout.Foldout(showMonsters, $"Monsters ({dm.GetAllMonsters().Count()})", true);
        if (showMonsters)
        {
            foreach (var m in dm.GetAllMonsters())
            {
                if (!monsterFoldouts.ContainsKey(m.ID)) monsterFoldouts[m.ID] = false;
                monsterFoldouts[m.ID] = EditorGUILayout.Foldout(monsterFoldouts[m.ID], $"{m.ID} | {m.Name}");
                if (monsterFoldouts[m.ID])
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField($"HP: {m.HP}");
                    EditorGUILayout.LabelField($"Damage: {m.Damage}");
                    EditorGUILayout.LabelField($"MapID: {m.MapID}");
                    EditorGUILayout.LabelField($"Grade: {m.Grade}");
                    EditorGUILayout.LabelField($"Prefab: {m.PrefabName}");
                    EditorGUI.indentLevel--;
                }
            }
        }

        EditorGUILayout.Space();

        // Projectiles
        showProjectiles = EditorGUILayout.Foldout(showProjectiles, $"Projectiles ({dm.GetAllProjectiles().Count()})", true);
        if (showProjectiles)
        {
            foreach (var p in dm.GetAllProjectiles())
            {
                if (!projectileFoldouts.ContainsKey(p.ID)) projectileFoldouts[p.ID] = false;
                projectileFoldouts[p.ID] = EditorGUILayout.Foldout(projectileFoldouts[p.ID], $"{p.ID} | {p.Name}");
                if (projectileFoldouts[p.ID])
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField($"Speed: {p.Speed}");
                    EditorGUILayout.LabelField($"LifeTime: {p.LifeTime}");
                    EditorGUILayout.LabelField($"Prefab: {p.PrefabName}");
                    EditorGUI.indentLevel--;
                }
            }
        }

        EditorGUILayout.Space();

        // Upgrades
        showUpgrades = EditorGUILayout.Foldout(showUpgrades, $"Upgrades ({dm.GetAllUpgrades().Count()})", true);
        if (showUpgrades)
        {
            foreach (var u in dm.GetAllUpgrades())
            {
                if (!upgradeFoldouts.ContainsKey(u.ID)) upgradeFoldouts[u.ID] = false;

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button($"{u.ID} | {u.Name}", GUILayout.ExpandWidth(true)))
                {
                    UpgradeManager.Instance.ApplyUpgrade(u.ID);
                    PopupManager.Instance.ShowLabel($"{u.Name} : {UpgradeManager.Instance.GetDesc(u.ID, false)}", Color.green);
                }

                Rect foldoutRect = GUILayoutUtility.GetRect(15, EditorGUIUtility.singleLineHeight);
                upgradeFoldouts[u.ID] = EditorGUI.Foldout(foldoutRect, upgradeFoldouts[u.ID], "");

                EditorGUILayout.EndHorizontal();

                if (upgradeFoldouts[u.ID])
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField($"MaxLevel: {u.MaxLevel}");
                    EditorGUILayout.LabelField($"RequireWeapon: {u.RequireWeaponID}");
                    EditorGUILayout.LabelField($"RequireUpgrades: {u.RequireUpgrades.ToString()}");
                    EditorGUILayout.LabelField($"Value: {u.Param1}");
                    EditorGUI.indentLevel--;
                }
            }
        }

        EditorGUILayout.Space();

        // Weapons
        showWeapons = EditorGUILayout.Foldout(showWeapons, $"Weapons ({dm.GetAllWeapons().Count()})", true);
        if (showWeapons)
        {
            foreach (var w in dm.GetAllWeapons())
            {
                if (!weaponFoldouts.ContainsKey(w.ID)) weaponFoldouts[w.ID] = false;
                weaponFoldouts[w.ID] = EditorGUILayout.Foldout(weaponFoldouts[w.ID], $"{w.ID} | {w.Name}");
                if (weaponFoldouts[w.ID])
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField($"Speed: {w.Speed}");
                    EditorGUILayout.LabelField($"AttackSpeed: {w.AttackSpeed}");
                    EditorGUILayout.LabelField($"Prefab: {w.PrefabName}");
                    EditorGUI.indentLevel--;
                }
            }
        }

        EditorGUILayout.Space();

        // Characters
        showCharacters = EditorGUILayout.Foldout(showCharacters, $"Characters ({dm.GetAllCharacters().Count()})", true);
        if (showCharacters)
        {
            foreach (var c in dm.GetAllCharacters())
            {
                if (!characterFoldouts.ContainsKey(c.ID)) characterFoldouts[c.ID] = false;
                characterFoldouts[c.ID] = EditorGUILayout.Foldout(characterFoldouts[c.ID], $"{c.ID} | {c.Name}");
                if (characterFoldouts[c.ID])
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField($"HP: {c.HP}");
                    EditorGUILayout.LabelField($"AutoHeal: {c.AutoHeal}");
                    EditorGUILayout.LabelField($"StartUpgradeID: {c.StartUpgradeID}");
                    EditorGUI.indentLevel--;
                }
            }
        }

        EditorGUILayout.EndScrollView();
    }
}
