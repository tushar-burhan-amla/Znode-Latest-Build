using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models
{
    public class ParameterModelForWidgetCategory : BaseModel
    {
        /// <summary>
        /// This helps to pass in query parameter (Comma seperated string)
        /// </summary>
        [Required]
        public string CategoryCodes { get; set; }
        public string PublishCategoryId { get; set; }
        public int CMSWidgetsId { get; set; }
        public string WidgetsKey { get; set; }
        public int CMSMappingId { get; set; }
        public string TypeOFMapping { get; set; }
    }
}
