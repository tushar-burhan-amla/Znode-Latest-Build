using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;
using System;

namespace Znode.Engine.Admin.ViewModels
{
    public class NoteViewModel : BaseViewModel
    {
        public int NoteId { get; set; }
        public int UserId { get; set; }
        public int? AccountId { get; set; }
        public int? CaseRequestId { get; set; }

        [MaxLength(50, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorNoteTitleLength)]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorNoteTitleRequired)]
        [Display(Name = ZnodeAdmin_Resources.LabelNoteTitle, ResourceType = typeof(Admin_Resources))]
        public string NoteTitle { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelNoteBody, ResourceType = typeof(Admin_Resources))]
        [UIHint("RichTextBox")]
        [AllowHtml]
        public string NoteBody { get; set; }
        public AccountViewModel CompanyAccount { get; set; }
        public string UserName { get; set; }     
        public DateTime NoteDateTime { get; set; }
    }
}