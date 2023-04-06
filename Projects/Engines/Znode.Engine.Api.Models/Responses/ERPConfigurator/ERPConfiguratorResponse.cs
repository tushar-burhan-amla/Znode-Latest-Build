namespace Znode.Engine.Api.Models.Responses
{
    public class ERPConfiguratorResponse : BaseResponse
    {
        public ERPConfiguratorModel ERPConfigurator { get; set; }
        public string ActiveERPClassName { get; set; }
        public string ERPClassName { get; set; }
        public int ERPTaskSchedulerId { get; set; }
    }
}
