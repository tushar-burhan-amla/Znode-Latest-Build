using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class DateFormatListResponse : BaseListResponse
    {
        public List<DateFormatModel> DateFormats { get; set; }
    }
}
