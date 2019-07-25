using System;


namespace Utils.Console
{
    public abstract class BaseLogger
    {

        protected void LogColored(ConsoleColor consoleColor, Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            System.Console.ForegroundColor = consoleColor;
            action();
            System.Console.ForegroundColor = ConsoleColor.White;
        }
    }

}
