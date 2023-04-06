namespace Znode.Engine.Admin.ViewModels
{
    public class FormSubmissionViewModel : BaseViewModel
    {
        public int FormBuilderSubmitId { get; set; }

        public int FormBuilderId { get; set; }

        public string FormCode { get; set; }

        public string StoreName { get; set; }

        public string UserName { get; set; }

        public string FullName { get; set; }
    }
}
