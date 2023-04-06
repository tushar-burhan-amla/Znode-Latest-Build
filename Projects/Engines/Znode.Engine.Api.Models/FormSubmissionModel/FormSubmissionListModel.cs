using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class FormSubmissionListModel : BaseListModel
    {
        public FormSubmissionListModel()
        {
            FormSubmissionList = new List<FormSubmissionModel>();
        }

        public List<FormSubmissionModel> FormSubmissionList { get; set; }
    }
}