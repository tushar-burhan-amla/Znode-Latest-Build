using Znode.Engine.Api.Models;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Libraries.Admin
{
    public class SearchHelper
    {
        private readonly IZnodeRepository<ZnodeSearchIndexServerStatu> _searchIndexServerStatusRepository;
        private readonly IZnodeRepository<ZnodeCMSSearchIndexServerStatu> _cmsSearchIndexServerStatusRepository;

        public SearchHelper()
        {
            _searchIndexServerStatusRepository = new ZnodeRepository<ZnodeSearchIndexServerStatu>();
            _cmsSearchIndexServerStatusRepository = new ZnodeRepository<ZnodeCMSSearchIndexServerStatu>();
        }

        //Create search index server status.
        public virtual SearchIndexServerStatusModel CreateSearchIndexServerStatus(SearchIndexServerStatusModel searchIndexStatusModel)
        {
            if (HelperUtility.IsNotNull(searchIndexStatusModel))
                searchIndexStatusModel = _searchIndexServerStatusRepository.Insert(searchIndexStatusModel.ToEntity<ZnodeSearchIndexServerStatu>()).ToModel<SearchIndexServerStatusModel>();

            return searchIndexStatusModel;
        }

        //Update search index server status.
        public virtual bool UpdateSearchIndexServerStatus(SearchIndexServerStatusModel searchIndexStatusModel)
        {
            if (searchIndexStatusModel?.SearchIndexServerStatusId > 0)
                return _searchIndexServerStatusRepository.Update(searchIndexStatusModel.ToEntity<ZnodeSearchIndexServerStatu>());

            return false;
        }

        //Create CMS page search index server status.
        public virtual CMSSearchIndexServerStatusModel CreateCmsPageSearchIndexServerStatus(CMSSearchIndexServerStatusModel cmsSearchIndexStatusModel)
        {
            if (HelperUtility.IsNotNull(cmsSearchIndexStatusModel))
                cmsSearchIndexStatusModel = _cmsSearchIndexServerStatusRepository.Insert(cmsSearchIndexStatusModel.ToEntity<ZnodeCMSSearchIndexServerStatu>()).ToModel<CMSSearchIndexServerStatusModel>();

            return cmsSearchIndexStatusModel;
        }

        //Update CMS page search index server status.
        public virtual bool UpdateCmsPageSearchIndexServerStatus(CMSSearchIndexServerStatusModel cmsSearchIndexStatusModel)
        {
            if (cmsSearchIndexStatusModel?.CMSSearchIndexServerStatusId > 0)
                return _cmsSearchIndexServerStatusRepository.Update(cmsSearchIndexStatusModel.ToEntity<ZnodeCMSSearchIndexServerStatu>());

            return false;
        }
    }
}
