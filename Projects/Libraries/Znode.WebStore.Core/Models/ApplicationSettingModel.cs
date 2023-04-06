using System;

namespace Znode.Engine.WebStore.Models
{
    public class ApplicationSettingModel : BaseViewModel
    {
        public Int64 ApplicationSettingId { get; set; }
        public string SettingTableName { get; set; }
        public string GroupName { get; set; }
        public string ItemName { get; set; }
        public string Setting { get; set; }
        public int StatusId { get; set; }
        public string CreatedByName { get; set; }
        public string ModifiedByName { get; set; }
        public string ViewOptions { get; set; }
        public string FrontPageName { get; set; }
        public string FrontObjectName { get; set; }
        public bool IsCompressed { get; set; }
        public string ActionMode { get; set; }
    }
}