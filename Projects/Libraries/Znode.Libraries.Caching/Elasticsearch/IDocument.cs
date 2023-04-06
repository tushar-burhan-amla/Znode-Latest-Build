using System;

namespace Znode.Libraries.Caching.ElasticSearch
{
    public interface IDocument
    {
        /// <summary>
        /// Time that the original CLR object was instantiated/created in C# runtime. This is used to help
        /// provide a way to fetch the latest items, the items that have not yet been fetched. This is
        /// a way to help accomplish "at-least-once" processing, as queueing frameworks such as RabbitMQ
        /// will typically provide.
        /// </summary>
        DateTime CreatedDateTime { get; set; }
    }
}
