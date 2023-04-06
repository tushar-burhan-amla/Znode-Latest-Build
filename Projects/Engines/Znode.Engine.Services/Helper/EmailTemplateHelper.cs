using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public static class EmailTemplateHelper
    {
        #region Declarations      
        private static readonly Dictionary<string, string> parameterList = new Dictionary<string, string>();
        #endregion

        #region public Methods

        //Set the Email Template Parameter Key & Values.
        public static void SetParameter(string parameterName, string parameterValue)
        {
            //Add the Template Key/Value Parameter.
            parameterList.Add(parameterName, parameterValue);
        }

        public static string ReplaceTemplateTokens(string content)
        {
            if (parameterList.Count > 0)
            {
                foreach (var item in parameterList)
                    //Replace the Email Template Token Keys with respective Values.
                    content = HelperUtility.ReplaceTokenWithMessageText(item.Key, item.Value, content);
            }
            //Replace remaining Unknown Email Template Keys.
            return ReplaceUnknownKeys(content);
        }
        #endregion

        #region Private Methods
        //Replace Unknown keys in Email Template With Empty values.
        private static string ReplaceUnknownKeys(string messageText)
        {
            //It is used to find keys in between special characters.
            List<string> list = Regex.Matches(messageText, @"#\w*#")
                                    .Cast<Match>()
                                     .Select(m => m.Value)
                                        .ToList();

            foreach (string key in list)
                messageText = HelperUtility.ReplaceTokenWithMessageText(key, string.Empty, messageText);

            ClearParameters();
            return messageText;
        }

        //Clear the Parameter List.
        private static void ClearParameters()
        {
            parameterList.Clear();
        }
        #endregion
    }
}
