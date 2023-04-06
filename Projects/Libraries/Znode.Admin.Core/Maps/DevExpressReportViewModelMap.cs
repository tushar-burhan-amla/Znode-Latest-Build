using System;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Models;
using Znode.Libraries.DevExpress.Report;

namespace Znode.Engine.Admin.Maps
{
    public class DevExpressReportViewModelMap
    {
        public static ReportCategoryModel ToModel(ReportCategoryViewModel model)
        {
            if (!Equals(model, null))
            {
                return new ReportCategoryModel()
                {
                    CategoryName = model.CategoryName,
                    IsActive = model.IsActive
                };
            }
            return null;
        }

        public static ReportCategoryViewModel ToViewModel(ReportCategoryModel model)
        {
            if (!Equals(model, null))
            {
                return new ReportCategoryViewModel()
                {
                    CategoryName = model.CategoryName,
                    IsActive = model.IsActive,
                    ReportCategoryId = model.ReportCategoryId
                };
            }
            return null;
        }

        public static ReportDetailViewModel ToViewModel(ReportDetailModel model)
        {
            if (!Equals(model, null))
            {
                return new ReportDetailViewModel()
                {
                    ReportCategoryId = model.ReportCategoryId,
                    ReportCode = model.ReportCode,
                    ReportName = model.ReportName,
                    Description = model.Description
                };
            }
            return null;
        }

        public static OrderInfo ToViewModel(ReportOrderDetailsModel model)
        {
            if (!Equals(model, null))
            {
                return new OrderInfo()
                {
                    OrderNumber = model.OrderNumber,
                    BillingFirstName = model.BillingFirstName,
                    BillingLastName = model.BillingLastName,
                    BillingCompanyName = model.BillingCompanyName,
                    BillingPhoneNumber = model.BillingPhoneNumber,
                    BillingEmailId = model.BillingEmailId,
                    TaxCost = model.TaxCost,
                    ShippingCost = model.ShippingCost,
                    SubTotal = model.SubTotal,
                    DiscountAmount = model.DiscountAmount,
                    TotalAmount = model.TotalAmount,
                    OrderDate = model.OrderDate,
                    PurchaseOrderNumber = model.PurchaseOrderNumber,
                    SalesTax = model.SalesTax,
                    StoreName = model.StoreName,
                    OrderStatus = model.OrderStatus,
                    PaymentTypeName = model.PaymentTypeName,
                    ShippingTypeName = model.ShippingTypeName,
                    Symbol = model.Symbol
                };
            }
            return null;
        }
    }
}
