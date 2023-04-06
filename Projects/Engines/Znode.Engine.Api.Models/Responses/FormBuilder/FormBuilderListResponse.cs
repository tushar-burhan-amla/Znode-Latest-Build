using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class FormBuilderListResponse : BaseListResponse
    {
        public List<FormBuilderModel> FormBuilderList { get; set; }
    }
}