namespace Znode.Engine.Api.Models
{
    public class ECertTotalModel : BaseModel
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public WebStoreWidgetParameterModel WidgetParameter { get; set; }
    }
}
