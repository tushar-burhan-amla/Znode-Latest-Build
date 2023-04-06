using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IGlobalAttributeGroupClient : IBaseClient
    {
        /// <summary>
        /// Gets paged global attribute group list.
        /// </summary>
        /// <param name="expands">Expands for Attribute Group List.</param>
        /// <param name="filters">Filters for Attribute Group List.</param>
        /// <param name="sorts">Sorts for Attribute GroupList.</param>
        /// <param name="pageIndex">Start page index.</param>
        /// <param name="pageSize">Last page index.</param>
        /// <returns>Paged list of attribute groups.</returns>
        GlobalAttributeGroupListModel GetAttributeGroupList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Create attribute group.
        /// </summary>
        /// <param name="AttributeGroupModel">Attribute group model to be created.</param>
        /// <returns>Return model with data.</returns>
        GlobalAttributeGroupModel CreateAttributeGroupModel(GlobalAttributeGroupModel attributeGroupModel);

        /// <summary>
        /// Gets Attribute Group by ID.
        /// </summary>
        /// <param name="id">ID of Attribute Group to be fetched.</param>
        /// <param name="expands">Expands for attribute groups.</param>
        /// <returns>Updated Attribute Group Model.</returns>
        GlobalAttributeGroupModel GetAttributeGroup(int id, ExpandCollection expands);

        /// <summary>
        /// Updates Attribute Group Model.
        /// </summary>
        /// <param name="attributeGroupModel">Attribute group model to be updated.</param>
        /// <returns>Attribute Group Model.</returns>
        GlobalAttributeGroupModel UpdateAttributeGroupModel(GlobalAttributeGroupModel attributeGroupModel);

        /// <summary>
        /// Get global attribute group locales.
        /// </summary>
        ///<param name="globalAttributeGroupLocaleId">global Attribute Group locale id.</param>
        /// <returns>Returns list of global attribute group locales.</returns>
        GlobalAttributeGroupLocaleListModel GetGlobalAttributeGroupLocales(int globalAttributeGroupLocaleId);

        /// <summary>
        /// Deletes Global Attribute Group Model by specified ID.
        /// </summary>
        /// <param name="globalAttributeGroupIds">Ids of Attribute group model to be deleted.</param>
        /// <returns>True or false.</returns>
        bool DeleteAttributeGroup(ParameterModel globalAttributeGroupIds);

        /// <summary>
        /// Get assigned attributes.
        /// </summary>
        /// <param name="expands">Expands for Attribute.</param>
        /// <param name="filters">Filters for Attribute.</param>
        /// <param name="sorts">Sorts for Attribute.</param>
        /// <param name="pageIndex">Start page index.</param>
        /// <param name="pageSize">page size.</param>
        /// <returns>Returns list of assigned attributes.</returns>
        GlobalAttributeGroupMapperListModel GetAssignedAttributes(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get unassigned attributes.
        /// </summary>
        /// <param name="expands">Expands for Attribute.</param>
        /// <param name="filters">Filters for Attribute.</param>
        /// <param name="sorts">Sorts for Attribute.</param>
        /// <param name="pageIndex">Start page index.</param>
        /// <param name="pageSize">page size.</param>
        /// <returns>Returns list of assigned attributes.</returns>
        GlobalAttributeListModel GetUnAssignedAttributes(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Associate attributes
        /// </summary>
        /// <param name="model">Global attribute group mapper list model.</param>
        /// <returns>Returns the associated records.</returns>
        GlobalAttributeGroupMapperListModel AssociateAttributes(GlobalAttributeGroupMapperListModel model);

        /// <summary>
        /// Remove associated attributes.
        /// </summary>
        /// <param name="model">Model contains data to remove.</param>
        /// <returns>Returns true if removed otherwise false.</returns>
        bool RemoveAssociatedAttributes(RemoveGroupAttributesModel model);

        /// <summary>
        /// Update Attribute Display Order
        /// </summary>
        /// <param name="globalAttributeDataModel"></param>
        /// <returns></returns>
        GlobalAttributeDataModel UpdateAttributeDisplayOrder(GlobalAttributeDataModel globalAttributeDataModel);
    }
}
