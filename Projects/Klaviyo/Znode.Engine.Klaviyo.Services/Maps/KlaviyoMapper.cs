using System.Collections.Generic;

using Znode.Engine.klaviyo.Models;

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
            model.StoreName = klaviyoProductDetailModel.StoreName;
            model.PropertyType = klaviyoProductDetailModel.PropertyType;
            model.OrderLineItems = new List<OrderLineItemDetailsModel>();

            foreach (OrderLineItemDetailsModel row in klaviyoProductDetailModel.OrderLineItems)
            {
                model.OrderLineItems.Add(new OrderLineItemDetailsModel { ProductName = row.ProductName, SKU = row.SKU, Quantity = row.Quantity, UnitPrice = row.UnitPrice, Image = row.Image });
            }
            return model;
        }

        /// <summary>
        /// Map datatable to UserModel 
        /// </summary>
        /// <param name="dataTable">DataTable</param>
        /// <returns>return identifyModel</returns>
        public static IdentifyModel ToIdentifyModel(IdentifyModel userModel)
        {
            IdentifyModel identifyModel = new IdentifyModel();
            identifyModel.Email = userModel.Email;
            identifyModel.FirstName = userModel.FirstName;
            identifyModel.LastName = userModel.LastName;
            identifyModel.StoreCode = userModel.StoreCode;
            identifyModel.StoreName = userModel.StoreName;
            identifyModel.CompanyName = userModel.CompanyName;
            identifyModel.PhoneNumber = userModel.PhoneNumber;
            identifyModel.UserName = userModel.UserName;

            return identifyModel;
        }
    }
}
