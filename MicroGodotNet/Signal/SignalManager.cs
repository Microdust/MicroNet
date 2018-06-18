using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroGodotNet.Signal
{
    public static class SignalManager
    {
        private readonly static IDictionary<Type, IList<object>> subscriptions;


        static SignalManager()
        {
            subscriptions = new Dictionary<Type, IList<object>>(20);
        }

        internal static void ClearAll()
        {
            subscriptions.Clear();
        }

        public static void Subscribe<T>(ISignal<T> subscriber)
        {

            IList<object> subscribers;
            if (!subscriptions.TryGetValue(typeof(T), out subscribers))
            {
                subscribers = new List<object>();
                subscriptions.Add(typeof(T), subscribers);
            }

            subscribers.Add(subscriber);
        }

        public static void Unsubscribe<T>(ISignal<T> subscriber)
        {
            IList<object> subscribers;
            if (!subscriptions.TryGetValue(typeof(T), out subscribers))
            {
                return;
            }

            subscribers.Remove(subscriber);

            if (subscribers.Count == 0)
            {
                subscriptions.Remove(typeof(T));
            }
        }

        public static void Signal<T>(T message)
        {
            IList<object> subscribers;
            if (!subscriptions.TryGetValue(typeof(T), out subscribers))
            {
                return;
            }

            for (int i = subscribers.Count - 1; i >= 0; i--)
            {
                var subscriber = subscribers[i] as ISignal<T>;
                subscriber.Execute(message);
            }
        }

    }
}
