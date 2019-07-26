using System;

namespace Utils.Console
{
    public class CyanLogger : BaseLogger
    {
        public void Log(Action action)
        {
            LogColored(ConsoleColor.Cyan, action);
        }
    }
}
