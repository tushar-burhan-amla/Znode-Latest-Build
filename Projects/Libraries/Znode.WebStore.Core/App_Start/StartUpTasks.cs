using Autofac;
using Autofac.Integration.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.WebStore
{
    public class StartUpTasks
    {
        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            //Get IDependencyRegistration Implementation Dependency Classes.
            var drTypes = GetDependencyClasses();
            var drInstances = new List<IDependencyRegistration>();

            foreach (var drType in drTypes)
                drInstances.Add((IDependencyRegistration)Activator.CreateInstance(drType));

            //Sort Classes by Order, to Load it in Auto fac Container.
            drInstances = drInstances.AsQueryable().OrderBy(t => t.Order).ToList();
            foreach (var dependencyRegistrar in drInstances)
                dependencyRegistrar.Register(builder);

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        //Get List of Classes having the IDependencyRegistration Implementation.
        private static IEnumerable<Type> GetDependencyClasses()
        {
            var result = new List<Type>();
            var type = typeof(IDependencyRegistration);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var a in assemblies)
            {
                Type[] types = null;
                try
                {
                    types = a.GetTypes();
                }
                catch
                { }
                if (!Equals(types, null))
                {
                    foreach (var t in types)
                    {
                        if (type.IsAssignableFrom(t) || (type.IsGenericTypeDefinition))
                        {
                            if (!t.IsInterface)
                            {
                                result.Add(t);
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}
