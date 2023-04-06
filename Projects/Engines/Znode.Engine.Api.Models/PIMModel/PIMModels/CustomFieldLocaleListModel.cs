using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class CustomFieldLocaleListModel : BaseListModel
    {
        public List<CustomFieldLocaleModel> CustomFieldLocales { get; set; }
    }
}
