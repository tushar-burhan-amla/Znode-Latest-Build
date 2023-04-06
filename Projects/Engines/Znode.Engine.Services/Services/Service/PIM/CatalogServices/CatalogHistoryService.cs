using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Services
{
    public class CatalogHistoryService : BaseService, ICatalogHistoryService
    {
        #region Private Variables

        #endregion

        public CatalogHistoryService()
        {
            //Initialization of PIM Repositories.
        }

        #region Public Methods

        //Gets the list of catalog history.
        public virtual CatalogHistoryListModel GetCatalogHistoryList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            try
            {
                //set paging parameters.
                int pagingStart = 0;
                int pagingLength = 0;
                int totalCount = 0;
                SetPaging(page, out pagingStart, out pagingLength);

                //gets the order by clause
                string orderBy = DynamicClauseHelper.GenerateDynamicOrderByClause(sorts);

                //gets the where clause with filter Values.              
                EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());

                //Get expands
                var navigationProperties = GetExpands(expands);

                CatalogHistoryListModel listModel = new CatalogHistoryListModel();

                //To Do: Add repository calls.
                // var list = _attributeFamily.GetPagedList(whereClause, orderBy, null, filterValue, pagingStart, pagingLength, out totalCount);


                listModel.TotalResults = totalCount;
                listModel.PageIndex = pagingStart;
                listModel.PageSize = pagingLength;

                return listModel;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                throw new ZnodeException(ErrorCodes.NotFound, ex.Message);
            }
        }

        //Gets catalog history by ID.
        public virtual CatalogHistoryModel GetCatalogHistoryById(int id, NameValueCollection expands)
        {
            try
            {
                if (id < 1)
                    throw new ZnodeException(ErrorCodes.InvalidData, "ID cannot be less than 1.");

                //To Do:
                //Get call for repository and mapper convertion
                return new CatalogHistoryModel();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                throw new ZnodeException(ErrorCodes.NotFound, ex.Message);
            }
        }

        //Updates catalog history.
        public virtual bool UpdateCatalogHistory(CatalogHistoryModel catalogHistoryModel)
        {
            try
            {
                if (Equals(catalogHistoryModel, null))
                    throw new ZnodeException(ErrorCodes.InvalidData, "Model cannot be null.");
                if (catalogHistoryModel.ID < 1)
                    throw new ZnodeException(ErrorCodes.InvalidData, "ID cannot be less than 1.");

                //ToDo :Repository call to update.

                return true;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                throw new ZnodeException(ErrorCodes.NotFound, ex.Message);
            }
        }

        //Creates catalog history.
        public virtual CatalogHistoryModel CreateCatalogHistory(CatalogHistoryModel catalogHistoryModel)
        {
            try
            {
                if (Equals(catalogHistoryModel, null))
                    throw new ZnodeException(ErrorCodes.InvalidData, "Model cannot be null.");

                //To Do:
                //Create new PIM Attribute model.
                //Log created message.

                return new CatalogHistoryModel();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                throw new ZnodeException(ErrorCodes.NotFound, ex.Message);
            }
        }

        //Deletes catalog history.
        public virtual bool DeleteCatalogHistory(int id)
        {
            try
            {
                if (id < 1)
                    throw new ZnodeException(ErrorCodes.InvalidData, "ID cannot be less than 1");

                if (/*TO DO: Repository call to delete*/true)
                    return true;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                throw new ZnodeException(ErrorCodes.NotFound, ex.Message);
            }
        }

        #endregion

        #region Private Methods
        //Get expands and add them to navigation properties
        private List<string> GetExpands(NameValueCollection expands)
        {
            List<string> navigationProperties = new List<string>();
            if (expands != null && expands.HasKeys())
            {
                foreach (string key in expands.Keys)
                {
                    //Add expand keys
                    //if (Equals(key, ExpandKeys.GroupLocale)) SetExpands(ZnodeAttributeGroupEnum.ZnodeAttributeGroupLocales.ToString(), navigationProperties);
                    //if (Equals(key, ExpandKeys.GroupMappers)) SetExpands(ZnodeAttributeGroupEnum.ZnodeAttributeGroupMappers.ToString(), navigationProperties);
                }
            }
            return navigationProperties;
        }
        #endregion
    }
}
