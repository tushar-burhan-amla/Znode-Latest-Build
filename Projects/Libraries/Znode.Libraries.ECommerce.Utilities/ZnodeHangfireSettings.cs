using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;

using Znode.Engine.Exceptions;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.ECommerce.Utilities
{
    public static class ZnodeHangfireSettings
    {
        private static NameValueCollection settings = ConfigurationManager.GetSection("HangfireConfigSection") as NameValueCollection;

        public static void SetConfigurationSettingSource(NameValueCollection settingSource)
        {
            settings = settingSource;
        }

        public static bool HangfirePrepareSchemaIfNecessary
        {
            get
            {
                return Convert.ToBoolean(settings["HangfirePrepareSchemaIfNecessary"]);
            }
        }

        public static int HangfireCommandBatchMaxTimeout
        {
            get
            {
                return Convert.ToInt32(settings["HangfireCommandBatchMaxTimeout"]);
            }
        }

        public static int HangfireSlidingInvisibilityTimeout
        {
            get
            {
                return Convert.ToInt32(settings["HangfireSlidingInvisibilityTimeout"]);
            }
        }

        public static int HangfireQueuePollInterval
        {
            get
            {
                return Convert.ToInt32(settings["HangfireQueuePollInterval"]);
            }
        }

        public static bool EnableHangfireDashboard
        {
            get
            {
                return Convert.ToBoolean(settings["EnableHangfireDashboard"]);
            }
        }

        public static int HangfireStatsPollingInterval
        {
            get
            {
                int interval = Convert.ToInt32(settings["HangfireStatsPollingInterval"]);

                //return 300000ms or 5 minutes as the default interval.
                return interval == 0 ? 300000 : interval * 60000;
            }
        }

        public static bool MakeHangfireDashboardReadOnly
        {
            get
            {
                return Convert.ToBoolean(settings["MakeHangfireDashboardReadOnly"]);
            }
        }

        public static Dictionary<string, string> HangfireDashboardCredentials
        {
            get
            {
                string credsFromConfig = Convert.ToString(settings["HangfireDashboardCredentials"]);
                if (string.IsNullOrEmpty(credsFromConfig) && EnableHangfireDashboard)
                    LogAndThrow();

                string[] credsFromConfigArray = credsFromConfig.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                Dictionary<string, string> credentials = new Dictionary<string, string>();

                if (credsFromConfigArray.Length > 0)
                {
                    foreach (string credPair in credsFromConfigArray)
                    {
                        //Array of Username:Password
                        string[] creds = credPair.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                        if (creds.Length == 2 && !credentials.ContainsKey(creds[0].Trim()))
                        {
                            credentials.Add(creds[0].Trim(), creds[1].Trim());
                        }
                        else
                            LogAndThrow();
                    }
                }

                return credentials;
            }
        }

        private static void LogAndThrow()
        {
            string errorMessage = "Hangfire dashboard login credentials not configured properly in the API web.config";

            ZnodeLogging.LogMessage(errorMessage, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);

            throw new ZnodeException(ErrorCodes.InvalidData, errorMessage);
        }
    }
}