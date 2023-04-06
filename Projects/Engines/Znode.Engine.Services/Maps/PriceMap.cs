using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;

namespace Znode.Engine.Services.Maps
{
    public static class PriceMap
    {
        //Mapping list of PricePortalModel to list of ZnodePriceListPortal entity.
        public static List<ZnodePriceListPortal> ToAssociateStoreListEntity(List<PricePortalModel> pricePortalListModel)
        {
            List<ZnodePriceListPortal> priceListPortal = new List<ZnodePriceListPortal>();
            if (pricePortalListModel?.Count > 0)
            {
                foreach (var pricePortal in pricePortalListModel)
                {
                    priceListPortal.Add(ToAssociateStoreEntity(pricePortal));
                }
            }
            return priceListPortal;
        }

        //Mapping PricePortalModel to ZnodePriceListPortal entity.
        public static ZnodePriceListPortal ToAssociateStoreEntity(PricePortalModel pricePortalModel)
        {
            if (!Equals(pricePortalModel, null))
            {
                return new ZnodePriceListPortal
                {
                    PriceListId = pricePortalModel.PriceListId,
                    PortalId = pricePortalModel.PortalId
                };
            }
            else
                return null;
        }

        ////Mapping list of PriceAccountModel to list of ZnodePriceListAccount entity.
        //public static List<ZnodePriceListAccount> ToAssociateCustomerListEntity(List<PriceAccountModel> priceAccountListModel)
        //{
        //    List<ZnodePriceListAccount> priceListAccount = new List<ZnodePriceListAccount>();
        //    if (priceAccountListModel?.Count > 0)
        //    {
        //        foreach (var priceAccount in priceAccountListModel)
        //        {
        //            priceListAccount.Add(ToAssociateAccountEntity(priceAccount));
        //        }
        //    }
        //    return priceListAccount;
        //}

        ////Mapping PriceAccountModel to ZnodePriceListAccount entity.
        //public static ZnodePriceListAccount ToAssociateAccountEntity(PriceAccountModel priceAccountModel)
        //{
        //    if (!Equals(priceAccountModel, null))
        //    {
        //        return new ZnodePriceListAccount
        //        {
        //            PriceListId = priceAccountModel.PriceListId,
        //            AccountId = priceAccountModel.AccountId
        //        };
        //    }
        //    else
        //        return null;
        //}
    }
}
