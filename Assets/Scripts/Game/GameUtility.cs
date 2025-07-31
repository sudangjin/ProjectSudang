using System.Collections.Generic;
using System.Globalization;

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

    #region Data Loader
    public static string GetString(this string[] values, Dictionary<string, int> columnIndex, string key)
    {
        return values[columnIndex[key]];
    }

    public static int GetInt(this string[] values, Dictionary<string, int> columnIndex, string key)
    {
        return int.Parse(values[columnIndex[key]]);
    }

    public static long GetLong(this string[] values, Dictionary<string, int> columnIndex, string key)
    {
        return long.Parse(values[columnIndex[key]]);
    }

    public static float GetFloat(this string[] values, Dictionary<string, int> columnIndex, string key)
    {
        return float.Parse(values[columnIndex[key]]);
    }

    public static bool GetBool(this string[] values, Dictionary<string, int> columnIndex, string key)
    {
        string raw = values[columnIndex[key]].Trim().ToLower();
        return raw == "1" || raw == "true" || raw == "yes";
    }

    public static T GetEnum<T>(this string[] values, Dictionary<string, int> columnIndex, string key) where T : struct
    {
        string raw = values[columnIndex[key]].Trim();
        if (System.Enum.TryParse(raw, true, out T result))
            return result;

        return default;
    }

    #endregion
}
