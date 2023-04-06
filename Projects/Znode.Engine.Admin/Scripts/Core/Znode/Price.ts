class Price extends ZnodeBase {
    _endPoint: Endpoint;
    constructor() {
        super();

    }
    Init() {
        Inventory.prototype.AutocompleteSku();
        Price.prototype.EditAssociatedStoresPrecedence();
        Price.prototype.EditAssociatedCustomerPrecedence();
        Price.prototype.EditAssociatedAccountPrecedence();
        Price.prototype.EditAssociatedProfilePrecedence();
        Price.prototype.HideLoader();
        $(document).on("change", "#txtUpload", function () {
            if (Import.prototype.ValidateTemplateName()) {
                $("#error-templatename").html('');
                if (Import.prototype.ValidateImportedFileType()) {
                    return true;
                }
            }
        });
    }

    CheckFileValidataion(): boolean {
        if ($("#fileName").text().length > 0) {
            if (Import.prototype.ValidateImportedFileType()) {
                return Import.prototype.ValidateTemplateName();
            } else {
                return false;
            }
        } else {
            return true;
        }
    }

    DeletePrice(control): any {
        var priceListId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (priceListId.length > 0) {
            Endpoint.prototype.DeletePrice(priceListId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    DeleteMultipleSKUPrice(control, priceListId: number): any {
        var priceId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (priceId.length > 0) {
            Endpoint.prototype.DeleteMultipleSKUPrice(priceId, priceListId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    ClearEditTierPrice(): any {
        $('#createFormId input[type="text"]').val("");
        $('#priceId').html("");
        $('#valMaxQuantity').html("");
        $('#valMinQuantity').html("");
        $("#valMaxQty").html("");
        $("#validateMinQty").html("");
    }


    CreateMessage(): any {
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("TierPriceCreatedSuccessfully"), "success", isFadeOut, fadeOutTime);
    }

    ErrorMessage(): any {
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("TierPriceNotCreatedSuccessfully"), "error", isFadeOut, fadeOutTime);
    }

    GetTierPriceData(): any {
        var PriceTierList = [];
        var xml = "<rows>";
        $("#tierPrice tbody tr").each(function () {
            var priceTierId = 0;
            if ($(this).find('.priceTierId').val() != "")
                priceTierId = $(this).find('.priceTierId').val();

            if ($(this).find('#tierQuantity').val() != "" && $(this).find('#priceTier').val() != "") {
                xml = xml + "<row><Quantity>" + $(this).find('#tierQuantity').val() + "</Quantity><Price>" + $(this).find('#priceTier').val() + "</Price><Custom1>" + $(this).find('#priceTier1').val() + "</Custom1><Custom2>" + $(this).find('#priceTier2').val() + "</Custom2><Custom3>" + $(this).find('#priceTier3').val() + "</Custom3><PriceTierId>" + priceTierId + "</PriceTierId></row>";
            }
        });
        xml = xml + "</rows>";
        $('#PriceTierListData').val(xml);
    }

    AssociatePriceStore(priceListId: number) {
        var storeIds = "";
        var storeIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (storeIds.length > 0) {
            Endpoint.prototype.AssociateStoreList(priceListId, storeIds, function (res) {
                $("#associatestorelist").hide(700);
                window.location.href = window.location.protocol + "//" + window.location.host + "/Price/GetAssociatedStoreList?priceListId=" + priceListId + "&listName=" + $("#ListName").val();
            });
        }
        else {
            $("#associatestorelist").slideDown(200);
            $("#associatedTaxStoreId").slideDown(200);
        }
    }

    DeleteAssociatedStores(control, priceListId: number): any {
        var priceListPortalId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (priceListPortalId.length > 0) {
            Endpoint.prototype.DeleteAssociatedStores(priceListPortalId, priceListId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    AssociatePriceCustomer(priceListId: number) {
        var customerIds = "";
        var customerIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (customerIds.length > 0) {
            Endpoint.prototype.AssociateCustomerList(priceListId, customerIds, function (res) {
                $("#associatecustomerlist").hide(700);
                window.location.href = window.location.protocol + "//" + window.location.host + "/Price/GetAssociatedCustomerList?priceListId=" + priceListId + "&listName=" + $("#ListName").val();
            });
        }
        else {

            $("#associatecustomerlist").slideDown(200);
            $("#associatedCustomerId").slideDown(200);
        }
    }

    DeleteMultipleCustomerPrice(control, priceListId: number): any {
        var priceListUserId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (priceListUserId.length > 0) {
            Endpoint.prototype.DeleteMultipleCustomerPrice(priceListUserId, priceListId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    AssociatePriceAccount(priceListId: number) {
        var accountIds = "";
        var accountIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (accountIds.length > 0) {
            Endpoint.prototype.AssociateAccountList(priceListId, accountIds, function (res) {
                $("#associateaccountlist").hide(700);
                window.location.href = window.location.protocol + "//" + window.location.host + "/Price/GetAssociatedAccountList?priceListId=" + priceListId + "&listName=" + $("#ListName").val();
            });
        }
        else {
            $("#associateaccountlist").slideDown(200);
            $("#associatedAccountId").slideDown(200);
        }
    }

    DeleteMultipleAccountPrice(control, priceListId: number): any {
        var priceListAccountId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (priceListAccountId.length > 0) {
            Endpoint.prototype.DeleteMultipleAccountPrice(priceListAccountId, priceListId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    AssociatePriceProfile(priceListId: number) {
        var profileIds = "";
        var profileIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (profileIds.length > 0) {
            Endpoint.prototype.AssociateProfileList(priceListId, profileIds, function (res) {
                $("#associateprofilelist").hide(700);
                window.location.href = window.location.protocol + "//" + window.location.host + "/Price/GetAssociatedProfileList?priceListId=" + priceListId + "&listName=" + $("#ListName").val();
            });
        }
        else {
            $("#associateprofilelist").slideDown(200);
            $("#associatedProfileId").slideDown(200);
        }
    }

    ShowHideCustomColumn(obj: string) {     
        if ($("#" + obj).prop("checked") == true)
            $("." + obj).show();
        else {
            $("." + obj).hide();
            $("#tierPrice tbody tr").each(function () {
                if(obj=="Custom1")
                    $(this).find('#priceTier1').val($(this).find('#priceTier1').attr('data-Custom1'))
                if (obj == "Custom2")
                    $(this).find('#priceTier2').val($(this).find('#priceTier2').attr('data-Custom2'))
                if (obj == "Custom3")
                    $(this).find('#priceTier3').val($(this).find('#priceTier3').attr('data-Custom3'))
            });
        }
    }
   
    DeleteAssociatedProfiles(control, priceListId: number): any {
        var priceListProfileId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (priceListProfileId.length > 0) {
            Endpoint.prototype.DeleteAssociatedProfiles(priceListProfileId, priceListId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    ValidateMinQuantity(): any {
        var maxval = parseInt($("#PriceTier_MaxQuantity").val());
        var minval = parseInt($("#PriceTier_MinQuantity").val());

        if ($("#PriceTier_MinQuantity").val() == "") {
            $("#validateMinQty").html("");
            return true;
        }      
        else if (!isNaN(parseInt("minval")) && (maxval <= minval)) {
            $("#validateMinQty").html(ZnodeBase.prototype.getResourceByKeyName("MinimumQuantity"));
            return false;
        }
        else {
            $("#validateMinQty").html("");
            $("#valMaxQty").html("");
            return true;

        }
    }

    ValidateMaxQuantity(): any {
        var maxval = $("#PriceTier_MaxQuantity").val();
        var minval = parseInt($("#PriceTier_MinQuantity").val());

        if ($("#PriceTier_MaxQuantity").val() == "") {
            $("#valMaxQty").html("");
            return true;
        } 
        else if (!isNaN(parseInt("maxval")) && (maxval < minval)) {
            $("#valMaxQty").html(ZnodeBase.prototype.getResourceByKeyName("MaximumQuantity"));
            return false;
        }
        else {
            $("#valMaxQty").html("");
            $("#validateMinQty").html("");
            return true;
        }
    }

    ValidateSubmit(): any {
        var minQuantityValidation = Price.prototype.ValidateMinQuantity();
        var maxQuantityValidation = Price.prototype.ValidateMaxQuantity();

        if (minQuantityValidation && maxQuantityValidation)
            return true;
        else {
            $("#valMaxQty").html(ZnodeBase.prototype.getResourceByKeyName("MaximumQuantity"));
            $("#validateMinQty").html(ZnodeBase.prototype.getResourceByKeyName("MinimumQuantity"));
            return false;
        }

    }

    AssociatePriceListToStore(portalId: number) {
        var priceListIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (priceListIds.length > 0) {
            Endpoint.prototype.AssociatePriceListToStore(portalId, priceListIds, function (res) {
                if (res.status) {
                    Endpoint.prototype.GetAssociatedPriceListForStore(portalId, function (res) {
                        $("#associateStore").html(res);
                        DynamicGrid.prototype.ClearCheckboxArray();
                    });
                }
                $("#DivGetUnAssociatedPriceListForStore").hide(700);
                ZnodeBase.prototype.RemovePopupOverlay();
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, res.status ? 'success' : 'error', isFadeOut, fadeOutTime);
                ZnodeBase.prototype.RemoveAsidePopupPanel();
            });
        }
        else {
            $("#associatedTaxStoreId").slideDown(200);
        }
    }

    AssociatePriceListToProfile(portalId: number) {
        var profileId = $("#profileId :selected").val();
        var priceListIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (priceListIds.length > 0) {
            Endpoint.prototype.AssociatePriceListToProfile(profileId, priceListIds, portalId, function (res) {
                if (res.status) {
                    Endpoint.prototype.GetAssociatedPriceListForProfile(portalId, profileId, function (res) {
                        $("#associateProfile").html(res);
                        DynamicGrid.prototype.ClearCheckboxArray();
                    });
                }
                $("#associatepricelistforprofile").hide(700);
                ZnodeBase.prototype.RemovePopupOverlay();
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, res.status ? 'success' : 'error', isFadeOut, fadeOutTime);
            });
        }
        else {
            $("#associatedProfile").slideDown(200);
        }
    }

    GetUnAssociatedPriceListForStore(PortalId: number): any {
        DynamicGrid.prototype.ClearCheckboxArray();
        Endpoint.prototype.GetUnAssociatedPriceListForStore(PortalId, function (res) {
            if (res != null && res != "") {
                var message = $('#ErrorMessage', res).val();
                if (message != null && message != "")
                    return ZnodeNotification.prototype.DisplayNotificationMessagesHelper(message, 'error', isFadeOut, fadeOutTime);
                else
                    Price.prototype.AppendResponseToAsidePanel(res);

                if ($("#UnAssociatedPriceList").find("tr").length == 0) {
                    $("#UnAssociatedPriceList").parent().next().hide();
                    $("#UnAssociatedPriceList").find(".filter-component").hide();
                }
                $("body").append("<div class='modal-backdrop fade in'></div>");
            }
        });
    }

    AppendResponseToAsidePanel(res: any) {
        ZnodeBase.prototype.ShowLoader();
        $("#" + 'DivGetUnAssociatedPriceListForStore').html('');
        $("#" + 'DivGetUnAssociatedPriceListForStore').append(res);
        $("#" + 'DivGetUnAssociatedPriceListForStore').slideDown(200);
        $("body").css('overflow', 'hidden');
        ZnodeBase.prototype.HideLoader();
        GridPager.prototype.Init();
    }


    GetUnAssociatedPriceListForProfile(portalId: number): any {
        DynamicGrid.prototype.ClearCheckboxArray();
        var profileId = $("#profileId :selected").val();
        if (parseInt(profileId) < 1) {
            $("#errSelectProfile").html(ZnodeBase.prototype.getResourceByKeyName("SelectAssociateProfileError"));
            return false;
        }
        else {
            $("#errSelectProfile").html("");
            ZnodeBase.prototype.BrowseAsidePoupPanel('/Store/GetUnAssociatedPriceListForProfile?profileId=' + profileId + '&portalId=' + portalId + '', 'associatepricelistforprofile');
        }
    }

    GetPriceListAssociatedtoProfile(portalId: number): any {
        var profileId = $("#profileId :selected").val();
        Endpoint.prototype.GetAssociatedPriceListForProfile(portalId, profileId, function (response) {
            $("#ProfileBasePriceManagement").html(response);
        });
    }

    RemoveAssociatedPriceListFromStore(control): any {
        var priceListPortalId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (priceListPortalId.length > 0) {
            Endpoint.prototype.RemoveAssociatedPriceListFromStore(priceListPortalId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    EditAssociatedStoresPrecedence(): any {
        $("section[update-container-id=ZnodePriceListPortal]").find(".z-edit").click(function (e) {
            e.preventDefault();
            var priceListPortalId = $(this).attr("data-parameter").split('&')[0].split('=')[1];
            Endpoint.prototype.EditAssociatedStoresPrecedence(priceListPortalId, function (res) {
                $("#storePrecedence").modal("show");
                $("#storePrecedence").html(res);
            });
        });
    }

    EditAssociatedStoresPrecedenceResult(data: any) {
        $("#storePrecedence").modal("hide");
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(data.Message, "success", isFadeOut, fadeOutTime);
        ZnodeBase.prototype.RemovePopupOverlay();
        Price.prototype.AssociatedStoresList();
    }

    AssociatedStoresList() {
        Endpoint.prototype.AssociatedStoresList(parseInt($("#PriceListId").val(), 10), $("#ListName").val(), function (response) {
            $("#ZnodePriceListPortal").html('');
            $("#ZnodePriceListPortal").html(response);
            GridPager.prototype.UpdateHandler();
        });
    }

    EditAssociatedCustomerPrecedence(): any {
        $("section[update-container-id=PriceListAccount]").find(".z-edit").click(function (e) {
            e.preventDefault();
            var priceListUserId = $(this).attr("data-parameter").split('&')[0].split('=')[1];
            Endpoint.prototype.EditAssociatedCustomerPrecedence(priceListUserId, function (res) {
                $("#customerPrecedence").modal("show");
                $("#customerPrecedence").html(res);
            });
        });
    }

    EditAssociatedCustomerPrecedenceResult(data: any) {
        $("#customerPrecedence").modal("hide");
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(data.Message, "success", isFadeOut, fadeOutTime);
        ZnodeBase.prototype.RemovePopupOverlay();
        Price.prototype.AssociatedCustomerList();
    }

    AssociatedCustomerList() {
        Endpoint.prototype.AssociatedCustomerList(parseInt($("#PriceListId").val(), 10), $("#ListName").val(), function (response) {
            $("#PriceListAccount").html('');
            $("#PriceListAccount").html(response);
            GridPager.prototype.UpdateHandler();
        });
    }

    EditAssociatedAccountPrecedence(): any {
        $("section[update-container-id=AccountPriceList]").find(".z-edit").click(function (e) {
            e.preventDefault();
            var priceListAccountId = $(this).attr("data-parameter").split('&')[0].split('=')[1];
            Endpoint.prototype.EditAssociatedAccountPrecedence(priceListAccountId, function (res) {
                $("#accountPrecedence").modal("show");
                $("#accountPrecedence").html(res);
            });
        });
    }

    EditAssociatedAccountPrecedenceResult(data: any) {
        $("#accountPrecedence").modal("hide");
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(data.Message, "success", isFadeOut, fadeOutTime);
        ZnodeBase.prototype.RemovePopupOverlay();
        Price.prototype.AssociatedAccountList();
    }

    AssociatedAccountList() {
        Endpoint.prototype.AssociatedAccountList(parseInt($("#PriceListId").val(), 10), $("#ListName").val(), function (response) {
            $("#AccountPriceList").html('');
            $("#AccountPriceList").html(response);
            GridPager.prototype.UpdateHandler();
        });
    }

    EditAssociatedProfilePrecedence(): any {
        $("section[update-container-id=ZnodePriceListProfile]").find(".z-edit").click(function (e) {
            e.preventDefault();
            var priceListProfileId = $(this).attr("data-parameter").split('&')[0].split('=')[1];
            Endpoint.prototype.EditAssociatedProfilePrecedence(priceListProfileId, function (res) {
                $("#profilePrecedence").modal("show");
                $("#profilePrecedence").html(res);
            });
        });
    }

    EditAssociatedProfilePrecedenceResult(data: any) {
        $("#profilePrecedence").modal("hide");
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(data.Message, "success", isFadeOut, fadeOutTime);
        ZnodeBase.prototype.RemovePopupOverlay();
        Price.prototype.AssociatedProfileList();
    }

    AssociatedProfileList() {
        Endpoint.prototype.AssociatedProfileList(parseInt($("#PriceListId").val(), 10), $("#ListName").val(), function (response) {
            $("#ZnodePriceListProfile").html('');
            $("#ZnodePriceListProfile").html(response);
            GridPager.prototype.UpdateHandler();
        });
    }

    ValidatePrecedence(): any {
        var precedence = parseInt($("#Precedence").val());
        var regex = new RegExp("^([0-9]{0,3})$");

        if (regex.test($("#Precedence").val()) && ($("#Precedence").val() != "")) {
            $("#valPrecedence").html("");
            $("#errorPrecedence").html("");
        }
        else {
            $("#errorPrecedence").html(ZnodeBase.prototype.getResourceByKeyName("ErrorPrecedence"));
            return false;
        }
    }

    RemoveAssociatedPriceListFromProfile(control): any {
        var priceListProfileId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (priceListProfileId.length > 0) {
            Endpoint.prototype.RemoveAssociatedPriceListFromProfile(priceListProfileId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    TierPriceAddResult(data: any) {
        Endpoint.prototype.PriceTierList(parseInt($("#TierPriceListId").val(), 10), $("#SKUTierPrice").val(), function (response) {
            $("#tierPriceList").html('');
            $("#tierPriceList").html(response);
            if (!data.status) {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(data.message, "error", data.status, 5000);
            }
            else {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(data.message, "success", data.status, 5000);
            }

            GridPager.prototype.UpdateHandler();
        });
    }
    //Append Ajax Respose to tier price list.
    AppendTierPriceUsingLazyLoading() {
        var pageNumber = parseInt($("#pageNumber").val(), 10);
        if(pageNumber==1) {
            var endOfRecord = false;
        }
        if (!endOfRecord) {              
            pageNumber++;
            Endpoint.prototype.PriceTierListUsingLazyLoading(parseInt($("#PriceListId").val(), 10), $("#PriceSKU_SKU").val(), pageNumber, function (response) {
                if (!response || response.html.trim()=="") {
                    endOfRecord = true;
                }
                else {
                    $("#tierPriceQuantity").append(response.html);
                    $("#pageNumber").val(pageNumber);
                }
                });           
        }
    }


    ExportPriceData(): any {
        var listCode = "";
        $("#grid").find("tr").each(function () {
            if ($(this).find(".grid-row-checkbox").length > 0) {
                if ($(this).find(".grid-row-checkbox").is(":checked")) {
                    listCode = $(this).find("td[class='priceListCode']").html();
                }
            }
        });
        var priceListIds = [];
        $("#ZnodePriceList").find("tr").each(function () {
            if ($(this).find(".grid-row-checkbox").length > 0) {
                if ($(this).find(".grid-row-checkbox").is(":checked")) {
                    var id = $(this).find(".grid-row-checkbox").attr("id").split("_")[1];
                    priceListIds.push(id);
                }
            }
        });

        if (priceListIds.length == 1) {
            window.location.href = "/Price/ExportPriceData?priceListIds=" + priceListIds + "&listCode=" + encodeURIComponent(listCode);
        }
        else {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorSelectAtMostOnePriceList"), "error", false, fadeOutTime);
        }
        setTimeout(function () { ZnodeBase.prototype.HideLoader() }, 1000);
    }

    DownloadImportErrorList(): any {
        var priceListId = $("#PriceListId").val();
        var inventoryListId = $("#InventoryListId").val();
        if (priceListId != null && priceListId != "") {
            window.location.href = "/Price/DownLoadInValidImportPriceData?priceListId=" + priceListId;
        }
        else if (inventoryListId != null && inventoryListId != "") {
            window.location.href = "/Inventory/DownLoadInValidImportInventoryData?inventoryListId=" + inventoryListId;
        }
    }

    GetUnassociatedStoreList(): any {
        $('#associatestorelist').modal('show');
        if ($("#modelAssociatedStore").find("tr").length == 0) {
            $("#modelAssociatedStore").find(".modal-footer").hide();
            $("#modelAssociatedStore").find(".filter-component").hide();
        }
    }

    PriceSKuList() {
        $("#AssociatedPriceSKUList").html(Constant.innerLoderHtml);
        Endpoint.prototype.PriceSKUList(parseInt($("#PriceListId").val(), 10), $("#ListName").val(), function (response) {
            $("#AssociatedPriceSKUList").html('');
            $("#AssociatedPriceSKUList").html(response);
            GridPager.prototype.UpdateHandler();
        });
    }

    GetSelectedSKU(): void {
        $("#grid").find("tr").click(function () {
            var sku: string = $(this).find("td[class='skucolumn']").html();
            var pimProductId: string = $(this).find("td")[0].innerHTML;
            var productType: string = $(this).find("td[class='productType']").html();
            $('#txtSKU').val(sku.match(/>(.*?)</)[1]);
            $('#hdnProductId').val(pimProductId);
            $('#skuList').hide(700);
            Inventory.prototype.isSKUValid = true;
           
            if (productType == "Configurable Product" || productType == "Grouped Product") {
                Endpoint.prototype.GetPriceBySku(pimProductId, sku.match(/>(.*?)</)[1], productType, function (response) {
                    $('#valSalesPrice').val(response.data.SalesPrice);
                    $('#valRetailPrice').val(response.data.RetailPrice);
                });
            }
            else {
                $('#valSalesPrice').val("");
                $('#valRetailPrice').val("");
            }

            var priceListId: number = $('#PriceListId').val();
            if (priceListId <= 0 || priceListId == undefined) {
                var isDownloadable: string = $(this).find("td[class='isDownloadable']")[0].children[0].className;
                if (isDownloadable == "z-active") {
                    $('#Quantity').val("0");
                    $("#Quantity").prop("readonly", true);                  
                    $("#WarehouseId").prop("disabled", true);
                    $("#BackOrderQuantity").val("0");
                    $("#BackOrderQuantity").prop("readonly", true); 
                    $("#BackOrderExpectedDate").val("");
                    $("#BackOrderExpectedDate").removeClass("datepicker");
                    $("#BackOrderExpectedDate").attr("disabled", true);
                    $("#IsDownloadable").val("true");
                }
                else {
                    $('#Quantity').val("");
                    $("#Quantity").prop("readonly", false);  
                    $("#WarehouseId").prop("disabled", false);
                    $("#BackOrderQuantity").val("0");
                    $("#BackOrderQuantity").prop("readonly", false);
                    $("#BackOrderExpectedDate").val("");
                    $("#BackOrderExpectedDate").addClass("datepicker");
                    $("#BackOrderExpectedDate").attr("disabled", false);
                    $("#IsDownloadable").val("false");
                }
            }

            Products.prototype.HideErrorMessage($("#txtSKU"), $("#valSKU"));
            ZnodeBase.prototype.RemovePopupOverlay();
        });
    }

    HideLoader(): void {
        $("#priceSample").click(function () {
            $('#loading-div-background').removeClass('overlay');
            $('#loading-div').removeClass('loader');
            $("#loading-div-background").empty();
        });
    }

    ValidateTierPrice(): any {
        var rowLength = $('#tierPriceQuantity tr').length;
        if (rowLength == 1) {
            if ($("#priceTier").val() == "" && $("#tierQuantity").val() == "") {
                $("#error-price").html("");
                $("#error-quantity").html("");
                return false;
            }
            else if ($("#tierQuantity").val() == "" && $("#priceTier").val() != "") {
                $("#error-price").html("");
                return false;
            }
            else if ($("#tierQuantity").val() != "" && $("#priceTier").val() == "") {
                $("#error-quantity").html("");
                return false;
            }
            else {
                $("#error-price").html("");
                $("#error-quantity").html("");
            }
        }
    }

    public ImportPricing(): void {
        if (Import.prototype.ValidateTemplateName() &&
            Import.prototype.ValidateImportFileType()) {
            let templateName: string = "";
            if ($("#templateList").val() > 0) {
                templateName = $("#templateList option:selected").text();
            } else {
                templateName = $("#TemplateName").val();
            }

            var ImportViewModels = {
                ImportHeadId: $("#ImportHeadId").val(),
                ImportType: $("#ImportType").val(),
                TemplateId: $("#templateList").val(),
                TemplateName: templateName,
                FileName: $("#ChangedFileName").val(),
                PriceListId: $("#PriceListId").val(),
                IsPartialPage: true
            };

            Endpoint.prototype.ImportPost(ImportViewModels, function (res) {
                setTimeout(function () {
                    ZnodeBase.prototype.HideLoader();
                    location.reload();
                }, 900);
            });
        }
    }
}