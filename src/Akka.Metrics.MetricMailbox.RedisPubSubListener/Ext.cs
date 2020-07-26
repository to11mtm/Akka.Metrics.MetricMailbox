using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akka.Mailbox.Visualizer;
using StackExchange.Redis;

namespace Akka.Metrics.MetricMailbox.RedisPubSubListener
{
    public static class Ext
    {
        public static IDisposable SubscribeAsync<T>(this IObservable<T> source, 
            Func<Task> asyncAction, Action<Exception> handler = null)
        {
            Func<T,Task<Unit>> wrapped = async t =>
            {
                await asyncAction();
                return Unit.Default;
            };
            if(handler == null)
                return source.SelectMany(wrapped).Subscribe(_ => { });
            else
                return source.SelectMany(wrapped).Subscribe(_ => { }, handler);
        }

        public static IDisposable SubscribeAsync<T>(this IObservable<T> source, 
            Func<T,Task> asyncAction, Action<Exception> handler = null)
        {
            Func<T, Task<Unit>> wrapped = async t =>
            {
                await asyncAction(t);
                return Unit.Default;
            };
            if(handler == null)
                return source.SelectMany(wrapped).Subscribe(_ => { });
            else
                return source.SelectMany(wrapped).Subscribe(_ => { }, handler);
        }
        public static IObservable<MailboxMetric> WhenMessageReceived(this ISubscriber subscriber, RedisChannel channel, IMetricSerializer serializer)
        {
            return Observable.Create<MailboxMetric>(async (obs, ct) =>
            {
                await subscriber.SubscribeAsync(channel, (_, message) =>
                {
                    try
                    {
                        var set = serializer.GetObject(message);
                        foreach (var item in set.Set)
                        {
                            obs.OnNext(item);    
                        }
                        
                    }
                    catch (Exception e)
                    {
                     
                    }
                    
                }).ConfigureAwait(false);

                return Disposable.Create(() => subscriber.Unsubscribe(channel));
            });
        }
    }
}