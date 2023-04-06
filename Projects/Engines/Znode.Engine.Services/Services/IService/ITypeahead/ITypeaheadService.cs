using Znode.Engine.Api.Models;

namespace Znode.Engine.Services
{
    public interface ITypeaheadService
    {
        /// <summary>
        /// Gets TypeaheadResponse
        /// </summary>
        /// <param name="typeaheadreqModel">Typeahead Request model.</param>
        /// <returns>Returns suggestions list.</returns>
        TypeaheadResponselistModel GetTypeaheadList(TypeaheadRequestModel typeaheadreqModel);
    }
}
