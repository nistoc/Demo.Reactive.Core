using System;

namespace Utils.Console
{
    public class WhiteLogger : BaseLogger
    {
        public void Log(Action action)
        {
            LogColored(ConsoleColor.White, action);
        }
    }
}