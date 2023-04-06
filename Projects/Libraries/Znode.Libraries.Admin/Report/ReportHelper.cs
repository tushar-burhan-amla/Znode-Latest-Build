using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Libraries.Admin
{
    public class ReportHelper
    {
        #region Private Variables
        readonly ArrayList columnList;
        readonly ReportGenerator reportGenerator;
        readonly ReportUploader reportUploader;
        readonly Dictionary<string, string> parameters;
        private const int IsTestSP = 1;
        #endregion

        #region Constructor
        public ReportHelper()
        {
            columnList = new ArrayList();
            reportGenerator = new ReportGenerator();
            reportUploader = new ReportUploader();
            parameters = new Dictionary<string, string>();
        }
        #endregion

        #region Public Methods
        //Get the Available SSRS Report List
        public static ReportListModel GetReportList()
        {
            ReportListModel model = new ReportListModel();
            ExecuteSpHelper helperService = new ExecuteSpHelper();

            //Set Query to Get available Reports from the Report Database.
            string query = "SELECT Name,[Path],SUBSTRING ( [Path],2,CHARINDEX('/',[Path],CHARINDEX('/',[Path])+1)-2) AS FolderName FROM [dbo].[Catalog] WHERE [Type] = 2 ORDER BY [Path]";

            //Execute the Query & Get Result Set.    
            DataSet result = helperService.GetQueryResultInDataSet(HelperMethods.GetSSRSReportConnectionString, query);
            model.ReportList = result?.Tables[0]?.ToList<ReportModel>()?.FindAll(x => x.FolderName == ZnodeApiSettings.ReportServerDynamicReportFolderName);
            return model;
        }

        //Generate and upload the Dynamic report
        public virtual bool GenerateDynamicReport(DynamicReportModel model, string whereClause, out int errorCode) => GetDynamicReport(model, whereClause, out errorCode);

        //Delete the dynamic report
        public virtual bool DeleteDynamicReport(string reportName) => RemoveDynamicReport(reportName);


        #endregion

        #region Private Methods

        /// <summary>
        /// Delete the dynamic report
        /// </summary>
        /// <param name="reportName">reportName</param>
        /// <returns>bool</returns>
        protected virtual bool RemoveDynamicReport(string reportName) => reportUploader.DeleteReport(reportName);

        /// <summary>
        /// Generate the Dynamic report
        /// </summary>
        /// <param name="model">DynamicReportModel</param>
        /// <returns>bool</returns>
        protected virtual bool GetDynamicReport(DynamicReportModel model,string whereClause, out int errorCode)
        {
            bool isExecuted = ExecuteSP(model, whereClause);
            if (isExecuted)
                return GenerateAndUploadRDLReport(model, out errorCode);
            else
            {
                errorCode = 20;
                ZnodeLogging.LogMessage("Stored Procedure execution failed for SP Name = " + model.StoredProcedureName, ZnodeLogging.Components.DynamicReports.ToString());
                return false;
            }         
        }


        /// <summary>
        /// This method will create the RDLC formatted report and upload it in Report server
        /// </summary>
        /// <param name="model">DynamicReportModel</param>
        /// <returns></returns>
        protected virtual bool GenerateAndUploadRDLReport(DynamicReportModel model, out int errorCode)
        {
            XmlDocument doc = new XmlDocument();
            reportGenerator.GenerateRDLReport(doc, parameters, columnList, model);
            return reportUploader.UploadRDLReport(doc, model.ReportName, out errorCode);
        }

        /// <summary>
        /// Execute the report SP
        /// </summary>
        /// <param name="model">DynamicReportModel</param>
        /// <param name="columnList">String</param>
        /// <returns></returns>
        protected virtual bool ExecuteSP(DynamicReportModel model, string whereClause)
        {
            try
            {
                string commaSepColumns = string.Join(",", model?.Columns?.ColumnList.Select(x => x.ColumnName).ToList());
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ZnodeECommerceDB"].ConnectionString))
                {
                    if(IsNotNull(model))
                    {
                        //Update parameterList for SP in reports
                        model.StoredProcedureName = "Znode_GetDynamicReport";
                        parameters.Add("LocaleId", Convert.ToString(model.LocaleId));
                        parameters.Add("ReportType", model.ReportType);
                        parameters.Add("Columns", commaSepColumns);
                        parameters.Add("WhereClause", whereClause);
                        parameters.Add("CatalogId", Convert.ToString(model.CatalogId));
                        parameters.Add("PriceListId", Convert.ToString(model.PriceId));
                        parameters.Add("WarehouseId", Convert.ToString(model.WarehouseId));

                        if (model.ReportType.Equals("Product") || model.ReportType.Equals("Category"))
                        {
                            SqlCommand command;
                            SqlDataAdapter adpter;
                            DataSet ds;

                            // Executing a query to retrieve a fields list for the report
                            command = conn.CreateCommand();
                            command.CommandText = "Znode_GetDynamicReport";
                            command.CommandType = CommandType.StoredProcedure;

                            // Add parameters
                            command.Parameters.AddWithValue("@LocaleId", model.LocaleId);
                            command.Parameters.AddWithValue("@ReportType", model.ReportType);
                            command.Parameters.AddWithValue("@Columns", commaSepColumns);
                            command.Parameters.AddWithValue("@WhereClause", whereClause);
                            command.Parameters.AddWithValue("@CatalogId", model.CatalogId);
                            command.Parameters.AddWithValue("@PriceListId", model.PriceId);
                            command.Parameters.AddWithValue("@WarehouseId", model.WarehouseId);
                            command.Parameters.AddWithValue("@IscallforTest", IsTestSP);
                            if (conn.State.Equals(ConnectionState.Closed))
                                conn.Open();

                            ds = new DataSet();
                            adpter = new SqlDataAdapter(command);
                            adpter.Fill(ds);

                            foreach (DataColumn dc in ds?.Tables[0]?.Columns)
                                columnList.Add(dc.ColumnName);
                        }
                        else
                        {
                            columnList.AddRange(commaSepColumns.Split(','));
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage($"Error occurred while executing SP for report. Error - {ex.Message}", ZnodeLogging.Components.DynamicReports.ToString(),TraceLevel.Info,ex);
                return false;
            }
        }

        /// <summary>
        /// Get the parameter type to pass to the DB
        /// </summary>
        /// <param name="paramType">paramType</param>
        /// <returns>SqlDbType</returns>
        protected virtual SqlDbType GetDBType(string paramType)
        {
            SqlDbType dbType = new SqlDbType();
            switch (paramType.ToLower())
            {
                case "int":
                    dbType = SqlDbType.Int;
                    break;
                case "datetime":
                    dbType = SqlDbType.DateTime;
                    break;
                case "decimal":
                    dbType = SqlDbType.Decimal;
                    break;
                case "nvarchar":
                    dbType = SqlDbType.NVarChar;
                    break;
                case "bit":
                    dbType = SqlDbType.Bit;
                    break;
            }
            return dbType;
        }

        /// <summary>
        /// Get the comma seperated Columns.
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        protected virtual string GetCommaSeperatedColumns(ReportColumnsListModel columns)
        {
            string colList = string.Empty;

            if (IsNotNull(columns.ColumnList) && columns.ColumnList.Count > 0)
                colList = string.Join(",", columns.ColumnList);

            return colList;
        }

        /// <summary>
        /// Generates the report query
        /// </summary>
        /// <param name="model">DynamicReportModel</param>
        /// <returns>string</returns>
        protected virtual string GenerateReportQuery(DynamicReportModel model)
        {
            FilterDataCollection filters = new FilterDataCollection();

            if (IsNotNull(model.Parameters) && IsNotNull(model.Parameters.ParameterList))
            {
                foreach (ReportParameterModel parameter in model.Parameters.ParameterList)
                {
                    filters.Add(parameter.Name, parameter.Operator, parameter.Value);
                }
            }
            return DynamicClauseHelper.GenerateDynamicWhereClause(filters);
        }

        #endregion
    } 
}
