using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Promotions;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Services
{
    public class PromotionTypeService : BaseService, IPromotionTypeService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodePromotionType> _promotionTypeRepository;
        private readonly IZnodeRepository<ZnodePromotion> _promotionRepository;
        #endregion

        #region Constructor
        public PromotionTypeService()
        {
            _promotionRepository = new ZnodeRepository<ZnodePromotion>();
            _promotionTypeRepository = new ZnodeRepository<ZnodePromotionType>();
        }
        #endregion

        #region Public Methods
        public virtual PromotionTypeListModel GetPromotionTypeList(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            //Set Paging Parameters
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to generate currency list ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            PromotionTypeListModel promotionTypeListModel = new PromotionTypeListModel();

            IList<ZnodePromotionType> promotionTypeList = _promotionTypeRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("promotionTypeList list count:", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, promotionTypeList?.Count());
            foreach (ZnodePromotionType promotionType in promotionTypeList)
                promotionTypeListModel.PromotionTypes.Add(PromotionTypeMap.ToPromotionTypeModel(promotionType));

            promotionTypeListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            return promotionTypeListModel;
        }

        public virtual PromotionTypeModel GetPromotionType(int promotionTypeId) => promotionTypeId > 0 ? PromotionTypeMap.ToPromotionTypeModel(_promotionTypeRepository.Table.FirstOrDefault(x => x.PromotionTypeId == promotionTypeId)) : new PromotionTypeModel();

        public virtual PromotionTypeModel CreatePromotionType(PromotionTypeModel promotionTypeModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            if (Equals(promotionTypeModel, null))
                throw new ZnodeException(ErrorCodes.NullModel,Admin_Resources.ModelNotNull);

            //Create new promotion type and return it.
            ZnodePromotionType promotionType = _promotionTypeRepository.Insert(PromotionTypeMap.ToPromotionTypeEntity(promotionTypeModel));
            ZnodeLogging.LogMessage("Inserted PromotionType with id :", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, promotionTypeModel?.PromotionTypeId);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            return PromotionTypeMap.ToPromotionTypeModel(promotionType);
        }

        public virtual bool UpdatePromotionType(PromotionTypeModel promotionTypeModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            if (Equals(promotionTypeModel, null))
                throw new ZnodeException(ErrorCodes.NullModel,Admin_Resources.ModelNotNull);

            if (promotionTypeModel.PromotionTypeId < 1)
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.PromotionTypeIdNotLessThanOne);

            //Check if any promotion is associated to given promotion type.
            if (CheckActivePromotion(promotionTypeModel.PromotionTypeId, promotionTypeModel.IsActive))
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, Admin_Resources.PromotionTypeCanNotDisableSomeAssociation);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            //if successfully update the promotion then returns true else returns false.
            return _promotionTypeRepository.Update(PromotionTypeMap.ToPromotionTypeEntity(promotionTypeModel));
        }

        public virtual bool DeletePromotionType(ParameterModel promotionTypeIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            if (Equals(promotionTypeIds, null) || string.IsNullOrEmpty(promotionTypeIds.Ids))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.PromotionTypeIdNotLessThanOne);

            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodePromotionTypeEnum.PromotionTypeId.ToString(), ProcedureFilterOperators.In, promotionTypeIds.Ids));

            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection());
            ZnodeLogging.LogMessage("whereClauseModel to generate promotions list ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, whereClauseModel);
            //Get the list of promotions to which selected promotion types associated.
            List<ZnodePromotion> promotions = _promotionRepository.GetEntityList(whereClauseModel.WhereClause)?.ToList();
            ZnodeLogging.LogMessage("promotions list count:", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, promotions?.Count());
            //Get unAssociated promootion type ids and delete if some promotion types associated to promotions.
            if (promotions?.Count > 0)
            {
                List<int> unAssociatedIds = GetUnAssociatedPromotionTypeIds(promotions, promotionTypeIds);
                ZnodeLogging.LogMessage("unAssociatedIds list count:", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, unAssociatedIds?.Count());
                DeleteUnAssociatedPromotionTypes(unAssociatedIds);
            }

            //if successfully deleted the promotion then returns true else returns false.
            bool isDeleted = _promotionTypeRepository.Delete(whereClauseModel.WhereClause);
            ZnodeLogging.LogMessage(isDeleted ? Admin_Resources.SuccessPromotionDelete : Admin_Resources.ErrorPromotionDelete, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return isDeleted;
        }

        public virtual bool EnableDisablePromotionType(ParameterModel promotionTypeIds, bool isEnable)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            bool isUpdated = false;
            FilterCollection filterList = new FilterCollection();
            filterList.Add(new FilterTuple(ZnodePromotionTypeEnum.PromotionTypeId.ToString(), ProcedureFilterOperators.In, promotionTypeIds.Ids));
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filterList.ToFilterDataCollection());
            ZnodeLogging.LogMessage("whereClauseModel to generate promotions list ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, whereClauseModel);
            List<ZnodePromotion> promotions = _promotionRepository.GetEntityList(whereClauseModel.WhereClause)?.ToList();
            ZnodeLogging.LogMessage("promotions list count:", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, promotions?.Count());
            //Get unAssociated promootion type ids and disable if some promotion types associated to promotions.
            if (promotions?.Count > 0 && !isEnable)
            {
                List<int> unAssociatedIds = GetUnAssociatedPromotionTypeIds(promotions, promotionTypeIds);
                ZnodeLogging.LogMessage("unAssociatedIds list count:", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, unAssociatedIds?.Count());
                DisableUnAssociatedPromotionTypes(unAssociatedIds, isEnable);
            }

            //Get the list of selected promotion types.
            List<ZnodePromotionType> list = _promotionTypeRepository.GetEntityList(whereClauseModel.WhereClause).ToList();
            ZnodeLogging.LogMessage("ZnodePromotionType list count:", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, list?.Count());
            //If selected promotion type list has records then update them.
            if (list?.Count > 0)
            {
                foreach (ZnodePromotionType promotionType in list)
                {
                    if ((promotionType.IsActive && isEnable) || (!promotionType.IsActive && !isEnable))
                        isUpdated = true;
                    else
                    {
                        promotionType.IsActive = isEnable;
                        isUpdated = _promotionTypeRepository.Update(promotionType);
                    }
                }
            }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            return isUpdated;
        }

        public virtual PromotionTypeListModel GetAllPromotionTypesNotInDatabase()
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            PromotionTypeListModel promotionTypeModel = new PromotionTypeListModel();

            List<IZnodePromotionsType> promotionTypes = ZnodePromotionManager.GetAvailablePromotionTypes();
            ZnodeLogging.LogMessage("promotionTypes list count:", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, promotionTypes?.Count());
            PromotionTypeListModel availablePromotionTypes = GetPromotionTypeList(new FilterCollection(), new NameValueCollection(), new NameValueCollection());

            if (availablePromotionTypes?.PromotionTypes?.Count < promotionTypes?.Count)
            {
                foreach (IZnodePromotionsType promotion in promotionTypes)
                {
                    bool found = false;
                    foreach (PromotionTypeModel promotionType in availablePromotionTypes.PromotionTypes)

                    { 
                    if (!found && string.Equals(promotion.ClassName, promotionType.ClassName))
                        found = true;
                    }
                    if (!found)
                        promotionTypeModel.PromotionTypes.Add(PromotionTypeMap.ToModel(promotion));
                }
            }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            return promotionTypeModel;
        }
        #endregion

        #region Private Methods
        //Get unassociated promotion type ids.
        private List<int> GetUnAssociatedPromotionTypeIds(List<ZnodePromotion> promotins, ParameterModel promotionTypeIds)
        {
            List<int> promoitonList = promotins?.Where(x => x.EndDate > HelperUtility.GetDateTime()).Select(x => Convert.ToInt32(x.PromotionTypeId)).Distinct()?.ToList();
            ZnodeLogging.LogMessage("promoitonList list count:", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, promoitonList?.Count());
            List<int> promotionTypeList = promotionTypeIds.Ids.Split(',')?.Select(x => Convert.ToInt32(x))?.ToList();
            ZnodeLogging.LogMessage("promotionTypeList list count:", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, promotionTypeList?.Count());
            return promotionTypeList.Except(promoitonList).ToList();
        }

        //Check if any promotion associated to promotionType.
        private bool CheckActivePromotion(int promotionTypeId, bool isEnable)
        {

            ZnodeLogging.LogMessage("Input Parameter promotionTypeId and isEnable:", ZnodeLogging.Components.Marketing.ToString(),TraceLevel.Verbose, new object[] { promotionTypeId, isEnable });
            if (promotionTypeId > 0 && !isEnable)
            {
                //Filter of Id to get Promotion list.
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodePromotionTypeEnum.PromotionTypeId.ToString(), FilterOperators.Equals, promotionTypeId.ToString()));

                List<ZnodePromotion> promotionList = _promotionRepository.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection())?.WhereClause)?.ToList();
                ZnodeLogging.LogMessage("promotionList list count:", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, promotionList?.Count());
                //Check if any promotion is associated to given promotion type.
                if (promotionList?.Count > 0)
                    return promotionList.TrueForAll(x => x.EndDate > HelperUtility.GetDateTime());
            }
            return false;
        }

        //Delete unassociated promotion types
        private void DeleteUnAssociatedPromotionTypes(List<int> unAssociatedIds)
        {
            //if un-associated promotion type ids has record then delete.
            if (unAssociatedIds.Count > 0)
            {
                FilterCollection filter = new FilterCollection();
                filter.Add(new FilterTuple(ZnodePromotionTypeEnum.PromotionTypeId.ToString(), ProcedureFilterOperators.In, string.Join(",", unAssociatedIds)));
                _promotionTypeRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection())?.WhereClause);
            }

            //If promotion type ids has any association then throw association delete exception.
            throw new ZnodeException(ErrorCodes.AssociationDeleteError,Admin_Resources.ErrorPromotionTypeDelete);
        }

        //Delete unassociated promotion types
        private void DisableUnAssociatedPromotionTypes(List<int> unAssociatedIds, bool isDisable)
        {

            ZnodeLogging.LogMessage("Input Parameter unAssociatedIds and isDisable", ZnodeLogging.Components.Marketing.ToString(),TraceLevel.Verbose, new object[] { unAssociatedIds, isDisable });
            //if un-associated promotion type ids has record then disable.
            if (unAssociatedIds.Count > 0)
            {
                FilterCollection filter = new FilterCollection();
                filter.Add(new FilterTuple(ZnodePromotionTypeEnum.PromotionTypeId.ToString(), ProcedureFilterOperators.In, string.Join(",", unAssociatedIds)));
                EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection());
                List<ZnodePromotionType> list = _promotionTypeRepository.GetEntityList(whereClauseModel.WhereClause).ToList();
                ZnodeLogging.LogMessage("ZnodePromotionType list count:", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, list?.Count());
                foreach (ZnodePromotionType promotionType in list)
                {
                    promotionType.IsActive = isDisable;
                    _promotionTypeRepository.Update(promotionType);
                }
            }

            //If promotion type ids has any association then throw association disable exception.
            throw new ZnodeException(ErrorCodes.AssociationDeleteError, Admin_Resources.ErrorPromotionTypeDelete);
        }
        #endregion
    }
}
