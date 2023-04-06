using System.Collections.Generic;

namespace Znode.Libraries.Abstract.Configuration
{
    public class CacheConfiguration
	{
        public List<CacheRoute> CacheRoutes { get; set; } = new List<CacheRoute>();

        public bool Enabled { get; set; }
	}
}
