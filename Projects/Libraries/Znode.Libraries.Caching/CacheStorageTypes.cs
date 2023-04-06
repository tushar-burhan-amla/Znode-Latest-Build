using System;

namespace Znode.Libraries.Caching
{
    [Flags] public enum CacheStorageTypes
    {
        /// <summary>
        /// This corresponds to the HTTP Runtime Cache that stores key (string) value (object) pairs within the Znode Api application memory.
        /// </summary>
        ApiDictionaryCache = 1,

        /// <summary>
        /// This corresponds to the HTTP Runtime Cache that stores key (string) value (object) pairs within the Znode WebStore application memory.
        /// </summary>
        WebStoreDictionaryCache = 2,

        /// <summary>
        /// This corresponds to the MVC Donut caching framework that stores rendered partial HTML views within the Znode WebStore application memory.
        /// </summary>
        WebStoreHtmlCache = 4,

        // Next value must be set to '8', '16', '32', etc. for the 'flags' operations to work. See: https://stackoverflow.com/questions/8447/what-does-the-flags-enum-attribute-mean-in-c
    }
}
