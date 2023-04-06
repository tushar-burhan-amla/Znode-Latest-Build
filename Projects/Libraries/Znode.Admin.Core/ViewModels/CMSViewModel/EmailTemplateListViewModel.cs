using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class EmailTemplateListViewModel : BaseViewModel
    {
        public EmailTemplateListViewModel()
        {
            EmailTemplateList = new List<EmailTemplateViewModel>();
            GridModel = new GridModel();
        }
        public List<EmailTemplateViewModel> EmailTemplateList { get; set; }
        public GridModel GridModel { get; set; }
        public string CheckEmailType { get; set; }
    }
}