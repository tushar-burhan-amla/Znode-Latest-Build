using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Znode.Engine.Api.Models;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Taxes
{

    // Used to calculate and return inclusive/exclusive pricing for a product.
    [Serializable]
    public class ZnodeInclusiveTax : ZnodeBusinessBase
    {
        #region Member Variables
        private const string InclusiveTaxRules = "InclusiveTaxRules";
        #endregion

        // Checks to see if the current profile is tax exempt.
        public virtual bool TaxExemptProfile
        {
            get
            {
                IZnodeTaxHelper taxHelper = GetService<IZnodeTaxHelper>();
                if (IsNotNull(taxHelper.GetProfileCache()))
                {
                    ProfileModel customerProfile = taxHelper.GetProfileCache();
                    if (IsNotNull(customerProfile))
                        return customerProfile.TaxExempt;
                }

                return false;
            }
        }

        /// <summary>
        /// Caches the inclusive tax rules.
        /// </summary>
        public virtual void CacheInclusiveTax()
        {
            IZnodeTaxHelper taxHelper = GetService<IZnodeTaxHelper>();
            if (taxHelper.IsIncludePortalLevelTax(StoreFeature.Taxes_In_Product_Price.ToString()) && !TaxExemptProfile &&
                Equals(HttpContext.Current.Cache[InclusiveTaxRules], null))
            {
                List<TaxRuleModel> taxRuleList = taxHelper.GetInclusiveTaxRules();

                ZnodeCacheDependencyManager.Insert(InclusiveTaxRules, taxRuleList, "ZNodeTaxRule");
            }
        }

        /// Gets data from the inclusive tax cache.       
        private List<TaxRuleModel> InclusiveTaxTable
        {
            get
            {
                List<TaxRuleModel> taxRuleList = (List<TaxRuleModel>)HttpRuntime.Cache.Get(InclusiveTaxRules);
                if (Equals(taxRuleList, null))
                {
                    CacheInclusiveTax();
                    taxRuleList = (List<TaxRuleModel>)HttpRuntime.Cache.Get(InclusiveTaxRules);
                }

                return taxRuleList;
            }
        }


        /// Gets the tax rules for the tax class.       
        private List<TaxRuleModel> GetRulesByTaxClassId(int taxClassId, AddressModel address, AddressModel userShippingAddressModel)
        {

            List<TaxRuleModel> tempTaxRuleListModel = new List<TaxRuleModel>();
            if (IsNotNull(InclusiveTaxTable))
            {

                if (Equals(address?.AddressId, 0) && IsNotNull(userShippingAddressModel))
                    address = IsNotNull(userShippingAddressModel) ? userShippingAddressModel : new AddressModel();

                if (Equals(address, null))                
                    tempTaxRuleListModel = GetTaxRuleListByAddress(tempTaxRuleListModel, address, taxClassId);
                
                else
                {
                    tempTaxRuleListModel = GetTaxRuleListByWithoutAddress(InclusiveTaxTable, tempTaxRuleListModel, taxClassId);
                   
                }
            }
            return tempTaxRuleListModel;
        }
        
        /// Gets the product price including the tax.        
        public virtual decimal GetInclusivePrice(int taxClassId, decimal productPrice, AddressModel address, AddressModel userShippingAddressModel)
        {
            decimal totalTax = 0;
            IZnodeTaxHelper taxHelper = GetService<IZnodeTaxHelper>();
            if (taxHelper.IsIncludePortalLevelTax(StoreFeature.Taxes_In_Product_Price.ToString()) && !TaxExemptProfile && taxClassId > 0 && productPrice > 0)
            {
                // Need to pass userAddressModel for check user address.
                List<TaxRuleModel> taxRuleListModel = GetRulesByTaxClassId(taxClassId, address, userShippingAddressModel);

                if (taxRuleListModel?.Count > 0)
                {
                    foreach (TaxRuleModel taxRuleItem in taxRuleListModel)
                    {
                        totalTax += productPrice * (taxRuleItem.TaxRate / 100);
                    }
                }
            }
            return (productPrice + totalTax);
        }


        // Gets the total tax rate for the given tax class.
        public virtual decimal GetInclusiveTaxRate(int taxClassId, AddressModel userShippingAddressModel)
        {
            decimal totalTaxRate = 0;

            IZnodeTaxHelper taxHelper = GetService<IZnodeTaxHelper>();
            if (taxHelper.IsIncludePortalLevelTax(StoreFeature.Taxes_In_Product_Price.ToString()) && !TaxExemptProfile && taxClassId > 0)
            {
                List<TaxRuleModel> taxRuleListModel = GetRulesByTaxClassId(taxClassId, null, userShippingAddressModel);

                if (taxRuleListModel?.Count > 0)
                {
                    foreach (TaxRuleModel taxRuleItem in taxRuleListModel)
                    {
                        totalTaxRate += taxRuleItem.TaxRate;
                    }
                }
            }
            return totalTaxRate;
        }

        // Get rule list on basis of address.
        private List<TaxRuleModel> GetTaxRuleListByAddress(List<TaxRuleModel> tempTaxRuleListModel, AddressModel address, int taxClassId)
        {
            // Get tax rules list.
            List<TaxRuleModel> taxRuleList = (from taxTable in InclusiveTaxTable
                                              where taxTable.TaxClassId == taxClassId &&
                                              (taxTable.DestinationCountryCode == ""
                                              || (taxTable.DestinationCountryCode == address.CountryName && taxTable.DestinationStateCode == "")
                                              || (taxTable.DestinationCountryCode == address.CountryName && taxTable.DestinationStateCode == address.StateCode))
                                              orderby taxTable.CountyFIPS, taxTable.DestinationStateCode, taxTable.DestinationCountryCode, taxTable.Precedence descending

                                              select new TaxRuleModel { TaxClassId = taxTable.TaxClassId, TaxRate = (Convert.ToDecimal(taxTable.VAT) + Convert.ToDecimal(taxTable.SalesTax) + Convert.ToDecimal(taxTable.HST) + Convert.ToDecimal(taxTable.GST) + Convert.ToDecimal(taxTable.PST)), Precedence = taxTable.Precedence, DestinationCountryCode = taxTable.DestinationCountryCode ?? "", DestinationStateCode = taxTable.DestinationStateCode ?? "", CountyFIPS = taxTable.CountyFIPS ?? "" }).ToList();

            // Loop for check destination state code.
            foreach (TaxRuleModel taxRuleItem in taxRuleList)
            {
                if (string.IsNullOrEmpty(taxRuleItem.DestinationStateCode))
                {
                    tempTaxRuleListModel.Add(taxRuleItem);
                }
                else
                {
                    IZnodeTaxHelper taxHelper = GetService<IZnodeTaxHelper>();
                    if (!Equals(taxHelper.GetTaxRuleByCountryFIPSCityStateCodePostalCode(address, taxRuleItem).Count, 0))
                    {
                        tempTaxRuleListModel.Add(taxRuleItem);
                    }
                }
            }

            return tempTaxRuleListModel;
        }

        // Get tax rule without address.
        private List<TaxRuleModel> GetTaxRuleListByWithoutAddress(List<TaxRuleModel> inclusiveTaxTable, List<TaxRuleModel> tempTaxRuleListModel, int taxClassId)
        {
            // Get tax rule list.
            List<TaxRuleModel> taxRuleList = (from taxTable in InclusiveTaxTable
                                              where taxTable.TaxClassId == taxClassId && taxTable.DestinationCountryCode == ""
                                              orderby taxTable.Precedence ascending

                                              select new TaxRuleModel { TaxClassId = taxTable.TaxClassId, TaxRate = (Convert.ToDecimal(taxTable.VAT) + Convert.ToDecimal(taxTable.SalesTax) + Convert.ToDecimal(taxTable.HST) + Convert.ToDecimal(taxTable.GST) + Convert.ToDecimal(taxTable.PST)), Precedence = taxTable.Precedence, DestinationCountryCode = taxTable.DestinationCountryCode ?? "", DestinationStateCode = taxTable.DestinationStateCode ?? "", CountyFIPS = taxTable.CountyFIPS ?? "" }).ToList();

            tempTaxRuleListModel.Add(taxRuleList.FirstOrDefault());
            
            return tempTaxRuleListModel;
        }
    }
}
