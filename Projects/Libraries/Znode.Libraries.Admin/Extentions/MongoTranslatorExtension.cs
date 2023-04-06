using System.Collections.Generic;
using Znode.Libraries.MongoDB.Data;

namespace Znode.Libraries.ECommerce.Utilities
{
    public static class MongoTranslatorExtension
    {
        /// <summary>
        /// Translate Mongo Entity to Model
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public static TModel ToModel<TModel>(this MongoEntity entity)
            => Translator.Translate<TModel>(entity);

        /// <summary>
        /// Translate Collection of Mongo Entity to Collection Model
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="EntityCollection"></param>
        /// <returns></returns>
        public static IEnumerable<TModel> ToModel<TModel>(this IEnumerable<MongoEntity> entityCollection)
            => Translator.Translate<TModel>(entityCollection);
    }
}
