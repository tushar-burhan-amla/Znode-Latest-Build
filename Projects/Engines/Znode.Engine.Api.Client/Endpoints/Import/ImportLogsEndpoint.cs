namespace Znode.Engine.Api.Client.Endpoints
{
    public class ImportLogsEndpoint : BaseEndpoint
    {
        //Get the Import logs
        public static string GetImportLogs() => $"{ApiRoot}/importlogs/getimportlogs";

        //Get the Import logs status
        public static string GetImportLogStatus(int importProcessLogId) => $"{ApiRoot}/importlogs/getlogstatus/{importProcessLogId}";

        //Get the import log details
        public static string GetImportLogDetails(int importProcessLogId) => $"{ApiRoot}/importlogs/getimportlogdetails/{importProcessLogId}";

        //Delete the Import log records from ZnodeImportProcessLog as well as ZnodeImportLog table
        public static string DeleteLog(int importProcessLogId) => $"{ApiRoot}/importlogs/delete/{importProcessLogId}";
    }
}
