using System;
using System.Diagnostics;
using System.Linq;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.Caching
{
    public class BaseCacheEventPoller : ICacheEventPoller
    {
        /// <summary>
        /// Broker that handles passing each cache event to the proper evictor for processing (eviction of data).
        /// </summary>
        protected IEvictorBroker EvictorBroker = new EvictorBroker();

        /// <summary>
        /// Minimum time to wait between polls (in milliseconds).
        ///
        /// Setting a high value results in less polling operations, allowing potentially stale data to remain in the cache.
        /// Setting a low value results in more polling operations, potentially wasting system resources unnecessarily.
        ///
        /// Recommended value is in the 15,000 to 45,000 millisecond (15 to 45 second) range.
        /// </summary>
        private static readonly int pollFrequencyInMillis = CacheFrameworkSettings.GetCachePollFrequencyInMilliseconds();

        /// <summary>
        /// Poll for cache events created after last poll time, minus this amount (in milliseconds). This protects
        /// against a race condition in which multiple nodes are writing cache events close to the same time, but event
        /// A is created before event B and event B is written and seen by the poller before event A is written.
        ///
        /// Setting too high of a value here will result in pollers retrieving the same cache events multiple times.
        /// Setting too low of a value here will result in events being missed (not processed) as described above.
        ///
        /// Recommended value here is in the 500 to 1,500 millisecond range.
        /// </summary>
        private static readonly int pollOverlapThresholdInMillis = 1000;

        /// <summary>
        /// Events will not be processed until they have reached a minimum age of this many milliseconds.
        ///
        /// The purpose of this is to allow an cache holding data to wait until upstream services that it depends on
        /// have had sufficient time to properly handle the same cache events and evict any necessary data.
        ///
        /// Setting too high of a value here will result in stale data remaining in memory longer than necessary, possibly
        /// mildly degrading the user experience.
        /// Setting to low of a value here will result in stale data being re-fetched from upstream and re-cached. This would
        /// very seriously degrade the user experience because stale data will continue to be provided to the user.
        ///
        /// Recommended value here is 0ms for API, about 30,000-40,000ms for the API, and about 60,000-70,000ms for
        /// edge (Cloudflare) caching. It is important to guarantee the API finishes evicting data before the WebStore starts
        /// evicting data. Likewise, it is important to guarantee the WebStore finished evicting data before the edge (Cloudflare)
        /// evicts data.
        /// </summary>
        private readonly int eventProcessingDelayInMillis = 0;// CacheFrameworkSettings.GetCacheEventProcessingDelayInMilliseconds();

        /// <summary>
        /// Last time that events were polled for. This is initially set to a reasonable value that was slightly in the past.
        ///
        /// Initially setting this to a date too far back will result in old events being processed, which is unnecessary.
        /// Initially setting this to a date not far back enough in time might result in some events being missed, allowing stale data
        /// to remain in memory.
        /// </summary>
        private  DateTime lastPollStarted = DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(60));

        /// <summary>
        /// Helps read events as easily as possible (likely coming from elastic search).
        /// </summary>
        private static ICacheEventReader cacheEventReader = new ElasticsearchCacheEventReader();

        /// <summary>
        /// Used to force multiple simultaneous requests/threads to "wait" for previous threads to finish
        /// polling and evicting data before continuing execution and fulfilling request. This prevents
        /// race conditions where stale data is not evicted quite soon enough before another thread reads that
        /// stale data.
        /// </summary>
        private readonly object pollLock = new object();

        /// <summary>
        /// If the polling operation takes longer than this, a warning is written to the logs.
        /// </summary>
        private static readonly int pollDurationWarningThresholdInMillis = 10000;

        public BaseCacheEventPoller(int delayTime)
        {
            // Use reflection to get all of the "evictors" in the current assembly that can handle cache events.
            var instantiatedType = this.GetType();
            var assembly = instantiatedType.Assembly;
            var assemblyTypes = assembly.DefinedTypes;
            var evictors = assemblyTypes.Where(t =>
                t.FullName.EndsWith(BaseCacheEventEvictor<BaseCacheEvent>.EvictorSuffix));
            eventProcessingDelayInMillis = delayTime;
            // Register all evictors with the evictor broker. This let's each cache event be handled by the proper
            // corresponding evictor at runtime:
            foreach (var evictor in evictors)
            {
                EvictorBroker.RegisterEvictor(evictor);
            }
        }

        /// <summary>
        /// Poll for new cache events, handle evicting necessary data in response to any available events.
        ///
        /// Poll will not take place if not enough time has passed since last poll operation. This is to prevent
        /// the poll operation from happening so frequently that it wastes system resources unnecessarily.
        /// </summary>
        public void PollIfNecessary()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var isPollPerformed = false;
            var numCacheEventsRead = 0;
            var numCacheEventsProcessed = 0;

            try
            {
                // The following "lock" helps prevent against a couple of scenarios:
                //    * We don't want multiple threads running in parallel to waste resources by each polling elasticsearch for
                //      cache events at the same time.
                //    * We don't want multiple threads running in parallel to have a race condition resulting in stale data being
                //      read by a subsequent thread that thinks a poll operation is not needed but fails to account for the fact
                //      that an earlier thread is still in process of polling for cache events and yet to evict stale data.
                //
                // The following block of code is "locked". This means that if this application has multiple threads executing
                // this "PollIfNecessary" function in parallel (as a result of handling multiple HTTP requests in parallel),
                // then the first thread to begin polling will force the subsequent threads to wait until the first thread finishes
                // the whole process of reading cache events and evicting stale data. Once the first thread completes the process
                // the subsequent threads will realize that another poll is not yet necessary and immediately continue on to handle
                // the request with the ability to safely read from the cache knowing that stale data has been evicted.
                lock (pollLock)
                {
                    var currentTime = DateTime.UtcNow;
                    var millisSinceLastPoll = (currentTime - lastPollStarted).TotalMilliseconds;
                    var isPollRequired = millisSinceLastPoll >= pollFrequencyInMillis;

                    // Only perform poll operation if necessary:
                    if (isPollRequired)
                    {
                        isPollPerformed = true;

                        // Find the window of time that we should query for cache events. This window of time spans from 'now'
                        // back to the time of the previous poll operation, roughly.
                        //
                        // Apply a small overlap in time to prevent dropping events with sufficient probability. This helps prevent
                        // us from failing to process an event in a race condition, but means we will sometimes process the same event
                        // twice:
                        var minCreatedDateTime = lastPollStarted.Subtract(TimeSpan.FromMilliseconds(pollOverlapThresholdInMillis));
                        var maxCreatedDateTime = currentTime;

                        // Update static value with current time before fetching events. If this method/code is not synchronized,
                        // then this helps protect against the wasted resources in a scenario where the API receives multiple requests
                        // in parallel that trigger this function; this  approach helps prevent multiple concurrent threads from
                        // duplicating the unnecessary work of each querying for new events:
                        lastPollStarted = DateTime.UtcNow;

                        // Account for delayed processing of events. For example, it is expected that the WebStore will delay processing of
                        // events so that the API has had proper time to evict stale data before the WebStore risks re-fetching data:
                        var minCreatedDateTimeWithDelay = minCreatedDateTime.Subtract(TimeSpan.FromMilliseconds(eventProcessingDelayInMillis));
                        var maxCreatedDateTimeWithDelay = maxCreatedDateTime.Subtract(TimeSpan.FromMilliseconds(eventProcessingDelayInMillis));

                        // Read events, likely coming from an elasticsearch query directly:
                        var cacheEvents = cacheEventReader.ReadEvents(minCreatedDateTimeWithDelay, maxCreatedDateTimeWithDelay);
                        numCacheEventsRead = cacheEvents.Count;

                        if (cacheEvents.Count > 0)
                        {
                            foreach (var cacheEvent in cacheEvents)
                            {
                                // Use the broker to create an instance of the necessary evictor to handle eviction of data that
                                // has become stale as a result of this cache event:
                                EvictorBroker.InvokeEvictor(cacheEvent);
                                numCacheEventsProcessed++;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ZnodeLogging.LogMessage("Poll operation failed for unexpected reason.", CacheFrameworkConstants.ZnodeLogComponent, TraceLevel.Error, e);
            }

            // If the poll took longer than we expect a poll should ever take, then write a warning message to the logs:
            var pollDuration = stopwatch.ElapsedMilliseconds;
            if (pollDuration > pollDurationWarningThresholdInMillis)
            {
                ZnodeLogging.LogMessage($"PollIfNecessary operation took longer than expected ({pollDuration}ms). " +
                                        $"Poll performed: {isPollPerformed}. Number of cache events read: {numCacheEventsRead}. " +
                                        $"Number of cache events processed: {numCacheEventsProcessed}.", CacheFrameworkConstants.ZnodeLogComponent, TraceLevel.Warning);
            }

            // If a poll operation was actually performed and cache events existed, then write that to the log at info level.
            if (isPollPerformed && numCacheEventsRead > 0)
            {
                ZnodeLogging.LogMessage($"Poll performed in {pollDuration}ms, {numCacheEventsRead} cache events handled.", CacheFrameworkConstants.ZnodeLogComponent, TraceLevel.Info);
            }
        }
    }
}
