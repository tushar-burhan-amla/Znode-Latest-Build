using AutoMapper;
using System.Collections.Generic;

namespace Znode.Libraries.ECommerce.Utilities
{
    public static class Translator
    {
        /// <summary>
        ///   Translate to destination type.
        /// </summary>
        /// <param id="source">Entity/Model to translate.</param>
        /// <returns>returns translated entity or model.</returns>
        /// <exception cref="System.ArgumentNullException">source is null.</exception>
        public static TDest Translate<TDest>(object source) => Mapper.Map<TDest>(source);

        /// <summary>
        ///   Translate to list of destination type.
        /// </summary>
        /// <param id="source">Entity/Model to translate.</param>
        /// <param id="collection">List of Entity/Model to translate.</param>
        /// <returns>returns translated entity or model list.</returns>
        /// <exception cref="System.ArgumentNullException">source is null.</exception>
        /// <exception cref="System.ArgumentNullException">collection is null.</exception>
        public static IEnumerable<TDest> Translate<TDest>(IEnumerable<object> collection) => Mapper.Map<IEnumerable<TDest>>(collection);

        /// <summary>
        /// Translate Source to Destination.
        /// </summary>
        /// <typeparam name="TDest"></typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns>TDest</returns>
        /// <exception cref="System.ArgumentNullException">source is null.</exception>
        public static TDest Translate<TDest, TSource>(TSource source) => Mapper.Map<TSource, TDest>(source);

        /// <summary>
        /// Collection Translate Source to Destination.
        /// </summary>
        /// <typeparam name="TDest"></typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="collection"></param>
        /// <returns>IEnumerable<TDest></returns>
        /// <exception cref="System.ArgumentNullException">source is null.</exception>
        public static IEnumerable<TDest> Translate<TDest, TSource>(IEnumerable<TSource> collection) => Mapper.Map<IEnumerable<TSource>, IEnumerable<TDest>>(collection);
    }
}
