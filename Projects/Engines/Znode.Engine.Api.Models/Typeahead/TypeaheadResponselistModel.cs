using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class TypeaheadResponselistModel : BaseListModel
    {
        public List<TypeaheadResponseModel> Typeaheadlist { get; set; }
    }
}
