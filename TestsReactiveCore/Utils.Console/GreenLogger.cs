using System;

namespace Utils.Console
{
    public class GreenLogger : BaseLogger
    {
        public void Log(Action action)
        {
            LogColored(ConsoleColor.Green, action);
        }
    }
}
