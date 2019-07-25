using Contract.Abstracts.Data;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TestsReactiveCore
{
    public class TickerEnumerator : IEnumerator<ILogItem>
    {
        public ILogItem Current
        {
            get
            {
                return TickerFabrika.GetNewTicker();
            }
        }
        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public void Dispose()
        {
            
        }

        public bool MoveNext()
        {
            return true;
        }

        public void Reset()
        {
            
        }
    }

}
