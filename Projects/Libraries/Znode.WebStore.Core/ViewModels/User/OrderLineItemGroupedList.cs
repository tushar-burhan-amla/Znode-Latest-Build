using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    public class OrderLineItemGroupedList
    {
        public string GroupId { get; set; }

        public List<OrderLineItemViewModel> Children { get; set; }
    }
}
