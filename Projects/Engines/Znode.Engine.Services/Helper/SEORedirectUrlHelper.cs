using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Services
{
    public static class SEORedirectUrlHelper
    {
        private static readonly IZnodeRepository<ZnodeCMSUrlRedirect> _urlRedirectRepository = new ZnodeRepository<ZnodeCMSUrlRedirect>();

        //Create new URL redirect if isRedirect is true.
        public static void CreateUrlRedirect(SEODetailsModel seoDetailsModel)
        {
            ZnodeCMSUrlRedirect urlRedirectPresentWithOldSeoUrl = _urlRedirectRepository.Table.Where(urlRedirect => urlRedirect.RedirectTo == seoDetailsModel.OldSEOURL).Select(urlRedirect => urlRedirect).FirstOrDefault();
            if (seoDetailsModel.IsRedirect.GetValueOrDefault())
            {
                if (!string.IsNullOrEmpty(seoDetailsModel.OldSEOURL) && !string.IsNullOrEmpty(seoDetailsModel.SEOUrl) && !Equals(seoDetailsModel.OldSEOURL, seoDetailsModel.SEOUrl))
                {
                    UpdateExistingURLRedirectData(urlRedirectPresentWithOldSeoUrl);
                    CreateURLRedirect(MapURLRedirectData(seoDetailsModel));
                }

                if (string.IsNullOrEmpty(seoDetailsModel.OldSEOURL) && !string.IsNullOrEmpty(seoDetailsModel.SEOUrl))
                {
                    //3 is CMSSEOTypeId for ContentPage.
                    if (Equals(seoDetailsModel.CMSSEOTypeId, 3))
                        seoDetailsModel.OldSEOURL = $"contentpage/{seoDetailsModel.CMSContentPagesId}";

                    UpdateExistingURLRedirectData(urlRedirectPresentWithOldSeoUrl);
                    CreateURLRedirect(MapURLRedirectData(seoDetailsModel));
                }

                if (!string.IsNullOrEmpty(seoDetailsModel.OldSEOURL) && string.IsNullOrEmpty(seoDetailsModel.SEOUrl))
                    UpdateExistingURLRedirectData(urlRedirectPresentWithOldSeoUrl);
            }
            else {
                if (!string.IsNullOrEmpty(seoDetailsModel.OldSEOURL) && string.IsNullOrEmpty(seoDetailsModel.SEOUrl))
                    UpdateExistingURLRedirectData(urlRedirectPresentWithOldSeoUrl);
            }
        }

        //If old url already exists update its URL redirect data.
        private static void UpdateExistingURLRedirectData(ZnodeCMSUrlRedirect urlRedirectPresentWithOldSeoUrl)
        {
            if (HelperUtility.IsNotNull(urlRedirectPresentWithOldSeoUrl))
            {
                urlRedirectPresentWithOldSeoUrl.IsActive = false;
                _urlRedirectRepository.Update(urlRedirectPresentWithOldSeoUrl);
            }
        }

        //Map URL redirect from SEODetailsModel to UrlRedirectModel to save new data for URL redirect.
        private static UrlRedirectModel MapURLRedirectData(SEODetailsModel seoDetailsModel)
            => new UrlRedirectModel()
            {
                IsActive = true,
                RedirectFrom = seoDetailsModel.OldSEOURL,
                RedirectTo = seoDetailsModel.SEOUrl,
                PortalId = seoDetailsModel.PortalId
            };

        //Create new data for URL redirect.
        private static void CreateURLRedirect(UrlRedirectModel urlRedirectModel)
        {
            if (_urlRedirectRepository.Insert(urlRedirectModel.ToEntity<ZnodeCMSUrlRedirect>())?.CMSUrlRedirectId < 1)
            {
                ZnodeLogging.LogMessage("Failed to create URL Redirect.", ZnodeLogging.Components.CMS.ToString());
                throw new ZnodeException(ErrorCodes.URLRedirectCreationFailed, "Failed to create URL Redirect.");
            }
            ZnodeLogging.LogMessage("URL Redirect Created Successfully.", ZnodeLogging.Components.CMS.ToString());
        }
    }
}
