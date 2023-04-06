using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class CaseRequestListViewModel : BaseViewModel
    {
        public List<CaseRequestViewModel> CaseRequestsList { get; set; }
        public GridModel GridModel { get; set; }
        public int CaseRequestId { get; set; }
    }
}