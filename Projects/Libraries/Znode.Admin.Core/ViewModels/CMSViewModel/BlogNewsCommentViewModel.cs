using System.Collections.Generic;
using System.Web.Mvc;

namespace Znode.Engine.Admin.ViewModels
{
    public class BlogNewsCommentViewModel : BaseViewModel
    {
        public int BlogNewsId { get; set; }
        public int PortalId { get; set; }
        public int LocaleId { get; set; }
        public int BlogNewsCommentId { get; set; }
        public int? UserId { get; set; }
        public int BlogNewsCommentLocaleId { get; set; }
        public int CountComments { get; set; }

        public bool IsApproved { get; set; }

        public string Customer { get; set; }
        public string BlogNewsComment { get; set; }
        public string BlogNewsTitle { get; set; }
        public string StoreName { get; set; }
        public string BlogNewsType { get; set; }
        public string IsApprovedValue { get; set; }
        public string LocaleName { get; set; }

        public List<SelectListItem> IsApprovedList { get; set; }
    }
}
