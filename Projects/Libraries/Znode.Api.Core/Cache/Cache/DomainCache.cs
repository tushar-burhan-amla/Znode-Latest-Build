using System;
using System.Web;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.Data.DataModel;

namespace Znode.Engine.Api.Cache
{
    public class DomainCache : BaseCache, IDomainCache
    {
        #region Private Member
        private readonly IDomainService _service;
        #endregion

        #region Constructor
        public DomainCache(IDomainService domainService)
        {
            _service = domainService;
        }
        #endregion

        #region Public Methods
        public virtual ZnodeDomain GetDomain(string routeUri)
        {
            ZnodeDomain data = (ZnodeDomain)HttpRuntime.Cache.Get(routeUri);
            if (Equals(data, null))
            {
                ZnodeDomain domain = _service.GetDomain(routeUri);
                if (!Equals(domain, null))
                    HttpRuntime.Cache.Insert(routeUri, domain, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromSeconds(20));
                else
                    domain = new ZnodeDomain();
                return domain;
            }
            return data;
        }

        public virtual string GetDomains(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                DomainListModel list = _service.GetDomains(Filters, Sorts, Page);
                if (list?.Domains?.Count > 0)
                {
                    DomainListResponse response = new DomainListResponse { Domains = list.Domains };
                    response.MapPagingDataFromModel(list);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetDomain(int domainId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);

            if (string.IsNullOrEmpty(data))
            {
                DomainModel domain = _service.GetDomain(domainId);
                if (!Equals(domain, null))
                {
                    DomainResponse response = new DomainResponse { Domain = domain };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}