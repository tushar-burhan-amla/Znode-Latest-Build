class Endpoint extends ZnodeBase {
    private _global: string;
    constructor() {
        super();
    }
    gettreeviewdataset(http) {
        return http.get("/MediaManager/MediaManager/GetTreeData").success(function (data) {
            data;
        })
    }
    getdropdowndataset(http) {
        return http.get("/MediaManager/MediaManager/GetDropDownList").success(function (data) {
            data;
        });
    }

    SaveRights(data, roleId) {
        super.ajaxRequest("/RoleAndAccessRight/SavePermissions?data=" + data.toString() + "&roleId=" + roleId + "", "Post", {}, function (res) {
            window.location.href = "/RoleAndAccessRight/RoleList";
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, res.IsSuccess ? "success" : "error", isFadeOut, fadeOutTime);
        }, "HTML");
    }

    ManageRolePermission(collection, _roleId) {
        super.ajaxRequest("/RoleAndAccessRight/ManagePermission", "POST", { data: JSON.stringify(collection), roleId: _roleId }, function (data) {
            if (data) {
                window.location.href = "/RoleAndAccessRight/RoleList";
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(data.statusMessage, data.success ? "success" : "error", isFadeOut, fadeOutTime);
            }
        }, "json");
    }

    GetXml(id, callbackMethod) {
        super.ajaxRequest("/XMLGenerator/View", "get", { "id": id }, callbackMethod, "html");
    }
    GetColumnsList(entityType, entityName, columnListJson, callbackMethod) {
        super.ajaxRequest("/XMLGenerator/GetColumnList", "get", { "entityType": entityType, "entityName": entityName, "columnListJson": columnListJson }, callbackMethod, "html");
    }

    GetEntityName(term, entityType, callbackMethod) {
        super.ajaxRequest("/XMLGenerator/AutoCompleteEntityName", "POST", { "term": term, "entityType": entityType }, callbackMethod, "json");
    }
    SaveXmlData(url, _griddata, txtviewOptions, entityType, entityName, txtfrontPageName, txtfrontObjectName, id, callbackMethod) {
        super.ajaxRequest(url, "POST", { "columnCollection": _griddata, "viewOptions": txtviewOptions, "entityType": entityType, "entityName": entityName, "frontPageName": txtfrontPageName, "frontObjectName": txtfrontObjectName, "id": id }, callbackMethod, "json");
    }
    deleteRecordById(url, callbackMethod) {
        super.ajaxRequest(url, "post", {}, callbackMethod, "json");
    }
    GetAttributeInputValidationView(attributeTypeId: number, callbackMethod: any) {
        super.ajaxRequest("/MediaManager/Attributes/BindInputValidationList", "get", { "attributeTypeId": attributeTypeId }, callbackMethod, "html");
    }
    ValidationView(url: string, attributeTypeId: number, callbackMethod: any) {
        super.ajaxRequest(url, "get", { "AttributeTypeId": attributeTypeId }, callbackMethod, "html");
    }

    SaveDefaultValues(url: string, data: any, attributeId: number, defaultvaluecode: string, defaultvalueId: number, displayOrder: number, isDefault: boolean, isSwatch: boolean, swatch: string, callbackMethod: any) {
        super.ajaxRequest(url, "get", {
            "model": JSON.stringify(data), "attributeId": attributeId, "defaultvalueId": defaultvalueId, "defaultvaluecode": defaultvaluecode, "displayOrder": displayOrder, "isdefault": isDefault, "isswatch": isSwatch, "swatchtext": swatch
        }, callbackMethod, "json");
    }

    DeleteDefaultValues(url: string, defaultvalueId: number, callbackMethod: any) {
        super.ajaxRequest(url, "get", { "defaultvalueId": defaultvalueId }, callbackMethod, "json");
    }
    getView(url, callbackMethod) {
        super.ajaxRequest(url, "get", {}, callbackMethod, "html");
    }

    GetRecord(model: Znode.Core.MultiSelectDDlModel, SuccessCallBack: any) {
        super.ajaxRequest("/" + model.Controller + "/" + model.Action, "get", { id: model.ItemIds.toString(), flag: model.flag }, SuccessCallBack, "html");
    }

    GetMediaSetting(partialViewName: string, serverId: number, SuccessCallBack: any) {
        super.ajaxRequest("/MediaManager/MediaConfiguration/GetMediaSetting", "get", { partialViewName: partialViewName, serverId: serverId }, SuccessCallBack, "html");
    }

    GenerateImageOnEdit(mediaPath: string, fileName: string, callbackMethod: any) {
        super.ajaxRequest("/MediaManager/MediaManager/GenerateImageOnEdit", "get", { "mediaPath": mediaPath, "fileName": fileName }, callbackMethod, "json");
    }

    GenerateImages(callbackMethod: any) {
        super.ajaxRequest("/MediaManager/MediaConfiguration/GenerateImages", "get", {}, callbackMethod, "json");
    }

    TestAjaxCall(url: string, callbackMethod: any) {
        super.ajaxRequest(url, "get", {}, callbackMethod, "html");
    }

    GetAssociatedAttributes(url: string, attributeId: number, familyId: number, SuccessCallBack: any) {
        super.ajaxRequest(url, "GET", { "attributeGroupId": attributeId, "attributeFamilyId": familyId }, SuccessCallBack, "html");
    }
    SaveAttributeDefaultValue(attributeId, localeArray, defaultAttributeId, callbackMethod) {
        super.ajaxRequest("/MediaManager/Attributes/SaveDefaultAttributeValue", "post", { "attributeId": attributeId, "defaultAttributeId": defaultAttributeId, "localeValue": localeArray }, callbackMethod, "html");
    }
    DeleteAttributeDefaultValue(defaultAttributeId, attributeId, callbackMethod) {
        super.ajaxRequest("/MediaManager/Attributes/DeleteDefaultAttributeValue", "get", { "defaultAttributevalueId": defaultAttributeId, "attributeId": attributeId }, callbackMethod, "html");
    }
    MediaAddFolder(parentId: string, folderName: string, callbackMethod) {
        super.ajaxRequest("/MediaManager/MediaManager/AddFolder", "get", { "parentId": parentId, "folderName": folderName }, callbackMethod, "json");
    }
    MediaRenameFolder(folderId: string, folderName: string, callbackMethod) {
        super.ajaxRequest("/MediaManager/MediaManager/RenameFolder", "get", { "folderId": folderId, "folderName": folderName }, callbackMethod, "json");
    }

    MediaMoveFolder(addtoFolderId: string, folderId: string, callbackMethod) {
        super.ajaxRequest("/MediaManager/MediaManager/MoveFolder", "get", { "addtoFolderId": addtoFolderId, "folderId": folderId }, callbackMethod, "json");
    }
    MoveMedia(folderId: string, mediaIds: string, callbackMethod) {
        super.ajaxRequest("/MediaManager/MediaManager/MoveMediaToFolder", "get", { "folderId": folderId, "mediaIds": mediaIds }, callbackMethod, "json");
    }
    MediaUserList(folderId: string, callbackMethod) {
        super.ajaxRequest("/MediaManager/MediaManager/GetUserAccountList", "get", { "folderId": folderId }, callbackMethod, "html");
    }

    MediaShareFolder(folderId: string, accountIds: string, callbackMethod) {
        super.ajaxRequest("/MediaManager/MediaManager/ShareFolder", "post", { "folderId": folderId, "accountIds": accountIds }, callbackMethod, "json");
    }

    GetPromotionTypeDetails(className: string, callbackMethod) {
        super.ajaxRequest("/ProviderEngine/GetPromotionTypeByClassName", "get", { "className": className }, callbackMethod, "json");
    }

    GetSupplierTypeDetails(className: string, callbackMethod) {
        super.ajaxRequest("/ProviderEngine/GetSupplierTypeByClassName", "get", { "className": className }, callbackMethod, "json");
    }

    GetShippingTypeDetails(className: string, callbackMethod) {
        super.ajaxRequest("/ProviderEngine/GetShippingTypeByClassName", "get", { "className": className }, callbackMethod, "json");
    }

    GetTaxRuleTypeDetails(className: string, callbackMethod) {
        super.ajaxRequest("/ProviderEngine/GetTaxRuleTypeByClassName", "get", { "className": className }, callbackMethod, "json");
    }

    GetPaymentTypeForm(paymentName: string, paymentSettingModel: Object, paymentCode: string, callbackMethod) {
        super.ajaxRequest("/Payment/GetPaymentTypeForm", "post", { "paymentName": paymentName, "paymentSetting": JSON.stringify(paymentSettingModel), "paymentCode": paymentCode }, callbackMethod, "html");
    }

    GetPaymentGetwayForm(gatewayCode: string, paymentSettingModel: Object, callbackMethod) {
        super.ajaxRequest("/Payment/GetPaymentGetwayForm", "post", { "gatewayCode": gatewayCode, "paymentSetting": JSON.stringify(paymentSettingModel) }, callbackMethod, "html");
    }

    GetPaymentSettingCredentials(paymentCode: string, isTestMode: boolean, gatewayCode: string, paymentTypeCode: string, callbackMethod) {
        super.ajaxRequest("/Payment/GetPaymentSettingCredentials", "get", { "paymentCode": paymentCode, "isTestMode": isTestMode, "gatewayCode": gatewayCode, "paymentTypeCode": paymentTypeCode }, callbackMethod, "html");
    }

    MediaDelete(mediaIds: string, callbackMethod) {
        super.ajaxRequest("/MediaManager/MediaManager/DeleteMedia", "get", { "mediaId": mediaIds }, callbackMethod, "json");
    }

    FolderDelete(folderId: string, callbackMethod) {
        super.ajaxRequest("/MediaManager/MediaManager/DeleteFolder", "get", { "folderId": folderId }, callbackMethod, "json");
    }

    DeleteTaxRuleTypes(id: string, callbackMethod) {
        super.ajaxRequest("/ProviderEngine/DeleteTaxRuleType", "get", { "id": id }, callbackMethod, "json");
    }

    DeleteShippingTypes(id: string, callbackMethod) {
        super.ajaxRequest("/ProviderEngine/DeleteShippingType", "get", { "id": id }, callbackMethod, "json");
    }

    DeleteSupplierTypes(id: string, callbackMethod) {
        super.ajaxRequest("/ProviderEngine/DeleteSupplierType", "get", { "id": id }, callbackMethod, "json");
    }

    DeletePromotionTypes(id: string, callbackMethod) {
        super.ajaxRequest("/ProviderEngine/DeletePromotionType", "get", { "id": id }, callbackMethod, "json");
    }

    EnableTaxRuleTypes(id: string, isEnable: boolean, callbackMethod) {
        super.ajaxRequest("/ProviderEngine/BulkEnableDisableTaxRuleTypes", "get", { "id": id, "isEnable": isEnable }, callbackMethod, "html");
    }

    EnableSupplierTypes(id: string, isEnable: boolean, callbackMethod) {
        super.ajaxRequest("/ProviderEngine/BulkEnableDisableSupplierTypes", "get", { "id": id, "isEnable": isEnable }, callbackMethod, "html");
    }

    EnableShippingTypes(id: string, isEnable: boolean, callbackMethod) {
        super.ajaxRequest("/ProviderEngine/BulkEnableDisableShippingTypes", "get", { "id": id, "isEnable": isEnable }, callbackMethod, "html");
    }

    EnablePromotionTypes(id: string, isEnable: boolean, callbackMethod) {
        super.ajaxRequest("/ProviderEngine/BulkEnableDisablePromotionTypes", "get", { "id": id, "isEnable": isEnable }, callbackMethod, "html");
    }

    DeleteTaxClasses(taxClassId: string, callbackMethod) {
        super.ajaxRequest("/TaxClass/Delete", "get", { "taxClassId": taxClassId }, callbackMethod, "html");
    }

    SetGlobalConfigSetting(Url: string, model, callbackMethod) {
        super.ajaxRequest(Url, "post", { model: model }, callbackMethod, "json");
    }

    SetSearchProfileSetting(Url: string, portalId: number, id: number, publishCatalogId: number, callbackMethod) {
        super.ajaxRequest(Url, "post", { "portalId": portalId, "searchProfileId": id, "publishCatalogId": publishCatalogId }, callbackMethod, "json");
    }

    SetGlobalConfigSettingListType(Url: string, listType: any, callbackMethod) {
        super.ajaxRequest(Url, "get", { listType: listType }, callbackMethod, "html");
    }

    DeleteUsers(id: string, callbackMethod) {
        super.ajaxRequest("/User/DeleteUser", "get", { "userId": id }, callbackMethod, "json");
    }

    DeleteCustomer(id: string, callbackMethod) {
        super.ajaxRequest("/User/CustomerDelete", "get", { "userId": id }, callbackMethod, "json");
    }

    EnableDisableUserAccount(id: string, isEnable: boolean, callbackMethod) {
        super.ajaxRequest("/User/EnableDisableAccount", "get", { "userId": id, "isLock": isEnable }, callbackMethod, "html");
    }

    EnableDisableSingleUserAccount(id: string, isEnable: boolean, isAdminUser: boolean, callbackMethod) {
        super.ajaxRequest("/User/EnableDisableSingleAccount", "get", { "userId": id, "isLock": isEnable, "isAdminUser": isAdminUser }, callbackMethod, "json");
    }

    EnableDisableCustomerAccount(id: string, isEnable: boolean, callbackMethod) {
        super.ajaxRequest("/User/CustomerEnableDisableAccount", "get", { "userId": id, "isLock": isEnable }, callbackMethod, "html");
    }

    DeleteStore(portalId: string, callbackMethod) {
        super.ajaxRequest("/Store/DeleteStore", "get", { "portalId": portalId }, callbackMethod, "json");
    }

    DeleteStoreByStoreCode(storeCode: string, callbackMethod) {
        super.ajaxRequest("/Store/DeleteStoreByStoreCode", "get", { "storeCode": storeCode }, callbackMethod, "json");
    }

    UserResetPassword(id: string, callbackMethod) {
        super.ajaxRequest("/User/BulkResetPassword", "get", { "userId": id }, callbackMethod, "html");
    }

    CustomerResetPassword(id: string, callbackMethod) {
        super.ajaxRequest("/User/BulkResetPassword", "get", { "userId": id }, callbackMethod, "html");
    }
    getServiceListByShippingTypeId(serviceShippingTypeId, callbackMethod) {
        super.ajaxRequest("/Shippings/BindServiceList", "get", { "ServiceShippingTypeId": serviceShippingTypeId }, callbackMethod, "json");
    }

    DeleteUrl(urlIds: string, callbackMethod) {
        super.ajaxRequest("/Store/DeleteUrl", "get", { "domainId": urlIds }, callbackMethod, "json");
    }

    DeleteAdminAPIUrl(urlIds: string, callbackMethod) {
        super.ajaxRequest("/UrlManagement/DeleteUrl", "get", { "domainId": urlIds }, callbackMethod, "json");
    }

    DeletePortalProfile(portalProfileIds: string, callbackMethod) {
        super.ajaxRequest("/Store/DeletePortalProfile", "get", { "portalProfileId": portalProfileIds }, callbackMethod, "json");
    }

    GetMedia(model: any, callbackMethod: any) {
        super.ajaxRequest("/MediaManager/MediaManager/List", "post", { "popupViewModel": model }, callbackMethod, "html");
    }

    GetCurrencyInfo(currencyId: number, oldCurrencyId: number, cultureId: number, callbackMethod) {
        super.ajaxRequest("/Store/GetCurrencyInformation", "get", {
            "currencyId": currencyId, "oldCurrencyId": oldCurrencyId, "cultureId": cultureId
        }, callbackMethod, "json");
    }

    GetCultureInfo(currencyId: number, cultureId: number, callbackMethod) {
        super.ajaxRequest("/Store/GetCultureCodeList", "get", { "currencyId": currencyId, "cultureId": cultureId }, callbackMethod, "json");
    }

    GetCSSListForStore(cmsThemeId: number, callbackMethod) {
        super.ajaxRequest("/Store/GetCSSList", "get", { "cmsThemeId": cmsThemeId }, callbackMethod, "json");
    }

    GetParentAccountList(portalId: number, callbackMethod) {
        super.ajaxRequest("/Account/GetParentAccountList", "get", { "portalId": portalId }, callbackMethod, "json");
    }

    GetCountriesByPortalId(portalId: number, callbackMethod) {
        super.ajaxRequest("/Account/GetCountryBasedOnPortalId", "get", { "portalId": portalId }, callbackMethod, "json");
    }

    GetCountriesByPortalIdWithountAccountAccess(portalId: number, callbackMethod) {
        super.ajaxRequest("/Customer/GetCountryBasedOnPortalId", "get", { "portalId": portalId }, callbackMethod, "json");
    }

    GetAccountDepartmentList(accountId: any, callbackMethod) {
        super.ajaxRequest("/User/GetAccountDepartments", "get", { "accountId": accountId }, callbackMethod, "json");
    }

    GetPermissionList(accountId, accountPermissionAccessId, callbackMethod) {
        super.ajaxRequest("/User/GetPermissionList", "get", { accountId: accountId, accountPermissionId: accountPermissionAccessId }, callbackMethod, "json", false);
    }

    GetPermissionListWithoutAccountId(accountPermissionAccessId, callbackMethod) {
        super.ajaxRequest("/User/GetPermissionList", "get", { accountPermissionId: accountPermissionAccessId }, callbackMethod, "json", false);
    }

    GetApproverList(accountId: number, userId: number, callbackMethod) {
        super.ajaxRequest("/User/GetApproverList", "get", { "accountId": accountId, "userId": userId }, callbackMethod, "json");
    }

    GetRoleList(callbackMethod) {
        super.ajaxRequest("/User/GetRoleList", "get", {}, callbackMethod, "json");
    }

    SalesDetailsBasedOnSelectedPortal(portalId: number, callbackMethod) {
        super.ajaxRequest("/Dashboard/GetDashboardSalesDetails", "get", { "portalId": portalId }, callbackMethod, "json");
    }

    SalesDetailsBasedOnSelectedPortalAndAccount(portalId: number, accountId: number, callbackMethod) {
        super.ajaxRequest("/Dashboard/GetDashboardDetails", "get", { "portalId": portalId, "accountId": accountId }, callbackMethod, "json");
    }

    DeletePIMFamily(id: string, contollerName: string, callbackMethod) {
        super.ajaxRequest("/PIM/" + contollerName + "/Delete", "get", { "pimattributeFamilyId": id }, callbackMethod, "json");
    }

    DeletePIMAttributeGroup(id: string, contollerName: string, callbackMethod) {
        super.ajaxRequest("/PIM/" + contollerName + "/Delete", "get", { "pimAttributeGroupId": id }, callbackMethod, "json");
    }

    DeleteMediaFamily(id: string, callbackMethod) {
        super.ajaxRequest("/MediaManager/AttributeFamily/Delete", "get", { "mediaAttributeFamilyId": id }, callbackMethod, "json");
    }


    DeletePIMAttribute(id: string, callbackMethod) {
        super.ajaxRequest("/PIM/ProductAttribute/Delete", "get", { "pimAttributeId": id }, callbackMethod, "json");
    }

    DeleteMediaAttribute(id: string, callbackMethod) {
        super.ajaxRequest("/MediaManager/Attributes/Delete", "get", { "mediaAttributeId": id }, callbackMethod, "json");
    }

    AssociateAddons(model: Object, callbackMethod) {
        super.ajaxRequest("/PIM/Products/AssociateAddonGroup", "post", { "model": model }, callbackMethod, "html");
    }

    AssignLinkProducts(model: Object, callbackMethod) {
        super.ajaxRequest("/PIM/Products/AssignLinkProduct", "post", { "model": model }, callbackMethod, "json");
    }

    GetUnassociatedProducts(parentProductId: number, listType: any, associatedProductIds: string, addonGroupId: any, callbackMethod) {
        super.ajaxRequest("/PIM/Products/GetUnassociatedProducts", "get", { "parentProductId": parentProductId, "listType": listType, "addonProductId": addonGroupId, "associatedProductIds": associatedProductIds }, callbackMethod, "html");
    }

    AssociateProducts(model: Object, callbackMethod) {
        super.ajaxRequest("/PIM/Products/AssociateProducts", "post", { "model": model }, callbackMethod, "json");
    }

    DeleteMediaAttributeGroup(id: string, callbackMethod) {
        super.ajaxRequest("/MediaManager/AttributeGroup/Delete", "get", { "mediaAttributeGroupId": id }, callbackMethod, "json");
    }

    GetAssociatedProducts(productId: number, attributeId: number, callbackMethod) {
        super.ajaxRequest("/PIM/Products/GetAssociatedProducts", "get", { "parentProductId": productId, "attributeId": attributeId }, callbackMethod, "html");
    }

    GetAssociatedBundleProducts(productId: number, attributeId: number, callbackMethod) {
        super.ajaxRequest("/PIM/Products/GetAssociatedBundleProducts", "get", { "parentProductId": productId, "attributeId": attributeId }, callbackMethod, "html");
    }
    GetAssociatedConfigureProducts(productId: number, attributeId: string, callbackMethod) {
        super.ajaxRequest("/PIM/Products/GetAssociatedConfigureProducts", "get", { "parentProductId": productId, "associatedAttributeIds": attributeId }, callbackMethod, "html");
    }

    GetAttributeFamilyDetails(familyId: string, productId: string, callbackMethod) {
        super.ajaxRequest("/PIM/Products/GetAttributes", "get", { "familyId": familyId, "productId": productId }, callbackMethod, "html");
    }

    GetCustomField(productId: number, callbackMethod) {
        super.ajaxRequest("/PIM/Products/AddCustomField", "get", { "productId": productId }, callbackMethod, "html");
    }

    EditCustomField(productId: string, customFieldId: string, callbackMethod) {
        super.ajaxRequest("/PIM/Products/EditCustomField", "get", { "productId": productId, "customFieldId": customFieldId }, callbackMethod, "html");
    }

    SaveDefaultValuesMedia(data: any, attributeId: number, defaultvaluecode: string, defaultvalueId: number, callbackMethod: any) {
        super.ajaxRequest("/MediaManager/Attributes/SaveDefaultValues", "get", { "model": JSON.stringify(data), "attributeId": attributeId, "defaultvaluecode": defaultvaluecode, "defaultvalueId": defaultvalueId }, callbackMethod, "json");
    }

    DeleteDefaultValuesMedia(defaultvalueId: number, callbackMethod: any) {
        super.ajaxRequest("/MediaManager/Attributes/DeleteDefaultValues", "get", { "defaultvalueId": defaultvalueId }, callbackMethod, "json");
    }


    GetCustomFieldList(productId: number, callbackMethod) {
        super.ajaxRequest("/PIM/Products/CustomFields", "get", { "productId": productId }, callbackMethod, "html");
    }

    GetDownloadableProductKeyList(productId: number, sku: string, callbackMethod) {
        super.ajaxRequest("/PIM/Products/GetDownloadableProductKeys", "get", { "productId": productId, "sku": sku }, callbackMethod, "html");
    }

    AssociateCategories(catalogAssociation: Object, callbackMethod) {
        super.ajaxRequest("/PIM/Catalog/AssociateCatalogCategoryProduct", "post", { "catalogAssociation": catalogAssociation }, callbackMethod, "html");
    }

    GetCategoryFamilyDetails(familyId: string, CategoryId: string, callbackMethod) {
        super.ajaxRequest("/PIM/Category/GetAttributes", "get", { "familyId": familyId, "categoryId": CategoryId }, callbackMethod, "html");
    }

    DeleteCatalogs(pimCatalogId: string, isDeletePublishCatalog: boolean, callbackMethod) {
        super.ajaxRequest("/PIM/Catalog/Delete", "get", { "pimCatalogId": pimCatalogId, "isDeletePublishCatalog": isDeletePublishCatalog }, callbackMethod, "json");
    }

    DeleteCategories(pimCategoryId: string, callbackMethod) {
        super.ajaxRequest("/PIM/Category/Delete", "get", { "pimCategoryId": pimCategoryId }, callbackMethod, "json");
    }
    GetRegularExpressionValueByRuleName(attributeTypeId: string, ruleName: string, callbackMethod) {
        super.ajaxRequest("/MediaManager/Attributes/ValidationRuleRegularExpression", "post", { attributeTypeId: attributeTypeId, ruleName: ruleName }, callbackMethod, "json");
    }
    PublishCatagory(pimCategoryId: number, revisionType: string, callbackMethod) {
        super.ajaxRequest("/PIM/Category/PublishCategory", "get", { "pimCategoryId": pimCategoryId, "revisionType": revisionType }, callbackMethod, "json");
    }
    DeleteCustomFields(customFieldId: string, productId: number, callbackMethod) {
        super.ajaxRequest("/PIM/Products/DeleteCustomField", "get", { "customFieldId": customFieldId, "productId": productId }, callbackMethod, "json");
    }

    UnAssociateCategories(catalogAssociation: Object, callbackMethod) {
        super.ajaxRequest("/PIM/Catalog/UnAssociateCatalogCategoryProduct", "post", { "catalogAssociation": catalogAssociation }, callbackMethod, "html");
    }

    GetUnAssociateList(url: string, pimCatalogId: number, catalogName: string, callbackMethod) {
        super.ajaxRequest(url, "get", { "pimCatalogId": pimCatalogId, "catalogName": catalogName }, callbackMethod, "html");
    }

    GetCategoryTreePreview(pimCatalogId: number, callbackMethod) {
        super.ajaxRequest("/PIM/Catalog/GetCategoryTreePreview", "get", { "pimCatalogId": pimCatalogId }, callbackMethod, "json");
    }

    GetAutoCompleteCategorySearch(pimCatalogId: number, categoryName: string, callbackMethod) {
        super.ajaxRequest("/PIM/Catalog/GetAutoCompleteCategoryList", "get", { "pimCatalogId": pimCatalogId, "categoryName": categoryName }, callbackMethod, "json");
    }

    GetAutoCompleteProductSearch(productName: string, callbackMethod) {
        super.ajaxRequest("/PIM/Catalog/GetAutoCompleteProductList", "get", { "productName": productName }, callbackMethod, "json");
    }

    PublishCatalogCategoryProducts(pimCatalogId: number, pimCategoryHierarchyId: number, revisionType: string, callbackMethod) {
        super.ajaxRequest("/PIM/Catalog/PublishCatalogCategoryProducts", "get", { "pimCatalogId": pimCatalogId, "pimCategoryHierarchyId": pimCategoryHierarchyId, 'revisionType': revisionType }, callbackMethod, "json");
    }

    GetAssociatedAddons(productId: number, callbackMethod) {
        super.ajaxRequest("/PIM/Products/AssociatedAddonList", "get", { "parentProductId": productId }, callbackMethod, "html");
    }

    GetAssignedLinkProducts(productId: number, linkAttributeId: number, callbackMethod) {
        super.ajaxRequest("/PIM/Products/AssignedLinkProducts", "get", { "parentProductId": productId, "attributeId": linkAttributeId }, callbackMethod, "html");
    }

    GetPersonalizedAttribute(productId: number, callbackMethod) {
        super.ajaxRequest("/PIM/Products/GetAssignedPersonalizedAttributes", "get", { "productId": productId }, callbackMethod, "html");
    }

    DeletePrice(priceListId: string, callbackMethod) {
        super.ajaxRequest("/Price/Delete", "get", { "priceListId": priceListId }, callbackMethod, "json");
    }

    DeleteMultipleDepartments(departmentId: string, callbackMethod) {
        super.ajaxRequest("/Account/DeleteAccountDepartment", "get", { "departmentId": departmentId }, callbackMethod, "json");
    }

    DeleteMultipleNotes(noteId: string, callbackMethod) {
        super.ajaxRequest("/Account/DeleteAccountNote", "get", { "noteId": noteId }, callbackMethod, "json");
    }

    DeleteMultipleCustomerNotes(noteId: string, callbackMethod) {
        super.ajaxRequest("/Customer/DeleteCustomerNote", "get", { "noteId": noteId }, callbackMethod, "json");
    }

    DeleteMultipleCustomerAddress(userAddressId: string, accountAddressId: string, callbackMethod) {
        super.ajaxRequest("/Customer/DeleteAddress", "get", { "userAddressId": userAddressId, "accountAddressId": accountAddressId }, callbackMethod, "json");
    }

    DeleteMultipleAccount(accountId: string, callbackMethod) {
        super.ajaxRequest("/Account/Delete", "get", { "accountId": accountId }, callbackMethod, "json");
    }

    DeleteAddress(accountAddressId: string, callbackMethod) {
        super.ajaxRequest("/Account/DeleteAddress", "get", { "accountAddressId": accountAddressId }, callbackMethod, "json");
    }

    DeleteMultipleSKUPrice(priceId: string, priceListId: number, callbackMethod) {
        super.ajaxRequest("/Price/DeleteSKUPrice", "get", { "priceId": priceId, "priceListId": priceListId }, callbackMethod, "json");
    }

    DeleteMultipleTierPrice(priceTierId: string, callbackMethod) {
        super.ajaxRequest("/Price/DeleteTierPrice", "get", { "priceTierId": priceTierId }, callbackMethod, "json");
    }

    AddTierPrice(price: number, minQuantity: number, maxQuantity: number, activationDate: Date, expirationDate: Date, sku: string, callbackMethod) {
        super.ajaxRequest("/Price/AddTierPrice", "post", { price: price, minQuantity: minQuantity, maxQuantity: maxQuantity, activationDate: activationDate, expirationDate: expirationDate, sku: sku }, callbackMethod, "json");
    }

    GetCategoryAssociatedProductTreeStructure(catalogId: string, categoryId: string, callbackMethod) {
        super.ajaxRequest("/PIM/Catalog/GetCategoryAssociatedProducts", "get", { "catalogId": catalogId, "categoryId": categoryId }, callbackMethod, "json");
    }

    DeleteInventory(inventoryListId: string, callbackMethod) {
        super.ajaxRequest("/Inventory/Delete", "get", { "InventoryListId": inventoryListId }, callbackMethod, "json");
    }

    DeleteMultipleSKUInventory(inventoryId: string, callbackMethod) {
        super.ajaxRequest("/Inventory/DeleteSKUInventory", "get", { "InventoryId": inventoryId }, callbackMethod, "json");
    }

    DeleteAssociatedProfilesForAccounts(accountProfileIds: string, accountId: number, callbackMethod) {
        super.ajaxRequest("/Account/UnAssociateProfileToAccount", "get", { "accountProfileId": accountProfileIds, "accountId": accountId }, callbackMethod, "json");
    }

    DeleteMultipleAssociatedSkus(inventoryId: string, callbackMethod) {
        super.ajaxRequest("/Warehouse/DeleteSKUInventory", "get", { "InventoryId": inventoryId }, callbackMethod, "json");
    }

    EditTierPrice(priceTierId: string, callbackMethod) {
        super.ajaxRequest("/Price/EditTierPrice", "get", { "priceTierId": priceTierId }, callbackMethod, "html");
    }

    DeleteTierPrice(priceTierId: string, callbackMethod) {
        super.ajaxRequest("/Price/DeleteTierPrice", "get", { "priceTierId": priceTierId }, callbackMethod, "json");
    }

    GetUnAssociatedStoreListForCMS(PriceListId: number, callbackMethod) {
        super.ajaxRequest("/Theme/GetUnAssociatedStoreList", "get", { "cmsThemeId": PriceListId }, callbackMethod, "html");
    }

    AssociateStoreList(priceListId: number, storeIds: string, callbackMethod) {
        super.ajaxRequest("/Price/AssociateStore", "post", { "priceListId": priceListId, "storeIds": storeIds }, callbackMethod, "json");
    }

    AssociateStoreListForCMS(priceListId: number, storeIds: string, callbackMethod) {
        super.ajaxRequest("/Theme/AssociateStore", "post", { "cmsThemeId": priceListId, "storeIds": storeIds }, callbackMethod, "json");
    }

    SaveWidgetsToTheme(cmsThemeId: number, widgetIds: string, cmsAreaId: number, callbackMethod) {
        super.ajaxRequest("/Theme/SaveThemeWidgets", "post", { "cmsThemeId": cmsThemeId, "widgetIds": widgetIds, "cmsAreaId": cmsAreaId }, callbackMethod, "json");
    }

    AssociateCustomerList(priceListId: number, customerIds: string, callbackMethod) {
        super.ajaxRequest("/Price/AssociateCustomer", "post", { "priceListId": priceListId, "customerIds": customerIds }, callbackMethod, "json");
    }

    AssociatePriceListToStore(portalId: number, priceListIds: string, callbackMethod) {
        super.ajaxRequest("/Store/AssociatePriceListToStore", "post", { "portalId": portalId, "priceListIds": priceListIds }, callbackMethod, "json");
    }

    AssociatePriceListToProfile(profileId: number, priceListIds: string, portalId: number, callbackMethod) {
        super.ajaxRequest("/Store/AssociatePriceListToProfile", "post", { "profileId": profileId, "priceListIds": priceListIds, "portalId": portalId }, callbackMethod, "json");
    }

    DeleteMultipleCustomerPrice(priceListUserId: string, priceListId: number, callbackMethod) {
        super.ajaxRequest("/Price/DeleteAssociatedCustomer", "get", { "priceListUserId": priceListUserId, "priceListId": priceListId }, callbackMethod, "json");
    }

    DeleteAssociatedStores(priceListPortalId: string, priceListId: number, callbackMethod) {
        super.ajaxRequest("/Price/RemoveAssociatedStores", "get", { "priceListPortalId": priceListPortalId, "priceListId": priceListId }, callbackMethod, "json");
    }

    DeleteAssociatedStoresForCMS(priceListPortalId: string, callbackMethod) {
        super.ajaxRequest("/Theme/RemoveAssociatedStores", "get", { "priceListPortalId": priceListPortalId }, callbackMethod, "json");
    }

    AssociateProfileList(priceListId: number, profileIds: string, callbackMethod) {
        super.ajaxRequest("/Price/AssociateProfile", "post", { "priceListId": priceListId, "profileIds": profileIds }, callbackMethod, "json");
    }

    DeleteAssociatedProfiles(priceListProfileId: string, priceListId: number, callbackMethod) {
        super.ajaxRequest("/Price/RemoveAssociatedProfiles", "get", { "priceListProfileId": priceListProfileId, "priceListId": priceListId }, callbackMethod, "json");
    }

    DeleteWarehouse(warehouseId: string, callbackMethod) {
        super.ajaxRequest("/Warehouse/Delete", "get", { "warehouseId": warehouseId }, callbackMethod, "json");
    }

    GetUnAssociatedPriceListForStore(PortalId: number, callbackMethod) {
        super.ajaxRequest("/Store/GetUnAssociatedPriceListForStore", "get", { "PortalId": PortalId }, callbackMethod, "html");
    }

    GetUnAssociatedSortListForStore(PortalId: number, callbackMethod) {
        super.ajaxRequest("/Store/GetUnassociatedSortList", "get", { "PortalId": PortalId }, callbackMethod, "html");
    }

    GetUnassociatedSortList(PortalId: number, callbackMethod) {
        super.ajaxRequest("/Store/GetUnassociatedSortList", "get", { "PortalId": PortalId }, callbackMethod, "html");
    }

    GetUnassociatedPageList(PortalId: number, callbackMethod) {
        super.ajaxRequest("/Store/GetUnassociatedPageList", "get", { "PortalId": PortalId }, callbackMethod, "html");
    }

    RemoveAssociatedPriceListFromStore(priceListPortalId: string, callbackMethod) {
        super.ajaxRequest("/Store/RemoveAssociatedPriceListToStore", "get", { "priceListPortalId": priceListPortalId }, callbackMethod, "json");
    }

    RemoveAssociatedPriceListFromProfile(priceListProfileId: string, callbackMethod) {
        super.ajaxRequest("/Store/RemoveAssociatedPriceListToProfile", "get", { "priceListProfileId": priceListProfileId }, callbackMethod, "json");
    }

    GetUnAssociatedPriceListForProfile(profileId: number, portalId: number, callbackMethod) {
        super.ajaxRequest("/Store/GetUnAssociatedPriceListForProfile", "get", { "profileId": profileId, "portalId": portalId }, callbackMethod, "html");
    }

    GetAssociatedPriceListForProfile(PortalId, ProfileId: number, callbackMethod) {
        super.ajaxRequest("/Store/GetAssociatedPriceListForProfile", "get", { "PortalId": PortalId, "ProfileId": ProfileId }, callbackMethod, "html");
    }

    GetCMSAreaWidgets(cmsThemeId: number, cmsAreaId: number, callbackMethod) {
        super.ajaxRequest("/Theme/GetCMSAreaWidgets", "get", { "cmsThemeId": cmsThemeId, "cmsAreaId": cmsAreaId }, callbackMethod, "html");
    }

    GetCMSAreas(cmsThemeId: number, callbackMethod) {
        super.ajaxRequest("/Theme/GetCMSAreas", "get", { "cmsThemeId": cmsThemeId }, callbackMethod, "html");
    }

    AddonDelete(addonIds: string, callbackMethod) {
        super.ajaxRequest("/PIM/Products/UnassociateAddon", "get", { "pimAddonDetailId": addonIds }, callbackMethod, "json");
    }

    LinkProductDelete(linkProductDetailIds: string, callbackMethod) {
        super.ajaxRequest("/PIM/Products/UnAssignLinkProducts", "get", { "pimLinkProductDetailId": linkProductDetailIds }, callbackMethod, "json");
    }

    GetUnAssociatedCategoryProducts(categoryId: number, callbackMethod) {
        super.ajaxRequest("/PIM/Category/GetUnAssociatedCategoryProducts", "get", { "categoryId": categoryId }, callbackMethod, "html");
    }
    AssociateCategoryProducts(categoryId: number, productIds: string, callbackMethod) {
        super.ajaxRequest("/PIM/Category/AssociatedCategoryProducts", "get", { "categoryId": categoryId, "productIds": productIds }, callbackMethod, "json", false);
    }

    AssociateCategoriesToProduct(productId: number, categoryIds: string, callbackMethod) {
        super.ajaxRequest("/PIM/Category/AssociateCategoriesToProduct", "get", { "productId": productId, "categoryIds": categoryIds }, callbackMethod, "json");
    }

    DeleteMultipleAssociatedProducts(pimCategoryProductId: string, callbackMethod) {
        super.ajaxRequest("/PIM/Category/DeleteAssociatedProducts", "get", { "pimCategoryProductId": pimCategoryProductId }, callbackMethod, "json");
    }
    EditCategoryProduct(categoryProductId: string, callbackMethod) {
        super.ajaxRequest("/PIM/Category/EditCategoryProduct", "get", { "categoryProductId": categoryProductId }, callbackMethod, "html");
    }
    GetAssociatedCategoryProducts(categoryId: number, callbackMethod) {
        super.ajaxRequest("/PIM/Category/GetAssociatedCategoryProducts", "get", { "pimCategoryId": categoryId }, callbackMethod, "html");
    }

    GetWarehouses(portalId: number, warehouseId: number, callbackMethod: any) {
        super.ajaxRequest("/Store/GetAssociatedWarehouseList", "get", { "portalId": portalId, "warehouseId": warehouseId }, callbackMethod, "html");
    }

    AssociateWarehouseToStore(portalId: number, warehouseId: number, alternateWarehouseIds: string, callbackMethod) {
        super.ajaxRequest("/Store/AssociateWarehouseToStore", "post", { "portalId": portalId, "warehouseId": warehouseId, "alternateWarehouseIds": alternateWarehouseIds }, callbackMethod, "html");
    }

    DeleteAssociatedProducts(associatedProductIds: string, callbackMethod) {
        super.ajaxRequest("/PIM/Products/UnassociateProducts", "get", { "pimProductTypeAssociationId": associatedProductIds }, callbackMethod, "json");
    }

    DeleteAssociatedCategories(pimCategoryId: string, callbackMethod) {
        super.ajaxRequest("/PIM/Category/DeleteAssociatedCategories", "get", { "pimCategoryProductId": pimCategoryId }, callbackMethod, "json");
    }

    PreViewPriceImportList(file: any, callbackMethod) {
        super.ajaxRequest("/Price/PreviewImportPrice", "get", { "file": file }, callbackMethod, "html");
    }

    DeleteProducts(productId: string, callbackMethod) {
        super.ajaxRequest("/PIM/Products/Delete", "get", { "PimProductId": productId }, callbackMethod, "json");
    }

    GetSimilarCombination(productId: number, callbackMethod) {
        super.ajaxRequest("/PIM/Products/GetSimilarCombination", "get", { "productId": productId }, callbackMethod, "json");
    }

    GetAssociatedPriceListForStore(PortalId: number, callbackMethod) {
        super.ajaxRequest("/Store/GetAssociatedPriceListForStore", "get", { "PortalId": PortalId }, callbackMethod, "html");
    }

    GetAssociatedPriceListForAccount(accountId: number, callbackMethod) {
        super.ajaxRequest("/Account/GetAssociatedPriceListForAccount", "get", { "accountId": accountId }, callbackMethod, "html");
    }

    GetTierPrice(priceListId: number, sku: string, callbackMethod) {
        super.ajaxRequest("/Price/AddTierPrice", "get", { "priceListId": priceListId, "sku": sku }, callbackMethod, "html");
    }

    PriceTierList(priceListId: number, sku: string, callbackMethod) {
        super.ajaxRequest("/Price/PriceTierList", "get", { "priceListId": priceListId, "sku": sku }, callbackMethod, "html");
    }

    PriceTierListUsingLazyLoading(priceListId: number, sku: string, pageNumber: number, callbackMethod) {
        super.ajaxRequest("/Price/PriceTierList", "get", { "priceListId": priceListId, "sku": sku, 'pageNumber': pageNumber }, callbackMethod, "json", true, false);
    }

    BoostAndBuryList(catalogId: number, callbackMethod) {
        super.ajaxRequest("/Search/Search/GetBoostAndBuryRules", "get", { "catalogId": catalogId, "catalogName": "" }, callbackMethod, "html");
    }

    GetLinkWidgetConfigurationList(cmsWidgetsId: number, cmsMappingId: number, widgetKey: string, typeOFMapping: string, displayName: string, widgetName: string, fileName: string, localeId: number, callbackMethod) {
        super.ajaxRequest("/WebSite/GetLinkWidgetConfigurationList", "get", {
            "cmsWidgetsId": cmsWidgetsId, "cmsMappingId": cmsMappingId, "widgetsKey": widgetKey, "typeOFMapping": typeOFMapping, "displayName": displayName, "widgetName": widgetName, "fileName": fileName, "localeId": localeId
        }, callbackMethod, "html");
    }

    DeleteMultipleShipping(shippingIds: string, callbackMethod) {
        super.ajaxRequest("/shippings/Delete", "get", { "shippingId": shippingIds }, callbackMethod, "json");
    }

    DeleteMultipleShippingSKU(shippingSKUId: string, callbackMethod) {
        super.ajaxRequest("/Shippings/DeleteShippingSKU", "get", { "shippingSKUId": shippingSKUId }, callbackMethod, "json");
    }


    ShippingSKUList(shippingId: number, shippingRuleId: number, shippingRuleType: string, callbackMethod) {
        super.ajaxRequest("/Shippings/ShippingSKUList", "get", { "shippingId": shippingId, "shippingRuleId": shippingRuleId, "shippingRuleType": shippingRuleType }, callbackMethod, "html");
    }

    EditShippingSKU(shippingSKUId: string, callbackMethod) {
        super.ajaxRequest("/Shippings/EditShippingSKU", "get", { "shippingSKUId": shippingSKUId }, callbackMethod, "html");
    }

    EditPortalCatalog(portalCatalogId: string, callbackMethod) {
        super.ajaxRequest("/Store/EditPortalCatalog", "get", { "portalCatalogId": portalCatalogId }, callbackMethod, "html");
    }

    GetPortalAssociatedCatalog(portalId: number, callbackMethod) {
        super.ajaxRequest("/Store/GetAssociatedPortalCatalog", "get", { "portalId": portalId }, callbackMethod, "html");
    }

    DeleteMultipleTaxClass(taxClassIds: string, callbackMethod) {
        super.ajaxRequest("/TaxClass/Delete", "get", { "taxClassId": taxClassIds }, callbackMethod, "json");
    }

    DeleteMultipleTaxClassSKU(taxClassSKUId: string, callbackMethod) {
        super.ajaxRequest("/TaxClass/DeleteTaxClassSKU", "get", { "taxClassSKUId": taxClassSKUId }, callbackMethod, "json");
    }

    AddTaxClassSKU(taxClassId: number, name: string, callbackMethod) {
        super.ajaxRequest("/TaxClass/AddTaxClassSKU", "get", { "taxClassId": taxClassId, "name": name }, callbackMethod, "html");
    }

    TaxClassSKUList(taxClassId: number, name: string, callbackMethod) {
        super.ajaxRequest("/TaxClass/TaxClassSKUList", "get", { "taxClassId": taxClassId, "name": name }, callbackMethod, "html");
    }

    EditTaxClassSKU(taxClassSKUId: string, callbackMethod) {
        super.ajaxRequest("/TaxClass/EditTaxClassSKU", "get", { "taxClassSKUId": taxClassSKUId }, callbackMethod, "html");
    }

    GetConfigureAttributeDetails(familyId: number, productId: number, callbackMethod) {
        super.ajaxRequest("/PIM/Products/GetConfigureAttributeList/", "get", { "familyId": familyId, "productId": productId }, callbackMethod, "html");
    }

    AssociateAccountList(priceListId: number, accountIds: string, callbackMethod) {
        super.ajaxRequest("/Price/AssociateAccount", "post", { "priceListId": priceListId, "customerIds": accountIds }, callbackMethod, "json");
    }

    DeleteMultipleAccountPrice(priceListAccountId: string, priceListId: number, callbackMethod) {
        super.ajaxRequest("/Price/DeleteAssociatedAccount", "get", { "priceListAccountId": priceListAccountId, "priceListId": priceListId }, callbackMethod, "json");
    }

    DeleteMultipleTaxRule(taxRuleId: string, taxClassId: number, callbackMethod) {
        super.ajaxRequest("/TaxClass/DeleteTaxRule", "get", { "taxRuleId": taxRuleId, "taxClassId": taxClassId }, callbackMethod, "json");
    }

    AddTaxRule(taxClassId: number, callbackMethod) {
        super.ajaxRequest("/TaxClass/AddTaxRule", "get", { "taxClassId": taxClassId }, callbackMethod, "html");
    }

    TaxRuleList(taxClassId: number, callbackMethod) {
        super.ajaxRequest("/TaxClass/TaxRuleList", "get", { "taxClassId": taxClassId }, callbackMethod, "html");
    }

    PriceSKUList(priceListId: number, listName: string, callbackMethod) {
        super.ajaxRequest("/Price/PriceSKUList", "get", { "priceListId": priceListId, "listName": listName }, callbackMethod, "html");
    }

    GetPriceBySku(pimProductId: string, sku: string, productType: string, callbackMethod) {
        super.ajaxRequest("/Price/GetPriceBySku", "get", { "pimProductId": pimProductId, "sku": sku, "producttype": productType }, callbackMethod, "json");
    }

    EditTaxRule(taxRuleId: string, taxClassId: number, callbackMethod) {
        super.ajaxRequest("/TaxClass/EditTaxRule", "get", { "taxRuleId": taxRuleId, "taxClassId": taxClassId }, callbackMethod, "html");
    }

    UpdateAddonGroup(addonGroupData: any, callbackMethod) {
        super.ajaxRequest("/PIM/Products/EditAddonGroup/", "post", { "model": addonGroupData }, callbackMethod, "json");
    }

    GetUnassociatedConfigureProducts(parentProductId: number, associatedAttributeIds: string, associatedProductIds: string, callbackMethod) {
        super.ajaxRequest("/PIM/Products/GetUnassociatedConfigureProducts", "get", {
            "ParentProductId": parentProductId, "attributeIds": associatedAttributeIds, "associatedProductIds": associatedProductIds
        }, callbackMethod, "html");
    }

    AddSlider(callbackMethod) {
        super.ajaxRequest("/WebSite/CreateSlider", "get", "", callbackMethod, "html");
    }

    CopyStore(portalId: string, callbackMethod) {
        super.ajaxRequest("/Store/CopyStore", "get", { "portalId": portalId }, callbackMethod, "html");
    }

    EditSlider(cmsSliderId: string, callbackMethod) {
        super.ajaxRequest("/WebSite/EditSlider", "get", { "cmsSliderId": cmsSliderId }, callbackMethod, "html");
    }

    GetSliderList(callbackMethod) {
        super.ajaxRequest("/WebSite/GetSliderList", "get", "", callbackMethod, "html");
    }

    GetStoreList(callbackMethod) {
        super.ajaxRequest("/Store/List", "get", "", callbackMethod, "html");
    }

    DeleteSliders(cmsSliderId: string, callbackMethod) {
        super.ajaxRequest("/WebSite/DeleteSlider", "get", { "cmsSliderId": cmsSliderId }, callbackMethod, "json");
    }

    PublishSlider(CMSSliderId: string, targetPublishState: string, callbackMethod) {
        super.ajaxRequest("/WebSite/PublishSliderWithPreview", "get", { "cmsSliderId": CMSSliderId, "targetPublishState": targetPublishState, "portalId": 0, "localeId": 0, "takeFromDraftFirst": true }, callbackMethod, "json");
    }

    DeleteBanners(cmsSliderBannerId: string, callbackMethod) {
        super.ajaxRequest("/WebSite/DeleteBanner", "get", { "cmsSliderBannerId": cmsSliderBannerId }, callbackMethod, "json");
    }


    DeleteCustomerReview(cmsCustomerReviewId: string, callbackMethod) {
        super.ajaxRequest("/Review/Delete", "get", { "cmsCustomerReviewId": cmsCustomerReviewId }, callbackMethod, "json");
    }

    StateListByCountryCode(countryCode: string, callbackMethod) {
        super.ajaxRequest("/TaxClass/BindStateList", "get", { "countryCode": countryCode }, callbackMethod, "json");
    }

    CountyListByStateCode(stateCode: string, callbackMethod) {
        super.ajaxRequest("/TaxClass/BindCountyList", "get", { "stateCode": stateCode }, callbackMethod, "json");
    }

    DeleteTheme(cmsThemeId: string, name: string, callbackMethod) {
        super.ajaxRequest("/Theme/Delete", "get", { "cmsThemeId": cmsThemeId, "name": name }, callbackMethod, "json");
    }

    DeleteRevisedTheme(cmsThemeId: number, name: string, callbackMethod) {
        super.ajaxRequest("/Theme/DeleteRevisedTheme", "get", { "cmsThemeId": cmsThemeId, "name": name }, callbackMethod, "json");
    }

    GetEditStaticPage(cmsContentPagesId: number, callbackMethod) {
        super.ajaxRequest("/WebSite/EditStaticPage", "get", { "cmsContentPagesId": cmsContentPagesId }, callbackMethod, "html");
    }

    EditContentPage(cmsContentPagesId: string, callbackMethod) {
        super.ajaxRequest("/WebSite/EditStaticPage", "get", { "cmsContentPagesId": cmsContentPagesId }, callbackMethod, "html");
    }

    DeleteStaticPage(cmsContentPagesId: string, callbackMethod) {
        super.ajaxRequest("/WebSite/DeleteStaticPage", "get", { "cmsContentPagesId": cmsContentPagesId }, callbackMethod, "json");
    }

    DeleteLinkWidgetConfiguration(id: string, localeId: number, callbackMethod) {
        super.ajaxRequest("/WebSite/DeleteLinkWidgetConfiguration", "get", { "cmsWidgetTitleConfigurationId": id, "localeId": localeId }, callbackMethod, "json");
    }
    GetPublishInfo(url: string, callbackMethod) {
        super.ajaxRequest(url, "get", "", callbackMethod, "html");
    }

    DeleteAddonGroups(addonGroupIds: string, callbackMethod) {
        super.ajaxRequest("/PIM/AddonGroup/DeleteAddonGroup", "get", { "pimAddOnGroupId": addonGroupIds }, callbackMethod, "json");
    }

    DeleteMultipleShippingRule(shippingRuleId: string, shippingId: number, callbackMethod) {
        super.ajaxRequest("/Shippings/DeleteShippingRule", "get", { "shippingRuleId": shippingRuleId, "shippingId": shippingId }, callbackMethod, "json");
    }

    AddShippingRule(shippingId: number, callbackMethod) {
        super.ajaxRequest("/Shippings/AddShippingRule", "get", { "shippingId": shippingId }, callbackMethod, "html");
    }

    ShippingRuleList(shippingId: number, callbackMethod) {
        super.ajaxRequest("/Shippings/ShippingRuleList", "get", { "shippingId": shippingId }, callbackMethod, "html");
    }

    EditShippingRule(shippingRuleId: string, shippingId: number, callbackMethod) {
        super.ajaxRequest("/Shippings/EditShippingRule", "get", { "shippingRuleId": shippingRuleId, "shippingId": shippingId }, callbackMethod, "html");
    }

    GetEditContentPageData(cmsContentPagesId: number, callbackMethod) {
        super.ajaxRequest("/Content/EditStaticPage", "get", { "cmsContentPagesId": cmsContentPagesId }, callbackMethod, "html");
    }

    GetContentPageListData(id: number, callbackMethod) {
        super.ajaxRequest("/Content/StaticPageList", "get", { "id": id }, callbackMethod, "html");
    }

    DeleteContentPage(cmsContentPagesId: string, callbackMethod) {
        super.ajaxRequest("/Content/DeleteContentPage", "get", { "cmsContentPagesId": cmsContentPagesId }, callbackMethod, "json");
    }

    EditStaticPage(cmsContentPagesId: string, callbackMethod) {
        super.ajaxRequest("/Content/EditStaticPage", "get", { "cmsContentPagesId": cmsContentPagesId }, callbackMethod, "html");
    }

    GetUnassociatedAddonGroup(parentProductId: number, callbackMethod) {
        super.ajaxRequest("/PIM/Products/GetUnassociatedAddonGroups", "get", { "parentProductId": parentProductId }, callbackMethod, "html");
    }

    GetAssociatedAddonlist(parentProductId: number, callbackMethod) {
        super.ajaxRequest("/PIM/Products/GetAssociatedAddonGroups", "get", { "parentProductId": parentProductId }, callbackMethod, "html");
    }

    DeleteManageMessage(cmsPortalMessageId: string, callbackMethod) {
        super.ajaxRequest("/Content/DeleteManageMessage", "get", { "cmsPortalMessageId": cmsPortalMessageId }, callbackMethod, "json");
    }

    GetAssociatedStoreList(priceListId: number, callbackMethod) {
        super.ajaxRequest("/Theme/GetAssociatedStoreList", "get", { "cmsThemeId": priceListId }, callbackMethod, "html");
    }

    DeleteManageMessageForWebsite(cmsMessageId: string, callbackMethod) {
        super.ajaxRequest("/WebSite/DeleteManageMessage", "get", { "cmsMessageId": cmsMessageId }, callbackMethod, "json");
    }

    DeleteAddonProduct(addonProductId: number, parentProductId: number, callbackMethod) {
        super.ajaxRequest("/PIM/Products/DeleteAddonProduct", "get", { "addonProductId": addonProductId, "parentProductId": parentProductId }, callbackMethod, "html")
    }

    AssociatedAddonProduct(model: Object, callbackMethod) {
        super.ajaxRequest("/PIM/Products/AssociatedAddonProduct", "post", { "model": model }, callbackMethod, "json")
    }

    UnassociateAddonProducts(addonProductDetailId, callbackMethod) {
        super.ajaxRequest("/PIM/Products/UnassociateAddonProducts", "get", { "pimAddonProductDetailId": addonProductDetailId }, callbackMethod, "json");
    }

    AssociateProduct(cmsWidgetsId: number, cmsMappingId: number, WidgetsKey: string, TypeOfMapping: string, SKUs: string, callbackMethod) {
        super.ajaxRequest("/WebSite/AssociateProduct", "post", { "cmsWidgetsId": cmsWidgetsId, "cmsMappingId": cmsMappingId, "WidgetKey": WidgetsKey, "TypeOfMapping": TypeOfMapping, "SKUs": SKUs }, callbackMethod, "json");
    }

    UnAssociateProduct(cmsWidgetProductId: string, callbackMethod) {
        super.ajaxRequest("/WebSite/UnAssociateProduct", "get", { "cmsWidgetProductId": cmsWidgetProductId }, callbackMethod, "json");
    }

    GetUnAssociatedProductList(cmsContentPagesId: number, callbackMethod) {
        super.ajaxRequest("/Content/GetUnAssociatedProductList", "get", { "cmsContentPagesId": cmsContentPagesId }, callbackMethod, "html");
    }

    DeleteAccountCustomers(id: string, callbackMethod) {
        super.ajaxRequest("/Account/CustomerDelete", "get", { "userId": id }, callbackMethod, "json");
    }

    GetProductsToBeAssociated(associatedProductIds, productType, callbackMethod) {
        super.ajaxRequest("/PIM/Products/GetProductsToBeAssociated", "get", { "productIds": associatedProductIds, "productType": productType }, callbackMethod, "html");
    }

    GetConfigureProductsToBeAssociated(associatedProductIds, associatedAttributeIds, callbackMethod) {
        super.ajaxRequest("/PIM/Products/GetConfigureProductsToBeAssociated", "get", { "associatedProductIds": associatedProductIds, "associatedAttributeIds": associatedAttributeIds }, callbackMethod, "html");
    }

    GetUnassociatedCategory(cmsWidgetsId: number, cmsMappingId: number, widgetKey: string, typeOFMapping: string, displayName: string, widgetName: string, fileName: string, callbackMethod) {
        super.ajaxRequest("/WebSite/GetUnAssociatedCategoryList", "get", { "cmsWidgetsId": cmsWidgetsId, "cmsMappingId": cmsMappingId, "widgetKey": widgetKey, "typeOFMapping": typeOFMapping, "displayName": displayName, "widgetName": widgetName, "fileName": fileName }, callbackMethod, "html");
    }

    GetUnassociatedBrand(cmsWidgetsId: number, cmsMappingId: number, widgetKey: string, typeOFMapping: string, displayName: string, widgetName: string, fileName: string, callbackMethod) {
        super.ajaxRequest("/WebSite/GetUnAssociatedBrandList", "get", { "cmsWidgetsId": cmsWidgetsId, "cmsMappingId": cmsMappingId, "widgetKey": widgetKey, "typeOFMapping": typeOFMapping, "displayName": displayName, "widgetName": widgetName, "fileName": fileName }, callbackMethod, "html");
    }

    GetUnassociatedCountryList(portalId: number, callbackMethod) {
        super.ajaxRequest("/Store/GetUnAssociatedCountryList", "get", { "portalId": portalId }, callbackMethod, "html");
    }

    GetUnAssociatedProfileList(userId: number, callbackMethod) {
        super.ajaxRequest("/Customer/GetUnAssociatedProfileList", "get", { "userId": userId }, callbackMethod, "html");
    }

    CreateLinkWidgetConfiguration(cmsWidgetsId: number, cmsMappingId: number, widgetKey: string, typeOFMapping: string, displayName: string, widgetName: string, callbackMethod) {
        super.ajaxRequest("/WebSite/CreateLinkWidgetConfiguration", "get", { "cmsWidgetsId": cmsWidgetsId, "cmsMappingId": cmsMappingId, "widgetKey": widgetKey, "typeOFMapping": typeOFMapping, "displayName": displayName, "widgetName": widgetName }, callbackMethod, "html");
    }

    RemoveAssociatedCategories(cmsWidgetCategoryId: string, callbackMethod) {
        super.ajaxRequest("/WebSite/RemoveAssociatedCategories", "get", { "cmsWidgetCategoryId": cmsWidgetCategoryId }, callbackMethod, "json");
    }

    RemoveAssociatedBrands(cmsWidgetBrandId: string, callbackMethod) {
        super.ajaxRequest("/WebSite/RemoveAssociatedBrands", "get", { "cmsWidgetBrandId": cmsWidgetBrandId }, callbackMethod, "json");
    }

    RemoveAssociatedCountries(portalCountryId: string, portalId: number, callbackMethod) {
        super.ajaxRequest("/Store/UnAssociateCountries", "get", { "portalCountryId": portalCountryId, "portalId": portalId }, callbackMethod, "json");
    }

    UnAssociatePriceListFromAccount(priceListId: string, accountId: number, callbackMethod) {
        super.ajaxRequest("/Account/UnAssociatePriceListToAccount", "get", { "priceListId": priceListId, "accountId": accountId }, callbackMethod, "json");
    }

    UnAssociateProfiles(profileId: string, userId: number, callbackMethod) {
        super.ajaxRequest("/Customer/UnAssociateProfiles", "get", { "profileId": profileId, "userId": userId }, callbackMethod, "json");
    }

    GetProductListBySKU(attributeValue: string, callbackMethod) {
        super.ajaxRequest("/PIM/Products/GetProductListBySKU", "get", { "attributeValue": attributeValue }, callbackMethod, "json");
    }

    GetCatalogList(catalogName: string, callbackMethod) {
        super.ajaxRequest("/Account/GetCatalog", "get", { "catalogName": catalogName }, callbackMethod, "json");
    }

    GetTaxAssociatedStoreList(taxClassId: number, name: string, callbackMethod) {
        super.ajaxRequest("/TaxClass/GetAssociatedStoreList", "get", { "taxClassId": taxClassId, "name": name }, callbackMethod, "html");
    }

    GetUnassociatedStoreList(taxClassId: number, callbackMethod) {
        super.ajaxRequest("/TaxClass/GetUnassociatedStoreList", "get", { "taxClassId": taxClassId }, callbackMethod, "html");
    }

    AssociateStore(taxClassId: number, storeIds: string, callbackMethod) {
        super.ajaxRequest("/TaxClass/AssociateStore", "post", { "taxClassId": taxClassId, "storeIds": storeIds }, callbackMethod, "json");
    }

    UnassociateStore(taxClassPortalId: string, taxClassId: number, callbackMethod) {
        super.ajaxRequest("/TaxClass/UnassociateStore", "get", { "taxClassPortalId": taxClassPortalId, "taxClassId": taxClassId }, callbackMethod, "json");
    }

    AssociateCategoriesForWebSite(cmsWidgetsId: number, cmsMappingId: number, widgetKey: string, typeOFMapping: string, categoryCodes: string, callbackMethod) {
        super.ajaxRequest("/WebSite/AssociateCategories", "post", { "cmsWidgetsId": cmsWidgetsId, "categoryCodes": categoryCodes, "cmsMappingId": cmsMappingId, "widgetKey": widgetKey, "typeOFMapping": typeOFMapping }, callbackMethod, "json");
    }

    AssociateBrandsForWebSite(cmsWidgetsId: number, cmsMappingId: number, widgetKey: string, typeOFMapping: string, brandId: string, callbackMethod) {
        super.ajaxRequest("/WebSite/AssociateBrands", "post", { "cmsWidgetsId": cmsWidgetsId, "brandId": brandId, "cmsMappingId": cmsMappingId, "widgetKey": widgetKey, "typeOFMapping": typeOFMapping }, callbackMethod, "json");
    }

    AssociateCountriesForStore(portalId: number, countryCode: string, isDefault: boolean, portalCountryId: number, callbackMethod) {
        super.ajaxRequest("/Store/AssociateCountries", "post", { "countryCode": countryCode, "portalId": portalId, "isDefault": isDefault, "portalCountryId": portalCountryId }, callbackMethod, "json");
    }

    AssociateProfilesForCustomer(profileIds: string, userId: number, callbackMethod) {
        super.ajaxRequest("/Customer/AssociateProfiles", "post", { "profileIds": profileIds, "userId": userId }, callbackMethod, "json");
    }

    GetAssociatedCatagoryList(cmsWidgetsId: number, cmsMappingId: number, widgetKey: string, typeOFMapping: string, displayName: string, widgetName: string, callbackMethod) {
        super.ajaxRequest("/WebSite/GetAssociatedCategoryList", "get", { "cmsWidgetsId": cmsWidgetsId, "cmsMappingId": cmsMappingId, "widgetKey": widgetKey, "typeOFMapping": typeOFMapping, "displayName": displayName, "widgetName": widgetName }, callbackMethod, "html");
    }

    GetShippingAssociatedStoreList(shippingId: number, shippingTypeId: number, callbackMethod) {
        super.ajaxRequest("/Shippings/GetAssociatedStoreList", "get", { "shippingId": shippingId, "shippingTypeId": shippingTypeId }, callbackMethod, "html");
    }

    GetShippingUnassociatedStoreList(shippingId: number, callbackMethod) {
        super.ajaxRequest("/Shippings/GetUnassociatedStoreList", "get", { "shippingId": shippingId }, callbackMethod, "html");
    }

    AssociateStoreToShipping(shippingId: number, storeIds: string, callbackMethod) {
        super.ajaxRequest("/Shippings/AssociateStore", "post", { "shippingId": shippingId, "storeIds": storeIds }, callbackMethod, "json");
    }

    UnassociateStoreFromShipping(shippingPortalId: string, shippingId: number, callbackMethod) {
        super.ajaxRequest("/Shippings/UnassociateStore", "get", { "shippingPortalId": shippingPortalId, "shippingId": shippingId }, callbackMethod, "json");
    }

    EditAssociatedStoresPrecedence(priceListPortalId: string, callbackMethod) {
        super.ajaxRequest("/Price/EditAssociatedStoresPrecedence", "get", { "priceListPortalId": priceListPortalId }, callbackMethod, "html");
    }

    AssociatedStoresList(priceListId: number, listName: string, callbackMethod) {
        super.ajaxRequest("/Price/GetAssociatedStoreList", "get", { "priceListId": priceListId, "listName": listName }, callbackMethod, "html");
    }

    DeleteUrlRedirect(id: string, callbackMethod) {
        super.ajaxRequest("/SEO/DeleteUrlRedirect", "get", { "cmsUrlRedirectId": id }, callbackMethod, "json");
    }

    EditAssociatedCustomerPrecedence(priceListUserId: string, callbackMethod) {
        super.ajaxRequest("/Price/EditAssociatedCustomerPrecedence", "get", { "priceListUserId": priceListUserId }, callbackMethod, "html");
    }

    AssociatedCustomerList(priceListId: number, listName: string, callbackMethod) {
        super.ajaxRequest("/Price/GetAssociatedCustomerList", "get", { "priceListId": priceListId, "listName": listName }, callbackMethod, "html");
    }

    EditAssociatedAccountPrecedence(priceListAccountId: string, callbackMethod) {
        super.ajaxRequest("/Price/EditAssociatedAccountPrecedence", "get", { "priceListAccountId": priceListAccountId }, callbackMethod, "html");
    }

    AssociatedAccountList(priceListId: number, listName: string, callbackMethod) {
        super.ajaxRequest("/Price/GetAssociatedAccountList", "get", { "priceListId": priceListId, "listName": listName }, callbackMethod, "html");
    }

    EditAssociatedProfilePrecedence(priceListProfileId: string, callbackMethod) {
        super.ajaxRequest("/Price/EditAssociatedProfilePrecedence", "get", { "priceListProfileId": priceListProfileId }, callbackMethod, "html");
    }

    AssociatedProfileList(priceListId: number, listName: string, callbackMethod) {
        super.ajaxRequest("/Price/GetAssociatedProfileList", "get", { "priceListId": priceListId, "listName": listName }, callbackMethod, "html");
    }

    DeleteStoreLocator(portalId: string, callbackMethod) {
        super.ajaxRequest("/StoreLocator/Delete", "get", { "PortalAddressId": portalId }, callbackMethod, "json");
    }
    DeleteStoreLocatorByCode(storeLocationCode: string, callbackMethod) {
        super.ajaxRequest("/StoreLocator/DeleteByCode", "get", { "storeLocationCode": storeLocationCode }, callbackMethod, "json");
    }

    GetCreateAddonGroupForm(callbackMethod) {
        super.ajaxRequest("/PIM/AddonGroup/CreateAddonGroup", "get", {}, callbackMethod, "html");
    }

    CreateAddonGroup(model: any, callbackMethod) {
        super.ajaxRequest("/PIM/AddonGroup/CreateAddonGroup", "post", { "model": model }, callbackMethod, "json");
    }

    DeleteMultipleCatalogAssociatedCategories(pimCatalogId: number, pimCategoryHierarchyId: string, callbackMethod) {
        super.ajaxRequest("/PIM/Catalog/DeleteAssociateCategory", "get", { "pimCatalogId": pimCatalogId, "pimCategoryHierarchyId": pimCategoryHierarchyId }, callbackMethod, "json");
    }

    DeleteMultiplePromotion(promotionIds: string, callbackMethod) {
        super.ajaxRequest("/Promotion/Delete", "get", { "promotionId": promotionIds }, callbackMethod, "json");
    }

    DeleteProfiles(profileId: string, callbackMethod) {
        super.ajaxRequest("/Profiles/Delete", "get", { "profileId": profileId }, callbackMethod, "json");
    }

    GetAssociatedCategoryDetails(catalogId: number, categoryId: number, callbackMethod) {
        super.ajaxRequest("/PIM/Catalog/EditCategorySettings", "get", { "catalogId": catalogId, "categoryId": categoryId }, callbackMethod, "html");
    }

    GetAssociatedCategoryList(catalogId: number, callbackMethod) {
        super.ajaxRequest("/PIM/Catalog/GetAssociatedCategoryList", "get", { "catalogId": catalogId }, callbackMethod, "html");
    }

    UpdateAssociatedCategoryDetails(catalogAssociateCategoryViewModel: any, callbackMethod) {
        super.ajaxRequest("/PIM/Catalog/EditCategorySettings", "post", { "catalogAssociateCategoryViewModel": catalogAssociateCategoryViewModel }, callbackMethod, "json");
    }

    CatalogListByStorIds(storeIds: string, callbackMethod) {
        super.ajaxRequest("/Promotion/BindCatalogList", "get", { "storeIds": storeIds }, callbackMethod, "json", false);
    }

    CategoryListByStorIds(storeIds: string, callbackMethod) {
        super.ajaxRequest("/Promotion/BindCategoryList", "get", { "storeIds": storeIds }, callbackMethod, "json", false);
    }

    ProductListByStorIds(storeIds: string, callbackMethod) {
        super.ajaxRequest("/Promotion/BindProductList", "get", { "storeIds": storeIds }, callbackMethod, "json", false);
    }

    UpdateAddonProductAssociation(addonProductmodel: Object, callbackMethod) {
        super.ajaxRequest("/PIM/Products/UpdateProductAddonAssociation", "post", { "addonProductViewModel": addonProductmodel }, callbackMethod, "json", false);
    }

    IsAttributeCodeExist(attributeCode: string, callbackMethod) {
        super.ajaxRequest("/Attributes/IsAttributeCodeExist", "get", { "attributeCode": attributeCode }, callbackMethod, "json", false);
    }

    IsPIMAttributeCodeExist(attributeCode: string, isCategory: boolean, callbackMethod) {
        super.ajaxRequest("/PIM/ProductAttribute/IsAttributeCodeExist", "get", { "attributeCode": attributeCode, "isCategory": isCategory }, callbackMethod, "json", false);
    }

    ChangeStatus(cmsCustomerReviewId: string, statusId: string, callbackMethod) {
        super.ajaxRequest("/Review/BulkStatusChange", "get", { "cmsCustomerReviewId": cmsCustomerReviewId, "statusId": statusId }, callbackMethod, "html");
    }

    IsAttributeValueUnique(attributeCodeValues: string, id: number, isCategory: boolean, callbackMethod) {
        super.ajaxRequest("/PIM/ProductAttribute/IsAttributeValueUnique", "get", { "attributeCodeValues": attributeCodeValues, "id": id, "isCategory": isCategory }, callbackMethod, "json", false);
    }

    GetUnassociatedCatalogCategory(pimCatalogId: number, catalogName: string, isCategoryTreeUpdate: boolean, callbackMethod) {
        super.ajaxRequest("/PIM/Catalog/GetUnAssociatedCategoryList", "get", { "pimCatalogId": pimCatalogId, "catalogName": catalogName, "isCategoryTreeUpdate": isCategoryTreeUpdate }, callbackMethod, "json");
    }

    DeleteGiftCard(giftCardId: string, callbackMethod) {
        super.ajaxRequest("/GiftCard/Delete", "get", { "giftCardId": giftCardId }, callbackMethod, "json");
    }

    DeleteSearchProfile(searchProfileId: string, isDeletePublishSearchProfile: boolean, callbackMethod) {
        super.ajaxRequest("/Search/Search/Delete", "get", {
            "searchProfileId": searchProfileId, "isDeletePublishSearchProfile": isDeletePublishSearchProfile }, callbackMethod, "json");
    }

    DeleteSearchTriggers(searchProfileTriggerId: string, callbackMethod) {
        super.ajaxRequest("/Search/Search/DeleteSearchTriggers", "get", { "searchProfileTriggerId": searchProfileTriggerId }, callbackMethod, "json");
    }

    DeleteMultiplePaymentSettings(paymentSettingIds: string, callbackMethod) {
        super.ajaxRequest("/Payment/Delete", "get", { "PaymentSettingId": paymentSettingIds }, callbackMethod, "json");
    }
    GetAjaxHeaders(callbackMethod: any): any {
        super.ajaxRequest("/MediaManager/MediaManager/GetAjaxHeaders", "get", {}, callbackMethod, "json", false);
    }

    GetPaymentAppHeader(callbackMethod: any): any {
        super.ajaxRequest("/Order/GetPaymentAppHeader", Constant.GET, {}, callbackMethod, "json", true);
    }

    GetCSSList(cmsThemeId: number, callbackMethod) {
        super.ajaxRequest("/Theme/Manage", "get", { "cmsThemeId": cmsThemeId }, callbackMethod, "html");
    }

    DeleteTemplate(cmsTemplateId: string, fileName: string, callbackMethod) {
        super.ajaxRequest("/Template/Delete", "get", { "cmsTemplateId": cmsTemplateId, "fileName": fileName }, callbackMethod, "json");
    }

    DeleteCss(cmsThemeCssId: string, cssName: string, themeName: string, callbackMethod) {
        super.ajaxRequest("/Theme/DeleteCSS", "get", { "cmsThemeCssId": cmsThemeCssId, "cssName": cssName, "themeName": themeName }, callbackMethod, "json");
    }

    ContentPageAddFolder(parentId: string, folderName: string, callbackMethod) {
        super.ajaxRequest("/Content/AddFolder", "get", { "parentId": parentId, "folderName": folderName }, callbackMethod, "json");
    }

    ContentPageRenameFolder(folderId: string, folderName: string, callbackMethod) {
        super.ajaxRequest("/Content/RenameFolder", "get", { "folderId": folderId, "folderName": folderName }, callbackMethod, "json");
    }

    GetProfileList(portalId: number, callbackMethod): any {
        super.ajaxRequest("/Content/GetProfileList", "get", { "portalId": portalId }, callbackMethod, "html");
    }

    ContentPageFolderDelete(folderId: string, callbackMethod) {
        super.ajaxRequest("/Content/DeleteFolder", "get", { "folderId": folderId }, callbackMethod, "json");
    }

    GetCountryList(callbackMethod) {
        super.ajaxRequest("/Shippings/GetCountryList", "get", {}, callbackMethod, "html");
    }
    AssociateLocales(Url: string, model, callbackMethod) {
        super.ajaxRequest(Url, "post", { model: model }, callbackMethod, "html");
    }
    DeleteMenu(id: string, callbackMethod) {
        super.ajaxRequest("/RoleAndAccessRight/DeleteMenu", "get", { "menuId": id }, callbackMethod, "json");
    }
    DeleteRole(id: string, callbackMethod) {
        super.ajaxRequest("/RoleAndAccessRight/DeleteRole", "get", { "id": id }, callbackMethod, "json");
    }
    DeleteERPConfigurator(eRPConfiguratorId: string, callbackMethod) {
        super.ajaxRequest("/ProviderEngine/DeleteERPConfigurator", "get", { "eRPConfiguratorId": eRPConfiguratorId }, callbackMethod, "json");
    }
    ProfileListByStorId(storeId: number, callbackMethod) {
        super.ajaxRequest("/Promotion/ProfileListByStoreId", "get", { "storeIds": storeId }, callbackMethod, "json", false);
    }
    CatalogListByStorId(storeId: number, callbackMethod) {
        super.ajaxRequest("/Promotion/CatalogListByStoreId", "get", { "storeIds": storeId }, callbackMethod, "json", false);
    }
    CategoryListByStorId(storeId: number, callbackMethod) {
        super.ajaxRequest("/Promotion/CategoryListByStoreId", "get", { "storeIds": storeId }, callbackMethod, "json", false);
    }
    ShippingStateListByCountryCode(countryCode: string, callbackMethod) {
        super.ajaxRequest("/Shippings/GetStateListByCountryCode", "get", { "countryCode": countryCode }, callbackMethod, "json", false);
    }
    ShippingCityListByStateCode(stateCode: number, callbackMethod) {
        super.ajaxRequest("/Shippings/GetCityListByStateCode", "get", { "stateCode": stateCode }, callbackMethod, "json", false);
    }
    CreateNewOrderByPortalIdChange(portalId: number, callbackMethod) {
        super.ajaxRequest("/Order/CreateOrder", "get", { "portalId": portalId }, callbackMethod, "html");
    }
    CreateNewOrderByPortalIdChangeForUser(portalId: number, userId: number, callbackMethod) {
        super.ajaxRequest("/Order/CreateOrder", "get", { "portalId": portalId, "userId": userId }, callbackMethod, "html");
    }
    GetConvertedDecimalValues(price: number, currencyCode: string, callbackMethod) {
        super.ajaxRequest("/RmaManager/GetConvertedCurrencyValues", "post", { "decimalValue": price, "currencyCode": currencyCode }, callbackMethod, "json");
    }

    createRMARequest(requestModel: any, callbackMethod) {
        super.ajaxRequest("/RMAManager/CreateRMA", "post", { "requestModel": requestModel }, callbackMethod, "json");
    }

    GetCustomerList(portalId: number, callbackMethod) {
        super.ajaxRequest("/Order/GetCustomerList", "get", { "portalId": portalId }, callbackMethod, "html");
    }
    GetDomains(portalId: number, doaminIds: string, userId: number, callbackMethod) {
        super.ajaxRequest("/Customer/GetDomains", "get", { "portalId": portalId, "domainIds": doaminIds, "userId": userId }, callbackMethod, "html");
    }

    SetCustomerDetailsById(userId: string, callbackMethod) {
        super.ajaxRequest("/Order/SetCustomerDetailsById", "get", { "userId": userId }, callbackMethod, "json");
    }

    GetCreateLinkWidgetForm(cmsWidgetsId: number, cmsMappingId: number, widgetKey: string, typeOFMapping: string, displayName: string, widgetName: string, fileName: string, callbackMethod) {
        super.ajaxRequest("/WebSite/CreateLinkWidget", "get", { "cmsWidgetsId": cmsWidgetsId, "cmsMappingId": cmsMappingId, "widgetsKey": widgetKey, "typeOFMapping": typeOFMapping, "displayName": displayName, "widgetName": widgetName, "fileName": fileName }, callbackMethod, "html");
    }

    GetNewCustomerView(portalId: number, callbackMethod) {
        super.ajaxRequest("/Order/AddNewCustomer", "get", { "portalId": portalId }, callbackMethod, "html");
    }

    ManageTextWidgetConfiguration(mappingId, widgetId, widgetKey, mappingType, displayName, widgetName, fileName, localeId: number, callbackMethod) {
        super.ajaxRequest("/WebSite/ManageTextWidgetConfiguration", "get", { "mappingId": mappingId, "widgetId": widgetId, "widgetKey": widgetKey, "mappingType": mappingType, "displayName": displayName, "widgetName": widgetName, "fileName": fileName, "localeId": localeId }, callbackMethod, "html");
    }
    DeleteERPTaskScheduler(erpTaskSchedulerIds: string, callbackMethod) {
        super.ajaxRequest("/TouchPointConfiguration/Delete", "get", { "erpTaskSchedulerId": erpTaskSchedulerIds }, callbackMethod, "json");
    }

    EditBanner(CMSSliderBannerId, localeId: number, callbackMethod) {
        super.ajaxRequest("/WebSite/EditBanner", "get", { "CMSSliderBannerId": CMSSliderBannerId, "localeId": localeId }, callbackMethod, "html");
    }

    UpdateManageMessage(cmsMessageKeyId, cmsAreaId, portalId, localeId, callbackMethod) {
        super.ajaxRequest("/Content/UpdateManageMessage", "get", { "cmsMessageKeyId": cmsMessageKeyId, "cmsAreaId": cmsAreaId, "portalId": portalId, "localeId": localeId }, callbackMethod, "html");
    }
    AddTaxClassSKUList(taxClassId: number, SKUs: string, callbackMethod) {
        super.ajaxRequest("/TaxClass/AddTaxClassSKU", "get", { "taxClassId": taxClassId, "taxClassSKUs": SKUs }, callbackMethod, "json");
    }

    GetSchedulerFrequency(url: string, callbackMethod) {
        super.ajaxRequest(url, "get", {}, callbackMethod, "html");
    }

    GetCustomerListByName(customerName: string, portalId: number, isAccountCustomer: boolean, accountId: any, callbackMethod) {
        super.ajaxRequest("/Order/GetCustomerListByName", "get", { "customerName": customerName, "portalId": portalId, "isAccountCustomer": isAccountCustomer, "accountId": accountId }, callbackMethod, "json");
    }

    GetUserAddressBySelectedAddress(addressId: number, fromBillingShipping: string, isB2BCustomer: boolean, userId: number, portalId: number, accountId: number, callbackMethod) {
        super.ajaxRequest("/Order/GetAddressById", "get", { "userAddressId": addressId, "fromBillingShipping": fromBillingShipping, "isB2BCustomer": isB2BCustomer, "userId": userId, "portalId": portalId, "accountId": accountId }, callbackMethod, "json");
    }

    GetPartial(url: string, callbackMethod) {
        super.ajaxRequest(url, "get", {}, callbackMethod, "html");
    }

    EditEmailTemplate(EmailTemplateId, localeId: number, callbackMethod) {
        super.ajaxRequest("/EmailTemplate/Edit", "get", { "EmailTemplateId": EmailTemplateId, "localeId": localeId }, callbackMethod, "html");
    }

    AssociateCategoriesToCatalog(catalogAssociation: Object, callbackMethod) {
        super.ajaxRequest("/PIM/Catalog/AssociateCategory", "post", { "catalogAssociationViewModel": catalogAssociation }, callbackMethod, "json");
    }

    AddUpdateProductInventory(inventorySKUViewModel: Object, callbackMethod) {
        super.ajaxRequest("/Inventory/AddUpdateSKUInventoryProduct", "post", { "InventorySKUViewModel": inventorySKUViewModel }, callbackMethod, "json");
    }

    AssociateProductsToCatalogCategory(productAssociationModel: Object, callbackMethod) {
        super.ajaxRequest("/PIM/Catalog/AssociateProduct", "post", { "catalogAssociationViewModel": productAssociationModel }, callbackMethod, "json");
    }

    DeleteAssociateProducts(productUnassociationModel: Object, callbackMethod) {
        super.ajaxRequest("/PIM/Catalog/DeleteAssociateProducts", "post", { "catalogAssociationViewModel": productUnassociationModel }, callbackMethod, "json");
    }

    DeleteEmailTemplates(emailTemplateId: string, callbackMethod) {
        super.ajaxRequest("/EmailTemplate/Delete", "get", { "emailtemplateId": emailTemplateId }, callbackMethod, "json");
    }

    DeleteProductFeed(productFeedId: string, callbackMethod) {
        super.ajaxRequest("/ProductFeed/Delete", "get", { "productFeedId": productFeedId }, callbackMethod, "json");
    }

    EmailTemplatePreview(emailTemplateId: number, callbackMethod) {
        super.ajaxRequest("/EmailTemplate/Preview", "get", { "emailTemplateId": emailTemplateId }, callbackMethod, "html");
    }

    SingleResetPassword(id: Number, callbackMethod) {
        super.ajaxRequest("/User/SingleResetPassword", "get", { "userId": id }, callbackMethod, "json");
    }

    GetProductsList(portalCatalogId: number, portalId: number, callbackMethod) {
        super.ajaxRequest("/Order/ProductList", "get", { "portalCatalogId": portalCatalogId, "portalId": portalId }, callbackMethod, "html");
    }

    CalculateShippingCharges(data: Object, callbackMethod) {
        super.ajaxRequest("/Order/CalculateShippingCharges", "post", { "createOrderModel": data }, callbackMethod, "html");
    }

    CreateReasonForReturn(callbackMethod) {
        super.ajaxRequest("/RMAConfiguration/CreateReasonForReturn", "get", "", callbackMethod, "html");
    }

    EditReasonForReturn(rmaReasonForReturnId: string, callbackMethod) {
        super.ajaxRequest("/RMAConfiguration/EditReasonForReturn", "get", { "rmaReasonForReturnId": rmaReasonForReturnId }, callbackMethod, "html");
    }

    DeleteReasonForReturn(rmaReasonForReturnId: string, callbackMethod) {
        super.ajaxRequest("/RMAConfiguration/DeleteReasonForReturn", "get", { "rmaReasonForReturnId": rmaReasonForReturnId }, callbackMethod, "json");
    }

    GetReasonForReturnList(callbackMethod) {
        super.ajaxRequest("/RMAConfiguration/GetReasonForReturnList", "get", "", callbackMethod, "html");
    }

    EditRequestStatus(rmaRequestStatusId: string, callbackMethod) {
        super.ajaxRequest("/RMAConfiguration/EditRequestStatus", "get", { "rmaRequestStatusId": rmaRequestStatusId }, callbackMethod, "html");
    }

    GetRequestStatusList(callbackMethod) {
        super.ajaxRequest("/RMAConfiguration/GetRequestStatusList", "get", "", callbackMethod, "html");
    }

    DeleteRequestStatus(rmaRequestStatusId: string, callbackMethod) {
        super.ajaxRequest("/RMAConfiguration/DeleteRequestStatus", "get", { "rmaRequestStatusId": rmaRequestStatusId }, callbackMethod, "json");
    }

    EditHighlight(HighlightId, localeId: number, callbackMethod) {
        super.ajaxRequest("/Highlight/Edit", "get", { "HighlightId": HighlightId, "localeId": localeId }, callbackMethod, "html");
    }

    EditAssociatedPriceListPrecedence(priceListProfileId: number, priceListId: number, portalId: number, listName: string, callbackMethod) {
        super.ajaxRequest("/Store/EditAssociatedPriceListPrecedence", "get", { "priceListProfileId": priceListProfileId, "priceListId": priceListId, "portalId": portalId, "listName": listName }, callbackMethod, "json");
    }

    AddToCart(cartItem: any, callbackMethod) {
        super.ajaxRequest("/Order/AddToCart", "post", { "cartItem": cartItem }, callbackMethod, "html");
    }

    DeleteEmailTemplateAreaMapping(id: string, callbackMethod) {
        super.ajaxRequest("/EmailTemplate/DeleteEmailTemplateAreaMapping", "get", { "areaMappingId": id }, callbackMethod, "json");
    }

    GetAvailableTemplateArea(portalId: number, callbackMethod) {
        super.ajaxRequest("/EmailTemplate/GetAvailableTemplateArea", "get", { "portalId": portalId }, callbackMethod, "json");
    }
    GetEmailTemplateListByName(searchTerm: string, callbackMethod) {
        super.ajaxRequest("/EmailTemplate/GetEmailTemplateListByName", "get", { "searchTerm": searchTerm }, callbackMethod, "json");
    }

    SaveBoostValues(boostData: Object, callbackMethod) {
        super.ajaxRequest("/SearchConfiguration/SaveBoostValue", "post", { "boostData": boostData }, callbackMethod, "json");
    }

    DeleteMultipleHighlight(highlightIds: string, callbackMethod) {
        super.ajaxRequest("/Highlight/Delete", "get", { "highlightId": highlightIds }, callbackMethod, "json");
    }

    EditAssociatedPriceListPrecedenceForAccount(priceListId: number, accountId: number, listName: string, callbackMethod) {
        super.ajaxRequest("/Account/EditAssociatedPriceListPrecedence", "get", { "priceListId": priceListId, "accountId": accountId, "listName": listName }, callbackMethod, "html");
    }

    AssociatePriceListForAccount(accountId: number, priceListId: string, callbackMethod) {
        super.ajaxRequest("/Account/AssociatePriceListToAccount", "post", { "accountId": accountId, "priceListId": priceListId }, callbackMethod, "json");
    }

    AssociateProfileForAccount(accountId: number, profileId: string, callbackMethod) {
        super.ajaxRequest("/Account/AssociateProfileToAccount", "post", { "accountId": accountId, "profileIds": profileId }, callbackMethod, "json");
    }

    GetProductSKUList(callbackMethod) {
        super.ajaxRequest("/Price/GetProductSKUList", "get", {}, callbackMethod, "html");
    }

    UnAssociatePriceListFromCustomer(priceListId: string, userId: number, callbackMethod) {
        super.ajaxRequest("/Customer/UnAssociatePriceListToCustomer", "get", { "priceListId": priceListId, "userId": userId }, callbackMethod, "json");
    }

    EditAssociatedPriceListPrecedenceForCustomer(priceListId: number, userId: number, listName: string, callbackMethod) {
        super.ajaxRequest("/Customer/EditAssociatedPriceListPrecedence", "get", { "priceListId": priceListId, "userId": userId, "listName": listName }, callbackMethod, "json");
    }

    GetAssociatedPriceListForCustomer(userId: number, callbackMethod) {
        super.ajaxRequest("/Customer/GetAssociatedPriceListForCustomer", "get", { "userId": userId }, callbackMethod, "html");
    }

    AssociatePriceListForCustomer(userId: number, priceListId: string, callbackMethod) {
        super.ajaxRequest("/Customer/AssociatePriceListToCustomer", "post", { "userId": userId, "priceListId": priceListId }, callbackMethod, "json");
    }

    AssociateHighlightProductList(highlightCode: string, productIds: string, callbackMethod) {
        super.ajaxRequest("/Highlight/AssociateHighlightProducts", "get", { "highlightCode": highlightCode, "productIds": productIds }, callbackMethod, "json", false);
    }

    UnAssociateHighlightProduct(productId: string, highlightcode: string, callbackMethod) {
        super.ajaxRequest("/Highlight/UnAssociateHighlightProducts", "get", { "PimProductId": productId, "attributevalue": highlightcode }, callbackMethod, "json", false);
    }

    BindCustomerDetails(cartParameter: any, callbackMethod) {
        super.ajaxRequest("/Order/BindCustomerDetails", "post", { "cartParameter": cartParameter }, callbackMethod, "html");
    }

    UpdateCartQuantity(guid: string, quantity: any, productid: any, shippingid: any, isQuote: boolean, userId: any, callbackMethod) {
        super.ajaxRequest("/Order/UpdateCartQuantity", "post", { "guid": guid, "quantity": quantity, "productId": productid, "shippingId": shippingid, "isQuote": isQuote, "userId": userId }, callbackMethod, "json", true, false);
    }

    DeleteCartItem(guid: string, orderId: number, isQuote: boolean, userId: number, portalId: number, callbackMethod) {
        super.ajaxRequest("/Order/RemoveCartItem", "post", { "guid": guid, "orderId": orderId, "isQuote": isQuote, "userId": userId, "portalId": portalId }, callbackMethod, "json", true, false);
    }

    DeleteAllCartItem(userId: number, portalId: number, isQuote: boolean, callbackMethod) {
        super.ajaxRequest("/Order/RemoveAllShoppingCartItems", "post", {
            "userId": userId, "portalId": portalId, "isQuote": isQuote}, callbackMethod, "json", true, false);
    }

    GetPortalSeoSettings(portalId: number, callbackMethod) {
        super.ajaxRequest("/SEO/SEOSetting", "get", { "portalId": portalId }, callbackMethod, "html");
    }

    GetProductPrice(portalId: number, sku: string, parentProductSKU: string, quantity: any, selectedAddOnIds: any, parentProductId: number, omsOrderId: number = 0, userId: number = 0, callbackMethod) {
        super.ajaxRequest("/Order/GetProductPrice", "get", {
            "portalId": portalId, "productSKU": sku, "parentProductSKU": parentProductSKU, "quantity": quantity, "addOnIds": selectedAddOnIds, "parentProductId": parentProductId, "omsOrderId": omsOrderId, "userId": userId
        }, callbackMethod, "json", false);
    }
    GetAddLinkWidgetConfiguration(cmsWidgetsId: number, cmsMappingId: number, widgetKey: string, typeOFMapping: string, displayName: string, widgetName: string, fileName: string, localeId: number, callbackMethod) {
        super.ajaxRequest("/WebSite/AddNewLinkWidgetConfiguration", "get", { "cmsWidgetsId": cmsWidgetsId, "cmsMappingId": cmsMappingId, "widgetsKey": widgetKey, "typeOFMapping": typeOFMapping, "localeId": localeId, "displayName": displayName, "widgetName": widgetName, "fileName": fileName }, callbackMethod, "json");
    }

    GetProduct(parameters, callbackMethod) {
        super.ajaxRequest("/Order/GetConfigurableProduct", "post", { "parameters": parameters }, callbackMethod, "html");
    }
    GetPaymentDetails(paymentSettingId, userId: number, callbackMethod) {
        super.ajaxRequest("/Order/GetPaymentDetails", "get", { "paymentSettingId": paymentSettingId, "userId": userId }, callbackMethod, "json", false);
    }

    GetPublishProductsList(portalCatalogId, portalId, userId, callbackMethod) {
        super.ajaxRequest("/Order/ProductList", "get", { "portalCatalogId": portalCatalogId, "portalId": portalId, "userId": userId }, callbackMethod, "html", true, false);
    }
    GetEncryptedAmountByAmount(total, callbackMethod) {
        super.ajaxRequest("/Order/GetEncryptedAmount", "get", { "amount": total }, callbackMethod, "json", false);
    }
    GetInventoryDetails(productSKU, productId, isDownlodableProduct, callbackMethod) {
        super.ajaxRequest("/Inventory/ProductInventory", "get", {
            "SKU": productSKU,
            "productId": productId,
            "isDownloadable": isDownlodableProduct
        }, callbackMethod, "html");
    }

    GetDownloadableProductKeys(productSKU, productId, inventoryId, callbackMethod) {
        super.ajaxRequest("/Inventory/GetDownloadableProductKeys", "get", {
            "sku": productSKU,
            "productId": productId,
            "inventoryId": inventoryId
        }, callbackMethod, "html");
    }

    GetAssociatedProductCatagoryList(isAssociated, productId, callbackMethod) {
        super.ajaxRequest("/Category/GetAssociatedCategoriesToProduct", "get", {
            "isAssociateCategories": isAssociated,
            "productId": productId
        }, callbackMethod, "html");
    }

    GetProductSEODetails(seoTypeId, seoCode, publishProductId, localeId, portalId, callbackMethod) {
        super.ajaxRequest("/SEO/GetSEODetailsBySEOCode", "get", {
            "seoTypeId": seoTypeId,
            "localeId": localeId,
            "portalId": portalId,
            "seoCode": seoCode,
        }, callbackMethod, "html");
    }

    GetCategorySEODetails(seoTypeId, seoCode, categoryId, localeId, portalId, callbackMethod) {
        super.ajaxRequest("/SEO/CategorySEODetails", "get", {
            "seoTypeId": seoTypeId,
            "itemId": categoryId,
            "localeId": localeId,
            "portalId": portalId,
            "seoCode": seoCode
        }, callbackMethod, "html");
    }
    GetDefaultSEODetails(seoTypeId, seoCode, itemId, localeId, portalId, callbackMethod) {
        super.ajaxRequest("/SEO/GetDefaultSEODetails", "get", {
            "seoTypeId": seoTypeId,
            "localeId": localeId,
            "portalId": portalId,
            "seoCode": seoCode,
            "itemId": itemId
        }, callbackMethod, "html");
    }
    GetDefaultCMSSEODetails(seoTypeId, seoCode, itemId, localeId, portalId, callbackMethod) {
        super.ajaxRequest("/Content/GetDefaultCMSSEODetails", "get", {
            "seoTypeId": seoTypeId,
            "localeId": localeId,
            "portalId": portalId,
            "seoCode": seoCode,
            "itemId": itemId
        }, callbackMethod, "html");
    }
    GetProductPriceDetails(pimProductId, sku, productType, productpriceListId, callbackMethod) {
        super.ajaxRequest("/Products/GetProductPriceBySku", "get", { "pimProductId": pimProductId, "sku": sku, "productType": productType, "productpriceListId": productpriceListId }, callbackMethod, "html");
    }

    GetInventoryBySKU(sku, warehouseId, callbackMethod) {
        super.ajaxRequest("/Inventory/InventoryBySKUAndWarehouseId", "get", { "sku": sku, "warehouseId": warehouseId }, callbackMethod, "json");
    }

    DeleteTierPriceByIdAndPriceList(priceTierId, pimProductId, priceListId, callbackMethod) {
        super.ajaxRequest("/Products/DeleteTierPrice", "get", { "priceTierId": priceTierId, "pimProductId": pimProductId, "priceListId": priceListId }, callbackMethod, "json");
    }

    DeleteBrand(id: string, callbackMethod) {
        super.ajaxRequest("/PIM/Brand/Delete", "get", { "brandId": id }, callbackMethod, "json");
    }

    SEODetails(itemName, seoTypeId, seoCode, localeId, portalId: number, callbackMethod) {
        super.ajaxRequest("/SEO/SEODetails", "get", { "itemName": itemName, "seoTypeId": seoTypeId, "seoCode": seoCode, "localeId": localeId, "portalId": portalId }, callbackMethod, "html");
    }

    GetPublishedProductList(portalId: number, callbackMethod) {
        super.ajaxRequest("/SEO/GetProductsForSEO", "get", { "portalId": portalId }, callbackMethod, "html");
    }

    GetActiveCurrencyToStore(portalId: number, callbackMethod) {
        super.ajaxRequest("/GiftCard/GetActiveCurrencyToStore", "get", { "portalId": portalId }, callbackMethod, "json");
    }

    GetCurrencyDetailsByCode(currencyCode: string, callbackMethod) {
        super.ajaxRequest("/GiftCard/GetCurrencyDetailsByCode", "get", { "currencyCode": currencyCode }, callbackMethod, "json");
    }

    GetPublishedCategoryList(portalId: number, callbackMethod) {
        super.ajaxRequest("/SEO/GetCategoriesForSEO", "get", { "portalId": portalId }, callbackMethod, "html");
    }

    GetContentPagesList(portalId: number, callbackMethod) {
        super.ajaxRequest("/SEO/GetContentPages", "get", { "portalId": portalId }, callbackMethod, "html");
    }

    UrlRedirectList(portalId: number, callbackMethod) {
        super.ajaxRequest("/SEO/UrlRedirectList", "get", { "portalId": portalId }, callbackMethod, "html");
    }

    GetReportData(reportPath: string, callbackMethod) {
        super.ajaxRequest("/MyReports/GetReportDetail", "get", { "reportPath": reportPath }, callbackMethod, "json");
    }
    DeleteVendor(id: string, callbackMethod) {
        super.ajaxRequest("/PIM/Vendor/Delete", "get", { "PimVendorId": id }, callbackMethod, "json");
    }

    BrandAssociatePortalList(brandId: number, portalIds: string, callbackMethod) {
        super.ajaxRequest("/Brand/AssociateBrandPortals", "get", { "brandId": brandId, "portalIds": portalIds }, callbackMethod, "json", false);
    }

    BrandAssociateProductList(brandCode: string, productIds: string, callbackMethod) {
        super.ajaxRequest("/Brand/AssociateBrandProducts", "get", { "brandCode": brandCode, "productIds": productIds }, callbackMethod, "json", false);
    }

    BrandUnAssociateProductList(productIds: string, brandCode: string, callbackMethod) {
        super.ajaxRequest("/Brand/UnAssociateBrandProducts", "get", { "pimProductId": productIds, "attributeValue": brandCode }, callbackMethod, "json", false);
    }

    BrandUnAssociatePortalList(portalId: string, brandId: number, callbackMethod) {
        super.ajaxRequest("/Brand/UnAssociateBrandPortals", "get", { "portalId": portalId, "brandId": brandId }, callbackMethod, "json", false);
    }

    AssociatedBrandPortalList(brandId: number, brandCode: string, brandName: string, localeId: number, callbackMethod) {
        super.ajaxRequest("/Brand/AssociatedStoreList", "get", { "brandId": brandId, "localeId": localeId, "brandCode": brandCode, "brandName": brandName }, callbackMethod, "json");
    }


    GetAccountsPortal(accountId: number, callbackMethod) {
        super.ajaxRequest("/Account/GetAccountsPortal", "get", { "accountId": accountId }, callbackMethod, "json", false);
    }
    VendorAssociatedProductList(vendorCode: string, vendorName: string, productIds: string, callbackMethod) {
        super.ajaxRequest("/Vendor/AssociateVendorProducts", "get", { "vendorCode": vendorCode, "vendorName": vendorName, "productIds": productIds }, callbackMethod, "json", false);
    }

    AssociatedProductList(PimVendorId: number, vendorCode: string, vendorName: string, callbackMethod) {
        super.ajaxRequest("/PIM/Vendor/AssociatedProductList", "get", { "PimVendorId": PimVendorId, "vendorCode": vendorCode, "vendorName": vendorName }, callbackMethod, "html");
    }

    VendorUnAssociateProductList(productIds: string, vendorCode: string, callbackMethod) {
        super.ajaxRequest("/Vendor/UnAssociateVendorProducts", "get", { "pimProductId": productIds, "attributeValue": vendorCode }, callbackMethod, "json", false);
    }

    ActiveInactiveVendor(vendorIds: string, isActive: boolean, callBackMethod) {
        super.ajaxRequest("/PIM/Vendor/ActiveInactiveVendor", "get", { "vendorIds": vendorIds, "isActive": isActive }, callBackMethod, "json");

    }

    CreateUrlRedirect(portalId: number, callbackMethod) {
        super.ajaxRequest("/SEO/CreateUrlRedirect", "get", { "portalId": portalId }, callbackMethod, "html", false);
    }

    CheckGroupProductInventory(parameters, sku, quantity, callbackMethod) {
        super.ajaxRequest("/Order/CheckGroupProductInventory", "post", { "parameters": parameters, "productSKU": sku, "quantity": quantity }, callbackMethod, "json", false);
    }

    ManageEmailTemplateArea(portalId: number, callbackMethod) {
        super.ajaxRequest("/EmailTemplate/ManageEmailTemplateArea", "get", { "portalId": portalId }, callbackMethod, "html", false);
    }

    GetSearchConfiguration(publishCatalogId: number, catalogName: string, callbackMethod) {
        super.ajaxRequest("/SearchConfiguration/CreateIndex", "get", { "publishCatalogId": publishCatalogId, "catalogName": catalogName }, callbackMethod, "html");
    }

    GetSearchProfiles(callbackMethod) {
        super.ajaxRequest("/Search/Search/GetSearchProfiles", "get", {}, callbackMethod, "html");
    }

    GetSearchProfilesByCatalogId(publishCatalogId: string, catalogName: string, callbackMethod) {
        super.ajaxRequest("/Search/Search/GetSearchProfiles", "get", { "catalogId": publishCatalogId, "catalogName": catalogName }, callbackMethod, "html");
    }

    GetAssociatedCatalogAttributes(callbackMethod) {
        super.ajaxRequest("/Search/Search/GetAssociatedCatalogAttributes", "get", { "publishCatalogId": 0, "searchProfileId": 0 }, callbackMethod, "html");
    }
    GetAssociatedUnAssociatedCatalogAttributes(callbackMethod) {
        super.ajaxRequest("/Search/Search/GetAssociatedUnAssociatedCatalogAttributes", "get", { "publishCatalogId": 0, "searchProfileId": 0, "isAssociated": true }, callbackMethod, "html");
    }

    GetPromotionDiscountAttribute(discountName: string, callbackMethod) {
        super.ajaxRequest("/Promotion/GetPromotionDiscountAttribute", "get", { "discountName": discountName }, callbackMethod, "html");
    }

    InsertCreateIndexData(portalIndex: any, callbackMethod) {
        super.ajaxRequest("/SearchConfiguration/InsertCreateIndexData", "post", { "portalIndexModel": portalIndex }, callbackMethod, "html", false);
    }

    GetSearchIndexMonitorList(catalogIndexId: number, callbackMethod) {
        super.ajaxRequest("/SearchConfiguration/GetSearchIndexMonitor", "get", { "catalogIndexId": catalogIndexId }, callbackMethod, "html", false);
    }

    GetSearchIndexServerStatusList(searchIndexMonitorId: number, callbackMethod) {
        super.ajaxRequest("/SearchConfiguration/GetSearchIndexServerStatusList", "get", { "searchIndexMonitorId": searchIndexMonitorId }, callbackMethod, "html");
    }

    GetAssociatedCatelog(storeId: number, catelogIds: string, promotionId: number, callbackMethod) {
        super.ajaxRequest("/Promotion/GetAssociatedCatelog", "get", { "storeId": storeId, "catelogIds": catelogIds, "promotionId": promotionId }, callbackMethod, "html");
    }

    GetAssociatedCategory(storeId: number, categoryIds: string, promotionId: number, callbackMethod) {
        super.ajaxRequest("/Promotion/GetAssociatedCategory", "get", { "storeId": storeId, "categoryIds": categoryIds, "promotionId": promotionId }, callbackMethod, "html");
    }

    GetAssociatedProduct(storeId: number, productIds: string, promotionId: number, callbackMethod) {
        super.ajaxRequest("/Promotion/GetAssociatedProduct", "get", { "storeId": storeId, "productIds": productIds, "promotionId": promotionId }, callbackMethod, "html");
    }

    EnableDisableAdminAPIDomain(domainIds: string, isEnable: boolean, callbackMethod) {
        super.ajaxRequest("/UrlManagement/EnableDisableDomain", "get", { "DomainId": domainIds, "IsActive": isEnable }, callbackMethod, "html");
    }

    AssociateCatalogToPromotion(storeId: number, associatedCatelogIds: string, promotionId: number, callbackMethod) {
        super.ajaxRequest("/Promotion/AssociateCatalogToPromotion", "get", { "storeId": storeId, "associatedCatelogIds": associatedCatelogIds, "promotionId": promotionId }, callbackMethod, "json");
    }

    GetAssociatedBrands(brandIds: string, promotionId: number, callbackMethod) {
        super.ajaxRequest("/Promotion/GetAssociatedBrands", "get", { "assignedIds": brandIds, "promotionId": promotionId }, callbackMethod, "html");
    }

    GetProductBoostSetting(catalogId: number, callbackMethod) {
        super.ajaxRequest("/SearchConfiguration/GetGlobalProductBoost", "get", { "catalogId": catalogId }, callbackMethod, "html");
    }

    GetProductCategoryBoostSetting(catalogId: number, callbackMethod) {
        super.ajaxRequest("/SearchConfiguration/GetGlobalProductCategoryBoost", "get", { "catalogId": catalogId }, callbackMethod, "html");
    }

    GetFieldBoostSetting(catalogId: number, callbackMethod) {
        super.ajaxRequest("/SearchConfiguration/GetFieldLevelBoost", "get", { "catalogId": catalogId }, callbackMethod, "html");
    }

    AssociateCategoryToPromotion(storeId: number, associatedCategoryIds: string, promotionId: number, callbackMethod) {
        super.ajaxRequest("/Promotion/AssociateCategoryToPromotion", "get", { "storeId": storeId, "associatedCategoryIds": associatedCategoryIds, "promotionId": promotionId }, callbackMethod, "json");
    }

    CustomerEnableDisableAccount(accountid: number, id: string, isEnable: boolean, callbackMethod) {
        super.ajaxRequest("/Account/CustomerEnableDisableAccount", "get", { "accountId": accountid, "userId": id, "isLock": isEnable, "isRedirect": false }, callbackMethod, "json");
    }

    EnableDisableDomain(portalId: number, domainIds: string, isEnable: boolean, callbackMethod) {
        super.ajaxRequest("/Store/EnableDisableDomain", "get", { "PortalId": portalId, "DomainId": domainIds, "IsActive": isEnable }, callbackMethod, "html");
    }
    CustomerAccountResetPassword(accountid: number, id: string, callbackMethod) {
        super.ajaxRequest("/Account/BulkResetPassword", "get", { "accountid": accountid, "userId": id }, callbackMethod, "json");
    }

    useCouponCode(url: string, couponCode: string, callbackMethod: any) {
        super.ajaxRequest(url, "get", { "Coupon": couponCode }, callbackMethod, "json", true, false);
    }

    AssociateProductToPromotion(storeId: number, associatedProductIds: string, promotionId: number, discountTypeName: string, callbackMethod) {
        super.ajaxRequest("/Promotion/AssociateProductToPromotion", "get", { "storeId": storeId, "associatedProductIds": associatedProductIds, "promotionId": promotionId, "discountTypeName": discountTypeName }, callbackMethod, "json");
    }

    AssociateBrandToPromotion(associatedBrandIds: string, promotionId: number, callbackMethod) {
        super.ajaxRequest("/Promotion/AssociateBrandToPromotion", "get", { "associatedBrandIds": associatedBrandIds, "promotionId": promotionId }, callbackMethod, "json");
    }

    removeCouponCode(url: string, couponCode: string, callbackMethod: any) {
        super.ajaxRequest(url, "get", { "Coupon": couponCode }, callbackMethod, "json", true, false);
    }

    applyGiftCard(url: string, giftCard: string, userId: number, callbackMethod: any) {
        super.ajaxRequest(url, "get", { "giftCardNumber": giftCard, "userId": userId }, callbackMethod, "html", true, false);
    }
    GetTemplateList(importHeadId: number, familyId: number, promotionTypeId: number, callbackMethod) {
        super.ajaxRequest("/Import/BindTemplateList", "post", { "importHeadId": importHeadId, "familyId": familyId, "promotionTypeId": promotionTypeId }, callbackMethod, "json");
    }

    GetTemplateMappings(templateId: number, importHeadId: number, familyId: number, promotionTypeId: number, callbackMethod) {
        super.ajaxRequest("/Import/GetAssociatedTemplateList", "post", { "templateId": templateId, "importHeadId": importHeadId, "familyId": familyId, "promotionTypeId": promotionTypeId }, callbackMethod, "json");
    }

    GetContentPage(CMSContentPagesId, localeId: number, callbackMethod) {
        super.ajaxRequest("/Content/EditContentPage", "get", { "cmsContentPagesId": CMSContentPagesId, "localeId": localeId }, callbackMethod, "html");
    }

    PublishContentPage(cmsContentPagesId: string, targetPublishState: string, localeId: number, callbackMethod) {
        super.ajaxRequest("/Content/PublishContentPageWithPreview", "post", { "cmsContentPagesId": cmsContentPagesId, "localeId": localeId, "targetPublishState": targetPublishState, "takeFromDraftFirst": true }, callbackMethod, "json");
    }

    ShowLogStatus(importProcessLogId: number, callbackMethod) {
        super.ajaxRequest("/Import/ShowLogStatus", "get", { "importProcessLogId": importProcessLogId }, callbackMethod, "html");
    }

    ShowLogDetails(importProcessLogId: number, callbackMethod) {
        super.ajaxRequest("/Import/ShowLogDetails", "get", { "importProcessLogId": importProcessLogId }, callbackMethod, "html");
    }

    IsAttributeDefaultValueCodeExist(attributeId: number, attributeDefaultValueCode: string, defaultvalueId: number, callbackMethod) {
        super.ajaxRequest("/PIM/ProductAttribute/IsAttributeDefaultValueCodeExist", "get", { "attributeId": attributeId, "attributeDefaultValueCode": attributeDefaultValueCode, "defaultValueId": defaultvalueId }, callbackMethod, "json", false);
    }
    DeleteAssociatedPromotionProducts(associatedProductIds: string, promotionId: number, callbackMethod) {
        super.ajaxRequest("/Promotion/UnAssociateProducts", "get", { "publishProductId": associatedProductIds, "promotionId": promotionId }, callbackMethod, "json");
    }

    DeleteAssociatedPromotionBrands(associatedBrandIds: string, promotionId: number, callbackMethod) {
        super.ajaxRequest("/Promotion/UnAssociateBrands", "get", { "BrandId": associatedBrandIds, "promotionId": promotionId }, callbackMethod, "json");
    }

    UpdateQuoteStatus(quoteId: string, status: number, isPendingPaymentStatus: boolean, callbackMethod) {
        super.ajaxRequest("/Account/UpdateQuoteStatus", "get", { "quoteId": quoteId, "status": status, "isPendingPaymentStatus": isPendingPaymentStatus }, callbackMethod, "json");
    }

    DeclinePendingPayment(quoteId: string, status: number, isPendingPaymentStatus: boolean, orderStatus: string, callbackMethod) {
        super.ajaxRequest("/Account/UpdateQuoteStatus", "get", { "quoteId": quoteId, "status": status, "isPendingPaymentStatus": isPendingPaymentStatus, 'orderStatus': orderStatus }, callbackMethod, "json");
    }

    DeleteAssociatedPromotionCategorys(associatedCategoryIds: string, promotionId: number, callbackMethod) {
        super.ajaxRequest("/Promotion/UnAssociateCategories", "get", { "publishCategoryId": associatedCategoryIds, "promotionId": promotionId }, callbackMethod, "json");
    }

    DeleteAssociatedPromotionCatalogs(associatedCatalogIds: string, promotionId: number, callbackMethod) {
        super.ajaxRequest("/Promotion/UnAssociateCatalogs", "get", { "publishCatalogId": associatedCatalogIds, "promotionId": promotionId }, callbackMethod, "json");
    }

    DeleteAssociatedProfileCatalog(profileId: string, callbackMethod) {
        super.ajaxRequest("/Profiles/DeleteAssociatedProfileCatalog", "get", { "profileId": profileId }, callbackMethod, "json");
    }

    UnAssociateAssociatedShipping(shippingId: string, profileId: number, callbackMethod) {
        super.ajaxRequest("/Profiles/UnAssociateAssociatedShipping", "get", { "shippingId": shippingId, "profileId": profileId }, callbackMethod, "json");
    }

    UnAssociateAssociatedShippingToStore(shippingId: string, portalId: number, callbackMethod) {
        super.ajaxRequest("/Store/UnAssociateAssociatedShipping", "get", { "shippingId": shippingId, "portalId": portalId }, callbackMethod, "json");
    }

    ApplyCSRDiscount(csrDiscount: number, csrDesc: string, userId: number, callbackMethod: any) {
        super.ajaxRequest("/Order/ApplyCSRDiscount", "get", { "CSRDiscount": csrDiscount, "csrDesc": csrDesc, "userId": userId }, callbackMethod, "html", true, false);
    }

    AssociateCatalogToProfile(profileId: number, pimCatalogId: string, callbackMethod) {
        super.ajaxRequest("/Profiles/AssociateCatalogToProfile", "get", { "profileId": profileId, "pimCatalogId": pimCatalogId }, callbackMethod, "json");
    }

    AssociateShipping(profileId: number, shippingIds: string, callbackMethod) {
        super.ajaxRequest("/Profiles/AssociateShipping", "get", { "profileId": profileId, "shippingIds": shippingIds }, callbackMethod, "json");
    }

    AssociateShippingToStore(portalId: number, shippingIds: string, callbackMethod) {
        super.ajaxRequest("/Store/AssociateShipping", "get", { "portalId": portalId, "shippingIds": shippingIds }, callbackMethod, "json");
    }

    GetAssociatedShippingList(profileId: number, portalId: number, callbackMethod) {
        super.ajaxRequest("/Profiles/GetAssociatedShippingList", "get", { "profileId": profileId, "portalId": portalId }, callbackMethod, "html");
    }

    GetAssociatedShippingListToStore(portalId: number, callbackMethod) {
        super.ajaxRequest("/Store/GetAssociatedShippingList", "get", { "portalId": portalId }, callbackMethod, "html");
    }

    GetProfileCatalogList(profileId: number, callbackMethod) {
        super.ajaxRequest("/Profiles/GetProfileCatalogList", "get", { "profileId": profileId }, callbackMethod, "html");
    }

    DeleteAssociateProductsFromProfile(catalogAssociation: Object, callbackMethod) {
        super.ajaxRequest("/PIM/Catalog/DeleteAssociateProductsFromProfile", "post", { "catalogAssociationViewModel": catalogAssociation }, callbackMethod, "json");
    }

    MoveCategory(addtoFolderId: string, folderId: string, catalogId: number, callbackMethod) {
        super.ajaxRequest("/PIM/Catalog/MoveFolder", "get", { "addtoFolderId": addtoFolderId, "folderId": folderId, "pimCatalogId": catalogId }, callbackMethod, "json");
    }

    AssociateProductsToProfileCatalog(catalogAssociation: Object, callbackMethod) {
        super.ajaxRequest("/PIM/Catalog/AssociateProductToProfileCatalog", "post", { "catalogAssociationViewModel": catalogAssociation }, callbackMethod, "json");
    }

    DeleteMultipleCatalogAssociatedCategoriesForProfile(pimCatalogId: number, pimCategoryHierarchyId: string, profileCatalogId: number, callbackMethod) {
        super.ajaxRequest("/PIM/Catalog/DeleteAssociateCategoryForProfile", "get", { "pimCatalogId": pimCatalogId, "pimCategoryHierarchyId": pimCategoryHierarchyId, "profileCatalogId": profileCatalogId }, callbackMethod, "json");
    }

    CreateNewView(itemViewId: number, itemText: string, isPublic: boolean, isDefault: boolean, callbackMethod) {
        super.ajaxRequest("/XMLGenerator/CreateNewView", "get", { "itemViewId": itemViewId, "itemText": itemText, "isPublic": isPublic, "isDefault": isDefault }, callbackMethod, "json");
    }

    CreateSearchScheduler(erpTaskSchedulerViewModel: any, callbackMethod) {
        super.ajaxRequest("/TouchPointConfiguration/Create", "post", { "erpTaskSchedulerViewModel": erpTaskSchedulerViewModel }, callbackMethod, "json");
    }

    AddSynonyms(searchSynonymsViewModel: any, callbackMethod) {
        super.ajaxRequest("/SearchConfiguration/CreateSearchSynonyms", "post", { "searchSynonymsViewModel": searchSynonymsViewModel }, callbackMethod, "json");
    }

    CreateSearchProfileTriggers(SearchTriggersViewModel: any, callbackMethod) {
        super.ajaxRequest("/Search/Search/CreateSearchProfileTriggers", "post", { "searchTriggersViewModel": SearchTriggersViewModel }, callbackMethod, "json");
    }

    GetSearchProfilesTriggers(searchProfileId: number, callbackMethod) {
        super.ajaxRequest("/Search/Search/GetSearchProfilesTriggers", "get", { "searchProfileId": searchProfileId }, callbackMethod, "html");
    }

    EditSynonyms(searchSynonymsViewModel: any, callbackMethod) {
        super.ajaxRequest("/SearchConfiguration/EditSearchSynonyms", "post", { "searchSynonymsViewModel": searchSynonymsViewModel }, callbackMethod, "json");
    }

    EditSearchScheduler(erpTaskSchedulerViewModel: any, callbackMethod) {
        super.ajaxRequest("/TouchPointConfiguration/Edit", "post", { "erpTaskSchedulerViewModel": erpTaskSchedulerViewModel }, callbackMethod, "json");
    }

    DeleteView(itemViewId: number, callbackMethod) {
        super.ajaxRequest("/XMLGenerator/DeleteView", "get", { "itemViewId": itemViewId }, callbackMethod, "json");
    }

    GiftCardList(isExcludeExpired: boolean, callbackMethod: any) {
        super.ajaxRequest("/GiftCard/List", "get", { "isExcludeExpired": isExcludeExpired }, callbackMethod, "html");
    }
    GetView(itemViewId: number, viewName: string, callbackMethod) {
        super.ajaxRequest("/XMLGenerator/GetView", "get", { "itemViewId": itemViewId, "viewName": viewName }, callbackMethod, "json");
    }
    ProcessPayPalPayment(paymentmodel: any, callbackMethod) {
        super.ajaxRequest("/Order/ProcessPayPalPayment", "post", { "paymentmodel": paymentmodel }, callbackMethod, "json", false);
    }

    IssueGiftCard(requestModel: any, callbackMethod) {
        super.ajaxRequest("/RMAManager/IssueGiftCard", "post", { "requestModel": requestModel }, callbackMethod, "json");
    }

    DeleteTableRowData(id: string, callbackMethod) {
        super.ajaxRequest("/MyReports/DeleteTableRowData", "get", { "areaMappingId": id }, callbackMethod, "json");
    }

    GetExportData(dynamicReportType: string, callbackMethod) {
        super.ajaxRequest("/MyReports/GetExportData", "get", { "dynamicReportType": dynamicReportType }, callbackMethod, "json", false);
    }

    GetAllFamilies(isCategory: boolean, callbackMethod) {
        super.ajaxRequest("/Import/GetAllFamilies", "get", { "isCategory": isCategory }, callbackMethod, "json");
    }

    GetEditorFormats(currentPortalId: number, callbackMethod) {
        super.ajaxRequest("/DynamicContent/GetEditorFormats", "get", { "portalId": currentPortalId }, callbackMethod, "", false);
    }

    GetOperators(reportType: string, filterName: string, callbackMethod) {
        super.ajaxRequest("/MyReports/GetOperators", "get", {
            "reportType": reportType, "filterName": filterName
        }, callbackMethod, "json");
    }

    ActivateDeactivateProducts(productIds: string, isActive: boolean, callbackMethod) {
        super.ajaxRequest("/PIM/Products/ActivateDeactivateProducts", "get", { "productIds": productIds, "isActive": isActive }, callbackMethod, "json");
    }

    HighlightProductList(localeId: number, highlightId: number, highlightCode: string, callbackMethod) {
        super.ajaxRequest("/Highlight/HighlightProductList", "get", { "localeId": localeId, "highlightId": highlightId, "highlightCode": highlightCode }, callbackMethod, "html");
    }
    AssociatedBrandProductList(brandId: number, brandCode: string, brandName: string, localeId: number, callbackMethod) {
        super.ajaxRequest("/PIM/Brand/AssociatedProductList", "get", { "brandId": brandId, "localeId": localeId, "brandCode": brandCode, "brandName": brandName }, callbackMethod, "html");
    }

    ActiveInactiveBrand(brandIds: string, isActive: boolean, callBackMethod) {
        super.ajaxRequest("/PIM/Brand/ActiveInactiveBrand", "get", { "brandIds": brandIds, "isActive": isActive }, callBackMethod, "json");
    }

    SetCustomerDefaultProfile(userId: number, profileId: any, callbackMethod) {
        super.ajaxRequest("/Customer/SetDefaultProfile", "get", { "userId": userId, "profileId": profileId }, callbackMethod, "html");
    }

    SetAccountDefaultProfile(accountId: number, accountProfileId: number, profileId: number, callbackMethod) {
        super.ajaxRequest("/Account/SetDefaultProfile", "get", { "accountId": accountId, "accountProfileId": accountProfileId, "profileId": profileId }, callbackMethod, "html");
    }

    IsMediaAttributeDefaultValueCodeExist(attributeId: number, attributeDefaultValueCode: string, defaultvalueId: number, callbackMethod) {
        super.ajaxRequest("/MediaManager/Attributes/IsAttributeDefaultValueCodeExist", "get", { "attributeId": attributeId, "attributeDefaultValueCode": attributeDefaultValueCode, "defaultValueId": defaultvalueId }, callbackMethod, "json", false);
    }
    GetDashboardSales(portalIds: string, durationId: string, callbackMethod) {
        super.ajaxRequest("/Dashboard/DashboardSaleReport", "get", { "portalIds": portalIds, "durationId": durationId }, callbackMethod, "html");
    }
    GetDashboardOrders(portalIds: string, durationId: string, callbackMethod) {
        super.ajaxRequest("/Dashboard/DashboardOrderReport", "get", { "portalIds": portalIds, "durationId": durationId }, callbackMethod, "html");
    }
    GetDashboardRevenue(portalIds: string, durationId: string, callbackMethod) {
        super.ajaxRequest("/Dashboard/DashboardRevenueReport", "get", { "portalIds": portalIds, "durationId": durationId }, callbackMethod, "html");
    }
    GetDashboardTopResult(portalIds: string, durationId: string, callbackMethod) {
        super.ajaxRequest("/Dashboard/DashboardTopReport", "get", { "portalIds": portalIds, "durationId": durationId }, callbackMethod, "html");
    }
    GetDashboardLowProductInventory(portalIds: string, durationId: string, callbackMethod) {
        super.ajaxRequest("/Dashboard/DashboardLowProductInventoryReport", "get", { "portalIds": portalIds, "durationId": durationId }, callbackMethod, "html");
    }
    IsSliderNameExist(name: string, cmsSliderId: number, callbackMethod) {
        super.ajaxRequest("/WebSite/IsSliderNameExist", "get", {
            "name": name, "cmsSliderId": cmsSliderId
        }, callbackMethod, "json", false);
    }
    IsContentPageNameExist(name: string, portalId: number, callbackMethod) {
        super.ajaxRequest("/Content/IsContentPageNameExistForPortal", "get", {
            "name": name,
            "portalId": portalId
        }, callbackMethod, "json", false);
    }
    IsFileNameExist(name: string, callbackMethod) {
        super.ajaxRequest("/ProductFeed/IsFileNameExist", "get", {
            "name": name,
        }, callbackMethod, "json", false);
    }

    IsThemeNameExist(name: string, cmsThemeId: number, callbackMethod) {
        super.ajaxRequest("/Theme/IsThemeNameExist", "get", {
            "name": name, "cmsThemeId": cmsThemeId
        }, callbackMethod, "json", false);
    }
    IsCatalogNameExist(catalogName: string, pimCatalogId: number, callbackMethod) {
        super.ajaxRequest("/Catalog/IsCatalogNameExist", "post", { "CatalogName": catalogName, "PimCatalogId": pimCatalogId }, callbackMethod, "json", false);
    }
    EditCatalog(pimCatalogId: number, callbackMethod) {
        super.ajaxRequest("/PIM/Catalog/EditCatalog", "get", { "PimCatalogId": pimCatalogId }, callbackMethod, "html");
    }
    IsRuleNameExist(ruleName: string, publishCatalogId: number, callbackMethod) {
        super.ajaxRequest("/Search/Search/IsRuleNameExist", "post", { "ruleName": ruleName, "publishCatalogId": publishCatalogId }, callbackMethod, "json", false);
    }
    IsShippingNameExist(shippingName: string, shippingId: number, callbackMethod) {
        super.ajaxRequest("/Shippings/IsShippingNameExist", "post", { "ShippingName": shippingName, "shippingId": shippingId }, callbackMethod, "json", false);
    }
    IsPaymentCodeExist(paymentCode: string, paymentSettingId: number, callbackMethod) {
        super.ajaxRequest("/Payment/IsPaymentCodeExist", "post", { "paymentCode": paymentCode, "paymentSettingId": paymentSettingId }, callbackMethod, "json", false);
    }
    IsPromotionCodeExist(promotionCode: string, promotionId: number, callbackMethod) {
        super.ajaxRequest("/Promotion/IsPromotionNameExist", "post", { "PromoCode": promotionCode, "promotionId": promotionId }, callbackMethod, "json", false);
    }
    IsRoleNameExist(name: string, id: string, callbackMethod) {
        super.ajaxRequest("/RoleAndAccessRight/IsRoleNameExist", "post", { "Name": name, "id": id }, callbackMethod, "json", false);
    }
    IsUserIdForGiftCardExist(userId: number, portalId: number, callbackMethod) {
        super.ajaxRequest("/GiftCard/IsUserIdExist", "post", { "UserId": userId, "portalId": portalId }, callbackMethod, "json", false);
    }
    IsSeoNameExist(seoUrl: string, cmsContentPagesId: number, portalId: number, callbackMethod) {
        super.ajaxRequest("/Content/IsSeoNameExist", "post", {
            "SEOUrl": seoUrl, "CMSContentPagesId": cmsContentPagesId, "PortalId": portalId
        }, callbackMethod, "json", false);
    }
    IsBannerNameExist(title: string, cmsSliderBannerId: number, cmsSliderId: number, callbackMethod) {
        super.ajaxRequest("/WebSite/IsBannerNameExist", "post", {
            "Title": title, "CMSSliderBannerId": cmsSliderBannerId, "CMSSliderId": cmsSliderId
        }, callbackMethod, "json", false);
    }
    IsDomainNameExist(DomainName: string, DomainId: number, callbackMethod) {
        super.ajaxRequest("/Store/IsDomainNameExist", "post", {
            "DomainName": DomainName, "DomainId": DomainId
        }, callbackMethod, "json", false);
    }
    IsAccountPermissionExist(accountPermissionName: string, accountId: number, accountPermissionId: number, callbackMethod) {
        super.ajaxRequest("/Account/IsAccountPermissionExist", "post", {
            "AccountPermissionName": accountPermissionName, "AccountId": accountId, "AccountPermissionId": accountPermissionId
        }, callbackMethod, "json", false);
    }
    IsMediaAttributeGroupCodeExist(groupCode: string, mediaAttributeGroupId: number, callbackMethod) {
        super.ajaxRequest("/AttributeGroup/IsGroupCodeExist", "post", {
            "groupCode": groupCode, "mediaAttributeGroupId": mediaAttributeGroupId
        }, callbackMethod, "json", false);
    }
    IsAttributeFamilyCodeExist(familyCode: string, isCategory: boolean, PimAttributeFamilyId: number, callbackMethod) {
        super.ajaxRequest("/ProductAttributeFamily/IsFamilyCodeExist", "post", {
            "FamilyCode": familyCode, "IsCategory": isCategory, "PimAttributeFamilyId": PimAttributeFamilyId
        }, callbackMethod, "json", false);
    }
    IsAttributeGroupCodeExist(groupCode: string, isCategory: boolean, pimAttributeGroupId: number, callbackMethod) {
        super.ajaxRequest("/ProductAttributeGroup/IsGroupCodeExist", "post", {
            "GroupCode": groupCode, "IsCategory": isCategory, "PimAttributeGroupId": pimAttributeGroupId
        }, callbackMethod, "json", false);
    }
    IsBrandSEOFriendlyPageNameExist(seoFriendlyPageName: string, seoDetailsId: number, callbackMethod) {
        super.ajaxRequest("/PIM/Brand/IsBrandSEOFriendlyPageNameExist", "post", { "seoFriendlyPageName": seoFriendlyPageName, "seoDetailsId": seoDetailsId }, callbackMethod, "json", false);
    }
    MoveContentPagesFolder(addtoFolderId: string, folderId: string, callbackMethod) {
        super.ajaxRequest("/Content/MoveContentPagesFolder", "get", { "addtoFolderId": addtoFolderId, "folderId": folderId }, callbackMethod, "json");
    }
    GetPublishProduct(publishProductId: number, localeId: number, portalId: number, userId: number, catalogId: number, callbackMethod) {
        super.ajaxRequest("/Order/GetPublishProduct", "get", { "publishProductId": publishProductId, "localeId": localeId, "portalId": portalId, "userId": userId, "catalogId": catalogId }, callbackMethod, "html");
    }

    AddProductToCart(orderId: number, userId: number, callbackMethod) {
        super.ajaxRequest("/Order/AddProductToCart", "get", { "cartItems": true, "orderId": orderId, "userId": userId }, callbackMethod, "json");
    }
    SyncMedia(folderName, callbackMethod) {
        super.ajaxRequest("/MediaConfiguration/SyncMedia", "post", { "folderName": folderName }, callbackMethod, "html");
    }

    UpdateDisplayOrder(pimCatalogId: number, pimCategoryId: number, displayOrder: number, pimCategoryHierarchyId: number, isDown: boolean, callbackMethod) {
        super.ajaxRequest("/Catalog/UpdateDisplayOrder", "get", { "pimCatalogId": pimCatalogId, "pimCategoryId": pimCategoryId, "displayOrder": displayOrder, "pimCategoryHierarchyId": pimCategoryHierarchyId, "isDown": isDown }, callbackMethod, "json", false);
    }

    CaptureVoidPayment(url: string, callbackMethod) {
        super.ajaxRequest(url, "get", {}, callbackMethod, "json");
    }
    AssociateTaxClassListToStore(taxClassIds: string, portalId: number, callbackMethod) {
        super.ajaxRequest("/Store/AssociateTaxClass", "get", { "taxClassIds": taxClassIds, "portalId": portalId }, callbackMethod, "json", false);
    }
    StoreAssociatedTaxClassList(portalId: number, callbackMethod) {
        super.ajaxRequest("/Store/TaxList", "get", { "portalId": portalId }, callbackMethod, "html");
    }
    UnAssociateTaxClass(taxClassIds: string, portalId: number, callbackMethod) {
        super.ajaxRequest("/Store/UnAssociateTaxClass", "get", { "taxClassId": taxClassIds, "portalId": portalId }, callbackMethod, "json");
    }

    AssociateBrandsToPortal(brandIds: string, portalId: number, callbackMethod) {
        super.ajaxRequest("/Store/AssociatePortalBrand", "get", { "brandIds": brandIds, "portalId": portalId }, callbackMethod, "json", false);
    }

    GetStoreAssociatedBrandList(portalId: number, callbackMethod) {
        super.ajaxRequest("/Store/BrandList", "get", { "portalId": portalId }, callbackMethod, "html");
    }

    UnAssociateBrandsFromPortal(brandIds: string, portalId: number, callbackMethod) {
        super.ajaxRequest("/Store/UnAssociatePortalBrand", "get", { "brandId": brandIds, "portalId": portalId }, callbackMethod, "json");
    }

    SetPortalDefaultTax(taxClassIds: string, portalId: number, callbackMethod) {
        super.ajaxRequest("/Store/SetPortalDefaultTax", "get", { "taxClassId": taxClassIds, "portalId": portalId }, callbackMethod, "html");
    }

    AssociatePaymentSetting(paymentSettingId: string, portalId: number, callbackMethod) {
        super.ajaxRequest("/Store/AssociatePaymentSetting", "get", { "paymentSettingId": paymentSettingId, "portalId": portalId }, callbackMethod, "json", false);
    }

    AssociateOfflinePaymentSetting(paymentSettingId: string, portalId: number, callbackMethod) {
        super.ajaxRequest("/Store/AssociateOfflinePaymentSetting", "get", { "paymentSettingId": paymentSettingId, "portalId": portalId }, callbackMethod, "json", false);
    }

    AssociateSortSetting(sortSettingId: string, portalId: number, callbackMethod) {
        super.ajaxRequest("/Store/AssociateSortSetting", "get", { "sortSettingId": sortSettingId, "portalId": portalId }, callbackMethod, "json", false);
    }

    AssociatePageSetting(pageSettingId: string, portalId: number, callbackMethod) {
        super.ajaxRequest("/Store/AssociatePageSetting", "get", { "pageSettingId": pageSettingId, "portalId": portalId }, callbackMethod, "json", false);
    }

    GetAssociatedPaymentList(portalId: number, callbackMethod) {
        super.ajaxRequest("/Store/GetAssociatedPaymentList", "get", { "portalId": portalId }, callbackMethod, "html");
    }

    GetAssociatedInvoiceManagementPaymentList(portalId: number, callbackMethod) {
        super.ajaxRequest("/Store/GetAssociatedInvoiceManagementPaymentList", "get", { "portalId": portalId }, callbackMethod, "html");
    }

    GetAssociatedSortList(portalId: number, callbackMethod) {
        super.ajaxRequest("/Store/GetAssociatedSortForStore", "get", { "portalId": portalId }, callbackMethod, "html");
    }

    GetAssociatedPageList(portalId: number, callbackMethod) {
        super.ajaxRequest("/Store/GetAssociatedPageForStore", "get", { "portalId": portalId }, callbackMethod, "html");
    }

    RemoveAssociatedSortSetting(portalSortSettingId: string, portalId: number, callbackMethod) {
        super.ajaxRequest("/Store/RemoveAssociatedSortSetting", "get", { "portalSortSettingId": portalSortSettingId, "portalId": portalId }, callbackMethod, "json");
    }

    RemoveAssociatedPageSetting(portalPageSettingId: string, portalId: number, callbackMethod) {
        super.ajaxRequest("/Store/RemoveAssociatedPageSetting", "get", { "portalPageSettingId": portalPageSettingId, "portalId": portalId }, callbackMethod, "json");
    }

    RemoveAssociatedPaymentSetting(paymentSettingId: string, portalId: number, isUsedForOfflinePayment: boolean, callbackMethod) {
        super.ajaxRequest("/Store/RemoveAssociatedPaymentSetting", "get", { "paymentSettingId": paymentSettingId, "portalId": portalId, "isUsedForOfflinePayment": isUsedForOfflinePayment }, callbackMethod, "json");
    }

    AssociatePaymentSettingForProfiles(paymentSettingId: string, portalId: number, callbackMethod) {
        super.ajaxRequest("/Profiles/AssociatePaymentSetting", "get", { "paymentSettingId": paymentSettingId, "profileId": portalId }, callbackMethod, "json", false);
    }

    GetAssociatedPaymentListForProfiles(profileId: number, portalId: number, callbackMethod) {
        super.ajaxRequest("/Profiles/GetAssociatedPaymentList", "get", { "profileId": profileId, "portalId": portalId }, callbackMethod, "html");
    }

    RemoveAssociatedPaymentSettingForProfiles(paymentSettingId: string, portalId: number, callbackMethod) {
        super.ajaxRequest("/Profiles/RemoveAssociatedPaymentSetting", "get", { "paymentSettingId": paymentSettingId, "profileId": portalId }, callbackMethod, "json");
    }

    GetImportInventoryView(callbackMethod) {
        super.ajaxRequest("/Inventory/ImportInventoryView", "get", {}, callbackMethod, "html");
    }

    ImportPost(importModel: any, callbackMethod) {
        super.ajaxRequest("/Import/Index", "post", { "importModel": importModel }, callbackMethod, "json");
    }

    ExportPost(exportModel: any, callbackMethod) {
        super.ajaxRequest("/Export/Index", "post", { "exportModel": exportModel }, callbackMethod, "json");
    }
    GetPricingList(callbackMethod) {
        super.ajaxRequest("/Import/GetPricingList", "get", {}, callbackMethod, "json");
    }

    GetImportCatalogList(callbackMethod) {
        super.ajaxRequest("/Import/GetCatalogList", "get", {}, callbackMethod, "json");
    }

    GetImportPromotionTypeList(callbackMethod) {
        super.ajaxRequest("/Import/GetPromotionTypeList", "get", {}, callbackMethod, "json");
    }

    GetImportPortalList(callbackMethod) {
        super.ajaxRequest("/Import/GetPortalList", "get", {}, callbackMethod, "json");
    }
    ImportLogDetailsDownloadPdf(importProcessLogId: string, callbackMethod) {
        super.ajaxRequest("/Import/DownloadPDF", "post", { "importProcessLogId": importProcessLogId }, callbackMethod , "json");
    }

    GetLocalServerURL(callbackMethod) {
        super.ajaxRequest("/MediaConfiguration/GetLocalServerURL", "get", {}, callbackMethod, "json");
    }
    GetNetworkDriveURL(callbackMethod) {
        super.ajaxRequest("/MediaConfiguration/GetNetworkDriveURL", "get", {}, callbackMethod, "json");
    }

    DeleteImportLogs(importProcessLogId: string, callbackMethod) {
        super.ajaxRequest("/Import/DeleteLogs", "get", { "importProcessLogId": importProcessLogId }, callbackMethod, "json");
    }
    DeleteExportLogs(exportProcessLogIds: string, callbackMethod) {
        super.ajaxRequest("/Export/DeleteLogs", "get", { "exportProcessLogId": exportProcessLogIds }, callbackMethod, "json");
    }

    GetColumnList(reportId: number, dynamicReportType: string, callbackMethod) {
        super.ajaxRequest("/MyReports/GetColumnList", "get", { "reportId": reportId, "dynamicReportType": dynamicReportType }, callbackMethod, "json");
    }

    DeleteDynamicReport(customReportTemplateId: string, callbackMethod) {
        super.ajaxRequest("/MyReports/DeleteReport", "get", { "customReportTemplateId": customReportTemplateId }, callbackMethod, "json");
    }

    CheckCouponCodeExist(couponCode: string, promotionId: number, callbackMethod) {
        super.ajaxRequest("/Promotion/CheckCouponCodeExist", "post", { "couponCode": couponCode, "promotionId": promotionId }, callbackMethod, "json", false);
    }

    SetCollapseMenuStatus(status: Boolean, callbackMethod) {
        super.ajaxRequest("/User/SetCollapseMenuStatus", "get", { "status": status }, callbackMethod, "json");
    }

    SaveCustomReport(model, callbackMethod) {
        super.ajaxRequest("/MyReports/DynamicReport", "post", { "model": model }, callbackMethod, "json", false);
    }
    GetShippingOptions(userId: number, excludeCustomShippingFromCreateOrder: boolean, isQuote: boolean, callbackMethod) {
        super.ajaxRequest("/Order/GetShippingOptionsListWithRates", "get", { "userId": userId, "excludeCustomShippingFromCreateOrder": excludeCustomShippingFromCreateOrder, "isQuote": isQuote }, callbackMethod, "json", true, false);
    }
    GetShippingOptionsForManage(orderId: number, callbackMethod) {
        super.ajaxRequest("/Order/GetShippingOptionsListForManage", "get", { "orderId": orderId }, callbackMethod, "json");
    }
    GetReportView(reportType: string, callbackMethod) {
        super.ajaxRequest("/MyReports/GetReportView", "get", { "reportType": reportType }, callbackMethod, "html", false);
    }

    GetImportCountryList(callbackMethod) {
        super.ajaxRequest("/Import/GetCountryList", "get", {}, callbackMethod, "json");
    }
    showDiagnosticsTrace(callbackMethod) {
        super.ajaxRequest("/Trace.axd?id=0", "get", {}, callbackMethod, "html");
    }
    UpdateAttributeGroupDisplayOrder(pimAttributeGroupId: number, displayOrder: number, pimAttributeFamilyId: number, callbackMethod) {
        super.ajaxRequest("/PIM/ProductAttributeFamily/UpdateAttributeGroupDisplayOrder", "post", { "pimAttributeGRoupId": pimAttributeGroupId, "pimAttributeFamilyId": pimAttributeFamilyId, "displayOrder": displayOrder }, callbackMethod, "json");
    }

    QuickOrderAddToCart(cartItemModel: Znode.Core.CartModel, callbackMethod) {
        super.ajaxRequest("/Order/AddToCart", "post", { "cartItem": cartItemModel }, callbackMethod, "html");
    }
    MovePage(folderId: string, pageIds: string, callbackMethod) {
        super.ajaxRequest("/Content/MovePageToFolder", "get", { "folderId": folderId, "pageIds": pageIds }, callbackMethod, "json");
    }


    UpdateOrderPaymentStatus(omsOrderId: number, paymentStatus: string, callbackMethod) {
        super.ajaxRequest("/Order/UpdateOrderPaymentStatus", "get", { "omsOrderId": omsOrderId, "paymentStatus": paymentStatus }, callbackMethod, "json");
    }
    UpdateTrackingNumber(omsOrderId: number, TrackingNumber: string, callbackMethod) {
        super.ajaxRequest("/Order/UpdateTrackingNumber", "get", { "omsOrderId": omsOrderId, "trackingnumber": TrackingNumber }, callbackMethod, "json");
    }
    UpdateColumnShipping(OmsOrderId: number, ColumnShipping: string, callbackMethod) {
        super.ajaxRequest("/Order/UpdateColumnShipping", "get", { "OmsOrderId": OmsOrderId, "shipping": ColumnShipping }, callbackMethod, "json");
    }
    UpdateCSRDiscountAmount(OmsOrderId: number, CSRDiscountAmount: string, callbackMethod) {
        super.ajaxRequest("/Order/UpdateCSRDiscountAmount", "get", { "OmsOrderId": OmsOrderId, "CSRDiscountAmount": CSRDiscountAmount }, callbackMethod, "json");
    }
    UpdateTaxCost(OmsOrderId: number, TaxCost: string, callbackMethod) {
        super.ajaxRequest("/Order/UpdateTaxCost", "get", { "OmsOrderId": OmsOrderId, "taxCost": TaxCost }, callbackMethod, "json");
    }
    UpdateShippingType(OmsOrderId: number, ShippingType: string, callbackMethod) {
        super.ajaxRequest("/Order/UpdateShippingType", "get", { "OmsOrderId": OmsOrderId, "shippingType": ShippingType }, callbackMethod, "json");
    }
    GetOrderInformation(orderId: number, callbackMethod) {
        super.ajaxRequest("/Order/GetOrderInformation", "get", { "orderId": orderId }, callbackMethod, "html");
    }
    GetCustomerInformation(orderId: number, callbackMethod) {
        super.ajaxRequest("/Order/GetCustomerInformation", "get", { "orderId": orderId }, callbackMethod, "html");
    }
    GetOrderLineItems(orderId: number, callbackMethod) {
        super.ajaxRequest("/Order/GetOrderLineItems", "get", { "orderId": orderId }, callbackMethod, "json");
    }
    GetReturnLineItems(orderId: number, callbackMethod) {
        super.ajaxRequest("/Order/GetReturnLineItems", "get", { "orderId": orderId }, callbackMethod, "html");
    }
    CheckTemplateName(fileName: string, callbackMethod) {
        super.ajaxRequest("/Template/TemplateName", "post", { "templateName": fileName }, callbackMethod, "json");
    }
    ShippingSelectHandler(userId: number, shippingId: number, orderId: number, callBackMethod) {
        super.ajaxRequest("/Order/CalculateShippingInManage", "post", { "userId": userId, "shippingId": shippingId, "omsOrderId": orderId }, callBackMethod, "json")
    }
    UpdateCartItem(_orderLineItemDetail: Znode.Core.OrderLineItemModel, callbackMethod) {
        super.ajaxRequest("/Order/UpdateCartItem", "post", { "orderDataModel": _orderLineItemDetail }, callbackMethod, "json");
    }

    UpdateManageOrder(orderId: number, additionalNotes: string, callbackMethod) {
        super.ajaxRequest("/Order/UpdateOrder", "post", { "orderId": orderId, "additionalNote": additionalNotes }, callbackMethod, "json");
    }
    SubmitEditOrderpayment(submitPaymentViewModel: Object, callbackMethod) {
        super.ajaxRequest("/Order/SubmitEditOrderpayment", "post", { "submitPaymentViewModel": submitPaymentViewModel }, callbackMethod, "json");
    }

    AssociateAddonGroupProducts(model: Object, callbackMethod) {
        super.ajaxRequest("/PIM/AddonGroup/AssociateAddonGroupProducts", "post", { "model": model }, callbackMethod, "json");
    }

    DeleteAddonGroupProducts(addonGroupProductIds: string, callbackMethod) {
        super.ajaxRequest("/PIM/AddonGroup/DeleteAddonGroupProducts", "get", { "addonGroupProductId": addonGroupProductIds }, callbackMethod, "json");
    }

    GetAssociatedAddonProducts(addonGroupId: number, addonGroupName: string, localeId: number, callbackMethod) {
        super.ajaxRequest("/PIM/AddonGroup/GetAssociatedProducts", "get", { "addonGroupId": addonGroupId, "addonGroupName": addonGroupName, "localeId": localeId }, callbackMethod, "html");
    }

    PrintOnManage(omsOrderId: number, callbackMethod) {
        super.ajaxRequest("/Order/PrintOnManage", "get", { "omsOrderId": omsOrderId }, callbackMethod, "html")
    }

    UpdateForTaxExempt(omsOrderId: number, orderTextValue: string, pageName: string, callbackMethod) {
        super.ajaxRequest("/Order/UpdateOrderText", "get", { "omsOrderId": omsOrderId, "orderTextValue": orderTextValue, "pageName": pageName }, callbackMethod, "html");
    }

    UpdateTaxExemptOnCreateOrder(userId: number, orderTextValue: string, pageName: string, isTaxExempt: boolean, isQuote: boolean, callbackMethod) {
        super.ajaxRequest("/Order/UpdateTaxExemptOnCreateOrder", "get", {
            "userId": userId, "orderTextValue": orderTextValue, "pageName": pageName, "isTaxExempt": isTaxExempt, "isQuote": isQuote }, callbackMethod, "html", true, false);
    }

    GetReasonsForReturn(callbackMethod) {
        super.ajaxRequest("/Order/GetReasonsForReturn", "get", {}, callbackMethod, "json", false)
    }

    GetShippingList(omsOrderId: number, callbackMethod) {
        super.ajaxRequest("/Order/GetShippingPanel", "get", { "omsOrderId": omsOrderId }, callbackMethod, "json");
    }

    GetOrderStateValueById(omsOrderStateId: number, callbackMethod) {
        super.ajaxRequest("/Order/GetOrderStateValueById", "get", { "omsOrderStateId": omsOrderStateId }, callbackMethod, "json", false)
    }
    IsUserNameExist(userName: string, portalId: number, callbackMethod) {
        super.ajaxRequest("/User/IsUserNameExists", "get", {
            "userName": userName, "portalId": $("#PortalId").val(),
        }, callbackMethod, "json", false);
    }

    IsAccountNameExist(accountName: string, accountId: number, portalId: number, callbackMethod) {
        super.ajaxRequest("/Account/IsAccountNameExists", "get", {
            "accountName": accountName, "accountId": accountId, "portalId": portalId
        }, callbackMethod, "json", false);
    }

    SendReturnedOrderEmail(omsOrderId: number, callbackMethod) {
        super.ajaxRequest("/Order/SendReturnedOrderEmail", "get", { "omsOrderId": omsOrderId }, callbackMethod, "json");
    }

    PublishCatalog(pimCatalogId: number, revisionType: string, publishContent: string, callbackMethod) {
        super.ajaxRequest("/Catalog/Publish", "get", { "pimCatalogId": pimCatalogId, "revisionType": revisionType, "publishContent": publishContent }, callbackMethod, "json");
    }

    PreviewCatalog(pimCatalogId: number, callbackMethod) {
        super.ajaxRequest("/Catalog/Preview", "get", { "pimCatalogId": pimCatalogId }, callbackMethod, "json");
    }

    PublishStoreSetting(portalId: number, targetPublishState: string, publishContent: string, callbackMethod) {
        super.ajaxRequest("/Store/PublishStoreSetting", "get", { "portalId": portalId, "targetPublishState": targetPublishState, "publishContent": publishContent }, callbackMethod, "json");
    }

    PublishStoreCMSContent(portalId: number, targetPublishState: string, publishContent: string, callbackMethod) {
        super.ajaxRequest("/StoreExperience/PublishStoreCMSContent", "get", { "portalId": portalId, "targetPublishState": targetPublishState, "publishContent": publishContent }, callbackMethod, "json");
    }

    PublishProduct(pimProductId: number, revisionType: string, callbackMethod) {
        super.ajaxRequest("/PIM/Products/PublishProduct", "get", { "pimProductId": pimProductId, "revisionType": revisionType }, callbackMethod, "json");
    }

    PublishMessage(cmsMessageKeyId: number, portalId: number, targetPublishState: string, localeId: number, callbackMethod) {
        super.ajaxRequest("/Content/PublishManageMessageWithPreview", "get", {
            "cmsMessageKeyId": cmsMessageKeyId, "portalId": portalId, "localeId": localeId, "targetPublishState": targetPublishState, "takeFromDraftFirst": true
        }, callbackMethod, "json");
    }

    PublishSeo(seoCode: string, portalId: number, localeId: number, seoTypeId: number, callbackMethod) {
        super.ajaxRequest("/Seo/Publish", "get", { "seoCode": seoCode, "portalId": portalId, "localeId": localeId, "seoTypeId": seoTypeId }, callbackMethod, "json");
    }

    PublishSeoWithPreview(seoCode: string, portalId: number, localeId: number, seoTypeId: number, targetPublishState: string, takeFromDraftFirst: boolean, callbackMethod) {
        super.ajaxRequest("/Seo/Publishwithpreview", "get", { "seoCode": seoCode, "portalId": portalId, "localeId": localeId, "seoTypeId": seoTypeId, "targetPublishState": targetPublishState, "takeFromDraftFirst": takeFromDraftFirst }, callbackMethod, "json");
    }

    AssignTouchPointToActiveERP(touchPointNames: string, callbackMethod) {
        super.ajaxRequest("/TouchPointConfiguration/AssignTouchPointToActiveERP", "get", { "touchPointNames": touchPointNames }, callbackMethod, "json");
    }

    AssignedTouchPointList(callbackMethod) {
        super.ajaxRequest("/TouchPointConfiguration/List", "get", {}, callbackMethod, "html");
    }

    ShowTaskSchedularLogDetails(schedulerName: string, recordId: string, callbackMethod) {
        super.ajaxRequest("/TouchPointConfiguration/SchedulerLogDetails", "get", {
            "schedulerName": schedulerName, "recordId": recordId
        }, callbackMethod, "html");
    }

    CreateCatalogSchedular(connectorTouchPoints: string, schedulerCallFor: string, callbackMethod) {
        super.ajaxRequest("/PIM/Catalog/CreateScheduler", "get", {
            "connectorTouchPoints": connectorTouchPoints, "schedulerCallFor": schedulerCallFor
        }, callbackMethod, "html");
    }

    ResendOrderConfirmationForCartItem(orderId: number, cartLineItemId: number, callbackMethod) {
        super.ajaxRequest("/Order/ResendOrderLineItemConfirmMail", "get", { "omsOrderId": orderId, "orderLineItemId": cartLineItemId }, callbackMethod, "html");
    }

    AssociateShippingToPromotion(associatedShippingIds: string, promotionId: number, callbackMethod) {
        super.ajaxRequest("/Promotion/AssociateShippingToPromotion", "get", { "associatedShippingIds": associatedShippingIds, "promotionId": promotionId }, callbackMethod, "json");
    }

    GetAssociatedShippings(storeId: number, shippingIds: string, promotionId: number, callbackMethod) {
        super.ajaxRequest("/Promotion/GetAssociatedShippings", "get", { "storeId": storeId, "assignedIds": shippingIds, "promotionId": promotionId }, callbackMethod, "html");
    }

    DeleteAssociatedPromotionShippings(associatedShippingIds: string, promotionId: number, callbackMethod) {
        super.ajaxRequest("/Promotion/UnAssociateShippings", "get", { "ShippingId": associatedShippingIds, "promotionId": promotionId }, callbackMethod, "json");
    }

    GenerateProductFeed(url: string, callbackMethod) {
        super.ajaxRequest(url, "get", {}, callbackMethod, "json");
    }

    GetBlogNews(blogNewsId, localeId: number, callbackMethod) {
        super.ajaxRequest("/BlogNews/EditBlogNews", "get", { "blogNewsId": blogNewsId, "localeId": localeId }, callbackMethod, "html");
    }

    DeleteBlogNews(blogNewsId: string, callbackMethod) {
        super.ajaxRequest("/BlogNews/DeleteBlogNews", "get", { "BlogNewsId": blogNewsId }, callbackMethod, "json");
    }

    DeleteBlogNewsComment(blogNewsCommentId: string, callbackMethod) {
        super.ajaxRequest("/BlogNews/DeleteBlogNewsComment", "get", { "BlogNewsCommentId": blogNewsCommentId }, callbackMethod, "json");
    }

    ActivateDeactivateBlogNews(blogNewsIds: string, isTrueOrFalse: boolean, activity: string, callBackMethod) {
        super.ajaxRequest("/BlogNews/ActivateDeactivateBlogNews", "get", { "blogNewsIds": blogNewsIds, "isTrueOrFalse": isTrueOrFalse, "activity": activity }, callBackMethod, "json");
    }

    ApproveDisapproveBlogNewsComment(blogNewsCommentIds: string, isApproved: boolean, callBackMethod) {
        super.ajaxRequest("/BlogNews/ApproveDisapproveBlogNewsComment", "get", { "blogNewsCommentIds": blogNewsCommentIds, "isApproved": isApproved }, callBackMethod, "json");
    }

    UpdateReturnShippingHistory(lineItemId: number, omsOrderId: number, isInsert: boolean, callBackMethod) {
        super.ajaxRequest("/Order/UpdateReturnShippingHistory", "post", {
            "lineItemId": lineItemId, "omsOrderId": omsOrderId, "isInsert": isInsert
        }, callBackMethod, "json");
    }

    //SaveReturnShippingHistory(_orderLineItemDetail: Znode.Core.OrderLineItemModel, callbackMethod) {
    //    super.ajaxRequest("/Order/SaveReturnShippingHistory", "post", { "orderDataModel": _orderLineItemDetail }, callbackMethod, "json");
    //}

    GetSynonymsList(catalogId: number, callbackMethod) {
        super.ajaxRequest("/SearchConfiguration/GetSearchSynonymsList", "get", { "catalogId": catalogId }, callbackMethod, "html");
    }

    GetKeywordsRedirectList(catalogId: number, callbackMethod) {
        super.ajaxRequest("/SearchConfiguration/GetCatalogKeywordsList", "get", { "catalogId": catalogId }, callbackMethod, "html");
    }

    DeleteSearchSynonyms(searchSynonymsId: string, publishCataLogId: number, callbackMethod) {
        super.ajaxRequest("/SearchConfiguration/DeleteSearchSynonyms", "get", { "SearchSynonymsId": searchSynonymsId, "publishCataLogId": publishCataLogId }, callbackMethod, "json");
    }

    DeleteKeywords(searchKeywordsRedirectId: string, callbackMethod) {
        super.ajaxRequest("/SearchConfiguration/DeleteSearchKeywordsRedirect", "get", { "searchKeywordsRedirectId": searchKeywordsRedirectId }, callbackMethod, "json");
    }

    AddKeywords(searchKeywordsViewModel: any, callbackMethod) {
        super.ajaxRequest("/SearchConfiguration/CreateSearchKeywordsRedirect", "post", { "searchKeywordsRedirectViewModel": searchKeywordsViewModel }, callbackMethod, "json");
    }

    EditKeywords(searchKeywordsViewModel: any, callbackMethod) {
        super.ajaxRequest("/SearchConfiguration/EditSearchKeywordsRedirect", "post", { "searchKeywordsRedirectViewModel": searchKeywordsViewModel }, callbackMethod, "json");
    }

    DeleteIndex(catalogIndexId: number, callbackMethod) {
        super.ajaxRequest("/SearchConfiguration/DeleteIndex", "get", { "catalogIndexId": catalogIndexId }, callbackMethod, "json");
    }

    WriteSynonymsFile(publishCataLogId: number, isSynonymsFile: boolean, callbackMethod) {
        super.ajaxRequest("/SearchConfiguration/WriteSearchFile", "get", { "publishCataLogId": publishCataLogId, "isSynonymsFile": isSynonymsFile }, callbackMethod, "json");
    }
    PrintOnPackageSlip(omsOrderId: number, OmsOrderLineItemsId: string, callbackMethod) {
        super.ajaxRequest("/Order/PrintPackagingSlip", "get", { "omsOrderId": omsOrderId, "OmsOrderLineItemsId": OmsOrderLineItemsId }, callbackMethod, "html");
    }

    SendPoEmail(receiverEmail: string, omsOrderId: number, callbackMethod) {
        super.ajaxRequest("/Order/SendPOEmail", "get", { "receiverEmail": receiverEmail, "omsOrderId": omsOrderId }, callbackMethod, "html");
    }

    GetUnAssigedAttributes(attributeGroupId: number, attributeFamilyId: number, contollerName: string, callbackMethod) {
        super.ajaxRequest("/PIM/" + contollerName + "/GetUnAssignedAttributes", "post", { "attributeFamilyId": attributeFamilyId, "attributeGroupId": attributeGroupId }, callbackMethod, "html");
    }

    IsGlobalAttributeCodeExist(attributeCode: string, callbackMethod) {
        super.ajaxRequest("/GlobalAttribute/IsAttributeCodeExist", "get", { "attributeCode": attributeCode }, callbackMethod, "json", false);
    }

    DeleteGlobalAttribute(id: string, callbackMethod) {
        super.ajaxRequest("/GlobalAttribute/Delete", "get", { "globalAttributeId": id }, callbackMethod, "json");
    }

    SaveGlobalAttributeDefaultValues(url: string, data: any, attributeId: number, defaultvaluecode: string, defaultvalueId: number, displayOrder: number, isDefault: boolean, isSwatch: boolean, swatch: string, callbackMethod: any) {
        super.ajaxRequest(url, "get", {
            "model": JSON.stringify(data), "attributeId": attributeId, "defaultvalueId": defaultvalueId, "defaultvaluecode": defaultvaluecode, "displayOrder": displayOrder, "isdefault": isDefault, "isswatch": isSwatch, "swatchtext": swatch
        }, callbackMethod, "json");
    }

    IsGlobalAttributeDefaultValueCodeExist(attributeId: number, attributeDefaultValueCode: string, defaultvalueId: number, callbackMethod) {
        super.ajaxRequest("/GlobalAttribute/IsAttributeDefaultValueCodeExist", "get", { "attributeId": attributeId, "attributeDefaultValueCode": attributeDefaultValueCode, "defaultValueId": defaultvalueId }, callbackMethod, "json", false);
    }

    DeleteGlobalAttributeDefaultValues(defaultvalueId: number, callbackMethod: any) {
        super.ajaxRequest("/GlobalAttribute/DeleteDefaultValues", "get", { "defaultvalueId": defaultvalueId }, callbackMethod, "json");
    }

    IsGlobalAttributeGroupCodeExist(groupCode: string, globalAttributeGroupId: number, callbackMethod) {
        super.ajaxRequest("/GlobalAttributeGroup/IsGroupCodeExist", "post", { "GroupCode": groupCode, "globalAttributeGroupId": globalAttributeGroupId }, callbackMethod, "json", false);
    }

    DeleteGlobalAttributeGroup(id: string, callbackMethod) {
        super.ajaxRequest("/GlobalAttributeGroup/Delete", "get", { "globalAttributeGroupId": id }, callbackMethod, "json");
    }

    UpdateGlobalAttributeGroupDisplayOrder(globalAttributeGroupId: number, displayOrder: number, globalAttributeEntityId: number, callbackMethod) {
        super.ajaxRequest("/GlobalAttribute/UpdateAttributeGroupDisplayOrder", "post", { "globalAttributeGroupId": globalAttributeGroupId, "globalAttributeEntityId": globalAttributeEntityId, "displayOrder": displayOrder }, callbackMethod, "json");
    }

    GetAssociatedGlobalAttributes(attributeId: number, SuccessCallBack: any) {
        super.ajaxRequest("/GlobalAttribute/AssignedGlobalAttributes", "GET", { "attributeGroupId": attributeId }, SuccessCallBack, "html");
    }

    UnAssignGlobalAttributeGroup(globalattributeGroupId: number, entityId: number, callbackMethod: any) {
        super.ajaxRequest("/GlobalAttribute/UnAssignAttributeGroups", "get", { "groupId": globalattributeGroupId, "entityId": entityId }, callbackMethod, "json");
    }

    DeleteDownloadablePrductKey(pimDownloadableProductKeyId: string, callbackMethod) {
        super.ajaxRequest("/Inventory/DeleteDownloadableProductKeys", "get", { "PimDownloadableProductKeyId": pimDownloadableProductKeyId }, callbackMethod, "json");
    }

    GetTabStructure(entityId: number, callbackMethod) {
        super.ajaxRequest("/GlobalAttribute/GetTabStructure", "get", { "globalEntityId": entityId }, callbackMethod, "html");
    }

    IsGlobalAttributeValueUnique(attributeCodeValues: string, id: number, entityType: string, callbackMethod) {
        super.ajaxRequest("/GlobalAttribute/IsGlobalAttributeValueUnique", "get", { "AttributeCodeValues": attributeCodeValues, "Id": id, "EntityType": entityType }, callbackMethod, "json", false);
    }

    AssociatePortalToProfile(searchProfileId: any, portalIds: string, callbackMethod) {
        super.ajaxRequest("/Search/Search/AssociatePortalToSearchProfile", "post", { "searchProfileId": searchProfileId, "portalIds": portalIds }, callbackMethod, "json");
    }

    PublishSearchProfile(searchProfileId: any, callbackMethod) {
        super.ajaxRequest("/Search/Search/PublishSearchProfile", "get", { "searchProfileId": searchProfileId }, callbackMethod, "json");
    }

    UnAssociatePortalToSearchProfile(searchProfileId: any, portalSearchProfileId: string, callbackMethod) {
        super.ajaxRequest("/Search/Search/UnAssociatePortalToSearchProfile", "post", { "portalSearchProfileId": portalSearchProfileId, "searchProfileId": searchProfileId }, callbackMethod, "json");
    }

    AssociateSearchAttributesToProfile(searchProfileId: number, attributeCode: string, callbackMethod) {
        super.ajaxRequest("/Search/Search/AssociateAttributesToProfile", "post", { "searchProfileId": searchProfileId, "attributeCode": attributeCode }, callbackMethod, "json");
    }

    UnAssociateSearchAttributesFromProfile(searchProfilesAttributeMappingId: string, callbackMethod) {
        super.ajaxRequest("/Search/Search/UnAssociateAttributesFromProfile", "post", { "searchProfileAttributeMappingId": searchProfilesAttributeMappingId }, callbackMethod, "json");
    }

    SetFeatureByQueyId(queryId: string, callbackMethod: any) {
        super.ajaxRequest("/Search/Search/GetFeaturesByQueryId", "get", { "queryId": queryId }, callbackMethod, "html");
    }

    DeleteFormBuilder(formBuilderId: string, callbackMethod) {
        super.ajaxRequest("/FormBuilder/Delete", "get", { "formBuilderId": formBuilderId }, callbackMethod, "json");
    }

    IsFormCodeExist(formCode: string, callbackMethod) {
        super.ajaxRequest("/FormBuilder/IsFormCodeExist", "get", { "formCode": formCode }, callbackMethod, "json", false);
    }

    UpdateAttributeDisplayOrder(model: Object, callbackMethod) {
        super.ajaxRequest("/FormBuilder/UpdateAttributeDisplayOrder", "post", { "model": model }, callbackMethod, "json");
    }

    UpdateGroupDisplayOrder(model: Object, callbackMethod) {
        super.ajaxRequest("/FormBuilder/UpdateGroupDisplayOrder", "post", { "model": model }, callbackMethod, "json");
    }

    UnAssignAttribute(formBuilderId: number, attributeId: number, callbackMethod) {
        super.ajaxRequest("/FormBuilder/UnAssignAttribute", "get", { "formBuilderId": formBuilderId, "attributeId": attributeId }, callbackMethod, "json");
    }

    ManageSearchWidgetConfiguration(mappingId, widgetId, widgetKey, mappingType, displayName, widgetName, fileName, localeId: number, callbackMethod) {
        super.ajaxRequest("/WebSite/ManageSearchWidgetConfiguration", "get", { "mappingId": mappingId, "widgetId": widgetId, "widgetKey": widgetKey, "mappingType": mappingType, "displayName": displayName, "widgetName": widgetName, "fileName": fileName, "localeId": localeId }, callbackMethod, "html");
    }

    UnAssignGroup(formBuilderId: number, groupId: number, callbackMethod) {
        super.ajaxRequest("/FormBuilder/UnAssignGroup", "get", { "formBuilderId": formBuilderId, "groupId": groupId }, callbackMethod, "json");
    }

    GetFormBuilderAttributeGroup(formBuilderId: number, callbackMethod) {
        super.ajaxRequest("/FormBuilder/AssignedAttributeGroupList", "get", { "id": formBuilderId }, callbackMethod, "html");
    }

    ManageFormWidgetConfiguration(mappingId, widgetId, widgetKey, mappingType, displayName, widgetName, fileName, localeId: number, callbackMethod) {
        super.ajaxRequest("/WebSite/ManageFormWidgetConfiguration", "get", { "mappingId": mappingId, "widgetId": widgetId, "widgetKey": widgetKey, "mappingType": mappingType, "displayName": displayName, "widgetName": widgetName, "fileName": fileName, "localeId": localeId }, callbackMethod, "html");
    }


    IsShowChangePasswordPopup(callbackMethod) {
        super.ajaxRequest("/User/IsShowChangePasswordPopup", "get", {}, callbackMethod, "json");
    }

    SaveInCookie(callbackMethod) {
        super.ajaxRequest("/User/SaveInCookie", "get", {}, callbackMethod, "json");
    }

    ProductUpdateImportPost(importModel: any, callbackMethod) {
        super.ajaxRequest("/PIM/Products/UpdateProducts", "post", { "importModel": importModel }, callbackMethod, "json");
    }

    DownloadProductUpdateTemplate(callbackMethod) {
        super.ajaxRequest("/PIM/Products/DownloadProductTemplate", "post", {}, callbackMethod, "json");
    }
    AddCustomShippingAmount(customShippingCost: any, estimateShippingCost: any, userId: number, isQuote: boolean, callBackMethod) {
        super.ajaxRequest("/Order/AddCustomShippingAmount", "post", {
            "customShippingCost": customShippingCost, "estimateShippingCost": estimateShippingCost, "userId": userId, "isQuote": isQuote}, callBackMethod, "json", false);
    }

    GetStates(countryCode, callbackMethod) {
        super.ajaxRequest("/Customer/GetStates", "get", { "countryCode": countryCode }, callbackMethod, "json");
    }

    CheckCodeExists(url: string, code: string, callbackMethod) {
        super.ajaxRequest(url, "get", { "codeField": code }, callbackMethod, "json", false);
    }

    GetInventoryDetailBySKU(sku, callbackMethod) {
        super.ajaxRequest("/Inventory/GetInventoryDetail", "get", { "sku": sku }, callbackMethod, "json");
    }

    CheckUniqueBrandCode(code: string, callbackMethod) {
        super.ajaxRequest("/PIM/Brand/CheckUniqueBrandCode", "get", { "code": code }, callbackMethod, "json");
    }

    GetBrandName(code: string, localeId: number, callbackMethod) {
        super.ajaxRequest("/PIM/Brand/GetBrandName", "get", {
            "code": code, "localeid": localeId
        }, callbackMethod, "json");
    }

    GetFieldValueList(publishCatalogId: number, searchProfileId: number, callbackMethod: any) {
        super.ajaxRequest("/Search/Search/GetfieldValuesList", "get", { "publishCatalogId": publishCatalogId, "searchProfileId": searchProfileId }, callbackMethod, "html");
    }

    GetSearchRulesByCatalogId(publishCatalogId: string, catalogName: string, callbackMethod) {
        super.ajaxRequest("/Search/Search/GetBoostAndBuryRules", "get", { "catalogId": publishCatalogId, "catalogName": catalogName }, callbackMethod, "html");
    }

    PauseCatalogSearchRule(searchCatalogRuleId: string, isPause: boolean, callbackMethod) {
        super.ajaxRequest("/Search/Search/PauseCatalogSearchRule", "get", { "searchCatalogRuleId": searchCatalogRuleId, "isPause": isPause }, callbackMethod, "json");
    }

    DeleteCatalogSearchRule(ruleId: string, callbackMethod) {
        super.ajaxRequest("/Search/Search/DeleteCatalogSearchRule", "get", { "searchCatalogRuleId": ruleId }, callbackMethod, "json");
    }

    IsStoreCodeExist(storeCode: string, callbackMethod) {
        super.ajaxRequest("/Store/IsStoreCodeExist", "get", { "storeCode": storeCode }, callbackMethod, "json", false);
    }
    GetAutoSuggestion(query: string, fieldName: string, publishCatalogId: string, callbackMethod) {
        super.ajaxRequest("/Search/Search/GetAutoSuggestion", "get", { "query": query, "fieldName": fieldName, "publishCatalogId": publishCatalogId }, callbackMethod, "json");
    }

    EnableDisablePublishStateMapping(id: string, isEnable: boolean, callbackMethod) {
        super.ajaxRequest("/GeneralSetting/EnableDisablePublishStateMapping", "put", { "publishStateMappingId": id, "isEnabled": isEnable }, callbackMethod, "json");
    }

    GetAssociatedCatalog(pimProductId: number, callback) {
        super.ajaxRequest("/PIM/Catalog/GetAssociatedCatalog", "get", { pimProductId: pimProductId }, callback, "json");
    }

    DashboardLowInventoryProductCountOnSelectedPortal(portalId: number, callbackMethod) {
        super.ajaxRequest("/Dashboard/GetDashboardLowInventoryProductCount", "get", { "portalId": portalId }, callbackMethod, "json");
    }

    TestEmail(portalId: string, callbackMethod) {
        super.ajaxRequest("/Store/TestEmail", "get", { "portalId": portalId }, callbackMethod, "html");
    }

    GetLevelList(callbackMethod) {
        super.ajaxRequest("/Account/GetLevelList", "get", {}, callbackMethod, "json");
    }

    DeleteAreaMapping(id: string, callbackMethod) {
        super.ajaxRequest("/Account/DeleteApproverLevel", "delete", { "userApproverId": id }, callbackMethod, "json");
    }

    GetApprovalList(portalId: number, selectedApprovalType: string, selectedApprovalTypeId: number, callbackMethod) {
        super.ajaxRequest("/Store/GetApprovalList", "get", {
            "portalId": portalId, "selectedApprovalType": selectedApprovalType, "selectedApprovalTypeId": selectedApprovalTypeId
        }, callbackMethod, "json");
    }

    GetApproverUsersByName(searchTerm: string, portalId: number, accountId: number, approvalUserIds: string, callbackMethod) {
        super.ajaxRequest("/Account/GetApproverUsersByName", "get", { "searchTerm": searchTerm, "portalId": portalId, "accountId": accountId, "approvalUserIds": approvalUserIds }, callbackMethod, "json");
    }

    GetApproverLevelList(userId: number, callbackMethod) {
        super.ajaxRequest("/Account/GetApproverLevelList", "get", { "userId": userId }, callbackMethod, "html", false);
    }

    GetApproverUsersByPortalId(searchTerm: string, portalId: number, approvalUserIds: string, callbackMethod) {
        super.ajaxRequest("/Store/GetApproverUsersByPortalId", "get", { "searchTerm": searchTerm, "portalId": portalId, "approvalUserIds": approvalUserIds }, callbackMethod, "json");
    }

    GetPortalApproverDetails(portalId: number, callbackMethod) {
        super.ajaxRequest("/Store/GetPortalApproverDetailsById", "get", { "portalId": portalId }, callbackMethod, "html", false);
    }

    UpdateBillingAccountNumber(userId: number, callbackMethod) {
        super.ajaxRequest("/User/UpdateBillingNumber", "get", { "userId": userId }, callbackMethod, "html");
    }

    SaveReportLayout(reportName: any, reportCode: any, callBackMethod) {
        super.ajaxRequest("/DevExpressReport/SaveReportLayout", "post", { "reportName": reportName, "reportCode": reportCode }, callBackMethod, "json", false);
    }

    LoadSavedReportLayout(reportName: any, reportCode: any, callBackMethod) {
        super.ajaxRequest("/DevExpressReport/LoadSavedReportLayout", "post", { "reportName": reportName, "reportCode": reportCode }, callBackMethod, "json", false);
    }
    fnLoadReportcomponents(reportName: any, reportCode: any, callBackMethod) {
        super.ajaxRequest("/DevExpressReport/LoadSavedReportLayout", "post", { "reportName": reportName, "reportCode": reportCode }, callBackMethod, "json", false);
    }
    fnDeleteSavedReportLayout(reportviewid: any, callBackMethod) {
        super.ajaxRequest("/DevExpressReport/DeleteSavedReportLayout", "post", { "reportviewid": reportviewid }, callBackMethod, "json", false);
    }
    GetPortalList(type: any, callBackMethod) {
        super.ajaxRequest("/Typeahead/GetSuggestions", "get", { "type": type }, callBackMethod, "json", false);
    }

    UpdateProductDisplayOrder(catalogAssociation: Object, callbackMethod) {
        super.ajaxRequest("/PIM/Catalog/UpdateProductDisplayOrder", "post", { "catalogAssociationViewModel": catalogAssociation }, callbackMethod, "json");
    }

    GetApproverOrder(portalApprovalId: number, callbackMethod) {
        super.ajaxRequest("/Store/GetApproverOrder", "get", { "portalApprovalId": portalApprovalId }, callbackMethod, "json");
    }

    DeletePortalAreaMapping(id: string, callbackMethod) {
        super.ajaxRequest("/Store/DeletePortalApproverUser", "delete", { "userApproverId": id }, callbackMethod, "json");
    }

    GetPaymentApproverOrder(portalId: number, count: number, paymentIds: string, callbackMethod) {
        super.ajaxRequest("/Store/GetPaymentApproverOrder", "get", { "portalId": portalId, "paymentdivcount": count, "paymentIds": paymentIds }, callbackMethod, "json");
    }
    GenerateOrderNumber(portalId, callbackMethod) {
        super.ajaxRequest("/Order/GenerateOrderNumber", Constant.GET, { "portalId": portalId }, callbackMethod, "json", false);
    }
    IsUserNameAnExistingShopper(userName, callbackMethod) {
        super.ajaxRequest("/User/IsUserNameAnExistingShopper", Constant.GET, { "userName": userName }, callbackMethod, "json", false);
    }
    IsUserNameExists(userName: string, portalId: number, userId: number, callbackMethod) {
        super.ajaxRequest("/Customer/IsUserNameExists", "get", {
            "userName": userName, "portalId": $("#PortalId").val(), "userId": userId,
        }, callbackMethod, "json");
    }

    GetUserList(portalId: number, portalName: string, callbackMethod) {
        super.ajaxRequest("/Customer/CustomersList", "get", { "portalId": portalId, "portalName": portalName }, callbackMethod, "html", true);
    }

    GetProductList(pimCatalogId: number, catalogName: string, callbackMethod) {
        super.ajaxRequest("/PIM/Products/List", "get", { "pimCatalogId": pimCatalogId, "catalogName": catalogName }, callbackMethod, "html", true);
    }

    GetOrderList(userId: number, accountId: number, portalId: number, portalName: string, callbackMethod) {
        super.ajaxRequest("/Order/List", "get", { "userId": userId, "accountId": accountId, "portalId": portalId, "portalName": portalName }, callbackMethod, "html", true);
    }

    GetCategoryList(pimCatalogId: number, catalogName: string, callbackMethod) {
        super.ajaxRequest("/PIM/Category/List", "get", { "pimCatalogId": pimCatalogId, "catalogName": catalogName }, callbackMethod, "html", true);
    }

    GetImpersonateURL(portalId: number, userId: number, callbackMethod) {
        super.ajaxRequest("/Customer/GetImpersonationUrl", "get", { "portalId": portalId, "userId": userId }, callbackMethod, "json");
    }

    EnableDisableSalesRepAccount(id: string, isEnable: boolean, callbackMethod) {
        super.ajaxRequest("/SalesRep/EnableDisableAccount", "get", { "userId": id, "isLock": isEnable }, callbackMethod, "html");
    }

    UserResetSalesRepPassword(id: string, callbackMethod) {
        super.ajaxRequest("/SalesRep/BulkResetPassword", "get", { "userId": id }, callbackMethod, "html");
    }

    SetCustomerAddress(addressId: number, otherAddressId: number, addressType: string, isB2BCustomer: boolean, userId: number, portalId: number, accountId: number, isQuote: boolean, callbackMethod) {
        super.ajaxRequest("/Order/GetAddressDetails", "get", { "userAddressId": addressId, "otherAddressId": otherAddressId, "addressType": addressType, "isB2BCustomer": isB2BCustomer, "userId": userId, "portalId": portalId, "accountId": accountId, "isQuote": isQuote }, callbackMethod, "json");
    }

    GetUsersByPhoneNoOrUserName(searchTerm: string, portalId: number, callbackMethod) {
        super.ajaxRequest("/Order/GetCustomerListBySearchTerm", "get", { "searchTerm": searchTerm, "portalId": portalId }, callbackMethod, "json");
    }

    AssociateUsersWithAccount(userIds: string, accountId: number, callbackMethod) {
        super.ajaxRequest("/Account/AssociateUsersWithAccount", "post", { "userIds": userIds, "accountId": accountId }, callbackMethod, "json", false);
    }

    GetAssociateUsers(accountId: number, callbackMethod) {
        super.ajaxRequest("/Account/CustomersList", Constant.GET, { "accountId": accountId }, callbackMethod, "html");
    }
    GetShoppingCart(userId: number, portalId: number, orderId: number, isQuote: boolean, callbackMethod) {
        super.ajaxRequest("/Order/GetShoppingCartItems", "get", { "userId": userId, "portalId": portalId, "orderId": orderId, isQuote: isQuote }, callbackMethod, "json");
    }

    GetTopKeywordsReport(portalId: number, portalName: string, callbackMethod) {
        super.ajaxRequest("/Search/SearchReport/GetTopKeywordsReport", "get", { "portalId": portalId, "portalName": portalName }, callbackMethod, "html", true);
    }

    GetNoResultsFoundReport(portalId: number, portalName: string, callbackMethod) {
        super.ajaxRequest("/Search/SearchReport/GetNoResultsFoundReport", "get", { "portalId": portalId, "portalName": portalName }, callbackMethod, "html", true);
    }

    CalculateShoppingCart(userId: number, portalId: number, orderId: number, isQuote: boolean, callbackMethod) {
        super.ajaxRequest("/Order/CalculateShoppingCart", "get", {
            "userId": userId, "portalId": portalId, "orderId": orderId, "isQuote": isQuote }, callbackMethod, "json", true, false);
    }

    GetPaymentMethods(portalId, userId, callbackMethod) {
        super.ajaxRequest("/Order/GetPaymentMethods", "get", { "portalId": portalId, "userId": userId }, callbackMethod, "html");
    }

    GetCartCount(userId: number, portalId: number, callbackMethod) {
        super.ajaxRequest("/Order/GetCartCount", "get", { "userId": userId, "portalId": portalId }, callbackMethod, "json");
    }

    GetCmsSearchConfiguration(portalId: number, storeName: string, callbackMethod) {
        super.ajaxRequest("/CMSSearchConfiguration/CreateIndex", "get", { "portalId": portalId, "storeName": storeName }, callbackMethod, "html");
    }

    GetCmsSearchIndexMonitorList(cmsSearchIndexId: number, portalId: number, callbackMethod) {
        super.ajaxRequest("/CMSSearchConfiguration/GetCmsPageSearchIndexMonitor", "get", { "cmsSearchIndexId": cmsSearchIndexId, "portalId": portalId }, callbackMethod, "html", false);
    }

    AssociateCategoryToCatalog(categoryAssociationModel: Object, callbackMethod) {
        super.ajaxRequest("/PIM/Catalog/AssociateCategoryToCatalog", "post", { "catalogAssociationViewModel": categoryAssociationModel }, callbackMethod, "json");
    }

    GetQuoteList(portalId: number, portalName: string, callbackMethod) {
        super.ajaxRequest("/Quote/QuoteList", "get", { "portalId": portalId, "portalName": portalName }, callbackMethod, "html", true);
    }

    GetQuoteStateValueById(omsQuoteStateId: number, callbackMethod) {
        super.ajaxRequest("/Quote/GetQuoteStateValueById", "get", { "omsQuoteStateId": omsQuoteStateId }, callbackMethod, "json", false)
    }

    GetQuoteShippingList(omsQuoteId: number, callbackMethod) {
        super.ajaxRequest("/Quote/GetQuoteShippingList", "get", { "omsQuoteId": omsQuoteId }, callbackMethod, "json");
    }

    CalculateShippingInManage(shippingId: number, omsQuoteId: number, callBackMethod) {
        super.ajaxRequest("/Quote/CalculateShippingInManage", "post", { "shippingId": shippingId, "omsQuoteId": omsQuoteId }, callBackMethod, "json")
    }
    SubmitOrderReturn(returnNumber, notes, callbackMethod) {
        super.ajaxRequest("/RMAReturn/SubmitOrderReturn", "post", { "returnNumber": returnNumber, "notes": notes }, callbackMethod, "json");
    }

    UpdateOrderReturnLineItem(orderReturnLineItemModel, returnNumber, callbackMethod) {
        super.ajaxRequest("/RMAReturn/UpdateOrderReturnLineItem", "post", { "orderReturnLineItemModel": orderReturnLineItemModel, "returnNumber": returnNumber }, callbackMethod, "json", false);
    }

    GetReturnList(portalId: number, portalName: string, callbackMethod) {
        super.ajaxRequest("/RMAReturn/List", "get", { "portalId": portalId, "portalName": portalName }, callbackMethod, "html", true);
    }


    UpdateOrderReturnStatus(returnStatusCode: number, returnNumber: string, callbackMethod) {
        super.ajaxRequest("/RMAReturn/UpdateOrderReturnStatus", "post", { "returnStatusCode": returnStatusCode, "returnNumber": returnNumber }, callbackMethod, "json", false);
    }

    DeleteQuoteCartItem(omsQuoteId: number, guid: string, callbackMethod) {
        super.ajaxRequest("/Quote/RemoveQuoteCartItem", "post", { "omsQuoteId": omsQuoteId, "guid": guid }, callbackMethod, "json", true, false);
    }

    CalculateQuoteShoppingCart(omsQuoteId: number, callbackMethod) {
        super.ajaxRequest("/Quote/CalculateShoppingCart", "get", { "omsQuoteId": omsQuoteId }, callbackMethod, "json", true, false);
    }

    IsPaymentDisplayNameExists(paymentDisplayName: string, paymentSettingId: number, callbackMethod) {
        super.ajaxRequest("/Payment/IsPaymentDisplayNameExists", "post", { "paymentDisplayName": paymentDisplayName, "paymentSettingId": paymentSettingId }, callbackMethod, "json", false);
    }

    UpdateQuoteCartItem(_quoteLineItemDetail: Znode.Core.QuoteLineItemModel, callbackMethod) {
        super.ajaxRequest("/Quote/UpdateQuoteCartItem", "post", { "quoteDataModel": _quoteLineItemDetail }, callbackMethod, "json");
    }

    UpdateTaxExemptForQuote(omsQuoteId: number, isTaxExempt: boolean, callbackMethod) {
        super.ajaxRequest("/Quote/UpdateTaxExemptForManage", "post", { "omsQuoteId": omsQuoteId, "isTaxExempt": isTaxExempt }, callbackMethod, "html");
    }

    PrintManangeQuote(omsQuoteId: number, callbackMethod) {
        super.ajaxRequest("/Quote/PrintManageQuote", "get", { "omsQuoteId": omsQuoteId }, callbackMethod, "json")
    }

    UpdateManageQuote(omsQuoteId: number, additionalNotes: string, callbackMethod) {
        super.ajaxRequest("/Quote/UpdateQuote", "post", { "omsQuoteId": omsQuoteId, "additionalNote": additionalNotes }, callbackMethod, "json");
    }
    UpdateAccountManageQuote(omsQuoteId: number, AdditionalNotes: string, OmsOrderStateId: string, callbackMethod) {
        super.ajaxRequest("/Account/UpdateAccountQuote", "post", { "omsQuoteId": omsQuoteId, "AdditionalNotes": AdditionalNotes, "OmsOrderStateId": OmsOrderStateId}, callbackMethod, "html");
    }

    CreateQuoteRequest(portalId: number, callbackMethod) {
        super.ajaxRequest("/Quote/CreateQuoteRequest", "get", { "portalId": portalId }, callbackMethod, "html");
    }

    PrintReturnReceipt(returnNumber, callbackMethod) {
        super.ajaxRequest("/RMAReturn/PrintReturn", Constant.GET, { "returnNumber": returnNumber }, callbackMethod, "html", false);
    }

    ClearAllPublishedData(callbackMethod) {
        super.ajaxRequest("/Diagnostics/Maintenance/PurgeAllPublishedData", "delete", {}, callbackMethod, "json");
    }

    PublishBlogNewsPage(blogNewsId: string, targetPublishState: string, localeId: number, callbackMethod) {
        super.ajaxRequest("/BlogNews/PublishBlogNewsPage", "post", { "blogNewsId": blogNewsId, "localeId": localeId, "targetPublishState": targetPublishState, "takeFromDraftFirst": true }, callbackMethod, "json");
    }

    GetPaymentOptions(portalId, userId, callbackMethod) {
        super.ajaxRequest("/Quote/GetPaymentMethods", "get", { "portalId": portalId, "userId": userId }, callbackMethod, "html");
    }

    SaveAndConvertQuoteToOrder(_convertQuoteToOrderViewModel: Znode.Core.ConvertQuoteToOrderViewModel, callbackMethod) {
        super.ajaxRequest("/Quote/SaveAndConvertQuoteToOrder", "post", { "convertToOrderViewModel": _convertQuoteToOrderViewModel }, callbackMethod, "json");
    }

    UpdateQuoteCartItemPrice(guid: string, unitPrice: any, productid: any, shippingid: any, isQuote: boolean, userId: any, callbackMethod) {
        super.ajaxRequest("/Quote/UpdateQuoteCartItemPrice", "post", { "guid": guid, "unitPrice": unitPrice, "productId": productid, "shippingId": shippingid, "isQuote": isQuote, "userId": userId }, callbackMethod, "json", true, false);
    }

    ActivateDeactivateVouchers(voucherIds: string, isActive: boolean, callbackMethod) {
        super.ajaxRequest("/GiftCard/ActivateDeactivateVouchers", "post", { "giftCardId": voucherIds, "isActive": isActive }, callbackMethod, "json", true, false);
    }

    GetVoucherHistoryList(voucherId: number, portalId: number, callbackMethod) {
        super.ajaxRequest("/GiftCard/GetVoucherHistoryList", "get", { "voucherId": voucherId, "portalId": portalId }, callbackMethod, "html", true);
    }

    RemoveVoucher(url: string, voucher: string, callbackMethod: any) {
        super.ajaxRequest(url, "get", { "voucher": voucher }, callbackMethod, "json", true, false);
    }

    ApplyVoucher(url: string, giftCard: string, userId: number, callbackMethod: any) {
        super.ajaxRequest(url, "get", { "voucherNumber": giftCard, "userId": userId }, callbackMethod, "json", true, false);
    }

    GetAttributeList(entityId: number, entityType: string, callbackMethod) {
        super.ajaxRequest("/GlobalAttribute/List", "get", { "entityId": entityId, "entityType": entityType }, callbackMethod, "html", true);
    }

    GetAttributeGroupList(entityId: number, entityType: string, callbackMethod) {
        super.ajaxRequest("/GlobalAttributeGroup/List", "get", { "entityId": entityId, "entityType": entityType }, callbackMethod, "html", true);
    }

    GetAttributeFamilyList(entityId: number, entityType: string, callbackMethod) {
        super.ajaxRequest("/GlobalAttributeFamily/List", "get", { "entityId": entityId, "entityType": entityType }, callbackMethod, "html", true);
    }

    DeleteGlobalAttributeFamily(id: string, callbackMethod) {
        super.ajaxRequest("/GlobalAttributeFamily/Delete", "get", { "globalAttributeFamilyId": id }, callbackMethod, "json");
    }

    UpdateFamilyGroupDisplayOrder(groupCode: string, displayOrder: number, familyCode: string, callbackMethod) {
        super.ajaxRequest("/GlobalAttributeFamily/UpdateAttributeGroupDisplayOrder", "post", { "groupCode": groupCode, "familyCode": familyCode, "displayOrder": displayOrder }, callbackMethod, "json");
    }

    UnAssignGlobalAttributeGroupFromFamily(groupCode: string, familyCode: string, callbackMethod: any) {
        super.ajaxRequest("/GlobalAttributeFamily/UnassignAttributeGroups", "get", { "groupCode": groupCode, "familyCode": familyCode }, callbackMethod, "json");
    }

    GetTabStructureForAttributeFamily(familyCode: string, callbackMethod) {
        super.ajaxRequest("/GlobalAttributeFamily/GetTabStructure", "get", { "familyCode": familyCode }, callbackMethod, "html");
    }

    IsGlobalFamilyCodeExist(familyCode: string, callbackMethod) {
        super.ajaxRequest("/GlobalAttributeFamily/IsFamilyCodeExist", "get", { "familyCode": familyCode }, callbackMethod, "json", false);
    }

    UpdateAssociatedProducts(pimProductTypeAssociationId: string, relatedProductId: string, data: string, pimProductId: string, productId: string, callbackMethod) {
        super.ajaxRequest("/PIM/Products/UpdateAssociatedProducts", "post", { "pimProductTypeAssociationId": pimProductTypeAssociationId, "relatedProductId": relatedProductId, "data": data, "pimProductId": pimProductId, "productId": productId }, callbackMethod, "json", false);
    }

    IsContainerTemplateExist(code: string, callbackMethod) {
        super.ajaxRequest("/ContainerTemplate/IsContainerTemplateExist", "get", { "code": code }, callbackMethod, "json", false);
    }

    GetUnassociatedProfileList(containerKey: string, callbackMethod) {
        super.ajaxRequest("/ContentContainer/GetUnassociatedProfiles", "get", { "containerKey": containerKey }, callbackMethod, "json", false);
    }


    GetAssociatedVariants(entityId: number, entityType: string, callbackMethod) {
        super.ajaxRequest("/ContentContainer/GetEntityAttributeDetails", "get", { "entityId": entityId, "entityType": entityType }, callbackMethod, "html", false, false);
    }

    DeleteContentContainer(contentContainerId: string, callbackMethod) {
        super.ajaxRequest("/ContentContainer/Delete", "get", { "contentContainerId": contentContainerId }, callbackMethod, "json");
    }

    DeleteAssociatedVariant(containerProfileVariantId: string, containerKey: string, callbackMethod) {
        super.ajaxRequest("/ContentContainer/DeleteAssociatedVariant", "post", { "containerProfileVariantId": containerProfileVariantId, "containerKey": containerKey }, callbackMethod, "json", false);
    }

    AssociateWidgetTemplate(variantId: number, containerTemplateId: number,callbackMethod) {
        super.ajaxRequest("/ContentContainer/AssociateContainerTemplate", "put", { "variantId": variantId, "containerTemplateId": containerTemplateId}, callbackMethod, "json", false);
    }

    IsContainerExist(containerKey: string, callbackMethod) {
        super.ajaxRequest("/ContentContainer/IsContainerExist", "get", { "containerKey": containerKey }, callbackMethod, "json", false);
    }

    DeleteWidgetTemplate(containerTemplateId: string, fileName: string, callbackMethod) {
        super.ajaxRequest("/ContainerTemplate/Delete", "get", { "ContainerTemplateId": containerTemplateId, "fileName": fileName }, callbackMethod, "json");
    }

    CalculateOrderReturn(calculateOrderReturnModel, callbackMethod) {
        super.ajaxRequest("/RMAReturn/CalculateOrderReturn", "post", { "calculateOrderReturnModel": calculateOrderReturnModel }, callbackMethod, "json", false);
    }

    SubmitCreateReturn(orderReturnModel, callbackMethod) {
        super.ajaxRequest("/RMAReturn/SubmitCreateReturn", "post", { "returnViewModel": orderReturnModel }, callbackMethod, "json", false);
    }

    IsValidReturnItem(orderReturnModel, callbackMethod) {
        super.ajaxRequest("/RMAReturn/IsValidReturnItems", "post", { "returnViewModel": orderReturnModel }, callbackMethod, "json", false);
    }

    PrintCreateReturnReceipt(returnNumber, callbackMethod) {
        super.ajaxRequest("/RMAReturn/PrintCreateReturnReceipt", Constant.GET, { "returnNumber": returnNumber }, callbackMethod, "html", false);
    }

    DeleteSeo(seoTypeId: number, portalId: number, SEOCode: string, callbackMethod) {
        super.ajaxRequest("/Seo/DeleteSeoDetail", "get", { "seoTypeId": seoTypeId, "portalId": portalId, "seoCode": SEOCode}, callbackMethod, "json");
    }

    GetGlobalAttributesForDefaultVariantData(familyCode: string, entityType: string, callbackMethod) {
        super.ajaxRequest("/ContentContainer/GetGlobalAttributesForDefaultData", "get", { "familyCode": familyCode, "entityType": entityType }, callbackMethod, "html", false, false);
    }

    EditAssociatedVariant(variantId: string, localeId: number, callbackMethod) {
        super.ajaxRequest("/ContentContainer/EditAssociatedVariant", "get", { "containerProfileVariantId": variantId, "localeId": localeId }, callbackMethod, "html", false);
    }

    GetVariantsList(containerKey, callbackMethod) {
        super.ajaxRequest("/ContentContainer/GetAssociatedVariantList", "get", { "containerKey": containerKey }, callbackMethod, "html", true, false);
    }

    GetAssociatedVariantData(variantId: number, localeId: number, callbackMethod) {
        super.ajaxRequest("/ContentContainer/GetAttributesDataOnLocaleChange", "get", { "variantId": variantId, "localeId": localeId }, callbackMethod, "json", true, true);
    }

    ActivateDeactivateVariant(containerProfileVariantIds: string, isActivate: boolean, callbackMethod) {
        super.ajaxRequest("/ContentContainer/ActivateDeactivateVariant", "post", { "containerProfileVariantIds": containerProfileVariantIds, "isActivate": isActivate }, callbackMethod, "json", false);
    }

    PublishContentContainer(containerKey: string, targetPublishState: string, callbackMethod) {
        super.ajaxRequest("/ContentContainer/PublishContentContainer", "get", {
            "containerKey": containerKey, "targetPublishState": targetPublishState
        }, callbackMethod, "json");
    }

    PublishContentContainerVariant(containerKey: string, containerProfileVariantId: number, targetPublishState: string, callbackMethod) {
        super.ajaxRequest("/ContentContainer/PublishContainerVariant", "get", {
            "containerKey": containerKey, "containerProfileVariantId": containerProfileVariantId, "targetPublishState": targetPublishState
        }, callbackMethod, "json");
    }

    SaveContainerDetails(configurationModel, callbackMethod) {
        super.ajaxRequest("/WebSite/SaveContainerDetails", "post", { "configurationViewModel": configurationModel}, callbackMethod, "json",false);
    }
    GetDateFormatGlobalSetting(callbackMethod: any) {
        super.ajaxRequest("/GeneralSetting/GetDateFormatGlobalSetting", "get", {}, callbackMethod, "json");
    }

    GetTimeFormatGlobalSetting(callbackMethod: any) {
        super.ajaxRequest("/GeneralSetting/GetTimeFormatGlobalSetting", "get", {}, callbackMethod, "json");
    }

    GetProviderTypeForm(providerName: string, portalId: number, currentProviderId :string, callbackMethod) {
        super.ajaxRequest("/Store/GetProviderTypeForm", "post", { "providerName": providerName, "portalId": portalId, "currentProviderId": currentProviderId, }, callbackMethod, "html");
    }

    UpdateExistingUserName(userDetailsViewModel: Znode.Core.UserDetailsViewModel, callbackMethod) {
        super.ajaxRequest("/Customer/UpdateUsernameForRegisteredUser", "put", { "userDetailsViewModel": userDetailsViewModel }, callbackMethod, "json", false);
    }

    GetAuthorizeNetToken(paymentTokenModel, callbackMethod) {
        super.ajaxRequest("/order/GetAuthorizeNetToken", "post", { "paymentTokenModel": paymentTokenModel }, callbackMethod, "json");

    }

    GetPaymentGatewayToken(paymentTokenModel, callbackMethod: any) {
        super.ajaxRequest("/Order/GetPaymentGatewayToken", "post", { "paymentTokenModel": paymentTokenModel }, callbackMethod, "json");
    }

    CheckFileNameExist(localeId: number, fileName: string, callBackMethod) {
        super.ajaxRequest("/ProductFeed/IsFileNameCombinationAlreadyExist", "get", { "localeId": localeId, "fileName": fileName }, callBackMethod, "json");
    }

    GetProviderTypeKlaviyoForm(providerName: string, portalId: number, currentProviderId: string, callbackMethod) {
        super.ajaxRequest("/Store/GetProviderTypeKlaviyoForm", "post", { "providerName": providerName, "portalId": portalId, "currentProviderId": currentProviderId, }, callbackMethod, "html");
    }

    FormSubmissionExport(exportType: string, callbackMethod) {
        super.ajaxRequest("/Export/ExportFormSubmission", "get", { "exportType": exportType }, callbackMethod, "json");
    }

    GetIframeViewWithToken(paymentTokenModel, partialView, callbackMethod) {
        super.ajaxRequest("/order/GetIframeViewWithToken", "post", { "paymentTokenModel": paymentTokenModel, "partialView": partialView }, callbackMethod, "json");
    }

    DeleteImportTemplate(importTemplateId: string, callbackMethod) {
        super.ajaxRequest("/Import/DeleteImportTemplate", "get", { "importTemplateId": importTemplateId }, callbackMethod, "json");
    }
};
