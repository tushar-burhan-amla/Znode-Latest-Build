namespace Znode.Libraries.ECommerce.Utilities
{
    public static class ZnodeCurrencyManager
    {
        #region Public Methods     

        //Format price with currency.
        public static string FormatPriceWithCurrency(decimal priceValue, string cultureCode, string uom = "")
        {
            if (HelperUtility.IsNotNull(priceValue))
            {
                //Set new culture to current thread
                System.Globalization.CultureInfo newcultureInfo = new System.Globalization.CultureInfo(cultureCode);
                string cultureValue = string.Empty;

                System.Globalization.CultureInfo info = new System.Globalization.CultureInfo(cultureCode);
                info.NumberFormat.CurrencyNegativePattern = 0;
                decimal price = priceValue;
                cultureValue = price.ToString("c", info.NumberFormat);
                return !string.IsNullOrEmpty(uom) ? $"{cultureValue} / {uom}" : cultureValue;
            }
            return null;
        }

        // Returns the Currency code for the current culture
        public static string CurrencyCode()
        {
            // Create RegionInfo using CultureInfo (Current Culture)
            System.Globalization.RegionInfo regionInfo = new System.Globalization.RegionInfo(System.Threading.Thread.CurrentThread.CurrentCulture.LCID);

            return regionInfo.ISOCurrencySymbol;
        }
        #endregion
    }
}
