using System.Collections.Generic;

namespace Znode.Libraries.Search
{
    public class ElasticSearchProfileModel
    {
        public string QueryClass { get; set; } = "MatchQueryBuilder";
        public string QueryType { get; set; } = "";
        public string SubQueryType { get; set; }
        public string Operator { get; set; }

        public bool IsAutoCorrect { get; set; }
        public bool IsPromoteProduct { get; set; }
        public bool IsBlackListProduct { get; set; }

        public int[] PromoteProductIds { get; set; }
        public int[] BlackListProductIds { get; set; }

        public Dictionary<string, double> SearchableFields { get; set; }
        public List<string> ProfileFacets { get; set; }

        public List<SearchAttributes> AttributeList { get; set; }

        public ElasticSearchProfileModel()
        {
            //List<string> list = new List<string>();
            //list.Add("brand");
            //list.Add("color");
            //list.Add("specials");
            //list.Add("calories");
            //ProfileFacets = list;
        }
    }
}
