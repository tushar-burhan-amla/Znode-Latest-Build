using System.Collections.Generic;
using Znode.Engine.Api.Models;

namespace Znode.Libraries.Search
{
    public class SearchCMSPagesParameterModel
    {
        public int PortalId { get; set; }
        public int CMSSearchIndexMonitorId { get; set; }
        public int CMSSearchIndexServerStatusId { get; set; }
        public long IndexStartTime { get; set; }
        public int VersionId { get; set; }
        public string RevisionType { get; set; }
        public List<LocaleModel> ActiveLocales { get; set; }        
    }
}
