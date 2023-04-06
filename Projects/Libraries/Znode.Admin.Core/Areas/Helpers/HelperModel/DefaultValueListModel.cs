using System.Collections.Generic;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Admin.AttributeValidationHelpers
{
    public class DefaultValueListModel
    {
        public int LocaleId { get; set; }
        public string LocaleName { get; set; }
        public string LocaleCode { get; set; }
        public List<DefaultValueModel> DefaultValues { get; set; }
        public List<PIMAttributeDefaultValueModel> AttributeDefaultValueCodeList { get; set; }
        public List<GlobalAttributeDefaultValueModel> GlobalAttributeDefaultValueCodeList { get; set; }
    }
}