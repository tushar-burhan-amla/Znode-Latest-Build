using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class GlobalAttributeFamilyListResponse : BaseListResponse
    {
        public List<GlobalAttributeFamilyModel> AttributeFamily { get; set; }

        public GlobalAttributeFamilyLocaleListModel AttributeFamilyLocales { get; set; }
    }
}
