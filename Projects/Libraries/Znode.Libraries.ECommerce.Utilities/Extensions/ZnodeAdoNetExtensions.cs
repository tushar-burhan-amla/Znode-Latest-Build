using System;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using static System.Convert;

namespace Znode.Libraries.ECommerce.Utilities
{
    public static class ZnodeAdoNetExtensions
    {
        #region DataRow

        public static string InferString(this DataRow dr, string columnName) =>
            (!dr.Table.Columns.Contains(columnName) || dr.IsNull(columnName))? string.Empty: dr[columnName].ToString();

        public static int InferInt(this DataRow dr, string columnName) =>
            (!dr.Table.Columns.Contains(columnName) || dr.IsNull(columnName)) ? 0 : ToInt32(dr[columnName]);

        public static decimal InferDecimal(this DataRow dr, string columnName) =>
            (!dr.Table.Columns.Contains(columnName) || dr.IsNull(columnName)) ? 0 : ToDecimal(dr[columnName]);

        public static bool InferBool(this DataRow dr, string columnName) =>
            (!dr.Table.Columns.Contains(columnName) || dr.IsNull(columnName)) ? false : ToBoolean(dr[columnName]);

        public static decimal? InferDecimal(this DataRow dr, string columnName, decimal? defaultValue) =>
            (!dr.Table.Columns.Contains(columnName) || dr.IsNull(columnName)) ? defaultValue : ToDecimal(dr[columnName]);

        public static int? InferInt(this DataRow dr, string columnName, int? defaultValue) =>
            (!dr.Table.Columns.Contains(columnName) || dr.IsNull(columnName)) ? defaultValue : ToInt32(dr[columnName]);

        public static DateTime InferDate(this DataRow dr, string columnName) =>
            (!dr.Table.Columns.Contains(columnName) || dr.IsNull(columnName)) ? DateTime.Today : ToDateTime(dr[columnName]);

        #endregion

        #region SqlDataReader

        public static string InferString(this SqlDataReader dr, string columnName)
        {
            bool exists = dr.IsColumnExist(columnName);

            if (exists)
            {
                int ordinal = dr.GetOrdinal(columnName);

                if (!dr.IsDBNull(ordinal))
                    return dr[columnName].ToString();
            }

            return string.Empty;
        }

        public static int InferInt(this SqlDataReader dr, string columnName)
        {
            bool exists = dr.IsColumnExist(columnName);

            if (exists)
            {
                int ordinal = dr.GetOrdinal(columnName);

                if (!dr.IsDBNull(ordinal))
                    return ToInt32(dr[columnName]);
            }

            return 0;
        }

        public static decimal InferDecimal(this SqlDataReader dr, string columnName)
        {
            bool exists = dr.IsColumnExist(columnName);

            if (exists)
            {
                int ordinal = dr.GetOrdinal(columnName);

                if (!dr.IsDBNull(ordinal))
                    return ToDecimal(dr[columnName]);
            }

            return 0;
        }

        public static bool InferBool(this SqlDataReader dr, string columnName)
        {
            bool exists = dr.IsColumnExist(columnName);

            if (exists)
            {
                int ordinal = dr.GetOrdinal(columnName);

                if (!dr.IsDBNull(ordinal))
                    return ToBoolean(dr[columnName]);
            }

            return false;
        }


        public static decimal? InferDecimal(this SqlDataReader dr, string columnName, decimal? defaultValue)
        {
            bool exists = dr.IsColumnExist(columnName);

            if (exists)
            {
                int ordinal = dr.GetOrdinal(columnName);

                if (!dr.IsDBNull(ordinal))
                    return ToDecimal(dr[columnName]);
            }

            return defaultValue;
        }

        public static int? InferInt(this SqlDataReader dr, string columnName, int? defaultValue)
        {
            bool exists = dr.IsColumnExist(columnName);

            if (exists)
            {
                int ordinal = dr.GetOrdinal(columnName);

                if (!dr.IsDBNull(ordinal))
                    return ToInt32(dr[columnName]);
            }

            return defaultValue;
        }

        public static DateTime InferDate(this SqlDataReader dr, string columnName)
        {
            bool exists = dr.IsColumnExist(columnName);

            if (exists)
            {
                int ordinal = dr.GetOrdinal(columnName);

                if (!dr.IsDBNull(ordinal))
                    return ToDateTime(dr[columnName]);
            }

            return DateTime.MinValue;
        }

        public static bool IsColumnExist(this SqlDataReader dr, string columnName) =>
            dr.GetSchemaTable().Rows.OfType<DataRow>().Any(x => x["ColumnName"].ToString() == columnName);

        #endregion
    }
}
