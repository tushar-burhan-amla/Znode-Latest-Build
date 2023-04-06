using System.Collections.Generic;
using System.Web.Mvc;

namespace Znode.Engine.Api.Models
{
    public class ERPConnectorControlModel : BaseModel
    {
        public ERPConnectorControlModel()
        {
            htmlAttributes = new Dictionary<string, object>();
        }
        public string Name { get; set; }
        public string Id { get; set; }
        public string CSSClass { get; set; }
        public string ControlType { get; set; }
        public string Value { get; set; }
        public string[] Values { get; set; }
        public string ControlLabel { get; set; }
        public bool IsDetailView { get; set; }
        public bool IsHeader { get; set; }
        public Dictionary<string, object> htmlAttributes { get; set; } = null;
        public List<SelectListItem> SelectOptions { get; set; } = null;
        public string HelpText { get; set; }
    }
}
