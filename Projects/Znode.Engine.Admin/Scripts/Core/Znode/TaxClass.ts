class TaxClass extends ZnodeBase {
    _endPoint: Endpoint;
    constructor() {
        super();
        this._endPoint = new Endpoint();
    }

    Init() {
        $("#divAddSKU").modal("hide");
        TaxClass.prototype.AddRule();
    }

    DeleteMultipleTaxClass(control): void {
        var taxClassIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (taxClassIds.length > 0) {
            Endpoint.prototype.DeleteMultipleTaxClass(taxClassIds, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    DeleteMultipleTaxClassSKU(control): void {
        var taxClassSKUId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (taxClassSKUId.length > 0) {
            Endpoint.prototype.DeleteMultipleTaxClassSKU(taxClassSKUId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    TaxClassSKUAddResult(data): void {
        ZnodeBase.prototype.RemovePopupOverlay();
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(data.Message, data.isSuccess ? "success" : "error", isFadeOut, fadeOutTime);
        TaxClass.prototype.TaxClassSKUList();
    }

    TaxClassSKUList(): void {
        Endpoint.prototype.TaxClassSKUList(parseInt($("#TaxClassId").val(), 10), $("#Name").val(), function (response) {
            $("#AssociatedSKUList").html('');
            $("#AssociatedSKUList").html(response);
            GridPager.prototype.UpdateHandler();
        });
    }

    DeleteMultipleTaxRule(control, taxClassId: number): void {
        var taxRuleId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (taxRuleId.length > 0) {
            Endpoint.prototype.DeleteMultipleTaxRule(taxRuleId, taxClassId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    AddRule(): void {
        $("#addTaxRule").on("click", function () {
            var taxClassId = parseInt($("#TaxClassId").val(), 10);
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.AddTaxRule(taxClassId, function (res) {
                $("#divAddRule").show(700);
                $("#divAddRule").html(res);
                ZnodeBase.prototype.HideLoader();
                TaxClass.prototype.ShowHideContainer();
                $("body").css('overflow', 'hidden');
                $("body").append("<div class='modal-backdrop fade in'></div>");
            });
        });
    }

    TaxRuleAddResult(data): void {
        $("#divAddRule").hide(700);
        ZnodeBase.prototype.RemovePopupOverlay();
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(data.Message, data.isSuccess ? "success" : "error", isFadeOut, fadeOutTime);
        TaxClass.prototype.TaxRuleList();
    }

    TaxRuleList(): void {
        $("#AssociatedRuleList").html(Constant.innerLoderHtml);
        Endpoint.prototype.TaxRuleList(parseInt($("#TaxClassId").val(), 10), function (response) {
            $("#AssociatedRuleList").html('');
            $("#AssociatedRuleList").html(response);
            GridPager.prototype.UpdateHandler();
        });
    }

    BindStateList(stateList, countryCode): void {
        Endpoint.prototype.StateListByCountryCode(countryCode, function (response) {
            var stateSelectedVal = '';
            $('#' + stateList).empty();
            for (var i = 0; i < response.length; i++) {
                if (response[i].Value == $("#hdnStateCode").val())
                    $('#' + stateList).append("<option selected  value='" + response[i].Value + "'>" + response[i].Text + "</option>");
                else
                    $('#' + stateList).append("<option  value='" + response[i].Value + "'>" + response[i].Text + "</option>");
            }

        });
    }

    AddSKUs(): void {
        var CheckBoxCollection = [];
        $("div[id=AssociatedTaxClassProductList]").find("#grid").find("tr").each(function () {
            if ($(this).find(".grid-row-checkbox").length > 0) {
                if ($(this).find(".grid-row-checkbox").is(":checked")) {
                    CheckBoxCollection.push($(this).find(".grid-row-checkbox").attr("id").replace("rowcheck_", ""));
                }
            }
        });
        if (CheckBoxCollection.length > 0)
        {
            TaxClass.prototype.SaveSKUs($("#TaxClassId").val(), CheckBoxCollection.join(","));
            ZnodeBase.prototype.RemoveAsidePopupPanel();
        }
        else
            TaxClass.prototype.DisplayNotificationMessagesForTaxClass(ZnodeBase.prototype.getResourceByKeyName("TextSelectSKU"), "error", isFadeOut, fadeOutTime);
    }

    SaveSKUs(taxClassId, SKUs): void {
        Endpoint.prototype.AddTaxClassSKUList(taxClassId, SKUs, function (res) {
            $("#ZnodeShippingSKU").html(res);
            $("#divtaxProductListPopup").hide(700);
            $(".modal-backdrop").remove();
            Endpoint.prototype.TaxClassSKUList(taxClassId, $("#Name").val(), function (response) {
                $("#AssociatedSKUList").html('');
                $("#AssociatedSKUList").html(response);
                GridPager.prototype.UpdateHandler();
            });
            ZnodeBase.prototype.RemovePopupOverlay();
            DynamicGrid.prototype.ClearCheckboxArray();
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, res.status ? "success" : "error", isFadeOut, fadeOutTime);
        });
    }

    DisplayNotificationMessagesForTaxClass(message: string, type: string, isFadeOut: boolean, fadeOutMilliSeconds: number): void {
        var element: any = $(".taxClassMessageBoxContainer");
        $(".taxClassMessageBoxContainer").removeAttr("style");
        $(window).scrollTop(0);
        $(document).scrollTop(0);
        if (element.length) {
            if (message !== "" && message != null) {
                element.html("<p>" + message + "</p>");
                element.find('p').addClass('error-msg');

                if (isFadeOut == null || typeof isFadeOut === "undefined") isFadeOut = true;
                if (fadeOutMilliSeconds == null || typeof fadeOutMilliSeconds === "undefined") fadeOutMilliSeconds = 10000;

                if (isFadeOut == true) {
                    setTimeout(function () {
                        element.fadeOut().empty();
                    }, fadeOutMilliSeconds);
                }
            }
        }
    }

    ShowHideContainer(): void {
        var selectedText: string;
        selectedText = $("#ddlTaxRuleTypeList option:selected").text();
        if (selectedText == 'Sales Tax' || selectedText.indexOf('Select') > -1) {
            $(".tax-rate-class").show();
            $("#ddlCountryList").val($("#hdnCountryCode").val()); 
        } else {
            $(".tax-rate-class").hide();
            $("#ddlCountryList").val("");
        }
    }
}

$(document).off("click", "frmCreateEditTaxClass .z-edit");
$(document).on("click", "#frmCreateEditTaxClass .z-edit", function (e) {
    e.preventDefault();
    var listName = $(this).closest("section").attr("update-container-id");
    var taxClassId = $("#TaxClassId").val();
    if (listName === "ZnodeTaxRule") {
        var dataParam = decodeURIComponent($(this).attr("data-parameter"))
        var taxRuleId = dataParam.split('&')[0].split('=')[1];
        ZnodeBase.prototype.ShowLoader();
        Endpoint.prototype.EditTaxRule(taxRuleId, taxClassId, function (res) {
            $("#divAddRule").show(700);
            $("#divAddRule").html(res);
            ZnodeBase.prototype.HideLoader();
            TaxClass.prototype.ShowHideContainer();
            $("body").css('overflow', 'hidden');
            $("body").append("<div class='modal-backdrop fade in'></div>");
        });
    }
});
