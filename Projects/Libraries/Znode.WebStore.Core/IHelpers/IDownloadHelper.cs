using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.WebStore
{
    public interface IDownloadHelper 
    {
        /// <summary>
        /// Downloads the file on the basis of list of data.
        /// </summary>
        /// <typeparam name="T">Any class/model type.</typeparam>
        /// <param name="list">List<T> of any class/model type.</param>
        /// <param name="fileType">String type of file to be downloaded.</param>
        /// <param name="response">Response object to write data to file.</param>
        void ExportDownload<T>(List<T> list, string fileType, HttpResponseBase response, string delimiter = null, string fileName = null, bool hasControl = false, bool displayColumn = true);

        /// <summary>
        /// Converts the generic list to datatable.
        /// </summary>
        /// <typeparam name="TSource">Any class/model type</typeparam>
        /// <param name="data">IList<TSource> data</param>
        /// <param name="IsJson">Set for Json string</param>
        /// <returns>Returns dataTable.</returns>
        DataTable ToDataTable<TSource>(IList<TSource> data, bool IsJson);


        /// <summary>
        /// Exports data to excel file.
        /// </summary>
        /// <param name="strFileName">string name of file</param>
        /// <param name="gridViewControl">Grid view control bound with dataset</param>
        /// <param name="Response">HttpResponseBase object to perform the export.</param>
        void ExportDataToExcel(string strFileName, GridView gridViewControl, HttpResponseBase Response, bool hasControl = false);

        /// <summary>
        ///  Exports data to csv file.
        /// </summary>
        /// <param name="fileName">The value of fileName</param>
        /// <param name="Data">The value of Data</param>
        /// <param name="Response">HttpResponseBase object to perform the export.</param>
        void ExportDataToCSV(string strFileName, byte[] Data, HttpResponseBase Response);
    }
}
