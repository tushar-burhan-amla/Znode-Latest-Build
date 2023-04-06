using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class ECertCache : BaseCache, IECertCache
    {

        #region Private Variable

        private readonly IECertService _service;

        #endregion Private Variable

        #region Constructor

        public ECertCache(IECertService eCerService)
        {
            _service = eCerService;
        }

        #endregion Constructor

        #region Public methods
        //Get available certificates in wallet.
        public string GetECertificateList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ECertificateListModel list = _service.GetWalletECertificatesList(Filters, Sorts, Page);
                if (list?.ECertificates?.Count > 0)
                {
                    ECertificateListResponse response = new ECertificateListResponse { ECertificates = list.ECertificates };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }
        #endregion
    }
}
