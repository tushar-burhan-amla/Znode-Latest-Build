using System.Collections.Generic;

namespace Znode.Engine.Services
{
    public interface IEmailTemplateSharedService
    {
        //Replace the Email Template Token Keys with respective Values.
        string ReplaceTemplateTokens(string content, Dictionary<string, string> parameterList);
        //Set the Email Template Parameter Key & Values.
        Dictionary<string, string> SetParameter(string parameterName, string parameterValue);
    }
}
