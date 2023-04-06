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
    public class CategoryHistoryService : BaseService, ICategoryHistoryService
    {
        #region Private Variables

        #endregion

        public CategoryHistoryService()
        {
            //Initialization of PIM Repositories.
        }

        #region Public Methods

        //Gets the list of category history.
        public virtual CategoryHistoryListModel GetCategoryHistoryList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
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
                EntityWhereClauseModel whereClauseModel =  DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());

                //Get expands
                var navigationProperties = GetExpands(expands);

                CategoryHistoryListModel listModel = new CategoryHistoryListModel();

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

        //Gets category history by ID.
        public virtual CategoryHistoryModel GetCategoryHistoryById(int id, NameValueCollection expands)
        {
            try
            {
                if (id < 1)
                    throw new ZnodeException(ErrorCodes.InvalidData, "ID cannot be less than 1.");

                //To Do:
                //Get call for repository and mapper convertion
                return new CategoryHistoryModel();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                throw new ZnodeException(ErrorCodes.NotFound, ex.Message);
            }
        }

        //Updates category history.
        public virtual bool UpdateCategoryHistory(CategoryHistoryModel categoryHistoryModel)
        {
            try
            {
                if (Equals(categoryHistoryModel, null))
                    throw new ZnodeException(ErrorCodes.InvalidData, "Model cannot be null.");
                if (categoryHistoryModel.ID < 1)
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

        //Creates category history.
        public virtual CategoryHistoryModel CreateCategoryHistory(CategoryHistoryModel categoryHistoryModel)
        {
            try
            {
                if (Equals(categoryHistoryModel, null))
                    throw new ZnodeException(ErrorCodes.InvalidData, "Model cannot be null.");

                //To Do:
                //Create new PIM Attribute model.
                //Log created message.

                return new CategoryHistoryModel();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                throw new ZnodeException(ErrorCodes.NotFound, ex.Message);
            }
        }

        //Deletes category history.
        public virtual bool DeleteCategoryHistory(int id)
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
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, ex.Message);
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
