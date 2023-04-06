using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Znode.Libraries.Data.Helpers
{
    public static class DynamicClauseHelper
    {
        static ZnodeDynamicClauseHelper DynamicClause
        {
            get
            {
                return new ZnodeDynamicClauseHelper();
            }
        }

        #region Public Methods

        #region Clauses for Entity Framework

        //To generate dynamic where clause for Entity Framework with filterValue.
        public static EntityWhereClauseModel GenerateDynamicWhereClauseWithFilter(FilterDataCollection filters)
        {
            return DynamicClause.GenerateDynamicWhereClauseWithFilter(filters);
        }

        //To generate dynamic order by clause for Entity Framework.-
        public static string GenerateDynamicOrderByClause(NameValueCollection sorts)
        {
            return DynamicClause.GenerateDynamicOrderByClause(sorts);
        }

        #endregion

        #region Clauses for SP
        //To generate query based on Input filter parameters
        public static string GenerateDynamicWhereClause(FilterDataCollection filters)
        {
            return DynamicClause.GenerateDynamicWhereClause(filters);
        }

        /// <summary>
        /// To Generate dynamic where clause for stored procedure.
        /// </summary>
        /// <param name="filters">FilterCollection filters</param>
        /// <returns>Return the where clause string.</returns>
        public static string GenerateDynamicWhereClauseForSP(FilterDataCollection filters)
        {
            return DynamicClause.GenerateDynamicWhereClauseForSP(filters);
        }

        /// <summary>
        /// To Generate dynamic where clause for stored procedure.
        /// </summary>
        /// <param name="filter">FilterDataTuple</param>
        /// <returns>Return the where clause string.</returns>
        public static string GenerateWhereClauseForSP(FilterDataTuple filter)
        {
            return DynamicClause.GenerateWhereClauseForSP(filter);
        }

        /// <summary>
        /// To Generate dynamic filter list with key value pair 
        /// </summary>
        /// <param name="filters">FilterCollection filters</param>
        /// <returns>Return the filter list</returns>
        public static List<Tuple<string, string>> GenerateDynamicFilterTupleForSP(FilterDataCollection filters)
        {
            return DynamicClause.GenerateDynamicFilterTupleForSP(filters);
        }
        
        public static string GenerateWhereClauseForSP(FilterDataCollection filters)
        {
            return DynamicClause.GenerateWhereClauseForSP(filters);
        }

        public static string GenerateWhereClauseForPublishStatusFilter(FilterDataCollection filters)
        {
           return DynamicClause.GenerateWhereClauseForPublishStatusFilter(filters);
        }

        /// <summary>
        /// To generate the where clause for custom reports
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static string GenerateReportsWhereClauseForSP(FilterDataTuple filter)
        {
            return DynamicClause.GenerateReportsWhereClauseForSP(filter);
        }

        #endregion

        #endregion

    }
}
