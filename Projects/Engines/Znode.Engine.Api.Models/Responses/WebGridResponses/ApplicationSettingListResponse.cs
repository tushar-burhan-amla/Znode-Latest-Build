using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ApplicationSettingListResponse : BaseListResponse
    {
        public List<ApplicationSettingDataModel> List { get; set; }
        public List<EntityColumnModel> ColumnList { get; set; }

        public ViewModel View { get; set; }
        public bool CreateStatus { get; set; }

        public ApplicationSettingListResponse()
        {
            ColumnList = new List<EntityColumnModel>();
        }
    }
}
