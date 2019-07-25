using Contract.Abstracts.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TestsReactiveCore
{
    public class TickerEnumerable : IEnumerable<ILogItem>
    {
        public IEnumerator<ILogItem> GetEnumerator()
        {
            return new TickerEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
