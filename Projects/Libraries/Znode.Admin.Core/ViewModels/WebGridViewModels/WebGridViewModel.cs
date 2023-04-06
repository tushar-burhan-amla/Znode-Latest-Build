using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class WebGridViewModel : BaseViewModel
    {
        public int Id { get; set; }
        public int EntityTypeId { get; set; }
        public string EntityType { get; set; }
        public string EntityName { get; set; }
        public string NotificationMessage { get; set; }
        public List<WebGridColumnViewModel> WebGridColumnModelList { get; set; }
        public string ViewOption { get; set; }
        public string FrontPageName { get; set; }
        public string FrontObjectName { get; set; }
        public string ViewMode { get; set; }
        public string XMLString { get; set; }
        public ListViewModel listViewModel { get; set; }
    }
}