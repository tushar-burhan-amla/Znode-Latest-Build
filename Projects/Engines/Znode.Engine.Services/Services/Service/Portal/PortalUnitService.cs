using System;
using System.Collections.Generic;
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
    public class PortalUnitService : BaseService, IPortalUnitService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodePortalUnit> _znodePortalUnitRepository;
        #endregion

        #region Constructor
        public PortalUnitService()
        {
            _znodePortalUnitRepository = new ZnodeRepository<ZnodePortalUnit>();
        }
        #endregion

        #region Public Methods
        public virtual PortalUnitModel GetPortalUnit(int portalId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                if (portalId > 0)
                {
                    //checks if the filter collection null
                    FilterCollection filter = new FilterCollection();
                    filter.Add(new FilterTuple(ZnodePortalEnum.PortalId.ToString(), ProcedureFilterOperators.Equals, portalId.ToString()));

                    PortalUnitModel portalUnitModel = _znodePortalUnitRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClause(filter.ToFilterDataCollection()))?.ToModel<PortalUnitModel>();
                    if (HelperUtility.IsNotNull(portalUnitModel))
                    {
                        ZnodeRepository<ZnodePortal> _portalRepository = new ZnodeRepository<ZnodePortal>();

                        //Bind store name.
                        portalUnitModel.PortalName = _portalRepository.Table.FirstOrDefault(x => x.PortalId == portalId)?.StoreName;
                        return portalUnitModel;
                    }

                }
                return null;
        }


        public virtual bool CreateUpdatePortalUnit(PortalUnitModel portalUnitModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("PortalUnitModel portalUnitModel: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, portalUnitModel);
                if (HelperUtility.IsNull(portalUnitModel))
                    throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorModelNull);

                if (portalUnitModel.PortalId > 0 && portalUnitModel.PortalUnitId > 0)
                {
                    //Check portal unit is updated or not. 
                    if (_znodePortalUnitRepository.Update(portalUnitModel?.ToEntity<ZnodePortalUnit>()))
                    {
                        DeletePriceList(portalUnitModel);
                        return true;
                    }
                }
                else
                {
                    if (_znodePortalUnitRepository.Insert(portalUnitModel?.ToEntity<ZnodePortalUnit>())?.PortalUnitId > 0)
                        return true;
                }
                return false;
            }
        #endregion

        #region Private Method
        //Delete Price List from ZnodePriceListPortal having old currency.
        private void DeletePriceList(PortalUnitModel portalUnitModel)
        {
            IZnodeRepository<ZnodePriceList> _priceListRepository = new ZnodeRepository<ZnodePriceList>();
            IZnodeRepository<ZnodePriceListPortal> _priceListPortalRepository = new ZnodeRepository<ZnodePriceListPortal>();

            if (!Equals(portalUnitModel?.OldCurrencyId, portalUnitModel?.CurrencyTypeID) || !Equals(portalUnitModel?.OldCultureId, portalUnitModel?.CultureId))
            {
                //If portal unit is updated with new currency get priceListPortalIds to delete record from ZnodePriceListPortal having old currency.
                IEnumerable<int> idsToDelete = (from priceList in _priceListRepository.Table
                                                join priceListPortal in _priceListPortalRepository.Table on priceList.PriceListId equals priceListPortal.PriceListId
                                                where priceList.CurrencyId == portalUnitModel.OldCurrencyId && priceListPortal.PortalId == portalUnitModel.PortalId
                                                select priceListPortal.PriceListPortalId)?.AsEnumerable().Distinct();
                ZnodeLogging.LogMessage("IdsToDelete:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info, idsToDelete);
                //If idsToDelete count is greater than zero record is deleted. 
                if (idsToDelete.Any())
                {
                    FilterCollection filter = new FilterCollection();
                    filter.Add(ZnodePriceListPortalEnum.PriceListPortalId.ToString(), FilterOperators.In, string.Join(",", idsToDelete));
                    _priceListPortalRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClause(filter.ToFilterDataCollection()));
                    ZnodeLogging.LogMessage("Deleted PriceList with id: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, idsToDelete);
                }
            }
        }
        #endregion
    }
}
