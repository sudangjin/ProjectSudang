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
}
