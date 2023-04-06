using Nest;

using System;
using System.Diagnostics;

using Znode.Libraries.Framework.Business;
using Znode.Libraries.Search;

using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Libraries.ElasticSearch
{
    public class CMSPageDefaultDataService : ICMSPageDefaultDataService
    {
        #region Protected Variables

        protected readonly ElasticClient elasticClient;
        protected readonly IElasticCMSPageSearchIndexerService elasticCMSPageSearchIndexerService;

        #endregion Protected Variables

        #region Constructor

        public CMSPageDefaultDataService()
        {
            ConnectionSettings settings = ElasticConnectionHelper.GetElasticConnectionSetting().DefaultMappingFor<SearchCMSPages>(m => m.IdProperty(p => p.indexid)).RequestTimeout(TimeSpan.FromMinutes(20));
            elasticClient = new ElasticClient(settings);

            //Elasticsearch implementation.
            elasticCMSPageSearchIndexerService = GetService<IElasticCMSPageSearchIndexerService>();
        }

        #endregion Constructor

        #region Public methods

        //Populate the default data.
        public virtual void IndexingDefaultData(string indexName, SearchCMSPagesParameterModel searchCmsPagesParameterModel)
        {
            try
            {
                //Call to Create index in elastic search.
                elasticCMSPageSearchIndexerService.CreateIndex(elasticClient, indexName, searchCmsPagesParameterModel);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Checks if the index exists on the local system.
        public virtual bool IsIndexExists(string indexName)
          => elasticCMSPageSearchIndexerService.IsIndexExists(elasticClient, indexName);

        //Rename the index name
        public virtual bool RenameCmsPageIndex(int cmsSearchIndexId, string oldIndexName, string newIndexName)
        {
            return elasticCMSPageSearchIndexerService.RenameIndexData(elasticClient, cmsSearchIndexId, oldIndexName, newIndexName);
        }

        //Populate the default data.
        public virtual bool DeleteCmsPagesDataByRevisionType(string indexName, string revisionType, long indexStartTime)
        {
            return elasticCMSPageSearchIndexerService.DeleteCmsPagesDataByRevisionType(elasticClient, indexName, revisionType, indexStartTime);
        }

        #endregion
    }
}
