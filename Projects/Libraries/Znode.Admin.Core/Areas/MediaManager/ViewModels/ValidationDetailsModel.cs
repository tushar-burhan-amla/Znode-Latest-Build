using System.Web.Mvc;

namespace Znode.Engine.Admin.ViewModels
{
    public class ValidationDetailsModel : BaseViewModel
    {
        public string LabelName { get; set; }
        public MvcHtmlString ControlName { get; set; }
    }
}