using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MessageHub
{
    public class MessageHub : IMessageHub
    {
        private readonly object lockObj = new object();
        private ConcurrentDictionary<Type, IList> Subscriber;

        private Action<Type, object> globalHandler;

        public event EventHandler<MessageHubErrorEventArgs> OnError;

        #region Singleton & Lazy loading

        private static readonly Lazy<IMessageHub> Lazy = new Lazy<IMessageHub>(() => new MessageHub());
        public static IMessageHub Instance { get { return Lazy.Value; } }

        private MessageHub()
        {
            Subscriber = new ConcurrentDictionary<Type, IList>();
        }

        #endregion


        public void Publish<T>(T message)
        {
            Type t = typeof(T);
            IList subList;

            globalHandler?.Invoke(t, message);

            if (Subscriber.ContainsKey(t))
            {
                //lock (lockObj)
                //{
                //}
                bool lockTaken = false;
                try
                {
                    Monitor.Enter(lockObj, ref lockTaken);
                    {
                        subList = new List<Subscription<T>>(Subscriber[t].Cast<Subscription<T>>());

                        foreach (Subscription<T> sub in subList)
                        {
                            sub.CreateAction()?.Invoke(message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    var copy = OnError;
                    copy?.Invoke(this, new MessageHubErrorEventArgs(message, $"Publish<{t.Name}>", ex));
                }
                finally
                {
                    if (lockTaken)
                    {
                        Monitor.Exit(lockObj);
                    }
                }
            }
        }

        public Subscription<T> Subscribe<T>(Action<T> action)
        {
            Type t = typeof(T);
            IList actionList;
            var subscription = new Subscription<T>(action);

            lock (lockObj)
            {
                if (!Subscriber.TryGetValue(t, out actionList))
                {
                    actionList = new List<Subscription<T>>();
                    actionList.Add(subscription);
                    Subscriber.TryAdd(t, actionList);
                }
                else
                {
                    actionList.Add(subscription);
                }
            }
            return subscription;
        }

        public bool UnSubscribe<T>(Subscription<T> subscription)
        {
            bool result = false;
            Type t = typeof(T);
            if (Subscriber.ContainsKey(t))
            {
                lock (lockObj)
                {
                    Subscriber[t].Remove(subscription);
                    if (Subscriber[t].Count == 0)
                    {
                        IList obj;
                        result = Subscriber.TryRemove(t, out obj);
                    }
                }
            }
            return result;
        }

        public bool IsRegistered(Type t)
        {
            return Subscriber.ContainsKey(t);
        }
        public bool IsRegistered<T>()
        {
            Type t = typeof(T);
            return IsRegistered(t);
        }
        public bool IsRegistered<T>(T obj)
        {
            return IsRegistered<T>();
        }

        public int Count
        {
            get
            {
                int _count = 0;

                lock (lockObj)
                {
                    _count = Subscriber.Count;
                }
                return _count;
            }
        }

        public IEnumerable<Type> GetRegisteredType()
        {
            ICollection<Type> result;

            lock (lockObj)
            {
                result = Subscriber.Keys;
                return result;
            }
        }

        public void ClearSubscriptions()
        {
            lock (lockObj)
            {
                Subscriber.Clear();
            }
        }

        public void Dispose()
        {
            globalHandler = null;
            ClearSubscriptions();
        }

        public void RegisterGlobalHandler(Action<Type, object> onMessage)
        {
            globalHandler += onMessage;
        }
    }
}
