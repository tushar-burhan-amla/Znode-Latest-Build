using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class CSSListViewModel : BaseViewModel
    {
        public List<CSSViewModel> CssList { get; set; }
        public GridModel GridModel { get; set; }
        public int CMSThemeId { get; set; }
        public string CMSThemeName { get; set; }
    }
}