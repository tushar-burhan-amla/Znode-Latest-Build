using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Znode.Engine.WebStore.Agents;

namespace Znode.Engine.WebStore.Controllers
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
        public virtual JsonResult GetSuggestions(string type, string fieldname, string query, int mappingId = 0, int pageSize = 0)
        {
            return new JsonResult { Data = _typeaheadAgent.GetAutocompleteList(query, type, fieldname, mappingId, pageSize), MaxJsonLength = int.MaxValue, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion
    }
}
