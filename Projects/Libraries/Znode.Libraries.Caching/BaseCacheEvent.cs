using System;

namespace Znode.Libraries.Caching
{
    public class BaseCacheEvent
    {
        public Guid CacheEventId { get; set; }

        /// <summary>
        /// Has no functional effect. Used to display a provide extra contextual information to developers
        /// regarding cache events that have been triggered.
        ///
        /// It would be helpful to show a list of recent cache events in the Admin UI, and show
        /// this comment with each cache event.
        ///
        /// For example, this comment might explain that this cache event resulted:
        ///   * "From admin user clicking CLEAR CACHE in the Admin UI."
        ///   * "From admin user publishing a banner slider."
        ///   * "From admin user changing logging configuration."
        /// </summary>
        public string Comment { get; set; }

        public BaseCacheEvent()
        {
            this.CacheEventId = Guid.NewGuid();
        }
    }
}
