using System;
using System.Configuration;
using System.Web;
using Znode.Libraries.Data.DataModel;

namespace Znode.Libraries.Data.Helpers
{
    public static class HelperMethods
    {
        /// <summary>
        /// Get Login User Id from Request Headers
        /// </summary>
        /// <returns>Login User Id</returns>
        public static int GetLoginUserId()
        {
            int userId = 0;
            var headers = HttpContext.Current.Request.Headers;

            int.TryParse(headers["Znode-UserId"], out userId);

            int loginAs = GetLoginAdminUserId();
            if (loginAs > 0)
                return loginAs;

            return userId;
        }

        /// <summary>
        /// Get Logged in admin userId from Request Headers
        /// </summary>
        /// <returns>Login User Id</returns>
        public static int GetLoginAdminUserId()
        {
            int loginAs = 0;
            var headers = HttpContext.Current.Request.Headers;

            int.TryParse(headers["Znode-LoginAsUserId"], out loginAs);

            return loginAs;
        }

        /// <summary>
        /// Get sales rep userId from Request Headers
        /// </summary>
        /// <returns>sales rep userId</returns>
        public static int GetSalesRepUserId()
        {
            int saleRepAsUserId = 0;
            var headers = HttpContext.Current.Request.Headers;

            int.TryParse(headers["Znode-SaleRepAsUserId"], out saleRepAsUserId);

            return saleRepAsUserId;
        }

        /// <summary>
        /// Get database connection string
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["ZnodeECommerceDB"].ConnectionString;
            }
        }

        /// <summary>
        /// Get SSRS Reports database connection string
        /// </summary>
        public static string GetSSRSReportConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["ZnodeReportDB"].ConnectionString;
            }
        }

        /// <summary>
        /// Get recommendation engine database connection string
        /// </summary>
        public static string GetRecommendationConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["ZnodeRecommendationDB"].ConnectionString;
            }
        }

        public static bool IsUserWantToDebugSql
        {
            get
            {
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["EnableLinqSQLDebugging"]))
                    return false;
                else
                    return Convert.ToBoolean(ConfigurationManager.AppSettings["EnableLinqSQLDebugging"]);
            }
        }

        //Gets current context object
        public static Znode_Entities CurrentContext
        {
            get
            {
                return GetObjectContext();
            }
        }

        // Get current datetime.
        public static DateTime GetEntityDateTime() => DateTime.Now;

        //Create the Context object, return the context.
        private static Znode_Entities GetObjectContext()
        {
            if (HttpContext.Current != null)
            {
                string objectContextKey = "ocm_" + HttpContext.Current.GetHashCode().ToString("x");
                if (!HttpContext.Current.Items.Contains(objectContextKey))
                    HttpContext.Current.Items.Add(objectContextKey, new Znode_Entities());
                return HttpContext.Current.Items[objectContextKey] as Znode_Entities;
            }
            else
                return new Znode_Entities();
        }


        public static ZnodePublish_Entities Context
        {
            get
            {
              return GetPublishEntityContext();                
            }
        }

        //Create the Context object, return the context.    
        private static ZnodePublish_Entities GetPublishEntityContext()
        {

            if (HttpContext.Current != null)
            {
                string objectContextKey = "publishentityocm_" + HttpContext.Current.GetHashCode().ToString("x");
                if (!HttpContext.Current.Items.Contains(objectContextKey))
                    HttpContext.Current.Items.Add(objectContextKey, new ZnodePublish_Entities());
                return HttpContext.Current.Items[objectContextKey] as ZnodePublish_Entities;
            }
            else
                return new ZnodePublish_Entities();
        }

        public static ZnodeKlaviyoEntities KlaviyoContext
        {
            get
            {
                return GetKlaviyoEntityContext();
            }
        }

        //Create the Context object, return the context.    
        private static ZnodeKlaviyoEntities GetKlaviyoEntityContext()
        {

            if (HttpContext.Current != null)
            {
                string objectContextKey = "klaviyoentityocm_" + HttpContext.Current.GetHashCode().ToString("x");
                if (!HttpContext.Current.Items.Contains(objectContextKey))
                    HttpContext.Current.Items.Add(objectContextKey, new ZnodeKlaviyoEntities());
                return HttpContext.Current.Items[objectContextKey] as ZnodeKlaviyoEntities;
            }
            else
                return new ZnodeKlaviyoEntities();
        }


        //Use this method very cautiously as it will create new DB context object each time which will be a costly operation.
        //This method should only be utilized when you need to access DB context from a thread/async call which is using a different thread than the APIs HTTP request thread.
        //It is recommended to use the new DB context when there is immense need of it otherwise it should be avoided.
        public static IDBContext GetNewDBContext() => new Znode_Entities();
    }
}
