using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    public class WidgetCategoryListViewModel : BaseViewModel
    {
        public List<WidgetCategoryViewModel> Categories { set; get; }

        public bool IsEmpty
        {
            get
            {
                return Categories?.Count > 0 ? false : true;

            }
        }
    }
}