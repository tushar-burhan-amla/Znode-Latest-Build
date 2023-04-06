using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.WebStore
{
    public static class TranslatorExtension
    {
        /// <summary>
        /// Translate Model to ViewModel
        /// </summary>
        /// <typeparam name="TDTOModel">Translate Model to ViewModel</typeparam>
        /// <param name="modelBase">APIBaseModel is extended class</param>
        /// <returns></returns>
        public static TDTOModel ToViewModel<TDTOModel>(this BaseModel modelBase)
            => Translator.Translate<TDTOModel>(modelBase);

        /// <summary>
        /// Translate Model to ViewModel
        /// </summary>
        /// <typeparam name="TDTOModel">TDTOModel is a destination model, having contraint TDTOModel is a BaseModel</typeparam>
        /// <typeparam name="TModel">TModel is source model</typeparam>
        /// <param name="Model">model is extended class</param>
        /// <returns></returns>
        public static TDTOModel ToViewModel<TDTOModel, TModel>(this TModel model) where TDTOModel : BaseModel
            => Translator.Translate<TDTOModel, TModel>(model);

        /// <summary>
        /// Translate Model collection to View Model
        /// </summary>
        /// <typeparam name="TDTOModel">TDTOModel is a destination model</typeparam>
        /// <param name="collection">collection is extended APIBaseModel class list</param>
        /// <returns></returns>
        public static IEnumerable<TDTOModel> ToViewModel<TDTOModel>(this IEnumerable<BaseModel> collection)
            => Translator.Translate<TDTOModel>(collection);

        /// <summary>
        /// Translate Model collection to View Model
        /// </summary>
        /// <typeparam name="TDTOModel"></typeparam>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static IEnumerable<TDTOModel> ToViewModel<TDTOModel, TModel>(this IEnumerable<TModel> collection)
            => Translator.Translate<TDTOModel, TModel>(collection);

        /// <summary>
        /// Transalate View Model to Model
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public static TModel ToModel<TModel>(this BaseViewModel viewModel)
            => Translator.Translate<TModel>(viewModel);

        /// <summary>
        /// Transalate View Model to Model
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TDTOModel"></typeparam>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public static TModel ToModel<TModel, TDTOModel>(this TDTOModel viewModel)
            => Translator.Translate<TModel, TDTOModel>(viewModel);

        /// <summary>
        /// Translate Collection View Model to Collection Model
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="EntityCollection"></param>
        /// <returns></returns>
        public static IEnumerable<TModel> ToModel<TModel>(this IEnumerable<BaseViewModel> viewModelCollection)
            => Translator.Translate<TModel>(viewModelCollection);

        /// <summary>
        /// Translate Collection View Model to Collection Model
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TDTOModel"></typeparam>
        /// <param name="EntityCollection"></param>
        /// <returns></returns>
        public static IEnumerable<TModel> ToModel<TModel, TDTOModel>(this IEnumerable<TDTOModel> viewModelCollection)
            => Translator.Translate<TModel, TDTOModel>(viewModelCollection);
    }
}