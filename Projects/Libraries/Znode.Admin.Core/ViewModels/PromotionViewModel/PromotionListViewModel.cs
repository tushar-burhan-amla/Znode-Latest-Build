using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class PromotionListViewModel : BaseViewModel
    {
        public List<PromotionViewModel> PromotionList { get; set; }
        public List<PromotionExportViewModel> PromotionExportList { get; set; }
        public GridModel GridModel { get; set; }
    }
}