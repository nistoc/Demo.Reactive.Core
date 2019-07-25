using Contract.Abstracts.Data;
using System;
using System.Collections.Generic;
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
            // Started


            //// plain console ticker generator
            //var tickerThread = new Thread(() => ConsoleLogTickers());
            //tickerThread.Start();


            ///* Cold Subscription */

            //// create Observable generator
            //var source = ObservableTickers();
            ////subscribe to observable
            //var subs = source.Subscribe(
            //    x => LogYellow(() => Console.WriteLine("OnNext: {0}", x)),
            //    ex => LogYellow(() => Console.WriteLine("OnError: {0}", ex.Message)),
            //    () => LogYellow(() => Console.WriteLine("OnCompleted")));
            ////subscribe to observable
            //var subs2 = source
            //    .Where(c => c.TypeId < 100)
            //    .Subscribe(
            //        x => LogBlue(() => Console.WriteLine("OnNext: {0}", x)),
            //        ex => LogBlue(() => Console.WriteLine("OnError: {0}", ex.Message)),
            //        () => LogBlue(() => Console.WriteLine("OnCompleted")));
            //// kill subscription
            //subs.Dispose();
            //subs2.Dispose();

            /* Hot Subscription */
            // http://rxwiki.wikidot.com/101samples#toc48
            //var inst = new TickerInstance();
            //IObservable<ILogItem> observable = inst.ObservableTickers(); ;
            //var shared = observable.Publish();
            IConnectableObservable<ILogItem> hotSource = Observable.Publish<ILogItem>(ObservableTickers());
            
            var yLogger = new YellowLogger();
            IDisposable hots = hotSource
                //.Where(c => c.TypeId >= 1000)
                .Subscribe(
                    x => yLogger.Log(() => Console.WriteLine("Observer 1: OnNext: {0}", x)),
                    ex => yLogger.Log(() => Console.WriteLine("Observer 1: OnError: {0}", ex.Message)),
                    () => yLogger.Log(() => Console.WriteLine("Observer 1: OnCompleted")));
            
            hotSource.Connect();       // hot is connected to source and starts pushing value to subscribers 
            var bLogger = new BlueLogger();
            IDisposable hots2 = hotSource
                //.Where(c => c.TypeId < 1000)
                .Subscribe(
                    x => bLogger.Log(() => Console.WriteLine("Observer 2: OnNext: {0}", x)),
                    ex => bLogger.Log(() => Console.WriteLine("Observer 2: OnError: {0}", ex.Message)),
                    () => bLogger.Log(() => Console.WriteLine("Observer 2: OnCompleted")));
            //hots.Dispose();
            //hots2.Dispose();

            ////Console.WriteLine("Current Time: " + DateTime.Now);
            IObservable<long> source = Observable.Interval(TimeSpan.FromSeconds(1));            //creates a sequence
            IConnectableObservable<long> hot = Observable.Publish<long>(source);  // convert the sequence into a hot sequence

            IDisposable subscription1 = hot.Subscribe(                        // no value is pushed to 1st subscription at this point
                                        x => Console.WriteLine("Observer 1: OnNext: {0}", x),
                                        ex => Console.WriteLine("Observer 1: OnError: {0}", ex.Message),
                                        () => Console.WriteLine("Observer 1: OnCompleted"));
            //Console.WriteLine("Current Time after 1st subscription: " + DateTime.Now);
            //Thread.Sleep(3000);  //idle for 3 seconds
            hot.Connect();       // hot is connected to source and starts pushing value to subscribers 
            //Console.WriteLine("Current Time after Connect: " + DateTime.Now);
            Thread.Sleep(3000);  //idle for 3 seconds
            //Console.WriteLine("Current Time just before 2nd subscription: " + DateTime.Now);

            IDisposable subscription2 = hot.Subscribe(     // value will immediately be pushed to 2nd subscription
                                        x => Console.WriteLine("Observer 2: OnNext: {0}", x),
                                        ex => Console.WriteLine("Observer 2: OnError: {0}", ex.Message),
                                        () => Console.WriteLine("Observer 2: OnCompleted"));


            //// Finish
            //Console.WriteLine("Press to exit");
            Console.ReadKey();
        }





        public static IObservable<ILogItem> ObservableTickers()
        {
            IObservable<ILogItem> observable = null;

            //observable = Tickers().ToObservable();
            observable = Observable.Create<ILogItem>((o)=> Tickers().ToObservable().Subscribe(o));

            return observable;
        }


        public static IEnumerable<ILogItem> Tickers()
        {
            Random random = new Random();
            int Limit = 50;
            while (Limit > 0)
            {
                yield return TickerFabrika.GetNewTicker();
                Thread.Sleep(random.Next(20, 500));
                Limit--;
            }
        }

        public static void ConsoleLogTickers()
        {
            foreach (var item in Tickers())
            {
                Console.WriteLine(item);
            }
        }
    }


    public class TickerInstance
    {

        public IObservable<ILogItem> ObservableTickers()
        {
            return Tickers().ToObservable();
        }


        public IEnumerable<ILogItem> Tickers()
        {
            Random random = new Random();
            int Limit = 50;
            while (Limit > 0)
            {
                yield return TickerFabrika.GetNewTicker();
                Thread.Sleep(random.Next(20, 500));
                Limit--;
            }
        }

        public void ConsoleLogTickers()
        {
            foreach (var item in Tickers())
            {
                Console.WriteLine(item);
            }
        }
    }

}
