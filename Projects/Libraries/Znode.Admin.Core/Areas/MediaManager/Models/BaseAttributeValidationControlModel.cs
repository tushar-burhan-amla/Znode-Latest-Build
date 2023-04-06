using System.Web.Mvc;

namespace Znode.Engine.Admin.ViewModels
{
    public class BaseAttributeValidationControlModel
    {
        public string Name { get; set; }
        public MvcHtmlString HtmlString { get; set; }
        public string ControlType { get; set; }
    }
}