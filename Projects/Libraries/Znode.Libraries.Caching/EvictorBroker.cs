using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.Caching
{
    /// <summary>
    /// Provides ability to invoke eviction logic on 'evictor' classes dynamically based on provided
    /// cache event. Reflection is heavily used to achieve this.
    /// </summary>
    public class EvictorBroker : IEvictorBroker
    {
        private IDictionary<string, MethodInfo> EvictorMethods = new Dictionary<string, MethodInfo>();
        private IDictionary<string, Type> EvictorTypes = new Dictionary<string, Type>();

        public EvictorBroker()
        {

        }
        
        public void RegisterEvictor(Type evictorType)
        {
            
            // Get the cache event type that this evictor can handle (the cache event type that this evictor
            // can evict data for):
            MethodInfo methodInfo = evictorType.GetMethod(BaseCacheEventEvictor<BaseCacheEvent>.GetCacheEventTypeMethodName);
            object evictor = Activator.CreateInstance(evictorType);
            object result = methodInfo.Invoke(evictor, new object[0]);
            Type cacheEventType = (Type)result;

            // We enforce a naming convention such that if, for example, a cache event is named "MyEvent", then
            // an evictor that will handle that event must be named "MyEvent_Evictor".
            // 
            // Following this convention allows a developer to write less code (because they don't have to
            // manually register anything), but it also means that they can't afford to misspell anything.
            //
            // To help prevent a developer from a spelling error, validate that the evictor is named appropriately
            // based on the event type it handles:
            string expectedCacheEvictorClassName = $"{cacheEventType.Name}{BaseCacheEventEvictor<BaseCacheEvent>.EvictorSuffix}";
            bool evictorNameIsValid = evictorType.Name.Equals(expectedCacheEvictorClassName);
            if (!evictorNameIsValid)
            {

                // Log error to the application logs:
                string errorMessage = $"Error, evictor '{evictorType.Name}' should be named " +
                                      $"'{expectedCacheEvictorClassName}' to follow convention and " +
                                      $"handle events of type '{cacheEventType.Name}'.";
                ZnodeLogging.LogMessage(errorMessage, CacheFrameworkConstants.ZnodeLogComponent, TraceLevel.Error);

                // Also throw exception to try to get the developer's attention as much as possible:
                throw new Exception(errorMessage);
            }

            // Now that we know the type of the cache event that this evictor can handle, we need to store
            // some more information about the evictor to allow us to efficiently use the evictor to handle
            // cache events at runtime.
            //
            // First, store a reference the evictor's class "type". This is to allow us to instantiate an instance
            // of the evictor to handle each cache event when necessary.
            // 
            // Second, store a reference to the evictor's "eviction entry point" method, which we will call to ask
            // the newly created instance to handle the cache event and evict necessary data:
            if (EvictorTypes.ContainsKey(cacheEventType.Name))
            {
                EvictorTypes.Remove(cacheEventType.Name);
            }
            EvictorTypes.Add(cacheEventType.Name, evictorType);
            MethodInfo evictMethodInfo = evictorType.GetMethod(BaseCacheEventEvictor<BaseCacheEvent>.HandleCacheEventMethodName);
            if (EvictorMethods.ContainsKey(cacheEventType.Name))
            {
                EvictorMethods.Remove(cacheEventType.Name);
            }
            EvictorMethods.Add(cacheEventType.Name, evictMethodInfo);
        }

        public void InvokeEvictor(BaseCacheEvent cacheEvent)
        {
            try
            {

                // We have a cache event that may or may not cause data to be evicted from the currently executing process.
                // If we find that we do have an "evictor" that wants to evict any data that might have become stale as a result
                // of this cache event, then instantiate and instance of the evictor and tell it to handle the cache event:
                Type cachEventType = cacheEvent.GetType();
                if (EvictorTypes.ContainsKey(cachEventType.Name))
                {
                    var evictMethodInfo = EvictorMethods[cachEventType.Name];
                    Type evictorType = EvictorTypes[cachEventType.Name];
                    object evictor = Activator.CreateInstance(evictorType);
                    evictMethodInfo.Invoke(evictor, new object[] {cacheEvent});
                }
            }
            catch (Exception e)
            {
                ZnodeLogging.LogMessage($"Failed to evict data for cache event with ID '${cacheEvent.CacheEventId}'", CacheFrameworkConstants.ZnodeLogComponent, TraceLevel.Error, e);
            }
        }
    }
}
