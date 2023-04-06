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

namespace Znode.Engine.Services
{
    public class WeightUnitService : BaseService, IWeightUnitService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeWeightUnit> _weightUnitRepository;
        #endregion

        #region Constructor
        public WeightUnitService()
        {
            _weightUnitRepository = new ZnodeRepository<ZnodeWeightUnit>();
        }
        #endregion

        #region Public Methods

        //Get a list of weightunits
        public virtual WeightUnitListModel GetWeightUnits(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to generate weightUnit list ", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            WeightUnitListModel weightUnitList = new WeightUnitListModel
            {
                WeightUnits = _weightUnitRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount)
                .ToModel<WeightUnitModel>()?.ToList()
            };
            weightUnitList.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            return weightUnitList;
        }

        //Update Weightunits
        public virtual bool UpdateWeightUnit(WeightUnitModel weightUnitModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter WeightUnitModel WeightUnitId:", ZnodeLogging.Components.GlobalSettings.ToString(),TraceLevel.Verbose, weightUnitModel.WeightUnitId);
            if (Equals(weightUnitModel, null))
                throw new ZnodeException(ErrorCodes.NullModel,Admin_Resources.WeightUnitModelNotNull);
            try
            {
                FilterTuple filter = new FilterTuple(ZnodeWeightUnitEnum.WeightUnitId.ToString(), ProcedureFilterOperators.Equals, weightUnitModel.WeightUnitId.ToString());
                FilterCollection filtersList = new FilterCollection();
                filtersList.Add(filter);

                //gets the where clause with filter Values.              
                EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filtersList.ToFilterDataCollection());
                ZnodeLogging.LogMessage("whereClauseModel generated", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, whereClauseModel);
                ZnodeWeightUnit weightUnit = _weightUnitRepository.GetEntity(whereClauseModel.WhereClause);

                if (!weightUnit.IsDefault)
                {   //Get list of entity from table.
                    List<ZnodeWeightUnit> defaultConfigurationList = _weightUnitRepository.Table.Where(x => x.IsDefault).ToList();
                    defaultConfigurationList.ForEach(x => x.IsDefault = false);
                    //Set IsDefault equal to true for the entity to update
                    weightUnit.IsDefault = true;
                    defaultConfigurationList.Add(weightUnit);

                    //Update List Of Entity To Database
                    defaultConfigurationList.ForEach(x => _weightUnitRepository.Update(x));
                    ZnodeLogging.LogMessage("defaultConfiguration list count:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, defaultConfigurationList?.Count());
                    return true;
                }
                else
                    return true;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorUpdateWeightUnit, ZnodeLogging.Components.GlobalSettings.ToString(),TraceLevel.Error,ex);
                throw new ZnodeException(ErrorCodes.NotFound, ex.Message);
            }
        }

        #endregion
    }
}
