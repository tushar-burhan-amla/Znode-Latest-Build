using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Taxes.Helper
{
    public class ZnodeTaxHelper : IZnodeTaxHelper
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodePortalFeature> _znodePortalFeature;
        private readonly IZnodeRepository<ZnodePortalFeatureMapper> _znodePortalFeatureMapper;
        private readonly IZnodeRepository<ZnodeTaxClass> _znodeTaxClass;
        private readonly IZnodeRepository<ZnodeTaxRule> _znodeTaxRule;
        private readonly IZnodeRepository<ZnodeCity> _znodeCity;
        private readonly IZnodeRepository<ZnodePortalTaxClass> _znodeTaxClassPortal;
        private readonly IZnodeRepository<ZnodeTaxRuleType> _znodeTaxRuleType;
        private readonly IZnodeRepository<ZnodeState> _stateRepository;
        private readonly IZnodeRepository<ZnodePortalWarehouse> _portalWareHouseRepository;
        private readonly IZnodeRepository<ZnodeShippingPortal> _shippingPortalRepository;
        private readonly IZnodeRepository<ZnodeWarehouse> _wareHouseRepository;
        private readonly IZnodeRepository<ZnodeWarehouseAddress> _wareHouseAddressRepository;
        private readonly IZnodeRepository<ZnodeAddress> _addressRepository;
        private readonly IZnodeRepository<ZnodeTaxPortal> _taxPortalRepository;
        private readonly IZnodeRepository<ZnodeOmsTaxOrderLineDetail> _omsTaxOrderLineDetails;
        private readonly IZnodeRepository<ZnodeUser> _userRepository;
        private readonly IZnodeRepository<ZnodeOmsOrderLineItem> _omsOrderLineItems;
        private readonly IZnodeRepository<ZnodeOmsOrderDetail> _omsOrderDetails;

        #endregion

        #region Constructor
        public ZnodeTaxHelper()
        {
            _znodePortalFeature = new ZnodeRepository<ZnodePortalFeature>();
            _znodePortalFeatureMapper = new ZnodeRepository<ZnodePortalFeatureMapper>();
            _znodeTaxClass = new ZnodeRepository<ZnodeTaxClass>();
            _znodeTaxRule = new ZnodeRepository<ZnodeTaxRule>();
            _znodeCity = new ZnodeRepository<ZnodeCity>();
            _znodeTaxClassPortal = new ZnodeRepository<ZnodePortalTaxClass>();
            _znodeTaxRuleType = new ZnodeRepository<ZnodeTaxRuleType>();
            _stateRepository = new ZnodeRepository<ZnodeState>();
            _portalWareHouseRepository = new ZnodeRepository<ZnodePortalWarehouse>();
            _shippingPortalRepository = new ZnodeRepository<ZnodeShippingPortal>();
            _wareHouseRepository = new ZnodeRepository<ZnodeWarehouse>();
            _wareHouseAddressRepository = new ZnodeRepository<ZnodeWarehouseAddress>();
            _addressRepository = new ZnodeRepository<ZnodeAddress>();
            _taxPortalRepository = new ZnodeRepository<ZnodeTaxPortal>();
            _omsTaxOrderLineDetails = new ZnodeRepository<ZnodeOmsTaxOrderLineDetail>();
            _userRepository = new ZnodeRepository<ZnodeUser>();
            _omsOrderLineItems = new ZnodeRepository<ZnodeOmsOrderLineItem>();
            _omsOrderDetails = new ZnodeRepository<ZnodeOmsOrderDetail>();
        }
        #endregion

        #region Public Method

        //Get the value for Taxes In Product Price.
        public virtual bool IsIncludePortalLevelTax(string storeFeature)
        {
            return false;
        }

        // Get inclusive tax rules.
        public virtual List<TaxRuleModel> GetInclusiveTaxRules()
        {
            List<TaxRuleModel> taxRuleList = (from taxClass in _znodeTaxClass.Table
                                              join taxRule in _znodeTaxRule.Table on taxClass.TaxClassId equals taxRule.TaxClassId
                                              select new TaxRuleModel { TaxClassId = taxClass.TaxClassId, TaxRate = (Convert.ToDecimal(taxRule.VAT) + Convert.ToDecimal(taxRule.SalesTax) + Convert.ToDecimal(taxRule.HST) + Convert.ToDecimal(taxRule.GST) + Convert.ToDecimal(taxRule.PST)), Precedence = taxRule.Precedence, DestinationCountryCode = taxRule.DestinationCountryCode ?? "", DestinationStateCode = taxRule.DestinationStateCode ?? "", CountyFIPS = taxRule.CountyFIPS ?? "" }
                                              ).ToList();

            return taxRuleList ?? new List<TaxRuleModel>();

        }

        // Get country list.
        public virtual List<CityModel> GetTaxRuleByCountryFIPSCityStateCodePostalCode(AddressModel address, TaxRuleModel taxRule)
        {
            List<CityModel> cityList = (from city in _znodeCity.Table
                                        where
                                           city.CountyFIPS == taxRule.CountyFIPS && city.CityName == address.CityName && city.StateCode == address.StateCode && city.ZIP == address.PostalCode

                                        select new CityModel
                                        {
                                            CountyFIPS = city.CountyFIPS,
                                            CityName = city.CityName,
                                            StateCode = city.StateCode,
                                            ZIP = city.ZIP
                                        }).ToList();

            return cityList ?? new List<CityModel>();
        }

        // Ger user Id from current request headers.
        public virtual int GetLoginUserId()
        {
            const string headerUserId = "Znode-UserId";
            int userId = 0;
            var headers = HttpContext.Current.Request.Headers;
            int.TryParse(headers[headerUserId], out userId);
            return userId;
        }

        // Get profile model on basis of user id from cache.
        public virtual ProfileModel GetProfileCache()
        {
            int portalId = HelperUtility.GetPortalId();
            string cacheName = GetLoginUserId() > 0 ? $"ProfileCache_{ GetLoginUserId() }_{portalId}" : $"ProfileCache_{portalId}";
            List<ProfileModel> profileListModel = (List<ProfileModel>)HttpContext.Current.Cache[cacheName];
            return HelperUtility.IsNotNull(profileListModel) ? profileListModel.FirstOrDefault() : new ProfileModel();
        }

        public virtual List<TaxRuleModel> GetTaxRuleListByPortalId(AddressModel shippingAddress, int portalId, int? profileId, int? userId = 0, int? orderId = 0)
        {
            string countryCode = shippingAddress?.CountryName ?? string.Empty;
            string stateName = shippingAddress?.StateName ?? string.Empty;
            string zipcode = shippingAddress?.PostalCode ?? string.Empty;

            //if user shipping address is null then no need to calculate tax
            if (string.IsNullOrEmpty(countryCode) || string.IsNullOrEmpty(stateName) || string.IsNullOrEmpty(zipcode))
            {
                return new List<TaxRuleModel>();
            }

            string stateCode = GetStateCode(stateName, countryCode);

            List<TaxRuleModel> taxRuleList = GetTaxRuleList(countryCode, stateCode, portalId, profileId, userId, orderId);

            //to filter tax rule by zipcode
            if (!string.IsNullOrEmpty(zipcode))
                taxRuleList = GetTaxRuleByZipCode(zipcode, taxRuleList);

            return taxRuleList ?? new List<TaxRuleModel>();
        }

        //Get portal shipping from address.
        public virtual AddressModel GetPortalShippingAddress(int portalId)
        {
            AddressModel portalShippingAddress = new AddressModel();
            AddressModel address = GetPortalShippingOriginAddress(portalId);
            portalShippingAddress = Convert.ToBoolean(address.IsUseWareHouseAddress)
                                            ? GetPortalWareHouseAddress(portalId) : address;

            return portalShippingAddress;
        }
        #endregion

        #region Private Method

        // Get tax rule list.
        public virtual List<TaxRuleModel> GetTaxRuleList(string countryCode, string stateCode, int? portalId, int? profileId = 0, int? userId = 0, int? orderId = 0)
        {
            IZnodeViewRepository<TaxRuleModel> objStoredProc = new ZnodeViewRepository<TaxRuleModel>();

            objStoredProc.SetParameter("@CountryCode", countryCode, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@StateCode", stateCode, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@ProfileId", Convert.ToInt32(profileId), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@UserId", Convert.ToInt32(userId) > 0 ? userId : -1, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@OrderId", Convert.ToInt32(orderId) > 0 ? orderId : -1, ParameterDirection.Input, DbType.Int32);
            IList<TaxRuleModel> taxRuleList = objStoredProc.ExecuteStoredProcedureList("Znode_GetTaxClassRule @CountryCode,@StateCode,@PortalId,@ProfileId,@UserId,@OrderId");
            return taxRuleList?.ToList() ?? new List<TaxRuleModel>();
        }

        //to get filtered taxrule by zipcode
        protected virtual List<TaxRuleModel> GetTaxRuleByZipCode(string zipcode, List<TaxRuleModel> taxRulelist)
        {
            if (taxRulelist?.Count > 0)
            {
                List<TaxRuleModel> filteredtaxRulelist = new List<TaxRuleModel>();
                //to check each tax rule have zipcode entered by user
                foreach (TaxRuleModel taxRule in taxRulelist)
                {
                    //if tax rule zipcode is null or "*" then allow for all zipcode entered by user 
                    if (string.IsNullOrEmpty(taxRule.ZipCode) || taxRule.ZipCode.Trim() == "*")
                    {
                        filteredtaxRulelist.Add(taxRule);
                    }
                    else
                    {
                        //if tax rule zipcode contains "," then it will have more than one zipcode allows
                        if (taxRule.ZipCode.Contains(","))
                        {
                            string[] allZipCodesAssignToTaxRule = taxRule.ZipCode.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            //to check each zipcode that entered against taxrule comma separate
                            foreach (string taxRuleZipCode in allZipCodesAssignToTaxRule)
                            {
                                //to check zipcode for each tax rule 
                                if (IsValidZipCode(zipcode, taxRuleZipCode, taxRule, filteredtaxRulelist))
                                    break;
                            }
                        }
                        else
                        {
                            IsValidZipCode(zipcode, taxRule.ZipCode, taxRule, filteredtaxRulelist);
                        }
                    }
                }
                return filteredtaxRulelist;
            }
            return taxRulelist;
        }

        //to check zipcode is valid for tax rule
        protected virtual bool IsValidZipCode(string userZipcode, string taxruleZipcode, TaxRuleModel taxRule, List<TaxRuleModel> filteredtaxRulelist)
        {
            bool result = false;
            //add tax rule zipcode having "*"
            if (taxruleZipcode.Contains("*"))
            {
                string shippingZipCode = taxruleZipcode.Replace("*", string.Empty).Trim();
                //taxrule Zipcode start with the user zipcode then allow to add
                if (userZipcode.Trim().StartsWith(shippingZipCode))
                {
                    filteredtaxRulelist.Add(taxRule);
                    result = true;
                }
            }
            //taxrule Zipcode is same as user zipcode then allow to add
            else if (string.Equals(taxruleZipcode.Trim(), userZipcode.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                filteredtaxRulelist.Add(taxRule);
                result = true;
            }
            return result;
        }

        // Get state code by stateName
        protected virtual string GetStateCode(string stateName, string countryCode)
        {
            stateName = !string.IsNullOrEmpty(stateName) ? stateName.Trim() : null;
            if ((stateName?.Length > 3))
            {
                //to state code by stateName
                string stateCode = Convert.ToString((from state in _stateRepository.Table
                                                     where state.StateName == stateName
                                                     && state.CountryCode == countryCode
                                                     select state.StateCode).FirstOrDefault());
                return stateCode ?? string.Empty;
            }
            return stateName ?? string.Empty;
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

        // Get tax on the basis of portal id.
        public virtual TaxPortalModel GetTaxPortalData(int portalId)
        {
            TaxPortalModel taxPortalModel = (from taxPortal in _taxPortalRepository.Table
                                             where taxPortal.PortalId == portalId
                                             select new TaxPortalModel()
                                             {
                                                 AvataxUrl = taxPortal.AvataxUrl,
                                                 AvalaraLicense = taxPortal.AvalaraLicense,
                                                 AvalaraAccount = taxPortal.AvalaraAccount,
                                                 AvalaraCompanyCode = taxPortal.AvalaraCompanyCode,
                                                 AvalaraFreightIdentifier = taxPortal.AvalaraFreightIdentifier,

                                             }).FirstOrDefault();
            return taxPortalModel ?? new TaxPortalModel();
        }

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

        //Get tax transaction number.
        public virtual string GetTaxTransactionNumber(int? orderId)
        {
            string taxTransactionNumber = (from ep in _omsTaxOrderLineDetails.Table
                                           join e in _omsOrderLineItems.Table on ep.OmsOrderLineItemsId equals e.OmsOrderLineItemsId
                                           join ord in _omsOrderDetails.Table on e.OmsOrderDetailsId equals ord.OmsOrderDetailsId
                                           where ord.OmsOrderId == orderId
                                           select new
                                           {
                                               TaxTransactionNumber = ep.TaxTransactionNumber
                                           })?.FirstOrDefault()?.TaxTransactionNumber;

            return taxTransactionNumber?.ToString();
        }

        //to check user is guest user or registered
        public virtual bool IsGuestUser(int userId)
        {
            ZnodeUser user = _userRepository.Table.FirstOrDefault(x => x.UserId == userId && x.AspNetUserId == null);
            if (HelperUtility.IsNotNull(user))
                return true;
            else
                return false;
        }

        //Get the currency.
        public virtual string GetCulture(int currentStore)
        {
            IZnodeRepository<ZnodeCurrency> znodeCurrency = new ZnodeRepository<ZnodeCurrency>();
            IZnodeRepository<ZnodePortalUnit> znodePortalUnit = new ZnodeRepository<ZnodePortalUnit>();
            IZnodeRepository<ZnodeCulture> znodeCulture = new ZnodeRepository<ZnodeCulture>();

            string culture = (from _znodePortalUnit in znodePortalUnit.Table
                               join _znodeCurrency in znodeCurrency.Table on _znodePortalUnit.CurrencyId equals _znodeCurrency.CurrencyId
                               join _znodeCulture in znodeCulture.Table on _znodePortalUnit.CultureId equals _znodeCulture.CultureId
                               where _znodePortalUnit.PortalId == currentStore
                               select
                                   _znodeCulture.CultureCode
                        )?.FirstOrDefault();
            return culture;
        }

        public static string GetLogFor(object target)
        {
            var properties =
                from property in target.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                select new
                {
                    Name = property.Name,
                    Value = property.GetValue(target, null)
                };

            var builder = new StringBuilder();

            foreach (var property in properties)
            {
                builder
                    .Append(property.Name)
                    .Append(" = ")
                    .Append(property.Value)
                    .AppendLine();
            }

            return builder.ToString();
        }

        #endregion
    }
}
