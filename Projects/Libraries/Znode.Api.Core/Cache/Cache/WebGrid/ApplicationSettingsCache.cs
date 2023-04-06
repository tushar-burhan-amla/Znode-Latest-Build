using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Services;
using System;

namespace Znode.Engine.Api.Cache
{
    public class ApplicationSettingsCache : BaseCache, IApplicationSettingsCache
    {
        private readonly IApplicationSettingsService _service;

        public ApplicationSettingsCache(IApplicationSettingsService configurationReaderService)
        {
            _service = configurationReaderService;
        }

        #region Public Methods
        public virtual string GetFilterConfigurationXML(string itemName,  string routeUri, string routeTemplate, int? userId = null)
        {
            //Check data exist in cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from service
                var filterxml = _service.GetFilterConfigurationXML(itemName,userId);
                if (!Equals(filterxml, null))
                {
                    //Create Response and insert in to cache
                    ApplicationSettingsResponse response = new ApplicationSettingsResponse { FilterXML = filterxml.Setting, XMLSetting = filterxml };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        #endregion

        #region XML Editor Public Methods

        //Get ApplicationSettings as string
        public virtual string GetApplicationSettings(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                var list = _service.GetApplicationSettings(Expands, Filters, Sorts, Page);
                if (list.ApplicationSettingList.Count > 0)
                {
                    ApplicationSettingListResponse response = new ApplicationSettingListResponse { List = list.ApplicationSettingList };
                    response.MapPagingDataFromModel(list);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }

            return data;
        }

        //Get ColumnList as string
        public virtual string GetColumnList(string routeUri, string routeTemplate, string entityType, string entityName)
        {
            //get data from cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                var list = _service.GetColumnList(entityType, entityName);
                if (!Equals(list, null))
                {
                    ApplicationSettingListResponse response = new ApplicationSettingListResponse { ColumnList = list.ColumnListList };

                    //insert data to cache
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }

            return data;
        }

        /// <summary>
        /// Get view by itemViewId
        /// </summary>
        /// <param name="itemViewId"></param>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns></returns>
        public virtual string GetView(int itemViewId, string routeUri, string routeTemplate)
        {
            //get data from cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                var result = _service.GetView(itemViewId);
                if (!Equals(result, null))
                {
                    ApplicationSettingListResponse response = new ApplicationSettingListResponse { View = result };

                    //insert data to cache
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}