using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class CultureListModel : BaseListModel
    {
        public List<CultureModel> Culture { get; set; }

        public CultureListModel()
        {
            Culture = new List<CultureModel>();
        }
    }
}
