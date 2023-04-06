using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Znode.Engine.Api.Models;
using Znode.Libraries.Admin;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public partial class ProductService : BaseService
    {
        #region Public Methods
        //Get Product List
        public virtual WebStoreProductListModel ProductList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);

            WebStoreProductListModel productListModel = new WebStoreProductListModel();

            SetLocaleFilterIfNotPresent(ref filters);
            SetVersionFilterIfNotPresent(ref filters);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel generated to get ProductList: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            productListModel.ProductList = GetService<IPublishedProductDataService>().GetPublishProducts(pageListModel)?.ToModel<WebStoreProductModel>()?.ToList();

            ZnodeLogging.LogMessage("ProductList count: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, productListModel?.ProductList?.Count);
            productListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return productListModel;
        }

        //Get product by id.
        public virtual WebStoreProductModel GetProduct(int productId, NameValueCollection expands)
        {
            //To do 
            WebStoreProductModel productModel = new WebStoreProductModel();
            return productModel;
        }

        //Get associated product
        public virtual WebStoreProductListModel GetAssociatedProducts(ParameterModel productIDs)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter productIDs to get associated products: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, productIDs?.Ids);
            WebStoreProductListModel productListModel = new WebStoreProductListModel();
            FilterCollection filters = new FilterCollection();
            filters.Add(ZnodePublishProductEnum.PublishProductId.ToString(), FilterOperators.In, productIDs.Ids);
            SetLocaleFilterIfNotPresent(ref filters);
            SetVersionFilterIfNotPresent(ref filters);
            ZnodeLogging.LogMessage("ProductList count: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, productListModel?.ProductList?.Count);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return productListModel;
        }

        //Get product by product sku.
        public virtual WebStoreProductListModel GetProductsBySkus(ParameterModel skus)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            WebStoreProductListModel productListModel = new WebStoreProductListModel();
            FilterCollection filters = new FilterCollection();
            filters.Add(ZnodeInventoryEnum.SKU.ToString(), FilterOperators.In, skus.Ids);
            SetLocaleFilterIfNotPresent(ref filters);
            SetVersionFilterIfNotPresent(ref filters);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return productListModel;
        }

        //Get product highlightlist.
        public virtual HighlightListModel GetProductHighlights(ParameterProductModel parameterModel, int productId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters productId and parameterModel property HighLightsCodes: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new object[] { productId, parameterModel?.HighLightsCodes });

            FilterCollection filter = new FilterCollection();
            filter.Add(ZnodeHighlightEnum.HighlightCode.ToString(), FilterOperators.In, parameterModel.HighLightsCodes);
            filter.Add(FilterKeys.LocaleId, FilterOperators.Equals, parameterModel.LocaleId.ToString());
            filter.Add(FilterKeys.IsActive, FilterOperators.Is, ZnodeConstant.TrueValue);

            NameValueCollection sorts = new NameValueCollection();
            sorts.Add(ZnodeHighlightEnum.DisplayOrder.ToString(), "asc");

            HighlightListModel model = new HighlightService().GetHighlightList(null, filter, sorts, null);
            ZnodeLogging.LogMessage("HighlightCodes and HighlightList count: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { HighlightCodesCount = model?.HighlightCodes?.Count, HighlightListCount = model?.HighlightList?.Count });
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return model;
        }

        //Send Product Compare Mail.
        public virtual bool SendComparedProductMail(ProductCompareModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("PortalId, WebstoreDomainName and WebstoreDomainScheme properties of input parameter ProductCompareModel: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { PortalId = model?.PortalId, WebstoreDomainName = model?.WebstoreDomainName, WebstoreDomainScheme = model?.WebstoreDomainScheme });

            string senderEmail = model.SenderEmailAddress;
            string subject = $"{senderEmail.Split('@').FirstOrDefault()}  wants you to see this item";

            if (!model.IsProductDetails)
            {

                string templatePath = string.Empty;
                //Method to get Email Template.
                EmailTemplateMapperModel emailTemplateMapperModel = GetEmailTemplateByCode(ZnodeConstant.ProductComparisonDetails, (model.PortalId > 0) ? model.PortalId : PortalId, model.LocaleId);

                if (HelperUtility.IsNotNull(emailTemplateMapperModel))
                {
                    List<PublishProductModel> products = GetCompareProductList(model);

                    templatePath = GetDynamicHtmlForTemplate(templatePath, emailTemplateMapperModel, products);

                    DataTable productDetailItems = BindDataToTabelRow(products, model.WebstoreDomainName,model.WebstoreDomainScheme, model.IsShowPriceAndInventoryToLoggedInUsersOnly);

                    ZnodeReceiptHelper receiptHelper = new ZnodeReceiptHelper(emailTemplateMapperModel.Descriptions);

                    string messageText = GetMessageText(model, productDetailItems, receiptHelper);

                    ZnodeLogging.LogMessage("Input parameters senderEmail, ReceiverEmailAddress, subject, messageText, IsEnableBcc, PortalId of method SendMail: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new object[] { senderEmail, model?.ReceiverEmailAddress, subject, messageText, emailTemplateMapperModel?.IsEnableBcc, model?.PortalId });

                    return SendEmailToFriend( model.ReceiverEmailAddress, subject, messageText,emailTemplateMapperModel.IsEnableBcc, model.PortalId);
                }
            }
            return false;
        }

        //Send Email To Friend.
        public virtual bool SendMailToFriend(EmailAFriendListModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);

            string senderEmail = model.YourMailId;
            string subject = $"{senderEmail.Split('@').FirstOrDefault()}  wants you to see this item at {ZnodeConfigManager.SiteConfig.StoreName}";

            //Method to get Email Template.
            EmailTemplateMapperModel emailTemplateMapperModel = GetEmailTemplateByCode(ZnodeConstant.EmailAFriend, (model.PortalId > 0) ? model.PortalId : PortalId, model.LocaleId);
            if (HelperUtility.IsNull(emailTemplateMapperModel))
                throw (new Exception("Not Implemented"));
            else
            {
                ZnodeReceiptHelper receiptHelper = new ZnodeReceiptHelper(emailTemplateMapperModel.Descriptions);

                string messageText = GetEmailTemplate(model, receiptHelper);

                ZnodeLogging.LogMessage("Input parameters senderEmail, FriendMailId, subject, messageText, IsEnableBcc, PortalId of method SendMail: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new object[] { senderEmail, model?.FriendMailId, subject, messageText, emailTemplateMapperModel?.IsEnableBcc, model?.PortalId });

                return SendEmailToFriend(model.FriendMailId, subject, messageText, emailTemplateMapperModel.IsEnableBcc, model.PortalId);
            }
        }

        //Get List Of Compare Product
        public virtual List<PublishProductModel> GetCompareProductList(ProductCompareModel model)
        {
            NameValueCollection expands = new NameValueCollection();
            expands.Add(ZnodeConstant.Pricing, ZnodeConstant.Pricing);
            expands.Add(ZnodeConstant.SEO, ZnodeConstant.SEO);
            expands.Add(ZnodeConstant.ConfigurableAttribute, ZnodeConstant.ConfigurableAttribute);

            FilterCollection filters = new FilterCollection();
            filters.Add(FilterKeys.LocaleId, FilterOperators.Equals, model.LocaleId.ToString());
            filters.Add(FilterKeys.PortalId, FilterOperators.Equals, model.PortalId.ToString());
            filters.Add(FilterKeys.ZnodeProductId, FilterOperators.In, model.ProductIds);
            filters.Add(FilterKeys.VersionId, FilterOperators.Equals, GetCatalogVersionId().ToString());
            IPublishProductService publishProductService = GetService<IPublishProductService>();
            List<PublishProductModel> products = publishProductService.GetPublishProductList(expands, filters, new NameValueCollection(), new NameValueCollection())?.PublishProducts;

            ZnodeLogging.LogMessage("PublishProductModel list count: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, products?.Count);
            return products;
        }

        //For replacing tags
        private string GetEmailTemplate(EmailAFriendListModel model, ZnodeReceiptHelper receiptHelper)
        {
            string messageText = receiptHelper.Output;

            Regex rx1 = new Regex("#ProductLink#", RegexOptions.IgnoreCase);
            messageText = rx1.Replace(HelperUtility.IsNull(messageText) ? string.Empty : messageText, "<a href=" + model.ProductUrl + ">" + model.ProductName + "</a>");

            rx1 = new Regex("#CatalogName#", RegexOptions.IgnoreCase);
            messageText = rx1.Replace(HelperUtility.IsNull(messageText) ? string.Empty : messageText, ZnodeConfigManager.SiteConfig.CompanyName);

            string storeLogoPath = GetCustomPortalDetails(model.PortalId)?.StoreLogo;

            rx1 = new Regex("#StoreLogo#", RegexOptions.IgnoreCase);
            messageText = rx1.Replace(HelperUtility.IsNull(messageText) ? string.Empty : messageText, storeLogoPath);

            messageText = GetService<IEmailTemplateSharedService>().ReplaceTemplateTokens(messageText, new Dictionary<string, string>());
            return messageText;
        }

        //Replace remaining values and get message text.
        protected virtual string GetMessageText(ProductCompareModel model, DataTable productDetailItems, ZnodeReceiptHelper receiptHelper)
        {
            receiptHelper.Parse("ComparedProducts", productDetailItems.CreateDataReader());
            string messageText = receiptHelper.Output;

            Regex rx1 = new Regex("#CustomerServiceEmail#", RegexOptions.IgnoreCase);
            messageText = rx1.Replace(messageText, ZnodeConfigManager.SiteConfig.CustomerServiceEmail.Replace(",", ", "));

            rx1 = new Regex("#CustomerServicePhoneNumber#", RegexOptions.IgnoreCase);
            messageText = rx1.Replace(messageText, ZnodeConfigManager.SiteConfig.CustomerServicePhoneNumber);

            string storeLogoPath = GetStoreLogoPath(model.PortalId);
            ZnodeLogging.LogMessage("storeLogoPath returned from method GetStoreLogoPath: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, storeLogoPath);

            rx1 = new Regex("#StoreLogo#", RegexOptions.IgnoreCase);
            messageText = rx1.Replace(messageText, storeLogoPath);

            messageText = HttpUtility.HtmlDecode(GetService<IEmailTemplateSharedService>().ReplaceTemplateTokens(messageText, new Dictionary<string, string>()));
            return messageText;
        }

        //Send Mail For Product Compare.
        private static bool SendMail(string senderEmail, string receiverEmail, string subject, string messageText,bool isEnableBcc,int portalId)
        {
            try
            {
                ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
                ZnodeEmail.SendEmail(receiverEmail, senderEmail, ZnodeEmail.GetBccEmail(isEnableBcc, portalId, string.Empty), subject, messageText, true);
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
                return true;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                return false;
            }
        }

        protected static bool SendEmailToFriend(string receiverEmail, string subject, string messageText, bool isEnableBcc, int portalId)
        {
            try
            {
                ZnodePortalSmtpSetting smtpSettings = ZnodeEmail.GetSMTPSetting(portalId);
                string bccEmailId = string.Empty;
                if (isEnableBcc && HelperUtility.IsNotNull(smtpSettings))
                    bccEmailId = smtpSettings.BccEmailAddress;
                ZnodeEmail.SendEmail(receiverEmail, smtpSettings?.FromEmailAddress, bccEmailId, subject, messageText, true);
                return true;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                return false;
            }
        }
        //Bind Data To Table Row.
        protected virtual DataTable BindDataToTabelRow(List<PublishProductModel> products, string webstoreDomainName, string webstoreDomainScheme, bool isShowPriceToLoggedInUsersOnly = false)
        {
            DataTable productDetailItems = new DataTable();
            productDetailItems.Columns.Add("Image");
            productDetailItems.Columns.Add("ProductName");
            productDetailItems.Columns.Add("Price");
            productDetailItems.Columns.Add("Variants");

            foreach (PublishProductModel product in products)
            {
                product.Attributes.RemoveAll(x => x.AttributeCode == "Image" || x.AttributeCode == "Price" || !x.IsComparable && !x.IsConfigurable);
                if (HelperUtility.IsNotNull(productDetailItems))
                {
                    DataRow productImageRow = productDetailItems.NewRow();
                    string ImageUrl = !string.IsNullOrEmpty(product.ImageThumbNailPath) ? product.ImageThumbNailPath.Replace(" ", "%20") : "";
                    productImageRow["Image"] = "<img src=" + ImageUrl + ">";
                    productImageRow["ProductName"] = (HelperUtility.IsNotNull(product.Name) || !Equals(product.Name, string.Empty)) ? $"<a href='{webstoreDomainScheme}://{webstoreDomainName}/{(string.IsNullOrEmpty(product?.SEOUrl) ? "product/" + product.PublishProductId : product.SEOUrl)}'>{product.Name}</a>" : "-";
                    if (isShowPriceToLoggedInUsersOnly)
                        productImageRow["Price"] = Api_Resources.PricingForLoggedInUsersMessage;
                    else
                        productImageRow["Price"] = !string.IsNullOrEmpty(product.GroupProductPriceMessage) ? product.GroupProductPriceMessage : (HelperUtility.IsNull(product.SalesPrice) ? ServiceHelper.FormatPriceWithCurrency(product.RetailPrice, string.IsNullOrEmpty(product.CultureCode) ? GetDefaultCulture() : product.CultureCode) : ServiceHelper.FormatPriceWithCurrency(product.SalesPrice, string.IsNullOrEmpty(product.CultureCode) ? GetDefaultCulture() : product.CultureCode));
                    productImageRow["Variants"] = !string.IsNullOrEmpty(GetProductVariants(product)) ? GetProductVariants(product) : "NA";

                    foreach (PublishAttributeModel attribute in product.Attributes)
                    {
                        DataColumnCollection columns = productDetailItems.Columns;
                        if (!columns.Contains(attribute.AttributeName))
                            productDetailItems.Columns.Add(attribute.AttributeName);
                        if (attribute?.SelectValues != null && attribute.SelectValues.Count > 0)
                            productImageRow[attribute.AttributeName] = attribute.SelectValues.FirstOrDefault()?.Value;
                        else
                            productImageRow[attribute.AttributeName] = attribute.AttributeValues;
                    }
                    productDetailItems.Rows.Add(productImageRow);
                }
            }
            return productDetailItems;
        }

        //Get product variants for configurable product.
        protected virtual string GetProductVariants(PublishProductModel product)
        {
            List<PublishAttributeModel> variants = product?.Attributes?.Where(x => x.IsConfigurable).ToList();
            ZnodeLogging.LogMessage("Product variants list count for configurable product: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, variants?.Count);

            string productType = product?.Attributes?.SelectAttributeList(ZnodeConstant.ProductType)?.FirstOrDefault()?.Code;
            string variantData = string.Empty;
            string variantHTML = string.Empty;
            if (Equals(productType, ZnodeConstant.ConfigurableProduct) && variants?.Count > 0)
            {
                //Bind variant html for the product.
                variantHTML = "<ul style='padding:0;margin:0 10px 0 0;list-style:none;border:1px solid #c3c3c3;border-bottom:0;'>";
                foreach (var item in variants)
                {
                    if (item.ConfigurableAttribute?.Count > 0)
                    {
                        variantData += $"<li style='padding:3px 5px;margin:0;border-bottom:1px solid #c3c3c3;'><span><strong> {item.AttributeName}</strong></span>:&nbsp;</span><span> {string.Join(", ", item.ConfigurableAttribute.Select(x => x.AttributeValue))}</span></li>";
                    }
                }
                return $"{variantHTML}{variantData}</ul>";
            }
            return null;
        }

        //Get Store Logo Path.
        protected virtual string GetStoreLogoPath(int portalId)
        {
            ZnodeLogging.LogMessage("Input parameter portalId: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, portalId);
            string storeLogo = GetService<IPublishedPortalDataService>().GetWebstorePortalDetails(portalId)?.WebsiteLogo;
            MediaConfigurationModel configurationModel = GetService<IMediaConfigurationService>().GetDefaultMediaConfiguration();
            string serverPath = GetMediaServerUrl(configurationModel);
            string thumbnailPath = $"{serverPath}{configurationModel.ThumbnailFolderName}";
            return $"{thumbnailPath}/{storeLogo}";
        }

        #endregion

        #region Private Methods
        //Get Dynamic Html for Email Template.
        protected  static string GetDynamicHtmlForTemplate(string templatePath, EmailTemplateMapperModel emailTemplateMapperModel, List<PublishProductModel> products)
        {
            List<PublishAttributeModel> comparableAttributeList = products.SelectMany(x => x.Attributes.Where(d => d.IsComparable)).GroupBy(z => z.AttributeCode).Select(grp => grp.First()).ToList();

            foreach (PublishAttributeModel item in comparableAttributeList)
            {
                if (!Equals(item.AttributeCode, ZnodeConstant.ProductType) && !Equals(item.AttributeCode, ZnodeConstant.ProductImage) && !Equals(item.AttributeCode, ZnodeConstant.FrequentlyBought) && !Equals(item.AttributeCode, ZnodeConstant.ProductName))
                    templatePath = templatePath + $"<div style=padding:5px 0;width:100%><div style=color:#af0604;font-weight:bold;>{item.AttributeName}</div><div>{"#"}{ item.AttributeName}{"#"}</div></div>";
            }
            emailTemplateMapperModel.Descriptions = emailTemplateMapperModel?.Descriptions.Replace("DyanamicHtml", templatePath);
            return templatePath;
        }
        #endregion
    }
}

