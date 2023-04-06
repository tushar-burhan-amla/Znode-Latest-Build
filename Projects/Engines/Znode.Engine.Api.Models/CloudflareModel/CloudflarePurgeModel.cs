using System.Collections.Generic;
using System;

namespace Znode.Engine.Api.Models
{
    public class CloudflarePurgeModel
    {
        public List<int> PortalId { get; set; }

        /// <summary>
        /// Property is used to pass categoryId or productId
        /// </summary>
        public List<int> Id { get; set; }

        public List<string> SeoUrl { get; set; }

        public SEODetailsEnum SeoType { get; set; }
    }
}
