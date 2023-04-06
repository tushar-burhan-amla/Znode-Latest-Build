namespace Znode.Engine.Api.Models
{
    public class WebStoreBlogNewsCommentModel : BaseModel
    {
        public int BlogNewsId { get; set; }
        public int PortalId { get; set; }
        public int LocaleId { get; set; }
        public int BlogNewsCommentId { get; set; }
        public int? UserId { get; set; }
        public int BlogNewsCommentLocaleId { get; set; }
        public int CountComments { get; set; }

        public string BlogNewsComment { get; set; }

        public bool IsApproved { get; set; }
        public string UserName { get; set; }
    }
}
