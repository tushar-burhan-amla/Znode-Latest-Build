using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public class ProductsClient : BaseClient, IProductsClient
    {
        #region Product

        //Get product attribute to edit
        public virtual PIMFamilyDetailsModel GetProduct(PIMGetProductModel model)
        {
            string endpoint = ProductsEndpoint.Get();

            ApiStatus status = new ApiStatus();
            PIMAttributeFamilyResponse response = PostResourceToEndpoint<PIMAttributeFamilyResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.PIMFamilyDetails;
        }

        //Get product list
        public virtual ProductDetailsListModel GetProducts(ExpandCollection expands, FilterCollection filters, SortCollection sorts)
            => GetProducts(expands, filters, sorts, null, null);

        //Get product list
        public virtual ProductDetailsListModel GetProducts(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = ProductsEndpoint.List();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ProductListResponse response = GetResourceFromEndpoint<ProductListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return GetProductDetailsListModelByResponse(response);
        }

        //Get product list
        public virtual ProductDetailsListModel GetBrandProducts(string productIds, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = ProductsEndpoint.GetBrandProductList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ProductListResponse response = GetResourceFromEndpoint<ProductListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return GetProductDetailsListModelByResponse(response);
        }


        //Gives product detail list model by product list response
        public ProductDetailsListModel GetProductDetailsListModelByResponse(ProductListResponse response)
        {
            ProductDetailsListModel list = new ProductDetailsListModel
            {
                ProductDetailList = response?.ProductDetails,
                Locale = response?.Locale,
                AttributeColumnName = response?.AttrubuteColumnName,
                XmlDataList = response?.XmlDataList,
                ProductDetailListDynamic = response?.ProductDetailsDynamic,
                NewAttributeList = response?.NewAttributeList
            };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        public virtual ProductDetailsListModel GetProducts(string productIds, ExpandCollection expands, FilterCollection filters, SortCollection sorts)
           => GetProducts(productIds, expands, filters, sorts, null, null);

        public virtual ProductDetailsListModel GetProducts(string productIds, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = ProductsEndpoint.List(productIds);
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ProductListResponse response = GetResourceFromEndpoint<ProductListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return GetProductDetailsListModelByResponse(response);
        }

        public virtual ProductDetailsListModel GetProductsToBeAssociated(string productIds, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = ProductsEndpoint.GetProductsToBeAssociated();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);
            ApiStatus status = new ApiStatus();
            ProductListResponse response = PostResourceToEndpoint<ProductListResponse>(endpoint, JsonConvert.SerializeObject(new ParameterModel() { Ids = productIds }), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return GetProductDetailsListModelByResponse(response);
        }

        // Method for Get Configure Products To Be Associated or unassociated
        public virtual ProductDetailsListModel GetAssociatedUnAssociatedConfigureProducts(int parentProductId, string associatedProductIds, string associatedAttributeIds, bool pimProductIdsIn, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = ProductsEndpoint.GetAssociatedUnAssociatedConfigureProducts(parentProductId, associatedProductIds, associatedAttributeIds, pimProductIdsIn);
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ProductListResponse response = GetResourceFromEndpoint<ProductListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return GetProductDetailsListModelByResponse(response);
        }

        //Create Product
        public virtual ProductModel CreateProduct(ProductModel model)
        {
            string endpoint = ProductsEndpoint.Create();

            ApiStatus status = new ApiStatus();
            ProductResponse response = PostResourceToEndpoint<ProductResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.Product;
        }

        //Delete Product
        public virtual bool DeleteProduct(string productIds)
        {
            string endpoint = ProductsEndpoint.Delete(productIds);

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(new ParameterModel() { Ids = productIds }), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Update assign product display order.
        public virtual LinkProductDetailModel UpdateAssignLinkProducts(LinkProductDetailModel linkProductDetailModel)
        {
            string endpoint = ProductsEndpoint.UpdateAssignLinkProducts();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            LinkProductDetailResponse response = PutResourceToEndpoint<LinkProductDetailResponse>(endpoint, JsonConvert.SerializeObject(linkProductDetailModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.LinkProductDetail;           
        }
    
        public virtual CategoryProductListModel GetAssociatedUnAssociatedCategoryProducts(int categoryId, bool associatedProduts, ExpandCollection expands, FilterCollection filters, SortCollection sorts)
            => GetAssociatedUnAssociatedCategoryProducts(categoryId, associatedProduts, expands, filters, sorts, null, null);

        public virtual CategoryProductListModel GetAssociatedUnAssociatedCategoryProducts(int categoryId, bool associatedProduts, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = ProductsEndpoint.GetAssociatedUnAssociatedCategoryProducts(categoryId, associatedProduts);
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            CategoryProductListResponse response = GetResourceFromEndpoint<CategoryProductListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            CategoryProductListModel list = new CategoryProductListModel { CategoryProducts = (Equals(response, null)) ? null : response.CategoryProducts };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get product family details on family id.
        public virtual PIMFamilyDetailsModel GetProductFamilyDetails(PIMFamilyModel model)
        {
            string endpoint = ProductsEndpoint.GetProductFamilyDetails();

            ApiStatus status = new ApiStatus();
            PIMAttributeFamilyResponse response = PostResourceToEndpoint<PIMAttributeFamilyResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.PIMFamilyDetails;
        }

        //Get assigned personalized attributes list
        public virtual PIMProductAttributeValuesListModel GetAssignedPersonalizedAttributes(ExpandCollection expands, FilterCollection filters)
        {
            string endpoint = ProductsEndpoint.GetAssignedPersonalizedAttributes();
            endpoint += BuildEndpointQueryString(expands, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            ProductListResponse response = GetResourceFromEndpoint<ProductListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PIMProductAttributeValuesListModel list = new PIMProductAttributeValuesListModel { ProductAttributeValues = response?.ProductAttributeValues.ProductAttributeValues };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get list of un-assigned personalized attributes
        public virtual PIMAttributeListModel GetUnassignedPersonalizedAttributes(ExpandCollection expands, FilterCollection filters)
        {
            string endpoint = ProductsEndpoint.GetUnassignedPersonalizedAttributes();
            endpoint += BuildEndpointQueryString(expands, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            PIMAttributeListResponse response = GetResourceFromEndpoint<PIMAttributeListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PIMAttributeListModel list = new PIMAttributeListModel { Attributes = response?.Attributes };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get assign personalized attributes
        public virtual bool AssignPersonalizedAttributes(PIMAttributeValueListModel model, FilterCollection filters)
        {
            //Get Endpoint.
            string endpoint = ProductsEndpoint.AssignPersonalizedAttributes();
            endpoint += BuildEndpointQueryString(null, filters, null, null, null);
            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            ProductListResponse response = PostResourceToEndpoint<ProductListResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return true;
        }

        //Get un-assign personalized attributes
        public virtual bool UnassignPersonalizedAttributes(ParameterModel parameter, int parentProductId)
        {
            string endpoint = ProductsEndpoint.UnassignPersonalizedAttributes(parentProductId);

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(parameter), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Get all configure attributes associated with Family
        public virtual PIMFamilyDetailsModel GetConfigureAttribute(PIMFamilyModel pimFamilyModel)
        {
            string endpoint = ProductsEndpoint.GetConfigureAttributes();

            ApiStatus status = new ApiStatus();
            PIMAttributeFamilyResponse response = PostResourceToEndpoint<PIMAttributeFamilyResponse>(endpoint, JsonConvert.SerializeObject(pimFamilyModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.PIMFamilyDetails;
        }

        //Activate/Deactivate bulk product
        public virtual bool ActivateDeactivateProducts(string productIds, bool isActive, int localeId)
        {
            string endpoint = ProductsEndpoint.ActivateDeactivateProducts();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(new ActivateDeactivateProductsModel() { ProductIds = productIds, IsActive = isActive, LocaleId = localeId }), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        #endregion Product

        #region Product Type

        //Gets list of associated products as per parent product.
        public virtual ProductDetailsListModel GetAssociatedProducts(int parentProductId, int attributeId, ExpandCollection expands, FilterCollection filters, SortCollection sorts)
            => GetAssociatedProducts(parentProductId, attributeId, expands, filters, sorts, null, null);

        //Gets list of associated products as per parent product.
        public virtual ProductDetailsListModel GetAssociatedProducts(int parentProductId, int attributeId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = ProductsEndpoint.GetAssociatedProducts(parentProductId, attributeId);
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ProductListResponse response = GetResourceFromEndpoint<ProductListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return GetProductDetailsListModelByResponse(response);
        }

        //Gets list of products those are not associated with parent product.
        public virtual ProductDetailsListModel GetUnassociatedProducts(int parentProductId, string associatedProductIds, ExpandCollection expands, FilterCollection filters, SortCollection sorts)
            => GetUnassociatedProducts(parentProductId, associatedProductIds, expands, filters, sorts, null, null);

        //Gets list of products those are not associated with parent product.
        public virtual ProductDetailsListModel GetUnassociatedProducts(int parentProductId, string associatedProductIds, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = ProductsEndpoint.GetUnassociatedProducts(parentProductId);
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ProductListResponse response = PostResourceToEndpoint<ProductListResponse>(endpoint, JsonConvert.SerializeObject(new ParameterModel() { Ids = associatedProductIds }), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return GetProductDetailsListModelByResponse(response);
        }

        //Adds product type association entries.
        public virtual ProductTypeAssociationListModel AssociateProducts(ProductTypeAssociationListModel model)
        {
            string endpoint = ProductsEndpoint.AssociateProducts();

            ApiStatus status = new ApiStatus();
            ProductTypeAssociationListResponse response = PostResourceToEndpoint<ProductTypeAssociationListResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            //check status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            ProductTypeAssociationListModel list = new ProductTypeAssociationListModel() { AssociatedProducts = response.AssociatedProductList };

            list.MapPagingDataFromResponse(response);
            return list;
        }

        // Removes a product type association entry.
        public virtual bool UnassociateProduct(ParameterModel productTypeAssociationId)
        {
            string endpoint = ProductsEndpoint.UnassociateProduct();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(productTypeAssociationId), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        #endregion Product Type

        #region Link Products

        // Gets list of products which are associated as link product to the parent product.
        public virtual ProductDetailsListModel GetAssignedLinkProducts(int parentProductId, int linkAttributeId, ExpandCollection expands, FilterCollection filters, SortCollection sorts)
            => GetAssignedLinkProducts(parentProductId, linkAttributeId, expands, filters, sorts, null, null);

        // Gets list of products which are associated as Add-ons to the parent product.
        public virtual ProductDetailsListModel GetAssignedLinkProducts(int parentProductId, int linkAttributeId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = ProductsEndpoint.GetAssociatedLinkProducts(parentProductId, linkAttributeId);
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ProductListResponse response = GetResourceFromEndpoint<ProductListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return GetProductDetailsListModelByResponse(response);
        }

        //Gets list of list of products which are not associated as link products to the parent product.
        public virtual ProductDetailsListModel GetUnassignedLinkProducts(int parentProductId, int linkAttributeId, ExpandCollection expands, FilterCollection filters, SortCollection sorts)
            => GetUnassignedLinkProducts(parentProductId, linkAttributeId, expands, filters, sorts, null, null);

        // Gets list of list of products which are not associated as link products to the parent product.
        public virtual ProductDetailsListModel GetUnassignedLinkProducts(int parentProductId, int linkAttributeId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = ProductsEndpoint.GetUnassociatedLinkProducts(parentProductId, linkAttributeId);
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ProductListResponse response = GetResourceFromEndpoint<ProductListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return GetProductDetailsListModelByResponse(response);
        }

        // Adds a product link product association entry.
        public virtual LinkProductDetailListModel AssignLinkProducts(LinkProductDetailListModel model)
        {
            string endpoint = ProductsEndpoint.AssignLinkProducts();

            ApiStatus status = new ApiStatus();
            LinkProductDetailListResponse response = PostResourceToEndpoint<LinkProductDetailListResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            LinkProductDetailListModel list = new LinkProductDetailListModel { LinkProductDetailList = response?.LinkProducts };

            list.MapPagingDataFromResponse(response);
            return list;
        }

        //Unassign Link products
        public virtual bool UnassignLinkProducts(ParameterModel linkProductDetailId)
        {
            string endpoint = ProductsEndpoint.UnassignLinkProducts();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(linkProductDetailId), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        #endregion Link Products

        #region Custom Field

        // Creates a CustomField.
        public virtual CustomFieldModel AddCustomField(CustomFieldModel customFieldModel)
        {
            //Get Endpoint.
            string endpoint = ProductsEndpoint.CreateCustomField();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            CustomFieldResponse response = PostResourceToEndpoint<CustomFieldResponse>(endpoint, JsonConvert.SerializeObject(customFieldModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.CustomField;
        }

        //Get custom field to edit
        public virtual CustomFieldModel GetCustomField(int customFieldId, ExpandCollection expands)
        {
            string endpoint = ProductsEndpoint.GetCustomField(customFieldId);
            endpoint += BuildEndpointQueryString(expands);

            ApiStatus status = new ApiStatus();
            CustomFieldResponse response = GetResourceFromEndpoint<CustomFieldResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.CustomField;
        }

        //Get custom field list associated to product.
        public virtual CustomFieldListModel GetCustomFields(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = ProductsEndpoint.CustomFieldList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            CustomFieldListResponse response = GetResourceFromEndpoint<CustomFieldListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            CustomFieldListModel customFieldListModel = new CustomFieldListModel { CustomFields = response?.CustomFields };
            customFieldListModel.MapPagingDataFromResponse(response);

            return customFieldListModel;
        }

        //Update custom fields
        public virtual CustomFieldModel UpdateCustomField(CustomFieldModel customFieldModel)
        {
            string endpoint = ProductsEndpoint.UpdateCustomField();

            ApiStatus status = new ApiStatus();
            CustomFieldResponse response = PutResourceToEndpoint<CustomFieldResponse>(endpoint, JsonConvert.SerializeObject(customFieldModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.CustomField;
        }

        //delete custom field
        public virtual bool DeleteCustomField(string customFieldId)
        {
            string endpoint = ProductsEndpoint.DeleteCustomField();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(new ParameterModel() { Ids = customFieldId }), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        #endregion Custom Field

        #region Associate Addon

        //Associate addon
        public virtual AddonProductListModel AssociateAddon(AddonProductListModel model)
        {
            string endpoint = ProductsEndpoint.AssociateAddon();

            ApiStatus status = new ApiStatus();
            AddonProductListResponse response = PostResourceToEndpoint<AddonProductListResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            AddonProductListModel list = new AddonProductListModel { AddonProducts = response?.AddonProducts };

            list.MapPagingDataFromResponse(response);
            return list;
        }

        //Delete associated addons by addonProductId
        public virtual bool DeleteAssociatedAddons(ParameterModel addonProductId)
        {
            string endpoint = ProductsEndpoint.DeleteAssociatedAddons();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(addonProductId), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Get list of associated addons
        public virtual AddonGroupListModel GetAssociatedAddonDetails(int parentProductId, ExpandCollection expands, FilterCollection filters, SortCollection sorts)
             => GetAssociatedAddonDetails(parentProductId, expands, filters, sorts, null, null);

        //Get associated addon details
        public virtual AddonGroupListModel GetAssociatedAddonDetails(int parentProductId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = ProductsEndpoint.GetAssociatedAddonDetails(parentProductId);
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            AddonGroupListResponse response = GetResourceFromEndpoint<AddonGroupListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            AddonGroupListModel list = new AddonGroupListModel { AddonGroups = response?.AddonGroups };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Create or associate addon product.
        public virtual AddOnProductDetailListModel CreateAddonProductDetail(AddOnProductDetailListModel model)
        {
            string endpoint = ProductsEndpoint.CreateAddonProductDetail();

            ApiStatus status = new ApiStatus();
            AddOnProductDetailListResponse response = PostResourceToEndpoint<AddOnProductDetailListResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            AddOnProductDetailListModel list = new AddOnProductDetailListModel { AddOnProductDetailList = response?.AddOnProductDetails };

            list.MapPagingDataFromResponse(response);
            return list;
        }

        //Update addon product Display order.
        public virtual AddOnProductDetailModel UpdateAddonDisplayOrder(AddOnProductDetailModel addOnProductDetailModel)
        {
            string endpoint = ProductsEndpoint.UpdateAddonDisplayOrder();

            ApiStatus status = new ApiStatus();
            AddonDisplayOrderResponse response = PutResourceToEndpoint<AddonDisplayOrderResponse>(endpoint, JsonConvert.SerializeObject(addOnProductDetailModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.AddonProductDisplayOrderResponse;
        }

        //Delete associated addon product.
        public virtual bool DeleteAddonProductDetails(ParameterModel addonProductDetailIds)
        {
            string endpoint = ProductsEndpoint.DeleteAddonProductDetails();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(addonProductDetailIds), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Get list of unassociated addonGroup.
        public virtual AddonGroupListModel GetUnassociatedAddonGroups(int parentProductId, ExpandCollection expands, FilterCollection filters, SortCollection sorts)
            => GetUnassociatedAddonGroups(parentProductId, expands, filters, sorts, null, null);

        //Get un-associated add-on group
        public virtual AddonGroupListModel GetUnassociatedAddonGroups(int parentProductId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = ProductsEndpoint.GetUnassociatedAddonGroups(parentProductId);
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            AddonGroupListResponse response = GetResourceFromEndpoint<AddonGroupListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            AddonGroupListModel list = new AddonGroupListModel { AddonGroups = response?.AddonGroups };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get list of unassociated addon product.
        public virtual ProductDetailsListModel GetUnassociatedAddonProducts(int addonProductId, ExpandCollection expands, FilterCollection filters, SortCollection sorts)
            => GetUnassociatedAddonProducts(addonProductId, expands, filters, sorts, null, null);

        //Get un-associated addon product list
        public virtual ProductDetailsListModel GetUnassociatedAddonProducts(int addonProductId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = ProductsEndpoint.GetUnassociatedAddonProducts(addonProductId);
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ProductListResponse response = GetResourceFromEndpoint<ProductListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return GetProductDetailsListModelByResponse(response);
        }

        //Update addon product association.
        public virtual AddOnProductModel UpdateProductAddonAssociation(AddOnProductModel addonProductModel)
        {
            string endpoint = ProductsEndpoint.UpdateProductAddonAssociation();

            ApiStatus status = new ApiStatus();
            AddonProductResponse response = PutResourceToEndpoint<AddonProductResponse>(endpoint, JsonConvert.SerializeObject(addonProductModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.AddonProduct;
        }

        #endregion Associate Addon

        // Gets a product by product ID.
        public virtual ProductTypeAssociationModel GetAssociatedProduct(int pimProductTypeAssociationId)
        {
            string endpoint = ProductsEndpoint.GetAssociatedProduct(pimProductTypeAssociationId);

            ApiStatus status = new ApiStatus();
            ProductTypeAssociationResponse response = GetResourceFromEndpoint<ProductTypeAssociationResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.ProductTypeAssociation;
        }

        //Update associated product
        public virtual ProductTypeAssociationModel UpdateAssociatedProduct(ProductTypeAssociationModel productTypeAssociationModel)
        {
            string endpoint = ProductsEndpoint.UpdateAssociatedProduct();

            ApiStatus status = new ApiStatus();
            ProductTypeAssociationResponse response = PutResourceToEndpoint<ProductTypeAssociationResponse>(endpoint, JsonConvert.SerializeObject(productTypeAssociationModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.ProductTypeAssociation;
        }

        #region Publish Product

        public virtual PublishedModel PublishProduct(ParameterModel parameterModel)
        {
            string endpoint = ProductsEndpoint.Publish();

            ApiStatus status = new ApiStatus();
            PublishedResponse response = PostResourceToEndpoint<PublishedResponse>(endpoint, JsonConvert.SerializeObject(parameterModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.PublishedModel;
        }

        #endregion Publish Product

        #region Product SKU list for Autocomplete feature

        public virtual PIMProductAttributeValuesListModel GetProductSKUsByAttributeCode(string attributeValue, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = ProductsEndpoint.GetProductSKUsByAttributeCode(attributeValue);
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            PIMProductAttributeValuesListResponse response = GetResourceFromEndpoint<PIMProductAttributeValuesListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PIMProductAttributeValuesListModel list = new PIMProductAttributeValuesListModel { ProductAttributeValues = (Equals(response, null)) ? null : response.AttributeValues };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        #endregion Product SKU list for Autocomplete feature

        #region Product Update Import
        //Post and process the update product import data
        public virtual bool ImportProductUpdateData(ImportModel model)
        {
            string endpoint = ProductsEndpoint.ImportProductUpdateData();
            endpoint += BuildEndpointQueryString(null, null, null, null, null);

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.IsSuccess;
        }

        #endregion
    }
}