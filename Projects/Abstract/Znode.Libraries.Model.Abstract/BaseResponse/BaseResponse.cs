using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Znode.Libraries.Abstract.Models.Responses
{
   public class BaseResponse : BaseModel
    {
        public int? ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        public Dictionary<string, string> CustomModelState { get; set; }
        public Dictionary<string, string> ErrorDetailList { get; set; }
    }
}
