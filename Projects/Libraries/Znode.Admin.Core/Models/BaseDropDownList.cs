using System.Collections.Generic;

namespace Znode.Engine.Admin.Models
{
    public class BaseDropDownList
    {
        public string id { get; set; }

        public int DataId { get; set; }

        public string name { get; set; }

        public bool IsChecked { get; set; }

        public bool HasSublist { get; set; }

        public bool IsHide { get; set; } = false;

        public bool IsDisabled { get; set; } = false;

        public List<BaseDropDownList> Sublist { get; set; }
    }
}