using System.Collections.Generic;

namespace Znode.Engine.Api.Configuration
{
    public class CacheConfiguration
	{
        public List<CacheRoute> CacheRoutes { get; set; } = new List<CacheRoute>();

        public bool Enabled { get; set; }
	}
}
