using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using Znode.Libraries.Resources;

namespace Znode.Engine.Services
{
    public partial class StoreLocatorService : BaseService, IStoreLocatorService
    {
        #region Private Variable
        private readonly IZnodeRepository<ZnodeAddress> _addressRepository;
        private readonly IZnodeRepository<ZnodePortalAddress> _portalAddressRepository;
        #endregion

        #region Public Constructor.
        public StoreLocatorService()
        {
            _addressRepository = new ZnodeRepository<ZnodeAddress>();
            _portalAddressRepository = new ZnodeRepository<ZnodePortalAddress>();
        }
        #endregion

        #region Public Methods.
        //Get Store List for location.
        public virtual StoreLocatorListModel GetStoreLocatorList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            //Bind the Filter conditions for the authorized portal access.
            BindUserPortalFilter(ref filters);
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            IZnodeViewRepository<StoreLocatorDataModel> objStoredProc = new ZnodeViewRepository<StoreLocatorDataModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@StateCode", "", ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            IList<StoreLocatorDataModel> list = objStoredProc.ExecuteStoredProcedureList("Znode_GetStoreDetail @WhereClause,@Rows,@PageNo,@Order_By,@StateCode,@RowCount OUT", 5, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("Store locator list count: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, list?.Count());

            StoreLocatorListModel listModel = new StoreLocatorListModel { StoreLocatorList = list?.ToList() };
            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Save store data for store location.
        public virtual StoreLocatorDataModel Create(StoreLocatorDataModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            ZnodeAddress address = _addressRepository.Insert(model.ToEntity<ZnodeAddress>());

            if (address.AddressId > 0)
            {
                model.AddressId = address.AddressId;
                ZnodePortalAddress entity = _portalAddressRepository.Insert(model.ToEntity<ZnodePortalAddress>());

                if (IsNotNull(entity))
                {
                    model.PortalAddressId = entity.PortalAddressId > 0 ? entity.PortalAddressId : 0;
                    ZnodeLogging.LogMessage(entity.PortalAddressId > 0 ?
                            Admin_Resources.SuccessStoreDataSave : Admin_Resources.ErrorStoreDataSave, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                }
            }
            ZnodeLogging.LogMessage("PortalId, AddressId and  PortalAddressId properties of StoreLocatorDataModel: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { PortalId = model?.PortalId, AddressId = model?.AddressId, PortalAddressId = model?.PortalAddressId });
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return model;
        }

        //Update an existing store data for store location.
        public virtual bool UpdateStoreLocator(StoreLocatorDataModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);
            ZnodeLogging.LogMessage("PortalId, AddressId and PortalAddressId properties of StoreLocatorDataModel: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { PortalId = model?.PortalId, AddressId = model?.AddressId, PortalAddressId = model?.PortalAddressId });

            bool status = false;
            if (model.PortalAddressId > 0)
            {
                status = _portalAddressRepository.Update(model.ToEntity<ZnodePortalAddress>());
                if (status)
                    status = _addressRepository.Update(model.ToEntity<ZnodeAddress>());

                return status;
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            return status;
        }

        //Delete an existing store data.
        public virtual bool DeleteStoreLocator(ParameterModel storeIds, bool isDeleteByCode)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            int status = 0;

            if (string.IsNullOrEmpty(storeIds?.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.StoreIdNotNullOrEmpty);
            ZnodeLogging.LogMessage("storeIds value to DeleteStoreLocator: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, storeIds?.Ids);

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = GetSPParameters(storeIds, isDeleteByCode);
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeletePortalAddress @PortalAddressId,@StoreLocationCode,@Status OUT", 2, out status);
            ZnodeLogging.LogMessage("Deleted result count :", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, deleteResult.Count());

            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(Admin_Resources.SuccessStoreLocatorDelete, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorStoreLocatorDelete, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                return false;
            }
        }
        /// <summary>
        /// Get SP Parameters 
        /// </summary>
        /// <param name="storeIds">StoresAddress ids/StoreLocationCode</param>
        /// <param name="isDeleteByStoreLocationCode">if true then store is deleted by storeLocationCode</param>
        /// <returns></returns>
        public virtual IZnodeViewRepository<View_ReturnBoolean> GetSPParameters(ParameterModel storeIds, bool isDeleteByStoreLocationCode)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            if (isDeleteByStoreLocationCode)
            {
                objStoredProc.SetParameter(ZnodePortalAddressEnum.StoreLocationCode.ToString(), storeIds.Ids, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter(ZnodePortalAddressEnum.PortalAddressId.ToString(), string.Empty, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            }
            else
            {
                objStoredProc.SetParameter(ZnodePortalAddressEnum.PortalAddressId.ToString(), storeIds.Ids, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter(ZnodePortalAddressEnum.StoreLocationCode.ToString(), string.Empty, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            return objStoredProc;
        }

        //Get store data for location.
        public virtual StoreLocatorDataModel GetStoreLocator(int storeId, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("storeId parameter: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, storeId);

            List<string> navigationProperties = GetExpands(expands);

            FilterCollection filter = new FilterCollection();
            filter.Add(ZnodePortalAddressEnum.PortalAddressId.ToString(), FilterOperators.Equals, storeId.ToString());

            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection());
            ZnodeLogging.LogMessage("WhereClause generated: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, whereClauseModel?.WhereClause);

            ZnodePortalAddress entity = _portalAddressRepository.GetEntity(whereClauseModel.WhereClause, navigationProperties);

            StoreLocatorDataModel model = MapPortalData(entity);

            if (IsNotNull(model))
            {
                model.MediaPath = GetMediaPath(model.MediaId);
                ZnodeLogging.LogMessage("MediaId and MediaPath properties of StoreLocatorDataModel: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { MediaId = model?.MediaId, MediaPath = model?.MediaPath } );
            }

            model.MapQuestURL = GetMapQuestURL(model, model.CountryName);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            return model;
        }

        //Get store data for location.
        public virtual StoreLocatorDataModel GetStoreLocator(string storeLocationCode, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { storeLocationCode = storeLocationCode });

            List<string> navigationProperties = GetExpands(expands);

            FilterCollection filter = new FilterCollection();
            filter.Add(ZnodePortalAddressEnum.StoreLocationCode.ToString(), ProcedureFilterOperators.Is, storeLocationCode.ToString());

            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection());
            ZnodeLogging.LogMessage("WhereClause generated: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, whereClauseModel?.WhereClause);

            ZnodePortalAddress entity = _portalAddressRepository.GetEntity(whereClauseModel.WhereClause, navigationProperties, whereClauseModel.FilterValues);

            StoreLocatorDataModel model = MapPortalData(entity);

            if (IsNotNull(model))
            {
                model.MediaPath = GetMediaPath(model.MediaId);
                ZnodeLogging.LogMessage("MediaId and MediaPath properties of StoreLocatorDataModel: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { MediaId = model?.MediaId, MediaPath = model?.MediaPath });
            }

            model.MapQuestURL = GetMapQuestURL(model, model.CountryName);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            return model;
        }
        public virtual bool IsCodeExists(HelperParameterModel parameterModel)
        {
            return _portalAddressRepository.Table.Any(a => a.StoreLocationCode.Equals(parameterModel.CodeField));
        }
        #endregion

        #region Private Methods
        //Method to get url for store location.
        private string GetMapQuestURL(StoreLocatorDataModel model, string countryName)
        {
            ZnodeLogging.LogMessage("Input parameter: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { countryName = countryName });
            string url = $"www.mapquest.com/maps/map.adp?country={countryName}&countryid={countryName}&addtohistory=&searchtab=address&searchtype=address";

            string mapquestbaseurl = $"http://{url}";
            StringBuilder MapQuestURL = new StringBuilder();
            MapQuestURL.Append(mapquestbaseurl);
            MapQuestURL.Append("&address=");
            MapQuestURL.Append(model.Address1);
            MapQuestURL.Append("&city=");
            MapQuestURL.Append(model.CityName);
            MapQuestURL.Append("&state=");
            MapQuestURL.Append(model.StateName);
            MapQuestURL.Append("&zipcode=");
            MapQuestURL.Append(model.PostalCode);

            return MapQuestURL.ToString();
        }

        //Method to get values.
        private List<string> GetExpands(NameValueCollection expands)
        {
            List<string> navigationProperties = new List<string>();
            if (expands != null && expands.HasKeys())
            {
                foreach (string key in expands.Keys)
                {
                    if (Equals(key, ZnodePortalAddressEnum.ZnodeAddress.ToString().ToLower())) SetExpands(ZnodePortalAddressEnum.ZnodeAddress.ToString(), navigationProperties);
                }
            }
            return navigationProperties;
        }

        //This method is used to get media path from media Id.
        private string GetMediaPath(int MediaId)
        {
            if (MediaId > 0)
            {
                MediaManagerModel mediaData = new MediaManagerServices().GetMediaByID(MediaId, null);
                return IsNotNull(mediaData) ? mediaData.MediaServerThumbnailPath : string.Empty;
            }
            return string.Empty;
        }

        //Map Portal Data.
        private StoreLocatorDataModel MapPortalData(ZnodePortalAddress entity)
        {
            StoreLocatorDataModel model = new StoreLocatorDataModel();
            if (IsNotNull(entity))
            {
                model.AddressId = entity.AddressId;
                model.Address1 = entity.ZnodeAddress?.Address1;
                model.Address2 = entity.ZnodeAddress?.Address2;
                model.Address3 = entity.ZnodeAddress?.Address3;
                model.PostalCode = entity.ZnodeAddress?.PostalCode;
                model.FaxNumber = entity.ZnodeAddress?.FaxNumber;
                model.PhoneNumber = entity.ZnodeAddress?.PhoneNumber;
                model.IsActive = entity.ZnodeAddress.IsActive;
                model.DisplayName = entity.ZnodeAddress?.DisplayName;
                model.CompanyName = entity.ZnodeAddress?.CompanyName;
                model.StateName = entity.ZnodeAddress?.StateName;
                model.CityName = entity.ZnodeAddress?.CityName;
                model.MediaId = entity.MediaId.GetValueOrDefault();
                model.CountryName = entity.ZnodeAddress?.CountryName;
                model.StoreName = entity.StoreName;
                model.DisplayOrder = entity.DisplayOrder;
                model.PortalId = entity.PortalId;
                model.PortalAddressId = entity.PortalAddressId;
                model.Latitude = entity.Latitude;
                model.Longitude = entity.Longitude;
                model.StoreLocationCode = entity.StoreLocationCode;
            }
            return model;
        }
        #endregion
    }
}
