using System;
using System.Text.RegularExpressions;

namespace Znode.Engine.WebStore.ViewModels
{
    public class WidgetTextViewModel : BaseViewModel
    {
        public int ContentPageId { get; set; }
        public string WidgetsKey { get; set; }
        public string TypeOFMapping { get; set; }
        public int MappingId { get; set; }
        public string Text { get; set; }
        public string PageTitle { get; set; }
        public bool IsEmpty
        {
            get
            {
                return string.IsNullOrEmpty(Text)?true:Equals(Text.Replace("<p>", String.Empty).Replace("</p>",string.Empty) ,String.Empty);
            }
        }
    }
}