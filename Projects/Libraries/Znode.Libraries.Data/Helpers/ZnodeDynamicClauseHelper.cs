using System;
using System.Collections.Generic;
using System.Collections.Specialized;

using Znode.Libraries.Data.Interfaces;

namespace Znode.Libraries.Data.Helpers
{
    public class ZnodeDynamicClauseHelper : IZnodeDynamicClauseHelper
    {
        private int placeLocator;
        private DateTime filterValueinDateTime;
        private const string CommaReplacer = "^";

        public virtual string AppendLikeQuery(string filterKey, string filterOperator, string filterValue)
        {
            if (filterKey.Contains("|")) return AppendOrClauseToQuery(filterKey, filterOperator, filterValue);
            return GenerateLikeQuery(filterKey, filterOperator, filterValue);
        }

        public virtual string AppendNullOrClauseToQuery(string filterKey, string filterOperator, string filterValue, List<object> filterList)
        {
            string[] strSplit = filterKey.Split('!');
            if (IsDateTime(filterValue, out filterValueinDateTime))
            {
                string query = $"{strSplit[0]}{filterOperator}(@{placeLocator})";
                if (!Equals(filterList, null))
                {
                    filterList.Add(filterValueinDateTime);
                    placeLocator++;
                }
                return $"({query} || {strSplit[0]} = null)";
            }
            else
                return $"({StoredProcedureKeys.TildOperator}{strSplit[0]}{StoredProcedureKeys.TildOperator}{filterOperator}{filterValue} OR {StoredProcedureKeys.TildOperator}{ strSplit[0]}{StoredProcedureKeys.TildOperator} Is NULL)";
        }

        public virtual string AppendOrClauseToQuery(string filterKey, string filterOperator, string filterValue)
        {
            string strQuery = string.Empty;
            if (filterKey.Contains("|"))
            {
                string innerQuery = string.Empty;
                foreach (string item in filterKey.Split('|'))
                {
                    if (!string.IsNullOrEmpty(item))
                        innerQuery = (string.IsNullOrEmpty(innerQuery)) ? GenerateLikeQuery(item, filterOperator, filterValue) : innerQuery + " OR " + GenerateLikeQuery(item, filterOperator, filterValue);
                }
                strQuery = (string.IsNullOrEmpty(innerQuery)) ? string.Empty : string.Format("({0})", innerQuery);
            }
            return strQuery;
        }

        public virtual string AppendOrClauseToQueryStartWith(string filterKey, string filterOperator, string filterValue)
        {
            string strQuery = string.Empty;
            if (filterKey.Contains("|"))
            {
                string innerQuery = string.Empty;
                foreach (string item in filterKey.Split('|'))
                {
                    if (!string.IsNullOrEmpty(item))
                        innerQuery = (string.IsNullOrEmpty(innerQuery)) ? GenerateStartwithQuery(item, filterOperator, filterValue) : innerQuery + " OR " + GenerateStartwithQuery(item, filterOperator, filterValue);
                }
                strQuery = (string.IsNullOrEmpty(innerQuery)) ? string.Empty : string.Format("({0})", innerQuery);
            }
            return strQuery;
        }

        public virtual string AppendQuery(string filterKey, string filterOperator, string filterValue)
        {
            //This Special Case required to Get the Franchisable Product in which Product belongs to Portals & all product having franchisable flat true will comes.
            if (filterKey.Contains("$")) return AppendSpecialOrClauseToQuery(filterKey, filterOperator, filterValue);
            //To generate OR condition with null value for the filter key passed.
            if (filterKey.Contains("!")) return AppendNullOrClauseToQuery(filterKey, filterOperator, filterValue, null);
            return $"{StoredProcedureKeys.TildOperator}{filterKey}{ StoredProcedureKeys.TildOperator}{filterOperator}{filterValue}";
        }

        public virtual string AppendSpecialOrClauseToQuery(string filterKey, string filterOperator, string filterValue)
        {
            string[] strSplit = filterKey.Split('$');
            return $"({StoredProcedureKeys.TildOperator}{strSplit[0]}{ StoredProcedureKeys.TildOperator}={filterValue} OR ({StoredProcedureKeys.TildOperator}{strSplit[0]}{StoredProcedureKeys.TildOperator} is null and { StoredProcedureKeys.TildOperator}{strSplit[1]}{StoredProcedureKeys.TildOperator} = 1 ))";
        }

        public virtual string GenerateContainsQuery(string filterKey, string filterOperator, string filterValue, List<object> filterList)
        {
            string query = $"{filterKey}.{filterOperator}(@{placeLocator})";
            if (!Equals(filterList, null))
            {
                filterList.Add(filterValue);
                placeLocator++;
            }
            return query;
        }

        public virtual string GenerateDateBetweenQuery(string filterKey, string filterOperator, string filterValue, List<object> filterList)
        {
            //This Special Case when date filter with BETWEEN used
            if (filterValue.Contains("'"))
                filterValue = filterValue.Replace("'", string.Empty).ToString();

            string query = string.Empty;
            int index = 0;
            if (filterValue.Contains("and"))
            {
                string[] dateFilterArray = filterValue.Split(new string[] { "and" }, StringSplitOptions.None);
                foreach (var dateItem in dateFilterArray)
                {
                    if (IsDateTime(dateItem, out filterValueinDateTime))
                    {
                        query = index == 0 ? $"{filterKey} >= (@{placeLocator})" : $"{query} and {filterKey} <= (@{ placeLocator})";
                        if (!Equals(filterList, null))
                        {
                            filterList.Add(filterValueinDateTime);
                            placeLocator++;
                            index++;
                        }
                    }
                }
            }
            return query;
        }

        public virtual List<Tuple<string, string>> GenerateDynamicFilterTupleForSP(FilterDataCollection filters)
        {
            List<Tuple<string, string>> filterTuples = new List<Tuple<string, string>>();

            if (filters?.Count > 0)
            {
                foreach (var tuple in filters)
                {
                    string whereClause = string.Empty;
                    var filterKey = tuple.Item1;
                    var filterOperator = tuple.Item2;
                    var filterValue = tuple.Item3;
                    if (!Equals(filterKey, "_") && !Equals(filterKey.ToLower(), "usertype"))
                    {
                        whereClause = SetQueryParameterForSP(filterKey, filterOperator, filterValue);
                        filterTuples.Add(new Tuple<string, string>(filterKey, whereClause));
                    }
                }
            }
            return filterTuples;
        }

        public virtual string GenerateDynamicOrderByClause(NameValueCollection sorts)
        {
            string orderBy = string.Empty;
            if (sorts?.Count > 0 && sorts.HasKeys())
            {
                foreach (string key in sorts.AllKeys)
                {
                    if (!string.IsNullOrWhiteSpace(key))
                    {
                        orderBy = orderBy + (!string.IsNullOrWhiteSpace(orderBy) ? "," : "") + $"{key} {sorts.Get(key)}";
                    }
                }
            }
            return orderBy;
        }

        public virtual string GenerateDynamicWhereClause(FilterDataCollection filters)
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
                        : $"{whereClause} {StoredProcedureKeys.AND} {SetQueryParameter(filterKey, filterOperator, filterValue, null)}";
                }
            }
            return whereClause;
        }

        public virtual string GenerateDynamicWhereClauseForSP(FilterDataCollection filters)
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
                            : $"{whereClause} {StoredProcedureKeys.AND} {SetQueryParameterForSP(filterKey, filterOperator, filterValue)}";
                }
            }
            return whereClause;
        }

        public virtual EntityWhereClauseModel GenerateDynamicWhereClauseWithFilter(FilterDataCollection filters)
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

                    if (filterValue.IndexOf("''") >= 0)
                        filterValue = filterValue.Replace("''", "'");

                    if (filterValue.IndexOf(CommaReplacer) >= 0)
                        filterValue = filterValue.Replace(CommaReplacer, ",");

                    //If filterkey contains | then seprate the filter key by | and generate or query.
                    if (filterKey.Contains("|"))
                    {
                        string whereClauseWithORClause = string.Empty;

                        foreach (string item in filterKey.Split('|'))
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                whereClauseWithORClause = (string.IsNullOrEmpty(whereClauseWithORClause))
                                    ? SetQueryParameter(item, filterOperator, filterValue, filterList)
                                    : $"{whereClauseWithORClause} {StoredProcedureKeys.OR} {SetQueryParameter(item, filterOperator, filterValue, filterList)}";
                                filterClause = filterList?.ToArray();
                            }
                        }

                        whereClause = string.IsNullOrEmpty(whereClause) ? $" ({whereClauseWithORClause})" : $" {whereClause} {StoredProcedureKeys.AND} ({whereClauseWithORClause})";
                    }
                    else
                    {
                        whereClause = (string.IsNullOrEmpty(whereClause))
                            ? SetQueryParameter(filterKey, filterOperator, filterValue, filterList)
                            : $"{whereClause} {StoredProcedureKeys.AND} {SetQueryParameter(filterKey, filterOperator, filterValue, filterList)}";
                        filterClause = filterList?.ToArray();
                    }
                }
            }
            return new EntityWhereClauseModel() { WhereClause = whereClause, FilterValues = filterClause };
        }

        public virtual string GenerateEndwithQuery(string filterKey, string filterOperator, string filterValue)
            => $"{StoredProcedureKeys.TildOperator}{filterKey}{StoredProcedureKeys.TildOperator}{filterOperator}'%{filterValue}'";


        public virtual string GenerateInQuery(string filterKey, string filterOperator, string filterValue)
        {
            string[] filterArray = filterValue.Split(',');
            string filterClause = string.Empty;
            for (int index = 0; index < filterArray.Length; index++)
            {
                string filter = $" or {filterKey} = {filterArray[index]}";
                filterClause += filter;
            }
            filterClause = filterClause.Remove(0, 4);

            if (!string.IsNullOrEmpty(filterClause))
                filterClause = string.Format("({0})", filterClause);

            return filterClause;
        }

        public virtual string GenerateIsQuery(string filterKey, string filterOperator, string filterValue)
        => $"{filterKey} == {filterValue}";

        public virtual string GenerateLikeQuery(string filterKey, string filterOperator, string filterValue)
        => $"{StoredProcedureKeys.TildOperator}{filterKey}{StoredProcedureKeys.TildOperator}{filterOperator}'%{filterValue}%'";


        public virtual string GenerateNotaEquals(string filterKey, string filterOperator, string filterValue)
        {
            //This Special Case required to Get the Franchisable Product in which Product belongs to Portals & all product having franchisable flat true will comes.
            if (filterKey.Contains("$")) return AppendSpecialOrClauseToQuery(filterKey, filterOperator, filterValue);
            //To generate OR condition with null value for the filter key passed.
            if (filterKey.Contains("!")) return AppendNullOrClauseToQuery(filterKey, filterOperator, filterValue, null);
            return string.Format("{0}{1}{2}{3}{4}'{5}'", StoredProcedureKeys.TildOperator, filterKey, " ", filterOperator, " ", filterValue);
        }

        public virtual string GenerateNotContainsQuery(string filterKey, string filterOperator, string filterValue, List<object> filterList)
        {
            string query = $"!{filterKey}.{filterOperator}(@{placeLocator})";
            if (!Equals(filterList, null))
            {
                filterList.Add(filterValue);
                placeLocator++;
            }
            return query;
        }

        public virtual string GenerateNotInQuery(string filterKey, string filterOperator, string filterValue)
        {
            string[] filterArray = filterValue.Split(',');
            string filterClause = string.Empty;
            for (int index = 0; index < filterArray.Length; index++)
            {
                string filter = $" and {filterKey} != {filterArray[index]}";
                filterClause += filter;
            }
            filterClause = filterClause.Remove(0, 4);

            return filterClause;
        }

        public virtual string GenerateOrQuery(string filterKey, string filterOperator, string filterValue, List<object> filterList)
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

        public virtual string GenerateQuery(string filterKey, string filterOperator, string filterValue)
        => string.Format("{0}{1}{2}{3}'{4}'", StoredProcedureKeys.TildOperator, filterKey, StoredProcedureKeys.TildOperator, filterOperator, filterValue);


        public virtual string GenerateQuery(string filterKey, string filterOperator, string filterValue, List<object> filterList)
        {
            //To generate OR condition with null value for the filter key passed.
            if (filterKey.Contains("!")) return AppendNullOrClauseToQuery(filterKey, filterOperator, filterValue, filterList);

            if (filterValue.Contains("'"))
                filterValue = filterValue.Replace("'", string.Empty).ToString();

            //Check if filter value is date time then generate date time query else generate query using filterKey, filterOperator, filterValue.
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

        public virtual string GenerateReportsWhereClauseForSP(FilterDataTuple filter)
        {
            string whereClause = string.Empty;
            var filterKey = filter.Item1;
            var filterOperator = filter.Item2;
            var filterValue = filter.Item3;
            whereClause = SetQueryParameterForSP(filterKey, filterOperator, filterValue);
            return whereClause;
        }

        public virtual string GenerateSPInQuery(string filterKey, string filterOperator, string filterValue)
        {
            string[] filterArray = filterValue.Split(',');
            string filterValues = string.Empty;

            string filterClause = $"{filterKey} in";

            for (int index = 0; index < filterArray.Length; index++)
                filterValues += string.IsNullOrEmpty(filterValues) ? $"'{filterArray[index]}'" : $",'{filterArray[index]}'";

            if (!string.IsNullOrEmpty(filterValues))
                filterClause = string.Format("({0}({1}))", filterClause, filterValues);
            else
                filterClause = string.Empty;

            return filterClause;
        }

        public virtual string GenerateStartwithQuery(string filterKey, string filterOperator, string filterValue)
        {
            if (filterKey.Contains("|")) return AppendOrClauseToQueryStartWith(filterKey, filterOperator, filterValue);
            return $"{StoredProcedureKeys.TildOperator}{filterKey}{StoredProcedureKeys.TildOperator}{filterOperator}'{filterValue}%'";
        }

        public virtual string GenerateWhereClauseForPublishStatusFilter(FilterDataCollection filters)
        {
            string publishStatus = string.Empty;
            if (filters.Count > 0)
            {
                foreach (var filter in filters)
                {
                    var filterOperator = filter.Item2;
                    var filterValue = filter.Item3;
                    publishStatus = SetQueryParameterForSP(string.Empty, filterOperator, filterValue);
                }
                return publishStatus;
            }
            return publishStatus;
        }

        public virtual string GenerateWhereClauseForSP(FilterDataTuple filter)
        {
            string whereClause = string.Empty;
            var filterKey = filter.Item1;
            var filterOperator = filter.Item2;
            var filterValue = filter.Item3;

            if (filter.FilterName.Contains("|"))
            {
                string commaSeperatedAttributeCode = filterKey.Replace("|", "','");
                whereClause = $"attributecode in ('{commaSeperatedAttributeCode}') and attributevalue {SetQueryParameterForSP(string.Empty, filterOperator, filterValue)}";

            }
            else if (!Equals(filterKey, "_") && !Equals(filterKey.ToLower(), "usertype"))
                whereClause = $"attributecode = '{filterKey }' and attributevalue {SetQueryParameterForSP(string.Empty, filterOperator, filterValue)}";

            return whereClause;
        }

        public virtual string GenerateWhereClauseForSP(FilterDataCollection filters)
        {
            if (filters.Count > 0)
            {
                string whereClause = "<ArrayOfWhereClauseModel>";
                foreach (var filter in filters)
                {
                    whereClause += "<WhereClauseModel>";
                    var filterKey = filter.Item1;
                    var filterOperator = filter.Item2;
                    var filterValue = filter.Item3;

                    ReplaceSpecialCharacters(ref filterValue);
                    if (filter.FilterName.Contains("|"))
                    {
                        string commaSeperatedAttributeCode = filterKey.Replace("|", "','");
                        whereClause += $"<attributecode> in ('{commaSeperatedAttributeCode}') </attributecode>";
                        whereClause += $"<attributevalue> {SetQueryParameterForSP(string.Empty, filterOperator, filterValue)} </attributevalue>";

                    }
                    else if (!Equals(filterKey, "_") && !Equals(filterKey.ToLower(), "usertype"))
                    {
                        whereClause += $"<attributecode> = '{filterKey }' </attributecode>";
                        whereClause += $"<attributevalue> {SetQueryParameterForSP(string.Empty, filterOperator, filterValue)}</attributevalue>";

                    }

                    whereClause += "</WhereClauseModel>";
                }
                whereClause += "</ArrayOfWhereClauseModel>";

                return whereClause;
            }
            return string.Empty;
        }

        public virtual bool IsDateTime(string filterValue, out DateTime filterValueInDateTime)
        => DateTime.TryParse(filterValue, out filterValueInDateTime);

        public virtual void ReplaceSpecialCharacters(ref string filterValue)
        {
            if (filterValue.Contains("&"))
            {
                filterValue = filterValue.Replace("&", "&amp;");
            }
            else if (filterValue.Contains("<"))
            {
                filterValue = filterValue.Replace("<", "&lt;");
            }
            else if (filterValue.Contains(">"))
            {
                filterValue = filterValue.Replace(">", "&gt;");
            }
            else if (filterValue.Contains("\""))
            {
                filterValue = filterValue.Replace("\"", "&quot;");
            }
            else if (filterValue.Contains("\'"))
            {
                filterValue = filterValue.Replace("\'", "&apos;");
            }
        }

        public virtual string SetQueryParameter(string filterKey, string filterOperator, string filterValue, List<object> filterList)
        {
            string query = string.Empty;
            if (Equals(filterOperator, ProcedureFilterOperators.Equals)) return GenerateQuery(filterKey, " = ", filterValue, filterList);
            if (Equals(filterOperator, ProcedureFilterOperators.Like)) return GenerateContainsQuery(filterKey, "Contains", filterValue, filterList);
            if (Equals(filterOperator, ProcedureFilterOperators.GreaterThan)) return GenerateQuery(filterKey, " > ", filterValue, filterList);
            if (Equals(filterOperator, ProcedureFilterOperators.GreaterThanOrEqual)) return GenerateQuery(filterKey, " >= ", filterValue, filterList);
            if (Equals(filterOperator, ProcedureFilterOperators.LessThan)) return GenerateQuery(filterKey, " < ", filterValue, filterList);
            if (Equals(filterOperator, ProcedureFilterOperators.LessThanOrEqual)) return GenerateQuery(filterKey, " <= ", filterValue, filterList);
            if (Equals(filterOperator, ProcedureFilterOperators.Contains)) return GenerateContainsQuery(filterKey, "Contains", filterValue, filterList);
            if (Equals(filterOperator, ProcedureFilterOperators.StartsWith)) return GenerateContainsQuery(filterKey, "StartsWith", filterValue, filterList);
            if (Equals(filterOperator, ProcedureFilterOperators.EndsWith)) return GenerateContainsQuery(filterKey, "EndsWith", filterValue, filterList);
            if (Equals(filterOperator, ProcedureFilterOperators.Is)) return GenerateContainsQuery(filterKey, "Equals", filterValue, filterList);
            if (Equals(filterOperator, ProcedureFilterOperators.In)) return GenerateInQuery(filterKey, "in", filterValue);
            if (Equals(filterOperator, ProcedureFilterOperators.NotIn)) return GenerateNotInQuery(filterKey, "not in", filterValue);
            if (Equals(filterOperator, ProcedureFilterOperators.NotEquals)) return GenerateQuery(filterKey, " != ", filterValue, filterList);
            if (Equals(filterOperator, ProcedureFilterOperators.Or)) return GenerateOrQuery(filterKey, "or", filterValue, filterList);  //only for string in
            if (Equals(filterOperator, ProcedureFilterOperators.NotContains)) return GenerateNotContainsQuery(filterKey, "Contains", filterValue, filterList);
            if (Equals(filterOperator, ProcedureFilterOperators.Between)) return GenerateDateBetweenQuery(filterKey, " Between ", filterValue, filterList);
            return query;
        }

        public virtual string SetQueryParameterForSP(string filterKey, string filterOperator, string filterValue)
        {
            string query = string.Empty;
            if (Equals(filterOperator, ProcedureFilterOperators.Equals)) return AppendQuery(filterKey, " = ", filterValue);
            if (Equals(filterOperator, ProcedureFilterOperators.Like)) return AppendLikeQuery(filterKey, " like ", filterValue);
            if (Equals(filterOperator, ProcedureFilterOperators.GreaterThan)) return AppendQuery(filterKey, " > ", filterValue);
            if (Equals(filterOperator, ProcedureFilterOperators.GreaterThanOrEqual)) return AppendQuery(filterKey, " >= ", filterValue);
            if (Equals(filterOperator, ProcedureFilterOperators.LessThan)) return AppendQuery(filterKey, " < ", filterValue);
            if (Equals(filterOperator, ProcedureFilterOperators.LessThanOrEqual)) return AppendQuery(filterKey, " <= ", filterValue);
            if (Equals(filterOperator, ProcedureFilterOperators.Contains)) return AppendLikeQuery(filterKey, " like ", filterValue);
            if (Equals(filterOperator, ProcedureFilterOperators.StartsWith)) return GenerateStartwithQuery(filterKey, " like ", filterValue);
            if (Equals(filterOperator, ProcedureFilterOperators.EndsWith)) return GenerateEndwithQuery(filterKey, " like ", filterValue);
            if (Equals(filterOperator, ProcedureFilterOperators.Is)) return GenerateQuery(filterKey, " = ", filterValue);
            if (Equals(filterOperator, ProcedureFilterOperators.NotIn)) return GenerateNotInQuery(filterKey, "not in", filterValue);
            if (Equals(filterOperator, ProcedureFilterOperators.In)) return GenerateSPInQuery(filterKey, "in", filterValue);
            if (Equals(filterOperator, ProcedureFilterOperators.NotEquals)) return GenerateNotaEquals(filterKey, "!=", filterValue);
            if (Equals(filterOperator, ProcedureFilterOperators.Between)) return AppendQuery(filterKey, " between ", filterValue);

            return query;
        }
    }
}
