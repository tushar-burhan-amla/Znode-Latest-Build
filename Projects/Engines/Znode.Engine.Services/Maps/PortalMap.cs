using System;
using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Services.Maps
{
    public static class PortalMap
    {
        //Map  ZnodePortal entity to Portal model.
        public static PortalModel ToModel(ZnodePortal portalEntity)
        {
            if (IsNotNull(portalEntity))
            {
                PortalModel portalModel = portalEntity.ToModel<PortalModel>();

                portalModel.DomainUrl = portalEntity?.ZnodeDomains?.Count > 0 ? portalEntity.ZnodeDomains.FirstOrDefault(x => x.IsActive && x.PortalId == portalEntity.PortalId && x.ApplicationType == ApplicationTypesEnum.WebStore.ToString())?.DomainName : "#";
                portalModel.OrderStatus = !Equals(portalEntity.ZnodeOmsOrderState, null) && !string.IsNullOrEmpty(portalEntity.ZnodeOmsOrderState.Description) ? portalEntity.ZnodeOmsOrderState.Description : string.Empty;
                portalModel.PublishCatalogId = !Equals(portalEntity.ZnodePortalCatalogs, null) && portalEntity.ZnodePortalCatalogs.FirstOrDefault()?.PublishCatalogId > 0 ? portalEntity.ZnodePortalCatalogs.FirstOrDefault()?.PublishCatalogId : null;
                portalModel.LocaleId = HelperUtility.IsNotNull(portalEntity.ZnodePortalLocales) && HelperUtility.IsNotNull(portalEntity.ZnodePortalLocales?.FirstOrDefault(x => x.IsDefault)) ? portalEntity.ZnodePortalLocales.FirstOrDefault(x => x.IsDefault)?.LocaleId : portalEntity.ZnodePortalLocales.FirstOrDefault()?.LocaleId;
                return portalModel;
            }
            return null;
        }

        //Map ZnodePortalFeature entity to PortalFeatureModel.
        public static PortalFeatureModel ToPortalFeatureModel(ZnodePortalFeature portalFeatureEntity)
        {
            if (IsNull(portalFeatureEntity))
                return null;

            return new PortalFeatureModel
            {
                PortalFeatureId = portalFeatureEntity.PortalFeatureId,
                PortalFeatureName = portalFeatureEntity.PortalFeatureName
            };
        }

        //Map list of ZnodePortalFeature entity to list of PortalFeatureModel.
        public static List<PortalFeatureModel> ToPortalFeatureModel(IList<ZnodePortalFeature> portalFeatureListEntity)
        {
            List<PortalFeatureModel> portalFeatureList = new List<PortalFeatureModel>();
            if (portalFeatureListEntity?.Count > 0)
            {
                foreach (ZnodePortalFeature portalFeatureEntity in portalFeatureListEntity)
                {
                    portalFeatureList.Add(ToPortalFeatureModel(portalFeatureEntity));
                }
            }
            return portalFeatureList;
        }

        //Map list of ZnodePortalFeatureMapper entity to list of PortalFeatureMapperModel.
        public static List<ZnodePortalFeatureMapper> ToPortalFeatureMapperListModel(int portalId, string[] portalFeatureIds)
        {
            List<ZnodePortalFeatureMapper> portalFeatureMapperList = new List<ZnodePortalFeatureMapper>();
            if (portalFeatureIds?.Length > 0)
            {
                for (int index = 0; index < portalFeatureIds.Length; index++)
                {
                    portalFeatureMapperList.Add(new ZnodePortalFeatureMapper { PortalId = portalId, PortalFeatureId = Convert.ToInt32(portalFeatureIds[index]) });
                }
            }
            return portalFeatureMapperList;
        }

        //Map list of ZnodePortalFeatureMapper entity to list of PortalFeatureModel.
        public static List<PortalFeatureModel> ToPortalFeatureListModel(IList<ZnodePortalFeatureMapper> portalFeatureMappers)
        {
            List<PortalFeatureModel> portalFeatureList = new List<PortalFeatureModel>();
            if (portalFeatureMappers?.Count > 0)
            {
                foreach (var portalFeature in portalFeatureMappers)
                {
                    if (!Equals(portalFeature.ZnodePortalFeature, null))
                        portalFeatureList.Add(new PortalFeatureModel { PortalFeatureName = portalFeature.ZnodePortalFeature.PortalFeatureName, PortalFeatureId = portalFeature.ZnodePortalFeature.PortalFeatureId });
                }
            }
            return portalFeatureList;
        }

        //Map list of  entity to list of User Approver List.
        public static List<UserApproverModel> ToUserApproverListModel(IList<ZnodeUserApprover> userApprovers)
        {
            List<UserApproverModel> _userApproverModel = new List<UserApproverModel>();
            if (userApprovers?.Count > 0)
            {
                foreach (var _userApprovers in userApprovers)
                {

                    _userApproverModel.Add(new UserApproverModel
                    {
                        UserApproverId = _userApprovers.UserApproverId,
                        UserId = _userApprovers.UserId,
                        ToBudgetAmount = _userApprovers.ToBudgetAmount,
                        FromBudgetAmount = _userApprovers.FromBudgetAmount,
                        ApproverOrder = _userApprovers.ApproverOrder,
                        ApproverLevelId = _userApprovers.ApproverLevelId.Value,
                        ApproverUserId = _userApprovers.ApproverUserId,
                        IsNoLimit = _userApprovers.IsNoLimit,
                        ApproverName = _userApprovers.ZnodeUser.Email,
                        IsActive = _userApprovers.IsActive,
                    });
                }
            }
            return _userApproverModel;
        }

        //To map list of user approvers to entity ZnodeUserApprover list
        public static List<ZnodeUserApprover> ToMapUserApprovalListModel(List<UserApproverModel> userApprovers, int portalApprovalId)
        {
            List<ZnodeUserApprover> _userApproverModel = new List<ZnodeUserApprover>();
            if (userApprovers?.Count > 0)
            {
                foreach (var _userApprovers in userApprovers)
                {
                    ZnodeUserApprover znodeUserApprover = new ZnodeUserApprover
                    {
                        UserApproverId = _userApprovers.UserApproverId,
                        UserId = _userApprovers.UserId,
                        ApproverOrder = _userApprovers.ApproverOrder,
                        ApproverLevelId = _userApprovers.ApproverLevelId,
                        ApproverUserId = _userApprovers.ApproverUserId,
                        IsActive = Convert.ToBoolean(_userApprovers.IsActive),
                        PortalApprovalId = _userApprovers.PortalApprovalId,
                    };
                    _userApproverModel.Add(znodeUserApprover);
                }
            }
            return _userApproverModel;
        }
    }
}
