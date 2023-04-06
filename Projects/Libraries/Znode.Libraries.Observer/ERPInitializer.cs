using System;
using System.Linq;
using System.Reflection;

namespace Znode.Libraries.Observer
{
    public class ERPInitializer<TEntity> : IDisposable
    {
        private bool isDisposed;
        public object Result { get; set; }
        public ERPInitializer(TEntity model, string touchPoint)
        {

            Assembly assembly = Assembly.Load("Znode.Engine.Services");
            Type className = (assembly?.GetTypes()).FirstOrDefault(g => g.Name == "ERPConfiguratorService");

            //Create Instance of active class
            object instance = Activator.CreateInstance(className);

            //Get Method Information from class
            MethodInfo info = className?.GetMethod("GetActiveERPClassName");

            //Calling method with null parameter
            var activeErp = info?.Invoke(instance, null);

            if (!string.IsNullOrEmpty(Convert.ToString(activeErp)))
            {

                Assembly _erpAssembly = Assembly.Load("Znode.Engine.ERPConnector");
                Type _erpClassName = (_erpAssembly?.GetTypes()).FirstOrDefault(g => g.Name == Convert.ToString(activeErp));
                if (!Equals(_erpClassName, null))
                {
                    //Create Instance of active class
                    object _erpInstance = Activator.CreateInstance(_erpClassName);

                    //Get Method Information from class
                    MethodInfo _touchPointInfo = _erpClassName?.GetMethod(touchPoint);

                    if (!Equals(_touchPointInfo, null))
                    {
                        var _attr = _touchPointInfo?.GetCustomAttributes();

                        string _attributeInfo = Convert.ToString(_attr?.FirstOrDefault().GetType().GetProperty("Name").GetValue(_attr?.FirstOrDefault(), null));

                        if (Equals(_attributeInfo, "RealTime"))
                        {
                            //Calling method with null parameter
                            Result = _touchPointInfo?.Invoke(_erpInstance, new object[] { model });
                        }
                    }
                }
            }
        }

        ~ERPInitializer()
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
