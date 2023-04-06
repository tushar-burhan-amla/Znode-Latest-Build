using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Services
{
    public class ContentWidgetService : BaseService, IContentWidgetService
    {
        #region Private variables
        protected readonly IZnodeRepository<ZnodeCMSContentWidget> _contentWidgetRepository;
        protected readonly IZnodeRepository<ZnodeCMSWidgetProfileVariant> _widgetProfileVariantRepository;
        protected readonly IZnodeRepository<ZnodePortal> _portalRepository;
        protected readonly IZnodeRepository<ZnodeProfile> _profileRepository;
        protected readonly IZnodeRepository<ZnodeGlobalEntityFamilyMapper> _entityFamilyMapperRepository;

        #endregion

        #region Public Constructor
        public ContentWidgetService()
        {
            _widgetProfileVariantRepository = new ZnodeRepository<ZnodeCMSWidgetProfileVariant>();
            _contentWidgetRepository = new ZnodeRepository<ZnodeCMSContentWidget>();
            _portalRepository = new ZnodeRepository<ZnodePortal>();
            _profileRepository = new ZnodeRepository<ZnodeProfile>();
            _entityFamilyMapperRepository = new ZnodeRepository<ZnodeGlobalEntityFamilyMapper>();
        }
        #endregion

        // Get the List of Content Widget
        public virtual ContentWidgetListModel List(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            //set paging parameters.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            IZnodeViewRepository<ContentWidgetResponseModel> objStoredProc = new ZnodeViewRepository<ContentWidgetResponseModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            IList<ContentWidgetResponseModel> contentWidgetListResponse = objStoredProc.ExecuteStoredProcedureList("Znode_GetCMSWidgetlist @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);

            ContentWidgetListModel contentWidgetListModel = new ContentWidgetListModel();
            contentWidgetListModel.WidgetList = contentWidgetListResponse?.Count > 0 ? contentWidgetListResponse?.ToList() : null;

            contentWidgetListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            return contentWidgetListModel;
        }

        //Create Content Widget
        public virtual ContentWidgetResponseModel Create(ContentWidgetCreateModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (Equals(model, null))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            if (IsWidgetKeyExists(model.WidgetKey))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.WidgetKeyExist);

            model.FamilyId = GetFamilyId(model.FamilyCode);
            model.PortalId = model.IsGlobalContentWidget ? null : GetPortalId(model.StoreCode);
 
            //Insert Content Widget Details
            ContentWidgetResponseModel contentWidget = _contentWidgetRepository.Insert(model.ToEntity<ZnodeCMSContentWidget>()).ToModel<ContentWidgetResponseModel>();
            contentWidget.GlobalEntityId = (int)EntityType.Widget;

            //Insert data into Entity Family Mapper
            ZnodeGlobalEntityFamilyMapper entityFamilyMapper = contentWidget.ToEntity<ZnodeGlobalEntityFamilyMapper>();
            _entityFamilyMapperRepository.Insert(entityFamilyMapper);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            return contentWidget;
        }
        
        //Update Content Widget
        public virtual ContentWidgetResponseModel Update(ContentWidgetUpdateModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (Equals(model, null))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            ZnodeCMSContentWidget contentWidget = _contentWidgetRepository.Table.FirstOrDefault(x => x.WidgetKey.ToLower() == model.WidgetKey.ToLower());

            if (Equals(contentWidget, null))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidWidgetKey);

            contentWidget.Tags = model.Tags;
            contentWidget.Name = model.Name;

            bool IsUpdated = _contentWidgetRepository.Update(contentWidget);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            return contentWidget.ToModel<ContentWidgetResponseModel>();
        }

        //Delete Content Widget
        public virtual bool DeleteContentWidget(ParameterModel contentWidgetIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            return DeleteWidgetByIds(contentWidgetIds.Ids);
        }

        //Delete Content Widget by Widget Key
        public virtual bool DeleteContentWidgetByWidgetKey(string widgetKey)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (string.IsNullOrEmpty(widgetKey))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidWidgetKey);

            int widgetId = GetWidgetId(widgetKey);

            return DeleteWidgetByIds(widgetId.ToString());
        }

        //Verify if the Widget Key Exists
        public virtual bool IsWidgetKeyExists(string widgetKey)
        {
            if (string.IsNullOrEmpty(widgetKey))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidWidgetKey);

            return _contentWidgetRepository.Table.Any(x => x.WidgetKey.ToLower() == widgetKey.ToLower());
        }
         

        //Get Content Widget
        public virtual ContentWidgetResponseModel GetContentWidget(string widgetKey)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (string.IsNullOrEmpty(widgetKey))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidWidgetKey);

            ContentWidgetResponseModel contentWidget = GetContentWidgetDetails(widgetKey);
           
            contentWidget.StoreName = string.IsNullOrEmpty(contentWidget.StoreName) ? Admin_Resources.LabelAllStores : (from portalData in _portalRepository.Table.Where(x => x.PortalId == PortalId) select portalData.StoreName).FirstOrDefault();

            contentWidget.WidgetVariants = GetAssociatedVariants(widgetKey);

            contentWidget.IsGlobalContentWidget = HelperUtility.IsNull(contentWidget.PortalId) ? ZnodeConstant.True : ZnodeConstant.False;

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            return contentWidget;
        }


        //Get Associated variants
        public virtual List<AssociatedVariantModel> GetAssociatedVariants(string widgetKey)
        {
            if (string.IsNullOrEmpty(widgetKey))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidWidgetKey);

            int widgetId = GetWidgetId(widgetKey);
            return GetAssociations(widgetId);
        }

        //Delete Variant
        public virtual bool DeleteAssociatedVariant(int variantId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if(variantId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.IdCanNotBeLessThanOne);

            int status = 0;
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("WidgetProfileVariantId", variantId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeleteAssociatedVariants @WidgetProfileVariantId,@Status OUT", 1, out status);

            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(Admin_Resources.DeleteMessage, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, Admin_Resources.ErrorFailedToDelete);
            }

        }

        //Associate Variant
        public virtual List<AssociatedVariantModel>  AssociateVariant(AssociatedVariantModel variant)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (_widgetProfileVariantRepository.Table.Any(x => x.CMSContentWidgetId == variant.ContentWidgetId && x.LocaleId == variant.LocaleId && x.ProfileId == variant.ProfileId))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidData);

            variant.ProfileId = (variant.ProfileId == 0 ||variant.ProfileId  == null ) ? null : variant.ProfileId;
            ZnodeCMSWidgetProfileVariant profileVariant = _widgetProfileVariantRepository.Insert(variant.ToEntity<ZnodeCMSWidgetProfileVariant>());

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            return GetAssociations(profileVariant.CMSContentWidgetId);
        }

        public virtual bool AssociateWidgetTemplate(int variantId, int widgetTemplateId)
        {
            ZnodeCMSWidgetProfileVariant model = _widgetProfileVariantRepository.Table.FirstOrDefault(x => x.CMSWidgetProfileVariantId == variantId);

            if (Equals(model, null))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelCanNotBeNull);

            if (widgetTemplateId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.IdCanNotBeLessThanOne);

            model.CMSWidgetTemplateId = widgetTemplateId;

            bool isUpdated = _widgetProfileVariantRepository.Update(model);

            return isUpdated;

        }

        //Delete widgets
        protected virtual bool DeleteWidgetByIds(string contentWidgetIds)
        {
            int status = 0;
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("ContentWidgetIds", contentWidgetIds, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeleteContentWidget @ContentWidgetIds,@Status OUT", 1, out status);

            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(Admin_Resources.DeleteMessage, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, Admin_Resources.ErrorFailedToDelete);
            }
        }

        #region Private Methods

        //Get Details of Content Widget
        private ContentWidgetResponseModel GetContentWidgetDetails(string widgetKey)
        {
            IZnodeRepository<ZnodeGlobalAttributeFamilyLocale> _globalAttributeFamilyRepository = new ZnodeRepository<ZnodeGlobalAttributeFamilyLocale>();

            ContentWidgetResponseModel contentWidget = (from _contentWidget in _contentWidgetRepository.Table
                                                        join _attributeFamily in _globalAttributeFamilyRepository.Table on _contentWidget.FamilyId equals _attributeFamily.GlobalAttributeFamilyId
                                                        join _portal in _portalRepository.Table on _contentWidget.PortalId equals _portal.PortalId
                                                        into data from _portal in data.DefaultIfEmpty()
                                                        where _contentWidget.WidgetKey.ToLower() == widgetKey.ToLower()
                                                        select new ContentWidgetResponseModel
                                                        {
                                                            Name = _contentWidget.Name,
                                                            WidgetKey = widgetKey,
                                                            PortalId = _contentWidget.PortalId,
                                                            FamilyName = _attributeFamily.AttributeFamilyName,
                                                            StoreName = _portal.StoreName,
                                                            Tags = _contentWidget.Tags,
                                                            ContentWidgetId = _contentWidget.CMSContentWidgetId
                                                        }
                                                     ).FirstOrDefault();
            return contentWidget;

        }

        //Get Associated Variants
        private List<AssociatedVariantModel> GetAssociations(int widgetId)
        {
            IZnodeRepository<ZnodeLocale> _localeRepository = new ZnodeRepository<ZnodeLocale>();

            return (from variantRepository in _widgetProfileVariantRepository.Table
                                                               join contentWidgets in _contentWidgetRepository.Table on variantRepository.CMSContentWidgetId equals contentWidgets.CMSContentWidgetId
                                                               join localeRepository in _localeRepository.Table on variantRepository.LocaleId equals localeRepository.LocaleId
                                                               join profileRepository in _profileRepository.Table on variantRepository.ProfileId equals profileRepository.ProfileId
                                                               into data from profileRepository in data.DefaultIfEmpty()
                                                               where variantRepository.CMSContentWidgetId == widgetId
                                                               select new AssociatedVariantModel
                                                               {
                                                                   LocaleId = variantRepository.LocaleId,
                                                                   ProfileId = variantRepository.ProfileId,
                                                                   ContentWidgetId = variantRepository.CMSContentWidgetId,
                                                                   LocaleName = localeRepository.Name,
                                                                   ProfileName = string.IsNullOrEmpty(profileRepository.ProfileName) ? Admin_Resources.LabelAllProfiles : profileRepository.ProfileName,
                                                                   WidgetKey = contentWidgets.WidgetKey,
                                                                   WidgetProfileVariantId = variantRepository.CMSWidgetProfileVariantId,
                                                                   WidgetTemplateId = variantRepository.CMSWidgetTemplateId
                                                               }).ToList();

        }

        //Get PortalId
        private int? GetPortalId(string storeCode)
        {
            int? portalId = (from portalRepository in _portalRepository.Table.Where(x => x.StoreCode.ToLower() == storeCode.ToLower()) select portalRepository.PortalId).FirstOrDefault();

            if (portalId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidData);

            return portalId;

        }

        //Get Family Id
        private int GetFamilyId(string familyCode)
        {
            IZnodeRepository<ZnodeGlobalAttributeFamily> _globalAttributeFamilyRepository = new ZnodeRepository<ZnodeGlobalAttributeFamily>();
            int familyId = (from globalFamilyRepository in _globalAttributeFamilyRepository.Table.Where(x => x.FamilyCode.ToLower() == familyCode.ToLower()) select globalFamilyRepository.GlobalAttributeFamilyId).FirstOrDefault();

            if (familyId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidFamilyCode);

            return familyId;
        }

        //Get Widget Id
        private int GetWidgetId(string widgetKey)
        {
            int widgetId = (from widgetRepository in _contentWidgetRepository.Table.Where(x => x.WidgetKey.ToLower() == widgetKey.ToLower()) select widgetRepository.CMSContentWidgetId).FirstOrDefault();

            if (widgetId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidWidgetKey);

            return widgetId;

        }
        #endregion


    }
}
