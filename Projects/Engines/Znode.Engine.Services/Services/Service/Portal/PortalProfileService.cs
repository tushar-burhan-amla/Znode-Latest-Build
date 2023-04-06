using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using Znode.Libraries.Resources;

namespace Znode.Engine.Services
{
    public class PortalProfileService : BaseService, IPortalProfileService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodePortalProfile> _portalProfileRepository;
        private readonly IZnodeRepository<ZnodeProfile> _profileRepository;
        private readonly IZnodeRepository<ZnodeUserProfile> _userProfileRepository;
        #endregion

        #region Constructor
        public PortalProfileService()
        {
            _portalProfileRepository = new ZnodeRepository<ZnodePortalProfile>();
            _profileRepository = new ZnodeRepository<ZnodeProfile>();
            _userProfileRepository = new ZnodeRepository<ZnodeUserProfile>();
        }
        #endregion

        //Get Portal Profile List.
        public virtual PortalProfileListModel GetPortalProfiles(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            //Bind the Filter, Sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            IZnodeViewRepository<PortalProfileModel> objStoredProc = new ZnodeViewRepository<PortalProfileModel>();
            ZnodeLogging.LogMessage("pageListModel: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            PortalProfileListModel portalProfileListModel = new PortalProfileListModel() { PortalProfiles = objStoredProc.ExecuteStoredProcedureList("Znode_GetPortalProfileList @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount).ToList() };

            portalProfileListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return portalProfileListModel;
        }

        public virtual PortalProfileModel GetPortalProfile(int portalProfileId, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (portalProfileId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorPortalProfileIdNo);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePortalProfileEnum.PortalProfileID.ToString(), FilterOperators.Equals, portalProfileId.ToString()));
            return _portalProfileRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection())?.WhereClause, new List<string> { ZnodePortalProfileEnum.ZnodeProfile.ToString() })?.ToModel<PortalProfileModel>();
        }

        public virtual PortalProfileModel CreatePortalProfile(PortalProfileModel portalProfileModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (IsNull(portalProfileModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorModelNull);

            if (portalProfileModel.PortalId < 1 || portalProfileModel.ProfileId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorPortalProfileIdAndPortalIdNo);
           
            //If new added model has IsDefaultRegisteredProfile or IsDefaultRegisteredProfile true ,it disables previous ones. 
            DisableAssociatedProfiles(portalProfileModel);

            PortalProfileModel portalProfile = _portalProfileRepository.Insert(portalProfileModel.ToEntity<ZnodePortalProfile>())?.ToModel<PortalProfileModel>();
            ZnodeLogging.LogMessage("Inserted portalProfile ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose);
            ZnodeLogging.LogMessage(IsNotNull(portalProfile) ? Admin_Resources.SuccessCreatePortalProfile : Admin_Resources.ErrorCreatePortalProfile, string.Empty, TraceLevel.Info);

            return portalProfile;
        }

        public virtual bool UpdatePortalProfile(PortalProfileModel portalProfileModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (IsNull(portalProfileModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorModelNull);

            if (portalProfileModel.PortalProfileID < 1 || portalProfileModel.ProfileId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorProfileIdAndPortalIdNo);

            //If new added or update model has IsDefaultRegisteredProfile or IsDefaultRegisteredProfile true ,it disables previous ones. 
            DisableAssociatedProfiles(portalProfileModel);

            bool isUpdated = _portalProfileRepository.Update(portalProfileModel.ToEntity<ZnodePortalProfile>());


            ZnodeLogging.LogMessage(isUpdated ? Admin_Resources.SuccessUpdatePortalProfile : Admin_Resources.ErrorUpdatePortalProfile, string.Empty, TraceLevel.Info);
            return isUpdated;
        }

        public virtual bool DeletePortalProfile(ParameterModel portalProfileId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (IsNull(portalProfileId) || string.IsNullOrEmpty(portalProfileId.Ids))
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorProfileIdNo);

            //Generates filter clause for multiple portal Profile Ids.
            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodePortalProfileEnum.PortalProfileID.ToString(), ProcedureFilterOperators.In, portalProfileId.Ids));

            List<int> profileIds = GetPortalIdsByPortalProfileIds(portalProfileId.Ids);
            ZnodeLogging.LogMessage("Delete PortalProfile with id ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, portalProfileId?.Ids);
            bool isDeleted = false;
            try
            {
                IList<ZnodePortalProfile> profileList = _portalProfileRepository.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause);
                if (profileList.Any(x => x.IsDefaultAnonymousProfile == true) || profileList.Any(x => x.IsDefaultRegistedProfile == true))
                    return isDeleted;
                else
                    //Returns true if portal Profile deleted successfully else return false.
                    isDeleted = _portalProfileRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause);
            }
            catch (System.Exception ex)
            {
                if (ex.InnerException.ToString().Contains("FK_ZnodePriceListProfile_ZnodePortalProfile"))
                    throw new ZnodeException(ErrorCodes.SQLExceptionDuringPublish, Admin_Resources.ErrorDeletingProfileAsAssociatedWithPriceList);
            }

            ZnodeLogging.LogMessage(isDeleted ? Admin_Resources.SuccessDeletePortalProfile : Admin_Resources.ErrorDeletePortalProfile, string.Empty, TraceLevel.Info);
            return isDeleted;
        }

        // Get portal ids by portal profileids
        public virtual List<int> GetPortalIdsByPortalProfileIds(string portalProfileIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            int[] arrProfileId = portalProfileIds.Split(',').Select(Int32.Parse).ToArray();

            var profiles = (from profile in _profileRepository.Table
                            join portalProfile in _portalProfileRepository.Table on profile.ProfileId equals portalProfile.ProfileId
                            where arrProfileId.Contains(portalProfile.PortalProfileID)
                            && profile.ParentProfileId != null
                            select profile.ProfileId)?.ToList() ?? null;
            ZnodeLogging.LogMessage("Profiles:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info, profiles);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return profiles;
        }

        //Delete Child Profile
        public virtual bool DeleteChildProfile(List<int> profileIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            bool isDeleted = true;
            if (IsNotNull(profileIds) && profileIds.Count > 0)
            {
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodeUserProfileEnum.ProfileId.ToString(), ProcedureFilterOperators.In, string.Join(",", profileIds)));
                ZnodeLogging.LogMessage("Delete PortalProfile with id ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, profileIds);
                _userProfileRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);

                ParameterModel model = new ParameterModel();
                model.Ids = string.Join(",", profileIds);
                IProfileService _profileService = ZnodeDependencyResolver.GetService<IProfileService>();
                try
                {
                    isDeleted = _profileService.DeleteProfile(model);
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                    throw new ZnodeException(ErrorCodes.ExceptionalError, "Failed to delete profile as it may be associated.");
                }
            }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return isDeleted;
        }

        //Copy the user profile.
        public virtual bool CopyUserProfile(PortalProfileModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, ZnodeConstant.NullModelError);

            if (string.IsNullOrEmpty(model.ProfileName))
                throw new ZnodeException(ErrorCodes.InvalidData,Admin_Resources.ProfileNameNotBlank);

            ZnodeProfile profile = _profileRepository.Table.Where(x => x.ProfileId == model.ProfileId).FirstOrDefault();
            if (IsNotNull(profile) && IsNull(model?.ParentProfileId))
            {
                if (!profile.ProfileName.Equals(model.ProfileName, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (_profileRepository.Table.Where(x => x.ProfileName.Trim() == model.ProfileName.Trim()).Any())
                        throw new ZnodeException(ErrorCodes.AlreadyExist,Admin_Resources.ProfileNameAlreadyExist);
                    else
                    {
                        try
                        {
                            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
                            objStoredProc.SetParameter(ZnodePortalProfileEnum.PortalId.ToString(), model.PortalId, ParameterDirection.Input, DbType.Int32);
                            objStoredProc.SetParameter(ZnodePortalProfileEnum.ProfileId.ToString(), model.ProfileId, ParameterDirection.Input, DbType.Int32);
                            objStoredProc.SetParameter(ZnodeProfileEnum.ProfileName.ToString(), model.ProfileName, ParameterDirection.Input, DbType.String);
                            objStoredProc.SetParameter(ZnodeUserEnum.UserId.ToString(), GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
                            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
                            int status = 0;
                            objStoredProc.ExecuteStoredProcedureList("Znode_CopyUserProfile @PortalId, @ProfileId,@ProfileName,@UserId, @Status OUT", 4, out status);
                            return status == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                            throw new ZnodeException(ErrorCodes.ExceptionalError, ex.Message);
                        }
                    }
                }
            }
            else if (IsNotNull(profile))
            {
                if (!profile.ProfileName.Equals(model.ProfileName, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (!_profileRepository.Table.Where(x => x.ProfileName.Trim() == model.ProfileName.Trim() && x.ProfileId != model.ProfileId).Any())
                    {
                        profile.ProfileName = model.ProfileName;
                        _profileRepository.Update(profile);
                    }
                    else
                        throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.ProfileNameAlreadyExist);
                }
            }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return true;
        }

        #region Private Method
        //If new added model has IsDefaultRegisteredProfile or IsDefaultRegisteredProfile true ,it disables previous ones. 
        private void DisableAssociatedProfiles(PortalProfileModel model)
        {
            //checks if the filter collection null
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePortalEnum.PortalId.ToString(), ProcedureFilterOperators.Equals, model.PortalId.ToString()));

            IList<ZnodePortalProfile> profileList = _portalProfileRepository.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);
            ZnodeLogging.LogMessage("profileList list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, profileList?.Count());
            if (profileList.Any(x => x.IsDefaultAnonymousProfile == true) && model.IsDefaultAnonymousProfile == false)
            {
                ZnodePortalProfile defaultAnonymousPortalProfile = profileList.FirstOrDefault(x => x.IsDefaultAnonymousProfile == true);
                if (defaultAnonymousPortalProfile.PortalProfileID == model.PortalProfileID)
                {
                    throw new ZnodeException(ErrorCodes.NotPermitted, Admin_Resources.ErrorAssociatedDefaultProfileUpdate);
                }
            }
            if (profileList.Any(x => x.IsDefaultRegistedProfile == true) && model.IsDefaultRegistedProfile == false)
            {
                ZnodePortalProfile defaultRegisteredPortalProfile = profileList.FirstOrDefault(x => x.IsDefaultRegistedProfile == true);
                if (defaultRegisteredPortalProfile.PortalProfileID == model.PortalProfileID)
                {
                    throw new ZnodeException(ErrorCodes.NotPermitted, Admin_Resources.ErrorAssociatedDefaultProfileUpdate);
                }
            }
            if (model.IsDefaultAnonymousProfile)
            {
                List<ZnodePortalProfile> defaultAnonymousProfileList = profileList.Where(x => x.IsDefaultAnonymousProfile).ToList();
                defaultAnonymousProfileList.ForEach(x => x.IsDefaultAnonymousProfile = false);
                defaultAnonymousProfileList.ForEach(x => _portalProfileRepository.Update(x));
            }

            if (model.IsDefaultRegistedProfile)
            {
                List<ZnodePortalProfile> defaultRegisteredProfileList = profileList.Where(x => x.IsDefaultRegistedProfile).ToList();
                defaultRegisteredProfileList.ForEach(x => x.IsDefaultRegistedProfile = false);
                defaultRegisteredProfileList.ForEach(x => _portalProfileRepository.Update(x));
            }
        }
        #endregion
    }
}
