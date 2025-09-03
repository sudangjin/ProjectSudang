using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static class GameUtility
{
    public static string ToMoneyFormat(this string num)
    {
        if (string.IsNullOrWhiteSpace(num)) return num;

        if (long.TryParse(num, out long value)) return value.ToString("N0", CultureInfo.InvariantCulture);

        return num;
    }

    public static string ToMoneyFormat(this int num)
    {
        return num.ToString("N0", CultureInfo.InvariantCulture);
    }
    public static string ToMoneyFormat(this long num)
    {
        return num.ToString("N0", CultureInfo.InvariantCulture);
    }

    public static string ToPercentFormat(this float value, int decimalPlaces)
    {
        string format = decimalPlaces > 0 ? $"F{decimalPlaces}" : "F0";
        return string.Format(CultureInfo.InvariantCulture, "{0}%", value.ToString(format, CultureInfo.InvariantCulture));
    }

    public static string SFormat(this string format, params object[] args)
    {
        if (string.IsNullOrEmpty(format)) return format;
        return string.Format(CultureInfo.InvariantCulture, format, args);
    }

    #region Data Loader
    public static string GetString(this string[] values, Dictionary<string, int> columnIndex, string key)
    {
        if (!columnIndex.ContainsKey(key))
        {
            Debug.LogWarning($"[GameUtility] Key '{key}' not found in CSV header.");
            return string.Empty;
        }
        return values[columnIndex[key]];
    }

    public static int GetInt(this string[] values, Dictionary<string, int> columnIndex, string key)
    {
        if (!columnIndex.ContainsKey(key))
        {
            Debug.LogWarning($"[GameUtility] Key '{key}' not found for int.");
            return 0;
        }

        string raw = values[columnIndex[key]].Trim();
        if (!int.TryParse(raw, out int result))
        {
            Debug.LogWarning($"[GameUtility] Failed to parse int for key '{key}', value='{raw}'");
            return 0;
        }
        return result;
    }

    public static long GetLong(this string[] values, Dictionary<string, int> columnIndex, string key)
    {
        if (!columnIndex.ContainsKey(key))
        {
            Debug.LogWarning($"[GameUtility] Key '{key}' not found for long.");
            return 0;
        }

        string raw = values[columnIndex[key]].Trim();
        if (!long.TryParse(raw, out long result))
        {
            Debug.LogWarning($"[GameUtility] Failed to parse long for key '{key}', value='{raw}'");
            return 0;
        }
        return result;
    }

    public static float GetFloat(this string[] values, Dictionary<string, int> columnIndex, string key)
    {
        if (!columnIndex.ContainsKey(key))
        {
            Debug.LogWarning($"[GameUtility] Key '{key}' not found for float.");
            return 0f;
        }

        string raw = values[columnIndex[key]].Trim();
        if (!float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
        {
            Debug.LogWarning($"[GameUtility] Failed to parse float for key '{key}', value='{raw}'");
            return 0f;
        }
        return result;
    }

    public static bool GetBool(this string[] values, Dictionary<string, int> columnIndex, string key)
    {
        if (!columnIndex.ContainsKey(key))
        {
            Debug.LogWarning($"[GameUtility] Key '{key}' not found for bool.");
            return false;
        }

        string raw = values[columnIndex[key]].Trim().ToLower();
        return raw == "1" || raw == "true" || raw == "yes";
    }

    public static T GetEnum<T>(this string[] values, Dictionary<string, int> columnIndex, string key) where T : struct
    {
        if (!columnIndex.ContainsKey(key))
        {
            Debug.LogWarning($"[GameUtility] Key '{key}' not found for enum {typeof(T).Name}.");
            return default;
        }

        string raw = values[columnIndex[key]].Trim();
        if (!System.Enum.TryParse(raw, true, out T result))
        {
            Debug.LogWarning($"[GameUtility] Failed to parse enum {typeof(T).Name} for key '{key}', value='{raw}'");
            return default;
        }
        return result;
    }
    #endregion
}
