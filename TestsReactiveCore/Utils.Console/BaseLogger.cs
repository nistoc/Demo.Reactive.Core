using System;


namespace Utils.Console
{
    public abstract class BaseLogger
    {

        protected void LogColored(ConsoleColor consoleColor, Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            System.Console.BackgroundColor = consoleColor;
            action();
            System.Console.BackgroundColor = ConsoleColor.Black;
        }
    }

}
