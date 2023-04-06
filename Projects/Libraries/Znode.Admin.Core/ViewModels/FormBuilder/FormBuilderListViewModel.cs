using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class FormBuilderListViewModel : BaseViewModel
    {
        public FormBuilderListViewModel()
        {
            FormBuilderList = new List<FormBuilderViewModel>();
        }
        public List<FormBuilderViewModel> FormBuilderList { get; set; }
        public GridModel GridModel { get; set; }
    }
}
