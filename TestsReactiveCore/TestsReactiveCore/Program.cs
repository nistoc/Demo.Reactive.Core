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
            var cLogger = new CyanLogger();
            var wLogger = new WhiteLogger();
            var gLogger = new GreenLogger();
            // Started


            /* Cold Subscription */

            // create Observable generator
            var source = ObservableTickers(50);
            Thread.Sleep(1300);
            //subscribe to observable
            var coldThread1 = new Thread(() =>
            {
                var subsCold = source
                //.Where(c => c.TypeId > 400 && c.TypeId < 500)
                .Subscribe(
                x => yLogger.Log(() => Console.WriteLine("Cold OnNext: {0}", x)),
                ex => yLogger.Log(() => Console.WriteLine("Cold OnError: {0}", ex.Message)),
                () => yLogger.Log(() => Console.WriteLine("Cold OnCompleted")));
                subsCold.Dispose();
            });
            //subscribe to observable
            var coldThread2 = new Thread(() =>
            {
                var subs2Cold = source
                    //.Where(c => c.TypeId > 400 && c.TypeId < 500)
                    .Subscribe(
                        x => cLogger.Log(() => Console.WriteLine("Cold OnNext: {0}", x)),
                        ex => cLogger.Log(() => Console.WriteLine("Cold OnError: {0}", ex.Message)),
                        () => cLogger.Log(() => Console.WriteLine("Cold OnCompleted")));
                // kill subscription
                subs2Cold.Dispose();
            });
            coldThread1.Start();
            coldThread2.Start();


            /* Hot Subscription - 1 */
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            var tab = "                                                                                            ";
            Console.WriteLine(tab + "/* Hot Subscription - 1 */");
            Console.WriteLine();
            // http://rxwiki.wikidot.com/101samples#toc48
            IConnectableObservable<ILogItem> hotSource = Observable.Publish<ILogItem>(ObservableTickers(100));
            Thread.Sleep(2000);
            wLogger.Log(() => Console.WriteLine(tab + "Стартую первый"));
            IDisposable hots = hotSource
                //.Where(c => c.TypeId >= 1000)
                .Subscribe(
                    x => yLogger.Log(() => Console.WriteLine(tab + "Hot1 Observer 1: OnNext: {0}", x)),
                    ex => yLogger.Log(() => Console.WriteLine(tab + "Hot1 Observer 1: OnError: {0}", ex.Message)),
                    () => yLogger.Log(() => Console.WriteLine(tab + "Hot1 Observer 1: OnCompleted")));

            wLogger.Log(() => Console.WriteLine(tab + "Коннект"));
            var threadHot1 = new Thread(() => hotSource.Connect());
            threadHot1.Start();

            Thread.Sleep(1500);
            wLogger.Log(() => Console.WriteLine(tab + "Стартую второй"));
            IDisposable hots2 = hotSource
                //.Where(c => c.TypeId < 1000)
                .Subscribe(
                    x => cLogger.Log(() => Console.WriteLine(tab + "Hot1 Observer 2: OnNext: {0}", x)),
                    ex => cLogger.Log(() => Console.WriteLine(tab + "Hot1 Observer 2: OnError: {0}", ex.Message)),
                    () => cLogger.Log(() => Console.WriteLine(tab + "Hot1 Observer 2: OnCompleted")));
            //hotSource.Connect();       // hot is connected to source and starts pushing value to subscribers 
            Thread.Sleep(3000);
            wLogger.Log(() => Console.WriteLine("Уничтожил Hot1 Observer 1"));
            hots.Dispose();

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
            var subs1 = observer.Where(c => c.TypeId >= 1000).Subscribe(Observer.Create<ILogItem>(c => cLogger.Log(() => Console.WriteLine("Hot2 Observer 1: OnNext: {0}", c))));


            Thread.Sleep(2000);
            wLogger.Log(() => Console.WriteLine("Стартую второй"));
            var subs2 = observer.Where(c => c.TypeId < 1000).Subscribe(Observer.Create<ILogItem>(c => yLogger.Log(() => Console.WriteLine("Hot2 Observer 2: OnNext: {0}", c))));

            Thread.Sleep(2000);
            subs1.Dispose();
            wLogger.Log(() => Console.WriteLine("Уничтожил первый"));

            Thread.Sleep(2000);
            wLogger.Log(() => Console.WriteLine("Стартую третий"));
            var subs3 = observer.Where(c => c.TypeId < 1000).Subscribe(Observer.Create<ILogItem>(c => gLogger.Log(() => Console.WriteLine("Hot2 Observer 3: OnNext: {0}", c))));

            Thread.Sleep(2000);
            subs2.Dispose();
            wLogger.Log(() => Console.WriteLine("Уничтожил второй"));

            Thread.Sleep(2000);
            subs3.Dispose();
            wLogger.Log(() => Console.WriteLine("Уничтожил третий"));

            Thread.Sleep(2000);
            observer.Dispose();
            wLogger.Log(() => Console.WriteLine("Уничтожил observer"));


            //// Finish
            Console.WriteLine("Press to exit");
            Console.ReadKey();
        }

        private static void TickerPopulation(Subject<ILogItem> observer, int amount)
        {
            foreach (var logItem in Tickers(amount))
            {
                if (observer == null || observer.IsDisposed) return;
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
                var logItem = TickerFabrika.GetNewTicker();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(logItem.TypeId + "   ");
                yield return logItem;
                Thread.Sleep(random.Next(20, 50));
                Limit--;
            }
        }
    }
}
