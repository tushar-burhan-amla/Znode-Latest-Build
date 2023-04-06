using System;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Admin.Maps
{
    public class RMARequestViewModelMap
    {
        //To Do:Currently working on it.
        /// <summary>
        /// Converts RMARequestModel to RMARequestViewModel.
        /// </summary>
        /// <param name="model">RMARequestModel</param>
        /// <returns>RMARequestViewModel</returns>
        public static RMARequestViewModel ToViewModel(RMARequestModel model)
        {
            if (Equals(model, null))
            {
                return null;
            }

            return new RMARequestViewModel()
            {
                RMARequestID = model.RmaRequestId,
                StoreName = model.StoreName,
                PortalId = model.PortalId,
                OmsOrderId = model.OmsOrderId,
                Comments = model.Comments,
                RequestDate = model.RequestDate,
                RequestStatus = model.RequestStatus,
                RmaRequestStatusId = model.RmaRequestStatusId,
                CustomerName = model.CustomerName,
                RequestNumber = model.RequestNumber,
                TaxCost = Equals(model.TaxCost, null) ? null : Convert.ToString(model.TaxCost.Value),
                SubTotal = Equals(model.SubTotal, null) ? null : Convert.ToString(model.SubTotal.Value),
                Total = Equals(model.Total, null) ? null : Convert.ToString(model.Total.Value),
                Discount = Equals(model.Discount, null) ? null : Convert.ToString(model.Discount.Value),
                TaxCostAmount = model.TaxCost,
                DiscountAmount = model.Discount,
                SubTotalAmount = model.SubTotal,
                TotalAmount = model.Total
            };
        }

        /// <summary>
        /// Converts RMARequestViewModel to RMARequestModel.
        /// </summary>
        /// <param name="viewModel">RMARequestViewModel</param>
        /// <returns>RMARequestModel</returns>
        public static RMARequestModel ToModel(RMARequestViewModel viewModel)
        {
            if (Equals(viewModel, null))
            {
                return null;
            }

            return new RMARequestModel()
            {
                RmaRequestId = viewModel.RMARequestID,
                StoreName = viewModel.StoreName,
                PortalId = viewModel.PortalId,
                OmsOrderId = viewModel.OmsOrderId,
                RequestDate = Convert.ToDateTime(viewModel.RequestDate),
                RequestStatus = viewModel.RequestStatus,
                RmaRequestStatusId = viewModel.RmaRequestStatusId,
                RequestNumber = viewModel.RequestNumber,
                Comments = viewModel.Comments,
                TaxCost = viewModel.TaxCostAmount,
                Discount = viewModel.DiscountAmount,
                Total = viewModel.TotalAmount,
                SubTotal = viewModel.SubTotalAmount
            };
        }
    }
}