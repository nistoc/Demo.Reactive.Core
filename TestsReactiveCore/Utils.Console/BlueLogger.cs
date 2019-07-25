using System;

namespace Utils.Console
{
    public class BlueLogger : BaseLogger
    {
        public void Log(Action action)
        {
            LogColored(ConsoleColor.Blue, action);
        }
    }

}
