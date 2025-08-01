using System.Collections.Generic;
using System.Linq;

public class IdValueCollection
{
    private readonly Dictionary<int, int> data = new Dictionary<int, int>();

    public IReadOnlyDictionary<int, int> Data => data;

    public IdValueCollection() { }

    public IdValueCollection(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return;

        string[] groups = raw.Split(new string[] { "],[" }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (var group in groups)
        {
            string cleaned = group.Replace("[", "").Replace("]", "").Trim();
            string[] values = cleaned.Split(',');

            if (values.Length == 2 &&
                int.TryParse(values[0], out int id) &&
                int.TryParse(values[1], out int val))
            {
                data[id] = val;
            }
        }
    }

    public void Add(int id, int value)
    {
        data[id] = value;
    }

    public bool TryGetValue(int id, out int value)
    {
        return data.TryGetValue(id, out value);
    }

    public bool Contains(int id)
    {
        return data.ContainsKey(id);
    }

    public override string ToString()
    {
        return string.Join(",", data.Select(kv => $"[{kv.Key},{kv.Value}]"));
    }
}
