using System.Collections.Generic;

namespace Znode.Engine.Admin.Models
{
    public class AutoComplete
    {
        public int Id { get; set; }
        public string Name { get; set; }
       
        public string DisplayText { get; set; }

        public string text { get; set; }

        public string value { get; set; }

        public Dictionary<string, object> Properties { get; set; }

        public AutoComplete()
        {
            Properties = new Dictionary<string, object>();
        }
    }
}
