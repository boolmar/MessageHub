using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MessageHub
{
    public class Subscription<T> : IDisposable
    {
        public readonly MethodInfo methodInfo;
        public readonly WeakReference TargetObject;
        public readonly bool IsStatic;

        private bool isDisposed;

        public Subscription(Action<T> action)
        {
            methodInfo = action.Method;
            if (action.Target == null)
            {
                IsStatic = true;
            }
            TargetObject = new WeakReference(action.Target);
        }

        ~Subscription()
        {
            if (!isDisposed)
            {
                Dispose();
            }
        }

        public Action<T> CreateAction()
        {
            if (TargetObject.Target != null && TargetObject.IsAlive)
            {
                return (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), TargetObject.Target, methodInfo);
            }
            if (this.IsStatic)
            {
                return (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), methodInfo);
            }
            return null;
        }

        public void Dispose()
        {
            isDisposed = true;
        }

    }
}
