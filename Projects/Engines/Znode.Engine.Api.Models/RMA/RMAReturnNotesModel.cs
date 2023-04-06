namespace Znode.Engine.Api.Models
{
    public class RMAReturnNotesModel : BaseModel
    {
        public int RmaReturnNotesId { get; set; }
        public int RmaReturnDetailsId { get; set; }
        public string Notes { get; set; }
        public string UserEmailId { get; set; }
    }
}