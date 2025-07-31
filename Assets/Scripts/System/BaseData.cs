public abstract class BaseData
{
    public int ID { get; private set; }
    public string Name { get; private set; }

    protected BaseData(int id, string name)
    {
        ID = id;
        Name = name;
    }
}
