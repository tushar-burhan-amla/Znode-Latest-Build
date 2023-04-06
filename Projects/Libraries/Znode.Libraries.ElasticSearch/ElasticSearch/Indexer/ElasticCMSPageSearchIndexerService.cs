using Nest;

using System;
using System.Collections.Generic;
using System.Linq;

using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.Admin;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Search;

using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Libraries.ElasticSearch
{
    public class ElasticCMSPageSearchIndexerService : IElasticCMSPageSearchIndexerService
    {
        #region Protected Variables
        protected readonly IZnodeSearchIndexSettingHelper _searchIndexSettingHelper;
        #endregion

        #region Public Properties      

        public int PageLength { get; set; }

        #endregion Public Properties

        #region Public constructor

        public ElasticCMSPageSearchIndexerService()
        {
            _searchIndexSettingHelper = GetService<IZnodeSearchIndexSettingHelper>();
        }

        public ElasticCMSPageSearchIndexerService(int pageLength = 1000)
        {
            PageLength = pageLength;
            _searchIndexSettingHelper = GetService<IZnodeSearchIndexSettingHelper>();
        }
        #endregion Public constructor

        #region Public methods

        //Checks if the index exists.
        public virtual bool IsIndexExists(ElasticClient elasticClient, IndexName indexName)
        {
            ExistsResponse result = elasticClient.Indices.Exists(indexName);
            return result.Exists;
        }

        //Rename the index
        public virtual bool RenameIndexData(ElasticClient elasticClient, int cmsPagesIndexId, string oldIndexName, string newIndexName)
        {
            if (IsIndexExists(elasticClient, oldIndexName))
            {
                ReindexOnServerResponse reindexResponse = elasticClient.ReindexOnServer(r => r
                                          .Source(s => s
                                              .Index(oldIndexName)
                                           )
                                          .Destination(d => d
                                              .Index(oldIndexName)
                                          )
                                          .WaitForCompletion(true)
                                      );

                DeleteIndexResponse deleteIndex = elasticClient.Indices.Delete(oldIndexName);
                return deleteIndex.IsValid;
            }
            else
                return false;
        }

        //Create index for the CMS content pages.
        public virtual void CreateIndex(ElasticClient elasticClient, IndexName indexName, SearchCMSPagesParameterModel searchCmsPagesParameterModel)
        {

            SearchHelper searchHelper = new SearchHelper();
            if (!IsIndexExists(elasticClient, indexName))
            {
                CreateIndexDescriptor createIndexDescriptor = AddPagesDescriptor(elasticClient, indexName, searchCmsPagesParameterModel.PortalId);
                CreateIndexResponse indexResponse = elasticClient.Indices.Create(indexName, i => createIndexDescriptor);

                if (!indexResponse.IsValid)
                {
                    searchHelper.UpdateCmsPageSearchIndexServerStatus(new CMSSearchIndexServerStatusModel() { CMSSearchIndexServerStatusId = searchCmsPagesParameterModel.CMSSearchIndexServerStatusId, CMSSearchIndexMonitorId = searchCmsPagesParameterModel.CMSSearchIndexMonitorId, ServerName = Environment.MachineName, Status = ZnodeConstant.SearchIndexFailedStatus });
                    throw new ZnodeException(null, $"Create CMS Pages Index failed. {indexResponse.OriginalException.Message}");
                }
            }

            ICMSContentPageSearchService cmsContentPageSearchService = GetService<ICMSContentPageSearchService>();

            List<ZnodePublishWebstoreEntity> webstoreEntities = cmsContentPageSearchService.GetVersionIds(searchCmsPagesParameterModel.PortalId, searchCmsPagesParameterModel.RevisionType, searchCmsPagesParameterModel.ActiveLocales);

            List<SearchCMSPages> searchBlogNews = cmsContentPageSearchService.GetAllBlogsNews(searchCmsPagesParameterModel);

            List<SearchCMSPages> pagedAllCmsPages = cmsContentPageSearchService.GetAllCMSPagesOfAllVersionAndLocales(searchCmsPagesParameterModel, webstoreEntities);

            if (searchBlogNews?.Count > 0 && pagedAllCmsPages?.Count > 0)
            {
                pagedAllCmsPages.AddRange(searchBlogNews);
            }
            
            foreach (ZnodePublishWebstoreEntity webStoreEntity in webstoreEntities)
            {
                int start = 0;
                decimal totalPages;
                bool continueIndex = true;
                                
                int chunkSize = Convert.ToInt32(ZnodeApiSettings.SearchIndexChunkSize);              

                if (webStoreEntity.VersionId > 0)
                {
                    do
                    {
                        List<SearchCMSPages> pagedCmsPages = GetPagedCMSPagesListByVersionId(pagedAllCmsPages, webStoreEntity.VersionId, start, chunkSize, out totalPages);

                        pagedCmsPages?.ForEach(x => x.revisionType = searchCmsPagesParameterModel.RevisionType);

                        if (start < totalPages)
                        {
                            if (pagedCmsPages?.Count > 0)
                            {
                                BulkResponse indexResponse = elasticClient.IndexMany<dynamic>(pagedCmsPages, indexName);
                                if (!indexResponse.IsValid)
                                {
                                    searchHelper.UpdateCmsPageSearchIndexServerStatus(new CMSSearchIndexServerStatusModel() { CMSSearchIndexServerStatusId = searchCmsPagesParameterModel.CMSSearchIndexServerStatusId, CMSSearchIndexMonitorId = searchCmsPagesParameterModel.CMSSearchIndexMonitorId, ServerName = Environment.MachineName, Status = ZnodeConstant.SearchIndexFailedStatus });
                                    throw new ZnodeException(null, $"Create CMS pages Index failed. {indexResponse.ItemsWithErrors?.FirstOrDefault()?.Error?.CausedBy?.Reason}");
                                }
                                //Save InProgress status for the server name.
                                searchHelper.UpdateCmsPageSearchIndexServerStatus(new CMSSearchIndexServerStatusModel() { CMSSearchIndexServerStatusId = searchCmsPagesParameterModel.CMSSearchIndexServerStatusId, CMSSearchIndexMonitorId = searchCmsPagesParameterModel.CMSSearchIndexMonitorId, ServerName = Environment.MachineName, Status = ZnodeConstant.SearchIndexInProgressStatus });
                            }
                            start++;
                        }
                        else
                            continueIndex = false;
                    } while (continueIndex);

                    elasticClient.Indices.Refresh(indexName);
                }
                else
                {
                    searchHelper.UpdateCmsPageSearchIndexServerStatus(new CMSSearchIndexServerStatusModel() { CMSSearchIndexServerStatusId = searchCmsPagesParameterModel.CMSSearchIndexServerStatusId, CMSSearchIndexMonitorId = searchCmsPagesParameterModel.CMSSearchIndexMonitorId, ServerName = Environment.MachineName, Status = ZnodeConstant.SearchIndexFailedStatus });
                    throw new ZnodeException(ErrorCodes.StoreNotPublished, "Please publish the selected store to create the index.");
                }
            }
            //Save complete status for the server name.
            searchHelper.UpdateCmsPageSearchIndexServerStatus(new CMSSearchIndexServerStatusModel() { CMSSearchIndexServerStatusId = searchCmsPagesParameterModel.CMSSearchIndexServerStatusId, CMSSearchIndexMonitorId = searchCmsPagesParameterModel.CMSSearchIndexMonitorId, ServerName = Environment.MachineName, Status = ZnodeConstant.SearchIndexCompleteStatus });
        }

        //Get pages list by version id.
        public virtual List<SearchCMSPages> GetPagedCMSPagesListByVersionId(List<SearchCMSPages> pagedAllCmsPages, int versionId, int start, int pageLength, out decimal totalPages)
        {
            int totalCount = 0;

            if (pagedAllCmsPages != null)
            {               
                List<SearchCMSPages> pagedPages = pagedAllCmsPages.Where(m => m.versionid == versionId).ToList();

                if(HelperUtility.IsNotNull(pagedPages))
                {
                    totalCount = pagedPages != null ? pagedPages.Count : 0;

                    pagedPages = pagedPages?.Skip(start * pageLength).Take(pageLength).ToList();
                }              
               
                if (totalCount < pageLength)
                    totalPages = 1;
                else
                    totalPages = Math.Ceiling((decimal)totalCount / pageLength);

                return pagedPages;
            }
            else
            {
                totalPages = 0;
            }
            return new List<SearchCMSPages>();
        }

        //Return the CMS pages descriptor.
        public virtual CreateIndexDescriptor AddPagesDescriptor(ElasticClient elasticClient, IndexName indexName, int portalId)
        {
            CreateIndexDescriptor pagesDescriptor = GetCreateIndexDescriptor(indexName);
            return pagesDescriptor;
        }

        //Delete CMS pages from search index
        public virtual bool DeleteCmsPagesDataByRevisionType(ElasticClient elasticClient, IndexName indexName, string revisionType, long indexStartTime)
        {
            //Delete request for delete documents which does not match to current create index time and revisiontype.
            DeleteByQueryRequest request = new DeleteByQueryRequest<dynamic>(indexName)
            {
                Query = new QueryContainer(new BoolQuery
                {
                    Must = new QueryContainer[] { new TermQuery { Field = "revisionType", Value = revisionType.ToLower() } },
                    MustNot = new QueryContainer[] { new TermQuery { Field = "timestamp", Value = indexStartTime.ToString() } },
                }
                )
            };

            DeleteByQueryResponse result = elasticClient.DeleteByQuery(request);
            bool isValid = result?.IsValid ?? false;

            return isValid;
        }


        public virtual CreateIndexDescriptor ConfigureIndex(IndexName indexName, IndexSettings settings)
        {
            return new CreateIndexDescriptor(indexName)
           .InitializeUsing(new IndexState() { Settings = settings })
           .Mappings(
               mappings => mappings.Map<dynamic>
                ("_default_", s => s.DateDetection(false).
               DynamicTemplates(dts => dts.DynamicTemplate("strings", dt => dt.Match("*")
               .MatchMappingType("string").Mapping(t => t.Text(x => x.Analyzer("standard_ngram_analyzer").SearchAnalyzer("standard_search_synonym_analyzer")))))));
        }

        //Set Analyzer for search settings
        public virtual void SetAnalyzers(List<string> tokenFilters, IndexSettings indexsettings)
        {
            SetSearchTimeAnalyzers(tokenFilters, indexsettings);
            SetIndexTimeAnalyzers(tokenFilters, indexsettings);
        }

        //Set search time analyzers for index.
        public virtual void SetSearchTimeAnalyzers(List<string> tokenFilters, IndexSettings indexsettings)
        {
            indexsettings.Analysis.Analyzers.Add("standard_search_synonym_analyzer", _searchIndexSettingHelper.SetCustomAnalyzer("standard", tokenFilters));
        }

        //Set index time analyzers for index.
        public virtual void SetIndexTimeAnalyzers(List<string> tokenFilters, IndexSettings indexsettings)
        {
            List<string> ngramAnalyzerFilter = tokenFilters.ToList();
            ngramAnalyzerFilter.Insert(ngramAnalyzerFilter.FindIndex(x => x == "edgeNgramTokenFilter"), ZnodeConstant.NGramTokenFilter);
            indexsettings.Analysis.Analyzers.Add("standard_ngram_analyzer", _searchIndexSettingHelper.SetCustomAnalyzer("standard", ngramAnalyzerFilter, new List<string> { "my_char_filter" }));
        }

        //Set filter for analyzers
        public virtual void SetFilterForAnalyzers(List<string> tokenFilters, IndexSettings indexsettings)
        {
            _searchIndexSettingHelper.SetNGramTokenFilter(indexsettings, null);

            _searchIndexSettingHelper.SetEdgeNGramFilter(tokenFilters, indexsettings);

            _searchIndexSettingHelper.SetShingleTokenFilter(tokenFilters, indexsettings);

            _searchIndexSettingHelper.SetHtmlStripCharacterFilter(indexsettings);
        }

        //Create create index descriptor.
        public virtual CreateIndexDescriptor GetCreateIndexDescriptor(IndexName indexName)
        {
            IndexSettings settings = CreateIndexSettings();

            return ConfigureIndex(indexName, settings);
        }

        //Get create index descriptor.
        public virtual IndexSettings CreateIndexSettings()
        {
            //Get default token filters.
            List<string> tokenFilters = ZnodeApiSettings.DefaultTokenFilters.Split(',').ToList();

            //create analysis object
            IndexSettings indexsettings = new IndexSettings
            {
                Analysis = new Analysis
                {
                    TokenFilters = new TokenFilters(),
                    Analyzers = new Analyzers(),
                    Tokenizers = new Tokenizers(),
                    CharFilters = new CharFilters()
                }
            };
            //The default difference between MinNGram and MaxNGram is set as 1 to set max difference need to add difference in index setting. 
            indexsettings.Add(UpdatableIndexSettings.MaxNGramDiff, 39);

            //The default difference between MaxShingleDiff and MaxShingleDiff is set as 1 to set max difference need to add difference in index setting. 
            indexsettings.Add(UpdatableIndexSettings.MaxShingleDiff, 10);

            SetFilterForAnalyzers(tokenFilters, indexsettings);

            _searchIndexSettingHelper.SetTokenizers(indexsettings, null);

            SetAnalyzers(tokenFilters, indexsettings);

            return indexsettings;
        }
    }

    #endregion
}
