using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class DateFormatListModel : BaseListModel
    {
        public List<DateFormatModel> DateFormats { get; set; }

        public DateFormatListModel()
        {
            DateFormats = new List<DateFormatModel>();
        }
    }
}
