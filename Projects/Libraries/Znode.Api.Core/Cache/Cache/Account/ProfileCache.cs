using Znode.Engine.Services;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;

namespace Znode.Engine.Api.Cache
{
    public class ProfileCache : BaseCache, IProfileCache
    {
        #region Global Variable
        private readonly IProfileService _service;
        #endregion

        #region Default Constructor
        public ProfileCache(IProfileService profileService)
        {
            _service = profileService;
        }
        #endregion

        #region Public Methods
        #region Profile
        //Get profile by profile Id.
        public virtual string GetProfile(int profileId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get profile by profile id.
                ProfileModel profile = _service.GetProfile(profileId);
                if (!Equals(profile, null))
                {
                    ProfileResponse response = new ProfileResponse { Profile = profile };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }

            return data;
        }

        //Get profile list.
        public virtual string GetProfiles(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //profile list
                ProfileListModel profileList = _service.GetProfileList(Expands, Filters, Sorts, Page);
                if (profileList?.Profiles?.Count > 0)
                {
                    //Get response and insert it into cache.
                    ProfileListResponse response = new ProfileListResponse { Profiles = profileList.Profiles };
                    response.MapPagingDataFromModel(profileList);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion

        //Get profile catalog list.
        public virtual string GetProfileCatalogs(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //profile catalog list.
                ProfileCatalogListModel profileList = _service.GetProfileCatalogList(Expands, Filters, Sorts, Page);
                if (profileList?.ProfileCatalogs?.Count > 0)
                {
                    //Get response and insert it into cache.
                    ProfileCatalogListResponse response = new ProfileCatalogListResponse { ProfileCatalogs = profileList.ProfileCatalogs };
                    response.MapPagingDataFromModel(profileList);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}