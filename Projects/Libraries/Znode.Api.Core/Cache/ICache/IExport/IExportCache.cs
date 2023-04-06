using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Znode.Engine.Api.Cache
{
    public interface IExportCache
    {
        /// <summary>
        /// Get export logs
        /// </summary>
        /// <param name="routeUri">Route Uri</param>
        /// <param name="routeTemplate">Route Template</param>
        /// <returns>string</returns>
        string GetExportLogs(string routeUri, string routeTemplate);
    }
}
