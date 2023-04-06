using System.IO;
using System.Text;

namespace Znode.Libraries.ECommerce.Utilities
{
    public sealed class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding { get; }
    }
}
