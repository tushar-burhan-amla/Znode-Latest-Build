using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class NoteModel : BaseModel
    {
        public int NoteId { get; set; }
        public int UserId { get; set; }
        public int? AccountId { get; set; }
        public int? CaseRequestId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorNoteTitleRequired)]
        [Display(Name = ZnodeAdmin_Resources.LabelNoteTitle, ResourceType = typeof(Admin_Resources))]
        public string NoteTitle { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelNoteBody, ResourceType = typeof(Admin_Resources))]
        [UIHint("RichTextBox")]
        public string NoteBody { get; set; }
        public string UserName { get; set; }
    }
}
