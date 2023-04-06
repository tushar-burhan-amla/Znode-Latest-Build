using System.Collections.Generic;

namespace Znode.Engine.Recommendations.Models
{

    // TODO - Need to remove this HistoricalContext entirely. Instead, we need to handle 'ETL'-ing all data from Znode to
    // the engine in batches. This is necessary because there may be millions of orders in the system; too much to handle at once.

    public class HistoricalContext
    {
        public List<Customer> Customers { get; set; }
    }
}