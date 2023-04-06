using System;
using System.Linq;

namespace Znode.Libraries.Observer
{
    public sealed class Initializer<TModel> : IDisposable
    {
        private bool isDisposed;
        public Initializer(TModel model): this(model,null)
        {           
        }
        // new initializer accepting operation type as parameter
        public Initializer(TModel model, string operationType=null)
        {
            EventAggregator eve = new EventAggregator();

            Type objectType = SetObjectType();

            Activator.CreateInstance(objectType, eve, operationType);

            eve.Submit(model);
        }

        // new initializer for delete condition where record deleted as per condition
        public Initializer(string condition, string operationType = null)
        {
            EventAggregator eve = new EventAggregator();

            Type objectType = SetObjectType();

            Activator.CreateInstance(objectType, eve, operationType);

            eve.Submit(condition);
        }

        // To set Object Type
        private Type SetObjectType()
        {
            var _assembly = AppDomain.CurrentDomain.GetAssemblies().
                            SingleOrDefault(assembly => assembly.GetName().Name == "Znode.Engine.Connector");

            Type objectType = (from type in _assembly.GetTypes()
                               where type.IsClass && type.Name == "GlobalConnector"
                               select type).Single();
            return objectType;
        }
        ~Initializer()
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
