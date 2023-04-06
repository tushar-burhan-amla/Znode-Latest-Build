using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class TypeaheadListResponse : BaseListResponse
    {
        public List<TypeaheadResponseModel> Typeaheadlist { get; set; }
    }
}
