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
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Services
{
    public class ProfileService : BaseService, IProfileService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeProfile> _profileRepository;
        #endregion

        #region Constructor
        public ProfileService()
        {
            _profileRepository = new ZnodeRepository<ZnodeProfile>();
        }
        #endregion

        #region Public Methods
        #region Profile 
        //Create new profile.
        public virtual ProfileModel CreateProfile(ProfileModel profileModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            if (HelperUtility.IsNull(profileModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            //Create new profile and _profileRepository it.
            ZnodeProfile profile = _profileRepository.Insert(profileModel.ToEntity<ZnodeProfile>());

            ZnodeLogging.LogMessage((profile?.ProfileId > 0) ? string.Format(Admin_Resources.SuccessInsertProfile,profileModel.ProfileName) : string.Format(Admin_Resources.ErrorInsertProfile, profileModel.ProfileName), ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Inserted profile with profileID: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, profile?.ProfileId);

            if (HelperUtility.IsNotNull(profile))
                return profile.ToModel<ProfileModel>();
            
            return profileModel;
        }

        //Update  profile.
        public virtual bool UpdateProfile(ProfileModel profileModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            if (Equals(profileModel, null))
                throw new ZnodeException(ErrorCodes.NullModel,Admin_Resources.ErrorProfileModelNull);
            if (profileModel.ProfileId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.IdCanNotBeLessThanOne);
            //Update profile.
            ZnodeLogging.LogMessage("profileID value: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, profileModel?.ProfileId);
            bool isProfileUpdated = _profileRepository.Update(profileModel.ToEntity<ZnodeProfile>());
            ZnodeLogging.LogMessage(isProfileUpdated ? string.Format(Admin_Resources.SuccessUpdateProfile,profileModel.ProfileName) : string.Format(Admin_Resources.ErrorUpdateProfile,profileModel.ProfileName), ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return isProfileUpdated;
        }

        //Get profile by profile id.
        public virtual ProfileModel GetProfile(int profileId)
        {
            if (profileId > 0)
                return _profileRepository.GetById(profileId).ToModel<ProfileModel>();

            return new ProfileModel();
        }

        //Get paged profile list.
        public virtual ProfileListModel GetProfileList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to get profile list: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            //Gets the entity list according to where clause, order by clause and pagination and maps the entity list to model
            List<ZnodeProfile> profileList = _profileRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount)?.ToList();
            ZnodeLogging.LogMessage("Profile list count: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, profileList?.Count());
            ProfileListModel listModel = new ProfileListModel();

            listModel.Profiles = profileList?.Count > 0 ? profileList.ToModel<ProfileModel>().ToList() : new List<ProfileModel>();

            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            //Returns result.
            return listModel;
        }

        //Delete profile  by profileId.
        public virtual bool DeleteProfile(ParameterModel profileId)
        {
            ZnodeLogging.LogMessage("DeleteProfile method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("profileId(s) to delete: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, profileId?.Ids);
            if (HelperUtility.IsNull(profileId) || string.IsNullOrEmpty(profileId.Ids))
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorProfileIdNo);

            try
            {
                IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
                objStoredProc.SetParameter(ZnodeProfileEnum.ProfileId.ToString(), profileId.Ids, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
                int status = 0;
                IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeleteProfile @ProfileId,  @Status OUT", 1, out status);
                ZnodeLogging.LogMessage("Deleted result count:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, deleteResult?.Count());
                if (deleteResult.FirstOrDefault().Status.Value)
                {
                    ZnodeLogging.LogMessage(string.Format(Admin_Resources.SuccessDeleteProfile, profileId.Ids), ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
                    return true;
                }
                else
                {
                    ZnodeLogging.LogMessage(Admin_Resources.ErrorDeleteUserProfile, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
                    throw new ZnodeException(ErrorCodes.AssociationDeleteError, Admin_Resources.ErrorDeleteUserProfile);
                }
            }
            catch (Exception)
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorDeleteUserProfile, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                throw;
            }
        }
        #endregion

        #region Profile Catalog
        //Get profile catalog list.
        public virtual ProfileCatalogListModel GetProfileCatalogList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            int profileId, isAssociated;
            //Get profileId and IsAssociated flag value from filter.
            GetValuesFromFilter(filters, out profileId, out isAssociated);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            IZnodeViewRepository<ProfileCatalogModel> objStoredProc = new ZnodeViewRepository<ProfileCatalogModel>();

            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@ProfileId", profileId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@IsAssociated", isAssociated, ParameterDirection.Input, DbType.Boolean);

            IList<ProfileCatalogModel> profileCatalogEntity = objStoredProc.ExecuteStoredProcedureList("Znode_GetProfileCatalog  @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT,@ProfileId,@IsAssociated", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("Profile catalog list count: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, profileCatalogEntity?.Count());

            ProfileCatalogListModel profileCatalogListModel = new ProfileCatalogListModel
            {
                ProfileCatalogs = profileCatalogEntity?.Count > 0 ? profileCatalogEntity.ToList() : null
            };
            profileCatalogListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            //Returns result.
            return profileCatalogListModel;
        }


       //Delete associated catalog to profile by profileId.
        public virtual bool DeleteAssociatedProfileCatalog(int profileId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            if (profileId <= 0) 
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorProfileIdLessThanOne);
            bool IsDeleted = false;

            ZnodeLogging.LogMessage("Input parameter to delete associated profile catalog: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { ProfileId = profileId });

            ZnodeProfile znodeProfile = _profileRepository.GetById(profileId);
            if (HelperUtility.IsNotNull(znodeProfile))
            {
                znodeProfile.PimCatalogId = null ; 
                IsDeleted = _profileRepository.Update(znodeProfile);
            }
            return IsDeleted;
        }


        //Method to associate Catalog Profile.
        public virtual bool AssociateCatalogToProfile(ProfileCatalogModel profileCatalogModel)
        {
            ZnodeLogging.LogMessage("AssociateCatalogsToProfile method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            if (profileCatalogModel?.PimCatalogId <= 0 && profileCatalogModel?.ProfileId <= 0)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorProfileCatalogModelCountLessThanZero);

            bool isProfileUpdated = false;

            ZnodeProfile znodeProfile = _profileRepository.GetById(profileCatalogModel.ProfileId);
            if(HelperUtility.IsNotNull(znodeProfile))
            {
                znodeProfile.PimCatalogId = profileCatalogModel?.PimCatalogId;
                isProfileUpdated = _profileRepository.Update(znodeProfile);
            }
                     
            if (isProfileUpdated)
            {
                ZnodeLogging.LogMessage(string.Format(Admin_Resources.SuccessAssociateCatalogToProfile,0, profileCatalogModel.ProfileId.ToString()), ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(string.Format(Admin_Resources.ErrorAssociateCatalogInProfile, profileCatalogModel.CatalogName), ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
                return false;
            }
        }
        #endregion
        #endregion

        #region Private Methods
        //Get profileId and IsAssociated flag value from filter.
        private static void GetValuesFromFilter(FilterCollection filters, out int profileId, out int isAssociated)
        {
            profileId = Convert.ToInt32(filters?.Where(x => x.Item1.Equals(ZnodeProfileEnum.ProfileId.ToString(), StringComparison.InvariantCultureIgnoreCase))?.FirstOrDefault()?.Item3);
            isAssociated = Convert.ToInt32(filters?.Where(x => x.Item1.Equals(FilterKeys.IsAssociated.ToString(), StringComparison.InvariantCultureIgnoreCase))?.FirstOrDefault()?.Item3);
            filters?.RemoveAll(x => x.FilterName.Equals(ZnodeProfileEnum.ProfileId.ToString().ToLower(), StringComparison.InvariantCultureIgnoreCase));
            filters?.RemoveAll(x => x.FilterName.Equals(FilterKeys.IsAssociated.ToString().ToLower(), StringComparison.InvariantCultureIgnoreCase));
        }
        #endregion
    }
}
