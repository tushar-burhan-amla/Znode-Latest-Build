using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ValidateServerModel
    {
        public ValidateServerModel()
        {
            ControlsData = new Dictionary<string, object>();
        }
        public Dictionary<string, object> ControlsData { get; set; }
        public Dictionary<string, string> ErrorDictionary { get; set; }
    }
}
