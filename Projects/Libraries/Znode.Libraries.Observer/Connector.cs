using System;
using System.Reflection;

namespace Znode.Libraries.Observer
{
    public sealed class Connector<TModel> : IDisposable
    {
        #region Private Variables
        public readonly MethodInfo MethodInfo;
        private readonly EventAggregator EventAggregator;
        public readonly WeakReference TargetObjet;
        public readonly bool IsStatic;
        private bool isDisposed;
        #endregion

        #region Constructor
        public Connector(Action<TModel> action, EventAggregator eventAggregator)
        {
            MethodInfo = action.Method;
            if (action.Target == null)
                IsStatic = true;
            TargetObjet = new WeakReference(action.Target);
            EventAggregator = eventAggregator;
        }

        ~Connector()
        {
            if (!isDisposed)
                Dispose();
        }

        public void Dispose()
        {
            EventAggregator.Detach(this);
            isDisposed = true;
        }
        #endregion

        #region Public Methods
        //Create Delegate according to attach process
        public Action<TModel> CreateAction()
        {
            if (TargetObjet.Target != null && TargetObjet.IsAlive)
                return (Action<TModel>)Delegate.CreateDelegate(typeof(Action<TModel>), TargetObjet.Target, MethodInfo);
            if (this.IsStatic)
                return (Action<TModel>)Delegate.CreateDelegate(typeof(Action<TModel>), MethodInfo);

            return null;
        }
        #endregion
    }
}
