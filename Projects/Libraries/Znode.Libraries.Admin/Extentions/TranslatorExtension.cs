using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data;
using Znode.Libraries.MongoDB.Data;

namespace Znode.Libraries.ECommerce.Utilities
{
    public static class TranslatorExtension
    {
        /// <summary>
        /// Translate Model to Entity
        /// </summary>
        /// <typeparam name="TEntity">Translate Model to TEntity</typeparam>
        /// <param name="modelBase">BaseModel is extended class</param>
        /// <returns></returns>
        public static TEntity ToEntity<TEntity>(this BaseModel modelBase)
            => Translator.Translate<TEntity>(modelBase);

        /// <summary>
        /// Translate Model to Entity
        /// </summary>
        /// <typeparam name="TEntity">TEntity is a destination model, having contraint TEntity is a ZnodeEntityBaseModel</typeparam>
        /// <typeparam name="TModel">TModel is source model</typeparam>
        /// <param name="Model">model is extended class</param>
        /// <returns></returns>
        public static TEntity ToEntity<TEntity, TModel>(this TModel model) where TEntity : ZnodeEntityBaseModel
            => Translator.Translate<TEntity, TModel>(model);

        /// <summary>
        /// Translate Model collection to Model Entity
        /// </summary>
        /// <typeparam name="TEntity">TEntity is a destination model</typeparam>
        /// <param name="collection">collection is extended BaseModel class list</param>
        /// <returns></returns>
        public static IEnumerable<TEntity> ToEntity<TEntity>(this IEnumerable<BaseModel> collection)
            => Translator.Translate<TEntity>(collection);

        /// <summary>
        /// Translate Model collection to Model Entity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static IEnumerable<TEntity> ToEntity<TEntity, TModel>(this IEnumerable<TModel> collection)
            => Translator.Translate<TEntity, TModel>(collection);

        /// <summary>
        /// Transalate Entity to Model
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public static TModel ToModel<TModel>(this ZnodeEntityBaseModel entity)
            => Translator.Translate<TModel>(entity);

        /// <summary>
        /// Transalate Entity to Model
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public static TModel ToModel<TModel, TEntity>(this TEntity entity)
            => Translator.Translate<TModel, TEntity>(entity);

        /// <summary>
        /// Translate Collection Entity to Collection Model
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="EntityCollection"></param>
        /// <returns></returns>
        public static IEnumerable<TModel> ToModel<TModel>(this IEnumerable<ZnodeEntityBaseModel> entityCollection)
            => Translator.Translate<TModel>(entityCollection);

        /// <summary>
        /// Translate Collection Entity to Collection Model
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="EntityCollection"></param>
        /// <returns></returns>
        public static IEnumerable<TModel> ToModel<TModel, TEntity>(this IEnumerable<TEntity> entityCollection)
            => Translator.Translate<TModel, TEntity>(entityCollection);

        /// <summary>
        /// Translate Filter Data Collection to Filter Mongo Collection
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public static FilterMongoCollection ToFilterMongoCollection(this FilterCollection filters)
        {
            FilterMongoCollection dataCollection = new FilterMongoCollection();
            if (HelperUtility.IsNotNull(filters))
                dataCollection.AddRange(filters.ToModel<FilterMongoTuple, FilterTuple>());
            return dataCollection;
        }

        /// <summary>
        /// Translate Filter Collection to Filter Data Collection
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public static FilterDataCollection ToFilterDataCollections(this FilterCollection filters)
        {
            FilterDataCollection dataCollection = new FilterDataCollection();
            if (!Equals(filters, null))
            {
                dataCollection.AddRange(filters.ToModel<FilterDataTuple, FilterTuple>());
            }
            return dataCollection;
        }
    }
}
