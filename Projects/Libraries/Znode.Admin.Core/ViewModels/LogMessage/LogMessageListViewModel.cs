using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class LogMessageListViewModel : BaseViewModel
    {
        public List<LogMessageViewModel> LogMessageList { get; set; }
        public GridModel GridModel { get; set; }
        public string Component { get; set; }
        public string LogMessage { get; set; }
        public string StackTraceMessage { get; set; }

        public LogMessageListViewModel()
        {
            LogMessageList = new List<LogMessageViewModel>();
        }
    }
}
