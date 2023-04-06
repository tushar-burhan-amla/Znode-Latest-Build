using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ApplicationSettingListModel : BaseListModel
    {
        public List<ApplicationSettingDataModel> ApplicationSettingList { get; set; }

        public ApplicationSettingListModel()
        {
            ApplicationSettingList = new List<ApplicationSettingDataModel>();
        }
    }
}