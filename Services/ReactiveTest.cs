using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using Bootstrap.Extensions.StartupTasks;
using log4net;
using Manufacturing.Framework.Datasource;
using Manufacturing.Framework.Dto;
using Microsoft.FluentMessaging;
using WorkerRoleWithSBQueue1.Configuration;

namespace Manufacturing.DevRunner.Services
{
    public class ReactiveTest : IStartupTask
    {
        #region Fields

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly CloudConfiguration _config;

        private readonly IDatasourceRecordSerializer _serializer;

        private readonly Subject<String> averageSubj = new Subject<String>();

        private readonly Subject<String> maxSubj = new Subject<String>();

        private readonly Subject<String> sampleSubj = new Subject<String>();

        private IObservable<String> average;

        private IObservable<String> max;

        private Subject<DatasourceRecord> rx = default(Subject<DatasourceRecord>);

        private IObservable<String> sample;

        private IObservable<IObservable<double>> seqWindowed = default(IObservable<IObservable<double>>);

        private IDisposable subscriptionSignalRPublisher = default(IDisposable);

        #endregion

        #region Constructors

        public ReactiveTest(CloudConfiguration configuration, IDatasourceRecordSerializer serializer)
        {
            _config = configuration;
            _serializer = serializer;
        }

        #endregion

        #region IStartupTask Members

        public void Reset() { }

        public async void Run()
        {
            rx =
                QueueFramework.FromTopicSubscription(_config.ReceiveQueue.GetConnectionString(),
                    _config.ReceiveQueue.QueueName, "rxtest")
                    .WithPrefetchCount(1000)
                    .WithMaxConcurrency(1)
                    .OutputToReactive(_serializer as DatasourceRecordSerializer);

            //rx.Sample(TimeSpan.FromSeconds(1)).Subscribe(x => Log.Debug(x.GetDecimalValue()));

            var eventsProcessed = 0;
            var init = DateTime.UtcNow;
            var start = DateTime.UtcNow;
            DateTime end;
            rx.Subscribe(x => { eventsProcessed++; }, ex => Log.DebugFormat("OnError: {0}", ex.Message), () =>
            {
                end = DateTime.UtcNow;
                Log.DebugFormat("OnCompleted: {0} events processed in {1}", eventsProcessed, (end - init).TotalSeconds);
            });

            // Analytic Subscriptions
            var rawValueStream = rx.Select<DatasourceRecord, double>(x =>
            {
                if(x.DataType == DatasourceRecord.DataTypeEnum.Double)
                {
                    return x.GetDoubleValue();
                }
                else
                {
                    return 0.0D;
                }
            });

            // Calculate Avg and Max values every second

            seqWindowed = rawValueStream.Window(() =>
            {
                var seqWindowControl = Observable.Interval(TimeSpan.FromSeconds(1));
                return seqWindowControl;
            });

            var total = 0.0D;
            var count = 0;
            var max = 0.0D;

            seqWindowed.Subscribe(seqWindow =>
            {
                var thisWindowCount = 0;
                seqWindow.Subscribe(x =>
                {
                    total += x;
                    count++;
                    thisWindowCount++;
                    if(x > max)
                    {
                        max = x;
                    }
                    //Console.WriteLine( "Integer : {0}", x ); 
                }, () =>
                {
                    // Emit objects to new Obersevable
                    averageSubj.OnNext((total / count).ToString("F2"));
                    maxSubj.OnNext(max.ToString("F2"));
                    sampleSubj.OnNext(String.Format("Window Avg: {0}, Count: {1} ", total / count, thisWindowCount));
                });
            });
        }

        #endregion
    }
}