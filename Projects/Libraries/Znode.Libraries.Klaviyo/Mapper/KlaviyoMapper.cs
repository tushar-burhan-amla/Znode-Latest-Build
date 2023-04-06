using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Znode.Engine.Api.Models;

namespace Znode.Libraries.Klaviyo
{
    public static class KlaviyoMapper
    {
        /// <summary>
        /// Map datatable to KlaviyoProductDetailModel 
        /// </summary>
        /// <param name="dataTable">DataTable</param>
        /// <returns>return OrderDetailsModel</returns>
        public static OrderDetailsModel ToOrderDetailsModel(KlaviyoProductDetailModel klaviyoProductDetailModel)
        {
            OrderDetailsModel model = new OrderDetailsModel();
            model.FirstName = klaviyoProductDetailModel.FirstName;
            model.LastName = klaviyoProductDetailModel.LastName;
            model.Email = klaviyoProductDetailModel.Email;
            model.IsProductDisplayPage = klaviyoProductDetailModel.IsProductDisplayPage;
            model.OrderLineItems = new List<OrderLineItemDetailsModel>();
            
        foreach (Znode.Engine.Api.Models.OrderLineItemDetailsModel row in klaviyoProductDetailModel.OrderLineItems)
            {
                model.OrderLineItems.Add( new OrderLineItemDetailsModel {ItemName = row.ItemName, Value=row.Value});
            }
            return model;
        }

        /// <summary>
        /// Map datatable to UserModel 
        /// </summary>
        /// <param name="dataTable">DataTable</param>
        /// <returns>return identifyModel</returns>
        public static IdentifyModel ToIdentifyModel(UserModel userModel)
        {
            IdentifyModel identifyModel = new IdentifyModel();
            identifyModel.Email = userModel.Email;
            identifyModel.FirstName = userModel.FirstName;
            identifyModel.LastName = userModel.LastName;

            return identifyModel;
        }
    }
}
