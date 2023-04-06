using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;

namespace Znode.Libraries.MongoDB.Data
{
    public static class MongoQueryHelper
    {
        /// <summary>
        /// Generate mongo Query from FilterCollection
        /// </summary>
        /// <param name="filters">FilterCollection</param>
        /// <returns>Mongo Query</returns>
        public static IMongoQuery GenerateDynamicWhereClause(FilterMongoCollection filters)
        {
            List<IMongoQuery> query = new List<IMongoQuery>();
            List<IMongoQuery> orQuery = new List<IMongoQuery>();
            if (filters?.Count > 0)
            {
                foreach (var tuple in filters)
                {
                    string filterKey = tuple.Item1;
                    string filterOperator = tuple.Item2;
                    string filterValue = tuple.Item3;
                    //Generate the or query.
                    if (filterKey.Contains("|"))
                        foreach (string item in filterKey.Split('|'))
                        {
                            if (!string.IsNullOrEmpty(item))
                                orQuery.Add(SetQueryParameter(item, filterOperator, filterValue));
                        }
                    else
                        query.Add(SetQueryParameter(filterKey, filterOperator, filterValue));
                }
            }
            query.RemoveAll(x => x == null);
            orQuery.RemoveAll(x => x == null);

            if (query.Count > 0 && orQuery.Count > 0)
                return Query.And(Query.And(query), Query.Or(orQuery));
            else if (query.Count > 0)
                return Query.And(query);
            else if (orQuery.Count > 0)
                return Query.Or(orQuery);
            return Query.Empty;
        }

        /// <summary>
        /// Generate mongo Query from FilterCollection
        /// </summary>
        /// <param name="filters">FilterCollection</param>
        /// <returns>Mongo Query</returns>
        public static IMongoQuery GenerateDynamicElemMatchWhereClause(FilterMongoCollection filters, string elementName)
        {
            if (string.IsNullOrEmpty(elementName))
                return Query.Empty;

            IMongoQuery finalQuery = Query.Empty;
            List<IMongoQuery> query = new List<IMongoQuery>();
            List<IMongoQuery> attrQuery = new List<IMongoQuery>();
            List<IMongoQuery> orQuery = new List<IMongoQuery>();
            List<IMongoQuery> intermediateQuery = new List<IMongoQuery>();

            if (filters?.Count > 0)
            {
                bool checkAttributeOnce = false;
                foreach (var tuple in filters)
                {
                    string filterKey = tuple.Item1;
                    string filterOperator = tuple.Item2;
                    string filterValue = tuple.Item3;

                    if (filterKey.ToLower().Contains($"{elementName}.".ToLower()))
                    {
                        if (!checkAttributeOnce)
                            attrQuery.Add(SetQueryParameter(filterKey.Substring(filterKey.IndexOf('.')).Trim('.'), filterOperator, filterValue));
                    }
                    else
                    {
                        if (filterKey.Contains("|"))
                            foreach (string item in filterKey.Split('|'))
                            {
                                if (!string.IsNullOrEmpty(item))
                                    orQuery.Add(SetQueryParameter(item, filterOperator, filterValue));
                            }
                        else
                            query.Add(SetQueryParameter(filterKey, filterOperator, filterValue));
                    }

                    if (attrQuery.Count == 2)
                    {
                        intermediateQuery.Add(Query.And(attrQuery));
                        attrQuery.Clear();
                        checkAttributeOnce = true;
                    }
                }
            }
            query.RemoveAll(x => x == null);
            orQuery.RemoveAll(x => x == null);
            attrQuery.RemoveAll(x => x == null);

            if (query.Count > 0 && orQuery.Count > 0)
                finalQuery = Query.And(Query.And(query), Query.Or(orQuery));
            else if (query.Count > 0)
                finalQuery = Query.And(query);
            else if (orQuery.Count > 0)
                finalQuery = Query.Or(orQuery);

            if (intermediateQuery.Count > 0)
                finalQuery = Query.And(finalQuery, Query.ElemMatch(elementName, Query.Or(intermediateQuery)));

            return finalQuery;
        }

        /// <summary>
        /// Generate Mongo Sort by clause 
        /// </summary>
        /// <param name="sorts">NameValueCollection</param>
        /// <returns>Mongo Sort by clause</returns>
        public static IMongoSortBy GenerateDynamicOrderByClause(NameValueCollection sorts)
        {
            IMongoSortBy sort = null;
            if (sorts?.Count > 0 && sorts.HasKeys())
            {
                foreach (string key in sorts.AllKeys)
                {
                    if (!string.IsNullOrWhiteSpace(key))
                    {
                        sort = ("asc".Equals(sorts.Get(key))) ? SortBy.Ascending(key) : SortBy.Descending(key);
                    }
                }
            }
            return sort;
        }

        //Returns query based on Input filter parameters
        private static IMongoQuery SetQueryParameter(string filterKey, string filterOperator, string filterValue)
        {
            bool isSkuFilterKey = filterKey.ToLower() == "sku";
            bool isInClause = (Equals(filterOperator, MongoFilterOperators.In) || Equals(filterOperator, MongoFilterOperators.NotIn));
            var parsedString = (isSkuFilterKey || isInClause) ? filterValue : ParseString(filterValue);


            if (Equals(filterOperator, MongoFilterOperators.Equals)) return Query.EQ(filterKey, BsonValue.Create(parsedString));
            if (Equals(filterOperator, MongoFilterOperators.NotEquals)) return Query.NE(filterKey, BsonValue.Create(parsedString));
            if (Equals(filterOperator, MongoFilterOperators.GreaterThan)) return Query.GT(filterKey, BsonValue.Create(parsedString));
            if (Equals(filterOperator, MongoFilterOperators.GreaterThanOrEqual)) return Query.GTE(filterKey, BsonValue.Create(parsedString));
            if (Equals(filterOperator, MongoFilterOperators.LessThan)) return Query.LT(filterKey, BsonValue.Create(parsedString));
            if (Equals(filterOperator, MongoFilterOperators.LessThanOrEqual)) return Query.LTE(filterKey, BsonValue.Create(parsedString));
            if (Equals(filterOperator, MongoFilterOperators.In)) return Query.In(filterKey, GetBsonArray(filterValue, isSkuFilterKey));
            if (Equals(filterOperator, MongoFilterOperators.NotIn)) return Query.NotIn(filterKey, GetBsonArray(filterValue, isSkuFilterKey));

            if (Equals(filterOperator, MongoFilterOperators.Contains)) return Query.Matches(filterKey, BsonRegularExpression.Create(new Regex($"{ReplaceWithEscapSequence(filterValue)}", RegexOptions.IgnoreCase)));
            if (Equals(filterOperator, MongoFilterOperators.Like)) return Query.Matches(filterKey, BsonRegularExpression.Create(new Regex($"{ReplaceWithEscapSequence(filterValue)}", RegexOptions.IgnoreCase)));
            if (Equals(filterOperator, MongoFilterOperators.StartsWith)) return Query.Matches(filterKey, BsonRegularExpression.Create(new Regex($"^{ReplaceWithEscapSequence(filterValue)}", RegexOptions.IgnoreCase)));
            if (Equals(filterOperator, MongoFilterOperators.EndsWith)) return Query.Matches(filterKey, BsonRegularExpression.Create(new Regex($"{ReplaceWithEscapSequence(filterValue)}$", RegexOptions.IgnoreCase)));
            if (Equals(filterOperator, MongoFilterOperators.Is)) return Query.Matches(filterKey, BsonRegularExpression.Create(new Regex($"^{ReplaceWithEscapSequence(filterValue)}$", RegexOptions.IgnoreCase)));

            return null;
        }

        //Generate Bson Array from Filter values
        private static BsonArray GetBsonArray(string filterValue, bool isSkuFilterKey)
        {
            string[] temp = filterValue.Split(',');
            BsonArray bsonArray = new BsonArray();
            if (temp.Length > 0)
            {
                foreach (var values in temp)
                {
                    bsonArray.Add(BsonValue.Create(isSkuFilterKey ? values : ParseString(values)));
                }
            }
            return bsonArray;
        }

        //Parse String to specific datatype
        private static object ParseString(string filterValue)
        {
            bool boolValue;
            int intValue;
            long bigintValue;
            double doubleValue;
            DateTime dateValue;

            // Place checks higher in if-else statement to give higher priority to type.
            if (string.IsNullOrEmpty(filterValue))
                return null;
            else if (bool.TryParse(filterValue, out boolValue))
                return boolValue;
            else if (int.TryParse(filterValue, out intValue))
                return intValue;
            else if (long.TryParse(filterValue, out bigintValue))
                return bigintValue;
            else if (double.TryParse(filterValue, out doubleValue))
                return doubleValue;
            else if (DateTime.TryParse(filterValue, out dateValue))
                return dateValue;
            else return BsonRegularExpression.Create(new Regex($"^{ReplaceWithEscapSequence(filterValue)}$", RegexOptions.IgnoreCase));
        }

        //Add escape sequence with special characters.
        public static string ReplaceWithEscapSequence(string filterValue)
        {
            //char array of all special characters.
            char[] specialCharacters = { '~', '`', '!', '@', '#', '$', '%', '\'', '&', '*', '(', ')', '=', '+', '|', ']', '}', '[', '{', '"', ':', ';', '?', '/', '.', '>', ',', '<', '\\' };

            //Check if filter value contains special character.
            if (!string.IsNullOrEmpty(filterValue) && filterValue.IndexOfAny(specialCharacters) >= 0)
            {
                IEnumerable<char> characters = filterValue.Distinct();

                //Add escape sequence with special characters if occurs.
                foreach (var item in characters)
                {
                    //Check if single colon present in filter value and replace the value.
                    if (filterValue.Contains("''"))
                        filterValue = filterValue.Replace("''", "\\'");
                    else if (specialCharacters.Contains(item) && !Equals(item, '\''))
                    {
                        filterValue = filterValue.Replace(item.ToString(), "\\" + item.ToString());
                    }
                }
            }
            return filterValue;
        }
    }
}
