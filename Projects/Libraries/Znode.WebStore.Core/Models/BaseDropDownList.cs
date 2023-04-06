using System.Collections.Generic;

namespace Znode.Engine.WebStore.Models
{
    public class BaseDropDownList
    {
        public string id { get; set; }

        public int DataId { get; set; }

        public string name { get; set; }

        public bool IsChecked { get; set; }

        public bool HasSublist { get; set; }

        public List<BaseDropDownList> Sublist { get; set; }
    }
}