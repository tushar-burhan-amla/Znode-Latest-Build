using Autofac;

using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.Search
{
    public class DependencyRegistration : IDependencyRegistration
    {
        public virtual void Register(ContainerBuilder builder)
        {
            #region FullText Elasticsearch

            builder.RegisterType<SearchProductService>().As<ISearchProductService>().InstancePerRequest();
            builder.RegisterType<SearchCategoryService>().As<ISearchCategoryService>().InstancePerRequest();

            #endregion FullText Elasticsearch

            #region CMS Elasticsearch

            builder.RegisterType<CMSContentPageSearchService>().As<ICMSContentPageSearchService>().InstancePerRequest();

            #endregion CMS Elasticsearch
        }

        public int Order
        {
            get { return 0; }
        }
    }
}
