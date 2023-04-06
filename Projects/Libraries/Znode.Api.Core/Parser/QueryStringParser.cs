using System.Collections.Specialized;
using System.Web;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Parser
{
    public class QueryStringParser
    {
        private readonly string _queryString;

        public NameValueCollection Expands
        {
            get { return GetKeyValuePairs("expand"); }
        }

        public FilterCollection MongoFriendlyFilters
        {
            get { return GetTuples("filter", true); }
        }

        public FilterCollection Filters
        {
            get { return GetTuples("filter"); }
        }

        public NameValueCollection Sorts
        {
            get { return GetKeyValuePairs("sort"); }
        }

        public NameValueCollection MongoFriendlySorts
        {
            get { return GetKeyValuePairs("sort", true); }
        }

        public NameValueCollection Page
        {
            get { return GetKeyValuePairs("page"); }
        }

        public NameValueCollection Format
        {
            get { return GetKeyValuePairs("format"); }
        }

        public NameValueCollection Indent
        {
            get { return GetKeyValuePairs("indent"); }
        }

        public NameValueCollection Cache
        {
            get { return GetKeyValuePairs("cache"); }
        }

        public QueryStringParser(string queryString)
        {
            _queryString = queryString;
        }

        private NameValueCollection GetKeyValuePairs(string param, bool keepExactCasing = false)
        {
            var keyValuePairs = new NameValueCollection();
            var query = HttpUtility.ParseQueryString(keepExactCasing ? _queryString : _queryString.ToLower());

            var uriItemSeparator = ZnodeApiSettings.ZnodeApiUriItemSeparator;
            var uriKeyValueSeparator = ZnodeApiSettings.ZnodeApiUriKeyValueSeparator;

            foreach (var key in query.AllKeys)
            {
                if (key.ToLower() == param)
                {
                    var value = query.Get(key);
                    var items = value.Split(uriItemSeparator.ToCharArray());

                    foreach (var item in items)
                    {
                        if (item.Contains(uriKeyValueSeparator))
                        {
                            var set = item.Split(uriKeyValueSeparator.ToCharArray());
                            if (keepExactCasing)
                                keyValuePairs.Add(set[0], HttpUtility.HtmlDecode(set[1]).ToLower());
                            else
                                keyValuePairs.Add(set[0].ToLower(), HttpUtility.HtmlDecode(set[1]));
                        }
                        else
                        {
                            // Just make the value the same as the key, for consistency of code in other places
                            if (keepExactCasing)
                                keyValuePairs.Add(item, item.ToLower());
                            else
                                keyValuePairs.Add(item.ToLower(), item.ToLower());
                        }
                    }

                    break;
                }
            }

            return keyValuePairs;
        }

        private FilterCollection GetTuples(string param, bool keepExactCasing = false)
        {
            var filters = new FilterCollection();
            var query = HttpUtility.ParseQueryString(keepExactCasing ? _queryString : _queryString.ToLower());

            var uriItemSeparator = ZnodeApiSettings.ZnodeApiUriItemSeparator;
            var uriKeyValueSeparator = ZnodeApiSettings.ZnodeApiUriKeyValueSeparator;
            string commaReplacer = ZnodeApiSettings.ZnodeCommaReplacer;

            foreach (var key in query.AllKeys)
            {
                if (key.ToLower() == param)
                {
                    var value = query.Get(key);
                    var items = value.Split(uriItemSeparator.ToCharArray());

                    foreach (var item in items)
                    {
                        if (item.Contains(uriKeyValueSeparator))
                        {
                            var tuple = item.Split(uriKeyValueSeparator.ToCharArray());
                            var filterKey = keepExactCasing ? tuple[0].Trim() : tuple[0].ToLower().Trim();
                            var filterOperator = keepExactCasing ? tuple[1].Trim() : tuple[1].ToLower().Trim();
                            var filterValue = keepExactCasing ? tuple[2].Trim() : tuple[2].ToLower().Trim();

                            filters.Add(new FilterTuple(filterKey, filterOperator, HttpUtility.HtmlDecode(filterValue?.Replace(commaReplacer, ","))));
                        }
                    }

                    break;
                }
            }

            return filters;
        }
    }
}