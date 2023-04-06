using System.Collections.Generic;

namespace Znode.Engine.WebStore.Agents
{
    public interface ITypeaheadAgent
    {
        /// <summary>
        ///Get the suggestions of typeahead.
        /// </summary>
        /// <param name="searchTerm">search term</param>
        /// <param name="searchtype">search type</param>
        /// <param name="fieldname">field name</param>
        /// <param name="mappingId">mappingId</param>
        /// <param name="pageSize">page size</param>
        /// <returns>Returns suggestions list.</returns>
        List<AutoComplete> GetAutocompleteList(string searchTerm, string searchtype, string fieldname, int mappingId = 0, int pageSize = 0);
    }
}
