using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Services
{
    public class RMARequestItemService : BaseService, IRMARequestItemService
    {
        private readonly IZnodeRepository<ZnodeRmaRequestItem> _rmaRequestItemRepository;

        public RMARequestItemService()
        {
            _rmaRequestItemRepository = new ZnodeRepository<ZnodeRmaRequestItem>();
        }

        //Create RMA Item Request.
        public virtual RMARequestItemModel CreateRMAItemRequest(RMARequestItemModel rmaRequestItemModel)
        {
            ZnodeLogging.LogMessage("CreateRMAItemRequest method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNull(rmaRequestItemModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorRMARequestItemModelNull);

            ZnodeRmaRequestItem rmaRequestItem = _rmaRequestItemRepository.Insert(rmaRequestItemModel.ToEntity<ZnodeRmaRequestItem>());
            ZnodeLogging.LogMessage("Inserted rmaRequestItem with id ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, rmaRequestItem?.RmaRequestId);

            ZnodeLogging.LogMessage("CreateRMAItemRequest method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return rmaRequestItem?.ToModel<RMARequestItemModel>() ?? new RMARequestItemModel();

        }

        //Get RMA Request Item List.
        public virtual RMARequestItemListModel GetRMARequestItemList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("GetRMARequestItemList method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            string flag;
            int omsOrderDetailsId, rmaId, isReturnable;
            //Set RMA Order Line Item Filter.
            SetRMAOrderLineItemFilter(filters, out flag, out omsOrderDetailsId, out rmaId, out isReturnable);

            RMARequestItemListModel listModel = new RMARequestItemListModel();

            IZnodeViewRepository<RMARequestItemModel> objStoredProc = new ZnodeViewRepository<RMARequestItemModel>();

            //SP call to revert order inventory, update this code once dba provide the sp.
            objStoredProc.SetParameter("@OmsOrderDetailsId", omsOrderDetailsId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@RMAId", rmaId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@IsReturnable", isReturnable, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Flag", flag, ParameterDirection.Input, DbType.String);

            List<RMARequestItemModel> list = objStoredProc.ExecuteStoredProcedureList("Znode_GetRMAOrderLineItem @OmsOrderDetailsId, @RMAId, @IsReturnable,@Flag").ToList();
            ZnodeLogging.LogMessage("Get RMA Request Item list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, list?.Count());
            listModel = new RMARequestItemListModel { RMARequestItemList = list?.ToList() };
            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("GetRMARequestItemList method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return listModel ?? new RMARequestItemListModel();

        }

      //Set RMA Order Line Item Filter.
        private static void SetRMAOrderLineItemFilter(FilterCollection filters, out string flag, out int omsOrderDetailsId, out int rmaId, out int isReturnable)
        {
            flag = string.Empty;
            omsOrderDetailsId = 0;
            rmaId = 0;
            isReturnable = 0;
            //Set filters for RMA Order Line Item.
            foreach (var filterItem in filters)
            {
                //set omsOrderDetailsId in filter.
                if (filterItem.Item1.Equals(ZnodeOmsOrderDetailEnum.OmsOrderDetailsId.ToString().ToLower()))
                    omsOrderDetailsId = Convert.ToInt32(filterItem.Item3);
                //rmaId in filter.
                else if (filterItem.Item1.Equals(FilterKeys.RMAId.ToLower()))
                    rmaId = Convert.ToInt32(filterItem.Item3);
                //Set RMA flag(create, edit, view, append) in filter.
                else if (filterItem.Item1.Equals(FilterKeys.Flag.ToLower()))
                    flag = filterItem.Item3;
                //Set isReturnable in filter.
                else if (filterItem.Item1.Equals(FilterKeys.IsReturnable.ToLower()))
                    isReturnable = Convert.ToInt32(filterItem.Item3);
            }
        }

        //Get RMA Request Items For Gift Card.
        public virtual RMARequestItemListModel GetRMARequestItemsForGiftCard(string orderLineItemList)
        {
            ZnodeLogging.LogMessage("GetRMARequestItemsForGiftCard method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (string.IsNullOrEmpty(orderLineItemList))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorOMSOrderLineItemIdsNullOrEmpty);

            if (HelperUtility.IsNotNull(orderLineItemList))
            {
                IZnodeViewRepository<RMARequestItemModel> objStoredProc = new ZnodeViewRepository<RMARequestItemModel>();
                //SP parameters
                objStoredProc.SetParameter("@RMARequestItemIDs", orderLineItemList, ParameterDirection.Input, DbType.String);

                List<RMARequestItemModel> rmaList = objStoredProc.ExecuteStoredProcedureList("Znode_GetOmsRMAOrderLineItem @RMARequestItemIDs").ToList();
                ZnodeLogging.LogMessage("rmaList list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, rmaList?.Count());
                return new RMARequestItemListModel { RMARequestItemList = rmaList?.ToList() };
            }
            ZnodeLogging.LogMessage("GetRMARequestItemsForGiftCard method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return new RMARequestItemListModel();
        }
    }
}
