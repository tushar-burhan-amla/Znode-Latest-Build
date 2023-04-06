using System.Collections.Generic;

namespace Znode.Engine.Api.Areas.HelpPage.ModelDescriptions
{
    public class EnumTypeModelDescription : ModelDescription
    {
        public EnumTypeModelDescription()
        {
            Values = new List<EnumValueDescription>();
        }

        public List<EnumValueDescription> Values { get; private set; }
    }
}