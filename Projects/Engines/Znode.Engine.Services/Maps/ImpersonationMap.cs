using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services.Maps
{
    public static class ImpersonationMap
    {
        // This method will map the ZnodeUserAddress Entity to AddressModel.
        public static ImpersonationActivityLogModel ToModel(ZnodeImpersonationLog entity)
        {
            if (HelperUtility.IsNotNull(entity))
            {
                ImpersonationActivityLogModel impersonationLog = entity.ToModel<ImpersonationActivityLogModel>();
                if (HelperUtility.IsNotNull(impersonationLog))
                {
                 
                    impersonationLog.CSRId = entity.CSRId.GetValueOrDefault();
                    impersonationLog.OperationType = entity.OperationType;
                    impersonationLog.ActivityType = entity.ActivityType;
                    impersonationLog.ShopperId = entity.WebstoreuserId.GetValueOrDefault();                
                    return impersonationLog;
                }
            }
            return null;
        }

        // This method will map the list of ZnodeUserAddress Entities to AddressListModel.
        public static ImpersonationActivityListModel ToImpersonationListModel(IList<ZnodeImpersonationLog> entities, PageListModel pageListModel)
        {
            ImpersonationActivityListModel impersonationActivityListModel = new ImpersonationActivityListModel();

            if (entities?.Count > 0)
            {
                impersonationActivityListModel.LogActivityList = new List<ImpersonationActivityLogModel>();
                foreach (ZnodeImpersonationLog logList in entities)
                {
                    ImpersonationActivityLogModel znodeImpersonationLog = ToModel(logList);
                    //Skip the null entries
                    if (HelperUtility.IsNotNull(logList))
                        impersonationActivityListModel.LogActivityList.Add(znodeImpersonationLog);
                }
            }
            impersonationActivityListModel.BindPageListModel(pageListModel);
            return impersonationActivityListModel;
        }
    }
}
