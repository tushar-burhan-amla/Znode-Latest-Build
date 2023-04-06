using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class CustomFieldListViewModel : BaseViewModel
    {
        public List<CustomFieldViewModel> CustomFields { get; set; }
        public GridModel GridModel { get; set; }
        public int ProductId { get; set; }

        public CustomFieldListViewModel()
        {
            CustomFields = new List<CustomFieldViewModel>();
        }
    }
}