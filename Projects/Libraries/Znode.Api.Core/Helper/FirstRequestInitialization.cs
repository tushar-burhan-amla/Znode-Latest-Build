using System;
using System.Web;
using Znode.Engine.Api.Models;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Api.Core
{
    public static class FirstRequestInitialization
    {
        private static string host = null;

        private static Object s_lock = new Object();

        // Initialize only on the first request
        public static void Initialize()
        {
            if (string.IsNullOrEmpty(host))
            {
                lock (s_lock)
                {
                    if (string.IsNullOrEmpty(host))
                    {
                        Uri uri = HttpContext.Current.Request.Url;
                        host = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;
                        IMediaConfigurationService mediaConfigurationService = GetService<IMediaConfigurationService>();
                        MediaConfigurationModel mediaConfiguration = mediaConfigurationService.GetDefaultMediaConfiguration();

                        if (mediaConfiguration?.Server == "Local")
                        {
                            MediaConfigurationModel updatedResult = mediaConfigurationService.Update(mediaConfiguration);
                        }
                    }
                }
            }
        }
    }
}
