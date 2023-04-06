using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class CustomFieldListModel : BaseListModel
    {
        public List<CustomFieldModel> CustomFields{ get; set; }

        public CustomFieldListModel()
        {
            CustomFields = new List<CustomFieldModel>();
        }
    }
}
