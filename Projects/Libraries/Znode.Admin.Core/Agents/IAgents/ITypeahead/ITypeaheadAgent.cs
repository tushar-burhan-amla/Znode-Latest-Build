using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.Agents
{
    public interface ITypeaheadAgent
    {
        /// <summary>
        ///Get the suggestions of typeahead.
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <param name="searchtype"></param>
        /// <param name="fieldname"></param>
        /// <param name="additionalOptions">Additional Options</param>
        /// <returns>Returns suggestions list.</returns>
        List<AutoComplete> GetAutocompleteList(string searchTerm, string searchtype, string fieldname, string additionalOptions = null, int mappingId = 0, int pageSize = 0);
    }
}
