using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface IPIMAttributeGroupClient : IBaseClient
    {
        /// <summary>
        /// Gets the list of Attribute Group List.
        /// </summary>
        /// <param name="expands">Expands for Attribute Group List.</param>
        /// <param name="filters">Filters for Attribute Group List.</param>
        /// <param name="sorts">Sorts for Attribute GroupList.</param>
        /// <returns> AttributeGroup list.</returns>
        PIMAttributeGroupListModel GetAttributeGroupList(ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Gets paged  Attribute Group list.
        /// </summary>
        /// <param name="expands">Expands for Attribute Group List.</param>
        /// <param name="filters">Filters for Attribute Group List.</param>
        /// <param name="sorts">Sorts for Attribute GroupList.</param>
        /// <param name="pageIndex">Start page index.</param>
        /// <param name="pageSize">Last page index.</param>
        /// <returns>Paged list of Attribute groups.</returns>
        PIMAttributeGroupListModel GetAttributeGroupList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Gets Attribute Group by ID.
        /// </summary>
        /// <param name="id">ID of Attribute Group to be fetched.</param>
        /// <param name="expands">Expands for attribute groups.</param>
        /// <returns>Updated Attribute Group Model.</returns>
        PIMAttributeGroupModel GetAttributeGroup(int id, ExpandCollection expands);
       
        /// <summary>
        /// Creates Attribute Group.
        /// </summary>
        /// <param name="AttributeGroupModel">Attribute group model to be created.</param>
        /// <returns>Created Attribute Group Model.</returns>
        PIMAttributeGroupModel CreateAttributeGroupModel(PIMAttributeGroupModel attributeGroupModel);

        /// <summary>
        /// Updates Attribute Group Model.
        /// </summary>
        /// <param name="attributeGroupModel">Attribute group model to be updated.</param>
        /// <returns>Attribute Group Model.</returns>
        PIMAttributeGroupModel UpdateAttributeGroupModel(PIMAttributeGroupModel attributeGroupModel);

        /// <summary>
        /// Deletes PIM Attribute Group Model by specified ID.
        /// </summary>
        /// <param name="pimAttributeGroupIds">IDs of Attribute group model to be deleted.</param>
        /// <returns>True or false.</returns>
        bool DeleteAttributeGroupModel(ParameterModel pimAttributeGroupIds);

        /// <summary>
        /// Get unassigned attributes.
        /// </summary>
        /// <param name="expands">Expands for Attribute.</param>
        /// <param name="filters">Filters for Attribute.</param>
        /// <param name="sorts">Sorts for Attribute.</param>
        /// <param name="pageIndex">Start page index.</param>
        /// <param name="pageSize">page size.</param>
        /// <returns>Returns list of assigned attributes.</returns>
        PIMAttributeListModel GetUnAssignedAttributes(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get assigned attributes.
        /// </summary>
        /// <param name="expands">Expands for Attribute.</param>
        /// <param name="filters">Filters for Attribute.</param>
        /// <param name="sorts">Sorts for Attribute.</param>
        /// <param name="pageIndex">Start page index.</param>
        /// <param name="pageSize">page size.</param>
        /// <returns>Returns list of assigned attributes.</returns>
        PIMAttributeGroupMapperListModel GetAssignedAttributes(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get PIM attribute group locales.
        /// </summary>
        /// <param name="expands">Expands</param>
        /// <param name="filters">Filters</param>
        /// <param name="sorts">Sorts</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageSize">Page Size</param>
        /// <returns>Returns list of PIM attribute group locales.</returns>
        PIMAttributeGroupLocaleListModel GetPIMAttributeGroupLocales(int pimAttributeGroupLocale);

        /// <summary>
        /// Save attribute group locales.
        /// </summary>
        /// <param name="model">model to save.</param>
        /// <returns>Returns saved model.</returns>
        PIMAttributeGroupLocaleListModel SaveAttributeGroupLocales(PIMAttributeGroupLocaleListModel model);

        /// <summary>
        /// Associate attributes
        /// </summary>
        /// <param name="model">PIM attribute group mapper list model.</param>
        /// <returns>Returns the inserted records.</returns>
        PIMAttributeGroupMapperListModel AssociateAttributes(PIMAttributeGroupMapperListModel model);

        /// <summary>
        /// Remove associated attributes.
        /// </summary>
        /// <param name="model">model contains data to remove.</param>
        /// <returns>Returns true if removed otherwise false.</returns>
        bool RemoveAssociatedAttributes(RemoveAssociatedAttributesModel model);

        /// <summary>
        /// Update Attribute Display Order
        /// </summary>
        /// <param name="pimAttributeDataModel"></param>
        /// <returns></returns>
        PIMAttributeDataModel UpdateAttributeDisplayOrder(PIMAttributeDataModel pimAttributeDataModel);
        
    }
}
