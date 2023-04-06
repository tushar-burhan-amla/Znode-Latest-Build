using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Znode.Libraries.Caching.ElasticSearch;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.Caching
{
    public class ElasticsearchCacheEventReader : ICacheEventReader
    {
        private static readonly string CacheEventsAssemblyName = "Znode.Libraries.Caching.Events";
        private static IDictionary<string, MethodInfo> JsonConvertDeserializeMethodByType = new Dictionary<string, MethodInfo>();

        static ElasticsearchCacheEventReader()
        {
            try
            {
                var cacheEventTypes = GetCacheEventTypes();
                JsonConvertDeserializeMethodByType = GetTypedDeserializeMethodInfos(cacheEventTypes);
            }
            catch (Exception e)
            {
                ZnodeLogging.LogMessage($"Failed to initialize cache event reader", CacheFrameworkConstants.ZnodeLogComponent, TraceLevel.Error, e);
            }
        }

        private static List<Type> GetCacheEventTypes()
        {
            Assembly assembly = Assembly.Load(CacheEventsAssemblyName);
            return assembly?.GetTypes()?.Where(type => type.IsSubclassOf(typeof(BaseCacheEvent))).ToList();
        }

        private static Dictionary<string, MethodInfo> GetTypedDeserializeMethodInfos(List<Type> deserializedTypes)
        {
            var methodInfos = typeof(JsonConvert).GetMethods();
            var JsonConvertDeserializeMethod = methodInfos.First(info =>
                info.Name.Equals("DeserializeObject") &&
                info.IsGenericMethod &&
                info.GetParameters().Length == 1 &&
                info.GetParameters()[0].ParameterType.Equals(typeof(string)));

            var deserializeMethodInfos = new Dictionary<string, MethodInfo>();
            foreach (var innerCacheEventType in deserializedTypes)
            {
                var genericMethodInfo =
                    JsonConvertDeserializeMethod.MakeGenericMethod(new Type[] { innerCacheEventType });
                deserializeMethodInfos.Add(innerCacheEventType.Name, genericMethodInfo);
            }

            return deserializeMethodInfos;
        }

        public List<BaseCacheEvent> ReadEvents(DateTime minDateTime, DateTime maxDateTime)
        {
            var unwrappedCacheEvents = new List<BaseCacheEvent>();

            try
            {
                var cacheEvents = ElasticSearchHelper.QueryDocuments<WrappedCacheEvent>(CacheFrameworkSettings.GetCacheEventIndexName(), minDateTime, maxDateTime);

                foreach (var cacheEvent in cacheEvents)
                {
                    string typeName = cacheEvent.CacheEventType;
                    string json = cacheEvent.CacheEventAsJson;
                    try
                    {
                        if (!JsonConvertDeserializeMethodByType.ContainsKey(typeName))
                        {
                            throw new Exception($"Couldn't deserialize cache event of type '{typeName}' because class was not found with reflection in '{CacheEventsAssemblyName}' assembly on startup.");
                        }
                        MethodInfo deserializeMethod = JsonConvertDeserializeMethodByType[typeName];

                        BaseCacheEvent evt = deserializeMethod.Invoke(null, new object[] { json }) as BaseCacheEvent;
                        if (evt == null)
                        {
                            throw new Exception($"Failed to deserialize cache event of type '{typeName}' from JSON '${json}' because it is null.");
                        }
                        else
                        {
                            unwrappedCacheEvents.Add(evt);
                        }
                    }
                    catch (Exception e)
                    {
                        ZnodeLogging.LogMessage($"Failed to deserialize cache event of type '{typeName}' from JSON '${json}'", CacheFrameworkConstants.ZnodeLogComponent, TraceLevel.Error, e);
                    }
                }
            }
            catch (Exception e)
            {
                ZnodeLogging.LogMessage($"Failed to read cache events.", CacheFrameworkConstants.ZnodeLogComponent, TraceLevel.Error, e);
            }

            return unwrappedCacheEvents;
        }
    }
}
