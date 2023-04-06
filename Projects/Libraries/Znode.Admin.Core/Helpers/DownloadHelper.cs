using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Admin.Helpers
{
    public class DownloadHelper
    {
        #region Public Methods

        /// <summary>
        /// Downloads the file on the basis of list of data.
        /// </summary>
        /// <typeparam name="T">Any class/model type.</typeparam>
        /// <param name="list">List<T> of any class/model type.</param>
        /// <param name="fileType">String type of file to be downloaded.</param>
        /// <param name="response">Response object to write data to file.</param>
        public void ExportDownload<T>(List<T> list, string fileType, HttpResponseBase response, string delimiter = null, string fileName = null, bool hasControl = false)
        {
            DataSet source = new DataSet();
            if (!Equals(list, null))
            {
                DataTable dataTable = ToDataTable(list, true);
                if (!Equals(dataTable, null))
                    source.Tables.Add(dataTable);
            }
            else
                source = null;
            CallSpecificDownloadOption(fileType, source, response, delimiter, fileName, hasControl);
        }

        /// <summary>
        /// return file stream in CSV/Excel format
        /// </summary>
        /// <typeparam name="T">Any class/model type.</typeparam>
        /// <param name="list">List<T> of any class/model type.</param>
        /// <param name="fileType">String type of file to be downloaded.</param>
        /// <param name="response">Response object to write data to file.</param>
        /// <returns>String - stream of data in CSV/Excel Format</returns>
        public string ExportDownload<T>(string fileType, List<T> list, HttpResponseBase response, string delimiter = null, string fileName = null, bool hasControl = false)
        {
            DataSet source = new DataSet();
            if (!Equals(list, null))
            {
                DataTable dataTable = ToDataTable(list, true);
                if (!Equals(dataTable, null))
                    source.Tables.Add(dataTable);
            }
            else
                source = null;
            return CallSpecificDownloadOption(source, fileType, hasControl);
        }

        /// <summary>
        /// Converts the generic list to datatable.
        /// </summary>
        /// <typeparam name="TSource">Any class/model type</typeparam>
        /// <param name="data">IList<TSource> data</param>
        /// <param name="IsJson">Set for Json string</param>
        /// <returns>Returns dataTable.</returns>
        public DataTable ToDataTable<TSource>(IList<TSource> data, bool IsJson)
        {
            DataTable dataTable = new DataTable();
            try
            {
                string jsonString = JsonConvert.SerializeObject(data);
                dataTable = JsonConvert.DeserializeObject<DataTable>(jsonString);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return dataTable;
        }

        /// <summary>
        ///  Exports data to csv file.
        /// </summary>
        /// <param name="fileName">The value of fileName</param>
        /// <param name="Data">The value of Data</param>
        /// <param name="Response">HttpResponseBase object to perform the export.</param>
        public void ExportDataToCSV(string strFileName, byte[] Data, HttpResponseBase Response)
        {
            Response.Clear();
            Response.ClearHeaders();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=\"" + strFileName + "\"");
            Response.AddHeader("Content-Type", "application/Excel");

            // Set text as the primary format.
            Response.ContentType = "text/csv";
            Response.ContentType = "application/vnd.xls";
            Response.AddHeader("Pragma", "public");

            Response.BinaryWrite(Data);
            Response.End();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Call the specific download method on the basis of file type provided.
        /// </summary>
        /// <param name="fileType">String type of file to be downloaded</param>
        /// <param name="source">Dataset having data</param>
        /// <param name="Response">HttpResponseBase object to perform the export.</param>
        private void CallSpecificDownloadOption(string fileType, DataSet source, HttpResponseBase response, string delimiter, string fileName, bool hasControl = false)
        {
            if (!Equals(source, null))
            {
                switch (fileType)
                {
                    case "1":
                        GridView gridView = new GridView();
                        gridView.DataSource = source;
                        gridView.DataBind();
                        ExportDataToExcel(fileName, gridView, response, hasControl);
                        break;
                    case "2":
                        string strData = ExportToCSV(source);
                        byte[] byteData = Encoding.ASCII.GetBytes(strData);
                        ExportDataToCSV(fileName, byteData, response);
                        break;
                }
            }
        }

        /// <summary>
        /// Call the specific download method on the basis of file type provided.
        /// </summary>
        /// <param name="fileType">String type of file to be downloaded</param>
        /// <param name="source">Dataset having data</param>
        /// <param name="Response">HttpResponseBase object to perform the export.</param>
        /// <returns>String - stream of data in CSV/Excel Format</returns>
        private string CallSpecificDownloadOption(DataSet source, string fileType, bool hasControls = false)
        {
            if (!Equals(source, null))
            {
                switch (fileType)
                {
                    case "1":
                        GridView gridView = new GridView();
                        gridView.DataSource = source;
                        return ExportToExcel(gridView, hasControls);
                    case "2":
                        return ExportToCSV(source);
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// Convert Grid Data to Excel
        /// </summary>
        /// <param name="gridView"></param>
        /// <param name="hasControls"></param>
        /// <returns>excel string</returns>
        private string ExportToExcel(GridView gridView, bool hasControls)
        {
            gridView.DataBind();
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            if (hasControls)
                RemoveGridControls(ref gridView);
            gridView.RenderControl(htw);
            //Remove Html Entities & white spaces from string
            string excelString = HttpUtility.HtmlDecode(sw.ToString());
            return excelString.Replace("<td> </td>", "<td></td>");
        }

        /// <summary>
        /// Exports data to excel file.
        /// </summary>
        /// <param name="strFileName">string name of file</param>
        /// <param name="gridViewControl">Grid view control bounded with dataset</param>
        /// <param name="Response">HttpResponseBase object to perform the export.</param>
        public void ExportDataToExcel(string strFileName, GridView gridViewControl, HttpResponseBase Response, bool hasControl = false)
        {
            Response.Clear();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=\"" + strFileName + "\"");

            // Set Excel as the primary format.
            Response.AddHeader("Content-Type", "application/Excel");
            Response.ContentType = "application/Excel";
            System.IO.StringWriter sw = new System.IO.StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            if (hasControl)
                RemoveGridControls(ref gridViewControl);
            gridViewControl.RenderControl(htw);
            Response.Write(sw.ToString());
            gridViewControl.Dispose();
            Response.End();
        }

        /// <summary>
        ///  Returns string for a given Dataset Values.
        /// </summary>
        /// <param name="ds">DataSet to export.</param>
        /// <param name="exportColumnHeadings">Indicates whether to export column has headings or not.</param>
        /// <param name="delimeter">Column delimeter string.</param>
        /// <param name="constraint">Filter constraint.</param>
        /// <returns>Returns the string in CSV format.</returns>
        private string ExportToCSV(DataSet dataset)
        {
            StringBuilder sb = new StringBuilder();

            IEnumerable<string> columnNames = dataset.Tables[0].Columns.Cast<DataColumn>().
                                              Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));
            // Regex to find html elements
            Regex regEx = new Regex("<[^>]*>");
            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field =>
                   regEx.Replace(string.Concat("\"", field.ToString().Replace("\"", "\"\""), "\""), ""));
                sb.AppendLine(string.Join(",", fields));
            }
            //Remove Html Entities from string
            return HttpUtility.HtmlDecode(sb.ToString());
        }
   
        /// <summary>
        /// Method to Remove the Checkbox Controls from the Grid View, So that the Downloaded excel dont show the checkbox in it.
        /// </summary>
        /// <param name="gridViewControl">Control of type GridView</param>
        private void RemoveGridControls(ref GridView gridViewControl)
        {
            // Remove controls from Column Headers
            if (gridViewControl.HeaderRow != null && gridViewControl.HeaderRow.Cells != null)
            {
                for (int rw = 0; rw < gridViewControl.Rows.Count; rw++)
                {
                    GridViewRow row = gridViewControl.Rows[rw];
                    for (int ct = 0; ct < row.Cells.Count; ct++)
                    {
                        // Save header text if found.
                        string headerText = row.Cells[ct].Text;

                        // Check for controls in header.
                        if (row.Cells[ct].HasControls())
                        {
                            // Check for link buttons (used in sorting).
                            if (row.Cells[ct].Controls[0].GetType().ToString() == "System.Web.UI.WebControls.CheckBox")
                                // Link button found, get text.
                                headerText = ((CheckBox)row.Cells[ct].Controls[0]).Checked.ToString();

                            // Remove controls from header.
                            row.Cells[ct].Controls.Clear();
                        }

                        // Reassign header text
                        row.Cells[ct].Text = headerText;
                    }
                }
            }
        }

        #endregion
    }
}