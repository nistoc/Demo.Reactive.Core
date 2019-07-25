using Contract.Abstracts.Data;
using System;
using System.Collections.Generic;
using System.Threading;

namespace TestsReactiveCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting");
            // Started


            var tickerThread = new Thread(() => ConsoleLogTickers());
            tickerThread.Start();


            

            //// Finish
            //Console.WriteLine("Press to exit");
            //Console.ReadLine();
        }



        public static void ConsoleLogTickers()
        {
            var random = new Random();
            foreach (var item in GenerateTickers())
            {
                Thread.Sleep(random.Next(20,500));
                Console.WriteLine(item);
            }
        }


        public static IEnumerable<ILogItem> GenerateTickers()
        {
            return new TickerEnumerable();
        }



    }

}
