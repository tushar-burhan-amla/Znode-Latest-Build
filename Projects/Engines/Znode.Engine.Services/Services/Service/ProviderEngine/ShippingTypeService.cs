using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Maps;
using Znode.Engine.Shipping;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Services
{
    public class ShippingTypeService : BaseService, IShippingTypeService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeShippingType> _shippingTypeRepository;
        private readonly IZnodeRepository<ZnodeShipping> _shippingRepository;
        private readonly IZnodeRepository<ZnodeShippingServiceCode> _shippingServiceCodeRepository;
        #endregion

        #region Constructor
        public ShippingTypeService()
        {
            _shippingTypeRepository = new ZnodeRepository<ZnodeShippingType>();
            _shippingRepository = new ZnodeRepository<ZnodeShipping>();
            _shippingServiceCodeRepository = new ZnodeRepository<ZnodeShippingServiceCode>();
        }
        #endregion

        #region Public Methods
        public virtual ShippingTypeListModel GetShippingTypeList(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            //Set Paging Parameters
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to GetPagedList :", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString() });

            ShippingTypeListModel shippingTypeListModel = new ShippingTypeListModel();

            IList<ZnodeShippingType> shippingTypeList = _shippingTypeRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("shippingTypeList count :", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new object[] { shippingTypeList.Count });

            foreach (ZnodeShippingType shippingType in shippingTypeList)
                shippingTypeListModel.ShippingTypeList.Add(ShippingTypeMap.ToShippingTypeModel(shippingType));

            shippingTypeListModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            return shippingTypeListModel;
        }

        public virtual ShippingTypeModel GetShippingType(int shippingTypeId) => shippingTypeId > 0 ? ShippingTypeMap.ToShippingTypeModel(_shippingTypeRepository.GetById(shippingTypeId)) : new ShippingTypeModel();

        public virtual ShippingTypeModel CreateShippingType(ShippingTypeModel shippingTypeModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            if (Equals(shippingTypeModel, null))
                throw new ZnodeException(ErrorCodes.NullModel,Admin_Resources.ModelNotNull);
            
            //Create new Shipping Type and return it.
            ZnodeShippingType shippingType = _shippingTypeRepository.Insert(ShippingTypeMap.ToShippingTypeEntity(shippingTypeModel));
            ZnodeLogging.LogMessage("Insert shippingTypeModel with ShippingTypeId :", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new object[] { shippingType?.ShippingTypeId });

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            return ShippingTypeMap.ToShippingTypeModel(shippingType);
        }

        public virtual bool UpdateShippingType(ShippingTypeModel shippingTypeModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            if (Equals(shippingTypeModel, null))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            if (shippingTypeModel.ShippingTypeId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ShippingIdNotLessThanOne);

            ZnodeLogging.LogMessage("shippingTypeModel with ShippingTypeId :", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new object[] { shippingTypeModel?.ShippingTypeId });

            ParameterModel parameterModel = new ParameterModel() { Ids = Convert.ToString(shippingTypeModel.ShippingTypeId) };

            FilterCollection filterList = new FilterCollection();

            filterList.Add(new FilterTuple(ZnodeShippingTypeEnum.ShippingTypeId.ToString(), ProcedureFilterOperators.In, parameterModel.Ids));
            List<ZnodeShippingType> list = _shippingTypeRepository.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filterList.ToFilterDataCollection()).WhereClause).ToList();

            bool isAssociated = CheckShippingAssociation(shippingTypeModel, parameterModel, list);
            return isAssociated ? _shippingTypeRepository.Update(ShippingTypeMap.ToShippingTypeEntity(shippingTypeModel)) : false;
        }

        public virtual bool DeleteShippingType(ParameterModel shippingTypeIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            if (Equals(shippingTypeIds, null) || string.IsNullOrEmpty(shippingTypeIds.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ShippingIdNotLessThanOne);

            ZnodeLogging.LogMessage("shippingTypeIds to be Deleted  :", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new object[] { shippingTypeIds?.Ids });

            List<int> shipingIds = shippingTypeIds.Ids.Split(',')?.Select(x => Convert.ToInt32(x))?.ToList();

            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodeShippingTypeEnum.ShippingTypeId.ToString(), ProcedureFilterOperators.In, shippingTypeIds.Ids));

            string whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection())?.WhereClause;
            ZnodeLogging.LogMessage("whereClause to GetEntityList:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new object[] { whereClause });

            IList<int> shippingList = _shippingRepository.GetEntityList(whereClause)?.Select(x => Convert.ToInt32(x.ShippingTypeId)).Distinct()?.ToList();
            IList<int> shippingServiceList = _shippingServiceCodeRepository.GetEntityList(whereClause)?.Select(x => Convert.ToInt32(x.ShippingTypeId)).Distinct()?.ToList();
            ZnodeLogging.LogMessage("shippingList and shippingServiceList count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new object[] { shippingList?.Count, shippingServiceList?.Count });

            if (shippingList?.Count > 0 || shippingServiceList?.Count > 0)
            {
                List<int> unAssociatedIds = GetUnAssociatedShippingTypeIds(shipingIds, shippingList, shippingServiceList);
                ZnodeLogging.LogMessage("unAssociatedIds returned from GetUnAssociatedShippingTypeIds method: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new object[] { unAssociatedIds });
                DeleteUnAssociatedShippingTypes(unAssociatedIds);
            }

            //if successfully deleted the shipping type then returns true else returns false.
            bool isDeleted = _shippingTypeRepository.Delete(whereClause);
            ZnodeLogging.LogMessage(isDeleted ? Admin_Resources.SuccessShippingTypeDelete : Admin_Resources.ErrorShippingTypeDelete, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return isDeleted;
        }

        public virtual bool EnableDisableShippingType(ParameterModel shippingTypeIds, bool isEnable)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("ParameterModel with Ids to enable or disable ShippingType:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new object[] { shippingTypeIds?.Ids });

            bool isUpdated = false;
            FilterCollection filterList = new FilterCollection();

            filterList.Add(new FilterTuple(ZnodeShippingTypeEnum.ShippingTypeId.ToString(), ProcedureFilterOperators.In, shippingTypeIds.Ids));
            List<ZnodeShippingType> list = _shippingTypeRepository.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filterList.ToFilterDataCollection()).WhereClause).ToList();

            if (!isEnable)
            {
                int[] typeIds = Array.ConvertAll(shippingTypeIds.Ids.Split(','), s => int.Parse(s));
                int count = 0;
                foreach (int id in typeIds)
                {
                    List<int> shippingIds = GetAssociateShippingIds(id);

                    if (list?.Count > 0 && !Equals(shippingIds?.Count, 0))
                    {
                        list?.Remove(list?.Single(s => s.ShippingTypeId == id));
                        count++;
                    }
                }
                isUpdated = UpdateSelectedShippingType(isEnable, list);
                if (count > 0)
                    throw new ZnodeException(ErrorCodes.AssociationDeleteError, Admin_Resources.ShippingTypeCanNotDisableSomeAssociation);
            }
            else
            {
                if (list?.Count > 0)
                    isUpdated = UpdateSelectedShippingType(isEnable, list);

            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            return isUpdated;
        }

        public virtual ShippingTypeListModel GetAllShippingTypesNotInDatabase()
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            ShippingTypeListModel shippingTypeList = new ShippingTypeListModel();

            List<IZnodeShippingsType> shippingTypes = ZnodeShippingManager.GetAvailableShippingTypes();
            ZnodeLogging.LogMessage("shippingTypes count :", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new object[] { shippingTypes?.Count});

            ShippingTypeListModel availableShippingTypes = GetShippingTypeList(new FilterCollection(), new NameValueCollection(), new NameValueCollection());
            if (availableShippingTypes?.ShippingTypeList?.Count < shippingTypes?.Count)
            {
                foreach (IZnodeShippingsType shipping in shippingTypes)
                {
                    bool found = false;
                    foreach (ShippingTypeModel shippingType in availableShippingTypes.ShippingTypeList)
                    {
                        if (!found && string.Equals(shipping.ClassName, shippingType.ClassName))
                                found = true;
                    }
                    if (!found)
                        shippingTypeList.ShippingTypeList.Add(ShippingTypeMap.ToModel(shipping));
                }
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            return shippingTypeList;
        }
        #endregion

        //Get unassociated shipping type ids.
        private List<int> GetUnAssociatedShippingTypeIds(List<int> shippingTypeList, IList<int> shippingList, IList<int> shippingServiceList)
        {
            if (shippingTypeList.Count > 0 || shippingServiceList.Count > 0)
            {
                foreach (var item in shippingServiceList)
                    shippingTypeList.Remove(item);

                return shippingTypeList.Except(shippingList).ToList();
            }
            return null;
        }

        //Delete unassociated shipping types
        private void DeleteUnAssociatedShippingTypes(List<int> unAssociatedIds)
        {
            ZnodeLogging.LogMessage("unAssociatedIds to be Deleted  :", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new object[] { unAssociatedIds });

            if (unAssociatedIds.Count > 0)
            {
                FilterCollection filter = new FilterCollection();
                filter.Add(new FilterTuple(ZnodeShippingTypeEnum.ShippingTypeId.ToString(), ProcedureFilterOperators.In, string.Join(",", unAssociatedIds)));
                _shippingTypeRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection())?.WhereClause);
            }

            //If Shipping type ids has any association then throw association delete exception.
            throw new ZnodeException(ErrorCodes.AssociationDeleteError, Admin_Resources.ShippingTypeCanNotDeleteSomeAssociation);
        }

        // update selected shipping type
        private bool UpdateSelectedShippingType(bool isEnable, List<ZnodeShippingType> list)
        {
            bool isUpdated = false;
            foreach (ZnodeShippingType shippingType in list)
            {
                if ((shippingType.IsActive && isEnable) || (!shippingType.IsActive && !isEnable))
                    isUpdated = true;
                else
                {
                    shippingType.IsActive = isEnable;
                    isUpdated = _shippingTypeRepository.Update(shippingType);
                }
            }
            return isUpdated;
        }

        private bool CheckShippingAssociation(ShippingTypeModel shippingTypeModel, ParameterModel parameterModel, List<ZnodeShippingType> list)
        {
            if (!shippingTypeModel.IsActive)
            {
                int[] typeIds = Array.ConvertAll(parameterModel.Ids.Split(','), s => int.Parse(s));
                int count = 0;
                foreach (int id in typeIds)
                {
                    List<int> shippingIds = GetAssociateShippingIds(id);

                    if (list?.Count > 0 && shippingIds?.Count > 0)
                    {
                        list?.Remove(list?.Single(s => s.ShippingTypeId == id));
                        count++;
                    }
                }
                if (count > 0)
                    throw new ZnodeException(ErrorCodes.AssociationDeleteError, Admin_Resources.ShippingTypeCanNotDisableSomeAssociation);
            }
            return true;
        }

        private List<int> GetAssociateShippingIds(int id) => _shippingRepository.Table.Where(w => w.ShippingTypeId == id && w.IsActive).Select(s => s.ShippingId).ToList();

    }
}
