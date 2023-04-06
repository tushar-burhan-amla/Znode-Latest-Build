using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class NoteListViewModel : BaseViewModel
    {
        public List<NoteViewModel> Notes { get; set; }
        public int UserId { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public string CustomerName { get; set; }
        public bool HasParentAccounts { get; set; }
        public GridModel GridModel { get; set; }
        public AccountViewModel CompanyAccount { get; set; }
        public int? CaseRequestId { get; set; }
        public string Title { get; set; }
    }
}