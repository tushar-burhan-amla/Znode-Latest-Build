using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class FormSubmissionListViewModel : BaseViewModel
    {
        public FormSubmissionListViewModel()
        {
            FormSubmissionList = new List<FormSubmissionViewModel>();
        }
        public List<FormSubmissionViewModel> FormSubmissionList { get; set; }
        public GridModel GridModel { get; set; }
    }
}
