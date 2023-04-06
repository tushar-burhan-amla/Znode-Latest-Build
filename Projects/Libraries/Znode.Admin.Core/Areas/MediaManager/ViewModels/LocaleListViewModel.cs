using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class LocaleListViewModel : BaseViewModel
    {
        public LocaleListViewModel()
        {
            Locales = new List<LocaleViewModel>();
        }
        public List<LocaleViewModel> Locales { get; set; }
        public GridModel GridModel { get; set; }
        public int PortalId { get; set; }
        public string PortalLocaleId { get; set; }
        public string PortalName { get; set; }
    }
}