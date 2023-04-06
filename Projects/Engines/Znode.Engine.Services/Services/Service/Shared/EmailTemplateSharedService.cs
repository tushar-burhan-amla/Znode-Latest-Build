using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public class EmailTemplateSharedService : BaseService, IEmailTemplateSharedService
    {
        #region Declarations      
        private readonly Dictionary<string, string> parameterList;//= new Dictionary<string, string>();
        #endregion

        #region public Methods

        //Set the Email Template Parameter Key & Values.
        public Dictionary<string, string> SetParameter(string parameterName, string parameterValue)
        {
            //Add the Template Key/Value Parameter.
            parameterList.Add(parameterName, parameterValue);
            return parameterList;
        }

        public string ReplaceTemplateTokens(string content, Dictionary<string, string> parameterList)
        {
            if (parameterList.Count > 0)
            {
                foreach (KeyValuePair<string, string> item in parameterList)
                {
                    //Replace the Email Template Token Keys with respective Values.
                    content = HelperUtility.ReplaceTokenWithMessageText(item.Key, item.Value, content);
                }
            }
            //Replace remaining Unknown Email Template Keys.
            return ReplaceUnknownKeys(content);
        }
        #endregion

        #region Private Methods
        //Replace Unknown keys in Email Template With Empty values.
        private string ReplaceUnknownKeys(string messageText)
        {
            //It is used to find keys in between special characters.
            List<string> list = Regex.Matches(messageText, @"#\w*#")
                                    .Cast<Match>()
                                     .Select(m => m.Value)
                                        .ToList();

            foreach (string key in list)
            {
                messageText = HelperUtility.ReplaceTokenWithMessageText(key, string.Empty, messageText);
            }

            //ClearParameters();
            return messageText;
        }

        //Clear the Parameter List.
        private void ClearParameters()
        {
            //parameterList.Clear();
        }
        #endregion
    }
}
