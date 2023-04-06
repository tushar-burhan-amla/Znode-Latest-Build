using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class LocaleListModel : BaseListModel
    {
        public List<LocaleModel> Locales { get; set; }

        public LocaleListModel()
        {
            Locales = new List<LocaleModel>();
        }
    }
}
