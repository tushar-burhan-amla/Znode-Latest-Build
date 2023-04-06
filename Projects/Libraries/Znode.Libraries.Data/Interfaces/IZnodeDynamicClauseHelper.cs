using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Znode.Libraries.Data.Helpers;

namespace Znode.Libraries.Data.Interfaces
{
    public interface IZnodeDynamicClauseHelper
    {
        #region Clauses for entity framework

        /// <summary>
        /////To generate dynamic where clause for Entity Framework with filterValue.
        /// </summary>
        /// <param name="filters"></param>
        /// <returns>Return dynamic where clause Entity model with filter</returns>
        EntityWhereClauseModel GenerateDynamicWhereClauseWithFilter(FilterDataCollection filters);

        /// <summary>
        /// To generate dynamic order by clause for Entity Framework.
        /// </summary>
        /// <param name="sorts"></param>
        /// <returns>Return dynamic order by clause string for Entity Framework.</returns>
        string GenerateDynamicOrderByClause(NameValueCollection sorts);

        #endregion

        #region Clauses for SP

        /// <summary>
        /// To generate dynamic where clause for Entity Framework with filterValue
        /// </summary>
        /// <param name="filters"></param>
        /// <returns>Return dynamic where clause string for Entity Framework with filterValue</returns>
        string GenerateDynamicWhereClause(FilterDataCollection filters);

        /// <summary>
        /// To Generate dynamic where clause for stored procedure.
        /// </summary>
        /// <param name="filters"></param>
        /// <returns>Return the dynamic where clause string.</returns>
        string GenerateDynamicWhereClauseForSP(FilterDataCollection filters);

        /// <summary>
        ///  To Generate dynamic where clause for stored procedure.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>Return the where clause string</returns>
        string GenerateWhereClauseForSP(FilterDataTuple filter);

        /// <summary>
        /// To Generate dynamic filter list with key value pair 
        /// </summary>
        /// <param name="filters"></param>
        /// <returns>Return List of Dynamic filter Tuples for SP</returns>
        List<Tuple<string, string>> GenerateDynamicFilterTupleForSP(FilterDataCollection filters);

        /// <summary>
        ///  To Generate dynamic where clause for SP 
        /// </summary>
        /// <param name="filters"></param>
        /// <returns>Return string dynamic where clause for SP  </returns>
        string GenerateWhereClauseForSP(FilterDataCollection filters);

        /// <summary>
        /// To generate where clause for publish status filter
        /// </summary>
        /// <param name="filters"></param>
        /// <returns>Return where clause for publish status filter</returns>
        string GenerateWhereClauseForPublishStatusFilter(FilterDataCollection filters);

        /// <summary>
        /// To replace the special characters
        /// </summary>
        /// <param name="filterValue"></param>
        void ReplaceSpecialCharacters(ref string filterValue);

        /// <summary>
        /// To generate reports where clause for SP
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>Return string where clause reports for SP</returns>
        string GenerateReportsWhereClauseForSP(FilterDataTuple filter);
        #endregion

        #region Helper Methods

        /// <summary>
        /// To set the Query Parameters
        /// </summary>
        /// <param name="filterKey"></param>
        /// <param name="filterOperator"></param>
        /// <param name="filterValue"></param>
        /// <param name="filterList"></param>
        /// <returns>Return string Query Parameter</returns>
        string SetQueryParameter(string filterKey, string filterOperator, string filterValue, List<object> filterList);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterKey"></param>
        /// <param name="filterOperator"></param>
        /// <param name="filterValue"></param>
        /// <returns>Return string query parameter for SP</returns>
        string SetQueryParameterForSP(string filterKey, string filterOperator, string filterValue);

        /// <summary>
        /// To Generate the Query Parameters for SP
        /// </summary>
        /// <param name="filterKey"></param>
        /// <param name="filterOperator"></param>
        /// <param name="filterValue"></param>
        /// <returns>Return string to generate the query</returns>
        string GenerateQuery(string filterKey, string filterOperator, string filterValue);

        /// <summary>
        /// To append the query
        /// </summary>
        /// <param name="filterKey"></param>
        /// <param name="filterOperator"></param>
        /// <param name="filterValue"></param>
        /// <returns>Return Append Query string</returns>
        string AppendQuery(string filterKey, string filterOperator, string filterValue);

        /// <summary>
        /// To generate not a Equals paramaters
        /// </summary>
        /// <param name="filterKey"></param>
        /// <param name="filterOperator"></param>
        /// <param name="filterValue"></param>
        /// <returns>Return string Not Equals query</returns>
        string GenerateNotaEquals(string filterKey, string filterOperator, string filterValue);

        /// <summary>
        /// To append the Like Query
        /// </summary>
        /// <param name="filterKey"></param>
        /// <param name="filterOperator"></param>
        /// <param name="filterValue"></param>
        /// <returns>Return Append like query string</returns>
        string AppendLikeQuery(string filterKey, string filterOperator, string filterValue);

        /// <summary>
        /// To generate the Like Query
        /// </summary>
        /// <param name="filterKey"></param>
        /// <param name="filterOperator"></param>
        /// <param name="filterValue"></param>
        /// <returns>Return string Like Query</returns>
        string GenerateLikeQuery(string filterKey, string filterOperator, string filterValue);

        /// <summary>
        /// To Generate start with Query
        /// </summary>
        /// <param name="filterKey"></param>
        /// <param name="filterOperator"></param>
        /// <param name="filterValue"></param>
        /// <returns>Return string Start With Query</returns>
        string GenerateStartwithQuery(string filterKey, string filterOperator, string filterValue);

        /// <summary>
        /// To generate end with query
        /// </summary>
        /// <param name="filterKey"></param>
        /// <param name="filterOperator"></param>
        /// <param name="filterValue"></param>
        /// <returns>Return string End With Query</returns>
        string GenerateEndwithQuery(string filterKey, string filterOperator, string filterValue);

        /// <summary>
        /// To generate the query
        /// </summary>
        /// <param name="filterKey"></param>
        /// <param name="filterOperator"></param>
        /// <param name="filterValue"></param>
        /// <param name="filterList"></param>
        /// <returns>Return string generate query</returns>
        string GenerateQuery(string filterKey, string filterOperator, string filterValue, List<object> filterList);

        /// <summary>
        /// To generate the Date Between Query
        /// </summary>
        /// <param name="filterKey"></param>
        /// <param name="filterOperator"></param>
        /// <param name="filterValue"></param>
        /// <param name="filterList"></param>
        /// <returns>Return string for the Date Between Query</returns>
        string GenerateDateBetweenQuery(string filterKey, string filterOperator, string filterValue, List<object> filterList);

        /// <summary>
        /// To generate the contains query
        /// </summary>
        /// <param name="filterKey"></param>
        /// <param name="filterOperator"></param>
        /// <param name="filterValue"></param>
        /// <param name="filterList"></param>
        /// <returns>Return string contains query</returns>
        string GenerateContainsQuery(string filterKey, string filterOperator, string filterValue, List<object> filterList);

        /// <summary>
        /// To generate Not contains query
        /// </summary>
        /// <param name="filterKey"></param>
        /// <param name="filterOperator"></param>
        /// <param name="filterValue"></param>
        /// <param name="filterList"></param>
        /// <returns>Return string Not Contains Query</returns>
        string GenerateNotContainsQuery(string filterKey, string filterOperator, string filterValue, List<object> filterList);

        /// <summary>
        /// To generate is query
        /// </summary>
        /// <param name="filterKey"></param>
        /// <param name="filterOperator"></param>
        /// <param name="filterValue"></param>
        /// <returns>Return String Is Query</returns>
        string GenerateIsQuery(string filterKey, string filterOperator, string filterValue);

        /// <summary>
        /// To generate In query
        /// </summary>
        /// <param name="filterKey"></param>
        /// <param name="filterOperator"></param>
        /// <param name="filterValue"></param>
        /// <returns>Return string In Query</returns>
        string GenerateInQuery(string filterKey, string filterOperator, string filterValue);

        /// <summary>
        /// To generate the SP IN query 
        /// </summary>
        /// <param name="filterKey"></param>
        /// <param name="filterOperator"></param>
        /// <param name="filterValue"></param>
        /// <returns>Return String IN Query for SP</returns>
        string GenerateSPInQuery(string filterKey, string filterOperator, string filterValue);

        /// <summary>
        /// To generate Not In Query
        /// </summary>
        /// <param name="filterKey"></param>
        /// <param name="filterOperator"></param>
        /// <param name="filterValue"></param>
        /// <returns>return string Not In Query</returns>
        string GenerateNotInQuery(string filterKey, string filterOperator, string filterValue);

        /// <summary>
        /// To generate OR Query
        /// </summary>
        /// <param name="filterKey"></param>
        /// <param name="filterOperator"></param>
        /// <param name="filterValue"></param>
        /// <param name="filterList"></param>
        /// <returns>Return string OR Query</returns>
        string GenerateOrQuery(string filterKey, string filterOperator, string filterValue, List<object> filterList);

        /// <summary>
        /// To append OR clause to query
        /// </summary>
        /// <param name="filterKey"></param>
        /// <param name="filterOperator"></param>
        /// <param name="filterValue"></param>
        /// <returns>Return string append OR clause to query</returns>
        string AppendOrClauseToQuery(string filterKey, string filterOperator, string filterValue);

        /// <summary>
        /// To append special OR Clause to Query
        /// </summary>
        /// <param name="filterKey"></param>
        /// <param name="filterOperator"></param>
        /// <param name="filterValue"></param>
        /// <returns>Return string append special OR Clause to Query</returns>
        string AppendSpecialOrClauseToQuery(string filterKey, string filterOperator, string filterValue);

        /// <summary>
        /// To append null or clause to query
        /// </summary>
        /// <param name="filterKey"></param>
        /// <param name="filterOperator"></param>
        /// <param name="filterValue"></param>
        /// <param name="filterList"></param>
        /// <returns>Return string append null OR clause to query</returns>
        string AppendNullOrClauseToQuery(string filterKey, string filterOperator, string filterValue, List<object> filterList);

        /// <summary>
        /// To append or clause to query start with 
        /// </summary>
        /// <param name="filterKey"></param>
        /// <param name="filterOperator"></param>
        /// <param name="filterValue"></param>
        /// <returns>Return string append or clause to query start with </returns>
        string AppendOrClauseToQueryStartWith(string filterKey, string filterOperator, string filterValue);

        /// <summary>
        /// To generate Date Time query
        /// </summary>
        /// <param name="filterValue"></param>
        /// <param name="filterValueInDateTime"></param>
        /// <returns>Return Bool DateTime query/returns>
        bool IsDateTime(string filterValue, out DateTime filterValueInDateTime);

        #endregion

    }
}
