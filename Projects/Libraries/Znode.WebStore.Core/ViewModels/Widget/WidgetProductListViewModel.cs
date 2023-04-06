using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    public class WidgetProductListViewModel : BaseViewModel
    {
        public List<WidgetProductViewModel> Products { set; get; }
        public string DisplayName { get; set; }
        public bool IsEmpty
        {
            get
            {
                return Products?.Count > 0 ? false : true;

            }
        }
    }
}