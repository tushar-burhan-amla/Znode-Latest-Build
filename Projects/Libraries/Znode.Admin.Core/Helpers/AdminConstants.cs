namespace Znode.Engine.Admin.Helpers
{
    public struct AdminConstants
    {
        public const string LoginCookieNameValue = "loginCookie";
        public const string AreaKey = "area";
        public const string Controller = "controller";
        public const string Action = "action";
        public const string UserPermissionListSessionKey = "UserPermissionList";
        public const string IsAdminUserSessionKey = "IsAdminUser";
        public const string UserAccountSessionKey = "UserAccount";
        public const string ErrorCodeSessionKey = "ErrorCode";
        public const string UserName = "UserName";
        public const string SuccessMessage = "SuccessMessage";
        public const string Notifications = "Notifications";
        public const string DetailsView = "Detail";
        public const string DefaultMediaFolder = "Data/Media";
        public const string ThumbnailFolderName = "Thumbnail";
        public const string TempImage = "TempImage";
        public const string DefaultMediaClassName = "LocalAgent";
        public const string DefaultMediaServerName = "Local";
        public const string NetworkDrive = "Network Drive";
        public const string MediaServerListSessionKey = "MediaServerList";
        public const string MediaConfigurationIdSessionKey = "MediaConfigurationId";
        public const string CreateEdit = "CreateEdit";
        public const string CreateEditProduct = "~/Areas/PIM/Views/Products/CreateEdit.cshtml";
        public const string ManageView = "Manage";
        public const string ManagetStoreAction = "ManageStore";
        public const string PublishImmediatelyText = "Publish Immediately";
        public const string DoNotPublish = "N";
        public const string PublishImmediately = "A";
        public const string DoNotPublishText = "Do Not Publish. Require Moderator Approval";
        public const string ListView = "List";
        public const string AdminRoleName = "Admin";
        public const string UserRoleName = "user";
        public const string TaxRuleTypeListAction = "TaxRuleTypeList";
        public const string SupplierTypeListAction = "SupplierTypeList";
        public const string ShippingTypeListAction = "ShippingTypeList";
        public const string PromotionTypeListAction = "PromotionTypeList";
        public const string GetSMTPDetails = "GetSMTPDetails";
        public const string GetKlaviyoDetails = "GetKlaviyoDetails";
        public const string PortalProfileList = "PortalProfileList";
        public const string Edit = "Edit";
        public const string Create = "Create";
        public const string Delete = "Delete";
        public const string GetPortalUnitDetails = "GetPortalUnitDetails";
        public const string AttributeFamilyId = "AttributeFamilyId";
        public const string PIMAttributeGroupId = "PIMAttributeGroupId";
        public const string ProductId = "ProductId";
        public const string AddEditSKUPrice = "AddSKUPrice";
        public const string AddEditTierPrice = "AddTierPrice";
        public const string ProfileId = "ProfileId";
        public const string TabStructurePath = "~/Views/Shared/Controls/_TabStructure.cshtml";
        public const string LocaleTabPath = "~/Areas/PIM/Views/ProductAttribute/_Locale.cshtml";
        public const string DefaultGlobalSettingSessionKey = "DefaultGlobalSetting";
        public const string DefaultGlobalSettingCacheKey = "DefaultGlobalSettingCache";
        public const string ListName = "listName";
        public const string ShippingSKUView = "ShippingSKU";
        public const string TaxClassSKUView = "TaxClassSKU";
        public const string WarehouseName = "warehouseName";
        public const string TaxRuleView = "TaxRule";
        public const string ShippingRuleView = "ShippingRule";
        public const string Name = "name";
        public const string CreateEditPartialView = "_CreateEdit";
        public const string Mode = "mode";
        public const string Profile = "profile";
        public const string ActiveLocale = "ActiveLocale";
        public const string TemplatePath = "TemplatePath";
        public const string DisplayName = "DisplayName";
        public const string B2B = "B2B";
        public const string Reason = "Reason";
        public const string InventoryListId = "InventoryListId";
        public const string PriceListId = "PriceListId";
        public const string ProductSKUListView = "_ProductSKUList";
        public const string SKU = "SKU";
        public const string Draft = "Draft";
        public const string Ordered = "Ordered";
        public const string ResetSort = "ResetSort";
        public const string ResetSortAfterEdit = "ResetSortAfterEdit";
        public const string SimpleProduct = "SimpleProduct";
        public const string ProductPricePartialView = "~/Areas/PIM/Views/Products/_ProductPrice.cshtml";
        public const string ProductEditUrl = "/PIM/Products/Edit";
        public const string PriceTab = "Price";
        public const string ProductSEOTab = "seo";
        public const string ProductInventoryTab = "Inventory";
        public const string ThemeNameValidation = @"[^'\""]+$";
        public const string AuthorizeNet = "authorizenet";
        public const string Checkout2 = "twocheckout";
        public const string CyberSource = "cybersource";
        public const string Stripe = "stripe";
        public const string PayPalDirectPayment = "paypal";
        public const string PaymentechOrbital = "paymentech";
        public const string WorldPay = "worldpay";
        public const string Braintree = "braintree";
        public const string Payflow = "payflow";
        public const string DefaultCMSSEOPage = "~/Views/Content/_ContentPageForLocale.cshtml";
        public const string CardConnect = "cardconnect";
        public const string PayPalExpressCheckout = "paypalexpress";

        public const string ChangePasswordPopup = "changepasswordpopup";

        public const string AuthorizeNetView = "~/Views/Payment/_Authorize_net.cshtml";
        public const string PayflowView = "~/Views/Payment/_Payflow.cshtml";
        public const string PaymentechOrbitalView = "~/Views/Payment/_PaymentechOrbital.cshtml";
        public const string PayPalDirectPaymentView = "~/Views/Payment/_PayPalDirect.cshtml";
        public const string WorldPayView = "~/Views/Payment/_WorldPay.cshtml";
        public const string CyberSourceView = "~/Views/Payment/_CyberSource.cshtml";
        public const string CustomView = "~/Views/Payment/_Custom.cshtml";
        public const string CreditCardView = "~/Views/Payment/_CreditCartDetails.cshtml";
        public const string PurchaseOrderView = "~/Views/Payment/_PurchaseOrder.cshtml";
        public const string PayPalExpressView = "~/Views/Payment/_PayPalExpress.cshtml";
        public const string GoogleCheckoutView = "~/Views/Payment/_GoogleCheckout.cshtml";
        public const string CODView = "~/Views/Payment/_Cod.cshtml";
        public const string Checkout2View = "~/Views/Payment/_2Checkout.cshtml";
        public const string StripeView = "~/Views/Payment/_Stripe.cshtml";
        public const string BraintreeView = "~/Views/Payment/_Braintree.cshtml";
        public const string AmazonPayView = "~/Views/Payment/_AmazonPay.cshtml";
        public const string ACHView = "~/Views/Payment/_ACHView.cshtml";
        public const string ACHCardConnectView = "~/Views/Payment/_ACHCardConnectView.cshtml";
        public const string CustomGatewayView = "~/Views/Payment/_CustomGateway.cshtml";
        public const string CreateEditUrlView = "~/Views/Store/CreateEditUrl.cshtml";
        public const string CreateEditPortalProfileView = "~/Views/Store/CreatePortalProfile.cshtml";
        public const string CardConnectView = "~/Views/Payment/_CardConnect.cshtml";
        public const string CreateEditSMTPView = "~/Views/Store/CreateSMTP.cshtml";
        public const string CreateEditKlaviyoView = "~/Views/Store/_CreateKlaviyo.cshtml";
        public const string CreateEditProfileView = "~/Views/Profiles/CreateEditProfile.cshtml";
        public const string AddCustomFieldView = "~/Areas/PIM/Views/Products/AddCustomField.cshtml";
        public const string CustomFieldListView = "~/Areas/PIM/Views/Products/CustomFieldList.cshtml";
        public const string PreviewImportPriceView = "~/Views/Price/_PreviewImportPriceList.cshtml";
        public const string TemplateNamePath = "~/Views/Shared/PDP Template";
        public const string IsExists = "IsExists";
        public const string Error = "Error";
        public const string Success = "Success";
        public const string CreateEditEmailTemplateView = "~/Views/EmailTemplate/CreateEditEmailTemplate.cshtml";
        public const string CreateEditHighlightView = "~/Views/Highlight/CreateEditHighlight.cshtml";
        public const string ReportCatalogView = "~/Views/MyReports/_CatalogList.cshtml";
        public const string ReportPriceView = "~/Views/MyReports/_PriceList.cshtml";
        public const string ReportWarehouseView = "~/Views/MyReports/_WarehouseList.cshtml";
        public const string TwilioView = "~/Views/SMS/_TwilioForm.cshtml";

        public const int CreditCard = 1;
        public const int PurchaseOrder = 2;
        public const int PayPalExpress = 3;
        public const int COD = 4;
        public const int AmazonPay = 5;

        public const string EmailRegularExpression = "^([\\w+-.%]+@[\\w-.]+\\.[A-Za-z]{2,4},?)+$";
        public const string WholeNoRegularExpression = "^([0-9][0-9]*)$";
        public const string FileNameRegularExpression = "([A-Za-z0-9-_]+)";
        public const string DecimalCoOrdinatesRegularExpression = @"^\-?[0-9]{1,3}(\.[0-9]{1,15})?$";

        public const string Admin = "Admin";
        public const string EmailValidation = "^[0-9]*$";
        public const string URLValidation = "(\b(https?|ftp|file)://)?[-A-Za-z0-9+&@#/%?=~_|!:,.;]+[-A-Za-z0-9+&@#/%=~_|]";
        public const string NumberValidation = "^[\\d]+";
        public const string SmtpPortNumberValidation = "\\d{0,5}";
        public const string DecimalNumberValidation = "^\\d{0,}(\\.\\d{1,6})?$";
        public const string DecimalPositiveNegativeNumberValidation = "^-?[0-9]\\d*(\\.\\d+)?$";
        public const string PhoneNumberValidation = "(\\([2-9]\\d\\d\\)|[2-9]\\d\\d) ?[-.,]? ?[2-9]\\d\\d?[-.,]? ?\\d{4}";
        public const string AlphaNumericOnlyValidation = "^[A-Za-z0-9 ]+$";
        public const string AlphanumericStartWithAlphabetValidation = @"^[A-Za-z][a-zA-Z0-9]*$";
        public const string MultipleEmailRegEx = "^((\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*)\\s*[,]{0,1}\\s*)+$";
        public const string True = "true";
        public const string False = "false";
        public const string UserMenuListSessionKey = "UserMenuListSessionKey";
        public const string HyperlinkValidation = "^http(s)?\\:\\/\\/[a-zA-Z0-9_\\-/:%#]+(?:\\.[a-zA-Z0-9_\\-/%#]+)*$";
        public const string SEOUrlValidation = "^[A-Za-z0-9-_/.]+$";
        public const string TagValidation = "^[A-Za-z0-9, ]+$";
        public const string CssNameValidation = "^[a-zA-Z0-9\\s.\\?\\,\\;\\:\\!\\(\\)\\-]+$";
        public const string DownloadableProductURLValidation = "(https?:\\/\\/(?:www\\.|(?!www))[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\\.[^\\s]{2,}|www\\.[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\\.[^\\s]{2,}|https?:\\/\\/(?:www\\.|(?!www))[a-zA-Z0-9]\\.[^\\s]{2,}|www\\.[a-zA-Z0-9]\\.[^\\s]{2,})";
        public const string RuleNameValidation = "^[a-zA-Z][a-zA-Z0-9_ ,'\\-]*$";

        public const string MultiSelect = "MultiSelect";
        public const string Extensions = "Extensions";
        public const string Select = "Select";
        public const string IsRequired = "IsRequired";
        public const string ImageExtensions = ".JPG,.PNG,.GIF,.JPEG,.SVG,.svg,.webp";
        public const string VideoExtensions = ".MP4,.OGV,.WEBM";
        public const string FileTypes = "jpg,jpeg,png,bmp,gif,pdf,doc,docx,ppt,xls,zip,ttf,xlsx";
        public const int ImageMaxFileSize = 10485760;
        public const string FaviconExtension = ".ico,.webp";
        public const string ImageExtensionForWebsiteLogo = ".JPG,.PNG,.GIF,.JPEG,.SVG,.webp";
        public const string IsAllowMultiUpload = "IsAllowMultiUpload";

        //TO DO.
        public const string XLSX = ".xlsx";

        public const string XLS = ".xls";
        public const string CSV = ".csv";
        
        //Constant For Attribute Validation
        public const string RegularExpression = "RegularExpression";

        public const string MinDate = "MinDate";
        public const string MaxDate = "MaxDate";
        public const string Group = "Group";
        public const string NumberExpression = "^[0-9]*$";
        public const string Date = "Date";
        public const string Number = "Number";
        public const string DateGroup = "DateGroup";
        public const string ImportErrorList = "ImportErrorList";
        public const string DownloadImportErrorList = "DownloadImportErrorList";

        public const int Zero = 0;
        public const int NegativeOne = -1;

        public const string MaxNumber = "MaxNumber";
        public const string MinNumber = "MinNumber";
        public const string AllowNegative = "AllowNegative";
        public const string AllowDecimals = "AllowDecimals";
        public const string No = "NO";

        public const string New = "N";
        public const string Active = "A";
        public const string Inactive = "I";

        public const string ZipFileType = "zip";
        public const string CSSFileType = "css";
        public const string CSHTMLFileType = "cshtml";
        public const string CSVFileType = "csv";

        public const string FormBuilderMediaPath = "/Data/FormBuilderMedia";
        public const string TemplatesPath = "~/Themes/Templates";
        public const string TemporaryThemeFolderPath = "~/Data/Themes";
        public const string TemporaryFolderPath = "~/Data";
        public const string ImportFolderPath = "~/Data/ImportFiles";
        public const string ThemeFolderPath = "~/Themes";
        public const string RevisionFolderPath = "~/Themes/Revision";
        public const string WidgetList = "~/Views/Shared/Controls/_WidgetList.cshtml";
        public const string WidgetPageList = "~/Views/Shared/Controls/_WidgetPageList.cshtml";
        public const string Product = "Product";
        public const string Category = "Category";
        public const string Pricing = "Pricing";
        public const string Inventory = "Inventory";
        public const string Voucher = "Voucher";
        public const string Account = "Account";
        public const string ContentPage = "Content Page";
        public const string SelectAccount = "Select Account";
        public const string ShippingAddressKey = "ShippingAddress";
        public const string BillingAddressKey = "BillingAddress";
        public const string BillingAddress1Key = "BillingAddress.Address1";
        public const string ShippingAddress1Key = "ShippingAddress.Address1";
        public const string OrderCustomerDetailsView = "_CustomerDetails";
        public const string CreateEditOrderCustomerView = "_CreateEditCustomer";
        public const string CartCookieKey = "CookieMappingID";
        public const string CartModelSessionKey = "ShoppingCartModel";
        public const string ReportPartialView = "~/Views/Shared/Controls/_ReportPartial.cshtml";
        public const string AddNewAddress = "Add New Address";
        public const string CustomerListView = "_CustomerList";
        public const string CreateEditCustomerAddressView = "_CustomerAddress";
        public const string CreateOrderView = "CreateOrder";
        public const string CustomCreateOrderView = "CreateOrder";
        public const string CreateCustomerView = "CreateCustomer";
        public const string OMSUserAccountSessionKey = "OMSUserAccount";

        public const string DisablePurchasing = "DisablePurchasing";
        public const string AllowBackOrdering = "AllowBackOrdering";
        public const string DontTrackInventory = "DontTrackInventory";
        public const string CheckoutReviewView = "CheckoutReview";
        public const string ShoppingCartView = "ShoppingCart";
        public const string PendingApproval = "Pending Approval";
        public const string PendingPayment = "Pending Payment";
        public const string ZnodeShippingFedEx = "ZnodeShippingFedEx";
        public const string ZnodeShippingUps = "ZnodeShippingUps";
        public const string ZnodeShippingUSPS = "ZnodeShippingUsps";
        public const string ClassName = "ClassName";
        public const string CreateEditOrderView = "CreateOrder";
        public const string CheckoutReceipt = "CheckoutReceipt";
        public const string Brand = "Brand";
        public const string ShippingCost = "ShippingCostRules";
        public const string Vendor = "Vendor";
        public const int DefaultDisplayOrder = 999;
        public const string TotalTableView = "_TotalTable";
        public const string IsCallForAttribute = "IsCallForAttribute";
        public const string Assortment = "Assortment";

        public const string QuoteHistory = "QuoteHistory";
        public const string AccountQuotes = "AccountQuotes";
        public const string Quotes = "Quotes";
        public const string Highlights = "Highlights";
        public const string ZWidgetFileKey = "ZWidget";
        public const string ZWidgetDisplayName = "ZWidgetDisplayName";
        public const string LinkType = "Link";
        public const string DateType = "Date";
        public const string EditUrlRedirect = "EditUrlRedirect";
        public const string SEODetails = "SEODetails";
        public const string CreateUrlRedirect = "CreateUrlRedirect";
        public const string IsStoreAdmin = "isstoreadmin";
        public const string SaveSEOSetting = "SaveSEOSetting";
        public const string OrderHistory = "OrderHistory";
        public const string FileValidation = @"[^0-9a-zA-Z\._]";
        public const string CollapseMenuStatus = "CollapseMenuStatus";
        public const string ColumnDateTime = "datetime";
        public const string CustomerOrderHistory = "CustomerOrderHistory";
        public const string AccountOrderHistory = "AccountOrderHistory";
        public const string ColumnBoolean = "boolean";
        public const string Percentage = "Percentage";
        public const string OMSOrderSessionKey = "OrderModel_";
        public const string ShippingView = "ShippingView";
        public const string CSRDiscountAmountView = "CSRDiscountAmountView";
        public const string TaxView = "TaxView";
        public const string AttributeCode = "AttributeCode";
        public const string PaymentSettled = "Settled";
        public const string Completed = "Completed";


        public const string OrderStatus = "OrderStatus";
        public const string PaymentStatus = "PaymentStatus";
        public const string SHIPPED = "SHIPPED";
        public const string SUBMITTED = "SUBMITTED";

        public const string Order = "Order";

        public const string OfferBanner = "OfferBanner";
        public const string JobIndicator = "JobIndicator";
        public const string ProductImage = "ProductImage";
        public const string PublishStatus = "PublishStatus";

        public const string CreateEditSearchSynonyms = "CreateEditSearchSynonyms";
        public const string CreateEditSearchKeywordsRedirect = "CreateEditSearchKeywordsRedirect";
        public const string LineItemHistorySession = "LineItemHistorySession";

        public const string POEmailSubject = "Order Invoice";

        public const string GlobalAttributeGroupId = "GlobalAttributeGroupId";
        public const string EntityId = "EntityId";
        public const string GlobalAttributeEntityView = "~/Views/Shared/Controls/GlobalAttribute/GlobalAttributeEntity.cshtml";
        public const string GlobalAttributeEntityAsidePanelPartialView = "~/Views/Shared/Controls/GlobalAttribute/_AsidePanel.cshtml";
        public const string DialogUpdateProductPartialView = "~/Views/Shared/Controls/GlobalAttribute/_AsidePanel.cshtml";
        public const string UpdateProductPartialView = "_UpdateProductPartialView";
        public const string UpdateProductSampleCSVString = "SKU,ProductName,IsActive";
        public const string ProductUpdate = "ProductUpdate";
        public const string ContentTypeCSV = "text/csv";

        public const string Media = "Media";
        public const string ImportErrorLogDetails = "ImportErrorLogDetails";
        public const string productInventory = "~/Areas/PIM/Views/Products/_ProductInventory.cshtml";
        public const string UnAssociatedCategoriesView = "~/Areas/PIM/Views/Products/_UnAssociatedCategories.cshtml";
        public const string associatedCategoriesToProducts = "~/Areas/PIM/Views/Products/_AssociatedProductCategories.cshtml";
        public const string productSEO = "~/Areas/PIM/Views/Products/ProductSEO.cshtml";
        public const string categorySEO = "~/Areas/PIM/Views/Category/CategorySEO.cshtml";
        public const string AssociatedCategoriesView = "~/Areas/PIM/Views/Products/_AssociatedProductCategories.cshtml";
        public const string PreviewContentPage = "_PreviewContentPage";

        public const string AddOn = "AddOns";
        public const string Personalization = "Personalization";
        public const string CustomFields = "CustomFields";
        public const string Downloadable = "Downloadable";
        public const string YouMayAlsoLike = "YouMayAlsoLike";
        public const string FrequentlyBought = "FrequentlyBought";
        public const string AssociatedProducts = "AssociatedProducts";
        public const string AddProductPriceBySku = "AddProductPriceBySku";

        public const string OperationToPerform = "OperationToPerform";
        public const string Rejected = "REJECTED";
        public const string CreateCustomerOrder = "~/Views/Order/CreateOrder.cshtml";

        public const string DateTimeRange = "DateTimeRange";
        public const string IsCatalogFilter = "IsCatalogFilter";

        public const string Stores = "Store";
        public const string Products = "Product";
        public const string Catalogs = "Catalog";
        public const string Accounts = "Accounts";

        public const string URLRedirectValidation = "^[a-zA-z0-9].*";
        public const string SalesRepRole = "Sales Rep";

        public const string AddressList = "AddressList";
        public const string UserCartView = "~/Views/Customer/_UserCart.cshtml";

        public const string OmsQuoteTypeId = "3"; //QuoteTypeCode = Quote (For Quote List)
        public const string Index = "Index";
        public const string Size = "size";
        public const string DefaultIndexValue = "1";
        public const string DefaultSizeValue = "50";
        public const string OMSQuoteSessionKey = "QuoteResponseModel_";
        public const string RMAReturnSessionKey = "RMAReturnViewModel_";

        public const string PowerBIApplicationId = "PowerBIApplicationId";
        public const string PowerBITenantId = "PowerBITenantId";
        public const string PowerBIReportId = "PowerBIReportId";
        public const string PowerBIGroupId = "PowerBIGroupId";
        public const string PowerBIUserName = "PowerBIUserName";
        public const string PowerBIPassword = "PowerBIPassword";

        public const string CreateQuoteView = "~/Views/Quote/CreateQuoteRequest.cshtml";
        public const string QuoteCartSessionKey = "Quote_";
        public const string ContainerVariantSessionKey = "ContainerVariantViewModel_";
        public const string PipeSeperator = " | ";
        public const string OrderStatesList = "OrderStatesList";
        public const string IsRedirectToEditScreen = "IsRedirectToEditScreen";
        public const string WidgetVariantSessionKey = "WidgetVariantViewModel_";
        public const string IsSalesRepList = "IsSalesRepList";

        public const string LabelZnodeKnowledgeBaseWiki = "http://knowledgebase.znode.com/";  
        public const string CreateSchedulerView = "~/Views/TouchPointConfiguration/Create.cshtml";
        public const string UpdateScheduler = "Update Scheduler";
        public const string CreateScheduler = "Create Scheduler";
        public const string CustomerOrderView = "CustomerOrderView";
        public const string AccountOrderView = "AccountOrderView";
        public const string FilterTupleForOfflinePayment = "IsUsedForOfflinePayment";
        public const string CreateEditKlaviyoProvider = "~/Views/Store/CreateEditKlaviyo.cshtml";
        public const string CreateKlaviyo = "~/Views/Store/_CreateKlaviyo.cshtml";
    }
}