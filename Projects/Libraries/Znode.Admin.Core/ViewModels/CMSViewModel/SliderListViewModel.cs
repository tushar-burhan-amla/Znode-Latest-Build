using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class SliderListViewModel : BaseViewModel
    {
        public List<SliderViewModel> Sliders { get; set; }
        public GridModel GridModel { get; set; }
        public string Name { get; set; }
        public int CMSSliderId { get; set; }
        public bool? IsPublished { get; set; }
        public string PublishStatus { get { return (IsPublished == null || IsPublished == false) ? "Draft" : "Published"; } }
    }
}