using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static System.String;

namespace Znode.Engine.Api.Cache
{
    public class PortalProfileCache : BaseCache, IPortalProfileCache
    {
        #region Private Variable
        private readonly IPortalProfileService _service;
        #endregion

        #region Constructor
        public PortalProfileCache(IPortalProfileService portalProfileService)
        {
            _service = portalProfileService;
        }
        #endregion

        #region Public Methods
        public virtual string GetPortalProfiles(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (IsNullOrEmpty(data))
            {
                PortalProfileListModel list = _service.GetPortalProfiles(Expands, Filters, Sorts, Page);
                if (list?.PortalProfiles?.Count > 0)
                {
                    PortalProfileListResponse response = new PortalProfileListResponse { PortalProfiles = list.PortalProfiles };
                    response.MapPagingDataFromModel(list);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetPortalProfile(int portalProfileId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (IsNullOrEmpty(data))
            {
                PortalProfileModel portalProfile = _service.GetPortalProfile(portalProfileId, Expands);
                if (!Equals(portalProfile, null))
                {
                    PortalProfileResponse response = new PortalProfileResponse { PortalProfile = portalProfile };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}