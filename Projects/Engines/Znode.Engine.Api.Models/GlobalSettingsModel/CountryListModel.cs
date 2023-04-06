using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class CountryListModel :BaseListModel
    {
        public List<CountryModel> Countries { get; set; }

        public CountryListModel()
        {
            Countries = new List<CountryModel>();
        }
    }
}
