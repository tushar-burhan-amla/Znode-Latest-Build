using System.IO;
using Parameters = DevExpress.XtraReports.Parameters;

namespace Znode.Libraries.DevExpress.Report
{
    public class ReportThemeHelper
    {
        public ReportThemeHelper()
        {

        }

        //Convert style sheet xml to stream.
        public Stream ConvertStyleXmlToStream(string styleXmlString)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(styleXmlString);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
