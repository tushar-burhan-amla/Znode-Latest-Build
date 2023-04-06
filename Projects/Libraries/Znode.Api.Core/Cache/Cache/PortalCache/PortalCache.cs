using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class PortalCache : BaseCache, IPortalCache
    {
        #region Private Variables
        private readonly IPortalService _portalService;
        #endregion

        #region Constructor
        public PortalCache(IPortalService portalService)
        {
            _portalService = portalService;
        }
        #endregion

        #region Public Methods
        public virtual string GetPortalList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from service
                PortalListModel portalList = _portalService.GetPortalList(Expands, Filters, Sorts, Page);
                if (portalList?.PortalList?.Count > 0)
                {
                    //Create Response and insert in to cache
                    PortalListResponse response = new PortalListResponse { PortalList = portalList.PortalList };

                    response.MapPagingDataFromModel(portalList);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }


#if DEBUG
        //Get all portals for select list item
        public virtual string GetDevPortalList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from service
                PortalListModel portalList = _portalService.GetDevPortalList();
                if (portalList?.PortalList?.Count > 0)
                {
                    //Create Response and insert in to cache
                    PortalListResponse response = new PortalListResponse { PortalList = portalList.PortalList };

                    response.MapPagingDataFromModel(portalList);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
#endif

        public virtual string GetPortal(int portalId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from portals service.
                PortalModel portalModel = _portalService.GetPortal(portalId, Expands);
                if (!Equals(portalModel, null))
                {
                    //Create Response and insert in to cache
                    PortalResponse response = new PortalResponse { Portal = portalModel };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get all portals on Catalog Id
        public virtual string GetPortalListByCatalogId(int catalogId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PortalListModel portalList = _portalService.GetPortalListByCatalogId(catalogId);
                if (portalList?.PortalList?.Count > 0)
                {
                    //Create Response and insert in to cache
                    PortalListResponse response = new PortalListResponse { PortalList = portalList.PortalList };

                    response.MapPagingDataFromModel(portalList);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get Portal By StoreCode
        public virtual string GetPortal(string storeCode, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from portals service.
                PortalModel portalModel = _portalService.GetPortal(storeCode, Expands);
                if (IsNotNull(portalModel))
                {
                    //Create Response and insert in to cache
                    PortalResponse response = new PortalResponse { Portal = portalModel };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetPortalFeatures(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from service
                List<PortalFeatureModel> portalFeatureList = _portalService.GetPortalFeatures();
                if (portalFeatureList?.Count > 0)
                {
                    //Create Response and insert in to cache
                    PortalListResponse response = new PortalListResponse { PortalFeatureList = portalFeatureList };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetAssociatedWarehouseList(int portalId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from service
                PortalWarehouseModel associatedWarehouseList = _portalService.GetAssociatedWarehouseList(portalId, Filters, Expands);
                if (IsNotNull(associatedWarehouseList))
                {
                    //Create Response and insert in to cache
                    PortalWarehouseResponse response = new PortalWarehouseResponse { PortalWarehouse = associatedWarehouseList };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get Locale List
        public virtual string LocaleList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get Menu list
                LocaleListModel list = _portalService.LocaleList(Expands, Filters, Sorts, Page);
                if (list?.Locales?.Count > 0)
                {
                    //Create response.
                    LocaleListResponse response = new LocaleListResponse { Locales = list.Locales };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get portal shipping on the basis of portalId.
        public virtual string GetPortalShippingInformation(int portalId, string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response
                PortalShippingModel portalShippingModel = _portalService.GetPortalShippingInformation(portalId, Filters);
                if (IsNotNull(portalShippingModel))
                {
                    PortalShippingResponse response = new PortalShippingResponse { PortalShipping = portalShippingModel };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get portal tax on the basis of portalId.
        public virtual string GetTaxPortalInformation(int portalId, string routeUri, string routeTemplate)
        {
            //Get data from Cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response.
                TaxPortalModel taxPortalModel = _portalService.GetTaxPortalInformation(portalId, Expands);
                if (IsNotNull(taxPortalModel))
                {
                    TaxPortalResponse response = new TaxPortalResponse { TaxPortal = taxPortalModel };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get portal publish status.
        public virtual PublishPortalLogListModel GetPortalPublishStatus(string routeUri, string routeTemplate)
        => _portalService.GetPortalPublishStatus(Filters, Sorts, Page);

        //Get portal tracking pixel by portal id.
        public virtual string GetPortalTrackingPixel(int portalId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from portals service.
                PortalTrackingPixelModel portalTrackingPixelModel = _portalService.GetPortalTrackingPixel(portalId, Expands);
                if (IsNotNull(portalTrackingPixelModel))
                {
                    //Create Response and insert into cache.
                    PortalTrackingPixelResponse response = new PortalTrackingPixelResponse { PortalTrackingPixel = portalTrackingPixelModel };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get robots.txt data.
        public virtual string GetRobotsTxt(int portalId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from portals service.
                RobotsTxtModel robotsTxtModel = _portalService.GetRobotsTxt(portalId, Expands);
                if (IsNotNull(robotsTxtModel))
                {
                    //Create Response and insert into cache.
                    RobotsTxtResponse response = new RobotsTxtResponse { RobotsTxt = robotsTxtModel };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }


        //Get Associate Portal Approval Management Details
        public virtual string GetPortalApprovalDetails(int portalId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from service
                PortalApprovalModel portalModel = _portalService.GetPortalApprovalDetails(portalId);
                if (IsNotNull(portalModel))
                {
                    //Create Response and insert in to cache
                    PortalApprovalResponse response = new PortalApprovalResponse { PortalApprovalModel = portalModel };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get Portal Approval type list
        public virtual string GetPortalApprovalTypeList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get level list.
                PortalApprovalTypeListModel portalApprovalTypeList = _portalService.GetPortalApprovalTypeList();

                if (IsNotNull(portalApprovalTypeList))
                {
                    PortalApprovalTypeListResponse response = new PortalApprovalTypeListResponse { portalApprovalTypeListResponse = portalApprovalTypeList.PortalApprovalTypes };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }

            return data;
        }

        //Get Portal Approval level list.
        public virtual string GetPortalApprovalLevelList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get level list.
                PortalApprovalLevelListModel portalApprovalLevelList = _portalService.GetPortalApprovalLevelList();

                if (IsNotNull(portalApprovalLevelList))
                {
                    PortalApprovalLevelListResponse response = new PortalApprovalLevelListResponse { portalApprovalLevelList = portalApprovalLevelList.PortalApprovalLevels };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }

            return data;
        }

        //Get Associate Portal Approver List
        public virtual string GetPortalApproverList(int portalApprovalId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from service
                List<UserApproverModel> userApproverList = _portalService.GetPortalApproverList(portalApprovalId);
                if (userApproverList.Count > 0)
                {
                    //Create Response and insert in to cache
                    UserApproverListResponse response = new UserApproverListResponse { UserApproverModelList = userApproverList };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get list of sort Settings.
        public virtual string GetPortalSortSettings(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get GiftCard list.
                PortalSortSettingListModel list = _portalService.GetPortalSortSettingList(Expands, Filters, Sorts, Page);
                if (list?.SortSettings?.Count > 0)
                {
                    //Create response.
                    SortSettingListResponse response = new SortSettingListResponse { SortSettings = list.SortSettings };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get list of sort Settings.
        public virtual string GetPortalPageSettings(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get GiftCard list.
                PortalPageSettingListModel list = _portalService.GetPortalPageSettingList(Expands, Filters, Sorts, Page);
                if (list?.PageSettings?.Count > 0)
                {
                    //Create response.
                    PageSettingListResponse response = new PageSettingListResponse { PageSettings = list.PageSettings };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get barcode scanner details
        public virtual string GetBarcodeScanner(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from service
                BarcodeReaderModel barcodeReader = _portalService.GetBarcodeScanner();
                if (IsNotNull(barcodeReader))
                {
                    //Create Response and insert in to cache
                    BarcodeReaderModelResponse response = new BarcodeReaderModelResponse { BarcodeReader = barcodeReader };
                    
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        #endregion
    }
}