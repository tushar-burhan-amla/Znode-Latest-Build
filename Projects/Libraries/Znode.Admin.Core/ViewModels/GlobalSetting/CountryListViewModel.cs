using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class CountryListViewModel : BaseViewModel
    {
        public CountryListViewModel()
        {
            Countries = new List<CountryViewModel>();
        }
        public List<CountryViewModel> Countries { get; set; }
        public GridModel GridModel { get; set; }
        public int PortalId { get; set; }
        public string PortalName { get; set; }
    }
}