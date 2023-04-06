using System.Text;
using System.Web;

namespace Znode.Libraries.ECommerce.Utilities
{
    public class ZnodeHttpHeaders
    {
        public const string Header_MinifiedJsonResponse = "Minified-Json-Response";

        public static string GetHeaderFormattedString(string header, string value)
        {
            return new StringBuilder(header).Append(": ").Append(value).ToString();
        }

        public static string GetHeaderValue(string header)
        {
            return HttpContext.Current.Request.Headers[header];
        }
    }
}
