using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class LocaleListResponse : BaseListResponse
    {
        public List<LocaleModel> Locales { get; set; }
    }
}
