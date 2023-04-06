using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class WebStoreLinkProductModel : BaseModel
    {
        public string AttributeName { get; set; }
        public List<PublishProductModel> PublishProduct { get; set; }

        public WebStoreLinkProductModel()
        {
            PublishProduct = new List<PublishProductModel>();
        }
    }
}
