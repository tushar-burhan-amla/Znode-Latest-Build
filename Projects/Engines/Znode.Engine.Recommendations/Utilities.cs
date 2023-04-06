using System.Collections.Generic;
using System;
using System.Data.SqlClient;
using Znode.Libraries.Data.Helpers;
using System.Data;
using Znode.Engine.Recommendations.Models;
using Znode.Engine.Recommendations.DataModel;
using System.Web;
using Znode.Libraries.Framework.Business;
using System.Diagnostics;

namespace Znode.Engine.Recommendations
{
    public static class Utilities
    {
        #region Double hash table operations
        //To create a double hash table for storing recommendations data.
        public static string CreateDoubleHashTable()
        {
            Guid tableGuid = Guid.NewGuid();
            string tableName = $"tempdb..[##RecommendationData_{tableGuid}]";
            MakeTable(tableName, GetTableColumnList());
            return tableName;
        }

        //This method will create double hash temp table in SQL.
        private static void MakeTable(string tableName, string columnList)
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
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //This method will provide the SQL connection.
        private static SqlConnection GetSqlConnection()
        {
            SqlConnection sqlConnection = null;
            try
            {
                sqlConnection = new SqlConnection(HelperMethods.GetRecommendationConnectionString);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }

            return sqlConnection;
        }


        //To get the double hash table column lists.
        private static string GetTableColumnList()
        {
            string hashTableColumns =
                "(" +
                "[RecommendationBaseProductsId] bigint, " +
                "[BaseSKU] nvarchar(600), " +
                "[PortalId] int, " +
                "[RecommendationProcessingLogsId] int, " +
                "[RecommendedProductsId] bigint, " +
                "[RecommendedSKU] nvarchar(600), " +
                "[Quantity] numeric(28,6), " +
                "[Occurrence] int" +
                ")";

            return hashTableColumns;
        }

        //This method will save the data in ##temp table using bulk upload
        public static void SaveDataInTempTable(string tableName, DataTable recommendationsData)
        {
            SqlConnection connection = GetSqlConnection();
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
            {
                bulkCopy.DestinationTableName = tableName;

                if (connection.State.Equals(ConnectionState.Closed))
                    connection.Open();

                bulkCopy.WriteToServer(recommendationsData);
                connection.Close();
            }
        }
        #endregion

        #region Map Processed data into data table
        //To map the processed data into data table.
        public static DataTable MapProcessedDataToDataTable(List<RecommendationBaseProductModel> recommendedBaseProductList)
        {
            DataTable processedDataTable = GetProcessedDataTableStructure();

            foreach (RecommendationBaseProductModel baseProduct in recommendedBaseProductList)
            {
                foreach (RecommendedProductModel recommendedProduct in baseProduct.RecommendedProducts)
                {
                    processedDataTable.Rows.Add(baseProduct.RecommendationBaseProductsId, baseProduct.SKU, baseProduct.PortalId, baseProduct.RecommendationProcessingLogsId,
                        recommendedProduct.RecommendedProductsId, recommendedProduct.SKU, recommendedProduct.Quantity, recommendedProduct.Occurrence);
                }
            }

            return processedDataTable;
        }

        //To create data table structure suitable for storing recommendation engine's processed data.
        private static DataTable GetProcessedDataTableStructure()
        {
            DataTable table = new DataTable("RecommendationProcessedData");
            //For storing base product details
            DataColumn RecommendationBaseProductsId = new DataColumn("RecommendationBaseProductsId");
            RecommendationBaseProductsId.DataType = typeof(long);
            RecommendationBaseProductsId.AllowDBNull = true;
            table.Columns.Add(RecommendationBaseProductsId);
            table.Columns.Add("BaseSKU", typeof(string));
            DataColumn PortalId = new DataColumn("PortalId");
            PortalId.DataType = typeof(int);
            PortalId.AllowDBNull = true;
            table.Columns.Add(PortalId);
            table.Columns.Add("RecommendationProcessingLogsId", typeof(int));

            //For saving recommended products details.
            DataColumn RecommendedProductsId = new DataColumn("RecommendedProductsId");
            RecommendedProductsId.DataType = typeof(long);
            RecommendedProductsId.AllowDBNull = true;
            table.Columns.Add(RecommendedProductsId);
            table.Columns.Add("RecommendedSKU", typeof(string));
            DataColumn Quantity = new DataColumn("Quantity");
            Quantity.DataType = typeof(decimal);
            Quantity.AllowDBNull = true;
            table.Columns.Add(Quantity);
            table.Columns.Add("Occurrence", typeof(int));

            return table;
        }
        #endregion

        #region DB context
        //Gets current context object
        public static Znode_Recommendation_Entities CurrentContext
        {
            get
            {
                return GetRecommendationObjectContext();
            }
        }

        //Create the recommendation context object, return the recommendation context.
        private static Znode_Recommendation_Entities GetRecommendationObjectContext()
        {
            
            if (HttpContext.Current != null)
            {
                string objectContextKey = "recommendationocm_" + HttpContext.Current.GetHashCode().ToString("x");
                if (!HttpContext.Current.Items.Contains(objectContextKey))
                    HttpContext.Current.Items.Add(objectContextKey, new Znode_Recommendation_Entities());
                return HttpContext.Current.Items[objectContextKey] as Znode_Recommendation_Entities;
            }
            else
                return new Znode_Recommendation_Entities();
        }
        #endregion 
    }
}