using Contract.Abstracts.Data;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using Utils.Console;

namespace TestsReactiveCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting");
            var yLogger = new YellowLogger();
            var bLogger = new BlueLogger();
            var wLogger = new WhiteLogger();
            // Started


            //// plain console ticker generator
            //var tickerThread = new Thread(() => ConsoleLogTickers());
            //tickerThread.Start();


            /* Cold Subscription */

            // create Observable generator
            var source = ObservableTickers(50);

            //subscribe to observable
            var subsCold = source.Subscribe(
                x => yLogger.Log(() => Console.WriteLine("Cold OnNext: {0}", x)),
                ex => yLogger.Log(() => Console.WriteLine("Cold OnError: {0}", ex.Message)),
                () => yLogger.Log(() => Console.WriteLine("Cold OnCompleted")));
            //subscribe to observable
            var subs2Cold = source
                .Where(c => c.TypeId < 500)
                .Subscribe(
                    x => bLogger.Log(() => Console.WriteLine("Cold OnNext: {0}", x)),
                    ex => bLogger.Log(() => Console.WriteLine("Cold OnError: {0}", ex.Message)),
                    () => bLogger.Log(() => Console.WriteLine("Cold OnCompleted")));
            // kill subscription
            subsCold.Dispose();
            subs2Cold.Dispose();


            /* Hot Subscription - 1 */
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("/* Hot Subscription - 1 */");
            Console.WriteLine();

            // http://rxwiki.wikidot.com/101samples#toc48
            IConnectableObservable<ILogItem> hotSource = Observable.Publish<ILogItem>(ObservableTickers(100));
            Thread.Sleep(2000);
            wLogger.Log(() => Console.WriteLine("Стартую первый"));
            IDisposable hots = hotSource
                //.Where(c => c.TypeId >= 1000)
                .Subscribe(
                    x => yLogger.Log(() => Console.WriteLine("Hot1 Observer 1: OnNext: {0}", x)),
                    ex => yLogger.Log(() => Console.WriteLine("Hot1 Observer 1: OnError: {0}", ex.Message)),
                    () => yLogger.Log(() => Console.WriteLine("Hot1 Observer 1: OnCompleted")));

            wLogger.Log(() => Console.WriteLine("Стартую второй"));
            IDisposable hots2 = hotSource
                //.Where(c => c.TypeId < 1000)
                .Subscribe(
                    x => bLogger.Log(() => Console.WriteLine("Hot1 Observer 2: OnNext: {0}", x)),
                    ex => bLogger.Log(() => Console.WriteLine("Hot1 Observer 2: OnError: {0}", ex.Message)),
                    () => bLogger.Log(() => Console.WriteLine("Hot1 Observer 2: OnCompleted")));
            wLogger.Log(() => Console.WriteLine("Коннект"));
            hotSource.Connect();       // hot is connected to source and starts pushing value to subscribers 



            /* Hot Subscription - 2 */
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("/* Hot Subscription - 2 */");
            Console.WriteLine();
            var observer = new Subject<ILogItem>();

            var thread = new Thread(() => TickerPopulation(observer, 1000));
            thread.Start();
            Thread.Sleep(2000);
            wLogger.Log(() => Console.WriteLine("Стартую первый"));
            var subs1 = observer.Where(c => c.TypeId >= 1000).Subscribe(Observer.Create<ILogItem>(c => bLogger.Log(() => Console.WriteLine("Hot2 Observer 1: OnNext: {0}", c))));


            Thread.Sleep(2000);
            wLogger.Log(() => Console.WriteLine("Стартую второй"));
            var subs2 = observer.Where(c => c.TypeId < 1000).Subscribe(Observer.Create<ILogItem>(c => yLogger.Log(() => Console.WriteLine("Hot2 Observer 2: OnNext: {0}", c))));

            Thread.Sleep(2000);
            subs1.Dispose();
            wLogger.Log(() => Console.WriteLine("Уничтожил первый"));

            Thread.Sleep(2000);
            wLogger.Log(() => Console.WriteLine("Стартую третий"));
            var subs3 = observer.Where(c => c.TypeId < 1000).Subscribe(Observer.Create<ILogItem>(c => wLogger.Log(() => Console.WriteLine("Hot2 Observer 3: OnNext: {0}", c))));

            Thread.Sleep(2000);
            subs2.Dispose();
            wLogger.Log(() => Console.WriteLine("Уничтожил второй"));

            Thread.Sleep(2000);
            subs3.Dispose();
            wLogger.Log(() => Console.WriteLine("Уничтожил третий"));


            //// Finish
            Console.WriteLine("Press to exit");
            Console.ReadKey();
        }

        private static void TickerPopulation(Subject<ILogItem> observer, int amount)
        {
            foreach (var logItem in Tickers(amount))
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(logItem.TypeId);
                observer.OnNext(logItem);
            }
        }


        public static IObservable<ILogItem> ObservableTickers(int amount)
        {
            IObservable<ILogItem> observable = null;

            //observable = Tickers().ToObservable();
            observable = Observable.Create<ILogItem>((o) => Tickers(amount).ToObservable().Subscribe(o));

            return observable;
        }


        public static IEnumerable<ILogItem> Tickers(int amount)
        {
            Random random = new Random();
            int Limit = amount;
            while (Limit > 0)
            {
                yield return TickerFabrika.GetNewTicker();
                Thread.Sleep(random.Next(20, 50));
                Limit--;
            }
        }
    }
}
