using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models
{
    public class PortalSEOSettingModel : BaseModel
    {        
        public int CMSPortalSEOSettingId { get; set; }
        [Required]
        public int PortalId { get; set; }
        [MaxLength(100)]
        public string CategoryTitle { get; set; }
        [MaxLength(100)]
        public string CategoryDescription { get; set; }
        [MaxLength(100)]
        public string CategoryKeyword { get; set; }
        [MaxLength(100)]
        public string ProductTitle { get; set; }
        [MaxLength(100)]
        public string ProductDescription { get; set; }
        [MaxLength(100)]
        public string ProductKeyword { get; set; }
        [MaxLength(100)]
        public string ContentTitle { get; set; }
        [MaxLength(100)]
        public string ContentDescription { get; set; }
        [MaxLength(100)]
        public string ContentKeyword { get; set; }
    }
}
