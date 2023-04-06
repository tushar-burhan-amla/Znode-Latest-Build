using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models
{
    public class GenerateImageModel : BaseModel
    {
        public int MediaId { get; set; }
        [Required]
        public string Path { get; set; }
    }
}
