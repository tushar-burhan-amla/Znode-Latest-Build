using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ImportLogDetailsListResponse : BaseListResponse
    {

        public ImportLogDetailsListResponse()
        {
            LogDetailsList = new List<ImportLogDetailsModel>();
        }

        public List<ImportLogDetailsModel> LogDetailsList { get; set; }
        public ImportLogsModel ImportLogs { get; set; }
    }
}
