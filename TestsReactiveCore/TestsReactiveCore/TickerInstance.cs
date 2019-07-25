using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using Contract.Abstracts.Data;

namespace TestsReactiveCore
{
    public class TickerInstance
    {

        public IObservable<ILogItem> ObservableTickers()
        {
            return Tickers().ToObservable();
        }


        public IEnumerable<ILogItem> Tickers()
        {
            Random random = new Random();
            int Limit = 50;
            while (Limit > 0)
            {
                yield return TickerFabrika.GetNewTicker();
                Thread.Sleep(random.Next(20, 500));
                Limit--;
            }
        }
    }
}