using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface ITypeaheadClient : IBaseClient
    {
        /// <summary>
        /// Gets suggestions
        /// </summary>
        /// <param name="model">Typeahead Request model.</param>
        /// <returns>Returns suggestions list.</returns>
        TypeaheadResponselistModel GetSearchlist(TypeaheadRequestModel model);
    }
}
