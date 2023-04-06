using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Entities;

namespace Znode.Engine.Shipping.Helper
{
    public class ZnodeShippingHelper : IZnodeShippingHelper 
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeShippingRule> _shippingRuleRepository;
        private readonly IZnodeRepository<Libraries.Data.DataModel.ZnodeShipping> _shippingRepository;
        private readonly IZnodeRepository<ZnodeCountry> _countryRepository;
        private readonly IZnodeRepository<ZnodeShippingPortal> _shippingPortalRepository;
        private readonly IZnodeRepository<ZnodePortalUnit> _portalUnitRepository;
        private readonly IZnodeRepository<ZnodeShippingType> _shippingTypeRepository;
        private readonly IZnodeRepository<ZnodeWarehouse> _wareHouseRepository;
        private readonly IZnodeRepository<ZnodeWarehouseAddress> _wareHouseAddressRepository;
        private readonly IZnodeRepository<ZnodePortalWarehouse> _portalWareHouseRepository;
        private readonly IZnodeRepository<ZnodeAddress> _addressRepository;
        private readonly IZnodeRepository<ZnodeState> _stateRepository;
        private readonly IZnodeRepository<ZnodePortalShipping> _portalShippingRepository;
        private readonly IZnodeRepository<ZnodePortal> _portalRepository;
        private readonly IZnodeRepository<ZnodePortalCatalog> _portalCatalogRepository;
        #endregion

        #region Constructor
        public ZnodeShippingHelper()
        {
            _shippingRuleRepository = new ZnodeRepository<ZnodeShippingRule>();
            _shippingRepository = new ZnodeRepository<Libraries.Data.DataModel.ZnodeShipping>();
            _countryRepository = new ZnodeRepository<ZnodeCountry>();
            _shippingPortalRepository = new ZnodeRepository<ZnodeShippingPortal>();
            _portalUnitRepository = new ZnodeRepository<ZnodePortalUnit>();
            _shippingTypeRepository = new ZnodeRepository<ZnodeShippingType>();
            _wareHouseAddressRepository = new ZnodeRepository<ZnodeWarehouseAddress>();
            _wareHouseRepository = new ZnodeRepository<ZnodeWarehouse>();
            _portalWareHouseRepository = new ZnodeRepository<ZnodePortalWarehouse>();
            _addressRepository = new ZnodeRepository<ZnodeAddress>();
            _stateRepository = new ZnodeRepository<ZnodeState>();
            _portalShippingRepository = new ZnodeRepository<ZnodePortalShipping>();
            _portalRepository = new ZnodeRepository<ZnodePortal>();
            _portalCatalogRepository = new ZnodeRepository<ZnodePortalCatalog>();
        }
        #endregion

        // Get shipping rule list on basis on rulename and country code.        
        public virtual List<ShippingRuleModel> GetShippingRuleList(string ruleName, string countryCode, int portalId = 0, int shippingId = 0)
        {
            IZnodeViewRepository<ShippingRuleModel> objStoredProcedure = new ZnodeViewRepository<ShippingRuleModel>();

            objStoredProcedure.SetParameter("@ShippingRuleTypeCode", ruleName, ParameterDirection.Input, DbType.String);
            objStoredProcedure.SetParameter("@CountryCode", countryCode, ParameterDirection.Input, DbType.String);
            objStoredProcedure.SetParameter("@ShippingId", shippingId, ParameterDirection.Input, DbType.Int32);
            objStoredProcedure.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);

            List<ShippingRuleModel> shippingRuleModel = objStoredProcedure.ExecuteStoredProcedureList("Znode_GetShippingRuleDetails @ShippingRuleTypeCode,@CountryCode,@ShippingId,@PortalId")?.ToList();

            return shippingRuleModel ?? new List<ShippingRuleModel>();
        }

        // Get order level shipping handling charge.        
        public virtual decimal GetOrderLevelShippingHandlingCharge(ZnodeShippingBag shippingBag, ZnodeShoppingCart shoppingCart)
        {
            decimal orderLevelShipping = 0.0m;

            switch ((ZnodeShippingHandlingChargesBasedON)Enum.Parse(typeof(ZnodeShippingHandlingChargesBasedON), shippingBag.HandlingBasedOn))
            {
                case ZnodeShippingHandlingChargesBasedON.SubTotal:
                    orderLevelShipping += shippingBag.CalculateShippingHandlingChargeInPercentage(shippingBag.SubTotal, shippingBag.HandlingCharge);
                    break;
                case ZnodeShippingHandlingChargesBasedON.Shipping:
                    orderLevelShipping += shippingBag.CalculateShippingHandlingChargeInPercentage(shoppingCart.OrderLevelShipping, shippingBag.HandlingCharge);
                    break;
                case ZnodeShippingHandlingChargesBasedON.Amount:
                    orderLevelShipping += shippingBag.HandlingCharge;
                    break;
                default:
                    break;
            }

            return orderLevelShipping;
        }

        // Get Item handling charge.       
        public virtual decimal GetItemHandlingCharge(decimal itemShippingCost, ZnodeShippingBag shippingBag)
        {
            decimal shippingCost = 0.0m;

            switch ((ZnodeShippingHandlingChargesBasedON)Enum.Parse(typeof(ZnodeShippingHandlingChargesBasedON), shippingBag.HandlingBasedOn))
            {
                case ZnodeShippingHandlingChargesBasedON.SubTotal:
                    shippingCost = itemShippingCost + shippingBag.CalculateShippingHandlingChargeInPercentage(shippingBag.SubTotal, shippingBag.HandlingCharge);
                    break;
                case ZnodeShippingHandlingChargesBasedON.Shipping:
                    shippingCost = itemShippingCost + shippingBag.CalculateShippingHandlingChargeInPercentage(itemShippingCost, shippingBag.HandlingCharge);
                    break;
                case ZnodeShippingHandlingChargesBasedON.Amount:
                    shippingCost = itemShippingCost + shippingBag.HandlingCharge;
                    break;
                default:
                    break;
            }

            return shippingCost;
        }

        // Get portal shipping on basis on portal id.        
        public virtual PortalShippingModel GetPortalShipping(int portalId, int publishStateId)
        {
            if (portalId > 0)
            {
                PortalShippingModel portalShippingModel = (from shippingPortal in _shippingPortalRepository.Table
                                                           join portalUnit in _portalUnitRepository.Table on shippingPortal.PortalId equals portalUnit.PortalId

                                                           where shippingPortal.PortalId == portalId && shippingPortal.PublishStateId == publishStateId

                                                           select new PortalShippingModel()
                                                           {
                                                               FedExAccountNumber = shippingPortal.FedExAccountNumber,
                                                               FedExLTLAccountNumber = shippingPortal.FedExLTLAccountNumber,
                                                               FedExMeterNumber = shippingPortal.FedExMeterNumber,
                                                               FedExProductionKey = shippingPortal.FedExProductionKey,
                                                               FedExSecurityCode = shippingPortal.FedExSecurityCode,
                                                               FedExDropoffType = shippingPortal.FedExDropoffType,
                                                               FedExPackagingType = shippingPortal.FedExPackagingType,
                                                               FedExUseDiscountRate = shippingPortal.FedExUseDiscountRate,
                                                               FedExAddInsurance = shippingPortal.FedExAddInsurance,
                                                               portalUnitModel = new PortalUnitModel() { WeightUnit = portalUnit.WeightUnit.ToUpper(), DimensionUnit = portalUnit.DimensionUnit },
                                                               PortalId = shippingPortal.PortalId,
                                                               UpsUsername = shippingPortal.UPSUserName,
                                                               UpsKey = shippingPortal.UPSKey,
                                                               UpsPassword = shippingPortal.UPSPassword,
                                                               LTLUpsAccessLicenseNumber = shippingPortal.LTLUPSAccessLicenseNumber,
                                                               LTLUpsAccountNumber = shippingPortal.LTLUPSAccountNumber,
                                                               LTLUpsPassword = shippingPortal.LTLUPSPassword,
                                                               LTLUpsUserName = shippingPortal.LTLUPSUsername,
                                                               USPSShippingAPIURL = shippingPortal.USPSShippingAPIURL,
                                                               USPSWebToolsUserID = shippingPortal.USPSWebToolsUserID,
                                                               PackageWeightLimit = shippingPortal.PackageWeightLimit,
                                                               UPSDropoffType = shippingPortal.UPSDropoffType,
                                                               UPSPackagingType = shippingPortal.UPSPackagingType
                                                           }).FirstOrDefault();

                return portalShippingModel ?? new PortalShippingModel();

            }
            return new PortalShippingModel();
        }

        // Get item ship together handling charge.        
        public virtual decimal GetShipTogetherItemsHandlingCharge(ZnodeShippingBag shippingBag, decimal itemShippingRate)
        {
            // Add handling charge for ship-together package
            switch ((ZnodeShippingHandlingChargesBasedON)Enum.Parse(typeof(ZnodeShippingHandlingChargesBasedON), shippingBag.HandlingBasedOn))
            {
                case ZnodeShippingHandlingChargesBasedON.SubTotal:
                    itemShippingRate += shippingBag.CalculateShippingHandlingChargeInPercentage(shippingBag.SubTotal, shippingBag.HandlingCharge);
                    break;
                case ZnodeShippingHandlingChargesBasedON.Shipping:
                    itemShippingRate += shippingBag.CalculateShippingHandlingChargeInPercentage(itemShippingRate, shippingBag.HandlingCharge);
                    break;
                case ZnodeShippingHandlingChargesBasedON.Amount:
                    itemShippingRate += shippingBag.HandlingCharge;
                    break;
                default:
                    break;
            }

            return itemShippingRate;
        }

        // Get item ship separately handling charge       
        public virtual decimal GetShipSeparatelyItemsHandlingCharge(ZnodeShippingBag shippingBag, ZnodeShoppingCartItem separateItem, decimal itemShippingRate)
        {
            switch ((ZnodeShippingHandlingChargesBasedON)Enum.Parse(typeof(ZnodeShippingHandlingChargesBasedON), shippingBag.HandlingBasedOn))
            {
                case ZnodeShippingHandlingChargesBasedON.SubTotal:
                    itemShippingRate += (shippingBag.CalculateShippingHandlingChargeInPercentage(shippingBag.SubTotal, shippingBag.HandlingCharge) * separateItem.Quantity);
                    break;
                case ZnodeShippingHandlingChargesBasedON.Shipping:
                    itemShippingRate += (shippingBag.CalculateShippingHandlingChargeInPercentage(itemShippingRate, shippingBag.HandlingCharge) * separateItem.Quantity);
                    break;
                case ZnodeShippingHandlingChargesBasedON.Amount:
                    itemShippingRate += shippingBag.HandlingCharge * separateItem.Quantity;
                    break;
                default:
                    break;
            }

            return itemShippingRate;
        }


        // Get string value of enum.
        public virtual string GetShippingRuleTypesEnumValue(string enumString)
            => ((DescriptionAttribute)typeof(ZnodeShippingRuleTypes).GetMember(enumString)[0].GetCustomAttributes(typeof(DescriptionAttribute), false)[0]).Description;

        //
        public virtual string GetUPSLTLServiceCodeEnumValue(string enumString)
           => ((DescriptionAttribute)typeof(UPSLTLServiceCode).GetMember(enumString)[0].GetCustomAttributes(typeof(DescriptionAttribute), false)[0]).Description;

        // Get ware house default address on the basis of portal id.
        public virtual AddressModel GetPortalWareHouseAddress(int portalId)
        {
            AddressModel wareHouseAddress = (from portalWareHouse in _portalWareHouseRepository.Table
                                             join warehouse in _wareHouseRepository.Table on portalWareHouse.WarehouseId equals warehouse.WarehouseId
                                             join portalwareHouseAddress in _wareHouseAddressRepository.Table on warehouse.WarehouseId equals portalwareHouseAddress.WarehouseId
                                             join address in _addressRepository.Table on portalwareHouseAddress.AddressId equals address.AddressId

                                             where portalWareHouse.PortalId == portalId

                                             select new AddressModel
                                             {
                                                 AddressId = address.AddressId,
                                                 FirstName = address.FirstName,
                                                 LastName = address.LastName,
                                                 DisplayName = address.DisplayName,
                                                 Address1 = address.Address1,
                                                 Address2 = address.Address2,
                                                 Address3 = address.Address3,
                                                 CountryName = address.CountryName,
                                                 StateName = address.StateName,
                                                 CityName = address.CityName,
                                                 PostalCode = address.PostalCode,
                                                 PhoneNumber = address.PhoneNumber,
                                                 Mobilenumber = address.Mobilenumber,
                                                 AlternateMobileNumber = address.AlternateMobileNumber,
                                                 IsDefaultBilling = address.IsDefaultBilling,
                                                 IsDefaultShipping = address.IsDefaultShipping,
                                                 WarehouseName = warehouse.WarehouseName

                                             }
                                             ).FirstOrDefault();

            return wareHouseAddress ?? new AddressModel();
        }

        // Get shipping origin address on the basis of portal id.
        public virtual AddressModel GetPortalShippingOriginAddress(int portalId)
        {
            AddressModel shippingOriginAddress = (from shippingportal in _shippingPortalRepository.Table
                                                  where shippingportal.PortalId == portalId
                                                  select new AddressModel()
                                                  {
                                                      Address1 = shippingportal.ShippingOriginAddress1,
                                                      Address2 = shippingportal.ShippingOriginAddress2,
                                                      CityName = shippingportal.ShippingOriginCity,
                                                      StateCode = shippingportal.ShippingOriginStateCode,
                                                      PostalCode = shippingportal.ShippingOriginZipCode,
                                                      PhoneNumber = shippingportal.ShippingOriginPhone,
                                                      CountryName = shippingportal.ShippingOriginCountryCode,
                                                      IsUseWareHouseAddress = shippingportal.IsUseWarehouseAddress
                                                  }).FirstOrDefault();
            return shippingOriginAddress ?? new AddressModel();
        }

        //Get portal shipping address.
        public virtual AddressModel GetPortalShippingAddress(int portalId)
        {
            AddressModel portalShippingAddress = new AddressModel();
            AddressModel address = GetPortalShippingOriginAddress(portalId);
            portalShippingAddress = Convert.ToBoolean(address.IsUseWareHouseAddress)
                                            ? GetPortalWareHouseAddress(portalId) : address;

            return portalShippingAddress;
        }

        //Get country name by country code.
        public virtual string GetCountryByCountryCode(string countryCode)
            => _countryRepository.Table.FirstOrDefault(x => x.CountryCode == countryCode)?.CountryName;

        // Get state code by stateName
        public virtual string GetStateCode(string stateName)
        {
            stateName = !string.IsNullOrEmpty(stateName) ? stateName.Trim() : null;
            if ((stateName?.Length > 3))
            {
                //to state code by stateName
                string stateCode = Convert.ToString((from state in _stateRepository.Table
                                                     where state.StateName == stateName
                                                     select state.StateCode).FirstOrDefault());
                return stateCode ?? string.Empty;
            }
            return stateName ?? string.Empty;
        }

        // Set error message for shipping.
        public virtual ZnodeShoppingCart SetShippingErrorMessage(ZnodeShoppingCart shoppingCart)
        {
            shoppingCart.Shipping.ResponseCode = "-1";
            shoppingCart.Shipping.IsValidShippingSetting = false;
            shoppingCart.AddErrorMessage = "Unable to calculate shipping rates at this time, please try again later.";
            return shoppingCart;
        }

        // Get shipping rule list.
        public virtual List<ShippingRuleModel> GetPortalProfileShippingRuleList(string shippingCountryCode, int shippingId, int? portalId = 0, int? profileId = 0, int? userId = 0)
        {
            IZnodeViewRepository<ShippingRuleModel> objStoredProc = new ZnodeViewRepository<ShippingRuleModel>();

            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@ProfileId", Convert.ToInt32(profileId), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@ShippingId", shippingId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@CountyCode", shippingCountryCode, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@UserId", Convert.ToInt32(userId) > 0 ? userId : -1, ParameterDirection.Input, DbType.Int32);

            IList<ShippingRuleModel> shippingRuleModel = objStoredProc.ExecuteStoredProcedureList("Znode_GetShippingRuleList @PortalId, 0,@UserId, @ShippingId,@CountyCode");
            return shippingRuleModel?.ToList() ?? new List<ShippingRuleModel>();
        }

        // Get shipping details.
        public virtual ShippingModel GetPortalProfileShipping(string shippingCountryCode, int shippingID, int? portalId = 0, int? profileId = 0, int? userId = 0)
        {
            int count = 0;

            IZnodeViewRepository<ShippingModel> objStoredProc = new ZnodeViewRepository<ShippingModel>();

            objStoredProc.SetParameter("@WhereClause", "(DestinationCountryCode = '" + shippingCountryCode + "'  OR DestinationCountryCode IS NULL)  AND ShippingId = " + shippingID + "", ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", "5", ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", "1", ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", "", ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", "", ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@ProfileId", Convert.ToInt32(profileId), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@UserId", Convert.ToInt32(userId) > 0 ? userId : -1, ParameterDirection.Input, DbType.Int32);
            List<ShippingModel> shippinglist = objStoredProc.ExecuteStoredProcedureList("Znode_GetShippingList @WhereClause, @Rows,@PageNo,@Order_By,@RowCount OUT, 0,@PortalId,@UserId", 4, out count)?.ToList();

            return shippinglist?.FirstOrDefault() ?? new ShippingModel();
        }

        public virtual PortalModel GetPortal(int portalId)      
        {
            IZnodeRepository<ZnodePimCatalog> _pimCatalogEntityRepository = new ZnodeRepository<ZnodePimCatalog>();

            return (from portalCatalog in _portalCatalogRepository.Table
                    join pimCatalog in _pimCatalogEntityRepository.Table on portalCatalog.PublishCatalogId equals pimCatalog.PimCatalogId
                    where portalCatalog.PortalId == portalId
                    select new PortalModel { PortalId = portalId, CatalogName = pimCatalog.CatalogName, PublishCatalogId = pimCatalog.PimCatalogId })?.FirstOrDefault();
        }
             

        public virtual DateTime GetPickUpDate()
        {
            DateTime pickupDate = DateTime.Now;

            if ((Equals(DateTime.Now.DayOfWeek, DayOfWeek.Friday)) || Equals(DateTime.Now.DayOfWeek, DayOfWeek.Saturday) || Equals(DateTime.Now.DayOfWeek, DayOfWeek.Sunday))
            {
                pickupDate = GetUPSPickUpDate(pickupDate);
            }

            return pickupDate;
        }

        public virtual DateTime GetUPSPickUpDate(DateTime pickupDate)
        {
            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Friday:
                    pickupDate = pickupDate.AddDays(3);
                    break;
                case DayOfWeek.Saturday:
                    pickupDate = pickupDate.AddDays(2);
                    break;
                case DayOfWeek.Sunday:
                    pickupDate = pickupDate.AddDays(1);
                    break;
                default:
                    pickupDate = pickupDate.AddDays(1);
                    break;
            }

            return pickupDate;
        }
    }
}
