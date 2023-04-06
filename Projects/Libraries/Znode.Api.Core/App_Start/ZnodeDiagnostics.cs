using System;
using System.Diagnostics;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api
{
    public static class ZnodeDiagnostics
    {
        private const string logComponentName = "Diagnostics";

        public static void RunDiagnostics()
        {
            try
            {
                //Can not resolve DiagnosticsService by DI because RunDiagnostics method is executed before dependency registration.
                IDiagnosticsService service = new DiagnosticsService();

                service.CheckSqlConnection();
                service.CheckMongoDBLogConnection();
                ZnodeLogging.LogMessage("Startup diagnostics have passed.", logComponentName, TraceLevel.Verbose);
            }
            catch (Exception e)
            {
                ZnodeLogging.LogMessage("Startup diagnostics have failed. See previously logged error(s) for more detail.", logComponentName, TraceLevel.Warning);
                throw e;
            }
        }
    }
}
