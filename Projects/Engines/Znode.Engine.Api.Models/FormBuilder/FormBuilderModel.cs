namespace Znode.Engine.Api.Models
{
    public class FormBuilderModel : BaseModel
    {
        public int FormBuilderId { get; set; }
      
        public string FormCode { get; set; }
        public string FormDescription { get; set; }

        public bool? IsShowCaptcha { get; set; } = false;
    }
}
