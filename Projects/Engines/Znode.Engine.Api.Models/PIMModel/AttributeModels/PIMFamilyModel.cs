using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models
{
    public class PIMFamilyModel : BaseModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int PIMAttributeFamilyId { get; set; }
        public int LocaleId { get; set; }
        public bool IsCategory { get; set; }
    }
}
