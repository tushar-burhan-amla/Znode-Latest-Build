using System.Collections.Generic;
using Znode.Engine.Api.Models;

namespace Znode.Libraries.Search
{
    public class SearchParameterModel
    {
        public int CatalogId { get; set; }
        public int SearchIndexMonitorId { get; set; }
        public int SearchIndexServerStatusId { get; set; }
        public long IndexStartTime { get; set; }
        public int VersionId { get; set; }
        public string revisionType { get; set; }
        public List<LocaleModel> ActiveLocales { get; set; }

        // To determine whether preview mode is enabled or not.
        public bool IsPreviewEnabled { get; set; }

        // Property to hold the updated(with timestamp in suffix) index name.
        public string NewIndexName { get; set; }

        //To check whether preview production is enabled or not.
        public bool IsPreviewProductionEnabled { get; set; }

        //To specify whether to publish only draft products or not.
        public bool IsPublishDraftProductsOnly { get; set; }
    }
}
