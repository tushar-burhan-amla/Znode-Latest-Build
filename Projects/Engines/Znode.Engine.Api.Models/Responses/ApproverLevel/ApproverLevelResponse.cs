namespace Znode.Engine.Api.Models.Responses
{
    public class ApproverLevelResponse : BaseListResponse
    {
        public ApproverLevelListModel ApproverList { get; set; }
        public ApproverLevelModel ApproverLevel { get; set; }
    }
}
