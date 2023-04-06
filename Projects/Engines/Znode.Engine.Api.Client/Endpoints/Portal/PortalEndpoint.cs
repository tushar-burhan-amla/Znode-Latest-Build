namespace Znode.Engine.Api.Client.Endpoints
{
    public class PortalEndpoint : BaseEndpoint
    {
        //Endpoint to get the list of all portals.
        public static string GetPortalList() => $"{ApiRoot}/portal/list";

#if DEBUG
        //Endpoint to get the list of all portals.
        public static string GetDevPortalList() => $"{ApiRoot}/portal/getdevportallist";
#endif
        //Endpoint to get details of portal by portal Id.
        public static string GetPortal(int portalId) => $"{ApiRoot}/portal/getportal/{portalId}";

        //Endpoint to get details of portal by portalId.
        public static string GetPortalListByCatalogId(int catalogId) => $"{ApiRoot}/portal/getportallistbycatalogId/{catalogId}";

        //Endpoint to get details of portal by store Code.
        public static string GetPortalByStoreCode(string storeCode) => $"{ApiRoot}/portal/getPortalByStoreCode/{storeCode}";

        //Endpoint to create new portal.
        public static string CreatePortal() => $"{ApiRoot}/portal/create";

        //Endpoint to update portal.
        public static string UpdatePortal() => $"{ApiRoot}/portal/update";

        //Endpoint to delete portal by portal Id.
        public static string DeletePortal(bool isDeleteByStoreCode) => $"{ApiRoot}/portal/delete/{isDeleteByStoreCode}";

        //Endpoint to Copy store.
        public static string CopyStore() => $"{ApiRoot}/portal/copystore";

        //Endpoint to get the list of all portal features.
        public static string GetPortalFeatureList() => $"{ApiRoot}/portal/getportalfeaturelist";

        // This is the endpoint method for clear cache
        public static string ClearCache(int domainId) => $"{ApiRoot}/portal/clearapicache/{domainId}";

        // This is the endpoint method for associated warehouses
        public static string GetAssociatedWarehouseList(int portalId) => $"{ApiRoot}/portal/associatedwarehouselist/{portalId}";

        // This is the endpoint method to associate warehouse to portal
        public static string AssociateWarehouseToStore() => $"{ApiRoot}/portal/associatewarehouse";

        //Check portal code already exist or not.
        public static string IsPortalCodeExist(string portalCode) => $"{ApiRoot}/portal/isportalcodeexist/{portalCode}";
        
        #region Locale Association
        //Get Locale list.
        public static string GetLocaleList() => $"{ApiRoot}/portal/Localelist";

        //Update Locale.
        public static string UpdateLocale() => $"{ApiRoot}/portal/updatelocale";
        #endregion

        #region Shipping Association
        //Get portal shipping data on the basis of portalId.
        public static string GetPortalShippingInformation(int portalId) => $"{ApiRoot}/portal/getportalshippinginformation/{portalId}";

        //Update portal shipping data.
        public static string UpdatePortalShipping() => $"{ApiRoot}/portal/updateportalshipping";
        #endregion

        #region Tax Association
        //Get portal tax data on the basis of portalId.
        public static string GetTaxPortalInformation(int portalId) => $"{ApiRoot}/portal/gettaxportalinformation/{portalId}";

        //Update portal tax data.
        public static string UpdateTaxPortal() => $"{ApiRoot}/portal/updatetaxportal";
        #endregion

        #region Country Association
        //Get list of unassociate countries.
        public static string GetUnAssociatedCountryList() => $"{ApiRoot}/portalcountry/getunassociatedcountrylist";

        //Get associated country list
        public static string GetAssociatedCountryList() => $"{ApiRoot}/portalcountry/getassociatedcountrylist";

        //UnAssociated associated countries. 
        public static string UnAssociateCountries() => $"{ApiRoot}/portalcountry/unassociatecountries";

        //Associate countries.
        public static string AssociateCountries() => $"{ApiRoot}/portalcountry/associatecountries";
        #endregion        

        #region Tax

        //Associate/Unassociate tax class to portal
        public static string AssociateAndUnAssociateTaxClass() => $"{ApiRoot}/portal/associateandunassociatetaxclass";

        //Set default tax for portal.
        public static string SetPortalDefaultTax() => $"{ApiRoot}/portal/setportaldefaulttax";
        #endregion

        #region Portal Tracking Pixel
        //Get portal tracking pixel.
        public static string GetPortalTrackingPixel(int portalId) => $"{ApiRoot}/portal/getportaltrackingpixel/{portalId}";

        //Save portal tracking pixel.
        public static string SavePortalTrackingPixel() => $"{ApiRoot}/portal/saveportaltrackingpixel";
        #endregion

        #region Product Search Filter Setting
        //Endpoint to get search result.
        public static string SortList() => $"{ApiRoot}/portal/sortlist";

        //Endpoint to get search result.
        public static string PageList() => $"{ApiRoot}/portal/pagelist";

        // Remove associated sort settings.
        public static string RemoveAssociatedSortSettings() => $"{ApiRoot}/portal/removeassociatedsortsettings";

        // Remove associated sort settings.
        public static string RemoveAssociatedPageSettings() => $"{ApiRoot}/portal/removeassociatedpagesettings";

        // Remove associated sort settings.
        public static string AssociateSortSettings() => $"{ApiRoot}/portal/associatesortsettings";

        // Remove associated page settings.
        public static string AssociatePageSettings() => $"{ApiRoot}/portal/associatepagesettings";

        // Update portal page settings.
        public static string UpdatePortalPageSetting() => $"{ApiRoot}/portal/updateportalpagesetting"; 

        #endregion


        //Get portal publish status list endpoint
        public static string GetPortalPublishStatus() => $"{ApiRoot}/portal/getportalpublishstatus";

        #region Robots.txt
        //Get robots.txt data.
        public static string GetRobotsTxt(int portalId) => $"{ApiRoot}/portal/getrobotstxt/{portalId}";

        //Save robots.txt data.
        public static string SaveRobotsTxt() => $"{ApiRoot}/portal/saverobotstxt";
        #endregion


        //Get portal approval management list endpoint
        public static string GetPortalApproverDetailsById(int portalId) => $"{ApiRoot}/portal/getportalapprovaldetailsbyid/{portalId}";

        //Get approver order list
        public static string GetApproverOrder() => $"{ApiRoot}/portal/getapproverorder";

        //Delete approval levels
        public static string DeletePortalApproverUserById() => $"{ApiRoot}/portal/deleteportalapproveruserbyid";

        //Save/Update the Portal Approval details.
        public static string SaveUpdatePortalApprovalDetails() => $"{ApiRoot}/portal/saveupdateportalapprovaldetails";

        //Get Barcode scanner details.
        public static string GetBarcodeScannerDetail() => $"{ApiRoot}/portal/getbarcodescanner";
    }
}
