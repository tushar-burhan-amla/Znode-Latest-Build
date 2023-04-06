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
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Services
{
    public class ThemeService : BaseService, IThemeService
    {
        #region Private Variable
        private readonly IZnodeRepository<ZnodeCMSTheme> _themeRepository;
        private readonly IZnodeRepository<ZnodeCMSPortalTheme> _cmsPortalThemeRepository;
        private readonly IZnodeRepository<ZnodeCMSArea> _cmsAreaRepository;
        private readonly IZnodeRepository<ZnodeCMSThemeCSS> _cmsThemeCSSRepository;
        #endregion

        #region Constructor
        public ThemeService()
        {
            _themeRepository = new ZnodeRepository<ZnodeCMSTheme>();
            _cmsPortalThemeRepository = new ZnodeRepository<ZnodeCMSPortalTheme>();
            _cmsAreaRepository = new ZnodeRepository<ZnodeCMSArea>();
            _cmsThemeCSSRepository = new ZnodeRepository<ZnodeCMSThemeCSS>();
        }
        #endregion

        #region Public Methods
        #region Theme Configuration

        //Get theme list.
        public virtual ThemeListModel GetThemes(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            //Set paging parameters.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel generated to get theme list: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            //Get orderBy Clause
            string orderBy = DynamicClauseHelper.GenerateDynamicOrderByClause(sorts);
            //gets the where clause with filter Values.              
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("orderBy Clause and WhereClause generated to get theme list: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { orderBy, whereClauseModel.WhereClause });

            ThemeListModel listModel = new ThemeListModel();
            listModel.Themes = _themeRepository.GetPagedList(whereClauseModel.WhereClause, orderBy, GetExpandsForParentTheme(), whereClauseModel.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount)?.ToModel<ThemeModel>().ToList();
            ZnodeLogging.LogMessage("Theme list count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, listModel?.Themes?.Count);

            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Create new Theme.
        public virtual ThemeModel CreateTheme(ThemeModel themeModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            //check if model is null.
            if (Equals(themeModel, null))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelCanNotBeNull);
            ZnodeLogging.LogMessage("Name and CMSThemeId properties of themeModel: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[]{themeModel?.Name, themeModel?.CMSThemeId});
            //check if theme is already exists.
            if (IsThemeAlreadyExists(themeModel.Name))
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.ErrorThemeAlreadyExist);

            ZnodeCMSTheme themeEntity = _themeRepository.Insert(themeModel.ToEntity<ZnodeCMSTheme>());
            themeModel.CMSThemeId = Convert.ToInt32(themeEntity?.CMSThemeId);

            if (themeEntity?.CMSThemeId > 0 && themeModel.CssList?.Count > 0)
            {
                themeModel.CssList.ForEach(x => x.CMSThemeId = themeEntity.CMSThemeId);
                IList<ZnodeCMSThemeCSS> themeCSSEntity = _cmsThemeCSSRepository.Insert(themeModel.CssList.ToEntity<ZnodeCMSThemeCSS>().ToList()).ToList();
                themeModel = themeEntity.ToModel<ThemeModel>();
                themeModel.CssList = themeCSSEntity?.ToModel<CSSModel>()?.ToList();
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return themeModel;
        }

        //Get details of theme by theme Id.
        public virtual ThemeModel GetTheme(int themeId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter themeId: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, themeId);

            //check if theme Id is less than 1.
            if (themeId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorThemeIdLessThanOne);

            FilterCollection filter = new FilterCollection() { new FilterTuple(ZnodeCMSThemeEnum.CMSThemeId.ToString(), FilterOperators.Equals, themeId.ToString()) };
            ThemeModel themeModel = _themeRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause, GetExpandsForParentTheme())?.ToModel<ThemeModel>();
            if(IsNotNull(themeModel))
                themeModel.CssList = _cmsThemeCSSRepository.Table.Where(x => x.CMSThemeId == themeId).ToModel<CSSModel>().ToList();

            ZnodeLogging.LogMessage("CssList count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, themeModel?.CssList?.Count);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return themeModel;
        }

        //Update theme.
        public virtual bool UpdateTheme(ThemeModel themeModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (IsNull(themeModel))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            if (themeModel.CMSThemeId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.IdCanNotBeLessThanOne);
            ZnodeLogging.LogMessage("Name and CMSThemeId properties of themeModel: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { themeModel?.Name, themeModel?.CMSThemeId });
            ZnodeLogging.LogMessage("CssList count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, themeModel?.CssList?.Count);

            //Update theme
            bool isThemeUpdated = _themeRepository.Update(themeModel.ToEntity<ZnodeCMSTheme>());
            if (themeModel?.CMSThemeId > 0 && themeModel.CssList?.Count > 0)
            {
                themeModel.CssList.ForEach(x => x.CMSThemeId = themeModel.CMSThemeId);
                IList<ZnodeCMSThemeCSS> themeCSSEntity = _cmsThemeCSSRepository.Insert(themeModel.CssList.ToEntity<ZnodeCMSThemeCSS>().ToList()).ToList();
                themeModel.CssList = themeCSSEntity?.ToModel<CSSModel>()?.ToList();
            }

            ZnodeLogging.LogMessage(isThemeUpdated ? Admin_Resources.SuccessThemeUpdated : Admin_Resources.ErrorThemeUpdate, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return isThemeUpdated;
        }

        //Delete the theme.
        public virtual bool DeleteTheme(ParameterModel themeId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (string.IsNullOrEmpty(themeId?.Ids))
                throw new ZnodeException(ErrorCodes.NotFound, Admin_Resources.ErrorCSSIdLessThanOne);
            ZnodeLogging.LogMessage("Input parameter themeId to delete: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, themeId?.Ids);

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter(ZnodeCMSThemeEnum.CMSThemeId.ToString(), themeId.Ids, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            int status = 0;
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeleteCMSTheme @CMSThemeId, @Status OUT", 1, out status);
            ZnodeLogging.LogMessage("deleteResult count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, deleteResult?.Count);

            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(Admin_Resources.SuccessThemeDeleted, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorThemeAssociatedToPortal, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, Admin_Resources.ErrorThemeAssociatedToPortal);
            }
        }

        #endregion

        #region Associate Store

        //Get associated store list.
        public virtual PricePortalListModel GetAssociatedStoreList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            //set paging parameters.
            int pagingStart = 0;
            int pagingLength = 0;
            int totalCount = 0;
            SetPaging(page, out pagingStart, out pagingLength);

            //gets the order by clause
            string orderBy = DynamicClauseHelper.GenerateDynamicOrderByClause(sorts);

            filters.Add(new FilterTuple("IsAssociated ", ProcedureFilterOperators.Equals, "1"));

            //gets the where clause with filter Values.              
            string whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseForSP(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("orderBy Clause and WhereClause generated to set SP parameters get associated store list: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { orderBy, whereClauseModel });
            IZnodeViewRepository<View_GetAssociatedCMSThemeToPortal> objStoredProc = new ZnodeViewRepository<View_GetAssociatedCMSThemeToPortal>();

            //SP parameters
            objStoredProc.SetParameter("@WhereClause", whereClauseModel, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", orderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", totalCount, ParameterDirection.Output, DbType.Int32);

            IList<View_GetAssociatedCMSThemeToPortal> associatedStoreEntity = objStoredProc.ExecuteStoredProcedureList("Znode_GetAssociatedCMSThemeToPortal  @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out totalCount);
            ZnodeLogging.LogMessage("associatedStoreEntity list count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, associatedStoreEntity?.Count);

            PricePortalListModel listModel = new PricePortalListModel { PricePortalList = associatedStoreEntity?.ToModel<PricePortalModel>().ToList() };
            listModel.TotalResults = totalCount;
            listModel.PageIndex = pagingStart;
            listModel.PageSize = pagingLength;
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Get list of unassociated store list.
        public virtual PortalListModel GetUnAssociatedStoreList(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            //set paging parameters.
            int pagingStart = 0;
            int pagingLength = 0;
            int totalCount = 0;
            SetPaging(page, out pagingStart, out pagingLength);

            //Generate the order by clause
            string orderBy = DynamicClauseHelper.GenerateDynamicOrderByClause(sorts);

            filters.Add(new FilterTuple(FilterKeys.IsAssociated, ProcedureFilterOperators.Equals, "0"));
            filters.Add(new FilterTuple(FilterKeys.IsAssociatedToPortal, ProcedureFilterOperators.Equals, "0"));

            //Gets the where clause.              
            string whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseForSP(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("orderBy Clause and WhereClause generated to set SP parameters get unassociated store list: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { orderBy, whereClauseModel });
            IZnodeViewRepository<View_GetAssociatedCMSThemeToPortal> objStoredProc = new ZnodeViewRepository<View_GetAssociatedCMSThemeToPortal>();

            //SP parameters
            objStoredProc.SetParameter("@WhereClause", whereClauseModel, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", orderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", totalCount, ParameterDirection.Output, DbType.Int32);

            IList<View_GetAssociatedCMSThemeToPortal> unAssociatedStoreEntity = objStoredProc.ExecuteStoredProcedureList("Znode_GetAssociatedCMSThemeToPortal  @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out totalCount);
            ZnodeLogging.LogMessage("unAssociatedStoreEntity list count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, unAssociatedStoreEntity?.Count);

            PortalListModel listModel = new PortalListModel() { PortalList = unAssociatedStoreEntity?.ToModel<PortalModel>().ToList() };
            listModel.TotalResults = totalCount;
            listModel.PageIndex = pagingStart;
            listModel.PageSize = pagingLength;
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Associate store.
        public virtual bool AssociateStore(PricePortalListModel pricePortalModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (IsNull(pricePortalModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelCanNotBeNull);

            if (pricePortalModel?.PricePortalList?.Count < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorCMSThemePortalModelCountLessThanOne);
            ZnodeLogging.LogMessage("PricePortalList count of input pricePortalModel: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, pricePortalModel?.PricePortalList?.Count);

            IEnumerable<ZnodeCMSPortalTheme> associateStore = _cmsPortalThemeRepository.Insert(pricePortalModel.PricePortalList.ToEntity<ZnodeCMSPortalTheme>().ToList());

            return associateStore?.Count() > 0;
        }


        //Remove Associated Stores.
        public virtual bool RemoveAssociatedStores(ParameterModel cmsPortalThemeId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (Equals(cmsPortalThemeId, null) || string.IsNullOrEmpty(cmsPortalThemeId?.Ids))
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorCMSPortalThemeIdLessThanOne);

            ZnodeLogging.LogMessage("Input parameter cmsPortalThemeId: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, cmsPortalThemeId?.Ids);

            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodeCMSPortalThemeEnum.CMSPortalThemeId.ToString(), ProcedureFilterOperators.In, cmsPortalThemeId.Ids));

            return _cmsPortalThemeRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause);
        }
        #endregion

        #region CMS Widgets

        //Get the list of all cms areas.
        public virtual CMSAreaListModel GetAreas(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            //Set Paging Parameters
            int pagingStart = 0;
            int pagingLength = 0;
            int totalCount = 0;
            SetPaging(page, out pagingStart, out pagingLength);

            //Get orderBy Clause
            string orderBy = DynamicClauseHelper.GenerateDynamicOrderByClause(sorts);

            filters.Add(ZnodeCMSAreaEnum.IsWidgetArea.ToString(), ProcedureFilterOperators.Equals, "true");

            //gets the where clause with filter Values.              
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("orderBy Clause and WhereClause generated to list of all cms areas: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { orderBy, whereClauseModel?.WhereClause });

            CMSAreaListModel listModel = new CMSAreaListModel();
            listModel.CMSAreas = _cmsAreaRepository.GetPagedList(whereClauseModel.WhereClause, orderBy, pagingStart, pagingLength, out totalCount)?.ToModel<CMSAreaModel>().ToList();
            ZnodeLogging.LogMessage("CMSAreas list count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, listModel?.CMSAreas);
            listModel.PageIndex = pagingStart;
            listModel.PageSize = pagingLength;
            listModel.TotalResults = totalCount;
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return listModel;
        }

        #endregion

        #endregion

        #region Private Method
        //Check whether the theme name already exists or not.
        private bool IsThemeAlreadyExists(string themeName)
            => _themeRepository.Table?.Where(x => x.Name.Trim() == themeName.Trim())?.Count() > 0;

        //Get expands and add them to navigation properties
        private List<string> GetExpandsForParentTheme()
        {
            List<string> navigationProperties = new List<string>();
            SetExpands(ZnodeCMSThemeEnum.ZnodeCMSTheme2.ToString(), navigationProperties);
            return navigationProperties;
        }
        #endregion

    }
}
