using System.Collections.Generic;
using System.Web.Mvc;
using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;
using System.Linq;
using AutoMapper;
using Znode.Engine.Exceptions;
using Znode.Libraries.Resources;

namespace Znode.Libraries.ECommerce.Utilities
{
    public static class ZnodeDependencyResolver
    {
        /// <summary>
        /// Resolves singly registered components.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Instance of component.</returns>
        public static T GetService<T>() where T : class
        {
            return (T)DependencyResolver.Current.GetService(typeof(T));
        }

        /// <summary>
        /// Resolves singly registered components having parameterized constructors.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters">Accepts any number of parameters of type ZnodeNamedParameter or ZnodeTypedParameter</param>
        /// <returns>Instance of component.</returns>
        public static T GetService<T>(params object[] parameters) where T : class
        {
            List<Parameter> parameterList = GetParameterList(parameters);
            AutofacDependencyResolver autofacDependencyResolver = (AutofacDependencyResolver)DependencyResolver.Current;
            return autofacDependencyResolver.RequestLifetimeScope.Resolve<T>(parameterList);
        }

        /// <summary>
        /// Resolves singly registered keyed components.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns>Instance of component.</returns>
        public static T GetKeyedService<T>(object key) where T : class
        {
            AutofacDependencyResolver autofacDependencyResolver = (AutofacDependencyResolver)DependencyResolver.Current;
            return autofacDependencyResolver.RequestLifetimeScope.ResolveKeyed<T>(key); 
        }

        /// <summary>
        /// Resolves singly registered keyed components having parameterized constructors.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="parameters"></param>
        /// <returns>Instance of component.</returns>
        public static T GetKeyedService<T>(object key, params object[] parameters) where T : class
        {
            List<Parameter> parameterList = GetParameterList(parameters);
            AutofacDependencyResolver autofacDependencyResolver = (AutofacDependencyResolver)DependencyResolver.Current;
            return autofacDependencyResolver.RequestLifetimeScope.ResolveKeyed<T>(key, parameterList);
        }

        /// <summary>
        /// Resolves list of services registered with given type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetServices<T>() where T : class
        {
            return (IEnumerable<T>)DependencyResolver.Current.GetServices(typeof(T));
        }


        /// <summary>
        /// To get list of type Parameter.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>List of type Parameter.</returns>
        private static List<Parameter> GetParameterList(params object[] parameters)
        {
            List<Parameter> parameterList = new List<Parameter>();
            foreach (object parameter in parameters)
            {
                if (HelperUtility.IsNotNull(parameter) && (parameter.GetType().Equals(typeof(ZnodeNamedParameter)) || parameter.GetType().Equals(typeof(ZnodeTypedParameter))))
                    parameterList.Add(GetMapping(parameter));
                else
                    throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorInvalidParameter);
            }
            return parameterList;
        }

        /// <summary>
        /// To get mapping.
        /// </summary>
        /// <param name="source"></param>
        /// <returns>Parameter type instance, NamedParameter or TypedParameter.</returns>
        private static Parameter GetMapping(object source)
        {
            var sourceType = source.GetType();

            var destinationType = Mapper.GetAllTypeMaps().
                Where(map => map.SourceType == sourceType).
                Single().DestinationType;

            return (Parameter)Mapper.Map(source, sourceType, destinationType);
        }

       
    }
}
