using System.Web.Mvc;
using Znode.Engine.Admin.Agents;

namespace Znode.Engine.Admin.Controllers
{
    public class TypeaheadController : BaseController
    {
        #region Private ReadOnly members

        private readonly ITypeaheadAgent _typeaheadAgent;

        #endregion

        #region Constructor

        public TypeaheadController(ITypeaheadAgent typeahead)
        {
            _typeaheadAgent = typeahead;
        }
        #endregion

        #region Public methods

        //Get Suggestions.
        [HttpGet]
        public virtual JsonResult GetSuggestions(string type, string fieldname, string query, string additionalOptions = null, int mappingId = 0, int pageSize = 0)
        {
            return new JsonResult { Data = _typeaheadAgent.GetAutocompleteList(query, type, fieldname, additionalOptions, mappingId, pageSize), MaxJsonLength = int.MaxValue, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion
    }
}
