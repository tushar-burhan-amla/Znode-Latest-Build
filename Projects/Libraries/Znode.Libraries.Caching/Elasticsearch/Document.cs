using System;

namespace Znode.Libraries.Caching.ElasticSearch
{
    public class Document : IDocument
    {
        public DateTime CreatedDateTime { get; set; }

        public Document()
        {
            this.CreatedDateTime = DateTime.UtcNow;
        }
    }
}
