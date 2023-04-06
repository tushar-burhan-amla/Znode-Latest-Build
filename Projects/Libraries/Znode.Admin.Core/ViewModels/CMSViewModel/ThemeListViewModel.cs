using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class ThemeListViewModel : BaseViewModel
    {
        public List<ThemeViewModel> ThemeList { get; set; }
        public GridModel GridModel { get; set; }
        public int CMSThemeId { get; set; }
        public ThemeListViewModel()
        {
            ThemeList = new List<ThemeViewModel>();
        }
    }
}