using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class DashboardDropDownListResponse : BaseListResponse
    {
        public List<DashboardDropDownModel> DashboardDropDown { get; set; }
    }
}
