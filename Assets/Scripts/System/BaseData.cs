public abstract class BaseData
{
    public int ID { get; private set; }
    public string Name { get; private set; }
    public string Desc { get; private set; }

    protected BaseData(int id, string name, string desc)
    {
        ID = id;
        Name = name;
        Desc = desc;
    }
}
