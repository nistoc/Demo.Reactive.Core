namespace Contract.Abstracts.Data
{
    public interface ILogItem
    {
        string TimeStamp { get; }
        int TypeId { get; }
    }
}