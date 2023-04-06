using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PublishProductListModel : BaseListModel
    {
        public List<PublishProductModel> PublishProducts { get; set; }
        public List<LocaleModel> Locale { get; set; }
        public List<WebStoreConfigurableProductModel> ConfigurableProducts { get; set; }
    }
}
