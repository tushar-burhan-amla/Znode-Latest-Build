using System.Collections.Generic;

namespace Znode.Engine.Admin.Models
{
    public class BaseDropDownOptionsModel
    {
        public int Id { get; set; }
        public string ControlId { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public bool Disabled { get; set; }
        public string Onchange { get; set; }
        public string SelectOptionName { get; set; }
        public List<BaseDropDownOptions> DropDownOptions { get; set; }
    }
}
