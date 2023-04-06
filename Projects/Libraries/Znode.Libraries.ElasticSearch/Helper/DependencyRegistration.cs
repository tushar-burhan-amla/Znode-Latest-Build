using Autofac;

using Znode.Libraries.Framework.Business;
using Znode.Libraries.Search;

namespace Znode.Libraries.ElasticSearch
{
    public class DependencyRegistration : IDependencyRegistration
    {
        public virtual void Register(ContainerBuilder builder)
        {
            #region FullText Search 

            #region Elasticsearch Provider

            builder.RegisterType<ElasticSearchProviderService>().As<IZnodeSearchProvider>().InstancePerRequest();
            builder.RegisterType<ElasticSearchRequest>().As<IZnodeSearchRequest>().InstancePerRequest();
            builder.RegisterType<ElasticSuggestionsService>().As<IElasticSuggestionsService>().InstancePerRequest();
            builder.RegisterType<ElasticSearchBaseService>().As<IElasticSearchBaseService>().InstancePerRequest();

            #endregion Elasticsearch Provider

            #region Indexer

            builder.RegisterType<ElasticSearchIndexerService>().As<IElasticSearchIndexerService>().InstancePerRequest();
            builder.RegisterType<DefaultDataService>().As<IDefaultDataService>().InstancePerRequest();

            #endregion Indexer

            #region Search Query

            builder.RegisterType<BaseQuery>().As<IBaseQuery>().InstancePerRequest();
            builder.RegisterType<MatchPhrasePrefixQueryBuilder>().As<ISearchQuery>().InstancePerRequest();
            builder.RegisterType<MatchPhraseQueryBuilder>().As<ISearchQuery>().InstancePerRequest();
            builder.RegisterType<MatchQueryBuilder>().As<ISearchQuery>().InstancePerRequest();
            builder.RegisterType<MultiMatchQueryBuilder>().As<ISearchQuery>().InstancePerRequest();

            #endregion Search Query

            #endregion FullText Search 

            #region CMS Search 

            #region Elasticsearch Provider

            builder.RegisterType<ElasticCMSPageSearchService>().As<IElasticCMSPageSearchService>().InstancePerRequest();
            builder.RegisterType<ElasticCMSPageSearchRequest>().As<IZnodeCMSPageSearchRequest>().InstancePerRequest();
            builder.RegisterType<CountQuery>().As<ICountQuery>().InstancePerRequest();

            #endregion Elasticsearch Provider

            #region Indexer

            builder.RegisterType<ElasticCMSPageSearchIndexerService>().As<IElasticCMSPageSearchIndexerService>().InstancePerRequest();
            builder.RegisterType<CMSPageDefaultDataService>().As<ICMSPageDefaultDataService>().InstancePerRequest();

            #endregion Indexer

            #endregion CMS Search 

            #region Elasticsearch helper
            builder.RegisterType<ZnodeSearchIndexSettingHelper>().As<IZnodeSearchIndexSettingHelper>().InstancePerRequest();
            #endregion
        }

        public int Order
        {
            get { return 0; }
        }
    }

}
