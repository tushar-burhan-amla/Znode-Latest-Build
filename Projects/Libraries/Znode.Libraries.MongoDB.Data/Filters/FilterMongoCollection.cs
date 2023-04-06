using System.Collections.Generic;

namespace Znode.Libraries.MongoDB.Data
{
    public class FilterMongoCollection : List<FilterMongoTuple>
	{
		public void Add(string filterName, string filterOperator, string filterValue) => Add(new FilterMongoTuple(filterName, filterOperator, filterValue));		
	}
}
