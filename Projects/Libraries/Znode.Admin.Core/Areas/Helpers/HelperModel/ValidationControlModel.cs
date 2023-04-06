using System.Web.Mvc;

namespace Znode.Engine.Admin.AttributeValidationHelpers
{
    public class ValidationControlModel
    {
        public string Label { get; set; }
        public MvcHtmlString Control { get; set; }
    }
}