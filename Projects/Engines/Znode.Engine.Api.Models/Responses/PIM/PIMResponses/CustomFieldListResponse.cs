using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class CustomFieldListResponse : BaseListResponse
    {
        public List<CustomFieldModel> CustomFields { get; set; }
    }
}
