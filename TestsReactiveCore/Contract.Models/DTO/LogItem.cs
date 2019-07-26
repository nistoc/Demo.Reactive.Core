using Contract.Abstracts.Data;
using Newtonsoft.Json;

namespace Contract.Models.DTO
{
    public class LogItem: ILogItem
    {
        public LogItem()
        {

        }
        public LogItem(string timeStamp, int typeId)
        {
            if (string.IsNullOrWhiteSpace(timeStamp))
            {
                throw new System.ArgumentException("message", nameof(timeStamp));
            }

            TimeStamp = timeStamp;
            TypeId = typeId;
        }
        public string TimeStamp { get; }

        public int TypeId { get; }

        public override string ToString()
        {
            //return JsonConvert.SerializeObject(this);
            return JsonConvert.SerializeObject(TypeId);
        }

    }
}
