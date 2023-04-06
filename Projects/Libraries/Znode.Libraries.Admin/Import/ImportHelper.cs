using GenericParsing;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Znode.Engine.Api.Models;
using Znode.Libraries.Admin.Import;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Libraries.Admin
{
    public class ImportHelper: IImportHelper
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeImportHead> _znodeImportHead;
        private readonly IZnodeRepository<ZnodeImportTemplate> _znodeImportTemplate;
        private readonly IZnodeRepository<ZnodeImportTemplateMapping> _znodeImportTemplateMapping;
        #endregion

        #region Public Constructor
        public ImportHelper()
        {
            _znodeImportHead = new ZnodeRepository<ZnodeImportHead>();
            _znodeImportTemplate = new ZnodeRepository<ZnodeImportTemplate>();
            _znodeImportTemplateMapping = new ZnodeRepository<ZnodeImportTemplateMapping>();
        }
        #endregion

        #region Public Methods
        //This method will process the data uploaded from the file.
        public virtual int ProcessData(ImportModel model, int userId)
        {
            try
            {
                string uniqueIdentifier = string.Empty;
                bool isRecordPresent = false;
                string tableName = ReadAndCreateTable(model.FileName, model.ImportType, out uniqueIdentifier, out isRecordPresent);
                int templateId = UpsertTemplateDetails(model);
                return ProcessImportData(model, tableName, templateId, uniqueIdentifier, userId);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage($"An error occurred in Import Process. Error - {ex.Message}", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info, ex);
                return 0;
            }
        }

        //Update the data of ZnodeImportHead table
        public virtual int UpdateTemplateData(ImportModel model)
        {
            //delete all mappings against the Template and re-add new one.
            FilterDataCollection filters = new FilterDataCollection();
            filters.Add(FilterKeys.ImportTemplateId, FilterOperators.Equals, Convert.ToString(model.TemplateId));
            bool status = _znodeImportTemplateMapping.Delete(DynamicClauseHelper.GenerateDynamicWhereClause(filters));
            if (status)
            {
                List<ZnodeImportTemplateMapping> entityMappings = new List<ZnodeImportTemplateMapping>();
                foreach (ImportMappingModel mapping in model.Mappings.DataMappings)
                {
                    ZnodeImportTemplateMapping mappingEntity = new ZnodeImportTemplateMapping();
                    mappingEntity.ImportTemplateId = model.TemplateId;
                    mappingEntity.SourceColumnName = string.IsNullOrEmpty(mapping.MapCsvColumn) ? string.Empty : mapping.MapCsvColumn;
                    mappingEntity.TargetColumnName = mapping.MapTargetColumn;

                    entityMappings.Add(mappingEntity);
                }

                var entities = _znodeImportTemplateMapping.Insert(entityMappings);
                return model.TemplateId;
            }
            else
                return 0;
        }

        //check the import status
        public virtual bool CheckImportStatus()
        {
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("@Status", null, ParameterDirection.Output, DbType.Int32);
            int status = 0;

            IList<View_ReturnBoolean> result = objStoredProc.ExecuteStoredProcedureList("Znode_GetImportProcessStatus  @Status OUT", null, out status);

            //check if the records present or not. if no records present then don't show the error message
            if (result.FirstOrDefault().Id.Equals(0))
                return true;
            else
                return result.FirstOrDefault().Status.Value;
        }

        //This method will process the data uploaded from the file.
        public virtual int ProcessProductUpdateData(ImportModel model, int userId, out string importedGuid)
        {
            try
            {
                string uniqueIdentifier = string.Empty;
                bool isRecordPresent = false;
                string tableName = ReadAndCreateTable(model.FileName, model.ImportType, out uniqueIdentifier, out isRecordPresent);
                importedGuid = uniqueIdentifier;
                return isRecordPresent ? ProcessProductUpdateImportData(model, tableName, uniqueIdentifier, userId) : 0;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage($"An error occurred in Import Process. Error - {ex.Message}", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info, ex);
                importedGuid = string.Empty;
                return 0;
            }
        }
        #endregion

        #region Private Methods
        //This method will read the file and create the table with the help of first row.  First row will act as a table column.
        //private string ReadAndCreateTable(string fileName, string importType, out string colNames)
        protected virtual string ReadAndCreateTable(string fileName, string importType, out string uniqueIdentifier, out bool isRecordPresent)
        {
            string tableName = string.Empty;
            uniqueIdentifier = string.Empty;
            DataTable dt = new DataTable();
            isRecordPresent = false;
            try
            {
                using (GenericParserAdapter parser = new GenericParserAdapter(fileName))
                {
                    parser.ColumnDelimiter = ZnodeConstant.ColumnDelimiter;
                    parser.FirstRowHasHeader = true;
                    parser.MaxBufferSize = 32768;
                    dt = parser.GetDataTable();
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage($"Something went wrong. Error message:- {ex.Message}", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info, ex);
                throw;
            }

            //added square [] bracket to avoid special character error.
            string[] columnNames = (from dc in dt.Columns.Cast<DataColumn>()
                                    select dc.ColumnName.Replace(dc.ColumnName,$"[{dc.ColumnName}]")).ToArray();

            if (IsNotNull(columnNames) && columnNames.Any())
            {
                Guid tableGuid = Guid.NewGuid();
                //generate the double hash temp table name
                tableName = $"tempdb..[##{importType}_{tableGuid}]";

                //add the same guid in datatable
                DataColumn guidCol = new DataColumn("guid", typeof(String));
                guidCol.DefaultValue = tableGuid;
                dt.Columns.Add(guidCol);

                //create the double hash temp table
                MakeTable(tableName, GetTableColumnsFromFirstLine(columnNames));

                //assign the guid to out parameter
                uniqueIdentifier = Convert.ToString(tableGuid);
            }

            //Set default value for perticular column from databse
            SetDefaultValueForImportDataTable(importType, dt);

            //If table created and it has headers then dump the CSV data in double hash temp table
            if (HelperUtility.IsNotNull(tableName) && columnNames.Count() > 0)
                SaveDataInChunk(dt, tableName, out isRecordPresent);

            return tableName;
        }

        //If TemplateId greater than 0 then we will update the template details else new template will get saved
        protected virtual int UpsertTemplateDetails(ImportModel model)
            => model.IsPartialPage ? model.TemplateId : model.TemplateId > 0 ? UpdateTemplateData(model) : InsertTemplateData(model);

        //Insert the data in ZnodeImportHead table
        protected virtual int InsertTemplateData(ImportModel model)
        {
            //save template data
            ZnodeImportTemplate entity = new ZnodeImportTemplate();
            entity.ImportHeadId = model.ImportTypeId;
            entity.TemplateName = model.TemplateName;
            entity.TemplateVersion = model.TemplateVersion;
            entity.PimAttributeFamilyId = model.FamilyId;
            entity.PromotionTypeId = model.PromotionTypeId;
            entity.IsActive = true;

            entity = _znodeImportTemplate.Insert(entity);

            //save mappings
            //model.Mappings.DataMappings
            List<ZnodeImportTemplateMapping> entityMappings = new List<ZnodeImportTemplateMapping>();
            foreach (ImportMappingModel mapping in model.Mappings.DataMappings)
            {
                ZnodeImportTemplateMapping mappingEntity = new ZnodeImportTemplateMapping();
                mappingEntity.ImportTemplateId = entity.ImportTemplateId;
                mappingEntity.SourceColumnName = mapping.MapCsvColumn ?? string.Empty;
                mappingEntity.TargetColumnName = mapping.MapTargetColumn;

                entityMappings.Add(mappingEntity);
            }

            var entities = _znodeImportTemplateMapping.Insert(entityMappings);
            return entity.ImportTemplateId;
        }


        //This method will create ##temp table in SQL
        protected virtual void MakeTable(string tableName, string columnList)
        {
            try
            {
                SqlConnection conn = GetSqlConnection();

                SqlCommand cmd = new SqlCommand("Znode_CreateTempTable", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@tableName", tableName);
                cmd.Parameters.AddWithValue("@columnList", columnList);


                if (conn.State.Equals(ConnectionState.Closed))
                    conn.Open();

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage($"Something went wrong. Error message:- {ex.Message}", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info, ex);
                throw;
            }
        }

        //This method will divide the data in chunk.  The chunk size is 5000.
        protected virtual void SaveDataInChunk(DataTable dt, string tableName, out bool isRecordPresent)
        {
            if (IsNotNull(dt) && dt.Rows.Count > 0)
            {
                isRecordPresent = true;
                int chunkSize = int.Parse(ConfigurationManager.AppSettings["ZnodeImportChunkLimit"].ToString());
                int startIndex = 0;
                int totalRows = dt.Rows.Count;
                int totalRowsCount = totalRows / chunkSize;

                if (totalRows % chunkSize > 0)
                    totalRowsCount++;

                for (int iCount = 0; iCount < totalRowsCount; iCount++)
                {
                    DataTable fileData = dt.Rows.Cast<DataRow>().Skip(startIndex).Take(chunkSize).CopyToDataTable();
                    startIndex = startIndex + chunkSize;
                    InsertData(tableName, fileData);
                }
            }
            else
                isRecordPresent = false;
        }

        //This method will save the chunk data in ##temp table using Bulk upload
        protected virtual void InsertData(string tableName, DataTable fileData)
        {
            SqlConnection conn = GetSqlConnection();
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
            {
                bulkCopy.DestinationTableName = tableName;

                if (conn.State.Equals(ConnectionState.Closed))
                    conn.Open();

                bulkCopy.WriteToServer(fileData);
                conn.Close();
            }
        }

        //This method will create the table columns and append the datatype to the column headers
        protected virtual string GetTableColumnsFromFirstLine(string[] firstLine)
        {
            StringBuilder sbColumnList = new StringBuilder();        
            sbColumnList.Append("(");
            sbColumnList.Append(string.Join(" nvarchar(max) , ", firstLine));
            sbColumnList.Append(" nvarchar(max), guid nvarchar(max) )");
            return sbColumnList.ToString();
        }

        //This method will execute the data inserted in the ##temp table
        //private void ProcessImportData(string importType, string tableName, string colNames)
        protected virtual int ProcessImportData(ImportModel model, string tableName, int templateId, string uniqueIdentifier, int userId)
        {
            try
            {
                string errorFileName = string.Empty;
                SqlConnection conn = GetSqlConnection();
                SqlCommand cmd = new SqlCommand("Znode_ImportData", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TableName", tableName);
                cmd.Parameters.AddWithValue("@TemplateId", templateId);
                cmd.Parameters.AddWithValue("@NewGUID", uniqueIdentifier);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@LocaleId", model.LocaleId);
                cmd.Parameters.AddWithValue("@DefaultFamilyId", model.FamilyId);
                cmd.Parameters.AddWithValue("@PriceListId", model.PriceListId);
                cmd.Parameters.AddWithValue("@CountryCode", model.CountryCode);
                cmd.Parameters.AddWithValue("@PortalId", model.PortalId);
                cmd.Parameters.AddWithValue("@IsAccountAddress", model.IsAccountAddress);
                cmd.Parameters.AddWithValue("@PimCatalogId", model.CatalogId);
                cmd.Parameters.AddWithValue("@PromotionTypeId", model.PromotionTypeId);
                if (conn.State.Equals(ConnectionState.Closed))
                    conn.Open();
                cmd.ExecuteReader();
                conn.Close();
                return 1;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Import.ToString(), TraceLevel.Error, ex);
                return 0;
            }
        }

        //This method will provide the SQL connection
        protected virtual SqlConnection GetSqlConnection() => new SqlConnection(HelperMethods.ConnectionString);

        //This method will execute the data inserted in the ##temp table
        //private void ProcessImportData(string importType, string tableName, string colNames)
        protected virtual int ProcessProductUpdateImportData(ImportModel model, string tableName, string uniqueIdentifier, int userId)
        {
            try
            {
                string errorFileName = string.Empty;
                SqlConnection conn = GetSqlConnection();
                SqlCommand cmd = new SqlCommand("Znode_ImportPartialProcessProductData", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TblGUID", uniqueIdentifier);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@IsAutoPublish", model.IsAutoPublish);

                conn.Open();
                cmd.ExecuteReader();
                return 1;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Import.ToString(), TraceLevel.Error, ex.StackTrace);
                return 0;
            }
        }

        //Reset default value for Targeted colume using stored procedure
        protected virtual DataTable GetDefaultValuefromImportAttributeDefaultValue(string importType)
        {
            DataTable defaultValueDataTable = new DataTable();
            try
            {
                SqlConnection conn = GetSqlConnection();
                using (SqlDataAdapter adapter = new SqlDataAdapter("Znode_GetImportReplacedAttributeValue", conn))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adapter.SelectCommand.Parameters.AddWithValue("@ImportAttributeType", importType);
                    adapter.Fill(defaultValueDataTable);
                };

            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Import.ToString(), TraceLevel.Error, ex);
            }
            return defaultValueDataTable;
        }

        //Reset default value for Targeted colume in argumentdatatable from  
        protected virtual DataTable SetDefaultValueForImportDataTable(string importType, DataTable importDataTable)
        {
            DataTable defaultValueDatatable = GetDefaultValuefromImportAttributeDefaultValue(importType);
            if (defaultValueDatatable != null)
            {
                foreach (DataRow defaultValueRow in defaultValueDatatable.Rows)
                {
                    string targetAttributeCode = defaultValueRow[ZnodeConstant.TargetAttributeCode].ToString();
                    bool IstargetAttributeCodeExist = importDataTable.Columns.Contains(targetAttributeCode);

                    if (IstargetAttributeCodeExist)
                    {
                        foreach (DataRow row in importDataTable.Rows)
                        {
                            if (defaultValueRow[ZnodeConstant.AllowAttributeValue].ToString().Contains(row[targetAttributeCode].ToString()))
                            {
                                row[targetAttributeCode] = defaultValueRow[ZnodeConstant.ReplacedAttributeValue].ToString();
                            }
                        }
                    }
                }
            }
            return importDataTable;
        }
        #endregion
    }
}
