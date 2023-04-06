using System;

namespace Znode.Engine.Api.Models
{
    public class ReportCategoryModel : BaseModel
    {
        public int ReportCategoryId { get; set; }
        public string CategoryName { get; set; }
        public Boolean IsActive { get; set; }
    }
}
