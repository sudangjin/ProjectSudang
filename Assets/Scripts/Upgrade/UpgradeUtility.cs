using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class UpgradeUtility
{
    public static int GetRandomUpgradeCount(List<float> ratios, bool containNegative)
    {
        if (!containNegative)
            return 1;

        float r = Random.value * ratios.Sum();
        for (int i = 0; i < ratios.Count; i++)
            if ((r -= ratios[i]) <= 0) return i + 1;
        return ratios.Count;
    }

    public static bool CanUnlockUpgrade(UpgradeData upgrade, System.Func<int, int> getUpgradeLevel)
    {
        foreach (var req in upgrade.RequireUpgrades.Data)
        {
            int currentLevel = getUpgradeLevel(req.Key);
            if (currentLevel < req.Value)
                return false;
        }
        return true;
    }
}
