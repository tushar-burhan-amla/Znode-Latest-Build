using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models
{
    public class PIMGetProductModel
    {
        [Required]
        public int ProductId { get; set; }
        public int FamilyId { get; set; }
        public bool IsCopy { get; set; }
        public int LocaleId { get; set; }
    }
}
