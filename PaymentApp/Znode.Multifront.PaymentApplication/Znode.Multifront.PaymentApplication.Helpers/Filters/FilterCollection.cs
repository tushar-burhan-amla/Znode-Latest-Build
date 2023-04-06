using System.Collections.Generic;

namespace Znode.Multifront.PaymentApplication.Helpers
{
    public class FilterCollection : List<FilterTuple>
	{
		public void Add(string filterName, string filterOperator, string filterValue) => Add(new FilterTuple(filterName, filterOperator, filterValue));		
	}
}
