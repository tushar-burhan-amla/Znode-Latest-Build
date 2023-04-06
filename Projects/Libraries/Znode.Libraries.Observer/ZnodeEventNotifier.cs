using System;
using System.Linq;

namespace Znode.Libraries.Observer
{
    public  class ZnodeEventNotifier<TModel> : IDisposable
    {
        //ZnodeEventNotifier
        private bool isDisposed;
        private const string defaultEvent = "DefaultEvent";

        public ZnodeEventNotifier(TModel model, string eventName = defaultEvent)
        {
            EventAggregator eve = new EventAggregator();

            var _assembly = AppDomain.CurrentDomain.GetAssemblies().
                            SingleOrDefault(assembly => assembly.GetName().Name == "Znode.Engine.Api");
            Type objectType = (from type in _assembly.GetTypes()
                               where type.IsClass && type.Name == "ZnodeEventObserver"
                               select type).Single();


            Activator.CreateInstance(objectType, eve);

            eve.Submit(model, eventName);
        }


        ~ZnodeEventNotifier()
        {
            if (!isDisposed)
                Dispose();
        }

        public void Dispose()
        {

            isDisposed = true;
        }
    }
}
