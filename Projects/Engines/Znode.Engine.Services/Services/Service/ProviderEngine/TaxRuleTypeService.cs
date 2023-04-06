using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Maps;
using Znode.Engine.Taxes;
using Znode.Engine.Taxes.Interfaces;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Services
{
    public class TaxRuleTypeService : BaseService, ITaxRuleTypeService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeTaxRuleType> _taxRuleTypeRepository;
        private readonly IZnodeRepository<ZnodeTaxRule> _taxRuleRepository;
        private readonly IZnodeRepository<ZnodeTaxClass> _taxClassRepository;
        #endregion

        #region Constructor
        public TaxRuleTypeService()
        {
            _taxRuleTypeRepository = new ZnodeRepository<ZnodeTaxRuleType>();
            _taxRuleRepository = new ZnodeRepository<ZnodeTaxRule>();
            _taxClassRepository = new ZnodeRepository<ZnodeTaxClass>();
        }
        #endregion

        #region Public Methods
        public virtual TaxRuleTypeListModel GetTaxRuleTypeList(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            filters.RemoveAll(x => x.Item1 == ZnodeTaxClassEnum.TaxClassId.ToString().ToLower());

            //Set Paging Parameters
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel for GetPagedList method", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            TaxRuleTypeListModel taxRuleTypeListModel = new TaxRuleTypeListModel();

            IList<ZnodeTaxRuleType> taxRuleTypeList = _taxRuleTypeRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("taxRuleTypeList count ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, taxRuleTypeList.Count);

            foreach (ZnodeTaxRuleType taxRuleType in taxRuleTypeList)
                taxRuleTypeListModel.TaxRuleTypes.Add(TaxRuleTypeMap.ToTaxRuleTypeModel(taxRuleType));

            taxRuleTypeListModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return taxRuleTypeListModel;
        }

        public virtual TaxRuleTypeModel GetTaxRuleType(int taxRuleTypeId) => taxRuleTypeId > 0 ? TaxRuleTypeMap.ToTaxRuleTypeModel(_taxRuleTypeRepository.GetById(taxRuleTypeId)) : new TaxRuleTypeModel();

        public virtual TaxRuleTypeModel CreateTaxRuleType(TaxRuleTypeModel taxRuleTypeModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            if (Equals(taxRuleTypeModel, null))
                throw new ZnodeException(ErrorCodes.NullModel,Admin_Resources.ErrorModelNull);

            ////Create new tax class and return it.
            ZnodeTaxRuleType taxRuleType = _taxRuleTypeRepository.Insert(TaxRuleTypeMap.ToTaxRuleTypeEntity(taxRuleTypeModel));
            ZnodeLogging.LogMessage("Inserted TaxRuleTypeMap with TaxRuleTypeId  ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, taxRuleType?.TaxRuleTypeId);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return TaxRuleTypeMap.ToTaxRuleTypeModel(taxRuleType);
        }

        public virtual bool UpdateTaxRuleType(TaxRuleTypeModel taxRuleTypeModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            if (Equals(taxRuleTypeModel, null))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorModelNull);

            if (taxRuleTypeModel.TaxRuleTypeId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.TaxClassIdNotLessThanOne);

            ZnodeLogging.LogMessage("taxRuleTypeModel with TaxRuleTypeId  ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, taxRuleTypeModel?.TaxRuleTypeId);

            ParameterModel parameterModel = new ParameterModel() { Ids = Convert.ToString(taxRuleTypeModel.TaxRuleTypeId) };

            FilterCollection filterList = new FilterCollection();

            filterList.Add(new FilterTuple(ZnodeTaxRuleTypeEnum.TaxRuleTypeId.ToString(), ProcedureFilterOperators.In, parameterModel.Ids));
            List<ZnodeTaxRuleType> list = _taxRuleTypeRepository.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filterList.ToFilterDataCollection()).WhereClause).ToList();
            ZnodeLogging.LogMessage("list count ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, list.Count);

            return CheckTaxRuleAssociation(taxRuleTypeModel.IsActive, parameterModel, list) ? _taxRuleTypeRepository.Update(TaxRuleTypeMap.ToTaxRuleTypeEntity(taxRuleTypeModel)) : false;
        }

        public virtual bool DeleteTaxRuleType(ParameterModel entityIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("entityIds to be deleted.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, entityIds?.Ids);

            if (Equals(entityIds, null) || string.IsNullOrEmpty(entityIds.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.TaxClassIdNotNull);

            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodeTaxRuleTypeEnum.TaxRuleTypeId.ToString(), ProcedureFilterOperators.In, entityIds?.Ids));

            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection());
            ZnodeLogging.LogMessage("whereClauseModel for Delete ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, whereClauseModel);

            if (_taxRuleRepository.GetEntityList(whereClauseModel.WhereClause)?.Count > 0)
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, Admin_Resources.TaxRuleTypeIsAssociated);

            //if successfully deleted the tax then returns true else returns false.
            return _taxRuleTypeRepository.Delete(whereClauseModel.WhereClause);
        }

        public virtual bool EnableDisableTaxRuleType(ParameterModel entityIds, bool isEnable)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter ParameterModel with entityIds ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, entityIds?.Ids);

            bool isUpdated = false;
            FilterCollection filterList = new FilterCollection();

            filterList.Add(new FilterTuple(ZnodeTaxRuleTypeEnum.TaxRuleTypeId.ToString(), ProcedureFilterOperators.In, entityIds.Ids));
            List<ZnodeTaxRuleType> list = _taxRuleTypeRepository.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filterList.ToFilterDataCollection()).WhereClause).ToList();
            ZnodeLogging.LogMessage("list count ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, list?.Count);

            if (!isEnable)
            {
                isUpdated = CheckTaxRuleAssociation(false, entityIds, list, isEnable);
            }
            else
            {
                if (list?.Count > 0)
                    isUpdated = UpdateSelectedTaxType(isEnable, list);

            }
            return isUpdated;
        }

        public virtual TaxRuleTypeListModel GetAllTaxRuleTypesNotInDatabase()
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            TaxRuleTypeListModel taxRuleTypeList = new TaxRuleTypeListModel();

            // Gets all available tax types from the application cache.
            List<IZnodeTaxesType> taxRuleTypes = ZnodeTaxManager.GetAvailableTaxTypes();
            ZnodeLogging.LogMessage("Gets all available tax types count ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, taxRuleTypes?.Count);

            // Gets all available tax types from the database table.
            TaxRuleTypeListModel availableTaxRuleTypes = GetTaxRuleTypeList(new FilterCollection(), new NameValueCollection(), new NameValueCollection());
            if (availableTaxRuleTypes?.TaxRuleTypes?.Count < taxRuleTypes?.Count)
            {
                foreach (IZnodeTaxesType taxRuleType in taxRuleTypes)
                {
                    bool found = false;
                    foreach (TaxRuleTypeModel taxType in availableTaxRuleTypes.TaxRuleTypes)
                    { 
                    if (!found && string.Equals(taxRuleType.ClassName, taxType.ClassName))
                        found = true;
                    }
                    if (!found)
                        taxRuleTypeList.TaxRuleTypes.Add(TaxRuleTypeMap.ToModel(taxRuleType));
                }
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            return taxRuleTypeList;
        }


        #endregion

        #region Private Methods

        // Update tax type.
        private bool UpdateSelectedTaxType(bool isEnable, List<ZnodeTaxRuleType> list)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            bool isUpdated = false;
            foreach (ZnodeTaxRuleType taxRuleType in list)
            {
                if ((taxRuleType.IsActive && isEnable) || (!taxRuleType.IsActive && !isEnable))
                    isUpdated = true;
                else
                {
                    taxRuleType.IsActive = isEnable;
                    isUpdated = _taxRuleTypeRepository.Update(taxRuleType);
                }
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            return isUpdated;
        }

        // Check tax association, throw exception if its have any.
        private bool CheckTaxRuleAssociation(bool isActive, ParameterModel parameterModel, List<ZnodeTaxRuleType> list, bool isMultipleUpdate = false, bool isEnable = false)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            bool isUpdated = false;
            if (!isActive)
            {
                int[] typeIds = Array.ConvertAll(parameterModel.Ids.Split(','), s => int.Parse(s));
                int count = 0;
                foreach (int id in typeIds)
                {
                    List<int> taxTypeIds = GetTaxRuleTypeIdAssociate(id);
                    if (list?.Count > 0 && taxTypeIds?.Count > 0)
                    {
                        list?.Remove(list?.Single(s => s.TaxRuleTypeId == id));
                        count++;
                    }
                }
                if (!isEnable && Equals(count, 0))
                {
                    isUpdated = UpdateSelectedTaxType(isActive, list);
                }
                if (count > 0)
                    throw new ZnodeException(ErrorCodes.AssociationDeleteError,Admin_Resources.TaxCanNotDisabled);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            return isMultipleUpdate ? isUpdated : true;
        }

        // Get associate tax rule ids.
        private List<int> GetTaxRuleTypeIdAssociate(int id)
        {
            return (from taxRuleType in _taxRuleTypeRepository.Table
                    join taxRule in _taxRuleRepository.Table on taxRuleType.TaxRuleTypeId equals taxRule.TaxRuleTypeId
                    join taxClass in _taxClassRepository.Table on taxRule.TaxClassId equals taxClass.TaxClassId

                    where taxRuleType.TaxRuleTypeId == id && taxClass.IsActive

                    select taxRuleType.TaxRuleTypeId).ToList();
        }
        #endregion
    }
}
