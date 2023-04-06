using System.Collections.Generic;
using System.Web.Mvc;

namespace Znode.Engine.Admin.ViewModels
{
    public class OrderStatusList : BaseViewModel
    {
        public int SelectedItemId { get; set; }
        public int OmsOrderId { get; set; }

        public string SelectedItemValue { get; set; }
        public string pageName { get; set; }
        public List<SelectListItem> listItem { get; set; }
        public string OrderTextValue { get; set; }
        public int OmsQuoteId { get; set; }
    }
}
