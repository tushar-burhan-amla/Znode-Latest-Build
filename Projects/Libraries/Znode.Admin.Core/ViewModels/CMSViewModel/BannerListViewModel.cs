using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class BannerListViewModel : BaseViewModel
    {
        public List<BannerViewModel> Banners { get; set; }
        public GridModel GridModel { get; set; }
        public int CMSSliderId { get; set; }
        public int CMSSliderBannerId { get; set; }
        public string Name { get; set; }
        public bool IsWidgetAssociated { get; set; }
        public bool BannersCreated { get; set; }
    }
}