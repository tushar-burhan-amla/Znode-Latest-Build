using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Znode.Engine.Api.Client.Endpoints
{
    class ExportEndpoint :  BaseEndpoint
    {
        //Get the Export logs.
        public static string GetExportLogs() => $"{ApiRoot}/export/GetExportLogs";
        #region Delete export files in given duration of web config file settings.
        
        //Delete export files.
        public static string DeleteExportFiles() => $"{ApiRoot}/export/deleteexpiredpaymentaccesstoken";

        //Get export file path.
        public static string GetExportFilePath(string tableName) => $"{ApiRoot}/export/getexportfilepath/{tableName}";

        #endregion
        //Delete the Export log records from ZnodeExportProcessLog as well as ZnodeExportProcessLog table
        public static string DeleteLog() => $"{ApiRoot}/export/delete";
    }
}
