using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;

using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Cache
{
   public  class ExportCache : BaseCache, IExportCache
    {
        private readonly IExportService _service;
        public ExportCache(IExportService exportService)
        {
            _service = exportService;
        }
        //Get the export logs
        public virtual string GetExportLogs(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (HelperUtility.IsNull(data))
            {
                //Get Inventory list.
                ExportLogsListModel exportLogsData = _service.GetExportLogs(Expands, Filters, Sorts, Page);

                //Create response.
                ExportLogsListResponse response = new ExportLogsListResponse { LogsList = exportLogsData?.ExportLogs?.ToList() };

                //apply pagination parameters.
                response.MapPagingDataFromModel(exportLogsData);

                //Insert data in cache
                data = InsertIntoCache(routeUri, routeTemplate, response);
            }
            return data;
        }
    }
}
