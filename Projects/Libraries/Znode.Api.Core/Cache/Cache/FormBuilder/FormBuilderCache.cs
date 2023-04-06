using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Cache
{
    public class FormBuilderCache : BaseCache, IFormBuilderCache
    {
        #region Private Variable
        private readonly IFormBuilderService _service;
        #endregion

        #region Constructor
        public FormBuilderCache(IFormBuilderService formBuilderService)
        {
            _service = formBuilderService;
        }
        #endregion

        #region Public Methods

        //Get list of form builder.
        public virtual string GetFormBuilderList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get form builder list.
                FormBuilderListModel list = _service.GetFormBuilderList(Expands, Filters, Sorts, Page);
                if (list?.FormBuilderList?.Count > 0)
                {
                    //Create response.
                    FormBuilderListResponse response = new FormBuilderListResponse { FormBuilderList = list.FormBuilderList };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get form on the basis of form id.
        public virtual string GetForm(int id, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                FormBuilderModel formModel = _service.GetFormBuilderById(id, Expands);
                if (IsNotNull(formModel))
                {
                    //Create response.
                    FormBuilderResponse response = new FormBuilderResponse { FormBuilder = formModel };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get form Attribute Group.
        public virtual string GetFormAttributeGroup(int formBuilderId, int localeId, int mappingId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get form builder list.
                FormBuilderAttributeGroupModel model = _service.GetFormBuilderAttributeGroup(formBuilderId, localeId, mappingId);
                if (IsNotNull(model))
                {
                    //Create response.
                    FormBuilderAttributeGroupResponse response = new FormBuilderAttributeGroupResponse { FormBuilderAttributeGroup = model };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }



        //Get unassigned form builder group by id.
        public virtual string UnAssignedAttributes(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            
            if (string.IsNullOrEmpty(data))
            {
                GlobalAttributeListModel unAssignedAttribute = _service.GetUnAssignedAttributes(Expands, Filters, Sorts, Page);
                if (IsNotNull(unAssignedAttribute))
                {
                    //Create response.
                    GlobalAttributeListResponse response = new GlobalAttributeListResponse { Attributes = unAssignedAttribute.Attributes };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get unassigned groups.
        public virtual string GetUnAssignedGroups(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                GlobalAttributeGroupListModel groupListModel = _service.GetUnAssignedGroups(Expands, Filters, Sorts, Page);
                if (groupListModel?.AttributeGroupList?.Count > 0)
                {
                    //Get response and insert it into cache.
                    AttributeEntityGroupListResponse response = new AttributeEntityGroupListResponse { AttributeEntityGroupList = groupListModel.AttributeGroupList };

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}
