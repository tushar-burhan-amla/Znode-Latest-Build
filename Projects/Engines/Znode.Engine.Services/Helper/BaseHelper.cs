using System.Configuration;
using System.Web;
using Znode.Engine.Payment.Client;

namespace Znode.Engine.Services
{
    public class BaseHelper
    {
        /// <summary>
        /// Get API client object with current domain name and key.
        /// </summary>
        /// <typeparam name="T">The type of API client object.</typeparam>
        /// <returns>An API client object of type T.</returns>
        protected T GetClient<T>(T obj) where T : class 
        {
            if (!(obj is BaseClient)) return obj;
            if (!(obj as BaseClient).IsGlobalAPIAuthorization)
            {
                // Need to use Url.Authority to set domain name and its API key for multi store
                var urlAuthority = HttpContext.Current.Request.Url.Authority;
                (obj as BaseClient).DomainName = urlAuthority;
                (obj as BaseClient).DomainKey = ConfigurationManager.AppSettings[urlAuthority];
            }
            else
            {
                (obj as BaseClient).DomainName = ConfigurationManager.AppSettings["ZnodeApiDomainName"];
                (obj as BaseClient).DomainKey = ConfigurationManager.AppSettings["ZnodeApiDomainKey"];
            }
            return obj;
        }
     }
  }
