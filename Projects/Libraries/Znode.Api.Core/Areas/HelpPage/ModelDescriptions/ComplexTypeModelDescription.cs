using System.Collections.Generic;

namespace Znode.Engine.Api.Areas.HelpPage.ModelDescriptions
{
    public class ComplexTypeModelDescription : ModelDescription
    {
        public ComplexTypeModelDescription()
        {
            Properties = new List<ParameterDescription>();
        }

        public List<ParameterDescription> Properties { get; private set; }
    }
}