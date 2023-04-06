using System;
using System.Configuration.Provider;
using System.Diagnostics;
using System.Web;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.ECommerce.Utilities
{
    public class ZnodeCacheDependencyProviderSQL : ZnodeCacheDependencyProvider
    {
        #region Public Methods

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            if ((Equals(config, null)) || (Equals(config.Count, 0)))
            {
                throw new ArgumentNullException("You must supply a valid configuration dictionary.");
            }

            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Default File storage provider ");
            }

            // Let ProviderBase perform the basic initialization
            base.Initialize(name, config);

            // Check to see if any attributes were set in configuration that does not need for file writing.
            if (config.Count > 0)
            {
                string extraAttribute = config.GetKey(0);
                if (!String.IsNullOrEmpty(extraAttribute))
                {
                    throw new ProviderException($"The following unrecognized attribute was found in { Name }'s configuration: '{extraAttribute}'");
                }
                else
                {
                    throw new ProviderException("An unrecognized attribute was found in the provider's configuration.");
                }
            }
        }

        // Inserts the cache for the given key with the object value
        public override void Insert(string key, object objValue, params string[] tableNames)
        {
            try
            {
                Insert(key, objValue, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, tableNames);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                throw;
            }
        }

        // Inserts the cache for the given key with the object value
        public override void Insert(string key, object objValue, DateTime absoluteExpiration, TimeSpan timeSpan, params string[] tableNames)
        {
            try
            {
                // Add SQL Cache dependency for standard install.
                System.Web.Caching.AggregateCacheDependency aggregateDependency = new System.Web.Caching.AggregateCacheDependency();



                if (!Equals(tableNames, null))
                {
                    foreach (string tableName in tableNames)
                    {
                        aggregateDependency.Add(new System.Web.Caching.SqlCacheDependency("ZnodeMultifront", tableName));
                    }
                }
                else
                {
                    aggregateDependency = null;
                }

                System.Web.HttpRuntime.Cache.Insert(key, objValue, aggregateDependency, absoluteExpiration, timeSpan);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                throw ex;
            }
        }

        // Clears the cache
        public override void Remove(string key)
        {
            try
            {
                HttpContext.Current.Cache.Remove(key);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                throw;
            }
        }

        #endregion
    }
}
