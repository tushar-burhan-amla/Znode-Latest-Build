using System;

namespace Znode.Engine.Api.Models
{
    /// <summary>
    /// Product History Model.
    /// </summary>
    public class ProductHistoryModel : BaseModel
    {
        public int ProductHistoryId { get; set; }
        public int Version { get; set; }
        public string Author { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string ProductHistoryStatus { get; set; }
    }
}
