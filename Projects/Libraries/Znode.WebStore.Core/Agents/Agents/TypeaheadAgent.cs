using System;
using System.Collections.Generic;
using System.Diagnostics;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.WebStore.Agents
{
    public class TypeaheadAgent : BaseAgent, ITypeaheadAgent
    {
        #region Private Variables
        private readonly ITypeaheadClient _typeaheadClient;
        #endregion

        #region Constructor
        public TypeaheadAgent(ITypeaheadClient typeaheadClient, IUserClient userClient)
        {
            _typeaheadClient = GetClient<ITypeaheadClient>(typeaheadClient);
        }
        #endregion

        #region Public Methods.
        //Get the suggestions of typeahead.
        public virtual List<AutoComplete> GetAutocompleteList(string searchTerm, string searchtype, string fieldname, int mappingId = 0, int pageSize = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);

            List<AutoComplete> typeaheadList = new List<AutoComplete>();
            //To bind the Typeahead RequestModel
            TypeaheadRequestModel typeaheadRequestModel = GetTypeaheadRequestModel(searchTerm, searchtype, fieldname, mappingId, pageSize);

            TypeaheadResponselistModel searchModel = _typeaheadClient.GetSearchlist(typeaheadRequestModel);

            ZnodeLogging.LogMessage("Typeaheadlist count :", string.Empty, TraceLevel.Verbose, new { typeaheadlistCount = searchModel?.Typeaheadlist?.Count });

            //Map autocomplete list
            searchModel?.Typeaheadlist?.ForEach(x =>
            {
                typeaheadList.Add(new AutoComplete
                {
                    text = x.Name,
                    value = Convert.ToString(x.Id),
                    Name = x.Name,
                    Id = x.Id,
                    DisplayText = x.Name
                });
            });

            ZnodeLogging.LogMessage("Agent method execution done.", string.Empty, TraceLevel.Info);

            return typeaheadList;
        }
        #endregion

        #region Private Methods
        //To bind the Typeahead RequestModel
        private TypeaheadRequestModel GetTypeaheadRequestModel(string searchTerm, string searchtype, string fieldname, int mappingId = 0, int pageSize = 0)
        {
            TypeaheadRequestModel requestmodel = new TypeaheadRequestModel();

            switch (searchtype)
            {
                case ZnodeConstant.EligibleReturnOrderNumberList:
                    requestmodel.Type = ZnodeTypeAheadEnum.EligibleReturnOrderNumberList;
                    requestmodel.TypeName = ZnodeTypeAheadTypeNameEnum.ReturnOrders.ToString();
                    requestmodel.MappingId = PortalAgent.CurrentPortal.PortalId;
                    requestmodel.PageSize = pageSize;
                    break;
            }
            requestmodel.FieldName = fieldname;
            requestmodel.Searchterm = searchTerm;
            return requestmodel;
        }
        #endregion
    }
}
