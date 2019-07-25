using System;

namespace Utils.Console
{
    public class YellowLogger : BaseLogger
    {
        public void Log(Action action)
        {
            LogColored(ConsoleColor.Yellow, action);
        }
    }

}
