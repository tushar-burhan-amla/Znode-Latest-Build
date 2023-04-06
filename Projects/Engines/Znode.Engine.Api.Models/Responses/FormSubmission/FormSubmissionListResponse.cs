using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class FormSubmissionListResponse : BaseListResponse
    {
        public List<FormSubmissionModel> FormSubmissionList { get; set; }

    }
}
