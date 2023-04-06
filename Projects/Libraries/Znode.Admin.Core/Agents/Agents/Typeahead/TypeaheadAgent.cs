using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Admin.Agents
{
    public class TypeaheadAgent : BaseAgent, ITypeaheadAgent
    {
        #region Private Variables
        private readonly ITypeaheadClient _typeaheadClient;
        private readonly IUserClient _userClient;

        #endregion

        #region Constructor
        public TypeaheadAgent(ITypeaheadClient typeaheadClient, IUserClient userClient)
        {
            _typeaheadClient = GetClient<ITypeaheadClient>(typeaheadClient);
            _userClient = GetClient<IUserClient>(userClient);
        }
        #endregion

        #region Public Methods.
        //Get the suggestions of typeahead.
        public virtual List<AutoComplete> GetAutocompleteList(string searchTerm, string searchtype, string fieldname, string additionalOptions = null, int mappingId = 0, int pageSize = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);

            List<AutoComplete> typeaheadList = new List<AutoComplete>();
            //To bind the Typeahead RequestModel
            TypeaheadRequestModel typeaheadRequestModel = GetTypeaheadRequestModel(searchTerm, searchtype, fieldname, mappingId, pageSize);

            TypeaheadResponselistModel searchModel = _typeaheadClient.GetSearchlist(typeaheadRequestModel);

            ZnodeLogging.LogMessage("Typeaheadlist count :", string.Empty, TraceLevel.Verbose, new { typeaheadlistCount= searchModel?.Typeaheadlist?.Count});

            if (Equals(searchtype, ZnodeConstant.StoreList))
            {
                string aspNetUserId = _userClient.GetAccountByUser(HttpContext.Current.User.Identity.Name)?.AspNetUserId;
                ZnodeLogging.LogMessage("Typeaheadlist count :", string.Empty, TraceLevel.Verbose, new { typeaheadlistCount= searchModel?.Typeaheadlist?.Count});

                string[] portalIds = _userClient.GetPortalIds(aspNetUserId)?.PortalIds;
                List<int> portalIdList = new List<int>();

                for (int index = 0; index < portalIds.Length; index++)
                    portalIdList.Add(Convert.ToInt32(portalIds[index]));

                searchModel.Typeaheadlist = (from item in searchModel.Typeaheadlist
                                             join portalId in portalIdList on item.Id equals portalId
                                             orderby item.Name ascending
                                             select item).ToList();
            }

            //Add additional options in typeahead list
            if(!string.IsNullOrEmpty(additionalOptions) && !string.IsNullOrEmpty(typeaheadRequestModel?.TypeName))
                typeaheadList = AddAdditionalOptionsInTypeAheadList(typeaheadRequestModel.TypeName, additionalOptions, searchtype);

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

        #region Private Methods.
        //To check item already exists
        private bool AlreadyExist(List<AutoComplete> autoCompleteList, AutoComplete autoCompleteModel)
            => autoCompleteList.Any(x => x.text == autoCompleteModel.text);

        //To bind the Typeahead RequestModel
        private TypeaheadRequestModel GetTypeaheadRequestModel(string searchTerm, string searchtype, string fieldname, int mappingId = 0, int pageSize = 0)
        {
            TypeaheadRequestModel requestmodel = new TypeaheadRequestModel();

            switch (searchtype)
            {
                case "StoreList":
                    requestmodel.Type = ZnodeTypeAheadEnum.StoreList;
                    requestmodel.TypeName = ZnodeTypeAheadTypeNameEnum.Stores.ToString();
                    break;
                case "CatalogList":
                    requestmodel.Type = ZnodeTypeAheadEnum.CatalogList;
                    requestmodel.TypeName = ZnodeTypeAheadTypeNameEnum.Catalogs.ToString();
                    break;
                case "ProductList":
                    requestmodel.Type = ZnodeTypeAheadEnum.ProductList;
                    requestmodel.TypeName = ZnodeTypeAheadTypeNameEnum.Products.ToString();
                    break;
                case "PIMCatalogList":
                    requestmodel.Type = ZnodeTypeAheadEnum.PIMCatalogList;
                    requestmodel.TypeName = ZnodeTypeAheadTypeNameEnum.Catalogs.ToString();
                    break;
                case "PIMCategoryList":
                    requestmodel.Type = ZnodeTypeAheadEnum.PIMCatalogList;
                    requestmodel.TypeName = ZnodeTypeAheadTypeNameEnum.Catalogs.ToString();
                    break;
                case "PIMProductList":
                    requestmodel.Type = ZnodeTypeAheadEnum.PIMCatalogList;
                    requestmodel.TypeName = ZnodeTypeAheadTypeNameEnum.Catalogs.ToString();
                    break;
                case "AccountList":
                    requestmodel.Type = ZnodeTypeAheadEnum.AccountList;
                    requestmodel.TypeName = ZnodeTypeAheadTypeNameEnum.Accounts.ToString();
                    requestmodel.MappingId = mappingId;
                    requestmodel.PageSize = pageSize;
                    break;
                case "EntityList":
                    requestmodel.Type = ZnodeTypeAheadEnum.EntityList;
                    requestmodel.TypeName = ZnodeTypeAheadTypeNameEnum.Entities.ToString();
                    break;
            }
            requestmodel.FieldName = fieldname;
            requestmodel.Searchterm = searchTerm;
            return requestmodel;    
        }

        //Add additional options in typeahead list
        protected virtual List<AutoComplete> AddAdditionalOptionsInTypeAheadList(string listTypeName, string additionalOptions ,string listName = "")
        {
            List<AutoComplete> typeaheadList = new List<AutoComplete>();
            List<string> additionalOptionsList = additionalOptions.Split(',').ToList();

            foreach (string additionOption in additionalOptionsList)
            {
                string optionDisplayText = GetDisplayTextOfTypeahead(listTypeName, additionOption, listName);

                typeaheadList.Add(new AutoComplete
                {
                    text = optionDisplayText,
                    value = optionDisplayText.Replace(" ", ""),
                    Name = optionDisplayText,
                    Id = (int)(ZnodeTypeAheadListGenericOptions)Enum.Parse(typeof(ZnodeTypeAheadListGenericOptions), additionOption),
                    DisplayText = optionDisplayText
                });
            }
            return typeaheadList;
        }

        //Get display text for the type ahead
        protected virtual string GetDisplayTextOfTypeahead(string listTypeName, string additionalOption,string listName)
        {
            string optionDisplayText = "";
            if (additionalOption == ZnodeTypeAheadListGenericOptions.No.ToString())
            {
                optionDisplayText = additionalOption + " " + typeof(AdminConstants).GetField(listTypeName).GetRawConstantValue();
            }
            else
            {
                if (listName.Equals(ZnodeTypeAheadEnum.PIMCategoryList.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    optionDisplayText = additionalOption + " Categories";
                }
                else if (listName.Equals(ZnodeTypeAheadEnum.PIMProductList.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    optionDisplayText = additionalOption + " Products";
                }
                else
                {
                    optionDisplayText = additionalOption + " " + listTypeName;
                }
            }
            return optionDisplayText;
        }
        #endregion
    }
}
