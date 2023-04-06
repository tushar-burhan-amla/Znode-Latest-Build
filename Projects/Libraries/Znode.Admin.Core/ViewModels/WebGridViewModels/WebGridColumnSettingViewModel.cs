using System;
using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{
    public class WebGridColumnSettingViewModel
    {
        public int Id { get; set; }
        public string ColumnNames { get; set; }
        public string ViewMode { get; set; }
        public string ViewOptions { get; set; }
        public string PageName { get; set; }
        public string ObjectName { get; set; }
        public List<WebGridColumnViewModel> _webGridColumnViewModels { get; set; }
    }
}
