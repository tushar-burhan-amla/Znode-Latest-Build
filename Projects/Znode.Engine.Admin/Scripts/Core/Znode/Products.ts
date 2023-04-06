var _data: any;
var addonId: number;
var selectedTab: string;
var proInfoTab: string;
var validateForm = true;

declare function reInitializationMce(): any;

class Products extends ZnodeBase {
    _Model: any;
    isAddOnGroupValid: boolean = true;
    isAddonProductDisplayOrderValid = true;
    addonProductDisplayOrderError: string = "";
    hdnPimCatalogIdForCatalog: number;
    hdnCategoryIdForCatalog: number;
    constructor() {
        super();
    }
    Init() {
        var selectedTabGroupCode = Products.prototype.GetParameterValues('selectedtab');
        Products.prototype.RemoveProductTypeBlankOption();
        Products.prototype.hdnPimCatalogIdForCatalog = $("#hdnPimCatalogIdForCatalog").val();
        Products.prototype.hdnCategoryIdForCatalog = $("#hdnCategoryIdForCatalog").val();
        $(document).on("change", "#txtUpload", function () {
            Import.prototype.ValidateImportedFileType();
        });
        Products.prototype.CallAttributeFamilyAssociatedProduct();
        $(".ProductInfo a").addClass("active");
        $(".groupPannel[data-groupcode=" + selectedTabGroupCode + "]").click();
        Products.prototype.GetPersonalizedAttributes($("#ProductId").val() ? parseInt($("#ProductId").val(), 10) : 10);
        Products.prototype.MultiSelectDropDown();
        $(document).on("ROW_PrevEDIT", function (e) {
            $('.z-update.z-ok').off('click');
            $('.z-cancel').on('click', function (e) {
                $('.field-validation-error').html('');
            });
            $('.z-update.z-ok').on('click', function (e) {
                if ($(this).closest('.grid-action').parent().find('input[data-columnname="DownloadableProductKey"]').length > 0)
                    return true;
                var object = $(this).closest('.grid-action').parent().find('input.input-text');
                validateForm = Products.prototype.ValidateDisplayOrderField(object);
            });
        });
    }

    MultiSelectDropDown() {
        $(".MultiSelectClass").fastselect();
    }

    //Method Show Get Parameter Values
    GetParameterValues(param) {
        var url = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
        for (var i = 0; i < url.length; i++) {
            var urlparam = url[i].split('=');
            if (urlparam[0] == param) {
                return urlparam[1].replace(/#$/, "");
            }
        }
    }

    //Method Show Hide Custom Field
    ShowHideCustomField() {
        Products.prototype.GetAssociatedProductsSpan();
        $('#AssociatedProductsList').hide();
        $('#CustomFieldsList').hide();
        $('#DownloadableProductKeyList').hide();
        $('#AssociatedAddonList').hide();
        $('#InventoryDetails').hide();
        $(".tab-selected").click();
        if ((window.location.href.toLowerCase().indexOf('edit') == -1)) {
            $(".pimAtribute").hide();
            $(".pimAtribute").eq(0).show();
            $(".accordion-toggle").eq(0).removeClass("collapsed");
        }
        if (parseInt($("#ProductId").val(), 10) <= 0) {
            if (window.location.href.toLowerCase().indexOf('copy') > -1) {
                $(".ProductType").prop("disabled", true);
                $("#frmProduct").append("<input type='hidden' name= '" + $(".ProductType").attr("id") + "' value= '" + $(".ProductType").val() + "' />");
                $("#ddlfamily").prop("disabled", true);
                $("#frmProduct").append("<input type='hidden' name= '" + $("#ddlfamily").attr("id") + "' value= '" + $("#ddlfamily").val() + "' />");
            }
            else {
                $(".ProductType").prop("disabled", false);
                $("#ddlfamily").prop("disabled", false);
            }
            $("#ddlCulture").prop("disabled", true);
            $("#ddlCulture").addClass("disabled");
            $("input:radio[name*=IsActive].yes").prop("checked", true);
            $("#copyProduct").hide();
        }
        else {
            $("#PublishProductLink").show();
            $("#copyProduct").show();
            $(".ProductType").prop("disabled", true);
            $("#frmProduct").append("<input type='hidden' name= '" + $(".ProductType").attr("id") + "' value= '" + $(".ProductType").val() + "' />");
            if ($(".ProductType option:selected").val() == Constant.configurableProduct) {
                $("#ddlfamily").prop("disabled", true);
            }
        }
    }

    //Method for Assign Link Products
    AssignLinkProducts(attributeId: number, AssignedLinkProductsGrid) {
        ZnodeBase.prototype.ShowLoader();
        var parentProductId: number = parseInt($("#ProductId").val(), 10);
        var linkProductIds: string = DynamicGrid.prototype.GetMultipleSelectedIds('UnassociatedProducts');

        if (linkProductIds.length == 0) {
            $("#asidePannelmessageBoxContainerId").show();
            $("#UnassociatedProductAsidePannel").show();
            ZnodeBase.prototype.HideLoader();
            return false;
        } else {
            var model = { "ParentId": parentProductId, "AssociatedIds": linkProductIds, "AttributeId": attributeId };

            if (parentProductId > 0) {
                Endpoint.prototype.AssignLinkProducts(model, function (response) {
                    ZnodeBase.prototype.RemovePopupOverlay();
                    Products.prototype.GetAssignedLinkProducts(parentProductId, attributeId, AssignedLinkProductsGrid);
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, response.status ? "success" : "error", true, 5000);
                    $("#UnassociatedProductAsidePannel").hide();
                });
            } else {
                $("#AssignedLinkProducts").val(linkProductIds);
                $("#UnassociatedProductAsidePannel").hide();
            }
        }
        ZnodeBase.prototype.RemoveAsidePopupPanel();
    }

    //Method  Associate Products
    AssociateProducts(associatedProductGrid: string) {
        ZnodeBase.prototype.ShowLoader();
        var parentProductId: number = parseInt($("#ProductId").val(), 10);
        var associatedProductIds: string = DynamicGrid.prototype.GetMultipleSelectedIds();
        var attributeValue: number = $(".ProductType option:selected").val();
        var productSplitArray = $(".ProductType").attr("id").split('_');
        var attributeId: string = productSplitArray[1];
        var assoicatedProducts: string = "";
        if (associatedProductIds.length === 0) {
            ZnodeBase.prototype.HideLoader();
            $("#asidePannelmessageBoxContainerId").show();
            return false;
        }
        else if ($(".ProductType option:selected").val() == Constant.configurableProduct && $("#ddlfamily").val() > 0) {
            var model = { "AssociatedProductIds": associatedProductIds, "ParentProductId": parentProductId, "AttributeId": attributeId };
            if (parentProductId > 0) {
                Endpoint.prototype.AssociateProducts(model, function (data) {
                    if (data) {
                        $("#UnassociatedProductAsidePannel").hide();
                        var associatedAttributeIds = [];
                        $("#ConfigureAttributeCheckboxes").find('input').each(function () {
                            if ($(this).is(":checked")) {
                                associatedAttributeIds.push($(this).attr('id'));
                            }
                        });
                        Endpoint.prototype.GetAssociatedConfigureProducts(parseInt($("#ProductId").val(), 10), associatedAttributeIds.toString(), function (response) {
                            Products.prototype.ShowResponceOfAssociatedProductsList(response);
                            ZnodeBase.prototype.RemovePopupOverlay();
                            DynamicGrid.prototype.ClearCheckboxArray();
                            $("#UnassociatedProductAsidePannel").hide();
                            ZnodeBase.prototype.HideLoader();
                            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(data.message, data.status ? 'success' : 'error', isFadeOut, fadeOutTime);
                        });
                    }
                });
            }
            else {
                if ($("#AssociatedProductIds").val() != "")
                    assoicatedProducts = $("#AssociatedProductIds").val() + "," + associatedProductIds;
                else
                    assoicatedProducts = associatedProductIds;

                var associatedAttributeIds = [];
                $("#ConfigureAttributeCheckboxes").find('input').each(function () {
                    if ($(this).is(":checked")) {
                        associatedAttributeIds.push($(this).attr('id'));
                    }
                });
                $("#AssociatedProductIds").val(assoicatedProducts);
                Products.prototype.GetConfigureProductsToBeAssociated(assoicatedProducts, associatedAttributeIds.toString());
                $("#UnassociatedProductAsidePannel").hide();
                ZnodeBase.prototype.RemovePopupOverlay();
                ZnodeBase.prototype.HideLoader();
            }
        }
        else {
            var model = { "AssociatedProductIds": associatedProductIds, "ParentProductId": parentProductId, "AttributeId": attributeId };
            if (parentProductId > 0) {
                Endpoint.prototype.AssociateProducts(model, function (data) {
                    if (data) {
                        $("#UnassociatedProductAsidePannel").hide();
                        ZnodeBase.prototype.ShowLoader();
                        if ($(".ProductType option:selected").val() == Constant.bundleProduct && $("#ddlfamily").val() > 0) {
                            Endpoint.prototype.GetAssociatedBundleProducts(parseInt($("#ProductId").val(), 10), parseInt(attributeId, 10), function (response) {
                                Products.prototype.ShowResponceOfAssociatedProductsList(response);
                                ZnodeBase.prototype.RemovePopupOverlay();
                                DynamicGrid.prototype.ClearCheckboxArray();
                                ZnodeBase.prototype.HideLoader();
                            });
                        }
                        else {
                            Endpoint.prototype.GetAssociatedProducts(parseInt($("#ProductId").val(), 10), parseInt(attributeId, 10), function (response) {
                                Products.prototype.ShowResponceOfAssociatedProductsList(response);
                                ZnodeBase.prototype.RemovePopupOverlay();
                                DynamicGrid.prototype.ClearCheckboxArray();
                                ZnodeBase.prototype.HideLoader();
                                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(data.message, data.status ? 'success' : 'error', isFadeOut, fadeOutTime);

                            });
                        }
                    }
                });
            }
            else {
                if ($("#AssociatedProductIds").val() != "")
                    assoicatedProducts = $("#AssociatedProductIds").val() + "," + associatedProductIds;
                else
                    assoicatedProducts = associatedProductIds;

                $("#AssociatedProductIds").val(assoicatedProducts);
                $("#UnassociatedProductAsidePannel").hide();
                Products.prototype.GetProductsToBeAssociated(assoicatedProducts);
                ZnodeBase.prototype.RemovePopupOverlay();
                ZnodeBase.prototype.HideLoader();
            }
        }
        ZnodeBase.prototype.RemoveAsidePopupPanel();
    }

    //Method  for Show Hide Associated ProductTab
    ShowHideAssociatedProductTab(): any {
        if ($(".ProductType option:selected").val() != Constant.simpleProduct || $(".ProductType option:selected").val() == "") {
            $("#productDetails ul li").filter(".AssociatedProducts").show();
            Products.prototype.GetAssociatedProductsSpan();
        }
        $(".ProductType").on("change", function () {
            Products.prototype.ChangeCancelUrl();            
            $("#AssociatedProductIds").val("");
            if ($(".ProductType option:selected").val() == Constant.simpleProduct || $(".ProductType option:selected").val() == "") {
                $("#productDetails ul li").filter(".AssociatedProducts").hide();
                if ($("#ddlfamily").val() > 0) {
                    Products.prototype.GetFamilyAttribute();
                }
            }
            else {
                Products.prototype.GetAssociatedProductsSpan();
                $("#productDetails ul li").filter(".AssociatedProducts").show();
                if ($(".ProductType option:selected").val() == Constant.configurableProduct) {
                    Products.prototype.GetConfigurableAttributes();
                }
                else {
                    if ($("#ddlfamily option:selected").text() != "DefaultFamily") {
                        Products.prototype.GetFamilyByType($(".ProductType option:selected").val());
                    }
                }
            }
        });
    }

    // Method for Get Associated Products List
    GetAssociatedProductsList(): any {
        $('#associateProduct').show();
        if ($(".ProductType option:selected").val() == Constant.configurableProduct) {
            $('#associateProduct').hide();
            $('#associateConfigureProducts').show();
            Products.prototype.CreateConfigureAttributeCheckboxes();
        }
        else { $('#ConfigureAttributeCheckboxes').hide(); $('#ConfigureAttributeDiv').hide(); }
        ZnodeBase.prototype.ShowLoader();
        var productSplitArray: string[] = $(".ProductType").attr("id").split('_');
        var attributeId: string = productSplitArray[1];
        if ($(".ProductType option:selected").val() == Constant.configurableProduct && $("#ddlfamily").val() > 0) {
            var associatedAttributeIds = [];
            $("#ConfigureAttributeCheckboxes").find('input').each(function () {
                if ($(this).is(":checked")) {
                    associatedAttributeIds.push($(this).attr('id'));
                }
            });
            Endpoint.prototype.GetAssociatedConfigureProducts(parseInt($("#ProductId").val(), 10), associatedAttributeIds.toString(), function (response) {
                Products.prototype.ShowResponceOfAssociatedProductsList(response);
                $('#AssociateProductError').hide();
                DynamicGrid.prototype.ClearCheckboxArray();
                ZnodeBase.prototype.HideLoader();
            });
        }
        else if ($(".ProductType option:selected").val() == Constant.bundleProduct && $("#ddlfamily").val() > 0) {
            Endpoint.prototype.GetAssociatedBundleProducts(parseInt($("#ProductId").val(), 10), parseInt(attributeId, 10), function (response) {
                Products.prototype.ShowResponceOfAssociatedProductsList(response);
                $('#AssociateProductError').hide();
                DynamicGrid.prototype.ClearCheckboxArray();
                ZnodeBase.prototype.HideLoader();
            });
        }
        else {
            Endpoint.prototype.GetAssociatedProducts(parseInt($("#ProductId").val(), 10), parseInt(attributeId, 10), function (response) {
                Products.prototype.ShowResponceOfAssociatedProductsList(response);
                $('#AssociateProductError').hide();
                DynamicGrid.prototype.ClearCheckboxArray();
                ZnodeBase.prototype.HideLoader();
            });
        }
    }

    CloseAddProductInventoryPopup() {
        ZnodeBase.prototype.CancelUpload('divAssociateWarehoueseInventory');
    }

    SaveProductInventory() {
        var inventorySKUViewModel =
        {
            "SKU": $("#SKU").val(),
            "Quantity": $("input[type='text'][id$='Quantity']").val() ? $("input[type='text'][id$='Quantity']").val() : $("input[type='text'][id$='QuantityInventory']").val(),
            "ReOrderLevel": $("input[type='text'][id$='ReOrderLevel']").val(),
            "WarehouseId": $("#WarehouseId").val(),
            "InventoryId": $("#InventoryId").val(),
            "IsDownloadable": true
        };

        Endpoint.prototype.AddUpdateProductInventory(inventorySKUViewModel, function (response) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, response.status ? "success" : "error", isFadeOut, fadeOutTime);
            if (response.status) {
                Products.prototype.GetInventoryDetails();
            }
        });
    }

    ValidateSKU(): boolean {
        if ($.isFunction($.fn.getTierPriceData)) {
            $.fn.getTierPriceData();
        }
        var rowLength = $('#tierPriceQuantity tr').length;
        if (rowLength == 1) {
            if ($("#tierQuantity").val() != "" && $("#priceTier").val() == "") {
                $("#error-price").html(ZnodeBase.prototype.getResourceByKeyName("TierPriceIsRequired"));
                $("#error-quantity").html("");
                return false;
            } else if ($("#tierQuantity").val() == "" && $("#priceTier").val() != "") {
                $("#error-quantity").html(ZnodeBase.prototype.getResourceByKeyName("MinimumQuantityIsRequired"));
                $("#error-price").html("");
                return false;
            }
            else {
                $("#error-price").html("");
                $("#error-quantity").html("");
            }
        }
        else {
            if ($("#tierQuantity").val() != "" && $("#priceTier").val() == "") {
                $("#error-price").html(ZnodeBase.prototype.getResourceByKeyName("TierPriceIsRequired"));
                $("#error-quantity").html("");
                return false;
            } else if ($("#tierQuantity").val() == "" && $("#priceTier").val() != "") {
                $("#error-quantity").html(ZnodeBase.prototype.getResourceByKeyName("MinimumQuantityIsRequired"));
                $("#error-price").html("");
                return false;
            }
            else if ($("#tierQuantity").val() == "" && $("#priceTier").val() == "") {
                $("#error-price").html(ZnodeBase.prototype.getResourceByKeyName("TierPriceIsRequired"));
                $("#error-quantity").html(ZnodeBase.prototype.getResourceByKeyName("MinimumQuantityIsRequired"));
                return false;
            }
            else {
                $("#error-price").html("");
                $("#error-quantity").html("");
            }
        }

        var flag = true;
        if (Inventory.prototype.isSKUValid != undefined && !Inventory.prototype.isSKUValid) {
            Products.prototype.ShowErrorMessage(ZnodeBase.prototype.getResourceByKeyName("InvalidSKU"), $("#txtSKU"), $("#valSKU"));
            return flag = false;
        }
        else {
            Products.prototype.HideErrorMessage($("#txtSKU"), $("#valSKU"));
        }

        var mindate = new Date($("#PriceSKU_ActivationDate").val());
        var maxdate = new Date($("#PriceSKU_ExpirationDate").val());
        if ((mindate > maxdate)) {
            $("#spamDate").html(ZnodeBase.prototype.getResourceByKeyName("ErrorActivationDate"));
            flag = false;
        }
        return flag;
    }

    //Method for Create Configure Attribute Checkboxes
    CreateConfigureAttributeCheckboxes() {
        if ($("#ProductId").val() > 0) {
            Products.prototype.GetConfigureAttributeDetails(parseInt($("#ProductId").val(), 10));
        }
        else {
            var url = decodeURIComponent(window.location.href);
            var orignalUrl = url.split(/[?#]/)[0];
            if (orignalUrl.indexOf('Copy') > -1) {
                Products.prototype.GetConfigureAttributeDetails(parseInt(Products.prototype.GetParameterValues('PimProductId'), 10));
            }
        }
        $('#ConfigureAttributeCheckboxes').show();
        $('#ConfigureAttributeDiv').show();
    }

    //Method Get Configure Attribute Details
    GetConfigureAttributeDetails(ProductId: number): void {
        Endpoint.prototype.GetConfigureAttributeDetails($("#ddlfamily").val(), ProductId, function (response) {

            var obj = JSON.parse(response);
            if (obj.status === 'true') {
                var div = $('#ConfigureAttributeCheckboxes');
                div.html('');
                for (var i = 0; i < obj.data.length; i++) {
                    div.append("<label style='margin-right:10px;'>" +
                        "<input type='checkbox' class='configure-checkbox' name='" + obj.data[i].AttributeCode + "' id='" + obj.data[i].PimAttributeId + "' data-familyid='" + obj.data[i].PimAttributeFamilyId + "'>" +
                        "<span class='lbl padding-8'></span>" + obj.data[i].AttributeName + "</label>");
                    $("input:checkbox[id=" + obj.data[i].PimAttributeId + "]").prop("checked", obj.data[i].IsConfigurableAttribute);
                    if (obj.data[i].IsConfigurableAttribute) {
                        $("input:checkbox[id=" + obj.data[i].PimAttributeId + "]").prop('disabled', true);
                    }
                }
            }
        });
    }

    //Method Get Attribute Family Details
    GetAttributeFamilyDetails(): any {
        $("#ddlfamily").off("change");
        $("#ddlfamily").on("change", function () {
            if ($(".ProductType option:selected").val() == Constant.configurableProduct) {
                Products.prototype.GetConfigurableAttributes();
                return false;
            }
            Products.prototype.GetFamilyAttribute();
        });
        $("#hdnPimCatalogIdForCatalog").val(Products.prototype.hdnPimCatalogIdForCatalog);
        $("#hdnCategoryIdForCatalog").val(Products.prototype.hdnCategoryIdForCatalog);
        Products.prototype.ChangeCancelUrl();
        Products.prototype.RemoveProductTypeBlankOption();
        Products.prototype.MultiSelectDropDown();
    }

    //Method Get FamilyAttribute
    GetFamilyAttribute() {
        Endpoint.prototype.GetAttributeFamilyDetails($("#ddlfamily").val(), $("#ProductId").val(), function (response) {
            $("#productDetails").html(response);
            Products.prototype.CallAttributeFamilyAssociatedProduct();
            $.getScript("/Scripts/References/DynamicValidation.js");
            reInitializationMce();
            Products.prototype.GetPersonalizedAttributes(parseInt($("#ProductId").val(), 10));
            $(this).scrollTop(0);
            $('body, html').animate({ scrollTop: 0 }, 'fast');
        });
    }

    //Method Get family by product type.
    GetFamilyByType(selectedType) {
        Endpoint.prototype.GetAttributeFamilyDetails($("#ddlfamily").val(), $("#ProductId").val(), function (response) {
            $("#productDetails").html(response);
            $(".ProductType").val(selectedType);
            Products.prototype.CallAttributeFamilyAssociatedProduct();
            $.getScript("/Scripts/References/DynamicValidation.js");
            reInitializationMce();
            $(this).scrollTop(0);
            $('body, html').animate({ scrollTop: 0 }, 'fast');
        });
    }

    // Method to set the Return URL.
    MaintainReturnURL(returnUrl) {
        if (typeof (returnUrl) != "undefined")
            $.cookie("_backURL", returnUrl, { path: '/' });
        return true;
    }

    //Method for Show / hide group attribute on group selected
    ShowHideAttributes(): any {
        $(".groupPannel").off("click");
        $(".groupPannel").on("click", function () {
            $('#UnassociatedProductAsidePannel').html('');
            Products.prototype.SectionToHidecontrols();
            var groupCode = $(this).attr("data-groupcode");
            var groupId = $(this).attr("data-groupId");
            var groupType = $(this).attr("data-groupType");
            selectedTab = groupCode;
            $('li[data-groupcode=' + groupCode + ']').addClass('tab-selected');
            $('#' + groupCode).show();
            $('#lblProductHeading').text($(this).find('a').text());
            var _currentPageContents = $(this);
            if (_currentPageContents.attr('class').indexOf("Image") > 0) {
                $("#currentGroupCode").val(groupCode);
            }

            if (_currentPageContents.attr('class').indexOf("GalleryImages") > 0) {
                $("#currentGroupCode").val(groupCode);
            }
            if (_currentPageContents.attr('class').indexOf("Personalization") > 0 || _currentPageContents.attr('class').indexOf("CustomFields") > 0 || _currentPageContents.attr('class').indexOf("Downloadable") > 0) {
                ZnodeBase.prototype.ShowLoader();
                if (_currentPageContents.attr('class').indexOf("CustomFields") > 0) {
                    $("#AssignCutomAttributes").show();
                    $("#AssignPersonalizedAttribute").hide();
                    $("#AssignDownloadableKeys").hide();
                    Products.prototype.CustomFieldList();
                } else if (_currentPageContents.attr('class').indexOf("Personalization") > 0) {
                    $("#AssignCutomAttributes").hide();
                    $("#AssignDownloadableKeys").hide();
                    $("#AssignPersonalizedAttribute").show();
                    $("#Personalization").show();
                }
                else if (_currentPageContents.attr('class').indexOf("Downloadable") > 0) {
                    $("#AssignCutomAttributes").hide();
                    $("#AssignDownloadableKeys").show();
                    $("#AssignPersonalizedAttribute").hide();
                    Products.prototype.DownloadableProductKeyList();
                }
                ZnodeBase.prototype.HideLoader();
            }
            if (_currentPageContents.attr('class').indexOf("ProductInfo") > 0 || _currentPageContents.attr('class').indexOf(Constant.image) > 0
                || _currentPageContents.attr('class').indexOf(Constant.shippingSettings) > 0 || _currentPageContents.attr('class').indexOf(Constant.productSetting) > 0
                || _currentPageContents.attr('class').indexOf(Constant.productDetails) > 0) {
                $('.productInfoPage').show();
                $('.productInfoPage').each(function (index) {
                    if (!$(this).children().children('h4').hasClass('collapsed') && index != 0) {
                        $(this).children().children('h4').addClass("collapsed");
                        $(this).children().next('.panel-collapse').removeAttr('style');
                    }
                });

                if (window.location.href.toLowerCase().indexOf('edit') > -1) {
                    var url = decodeURIComponent(window.location.href);
                    if (url.indexOf('selectedtab') > -1) {
                        var _proInfoTabValue = ZnodeBase.prototype.getCookie("_proInfoTab");
                        if (_proInfoTabValue != undefined || _proInfoTabValue != "") {
                            if (_proInfoTabValue != "" && $("#" + _proInfoTabValue).val() != undefined) {
                                $('.panel-collapse').removeAttr('style');
                                Products.prototype.fnShowHide(undefined, ZnodeBase.prototype.getCookie("_proInfoTab"));
                            }
                            else {
                                $(".pimAtribute").eq(0).show();
                            }
                        }
                    }
                    else {
                        $(".pimAtribute").eq(0).show();
                    }
                }
                else {
                    $(".pimAtribute").eq(0).show();
                }
            }
            if (_currentPageContents.attr('class').indexOf("AssociatedProducts") > 0) {
                if (parseInt($("#ProductId").val(), 10) === 0 && $("#AssociatedProductIds").val().length > 0) {
                    $("#associateProduct").show();
                    if ($(".ProductType option:selected").val() == Constant.configurableProduct) {
                        $('#associateProduct').hide();
                        $('#associateConfigureProducts').show();
                        Products.prototype.CreateConfigureAttributeCheckboxes();
                        var associatedAttributeIds = [];
                        $("#ConfigureAttributeCheckboxes").find('input').each(function () {
                            if ($(this).is(":checked")) {
                                associatedAttributeIds.push($(this).attr('id'));
                            }
                        });
                        if (associatedAttributeIds.length == 0) {
                            associatedAttributeIds = $("#ConfigureAttributeIds").val();
                        }
                        Products.prototype.GetConfigureProductsToBeAssociated($("#AssociatedProductIds").val(), associatedAttributeIds.toString());
                    }
                    else { Products.prototype.GetProductsToBeAssociated($("#AssociatedProductIds").val()); }
                }
                else
                    Products.prototype.GetAssociatedProductsList();
            }
            else { $("#associateProduct").hide(); $('#ConfigureAttributeCheckboxes').hide(); $('#ConfigureAttributeDiv').hide(); }

            if (_currentPageContents.attr('class').indexOf(Constant.addOns) > 0) {
                $("#associateAddonGroups").show();
                //$("#addAddonGroup").show();
                Products.prototype.GetAssociatedAddonList(parseInt($("#ProductId").val(), 10));
            }

            if (_currentPageContents.attr('class').indexOf(Constant.inventory) > 0) {
                $("#btnAssociateWarehouse").show();
                $("#divSaveInventory").show();
                $('#Inventory1').show();
                Products.prototype.GetInventoryDetails();
            }

            if (_currentPageContents.attr('class').indexOf(Constant.category) > 0) {
                $("#divAssociateCategory").show();
                $("#divCreateCategory").show();
                $('#AssociateCategory').show();
                Products.prototype.GetCatagoryAssociatedToProduct(false);
            }

            if (_currentPageContents.attr('class').indexOf(Constant.seo) > 0) {

                $('#divCreateSEO').show();
                Products.prototype.GetProductSEODetails();
            }

            if (_currentPageContents.attr('class').indexOf("price") > 0) {
                $('#ProductPrice').show();
                Products.prototype.GetProductPriceDetails();
            }

            if (groupType == "Link") {
                $("#assignLinkProducts" + groupId).show();
                Products.prototype.GetAssignedLinkProducts(parseInt($("#ProductId").val(), 10), parseInt(groupId, 10), groupCode);
            }
            if ($("div[id='" + groupCode + "']").find('.multi-upload-Image').length > 0)
                $("div[id='" + groupCode + "']").find('.multi-upload-Image').each(function () {
                    Products.prototype.GetMultipleUploadImages($(this).attr('id'));
                });

            if ($("div[id='" + groupCode + "']").find('.multi-upload-Files').length > 0)
                $("div[id='" + groupCode + "']").find('.multi-upload-Files').each(function () {
                    EditableText.prototype.GetMultipleUploadFiles($(this).attr('id'));
                });
            Products.prototype.GetCatalogTab(_currentPageContents);
        });
    }

    GetCatalogTab(_currentPageContents): void {
        if (_currentPageContents.attr('class').indexOf(Constant.CATALOG) > 0) {
            Endpoint.prototype.GetAssociatedCatalog(parseInt($("#ProductId").val()), Products.prototype.LoadTree);
        }
    }

    LoadTree(response): void {
        $("#CatalogScreen").show();
        $("#CatalogScreen").jstree("destroy").empty(); //Destroy already created JsTree to initialise another tree.
        $("#CatalogScreen").attr("data-tree", response.catalogTree);
        var catalogArray = eval(response.catalogTree)[0];
        if (catalogArray.length > 0) {
            $("#CatalogScreen").removeClass("product-catalog");
            $("#CatalogScreen").jstree({
                'core': {
                    data: catalogArray
                },
                "plugins": ["wholerow"]
            });
            Products.prototype.BindTreeEvent();
            $("#CatalogScreen").jstree(true);
            $("#CatalogScreen").show();
            return;
        }
        $("#CatalogScreen").addClass("product-catalog");
        $("#CatalogScreen").has("span").length == 0 ? $("#CatalogScreen").append("<span style='color:red'>No Result Found.</span>") : "";
    }


    BindTreeEvent(): void {
        $("#CatalogScreen").off('ready.jstree');
        $("#CatalogScreen").on('ready.jstree', Products.prototype.JsTreeLoaded);
    }

    JsTreeLoaded(e): void {
        var jsTreeNodes = $(e.currentTarget).find("li")
        var treeData = $('#CatalogScreen').attr('data-tree');
        for (var index = 0; index < jsTreeNodes.length; index++) {
            var node = jsTreeNodes[index];
            var objectArray = eval(treeData)[0];
            var obj = $.grep(objectArray, function (obj) {
                return obj["text"] == $(node).text().trim();
            });
            obj[0]["IsCatalogPublished"] ? $(node).append("<span Class='jstree-anchor'> (Published Successfully) </span>") : "";
        }
    }


    //For hide controls
    SectionToHidecontrols() {
        $('#CustomFieldsList').hide();
        $('#DownloadableProductKeyList').hide();
        $('#AssociatedProductsList').hide();
        $('#AssociatedAddonList').hide();
        $('#Inventory1').hide();
        $('#AssignedLinkProductsList').hide();
        $('#InventoryDetails').hide();
        $('#ProductSEO').hide();
        $('#ProductPrice').hide();
        $('.productInfoPage').hide();
        $(".assignLinks").hide();
        $(".lowerContainer").hide();
        $("#AssociateCategory").hide();
        $("#AssignPersonalizedAttribute").hide();
        $("#Personalization").hide();
        $("#CatalogScreen").hide();
        $("#productAsidePannel>li.tab-selected").removeClass("tab-selected");
    }

    //Method for Get Multiple Upload Images
    GetMultipleUploadImages(groupCode) {
        groupCode = groupCode.substring(3);
        var ImageSource = $("#" + groupCode).attr('src');
        var InitialImage = $("#" + groupCode);
        var MediaIds = $("input[name=" + groupCode + "]").val();
        var MediaIdArrays = MediaIds.split(',');
        if (MediaIds.length > 0 && ImageSource != undefined && ImageSource != "") {
            var ImageSourceCollection = ImageSource.split(',');
            if (ImageSourceCollection.length > 0) {
                var divName = $("#" + groupCode).closest('div').attr('id');
                var attributeName = $("#" + groupCode).attr('id');
                $("div#div" + groupCode).find('.multi-upload-images').remove();
                var presentDiv = $("#" + divName);
                presentDiv.append(InitialImage);
                for (var i = 0; i < ImageSourceCollection.length; i++) {
                    presentDiv.append("<div class='upload-images multi-upload-images dirtyignore'><img id='" + attributeName + '_' + i + "' src= '" + ImageSourceCollection[i] + "' class='img-responsive'>" +
                        "<a class=\"upload-images-close\" data-toggle=\"tooltip\" data-placement=\"top\" title=\"Remove\" onclick='EditableText.prototype.RemoveMultipleImage(\"" + attributeName + '_' + i + "\")'><i class='z-close-circle'></i></a>" +
                        "<input type='hidden' id='" + attributeName + "_" + i + "' value= " + MediaIdArrays[i] + " ></div>");
                }
            }
        }
    }

    //Method for Get Assigned Link Products
    GetAssignedLinkProducts(productId: number, linkAttributeid: number, containerid: string) {
        ZnodeBase.prototype.ShowLoader();
        Endpoint.prototype.GetAssignedLinkProducts(productId, linkAttributeid, function (response) {
            $('#AssignedLinkProductsList').show();
            $('#AssignedLinkProductsList').html("");
            $('#AssignedLinkProductsList').html(response);
            DynamicGrid.prototype.ClearCheckboxArray();
            ZnodeBase.prototype.HideLoader();
        });
    }

    //Method for Get Personalized Attributes
    GetPersonalizedAttributes(productId: number) {
        Endpoint.prototype.GetPersonalizedAttribute(productId, function (response) {
            $('div[id^=Personalization]').html(response);

            $(".multi-upload-Image").each(function () {
                Products.prototype.GetMultipleUploadImages($(this).attr('id'));
            });

            $(".multi-upload-Files").each(function () {
                EditableText.prototype.GetMultipleUploadFiles($(this).attr('id'));
            });

            $.getScript("/Scripts/References/DynamicValidation.js");
            Attributes.prototype.ParseForm();
            ZnodeBase.prototype.HideLoader();
        });
    }

    //Method for Custom Field List
    CustomFieldList() {
        Endpoint.prototype.GetCustomFieldList(parseInt($("#ProductId").val(), 10), function (response) {
            $('[id^=CustomFields]').hide();
            $("#CustomFieldsList").html('');
            $('#CustomFieldsList').show();
            $('#CustomFieldsList').addClass("col-sm-12 nopadding");
            $("#CustomFieldsList").html(response);
            GridPager.prototype.UpdateHandler();
            DynamicGrid.prototype.ClearCheckboxArray();
            ZnodeBase.prototype.HideLoader();
        });
    }

    //Method for Downloadable Product Key List
    DownloadableProductKeyList() {
        Endpoint.prototype.GetDownloadableProductKeyList(parseInt($("#ProductId").val(), 10), $('[id^=SKU]').val(), function (response) {
            $('[id^=Downloadable]').hide();
            $("#DownloadableProductKeyList").html('');
            $('#DownloadableProductKeyList').show();
            $('#DownloadableProductKeyList').addClass("col-sm-12 nopadding");
            $("#DownloadableProductKeyList").html(response);
            GridPager.prototype.UpdateHandler();
            DynamicGrid.prototype.ClearCheckboxArray();
            ZnodeBase.prototype.HideLoader();
        });
    }

    SuccessErrorProductSeoNotification(response: any): any {
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, response.status ? 'success' : 'error', isFadeOut, fadeOutTime);
        if (response.cmsseodetailId != 0) {
            $("#CMSSEODetailId").val(response.cmsseodetailId);
            $("#LocaleId").prop("disabled", false);
            $("#LocaleId").attr("readonly", false);
        }
    }

    SuccessErrorInventoryNotification(response: any): any {
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, response.status ? 'success' : 'error', isFadeOut, fadeOutTime);
        if (response.status)
            Products.prototype.GetInventoryBySKU();
    }

    //Get Inventory details by SKU.
    GetInventoryDetails() {
        var isDownlodableProduct = ($('#IsDownlodableProduct').val() == "true" ? true : false);
        var productId = parseInt($("#ProductId").val(), 10);
        if ($('[id^=SKU]').val() != "") {
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.GetInventoryDetails($('[id^=SKU]').val(), productId, isDownlodableProduct, function (response) {
                $('#InventoryDetails').show();
                $('#InventoryDetails').html("");
                $('#InventoryDetails').html(response);
                Endpoint.prototype.GetDownloadableProductKeys($('[id^=SKU]').val(), productId, 0, function (response) {
                    $('#GetDownloadableProductKeys').html("");
                    $('#GetDownloadableProductKeys').html(response);
                    Inventory.prototype.HideUsedDownloadableKeyEditLink();
                    ZnodeBase.prototype.HideLoader();
                });

            });
        }
    }

    //Get Product price detail by SKU.
    GetProductPriceDetails() {
        if ($('[id^=SKU]').val() != "") {
            ZnodeBase.prototype.ShowLoader();
            var productpriceListId = 0;
            if ($("#hdnpriceListId").val() != undefined) {
                productpriceListId = $("#hdnpriceListId").val();
            }
            Endpoint.prototype.GetProductPriceDetails($("#ProductId").val(), $('[id^=SKU]').val(), $(".ProductType option:selected").text(), productpriceListId, function (response) {
                $('#ProductPrice').show();
                $('#ProductPrice').html("");
                $('#ProductPrice').html(response);
                $("#PriceListId").change(function () {
                    if ($("#PriceListId").val() != "") {
                        Products.prototype.GetProductPriceDetailsByListId();
                    }
                });
                ZnodeBase.prototype.HideLoader();
            });
        }
    }
    GetProductPriceDetailsByListId() {
        if ($('[id^=SKU]').val() != "") {
            ZnodeBase.prototype.ShowLoader();
            var productpriceListId = $("#PriceListId option:selected").val();
            Endpoint.prototype.GetProductPriceDetails($("#ProductId").val(), $('[id^=SKU]').val(), $(".ProductType option:selected").text(), productpriceListId, function (response) {
                $('#ProductPrice').show();
                $('#ProductPrice').html("");
                $('#ProductPrice').html(response);
                if (!$('#valSalesPrice').val() || !$('#valRetailPrice').val()) {
                    Products.prototype.GetPriceBySku();
                }
                $("#PriceListId").change(function () {
                    if ($("#PriceListId").val() != "") {
                        Products.prototype.GetProductPriceDetailsByListId();
                    }
                });
                ZnodeBase.prototype.HideLoader();
            });
        }
    }
    fnDeleteTierPrice(priceTierId, pimProductId) {
        var productpriceListId = $("#PriceListId option:selected").val();
        Endpoint.prototype.DeleteTierPriceByIdAndPriceList(priceTierId, pimProductId, productpriceListId, function (response) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.Message, response.HasNoError ? 'success' : 'error', isFadeOut, fadeOutTime);
        });
    }
    GetCatagoryAssociatedToProduct(isAssociated: boolean) {
        var productId = $("#ProductId").val();
        if (productId != undefined) {
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.GetAssociatedProductCatagoryList(isAssociated, productId, function (response) {
                $('#AssociateCategory').show();
                $('#AssociateCategory').html("");
                $('#AssociateCategory').html(response);
                ZnodeBase.prototype.HideLoader();
            });
        }
    }

    GetProductSEODetails(): any {
        var productId = $("#ProductId").val();
        var productPublishId = $("#ProductPublishId").val();
        var seoCode = $('[id^=SKU]').val();
        var seoTypeId = $("#CMSSEOTypeId").val();
        if (seoTypeId == undefined)
            seoTypeId = 1;

        var portalId = $("#PortalId").val();
        if (portalId == undefined)
            portalId = 0;

        var localeId = $("#LocaleId").val();
        if (localeId == undefined)
            localeId = 0;

        if (productId != undefined) {
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.GetProductSEODetails(seoTypeId, seoCode, productPublishId, localeId, portalId, function (response) {
                $('#ProductSEO').show();
                $('#ProductSEO').html("");
                $('#ProductSEO').html(response);
                $('#hidden-pimproduct').val(productId);
                ZnodeBase.prototype.HideLoader();
            });
        }
    }

    GetProductDefaultSEODetails(): any {
        var itemId = $("#ProductId").val();
        var seoCode = $('[id^=SKU]').val();
        var seoTypeId = $("#CMSSEOTypeId").val();
        if (seoTypeId == undefined)
            seoTypeId = 1;

        var portalId = $("#PortalId").val();
        if (portalId == undefined)
            portalId = 0;

        var localeId = $("#LocaleId").val();
        if (localeId == undefined)
            localeId = 0;

        if (itemId != undefined) {
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.GetDefaultSEODetails(seoTypeId, seoCode, itemId, localeId, portalId, function (response) {
                $('#ProductSEO').show();
                $('#ProductSEO').html("");
                $('#ProductSEO').html(response);
                $('#hidden-pimproduct').val(itemId);
                ZnodeBase.prototype.HideLoader();
            });
        }
    }
    GetInventoryBySKU(): any {
        if ($("#IsDownloadable").val() == "True")
            return;

        ZnodeBase.prototype.ShowLoader();
        var sku = $('[id^=SKU]').val();
        var date = new Date();
        var warehouseId = $("#WarehouseId").val();

        if ((sku == undefined || sku == "") ||
            (warehouseId == undefined || warehouseId == "")) {
            $("#QuantityInventory").val("");
            $("#ReOrderLevel").val("");
            $("#InventoryId").val("");
            $("#BackOrderQuantity").val("");
            $("#BackOrderExpectedDate").val("");
            $("#BackOrderExpectedDate").datepicker("setDate", date);
            $('#BackOrderExpectedDate').val("").datepicker("update");
            ZnodeBase.prototype.HideLoader();
            return;
        }

        Endpoint.prototype.GetInventoryBySKU(sku, warehouseId, function (response) {
            if (response.status) {
                $("#QuantityInventory").val(response.quantity);
                $("#ReOrderLevel").val(response.reOrderLevel);
                $("#InventoryId").val(response.inventoryId);
                $("#BackOrderQuantity").val(response.backOrderQuantity);
                $("#BackOrderExpectedDate").val(response.backOrderExpectedDate);
                if (response.backOrderExpectedDate)
                    $("#BackOrderExpectedDate").datepicker("setDate", response.backOrderExpectedDate);
                else {
                    $("#BackOrderExpectedDate").datepicker("setDate", date);
                    $('#BackOrderExpectedDate').val("").datepicker("update");
                }
            }
            else {
                $("#QuantityInventory").val("");
                $("#ReOrderLevel").val("");
                $("#InventoryId").val(0);
                $("#BackOrderQuantity").val("");
                $("#BackOrderExpectedDate").val("");
                $("#BackOrderExpectedDate").datepicker("setDate", date);
                $('#BackOrderExpectedDate').val("").datepicker("update");
            }

            ZnodeBase.prototype.HideLoader();
        });
    }

    //Method for Add Custom Field Result
    CustomFieldAddResult(data: any) {
        $("#divAddCustomPopup").modal("show");
        $("#divAddCustomPopup").html(data);
        if (data.isSuccess == true) {
            $("#divAddCustomPopup").modal("hide");
            ZnodeBase.prototype.RemovePopupOverlay();
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(data.Message, data.isSuccess ? "success" : "error", isFadeOut, fadeOutTime);
            Products.prototype.CustomFieldList();
        }
    }

    //Method for Add Custom Field
    AddCustomField(): any {
        $("#addCustomField").on("click", function () {
            var productId: number = parseInt($("#ProductId").val(), 10);
            Endpoint.prototype.GetCustomField(productId, function (res) {
                $("#divAddCustomPopup").modal("show");
                $("#divAddCustomPopup").html(res);

            });
        });
    }

    //Method for Edit Custom Field
    EditCustomField(): any {
        $("#grid tbody tr td").find(".z-edit").click(function (e) {
            e.preventDefault();
            var dataParam = decodeURIComponent($(this).attr("data-parameter"));
            var splittedParam = dataParam.split('&');
            var productId = splittedParam[0].split('=')[1];
            var customFieldId = splittedParam[1].split('=')[1];
            Endpoint.prototype.EditCustomField(productId, customFieldId, function (res) {
                $("#divAddCustomPopup").modal("show");
                $("#divAddCustomPopup").html(res);
            });
        });
    }

    //Method for Delete Multiple Custom Fields
    DeleteMultipleCustomFields(productId: number, control: any): any {
        var customFieldId = [];
        customFieldId = MediaManagerTools.prototype.unique();
        if (customFieldId.length > 0) {
            Endpoint.prototype.DeleteCustomFields(customFieldId.join(","), productId, function (response) {
                DynamicGrid.prototype.RefreshGridOndelete(control, response);
            });
        }
    }

    //Method for Upload Images
    UploadImages(ImageId: string) {
        var associatedMediaIds = [];
        associatedMediaIds = MediaManagerTools.prototype.unique();
        if ($("#currentGroupCode").val()) {
            var imgId = $("#" + $("#currentGroupCode").val()).find("img").attr("id");
            if (imgId != undefined) {
                var lastSelectedMediaId = imgId.split('_')[2];
                if (associatedMediaIds.length === 0) {
                    associatedMediaIds.push(ImageId.substring(ImageId.indexOf('_') + 1));
                }
                $("input#" + ImageId).val(associatedMediaIds.toString());
                $("#divRichtextboxModelPartial").html("");
            }
        }
    }

    PublishProductPopup(zPublishAnchor) {
        if (zPublishAnchor != null) {
            zPublishAnchor.attr("href", "#");
            $("#HdnProductId").val($(zPublishAnchor).attr("data-parameter").split('&')[0].split('=')[1]);
        }
        $("#PublishProduct").modal('show');
    }

    PublishProduct(): any {
        let publishStateData: string = 'NONE';

        if ($('#radBtnPublishState').length > 0)
            publishStateData = ZnodeBase.prototype.mergeNameValuePairsToString($('#radBtnPublishState').serializeArray());

        ZnodeBase.prototype.ShowLoader();

        Endpoint.prototype.PublishProduct($("#HdnProductId").val(), ZnodeBase.prototype.mergeNameValuePairsToString($('#radBtnPublishState').serializeArray()), function (res) {
            DynamicGrid.prototype.RefreshGridOndelete($("#View_ManageProductList").find("#refreshGrid"), res);
        });
    }

    //Method for Publish Product
    UpdateAndPublishProduct() {
        let publishStateData: string = 'NONE';

        if ($('#radBtnPublishState').length > 0)
            publishStateData = ZnodeBase.prototype.mergeNameValuePairsToString($('#radBtnPublishState').serializeArray());
        $("#revisionType").val(publishStateData);

        $("#frmProduct").attr("action", "UpdateAndPublishProduct");
        this.SaveProduct(undefined);
    }

    //Method for Save Product
    SaveProduct(backURL: string) {
        ZnodeBase.prototype.ShowLoader();

        if (!$("#frmProduct").valid()) {
            $(".input-validation-error").parent().parent().parent().parent().each(function () {
                $('li[data-groupcode=' + $(this).parent().attr('id') + ']').addClass('active-tab-validation');
                ZnodeBase.prototype.HideLoader();
            });
        } else if (!Products.prototype.IsAttributeValueUnique(false)) {
            ZnodeBase.prototype.HideLoader();
            return false;
        }
        else {
            var configureAttributeIds = []; var configureFamilyIds = [];
            $("#ConfigureAttributeCheckboxes").find('input').each(function () {
                if ($(this).is(":checked")) {
                    configureAttributeIds.push($(this).attr('id'));
                    configureFamilyIds.push($(this).attr('data-familyId'));
                }
            });
            configureAttributeIds.length > 0 ? $("#ConfigureAttributeIds").val(configureAttributeIds) : $("#ConfigureAttributeIds").val();
            configureFamilyIds.length > 0 ? $("#ConfigureFamilyIds").val(configureFamilyIds[0]) : $("#ConfigureFamilyIds").val();
            if (selectedTab != undefined)
                $("#frmProduct").attr("action", $("#frmProduct").attr("action") + "?selectedtab=" + selectedTab);

            if (Products.prototype.ValidateFileTypeControl()) {
                $(".fileuploader").parent().parent().parent().each(function () {
                    $('li[data-groupcode=' + $(this).parent().parent().attr('id') + ']').addClass('active-tab-validation');
                    ZnodeBase.prototype.HideLoader();
                });
                return;
            }

            var ProductId: number = parseInt($("#ProductId").val(), 10);
            var url = decodeURIComponent(window.location.href);
            var orignalUrl = url.split(/[?#]/)[0];
            if (ProductId == 0 || orignalUrl.indexOf('Copy') > -1)
                $.cookie("_productCulture", "");

            if (typeof (backURL) != "undefined")
                $.cookie("_backURL", backURL, { path: '/' });

            //Enabled outofstockoptions which disabled in case of downloadable product.
            $(".OutOfStockOptions").prop("disabled", false);

            $("#frmProduct").submit();
        }
        Products.prototype.ProductPageRedirection();
    }

    RedirectToPersonalizationTab() {
        if ($("#Personalization .input-validation-error").length > 0) {
            $(".groupPannel.Personalization").click();
        }
    }

    RedirectToProductInfoTab() {
        $(".groupPannel").eq(0).click()
        $(".input-validation-error").parent().parent().parent().parent().parent().parent().each(function () {
            if ($("#" + $(this).parent().attr('id')).hasClass('panel-collapse')) {
                $("#" + $(this).parent().attr('id')).children().children().show();
                $("#" + $(this).parent().attr('id')).slideDown();
                $("#" + $(this).parent().attr('id')).parent().children().eq(0).children(".panel-title").removeClass("collapsed");
            }
        });
    }

    ProductPageRedirection() {
        var isProductInfoTab: boolean = false;
        $("#frmProduct .input-validation-error").each(function () {
            if (!$(this).closest('.Personalization').attr('id')) {
                isProductInfoTab = true;
            }
        });

        if (isProductInfoTab && ($('.tab-selected').data('group') != undefined && $('.tab-selected').data('group').toLowerCase() == ('ProductInfo').toLowerCase())) {
            $("#productInfoPage").hide();
            Products.prototype.RedirectToProductInfoTab();
        }
        else if (($('.tab-selected').data('group') == undefined) && ($($("#frmProduct .input-validation-error")[0]).closest('.Personalization')).length == 0)
        {
            Products.prototype.RedirectToProductInfoTab();
        }
        else if ($("#frmProduct .input-validation-error").closest('.Personalization').attr('id')) {
            $("#Personalization").hide();
            Products.prototype.RedirectToPersonalizationTab();
        }
        else {
            Products.prototype.RedirectToProductInfoTab();
        }
    }

    fnShowHide(control, divId: string) {
        if (divId != undefined) {
            var expiresTime: Date = ZnodeBase.prototype.SetCookiesExpiry();
            $.cookie("_proInfoTab", divId, { expires: expiresTime });
            proInfoTab = divId;
        }
        else {
            proInfoTab = ZnodeBase.prototype.getCookie("_proInfoTab");
        }

        control = $(proInfoTab).parent("div").find(".panel-heading").find("h4");
        if ($(control).hasClass("collapsed")) {
            $(control).removeClass("collapsed");
        }
        else {
            $(control).addClass("collapsed");
        }
        $("#" + proInfoTab).children(".panel-body").children(".pimAtribute").show();
        $("#" + proInfoTab).slideToggle();
    }

    //Method for Copy Product
    CopyProduct() {
        var ProductId: number = parseInt($("#ProductId").val(), 10);
        $("#ProductId").val('');
        window.location.href = '/PIM/Products/Copy?PimProductId=' + ProductId;
    }

    //Method for Assign Personilize Attribute Create
    AssignPersonilizeAttributeCreate() {
        var productId: number = parseInt($("#ProductId").val(), 10);
        Products.prototype.GetPersonalizedAttributes(productId);
    }

    DeleteAssociatedCategoriesToProduct() {
        var PimProductId: number = parseInt($("#ProductId").val(), 10);
        var associatedCategoryIds = null;

        if (PimProductId > 0) {
            var SelectedProductIds: string = DynamicGrid.prototype.GetMultipleSelectedIds('AssociatedCategoriesToProduct');

            if (SelectedProductIds.length > 0) {
                Endpoint.prototype.DeleteAssociatedCategories(SelectedProductIds, function (response) {
                    $(".modal-backdrop").hide();
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, response.status ? 'success' : 'error', isFadeOut, fadeOutTime);
                    Products.prototype.GetCatagoryAssociatedToProduct(false);
                });
            }
        }

    }

    //Method for Delete Associated Products
    DeleteAssociatedProducts(control) {
        var PimProductId: number = parseInt($("#ProductId").val(), 10);
        var associatedProductIds = null;
        if (PimProductId == 0) {
            var SelectedProductIds: string = DynamicGrid.prototype.GetMultipleSelectedIds('AssociatedProductsList');

            if (SelectedProductIds.length > 0) {
                associatedProductIds = $("#AssociatedProductIds").val();
                var associatedProductIdsArray = [];
                var selectedProductIdsArray = [];
                associatedProductIdsArray = associatedProductIds.split(",");
                selectedProductIdsArray = SelectedProductIds.split(",");

                var differenceOfAssociatedProductIdsArray = $(associatedProductIdsArray).not(selectedProductIdsArray).get();
                $("#AssociatedProductIds").val(differenceOfAssociatedProductIdsArray);
                if ($(".ProductType option:selected").val() == Constant.configurableProduct && $("#ddlfamily").val() > 0) {
                    var associatedAttributeIds = [];
                    $("#ConfigureAttributeCheckboxes").find('input').each(function () {
                        if ($(this).is(":checked")) {
                            associatedAttributeIds.push($(this).attr('id'));
                        }
                    });
                    Products.prototype.GetConfigureProductsToBeAssociated(differenceOfAssociatedProductIdsArray.toString(), associatedAttributeIds.toString());
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("UnassignSuccessful"), 'success', isFadeOut, fadeOutTime);
                }
                else {
                    Products.prototype.GetProductsToBeAssociated(differenceOfAssociatedProductIdsArray.toString());
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("UnassignSuccessful"), 'success', isFadeOut, fadeOutTime);
                }
            }
            else {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectAtleastOneRecord"), 'error', isFadeOut, fadeOutTime);
            }
        }
        else {
            associatedProductIds = DynamicGrid.prototype.GetMultipleSelectedIds();
            if (associatedProductIds.length > 0) {
                Endpoint.prototype.DeleteAssociatedProducts(associatedProductIds, function (res) {
                    DynamicGrid.prototype.RefreshGridOndelete(control, res);
                });
            }
        }
        ZnodeBase.prototype.RemovePopupOverlay();
        ZnodeBase.prototype.HideLoader();
    }

    //Method for Delete Link Products
    DeleteLinkProducts(gridName, control) {
        var LinkCheckBoxCollection = new Array();
        if (LinkCheckBoxCollection.length === 0) {
            $("#" + gridName).find("tr").each(function () {
                if ($(this).find(".grid-row-checkbox").length > 0) {
                    if ($(this).find(".grid-row-checkbox").is(":checked")) {
                        var id = $(this).find(".grid-row-checkbox").attr("id");
                        LinkCheckBoxCollection.push(id);
                    }
                }
            });
        }

        var result = [];
        $.each(LinkCheckBoxCollection, function (i, e) {
            if ($.inArray(e.split("_")[1], result) == -1) result.push(e.split("_")[1]);
        });
        Endpoint.prototype.LinkProductDelete(result.join(","), function (data) {
            DynamicGrid.prototype.RefreshGridOndelete($("#" + gridName).find("#refreshGrid"), data);
            DynamicGrid.prototype.ClearCheckboxArray();
        });
    }

    //Method for Delete Multiple Product
    DeleteMultipleProduct(control): any {
        var productId = [];
        productId = MediaManagerTools.prototype.unique();
        if (productId.length > 0) {
            Endpoint.prototype.DeleteProducts(productId.join(","), function (response) {
                DynamicGrid.prototype.RefreshGridOndelete(control, response);
            });
        }
    }

    //Method for Hide Edit Link
    HideEditLink(): any {
        $("#grid tbody tr td").find(".z-edit").each(function () {
            $(this).parent().remove();
        });
    }

    //Method for Get Configurable Attributes
    GetConfigurableAttributes() {
        Endpoint.prototype.GetConfigureAttributeDetails($("#ddlfamily").val(), parseInt($("#ProductId").val(), 10), function (response) {
            var obj = JSON.parse(response);
            if (obj.status === 'false') {
                $("#productDetails ul li").filter(".AssociatedProducts").hide();
                $('#divConfigureAttributeError').modal({ backdrop: 'static', keyboard: false });
            }
            else {
                _data = obj.data;
                $("#productDetails ul li").filter(".AssociatedProducts").show();
                $(".ProductType").val(Constant.configurableProduct);
                Products.prototype.GetAssociatedProductsSpan();
                Products.prototype.CreateAttributeCheckboxes();
            }
        });
    }

    //Method for Restrict Enter Button
    RestrictEnterButton(frmId: string): any {
        $(frmId).on('keyup keypress', function (e) {
            var keyCode = e.keyCode || e.which;
            if (keyCode === 13) {
                e.preventDefault();
                return false;
            }
        });
    }

    //Method for Create Attribute Checkboxes
    CreateAttributeCheckboxes() {
        var divCreateCheckbox = $('#divCreateConfigureAttributesCheckboxes');
        $("#errorSelectConfigCheckbox").hide();

        divCreateCheckbox.html('');
        for (var i = 0; i < _data.length; i++) {
            divCreateCheckbox.append("<label style='margin-right:10px;'>" +
                "<input type='checkbox' class='configure-checkbox' name='" + _data[i].AttributeCode + "' id='" + _data[i].PimAttributeId + "' data-familyid='" + _data[i].PimAttributeFamilyId + "' data-attributename='" + _data[i].AttributeName + "'>" +
                "<span class='lbl padding-8'></span>" + _data[i].AttributeName + "</label>");

            $("input:checkbox[id=" + _data[i].PimAttributeId + "]").prop("checked", _data[i].IsConfigurableAttribute);
        }

        $('#divConfigureAttributeCheckboxes').modal({ backdrop: 'static', keyboard: false });
    }

    //Method for Cancel Config
    CancelConfig() {
        $('#ddlfamily').prop('selectedIndex', 0);
        $('.ProductType').prop('selectedIndex', 0);
        location.reload();
        ZnodeBase.prototype.HideLoader();
    }

    //Method for Set Configure For Association
    SetConfigForAssociation() {
        var associatedAttributeIds = [];
        var _dataarray = [];
        _dataarray = _data;

        //Creating chekckbox for associated product
        var divCheckboxesForAssociation = $('#ConfigureAttributeCheckboxes');
        divCheckboxesForAssociation.html('');

        $("#divCreateConfigureAttributesCheckboxes").find('input').each(function () {
            if ($(this).is(":checked")) {
                var control = $(this);
                associatedAttributeIds.push(control.attr('id'));
                divCheckboxesForAssociation.append("<label style='margin-right:10px;'>" +
                    "<input type='checkbox' class='configure-checkbox' name='" + control.attr('name') + "' id='" + control.attr('id') + "' data-familyid='" + control.attr('data-familyid') + "' checked disabled='true' >" +
                    "<span class='lbl padding-8'></span>" + control.attr('data-attributename') + "</label>");

                var result = $.grep(_dataarray, function (e) { return e.PimAttributeId == control.attr('id'); });
                var controlname = result[0].AttributeCode + '_' + result[0].PimAttributeId;
                $('[id^=' + controlname + ']').parent().parent().parent().remove();
            }
        });

        $("#ConfigureAttributeIds").val(associatedAttributeIds);
        if (associatedAttributeIds.length === 0) {
            $("#errorSelectConfigCheckbox").show();
            return false;
        }
        $('#divConfigureAttributeCheckboxes').modal('hide');
    }

    //Method for Get Associated Addon List
    GetAssociatedAddonList(productId: number) {
        Endpoint.prototype.GetAssociatedAddonlist(productId, function (response) {
            $('[id^=' + Constant.addOns + ']').hide();
            $("#AssociatedAddonList").html('');
            $('#AssociatedAddonList').show();
            $('#AssociatedAddonList').addClass("col-sm-12 nopadding");
            $("#AssociatedAddonList").html(response);
            Products.prototype.GetCreateAddonGroupForm();
        });
    }

    //Method for Get Create Addon Group Form
    GetCreateAddonGroupForm() {
        $("#addAddonGroup").click(function () {
            $("#ErrorAssociateAddonGroupId").hide();
            $("#addOnMessageContainerId").hide();
            $("#addOnMessageContainerId").text("");
            Endpoint.prototype.GetCreateAddonGroupForm(function (response) {
                $("#AddonGroupForm").html(response);
            });
        });
    }

    //Method for Associate Addon Group
    AssociateAddonGroup() {
        var parentProductId: number = parseInt($("#ProductId").val(), 10);
        var addonGroupProductIds: string = DynamicGrid.prototype.GetMultipleSelectedIds('UnassociatedAddonGroups');

        if (addonGroupProductIds.length == 0) {
            $("#AssociateAddonGroupError").show();
        }
        else {
            var model = { "ParentId": parentProductId, "AssociatedIds": addonGroupProductIds };

            if (parentProductId > 0) {
                Endpoint.prototype.AssociateAddons(model, function (response) {
                    $('[id^=' + Constant.addOns + ']').hide();
                    $("#AssociatedAddonList").html('');
                    $('#AssociatedAddonList').show();
                    $('#AssociatedAddonList').html(response);
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SuccessAddonAssociated"), 'success', isFadeOut, fadeOutTime);
                    ZnodeBase.prototype.RemovePopupOverlay();
                });
            } else {
                $("#AssociatedAddonValue").val(addonGroupProductIds);
                $("#AssociateAddonPopup").modal("hide");
            }
            $("#UnassociatedAddonGroups").hide(700);
        }
    }

    //Method for Delete Addon Groups
    DeleteAddonGroups(control) {
        var addonGroupIds: string = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (addonGroupIds.length > 0) {
            Endpoint.prototype.DeleteAddonGroups(addonGroupIds, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    //Method for Remove Addon Product Association
    RemoveAddonProductAssociation(control) {
        var addonProductId = $(control).val();
        Endpoint.prototype.DeleteAddonProduct(addonProductId, parseInt($("#ProductId").val(), 10), function (response) {
            $('[id^=' + Constant.addOns + ']').hide();
            $("#AssociatedAddonList").html('');
            $('#AssociatedAddonList').show();
            $('#AssociatedAddonList').html(response);
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SuccessDeleteAssociatedAddon"), 'success', isFadeOut, fadeOutTime);
        });
    }

    //Method for Get Unassociated Addon Products
    GetUnassociatedAddonProducts(addonProductId, gridName): any {
        DynamicGrid.prototype.ClearCheckboxArray();
        ZnodeBase.prototype.BrowseAsidePoupPanel('/PIM/Products/GetUnassociatedProducts?parentProductId=' + parseInt($("#ProductId").val(), 10) + '&listType=' + UnAssociatedProductListType.Addon.toString() + '&addonProductId=' + addonProductId + '&associatedProductIds=0', 'UnassociatedProductAsidePannel');
        Products.prototype.AssociateAddonProducts(addonProductId, gridName);
    }

    //Method for Associate Addon Products
    AssociateAddonProducts(addonProductId, associationGrid): any {
        $(document).off("click", "#associateAddonProduct");
        $(document).on("click", "#associateAddonProduct", function () {
            var productIds: string = DynamicGrid.prototype.GetMultipleSelectedIds('UnassociatedProducts');

            var PimProductId: number = parseInt($("#ProductId").val(), 10);

            if (productIds.length == 0) {
                $("#asidePannelmessageBoxContainerId").show();
                $("#UnassociatedProductAsidePannel").show();
                ZnodeBase.prototype.HideLoader();
            } else {
                var model = { "ParentId": addonProductId, "AssociatedIds": productIds,"PimProductId" : PimProductId };

                if (addonProductId > 0) {
                    Endpoint.prototype.AssociatedAddonProduct(model, function (response) {
                        if (response.status) {
                            Products.prototype.GetAssociatedAddonList(parseInt($("#ProductId").val(), 10));
                        }
                        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, response.status ? 'success' : 'error', isFadeOut, fadeOutTime);
                        DynamicGrid.prototype.ClearCheckboxArray();
                        ZnodeBase.prototype.RemovePopupOverlay();
                        $("#UnassociatedProductAsidePannel").hide();
                    });
                }
            }
        });
    }

    //Method for Delete Addon Product Detail
    DeleteAddonProductDetail(gridName) {
        ZnodeBase.prototype.ShowLoader();
        var result: string = DynamicGrid.prototype.GetMultipleSelectedIds(gridName);
        Endpoint.prototype.UnassociateAddonProducts(result, function (data) {
            ZnodeBase.prototype.showDeleteStatus(data);
        });
    }

    //Method for Get Products ToBe Associated
    GetProductsToBeAssociated(associatedProductIds: string) {
        ZnodeBase.prototype.ShowLoader();
        var productType = $(".ProductType").val();
        Endpoint.prototype.GetProductsToBeAssociated(associatedProductIds, productType, function (response) {
            $('[id^=AssociatedProducts]').show();
            $('#AssociatedProductsList').html("");
            $('#AssociatedProductsList').addClass("col-sm-12 nopadding");
            $('#AssociatedProductsList').html(response);
            Products.prototype.HideEditLink();
            ZnodeBase.prototype.HideLoader();
        });
    }

    //Method for Get Configure Products ToBe Associated
    GetConfigureProductsToBeAssociated(associatedProductIds: string, associatedAttributeIds: string) {
        ZnodeBase.prototype.ShowLoader();
        Endpoint.prototype.GetConfigureProductsToBeAssociated(associatedProductIds, associatedAttributeIds, function (response) {
            $('[id^=AssociatedProducts]').show();
            $('#AssociatedProductsList').html("");
            $('#AssociatedProductsList').addClass("col-sm-12 nopadding");
            $('#AssociatedProductsList').html(response);
            Products.prototype.HideEditLink();
            var ActionIndex = $('th:contains("Action")').index();
            var totalColumn = $('th:last-child').index();
            if (totalColumn > 0) {
                Products.prototype.ChangeColumnIndex(ActionIndex, totalColumn);
            }
            ZnodeBase.prototype.HideLoader();
        });
    }

    //Method for Delete Addon Product Detail Item
    DeleteAddonProductDetailItem() {
        Endpoint.prototype.UnassociateAddonProducts(addonId, function (data) {
            if (data.status) {
                Products.prototype.GetAssociatedAddonList(parseInt($("#ProductId").val(), 10));
                $(".modal-backdrop").hide();
            }
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(data.message, data.status ? 'success' : 'error', isFadeOut, fadeOutTime);
        });
    }

    //Method for Addon Init
    AddonInit() {
        $(".dropdown-tool").hide();
        $(".manage-column").hide();
        $(".pagination").hide();
        $('a[data-swhglnk="true"]').contents().unwrap();
        $("[id^=" + Constant.addOns + "] .z-delete").each(function () {
            var _data = $(this).attr("onclick").substring($(this).attr("onclick").indexOf("=") + 1, $(this).attr("onclick").indexOf("',"));
            $(this).removeAttr("onclick");
            $(this).attr("data-id", _data);
        });
        $("[id^=" + Constant.addOns + "] .z-delete").click(function (e) {
            e.preventDefault();
            $("#PopUpConfirm").hide();
            $('#DeleteProductAddon').modal('show');
            addonId = parseInt($(this).attr("data-id"), 10);
            return false;
        });
    }

    //Method for Update Associated Addon Information
    UpdateAssociatedAddonInformation(pimAddonProductId: number) {
        var addonProductDisplayOrder: number = parseInt($("#txtDisplayOrder" + pimAddonProductId).val(), 10);
        Products.prototype.ValidateAddonProductDisplayOrder($("#txtDisplayOrder" + pimAddonProductId).val(), pimAddonProductId);
        var productId: number = parseInt($("#ProductId").val(), 10);
        var addonGroupId: number = parseInt($("#addonGroupId" + pimAddonProductId).val(), 10);
        var requiredTypeValue: string = $('.dropdownAddonGroupRequiredTypeValue' + pimAddonProductId).val();

        if (Products.prototype.isAddonProductDisplayOrderValid) {
            var addonProductmodel = { "PimAddOnProductId": pimAddonProductId, "DisplayOrder": addonProductDisplayOrder, "RequiredTypeValue": requiredTypeValue, "PimProductId": productId, "PimAddonGroupId": addonGroupId };
            Endpoint.prototype.UpdateAddonProductAssociation(addonProductmodel, function (response) {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, response.status ? "success" : "error", true, 5000);
                $("#valAddonProductDisplayOrder" + pimAddonProductId).val("");
            });
        }
    }

    //Method for Validate Addon Product Display Order
    ValidateAddonProductDisplayOrder(displayOrder, pimAddonProductId) {
        if (displayOrder.length <= 0) {
            Products.prototype.isAddonProductDisplayOrderValid = Products.prototype.isAddOnGroupValid && false;
            Products.prototype.addonProductDisplayOrderError = ZnodeBase.prototype.getResourceByKeyName("AddonDisplayRequired");
            Products.prototype.ShowErrorMessage(Products.prototype.addonProductDisplayOrderError, $("#txtDisplayOrder" + pimAddonProductId), $("#valAddonProductDisplayOrder" + pimAddonProductId));
        }
        else if (!/^\d+$/.test(displayOrder)) {
            Products.prototype.isAddonProductDisplayOrderValid = Products.prototype.isAddOnGroupValid && false;
            Products.prototype.addonProductDisplayOrderError = ZnodeBase.prototype.getResourceByKeyName("InvalidDisplayOrder");
            Products.prototype.ShowErrorMessage(Products.prototype.addonProductDisplayOrderError, $("#txtDisplayOrder" + pimAddonProductId), $("#valAddonProductDisplayOrder" + pimAddonProductId));
        }
        else if (parseInt(displayOrder, 10) <= 0 || parseInt(displayOrder, 10) > 999) {
            Products.prototype.isAddonProductDisplayOrderValid = Products.prototype.isAddOnGroupValid && false;
            Products.prototype.addonProductDisplayOrderError = ZnodeBase.prototype.getResourceByKeyName("InValidDisplayOrderRange");
            Products.prototype.ShowErrorMessage(Products.prototype.addonProductDisplayOrderError, $("#txtDisplayOrder" + pimAddonProductId), $("#valAddonProductDisplayOrder" + pimAddonProductId));
        }
        else {
            Products.prototype.isAddonProductDisplayOrderValid = true;
            Products.prototype.addonProductDisplayOrderError = "";
            Products.prototype.HideErrorMessage($("#txtDisplayOrder" + pimAddonProductId), $("#valAddonProductDisplayOrder" + pimAddonProductId));
        }
    }

    //Method for Show Error Message
    ShowErrorMessage(errorMessage: string = "", controlToValidateSelector: any, validatorSelector: any) {
        controlToValidateSelector.removeClass("input-validation-valid").addClass("input-validation-error");
        validatorSelector.removeClass("field-validation-valid").addClass("field-validation-error").html("<span>" + errorMessage + "</span>").show();
    }

    //Method for Hide Error Message
    HideErrorMessage(controlToValidateSelector: any, validatorSelector: any) {
        controlToValidateSelector.removeClass("input-validation-error").addClass("input-validation-valid");
        validatorSelector.removeClass("field-validation-error").addClass(" field-validation-valid").html("");
    }

    //Method for DdlCulture Change
    DdlCultureChange() {
        var expiresTime: Date = ZnodeBase.prototype.SetCookiesExpiry();
        $.cookie("_productCulture", $("#ddlCultureSpan").attr("data-value"), { expires: expiresTime }); // expires after 2 hours
        var url = decodeURIComponent(window.location.href);
        var orignalUrl = url.split(/[?#]/)[0];
        if (selectedTab != undefined)
            window.location.replace(orignalUrl + "?PimProductId=" + $("#ProductId").val() + "&selectedtab=" + selectedTab);
        else {
            if (url.indexOf('ProductId') > -1)
                window.location.replace(orignalUrl + "?PimProductId=" + $("#ProductId").val());
            else
                window.location.reload();
        }
    }

    //Method for Unassociated Products Change
    UnassociatedProductsChange(productId) {
        if (productId != undefined && parseInt(productId, 10) > 0) {
            Endpoint.prototype.GetSimilarCombination(parseInt(productId, 10), function (response) {
                if (response.combinationProductIds != '') {
                    var productIds = response.combinationProductIds.split(',');

                    if ($("#UnassociatedProductsDynamic [id=rowcheck_" + productId + "]").is(":checked")) {
                        for (var i = 0; i <= productIds.length - 1; i++) {
                            $("#UnassociatedProductsDynamic [id=rowcheck_" + productIds[i] + "]").attr('disabled', true);
                            $("#UnassociatedProductsDynamic [id=rowcheck_" + productIds[i] + "]").parent().parent().css("background", "#d9edf7");
                        }
                    }
                    else {
                        for (var i = 0; i <= productIds.length - 1; i++) {
                            $("#UnassociatedProductsDynamic [id=rowcheck_" + productIds[i] + "]").attr('disabled', false);
                            $("#UnassociatedProductsDynamic [id=rowcheck_" + productIds[i] + "]").parent().parent().css("background", "#fff");
                        }
                    }
                }
            });
        }
    }

    //Method for return true if value is Unique else false
    IsAttributeValueUnique(isCategory: boolean): boolean {
        //check for other validations        
        var result = ProductAttribute.prototype.Validate();
        var attributeCodeValues = "";
        $("input[type='text']").each(function () {
            if ($(this).attr("data-unique") != undefined && $(this).attr("data-unique") != "" && $(this).attr("data-unique") != "false") {
                attributeCodeValues = attributeCodeValues + $(this).attr("id").split('_')[0] + '#' + $(this).val() + '~';
            }
        });
        var id;
        if ($("#ProductId").val() == null && $("#CategoryId").val() != null && $("#CategoryId").val() != "")
            id = $("#CategoryId").val();
        else if ($("#CategoryId").val() == null && $("#ProductId").val() != null && $("#ProductId").val() != "")
            id = $("#ProductId").val();

        attributeCodeValues = attributeCodeValues.substr(0, attributeCodeValues.length - 1);
        Endpoint.prototype.IsAttributeValueUnique(attributeCodeValues, id, isCategory, function (res) {
            if (res.data != null && res.data != "") {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.data, 'error', isFadeOut, fadeOutTime);
                result = false;
            }
        });
        return result;
    }

    //Method for Validate File Type Control
    ValidateFileTypeControl(): boolean {
        var flag = false;
        $(".fileuploader").each(function () {
            var value = $(this).parent().find("input[type=text]").val();
            var isRequired = $(this).parent().find("input[type=text]").attr("isrequired");
            if ((value === undefined || value == "") && (isRequired === "True")) {
                $(this).parent().find('span[id="fileerrormsg"]').html(ZnodeBase.prototype.getResourceByKeyName("ErrorRequiredfile"));
                $(this).parent().find('span[id="fileerrormsg"]').show();
                return flag = true;
            }
            else {
                $(this).parent().find('span[id="fileerrormsg"]').html("");
                $(this).parent().find('span[id="fileerrormsg"]').hide();
            }
        });

        return flag;
    }

    //Method for Get Associated Products Span
    GetAssociatedProductsSpan(): void {
        if ($(".ProductType option:selected").val() == Constant.groupedProduct)
            $('.AssociatedProducts a').text(ZnodeBase.prototype.getResourceByKeyName(Constant.groupedProduct));
        else if ($(".ProductType option:selected").val() == Constant.bundleProduct)
            $('.AssociatedProducts a').text(ZnodeBase.prototype.getResourceByKeyName(Constant.bundleProduct));
        else if ($(".ProductType option:selected").val() == Constant.configurableProduct)
            $('.AssociatedProducts a').text(ZnodeBase.prototype.getResourceByKeyName(Constant.configurableProduct));
    }

    //Method for Remove Product Type Blank Option
    RemoveProductTypeBlankOption() {
        $(".ProductType").find('option').each(function (index, value) {
            if ($(this).val() == "")
                $(this).remove();
        });
    }

    //Delete associated product ids, if product type grouped/bundled is selected while creating new product.
    DeleteRecentlyAssociatedProducts(): any {
        DynamicGrid.prototype.ClearCheckboxArray();
        $("#grid tbody tr td").find(".z-delete").each(function () {
            var parentProductId = parseInt($("#ProductId").val(), 10);
            if (parentProductId == 0) {
                $(this).removeAttr("onclick");
                $(this).attr("data-target", "#DeletePopUpConfirm");
            }
        });

        $("#grid tbody tr td").find(".z-delete").click(function (e) {
            var SelectedProductIds = $(this).attr("data-parameter").split('&')[1].split('=')[1];
            $("#hdnDeleteProductID").val(SelectedProductIds);
            $("#DeletePopUpConfirm").show();
        });
    }

    //Method for Delete Associated Single Product
    DeleteAssociatedSingleProduct(control) {
        var associatedProductIds = null;
        associatedProductIds = $("#AssociatedProductIds").val();
        var associatedProductIdsArray = [];
        associatedProductIdsArray = associatedProductIds.split(",");
        var SelectedProductIds = $("#hdnDeleteProductID").val();
        var SelectedProductIdsArray = [];
        SelectedProductIdsArray = SelectedProductIds.split(",");
        var differenceOfAssociatedProductIdsArray = $(associatedProductIdsArray).not(SelectedProductIdsArray).get();

        $("#AssociatedProductIds").val(differenceOfAssociatedProductIdsArray);
        if ($(".ProductType option:selected").val() == Constant.configurableProduct && $("#ddlfamily").val() > 0) {
            var associatedAttributeIds = [];
            $("#ConfigureAttributeCheckboxes").find('input').each(function () {
                if ($(this).is(":checked")) {
                    associatedAttributeIds.push($(this).attr('id'));
                }
            });
            Products.prototype.GetConfigureProductsToBeAssociated(differenceOfAssociatedProductIdsArray.toString(), associatedAttributeIds.toString());
        }
        else {
            Products.prototype.GetProductsToBeAssociated(differenceOfAssociatedProductIdsArray.toString());
        }
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("UnassignSuccessful"), 'success', isFadeOut, fadeOutTime);
    }

    //Method for Change Cancel Url
    ChangeCancelUrl() {
        if (Products.prototype.hdnPimCatalogIdForCatalog > 0) {
            var parentCategoryId = (Products.prototype.hdnCategoryIdForCatalog > 0) ? "&pimCategoryHierarchyId=" + Products.prototype.hdnCategoryIdForCatalog : "";
            var cancelUrl = "/PIM/Catalog/Manage?pimCatalogId=" + Products.prototype.hdnPimCatalogIdForCatalog + parentCategoryId;
            $("#btnCancelForProduct").attr('href', cancelUrl);
        }
    }

    //Method for Show/Hide Save Cancel Button
    ShowHideSaveCancelButton() {
        if ($("#UnassociatedProductAsidePannel").find("tr").length > 0)
            $("#divSave").show();
        else
            $("#divSave").hide();
    }

    //Method for Change Column Index
    ChangeColumnIndex(index: number, totalColumn: number) {
        jQuery.each($("table tr"), function () {
            $(this).children(":eq(" + totalColumn + ")").after($(this).children(":eq(" + index + ")"));
        });
    }

    //Method for show Is Active Products
    IsActiveProducts(isActive: boolean): any {
        var productId: string = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (productId.length > 0) {
            Endpoint.prototype.ActivateDeactivateProducts(productId, isActive, function (response) {
                $("#View_ManageProductList #refreshGrid").click();
                DynamicGrid.prototype.ClearCheckboxArray();
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, response.status ? 'success' : 'error', isFadeOut, fadeOutTime);
            });
        }
        else {
            $('#NoCheckboxSelected').modal('show');
        }
    }

    //Method for Validate Display OrderField in Inline Editing for associated Products.
    ValidateDisplayOrderField(object): boolean {
        var isValid = true;
        var regex = new RegExp('^\\d{0,}?$');
        $.each(object, function (key, value) {

            if (isNaN($(object[key]).val()) || $(object[key]).val() == 0) {
                $(object[key]).addClass("input-validation-error");
                var columnName = $(object[key])[0].dataset.columnname;
                if (columnName == "BundleQuantity" || columnName == "DisplayOrder" ) {
                    var html = '<span id = "' + columnName + '_' + key + '" class="field-validation-valid field-validation-error" data-valmsg-for= "' + columnName+'" data-valmsg-replace= "true" >#errorMessage</span>';
                    var message = '';
                    if ($(object[key]).val() == '' && columnName == "BundleQuantity") {
                        message = ZnodeBase.prototype.getResourceByKeyName("QuantityIsRequired");
                    }
                    else if ($(object[key]).val() == '' && columnName == "DisplayOrder") {
                        message = ZnodeBase.prototype.getResourceByKeyName("InvalidDisplayOrder");
                    }
                    else if (isNaN($(object[key]).val())) {
                        message = ZnodeBase.prototype.getResourceByKeyName("EnterNumericError");
                    }
                    else {
                        message = ZnodeBase.prototype.getResourceByKeyName("FiveDigitRangeValidationMessage");
                    }
                    if ($('#' + columnName+ '_' + key).length) {
                        $('#' + columnName + '_'+ key).html(message);
                    }
                    else {
                        html = html.replace('#errorMessage', message);
                        $(object[key]).after(html);
                    }
                    $('#' + columnName + '_' + key).show();
                    isValid = false;
                }
                else {
                    if (isNaN($(object[key]).val()))
                        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("InvalidDisplayOrder"), 'error', isFadeOut, fadeOutTime);
                    else
                        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("FiveDigitRangeValidationMessage"), 'error', isFadeOut, fadeOutTime);
                    isValid = false;
                }
            }
            else if (!regex.test($(object[key]).val())) {
                var columnName = $(object[key])[0].dataset.columnname;
                if (columnName == "BundleQuantity" || columnName == "DisplayOrder") {
                    var message = ZnodeBase.prototype.getResourceByKeyName("FiveDigitRangeValidationMessage");
                    var html = '<span id = "' + columnName + '_' + key + '" class="field-validation-valid field-validation-error" data-valmsg-for= "' + columnName +'" data-valmsg-replace= "true">' + message + '</span>';
                    $(object[key]).addClass("input-validation-error");
                    if ($('#' + columnName +'_' + key).length) {
                        $('#' + columnName +'_' + key).html(message);
                    }
                    else {
                        $(object[key]).after(html);
                    }
                    $('#' + columnName +'_' + key).show();
                    isValid = false;
                }
                else {
                    $(object[key]).addClass("input-validation-error");
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("FiveDigitRangeValidationMessage"), 'error', isFadeOut, fadeOutTime);
                    isValid = false;
                }
            }
            else {
                var columnName = $(object[key])[0].dataset.columnname;
                if (columnName == "BundleQuantity" || columnName == "DisplayOrder") {
                    $('#' + columnName+'_' + key).html("");
                }
                else {
                    if (isValid == true) {
                        $(object[key]).remove("input-validation-error");
                        $(object[key]).removeClass("input-validation-error");
                        isValid = true;

                    }
                }
            }
        });
        return isValid;
    }

    //Method for Show Responce Of Associated ProductsList 
    ShowResponceOfAssociatedProductsList(response: any): void {
        $('[id^=AssociatedProducts]').hide();
        $('#AssociatedProductsList').show();
        $('#AssociatedProductsList').html("");
        $('#AssociatedProductsList').addClass("col-sm-12 nopadding");
        $('#AssociatedProductsList').html(response);
    }

    //Method for Call Attribute Family Associated Product
    CallAttributeFamilyAssociatedProduct(): void {
        Products.prototype.GetAttributeFamilyDetails();
        Products.prototype.ShowHideAssociatedProductTab();
        Products.prototype.ShowHideAttributes();
        Products.prototype.AddCustomField();
        Products.prototype.ShowHideCustomField();
        Products.prototype.InventoryValidation();
    }


    InventoryValidation() {
        this.DisableInventory();
        $('input[name^="IsDownloadable_"]').next('label').click(function () {
            Products.prototype.DisableInventory();
        });
    }

    DisableInventory() {
        if ($('input[name^="IsDownloadable_"]:checked').val() == "true") {
            $(".OutOfStockOptions").val("DisablePurchasing");
            $(".OutOfStockOptions").attr("readonly", "readonly").attr("disabled", true);
            $('[id^=errorSpamOutOfStockOptions_]').removeClass("error-msg field-validation-valid").hide();
            $('[id^=OutOfStockOptions_]').removeClass("input-validation-error");
        }
        else {
            if ($(".OutOfStockOptions").val().length != 0) {
                $(".OutOfStockOptions").prop("disabled", false);
                $(".OutOfStockOptions").attr("readonly", false);
            }
            else {
                $(".OutOfStockOptions").prop("disabled", false);
                $(".OutOfStockOptions").val("");
            }
        }
    }

    //Get product update import side panel.
    GetDialogUpdateProduct(): any {
        ZnodeBase.prototype.BrowseAsidePoupPanel('/PIM/Products/UpdateProducts', 'DialogUpdateProductAsidePannel');
    }

    //Method to download product upload sample csv file.
    DownloadTemplate(): any {
        Endpoint.prototype.DownloadProductUpdateTemplate(function (response) {
            var blob = new Blob([response.data], { type: "octet/stream" });
            if (ZnodeBase.prototype.getBrowser() == "IE")
                window.navigator.msSaveBlob(blob, response.fileName);
            else {
                var url = window.URL.createObjectURL(blob);
                var a = document.createElement("a");
                document.body.appendChild(a);
                a.href = url;
                a.download = response.fileName;
                a.click();
                window.URL.revokeObjectURL(url);
            }
            ZnodeBase.prototype.HideLoader();
        });
    }

    //Method to validate and post the product import data.
    ValidateAndPost(): boolean {
        if (Products.prototype.ValidateModel()) {
            Products.prototype.CreateAndPostModel();
            return false;
        } else
            return false;
    }

    //Method to validate the product import data.
    ValidateModel(): boolean {
        return Import.prototype.ValidateImportFileType() ? true : false;
    }

    //Method to post the product import data.
    CreateAndPostModel(): any {
        var ImportViewModels = Products.prototype.CreateModel();
        Products.prototype.ProductUpdatePostData(ImportViewModels);
    }

    CreateModel(): any {
        var ImportViewModels = {
            FileName: $("#ChangedFileName").val(),
            IsAutoPublish: $("#IsAutoPublish").val(),
        };

        return ImportViewModels;
    }

    ProductUpdatePostData(importModel): any {
        ZnodeBase.prototype.ShowLoader();
        Endpoint.prototype.ProductUpdateImportPost(importModel, function (res) {
            setTimeout(function () {
                ZnodeBase.prototype.HideLoader();
                window.location.href = window.location.protocol + "//" + window.location.host + "/PIM/Products/List";
            }, 900);
        });
    }

    //Method to make Export ajax request
    Export(e: any) {
        e.preventDefault();
        var currentTarget = e.currentTarget;
        var controller: string = currentTarget.getAttribute("data-controller");
        var action: string = currentTarget.getAttribute("data-action");
        var exportTypeId: string = currentTarget.getAttribute("data-exportTypeId");
        var exportType: string = currentTarget.getAttribute("data-exporttype");
        var localId: string = $("#ddlCultureSpan").attr("data-value") ? $("#ddlCultureSpan").attr("data-value") : "0";
        var url = this.getExportUrl(controller, action);
        var param = this.getExportParam(exportTypeId, exportType, localId);
        var catalogId = $('#hdnFilterCatalogId').val();
        param = catalogId != undefined ? (param += "&pimCatalogId=" + catalogId + "&catalogName=" + $('#hdnFilterCatalogName').val()) : param;
        var exportBase = this;
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ExportPleaseWaitMsg"), "success", true, 5000);
        ZnodeBase.prototype.ajaxRequest(url, "GET", param, function (response) {
            if (response.status)
                exportBase.downloadFile(response);
            else
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, "error", true, 5000);
        }, null);
    }

    //Return export parameter
    getExportParam(exportTypeId: string, exportType: string, localId: string) {
        return ("exportFileTypeId=".concat(exportTypeId, "&Type=", exportType, "&localId=", localId));
    }

    //Returns export url
    getExportUrl(controller, action) {
        return ("/".concat(controller, "/", action));
    }

    //Download Data into file to user
    downloadFile(response: any) {
        /*Byte Order Mark - used in encoding that allow reader to identify a file as being encoded in UTF-8.*/
        var BOM = "\uFEFF";
        var blob = new Blob([BOM.concat(response.content)], { type: "text/csv;charset=utf-8;" });

        if (ZnodeBase.prototype.getBrowser() == "IE")
            window.navigator.msSaveBlob(blob, response.fileName);
        else {
            var url = window.URL.createObjectURL(blob);
            var a = document.createElement("a");
            document.body.appendChild(a);
            a.href = url;
            a.download = response.fileName;
            a.click();
            window.URL.revokeObjectURL(url); /* Not to keep the reference to the file any longer.*/
        }
    }

    //Set IsAutoPublish value.
    IsAutoPublish() {
        if ($('#IsAutoPublish').prop("checked") == true)
            $('#IsAutoPublish').val('true');
        else
            $('#IsAutoPublish').val('false');
    }


    GetPriceBySku(): void {
        var productType = $(".ProductType").val();
        var sku = $('[id^=SKU]').val();
        var productId = $("#ProductId").val();

        if (productType == "ConfigurableProduct")
            productType = "Configurable Product";
        else if (productType == "GroupedProduct" || productType == "GroupProduct")
            productType = "grouped product";

        if (productType == "Configurable Product" || productType == "grouped product" || productType == "SimpleProduct" || productType == "BundleProduct") {
            Endpoint.prototype.GetPriceBySku(productId, sku, productType, function (response) {
                if (!$('#valSalesPrice').val()) {
                    $('#valSalesPrice').val(response.data.SalesPrice)
                }
                if (!$('#valRetailPrice').val())
                    $('#valRetailPrice').val(response.data.RetailPrice);
            });
        }
        else {
            $('#valSalesPrice').val("");
            $('#valRetailPrice').val("");
        }
    }

    SetInventoryURLForDownloadable() {
        var parentElement = $('#InventoryDetails');
        var inventoryURL = parentElement.find("section[data-pager-url]").attr("data-pager-url").split("?")[0];
        parentElement.find("table thead a").each(function () {
            var queryString = $(this).attr("href").split("?")[1];
            $(this).attr("href", inventoryURL + "?" + queryString);
        });
    }

    //This method is used to select catalog from fast select and show it on the textbox
    OnSelectCatalogAutocompleteDataBind(item: any): any {
        if (item != undefined) {
            let catalogName: string = item.text;
            let pimCatalogId: number = item.Id;
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.GetProductList(pimCatalogId, catalogName, function (response) {
                $("#productList").html("");
                $("#productList").html(response);
                ZnodeBase.prototype.HideLoader();
            });
        }
    }

    //Method to change default product
    UpdateDefaultProduct(): any {

        let productId: string = DynamicGrid.prototype.GetMultipleSelectedIds();
        let parentProductId: string = $("#ProductId").val()
        let dispalyOrder: string = "99";
        if (productId.split(",")[0] == "") {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectAtleastOneRecord"), 'error', isFadeOut, fadeOutTime);
        }
        else if (productId.split(",").length == 1) {
            let rowId: string = $("[data-swhgcontainer|='View_ManageProductTypeList']").find("input:checked").parents('tr').find("td[class='productId']").text();
            let isDefault: string = "false";

            let associatedAttributeIds = [];
            $("#ConfigureAttributeCheckboxes").find('input').each(function () {
                if ($(this).is(":checked")) {
                    associatedAttributeIds.push($(this).attr('id'));
                    dispalyOrder = $("#rowcheck_" + productId).closest('tr').find('.DisplayOrder').find('input').val();
                }
            });
            let className: string = $("[data-swhgcontainer|='View_ManageProductTypeList']").find("input:checked").parents('tr').find("td[class='isDefaultVariant']").find('i').attr('class');
            if (className == "z-inactive")
                isDefault = "true";

            let jsonData = [{ "PimProductTypeAssociationId": productId, "ProductId": parentProductId, "RelatedProductId": rowId, "DisplayOrder": dispalyOrder, "IsDefault": isDefault }];
            let jsonString = JSON.stringify(jsonData);
            Endpoint.prototype.UpdateAssociatedProducts(productId, parentProductId, jsonString, "0", rowId, function (response) {
                $("#View_ManageProductTypeList #refreshGrid").click();
                DynamicGrid.prototype.ClearCheckboxArray();
                if (response.status) {
                    if (isDefault == "true") {
                        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SuccessSetDefaultVariant"), 'success', isFadeOut, fadeOutTime);
                    }
                    else
                        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SuccessRemoveDefaultVariant"), 'success', isFadeOut, fadeOutTime);
                }
                else {
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, 'error', isFadeOut, fadeOutTime);
                }
            });
        }
        else if (productId.split(",").length > 1) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectAnyOneProductForDefault"), 'error', isFadeOut, fadeOutTime);
        }
    }

}

$(document).on("change", "#UnassociatedProductsDynamic .grid-row-checkbox", function () {
    var productId = $(this).attr('id').split('_')[1];
    Products.prototype.UnassociatedProductsChange(productId);
});

enum UnAssociatedProductListType {
    Addon,
    Link,
    Assoicatedproducts
}

