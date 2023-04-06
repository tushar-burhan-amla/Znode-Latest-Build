using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ManageMessageListModel : BaseListModel
    {
        public List<ManageMessageModel> ManageMessages { get; set; }
        public List<LocaleModel> Locale { get; set; }
    }
}
