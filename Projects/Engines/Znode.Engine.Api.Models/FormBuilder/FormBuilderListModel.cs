using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class FormBuilderListModel : BaseListModel
    {
        public List<FormBuilderModel> FormBuilderList { get; set; }

        public FormBuilderListModel()
        {
            FormBuilderList = new List<FormBuilderModel>();
        }
    }
}