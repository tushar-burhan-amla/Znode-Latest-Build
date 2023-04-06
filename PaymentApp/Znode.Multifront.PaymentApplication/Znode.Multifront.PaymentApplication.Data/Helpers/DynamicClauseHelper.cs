using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Znode.Multifront.PaymentApplication.Helpers;

namespace Znode.Multifront.PaymentApplication.Data
{
    public static class DynamicClauseHelper
    {
        static int placeLocator;
        static DateTime filterValueinDateTime;

        #region Public Methods

        #region Clauses for Entity Framework

        //To generate dynamic where clause for Entity Framework with filterValue.
        public static EntityWhereClauseModel GenerateDynamicWhereClauseWithFilter(FilterCollection filters)
        {
            placeLocator = 0;
            string whereClause = string.Empty;
            object[] filterClause = new object[] { };
            List<object> filterList = new List<object> { };

            if (filters?.Count > 0)
            {
                foreach (var tuple in filters)
                {
                    var filterKey = tuple.Item1;
                    var filterOperator = tuple.Item2;
                    var filterValue = tuple.Item3;
                    whereClause = (string.IsNullOrEmpty(whereClause))
                        ? SetQueryParameter(filterKey, filterOperator, filterValue, filterList)
                        : $"{whereClause} {EntityKeys.AND} {SetQueryParameter(filterKey, filterOperator, filterValue, filterList)}";
                    filterClause = filterList?.ToArray();
                }
            }
            return new EntityWhereClauseModel() { WhereClause = whereClause, FilterValues = filterClause };
        }

        //To generate dynamic where clause for Entity Framework with filterValue as output parameter
        public static string GenerateDynamicWhereClause(List<Tuple<string, string, string>> filters, out object[] filterClause)
        {
            placeLocator = 0;
            string whereClause = string.Empty;
            filterClause = new object[] { };
            List<object> filterList = new List<object> { };

            if (filters?.Count > 0)
            {
                foreach (var tuple in filters)
                {
                    var filterKey = tuple.Item1;
                    var filterOperator = tuple.Item2;
                    var filterValue = tuple.Item3;
                    whereClause = (string.IsNullOrEmpty(whereClause))
                        ? SetQueryParameter(filterKey, filterOperator, filterValue, filterList)
                        : $"{whereClause} {EntityKeys.AND} {SetQueryParameter(filterKey, filterOperator, filterValue, filterList)}";
                    filterClause = filterList?.ToArray();
                }
            }
            return whereClause;
        }

        //To generate dynamic where clause for Entity Framework with filterValue as output parameter
        public static string GenerateDynamicWhereClause(Tuple<string, string, string> filter)
        {
            placeLocator = 0;
            string whereClause = string.Empty;
            List<object> filterList = new List<object> { };
            whereClause = (string.IsNullOrEmpty(whereClause))
                ? SetQueryParameter(filter.Item1, filter.Item2, filter.Item3, filterList)
                : $"{whereClause} {EntityKeys.AND} {SetQueryParameter(filter.Item1, filter.Item2, filter.Item3, filterList)}";

            return whereClause;
        }

        //To generate dynamic order by clause for Entity Framework.
        public static string GenerateDynamicOrderByClause(NameValueCollection sorts)
        {
            string orderBy = string.Empty;
            if (sorts?.Count > 0 && sorts.HasKeys())
            {
                foreach (string key in sorts.AllKeys)
                {
                    if (!string.IsNullOrWhiteSpace(key))
                    {
                        orderBy = $"{key} {sorts.Get(key)}";
                    }
                }
            }
            return orderBy;
        }

        #endregion

        #region Clauses for SP
        //To generate query based on Input filter parameters
        public static string GenerateDynamicWhereClause(List<Tuple<string, string, string>> filters)
        {
            string whereClause = string.Empty;
            if (filters?.Count > 0)
            {
                foreach (var tuple in filters)
                {
                    var filterKey = tuple.Item1;
                    var filterOperator = tuple.Item2;
                    var filterValue = tuple.Item3;
                    whereClause = (string.IsNullOrEmpty(whereClause))
                        ? SetQueryParameter(filterKey, filterOperator, filterValue, null)
                        : $"{whereClause} {EntityKeys.AND} {SetQueryParameter(filterKey, filterOperator, filterValue, null)}";
                }
            }
            return whereClause;
        }

        /// <summary>
        /// To Generate dynamic where clause for stored procedure.
        /// </summary>
        /// <param name="filters">List<Tuple<string, string, string>> filters</param>
        /// <returns>Return the where clause string.</returns>
        public static string GenerateDynamicWhereClauseForSP(List<Tuple<string, string, string>> filters)
        {
            string whereClause = string.Empty;
            if (filters?.Count > 0)
            {
                foreach (var tuple in filters)
                {
                    var filterKey = tuple.Item1;
                    var filterOperator = tuple.Item2;
                    var filterValue = tuple.Item3;
                    if (!Equals(filterKey, "_") && !Equals(filterKey.ToLower(), "usertype"))
                        whereClause = (string.IsNullOrEmpty(whereClause))
                            ? SetQueryParameterForSP(filterKey, filterOperator, filterValue)
                            : $"{whereClause} {EntityKeys.AND} {SetQueryParameterForSP(filterKey, filterOperator, filterValue)}";
                }
            }
            return whereClause;
        }
        #endregion

        #endregion

        #region Helper Methods

        //Returns query based on Input filter parameters
        private static string SetQueryParameter(string filterKey, string filterOperator, string filterValue, List<object> filterList)
        {
            string query = string.Empty;
            if (Equals(filterOperator, EntityFilterOperators.Equals)) return GenerateQuery(filterKey, Operators.Equals, filterValue, filterList);
            if (Equals(filterOperator, EntityFilterOperators.Like)) return GenerateContainsQuery(filterKey, Operators.Like, filterValue, filterList);
            if (Equals(filterOperator, EntityFilterOperators.GreaterThan)) return GenerateQuery(filterKey, Operators.GreaterThan, filterValue, filterList);
            if (Equals(filterOperator, EntityFilterOperators.GreaterThanOrEqual)) return GenerateQuery(filterKey, Operators.GreaterThanOrEqual, filterValue, filterList);
            if (Equals(filterOperator, EntityFilterOperators.LessThan)) return GenerateQuery(filterKey, Operators.LessThan, filterValue, filterList);
            if (Equals(filterOperator, EntityFilterOperators.LessThanOrEqual)) return GenerateQuery(filterKey, Operators.LessThanOrEqual, filterValue, filterList);
            if (Equals(filterOperator, EntityFilterOperators.Contains)) return GenerateContainsQuery(filterKey, Operators.Contains, filterValue, filterList);
            if (Equals(filterOperator, EntityFilterOperators.StartsWith)) return GenerateContainsQuery(filterKey, Operators.StartsWith, filterValue, filterList);
            if (Equals(filterOperator, EntityFilterOperators.EndsWith)) return GenerateContainsQuery(filterKey, Operators.EndsWith, filterValue, filterList);
            if (Equals(filterOperator, EntityFilterOperators.Is)) return GenerateIsQuery(filterKey, Operators.Is, filterValue);
            if (Equals(filterOperator, EntityFilterOperators.In)) return GenerateInQuery(filterKey, Operators.In, filterValue);
            if (Equals(filterOperator, EntityFilterOperators.Or)) return GenerateOrQuery(filterKey, Operators.Or, filterValue, filterList);

            return query;
        }

        //Returns query based on Input filter parameters for SP.
        private static string SetQueryParameterForSP(string filterKey, string filterOperator, string filterValue)
        {
            string query = string.Empty;
            if (Equals(filterOperator, EntityFilterOperators.Equals)) return AppendQuery(filterKey, " = ", filterValue);
            if (Equals(filterOperator, EntityFilterOperators.Like)) return AppendLikeQuery(filterKey, " like ", filterValue);
            if (Equals(filterOperator, EntityFilterOperators.GreaterThan)) return AppendQuery(filterKey, " > ", filterValue);
            if (Equals(filterOperator, EntityFilterOperators.GreaterThanOrEqual)) return AppendQuery(filterKey, " >= ", filterValue);
            if (Equals(filterOperator, EntityFilterOperators.LessThan)) return AppendQuery(filterKey, " < ", filterValue);
            if (Equals(filterOperator, EntityFilterOperators.LessThanOrEqual)) return AppendQuery(filterKey, " <= ", filterValue);
            if (Equals(filterOperator, EntityFilterOperators.Contains)) return AppendLikeQuery(filterKey, " like ", filterValue);
            if (Equals(filterOperator, EntityFilterOperators.StartsWith)) return GenerateStartwithQuery(filterKey, " like ", filterValue);
            if (Equals(filterOperator, EntityFilterOperators.EndsWith)) return GenerateEndwithQuery(filterKey, " like ", filterValue);
            if (Equals(filterOperator, EntityFilterOperators.Is)) return AppendQuery(filterKey, " is ", filterValue);

            return query;
        }

        // Method Return the genrated query.
        private static string AppendQuery(string filterKey, string filterOperator, string filterValue)
        {
            //This Special Case required to Get the Franchisable Product in which Product belongs to Portals & all product having franchisable flat true will comes.
            if (filterKey.Contains("$")) return AppendSpecialOrClauseToQuery(filterKey, filterOperator, filterValue);
            //To generate OR condition with null value for the filter key passed.
            if (filterKey.Contains("|")) return AppendNullOrClauseToQuery(filterKey, filterOperator, filterValue);
            return $"{EntityKeys.TildeOperator}{filterKey}{ EntityKeys.TildeOperator}{filterOperator}{filterValue}";
        }

        //Method return the generated query for special case. for example "like"
        private static string AppendLikeQuery(string filterKey, string filterOperator, string filterValue)
        {
            if (filterKey.Contains("|")) return AppendOrClauseToQuery(filterKey, filterOperator, filterValue);
            return GenerateLikeQuery(filterKey, filterOperator, filterValue);
        }

        //Method return the generated query for special case. for example "like"
        private static string GenerateLikeQuery(string filterKey, string filterOperator, string filterValue)
            => $"{EntityKeys.TildeOperator}{filterKey}{EntityKeys.TildeOperator}{filterOperator}'%{filterValue}%'";

        //Method return the generated query for special case. for example "startswith"
        private static string GenerateStartwithQuery(string filterKey, string filterOperator, string filterValue)
            => $"{EntityKeys.TildeOperator}{filterKey}{EntityKeys.TildeOperator}{filterOperator}'{filterValue}%'";

        //Method return the generated query for special case. for example "endswith"
        private static string GenerateEndwithQuery(string filterKey, string filterOperator, string filterValue)
            => $"{EntityKeys.TildeOperator}{filterKey}{EntityKeys.TildeOperator}{filterOperator}'%{filterValue}'";

        //Returns query generated using filterKey, filterOperator, filterValue
        private static string GenerateQuery(string filterKey, string filterOperator, string filterValue, List<object> filterList)
        {
            //This Special Case required to Get the Franchisable Product in which Product belongs to Portals & all product having franchisable flat true will comes.
            //if (filterKey.Contains("$")) return AppendSpecialOrClauseToQuery(filterKey, filterOperator, filterValue);
            //To generate OR condition with null value for the filter key passed.
            //if (filterKey.Contains("|")) return AppendNullOrClauseToQuery(filterKey, filterOperator, filterValue);

            if (IsDateTime(filterValue, out filterValueinDateTime))
            {
                string query = $"{filterKey}{filterOperator}(@{placeLocator})";
                if (!Equals(filterList, null))
                {
                    filterList.Add(filterValueinDateTime);
                    placeLocator++;
                }
                return query;
            }
            else
                return $"{filterKey}{filterOperator}{filterValue}";
        }

        //Returns query generated for 'like/contains/starts with/ends with' filter operator
        private static string GenerateContainsQuery(string filterKey, string filterOperator, string filterValue, List<object> filterList)
        {
            string query = $"{filterKey}.{filterOperator}(@{placeLocator})";
            if (!Equals(filterList, null))
            {
                filterList.Add(filterValue);
                placeLocator++;
            }
            return query;
        }

        //Returns query generated using filterKey, filterOperator, filterValue for special case. for example "isNull"
        private static string GenerateIsQuery(string filterKey, string filterOperator, string filterValue)
            => $"{filterKey} == {filterValue}";

        //Returns query generated using filterKey, filterOperator, filterValue for special case. for example "in"
        private static string GenerateInQuery(string filterKey, string filterOperator, string filterValue)
        {
            string[] filterArray = filterValue.Split(',');
            string filterClause = string.Empty;
            for (int index = 0; index < filterArray.Length; index++)
            {
                string filter = $" or {filterKey} = {filterArray[index]}";
                filterClause += filter;
            }
            filterClause = filterClause.Remove(0, 4);

            return filterClause;
        }

        //Returns query generated using filterKey, filterOperator, filterValue for in condition in string. for example "or"
        private static string GenerateOrQuery(string filterKey, string filterOperator, string filterValue, List<object> filterList)
        {
            string[] filterArray = filterValue.Split(',');
            string filterClause = string.Empty;
            for (int index = 0; index < filterArray.Length; index++)
            {
                string filter = $" or {filterKey} = @{index}";
                filterClause += filter;
            }
            filterClause = filterClause.Remove(0, 4);

            foreach (var filter in filterValue.Split(','))
                filterList.Add(filter);

            return filterClause;
        }

        //To do
        //Appends 'Or' condition to query
        private static string AppendOrClauseToQuery(string filterKey, string filterOperator, string filterValue)
        {
            string strQuery = string.Empty;
            //if (filterKey.Contains("|"))
            //{
            //    string innerQuery = string.Empty;
            //    string[] strSplit = filterKey.Split('|');
            //    for (int i = 0; i < strSplit.Length; i++)
            //    {
            //        if (!string.IsNullOrEmpty(strSplit[i]))
            //        {
            //            innerQuery = (string.IsNullOrEmpty(innerQuery)) ? GenerateLikeQuery(strSplit[i], filterOperator, filterValue) : innerQuery + " OR " + GenerateLikeQuery(strSplit[i], filterOperator, filterValue);
            //        }
            //    }
            //    strQuery = (string.IsNullOrEmpty(innerQuery)) ? string.Empty : string.Format("({0})", innerQuery);
            //}
            return strQuery;
        }

        //To do
        //Appends 'Or' condition with null value for the filter key passed with "$" separator.
        private static string AppendSpecialOrClauseToQuery(string filterKey, string filterOperator, string filterValue)
        {
            string[] strSplit = filterKey.Split('$');
            return $"({EntityKeys.TildeOperator}{strSplit[0]}{ EntityKeys.TildeOperator}={filterValue} OR ({EntityKeys.TildeOperator}{strSplit[0]}{EntityKeys.TildeOperator} is null and { EntityKeys.TildeOperator}{strSplit[1]}{EntityKeys.TildeOperator} = 1 ))";
        }

        //To do
        //Appends 'Or' condition with null value for the filter key passed with "|" separator.
        private static string AppendNullOrClauseToQuery(string filterKey, string filterOperator, string filterValue)
        {
            string[] strSplit = filterKey.Split('|');
            return $"({EntityKeys.TildeOperator}{strSplit[0]}{EntityKeys.TildeOperator}={filterValue} OR {EntityKeys.TildeOperator}{ strSplit[0]}{EntityKeys.TildeOperator} Is NULL)";
        }

        // Check Whether String value is a DateTime or not.
        private static bool IsDateTime(string filterValue, out DateTime filterValueInDateTime) => DateTime.TryParse(filterValue, out filterValueInDateTime);

        #endregion
    }
}
