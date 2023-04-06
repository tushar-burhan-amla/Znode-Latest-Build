using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class ProductCache : BaseCache, IProductCache
    {
        #region Private Variables

        private readonly IProductService _service;

        #endregion Private Variables

        #region Public Constructor

        public ProductCache(IProductService productService)
        {
            _service = productService;
        }

        #endregion Public Constructor

        #region Public Methods

        //Get Product by Product Id
        public virtual string GetProduct(PIMGetProductModel model, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (Equals(data, null))
            {
                PIMFamilyDetailsModel product = _service.GetProduct(model);
                if (!Equals(product, null))
                {
                    PIMAttributeFamilyResponse response = new PIMAttributeFamilyResponse { PIMFamilyDetails = product };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get Products From Cache
        public virtual string GetProducts(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);

            //If Data From Cache Is Null Then Give Service call
            if (string.IsNullOrEmpty(data))
            {
                ProductDetailsListModel productList = _service.GetProductList(Expands, Filters, Sorts, Page);
                //Generate Response
                ProductListResponse response = GetProductListResponseFromDetailListModel(productList);

                data = InsertIntoCache(routeUri, routeTemplate, response);
            }
            return data;
        }

        //Get Products From Cache
        public virtual string GetBrandProducts(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);

            //If Data From Cache Is Null Then Give Service call
            if (string.IsNullOrEmpty(data))
            {
                ProductDetailsListModel productList = _service.GetProductBrandList(Expands, Filters, Sorts, Page);
                //Generate Response
                ProductListResponse response = GetProductListResponseFromDetailListModel(productList);

                data = InsertIntoCache(routeUri, routeTemplate, response);
            }
            return data;
        }

        //Generate response
        public ProductListResponse GetProductListResponseFromDetailListModel(ProductDetailsListModel productList)
        {
            ProductListResponse response = new ProductListResponse
            {
                ProductDetails = productList.ProductDetailList,
                Locale = productList.Locale,
                AttrubuteColumnName = productList.AttributeColumnName,
                XmlDataList = productList.XmlDataList,
                ProductDetailsDynamic = productList?.ProductDetailListDynamic,
                NewAttributeList = productList?.NewAttributeList
            };
            response.MapPagingDataFromModel(productList);
            return response;
        }

        //Get Products From Cache
        public virtual string GetProducts(ParameterModel parameter, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ProductDetailsListModel productList = _service.GetProductList(parameter, Expands, Filters, Sorts, Page);
                //Generate Response
                ProductListResponse response = GetProductListResponseFromDetailListModel(productList);

                data = InsertIntoCache(routeUri, routeTemplate, response);
            }
            return data;
        }

        //Get associated or unassociated Products with Category.
        public virtual string GetAssociatedUnAssociatedCategoryProducts(int categoryId, bool associatedProduts, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                CategoryProductListModel list = _service.GetAssociatedUnAssociatedCategoryProducts(categoryId, associatedProduts, Expands, Filters, Sorts, Page);
                if (list?.CategoryProducts?.Count > 0)
                {
                    CategoryProductListResponse response = new CategoryProductListResponse { CategoryProducts = list?.CategoryProducts };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        // Get products those are not associated with parent productId on family basis.
        public virtual string GetAssociatedUnAssociatedConfigureProducts(int parentProductId, string associatedProductIds, string associatedAttributeIds, bool pimProductIdsIn, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (Equals(data, null))
            {
                ProductDetailsListModel associatedProducts = _service.GetAssociatedUnAssociatedConfigureProducts(parentProductId, associatedProductIds, associatedAttributeIds, pimProductIdsIn, Expands, Filters, Sorts, Page);
                if (!Equals(associatedProducts, null))
                {
                    //Generate Response
                    ProductListResponse response = GetProductListResponseFromDetailListModel(associatedProducts);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        #region Product Type

        // Get products associated with parent productId.
        public virtual string GetAssociatedProducts(int parentProductId, int attributeId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (Equals(data, null))
            {
                ProductDetailsListModel associatedProducts = _service.GetAssociatedProducts(parentProductId, attributeId, Expands, Filters, Sorts, Page);
                if (!Equals(associatedProducts, null))
                {
                    //Generate Response
                    ProductListResponse response = GetProductListResponseFromDetailListModel(associatedProducts);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        // Get products those are not associated with parent productId.
        public virtual string GetUnassociatedProducts(int parentProductId, string associatedProductIds, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (Equals(data, null))
            {
                ProductDetailsListModel unassociatedProducts = _service.GetUnassociatedProducts(parentProductId, associatedProductIds, Expands, Filters, Sorts, Page);
                if (!Equals(unassociatedProducts, null))
                {
                    //Generate Response
                    ProductListResponse response = GetProductListResponseFromDetailListModel(unassociatedProducts);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        #endregion Product Type

        #region Addon Details

        //Get associated addon
        public virtual string GetAssociatedAddonDetails(int productId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (Equals(data, null))
            {
                AddonGroupListModel associatedAddonGroups = _service.GetAssociatedAddonDetails(productId, Expands, Filters, Sorts, Page);
                if (!Equals(associatedAddonGroups, null))
                {
                    //Create response.
                    AddonGroupListResponse response = new AddonGroupListResponse { AddonGroups = associatedAddonGroups.AddonGroups };
                    response.MapPagingDataFromModel(associatedAddonGroups);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get unassociated addon groups
        public virtual string GetUnassociatedAddonGroups(int productId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (Equals(data, null))
            {
                AddonGroupListModel unassociatedAddonGroups = _service.GetUnassociatedAddonGroups(productId, Expands, Filters, Sorts, Page);
                if (!Equals(unassociatedAddonGroups, null))
                {
                    //Create response.
                    AddonGroupListResponse response = new AddonGroupListResponse { AddonGroups = unassociatedAddonGroups.AddonGroups };
                    response.MapPagingDataFromModel(unassociatedAddonGroups);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get unassociated addon products
        public virtual string GetUnassociatedAddonProducts(int addonProductId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (Equals(data, null))
            {
                ProductDetailsListModel unassociatedProducts = _service.UnassociatedAddonProducts(addonProductId, Expands, Filters, Sorts, Page);
                if (!Equals(unassociatedProducts, null))
                {
                    //Generate Response
                    ProductListResponse response = GetProductListResponseFromDetailListModel(unassociatedProducts);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        #endregion Addon Details

        #region Link product Details

        //Get associated link products.
        public virtual string GetAssociatedLinkProducts(int parentProductId, int linkAttributeId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (Equals(data, null))
            {
                ProductDetailsListModel associatedProducts = _service.GetAssociatedLinkProducts(parentProductId, linkAttributeId, Expands, Filters, Sorts, Page);

                //Generate Response
                ProductListResponse response = GetProductListResponseFromDetailListModel(associatedProducts);

                data = InsertIntoCache(routeUri, routeTemplate, response);
            }
            return data;
        }

        //Gets list of list of products which are not associated as link products to the parent product.
        public virtual string GetUnassociatedLinkProducts(int parentProductId, int linkAttributeId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (Equals(data, null))
            {
                ProductDetailsListModel unassociatedProducts = _service.GetUnAssociatedLinkProducts(parentProductId, linkAttributeId, Expands, Filters, Sorts, Page);

                //Generate Response
                ProductListResponse response = GetProductListResponseFromDetailListModel(unassociatedProducts);

                data = InsertIntoCache(routeUri, routeTemplate, response);
            }
            return data;
        }

        #endregion Link product Details

        #region Custom Field

        //Get Custom Field From Cache by Custom Field Id.
        public virtual string GetCustomField(int customFieldId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                CustomFieldModel customField = _service.GetCustomField(customFieldId, Expands);
                if (!Equals(customField, null))
                {
                    CustomFieldResponse response = new CustomFieldResponse { CustomField = customField };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get Custom Field List From Cache.
        public virtual string GetCustomFields(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                CustomFieldListModel list = _service.GetCustomFieldList(Expands, Filters, Sorts, Page);
                if (list?.CustomFields?.Count > 0)
                {
                    CustomFieldListResponse response = new CustomFieldListResponse { CustomFields = list.CustomFields };
                    response.MapPagingDataFromModel(list);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        #endregion Custom Field

        //Get assigned personalized attributes list
        public virtual string GetAssignedPersonalizedAttributes(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PIMProductAttributeValuesListModel assignedPersonalizedAttributes = _service.GetAssignedPersonalizedAttributes(Expands, Filters, Sorts);
                if (!Equals(assignedPersonalizedAttributes, null))
                {
                    //Create response.
                    ProductListResponse response = new ProductListResponse { ProductAttributeValues = assignedPersonalizedAttributes };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get list of un-assigned personalized attributes
        public virtual string GetUnassignedPersonalizedAttributes(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PIMAttributeListModel assignedPersonalizedAttributes = _service.GetUnassignedPersonalizedAttributes(Expands, Filters, Sorts);
                if (!Equals(assignedPersonalizedAttributes, null))
                {
                    //Create response.
                    PIMAttributeListResponse response = new PIMAttributeListResponse { Attributes = assignedPersonalizedAttributes.Attributes };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get associated product using pimProductTypeAssociationId.
        public virtual string GetAssociatedProduct(int pimProductTypeAssociationId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ProductTypeAssociationModel productTypeAssociationModel = _service.GetAssociatedProduct(pimProductTypeAssociationId);
                if (!Equals(productTypeAssociationModel, null))
                {
                    ProductTypeAssociationResponse response = new ProductTypeAssociationResponse { ProductTypeAssociation = productTypeAssociationModel };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Assign personalized attributes
        public virtual string AssignedPersonalizedAttribute(PIMAttributeValueListModel model, string routeUri, string routeTemplate)
        {
            bool isPersonalizedAttributeAssigned = _service.AssignPersonalizedAttributes(model, Filters);
            TrueFalseResponse response = new TrueFalseResponse { IsSuccess = isPersonalizedAttributeAssigned };
            return InsertIntoCache(routeUri, routeTemplate, response);
        }

        #region Product SKU list for Autocomplete feature

        //Get product sku list for attribute code as sku and its attribute value.
        public virtual string GetProductSKUsByAttributeCode(string attributeValue, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PIMProductAttributeValuesListModel list = _service.GetProductSKUsByAttributeCode(attributeValue, Expands, Filters, Sorts, Page);
                if (list?.ProductAttributeValues?.Count > 0)
                {
                    PIMProductAttributeValuesListResponse response = new PIMProductAttributeValuesListResponse { AttributeValues = list?.ProductAttributeValues };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        #endregion Product SKU list for Autocomplete feature        
        #endregion Public Methods
    }
}