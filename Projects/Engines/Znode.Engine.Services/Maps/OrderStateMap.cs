using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;

namespace Znode.Engine.Services.Maps
{
    public static class OrderStateMap
    {
        public static OrderStateModel ToModel(ZnodeOmsOrderState entity)
        {
            if (Equals(entity, null))
                return null;

            OrderStateModel model = new OrderStateModel
            {
                OrderStateId = entity.OmsOrderStateId,
                OrderStateName = entity.OrderStateName,
                Description = entity.Description,
                IsEdit = entity.IsEdit,
                IsOrderState = entity.IsOrderState,
                IsOrderLineItemState = entity.IsOrderLineItemState

            };
            return model;
        }
    }
}
