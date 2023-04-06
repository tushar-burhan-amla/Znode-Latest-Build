using System;

namespace Znode.Libraries.ECommerce.Utilities
{
    public class FilterTuple : Tuple<string, string, string>
    {
        public string FilterName { get { return Item1; } }
        public string FilterOperator { get { return Item2; } }
        public string FilterValue { get { return Item3; } }

        public FilterTuple(string filterName, string filterOperator, string filterValue) : base(filterName, filterOperator, filterValue)
        {
        }
    }
}
