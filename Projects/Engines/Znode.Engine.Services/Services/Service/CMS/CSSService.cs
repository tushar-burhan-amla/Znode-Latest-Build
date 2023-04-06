using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public class CSSService : BaseService, ICSSService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeCMSThemeCSS> _znodeCMSThemeCSSRepository;
        private readonly IZnodeRepository<ZnodeCMSPortalTheme> _znodeCMSPortalThemeRepository;
        #endregion

        #region Constructor
        public CSSService()
        {
            _znodeCMSThemeCSSRepository = new ZnodeRepository<ZnodeCMSThemeCSS>();
            _znodeCMSPortalThemeRepository = new ZnodeRepository<ZnodeCMSPortalTheme>();
        }
        #endregion

        public virtual CSSListModel GetCssListByThemeId(int cmsThemeId, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            CSSNameFilter(filters);

            //Set Paging Parameters
            int pagingStart = 0;
            int pagingLength = 0;
            int totalCount = 0;
            SetPaging(page, out pagingStart, out pagingLength);

            //Get Sort Clause
            string orderBy = DynamicClauseHelper.GenerateDynamicOrderByClause(sorts);
            ZnodeLogging.LogMessage("Input parameter cmsThemeId value: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, cmsThemeId);

            //Add cmsThemeId to filter.
            filters.Add(new FilterTuple(ZnodeCMSThemeEnum.CMSThemeId.ToString(), FilterOperators.Equals, cmsThemeId.ToString()));

            //gets the where clause with filter Values.              
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("orderBy clause and whereClauseModel generated to get cssListEntity: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { orderBy, whereClauseModel.WhereClause });
            CSSListModel listModel = new CSSListModel();
            List<CSSModel> cssListEntity = _znodeCMSThemeCSSRepository.GetPagedList(whereClauseModel.WhereClause, orderBy, whereClauseModel.FilterValues, pagingStart, pagingLength, out totalCount)?.ToModel<CSSModel>()?.ToList();
            ZnodeLogging.LogMessage("cssListEntity list count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, cssListEntity?.Count());

            //Add .css extension to the CSS Name.          
            if (cssListEntity?.Count > 0)
                cssListEntity?.ForEach(x => x.CSSName = $"{x.CSSName}.css");

            //Get the name of theme.
            IThemeService themeService = GetService<IThemeService>();
            listModel.CMSThemeName = themeService.GetTheme(cmsThemeId)?.Name;

            listModel.CSSs = cssListEntity;
            listModel.PageIndex = pagingStart;
            listModel.PageSize = pagingLength;
            listModel.TotalResults = totalCount;
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return listModel;
        }

        public virtual CSSModel CreateCSS(CSSModel cssModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            bool isCssAlreadyExist = false;
            //Check if model is null.
            if (Equals(cssModel, null))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelCanNotBeNull);

            //Insert new css details.
            foreach (string cssName in cssModel.cssList)
            {
                //Check if css is already exists.
                if (IsCSSAlreadyExists(cssName, cssModel.CMSThemeId))
                    isCssAlreadyExist = true;
                else
                {
                    cssModel.CSSName = cssName;
                    cssModel = _znodeCMSThemeCSSRepository.Insert(cssModel.ToEntity<ZnodeCMSThemeCSS>())?.ToModel<CSSModel>();
                    ZnodeLogging.LogMessage("Inserted css details with CMSThemeId: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, cssModel?.CMSThemeId);
                }
            }

            if (isCssAlreadyExist)
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.ErrorCSSAlreadyExists);

            ZnodeLogging.LogMessage(cssModel?.CMSThemeCSSId > 0 ? Admin_Resources.SuccessCSSInserted : Admin_Resources.ErrorCSSInsert, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return cssModel;
        }

        public virtual bool DeleteCSS(ParameterModel cssIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (string.IsNullOrEmpty(cssIds?.Ids))
                throw new ZnodeException(ErrorCodes.NotFound, Admin_Resources.ErrorCSSIdLessThanOne);
            ZnodeLogging.LogMessage("Input parameter cssIds value to delete CSS: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, cssIds);

            //Create filter of css ids.
            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodeCMSThemeCSSEnum.CMSThemeCSSId.ToString(), ProcedureFilterOperators.In, cssIds.Ids));

            List<int> associatedCSSIds = _znodeCMSPortalThemeRepository.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause).Select(x => Convert.ToInt32(x.CMSThemeCSSId)).Distinct()?.ToList();
            ZnodeLogging.LogMessage("associatedCSSIds list count: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, associatedCSSIds?.Count());

            if (associatedCSSIds?.Count > 0)
            {
                //Get unassociated CSS ids to delete.
                List<int> unAssociatedIds = cssIds.Ids.Split(',').Select(int.Parse).AsEnumerable().Except(associatedCSSIds).ToList();
                ZnodeLogging.LogMessage("unAssociatedIds list count: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, unAssociatedIds?.Count());

                if (unAssociatedIds.Count > 0)
                    DeleteUnAssociatedCSSIds(unAssociatedIds);

                //If CSS ids are associated to store throws association delete exception.
                ZnodeLogging.LogMessage(Admin_Resources.ErrorCSSDeleteBecauseAssociatedToStore, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, Admin_Resources.ErrorCSSDeleteBecauseAssociatedToStore);
            }

            //Delete CSS ids which are not associated to store.
            bool isDeleted = _znodeCMSThemeCSSRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause);
            ZnodeLogging.LogMessage(isDeleted ? Admin_Resources.SuccessCSSDeleted : Admin_Resources.ErrorWhileDeletingCSS, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return isDeleted;
        }

        //Check whether the css of same name is already exists or not.
        private bool IsCSSAlreadyExists(string cssName, int cmsThemeId)
            => _znodeCMSThemeCSSRepository.Table?.Where(x => x.CSSName == cssName && x.CMSThemeId == cmsThemeId)?.Count() > 0;

        //Delete unassociated CSS ids to store.
        private void DeleteUnAssociatedCSSIds(List<int> unAssociatedIds)
        {
            FilterCollection newFilter = new FilterCollection();
            newFilter.Add(new FilterTuple(ZnodeCMSThemeCSSEnum.CMSThemeCSSId.ToString(), ProcedureFilterOperators.In, string.Join(",", unAssociatedIds)));

            //Delete unassociated CSS ids to store. 
            _znodeCMSThemeCSSRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(newFilter.ToFilterDataCollection()).WhereClause);
        }

        //Create filters for CSS Name.
        private void CSSNameFilter(FilterCollection filters)
        {
            if (filters?.Count > 0)
            {
                FilterTuple newFilter = filters.Find(x => x.FilterName == FilterKeys.CSSName);
                if (HelperUtility.IsNotNull(newFilter))
                {
                    filters.RemoveAll(x => x.FilterName == FilterKeys.CSSName);
                    string cssName = newFilter.FilterValue.Split('.').FirstOrDefault();
                    if (!string.IsNullOrEmpty(cssName))
                        filters.Add(newFilter.FilterName, newFilter.FilterOperator, cssName);
                }
            }
        }
    }
}
