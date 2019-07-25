using Contract.Abstracts.Data;
using Contract.Models.DTO;
using System;

namespace TestsReactiveCore
{
    public static class TickerFabrika
    {
        private static Random Randomaizer = new Random();
        public static ILogItem GetNewTicker()
        {
            return new LogItem(DateTime.Now.ToString("HH:mm:ss:fff"), Randomaizer.Next(10, 2000));
        }

    }
}
