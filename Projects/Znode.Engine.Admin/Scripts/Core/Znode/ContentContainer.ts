declare function reInitializationMce(): any;
var isChangedWidget: boolean = false;
var attributeData: string = "";
class ContentContainer extends ZnodeBase {

    constructor() {
        super();
    }

    GetVariants() {
        var contentContainerId = $("#ContentContainerId").val()
        ZnodeBase.prototype.BrowseAsidePoupPanel('/ContentContainer/GetVariants?contentContainerId=' + contentContainerId, 'VariantPanel');
    }

    GetUnassociatedProfiles(): any {
        var containerKey = $("#ContainerKey").val();
        Endpoint.prototype.GetUnassociatedProfileList(containerKey, function (response) {
            var profileList = response.ProfileList;
            $('#profileList').empty();
            $.each(profileList, function (index, element) {
                $('#profileList').append($(`<option value="${element.Value}"><text>${element.Text}</text></option>`));
            });
        });
    }

    BindVariantDropdown(data): any {
        if (data.message != "") {
            $('#containervarianterrormsg').show().html(data.message);

            ZnodeBase.prototype.HideLoader();
        }
        else {
            ZnodeBase.prototype.CancelUpload('VariantPanel');
            ContentContainer.prototype.GetVariantsList();
        }        
    }

    BindAssociatedAttributeValue(data): any {
        if ($("#IsClosePopup").val() == 'True') {
            window.location.href = $('#backUrl').val();
        } else {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SuccessChangesSaved"), "success", isFadeOut, fadeOutTime);
        }
    }
    GetAssociatedAttributes(): any {
        var entityType = "Widget";
        var entityId = $('#ddlContainerVariants option:selected').val();
        $('#drpContainerTemplate').val($('#ddlContainerVariants option:selected').attr("data-template"))
        $("#EntityId").val($('#ddlContainerVariants option:selected').val());
        $("#EntityType").val("Widget")
        if (entityId > 0) {
            Endpoint.prototype.GetAssociatedVariants(entityId, entityType, function (response) {
                $("#AssociatedGroups").html("");
                $("#AssociatedGroups").html(response);
                reInitializationMce();
                $("#DynamicHeading").text("Edit - " + $('#ddlContainerVariants option:selected').text())
                attributeData = $("#frmGlobalAttribute").serialize();
                $("#variantContainerTemplate").show();
                $("#variantAttributeData").show();
                ZnodeBase.prototype.HideLoader();
            });
        }
        ZnodeBase.prototype.HideLoader();
    }

    DeleteContentContainer(control): any {
        var contentContainerId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (contentContainerId.length > 0) {
            Endpoint.prototype.DeleteContentContainer(contentContainerId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    DeleteVariant(control): any {
        var widgetProfileVariantId = DynamicGrid.prototype.GetMultipleSelectedIds();
        var containerKey = $('#ContainerKey').val();
        if (widgetProfileVariantId.length > 0) {
            Endpoint.prototype.DeleteAssociatedVariant(widgetProfileVariantId, containerKey, function (response) {
                if (response.status) {
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, "success", isFadeOut, fadeOutTime);
                    ContentContainer.prototype.GetVariantsList();
                }
                else {
                    ZnodeBase.prototype.HideLoader();
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, "error", isFadeOut, fadeOutTime);
                }
            });
        }
    }


    ValidateVariant(): any {
     $('#IsActive').prop('checked', true);
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("DefaultActiveStatus"), "error", isFadeOut, fadeOutTime);
    }
    
    fnShowHide(control, divId: string) {
        control = $(divId).parent("div").find(".panel-heading").find("h4");
        if ($(control).hasClass("collapsed")) {
            $(control).removeClass("collapsed");
        }
        else {
            $(control).addClass("collapsed");
        }
        $("#" + divId).children(".panel-body").children(".widgetAtribute").show();
        $("#" + divId).slideToggle();
    }

    SaveEntityAttribute(backURL: string) {
        $("#frmGlobalAttribute").submit();
        ContentContainer.prototype.ValidateForm();
        ZnodeBase.prototype.HideLoader();
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SuccessChangesSaved"), "success", isFadeOut, fadeOutTime);
    }

    ValidateForm(): any {
        if (!$("#frmGlobalAttribute").valid()) {
            $("#frmGlobalAttribute .input-validation-error").closest('.panel-collapse').attr("style", "display:block");
        }
    }

    OnTemplateChange(): any {
        if ($('#drpContainerTemplate').val() != "") {
            $("#drpContainerTemplate").removeClass("input-validation-error");
            $("#errorRequiredTemplate").text('').text("").removeClass("field-validation-error").hide();
        }
        isChangedWidget = true
    }

    SetActiveGroup(group: string) {
        $(this).addClass('active-tab-validation');
    }

    ValidateData(): boolean {
        var isValid = true;
        if ($('#ContentContainerId').val() < 1 && $('#ContainerKey').val() != '') {
            Endpoint.prototype.IsContainerExist($('#ContainerKey').val(), function (response) {
                if (response.data) {
                    $("#ContainerKey").addClass("input-validation-error");
                    $("#errorSpanContainerKey").addClass("error-msg");
                    $("#errorSpanContainerKey").text(ZnodeBase.prototype.getResourceByKeyName("AlreadyExistAttributeContainerKey"));
                    $("#errorSpanContainerKey").show();
                    isValid = false;
                    ContentContainer.prototype.ShowContainerGeneralInformation();
                }
            });
        }
        return isValid;
    }

    ValidateAttributeData(): boolean {
        var isValid = true;
        $('#AssociatedGroups').find("[data-val-required]").each(function () {
            if ($(this).val() === "") {
                $('#' + $(this).attr('id')).addClass('input-validation-error');
                $('#' + $(this).attr('id')).attr('style', 'border: 1px solid rgb(195, 195, 195)');
                $('spam#' + 'errorSpam' + $(this).attr('name')).removeClass('field-validation-valid');
                $('spam#' + 'errorSpam' + $(this).attr('name')).addClass('field-validation-error');
                $('spam#' + 'errorSpam' + $(this).attr('name')).text($(this).attr('data-val-required'));
                isValid = false;
            }
        });
        if(!isValid){
            $(".input-validation-error").parent().parent().parent().parent().parent().parent().each(function () {
                $($(".input-validation-error").parent().parent().parent().parent()).parent().parent().show();
            });
        }
        return isValid;
    }

    public IsGlobal() {
        if ($('#PortalId').val() == "0" && window.location.href.toLowerCase().indexOf("update") !== -1) {
            $("#IsGlobalContentWidget").attr("checked", "checked");
        }
        if ($("#IsGlobalContentWidget").is(":checked")) {
            $("#txtPortalName").val('');
            $("#IsGlobalContentWidget").val("true")
            $('#hdnPortalId').val(0);
            $(".portalsuggestion *").attr('readonly', true);
            $('.portalsuggestion').css({ pointerEvents: "none" })
            $(".fstElement").css({ "background-color": "#e7e7e7" });
            $(".fstElement").removeClass('input-validation-error');
            $("#errorRequiredStore").text('').text("").removeClass("field-validation-error").hide();
            $("#txtPortalName").removeClass('input-validation-error');
            $(".fstToggleBtn").html("Select Store");
            $(".fstResultItem").removeClass('fstSelected');
            $('#PortalId').val("0");
            $('#hdnPortalId').val("0");
            $('.fstToggleBtn').html("All Stores");
            $('#StoreName').val("All Stores");
        }
        else {
            $(".portalsuggestion *").attr('readonly', false);
            $('.portalsuggestion').css({ pointerEvents: "visible" })
            $("#IsGlobalContentWidget").val("false")
            $(".fstElement").css({ "background-color": "#fff" });
            $('.fstToggleBtn').html("Select Store");
        }
    }

    public DeleteWidgetVariant(): void {
        $("#DeleteContainerVariant").modal("show");
    }

    OnSelectPortalResult(item: any): any {
        if (item != undefined) {
            let portalName: string = item.text;
            $('#StoreName').val(portalName);
            Store.prototype.OnSelectStoreAutocompleteDataBind(item);
        }
    }

    SubmitContainerVariantsForm(e): any {
        e.preventDefault();
        if (!($("#profileList").val())) {
            $("#profileListError").removeClass('hidden');
        }
        else {
            $("#profileListError").addClass('hidden');
            $("#formContainerVariants").submit();
        }
    }

    GetGlobalAttributeData(): any {
        var entityType = "Content Containers";
        var selectedFamilyCode = $('#drpFamilyName option:selected').val();
        if (selectedFamilyCode) {
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.GetGlobalAttributesForDefaultVariantData(selectedFamilyCode, entityType, function (response) {
                $("#AssociatedGroups").html("");
                $("#AssociatedGroups").html(response);
                reInitializationMce();
                attributeData = $("#frmCreateContentContainer").serialize();
                $("#variantContainerTemplate").show();
                $("#variantAttributeData").show();
                ZnodeBase.prototype.HideLoader();
            });
            ZnodeBase.prototype.HideLoader();
        }
    }

    AddDefaultVariantDetails(isPageLoad: boolean = false): any {
        let isValidate: boolean = true;
        $('#divContainerGeneralInformation').find("[data-val-required]").each(function () {
            if ($(this).attr('type') != 'hidden' && $(this).val() === "") {
                $('#' + $(this).attr('id')).addClass('input-validation-error');
                $('#' + $(this).attr('id')).attr('style', 'border: 1px solid rgb(195, 195, 195)');
                $('span#' + 'errorSpan' + $(this).attr('name')).removeClass('field-validation-valid');
                $('span#' + 'errorSpan' + $(this).attr('name')).addClass('field-validation-error');
                $('span#' + 'errorSpan' + $(this).attr('name')).text($(this).attr('data-val-required'));
                isValidate = false;
            }
        });
        if (isValidate) {
            $("#addVariantHeading").text(ZnodeBase.prototype.getResourceByKeyName("AddDefaultVariantHeading") + $("#ContainerName").val());
            $("#AddContainer").hide();
            $("#divContainerGeneralInformation").hide();
            $("#divAddDefaultVariant").show();
            $("#AddVariant").show();
        }
        else {
            return false;
        }
    }

    ShowContainerGeneralInformation(): any {
        $("#AddContainer").show();
        $("#divContainerGeneralInformation").show();
        $("#divAddDefaultVariant").hide();
        $("#AddVariant").hide();
    }

    GetVariantsList(): any {
        ZnodeBase.prototype.CancelUpload('VariantPanel');
        var containerKey = $("#ContainerKey").val();

        Endpoint.prototype.GetVariantsList(containerKey, function (response) {
            $("#variantListDiv").html('');
            $('#variantListDiv').html(response);
            $("#grid").find("tr").addClass('preview-link');
        });
    }

    HideIsDefaultVariantColumn(): void {
        $('#grid').find(".IsDefaultVarinat").hide(); 
    }

    SubmitManageVariantsForm(control): any {
        var saveButtonId = $(control).attr("id");
        if (saveButtonId == "btnSaveNClose") {
            $("#IsRedirectToEditScreen").val('True');
        }
        if (!ContentContainer.prototype.ValidateAttributeData()) {
            $(".input-validation-error").parent().parent().parent().parent().parent().each(function () {
                $($(".input-validation-error").parent().parent().parent().parent()).parent().parent().show();
            });
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ContainerVariantValidationMessage"), "error", isFadeOut, fadeOutTime);
        } else {
            attributeData = $("#formContainerVariants").serialize();
            $("#formContainerVariants").submit();
        }
    }

    GetAssociatedVariantData(): any {
        var variantId = $('#ProfileVariantId').val();
        var localeId = $('#ddl_locale_list_containers option:selected').val();
        ZnodeBase.prototype.ShowLoader();
        Endpoint.prototype.GetAssociatedVariantData(variantId, localeId, function (response) {
            if (response != null) {
                $("#AssociatedGroups").html("");
                $("#AssociatedGroups").html(response.html);
                $("#LocaleFamilyCode").val(response.familyCode);
                reInitializationMce();
                attributeData = $("#formContainerVariants").serialize();
                $("#variantContainerTemplate").show();
                $("#variantAttributeData").show();
            }
        });
    }

    //submit the Create Variant form
    SubmitCreateVariantsForm(control): any {
        var isValid = true;
        var saveButtonId = $(control).attr("id");
        if (saveButtonId == "dvSave") {
            $("#IsRedirectToEditVariantScreen").val('True');
        }

        if (ContentContainer.prototype.ValidateData()) {
            if (!ContentContainer.prototype.ValidateAttributeData()) {
                $(".input-validation-error").parent().parent().parent().parent().parent().each(function () {
                    $($(".input-validation-error").parent().parent().parent().parent()).parent().parent().show();
                });
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ContainerVariantValidationMessage"), "error", isFadeOut, fadeOutTime);
            } else {
                $("#frmCreateContentContainer").submit();
            }
        }
    }

    //Activate or Deactivate the variant(s) in case they are deactive or active respectively.
    ActivateDeactivateVariant(isActivateDeactivate: string): any {
        var widgetProfileVariantIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (widgetProfileVariantIds == '' || widgetProfileVariantIds == null || widgetProfileVariantIds == undefined) {
            var activateDeactivateUrl: string = $("#hdnEnableDisableActionURL").val();
            widgetProfileVariantIds = activateDeactivateUrl.split('=')[1].split('&')[0];
        }
        var isActivate = isActivateDeactivate.valueOf() == "true" ? true : false;
        if (widgetProfileVariantIds.length > 0) {
            Endpoint.prototype.ActivateDeactivateVariant(widgetProfileVariantIds, isActivate, function (response) {
                if (response.status) {
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, "success", isFadeOut, fadeOutTime);
                    ContentContainer.prototype.GetVariantsList();
                }
                else {
                    ZnodeBase.prototype.HideLoader();
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, "error", isFadeOut, fadeOutTime);
                }
            });
        }
    }

    //Toggle the icon of variants activate deactivate button
    ToggleActivateDeactivateActionClass(control): any {
        $("tr").each(function () {
            var target = $(this).find("td.grid-action").find('.action-ui').find("[data-managelink='Deactivate']");
            var isActive = $(target).hasClass('z-disable');
            if (isActive) {
                $(target).removeClass('z-enable');
                $(target).addClass('z-disable');
                $(target).attr('title', 'Deactivate');
            } else {
                $(target).removeClass('z-disable');
                $(target).addClass('z-enable');
                $(target).attr('title', 'Activate');
            }
        });
    }

    PublishContentPopup(zPublishAnchor): any {
        zPublishAnchor.attr("href", "#");
        $("#HdnContainerKey").val($(zPublishAnchor).attr("data-parameter").split('&')[0].split('=')[1]);
        $("#PublishContainer").modal('show');
    }

    PublishContentContainer(): any {
        let publishStateFormData: string = 'NONE';
        if ($('#radBtnPublishState').length > 0)
            publishStateFormData = ZnodeBase.prototype.mergeNameValuePairsToString($('#radBtnPublishState').serializeArray());
        Endpoint.prototype.PublishContentContainer($("#HdnContainerKey").val(), publishStateFormData, function (res) {
            DynamicGrid.prototype.RefreshGridOndelete($("#ZnodeCMSContentContainer").find("#refreshGrid"), res);
        });
    }

    UpdateAndPublishContentContainer(): any {
        let publishStateFormData: string = 'NONE';
        if ($('#radBtnPublishState').length > 0)
            publishStateFormData = ZnodeBase.prototype.mergeNameValuePairsToString($('#PublishContainer').find('#radBtnPublishState').serializeArray());
        $("#TargetPublishState").val(publishStateFormData);
        attributeData = $("#frmCreateContentContainer").serialize();
        $("#frmCreateContentContainer").attr("action", "/ContentContainer/UpdateAndPublishContentContainer");
        $("#frmCreateContentContainer").submit();
    }


    PublishContentVariantPopup(zPublishAnchor): any {
        zPublishAnchor.attr("href", "#");
        $("#containerProfileVariantId").val($(zPublishAnchor).attr("data-parameter").split('&')[1].split('=')[1]);

        $("#PublishContainerVariant").modal('show');
    }

    PublishContentContainerVariant(): any {
        let publishStateFormData: string = 'NONE' ;
        if ($('#radBtnPublishState').length > 0)
            publishStateFormData = ZnodeBase.prototype.mergeNameValuePairsToString($('#PublishContainerVariant').find('#radBtnPublishState').serializeArray());
        Endpoint.prototype.PublishContentContainerVariant($("#ContainerKey").val(), $("#containerProfileVariantId").val(), publishStateFormData, function (res) {
            DynamicGrid.prototype.RefreshGridOndelete($("#ZnodeCMSAssociatedVariant").find("#refreshGrid"), res);
        });
    }

    UpdateAndPublishContainerVariant(): any {
        let publishStateFormData: string = 'NONE';
        if ($('#radBtnPublishState').length > 0)
            publishStateFormData = ZnodeBase.prototype.mergeNameValuePairsToString($('#radBtnPublishState').serializeArray());
        $("#TargetPublishState").val(publishStateFormData);
        attributeData = $("#formContainerVariants").serialize();
        $("#formContainerVariants").attr("action", "/ContentContainer/UpdateAndPublishContainerVariant");

        if (!ContentContainer.prototype.ValidateAttributeData()) {
            $(".input-validation-error").parent().parent().parent().parent().parent().each(function () {
                $($(".input-validation-error").parent().parent().parent().parent()).parent().parent().show();
            });
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ContainerVariantValidationMessage"), "error", isFadeOut, fadeOutTime);
        } else {
            attributeData = $("#formContainerVariants").serialize();
            $("#formContainerVariants").submit();
        }
    }

    ShowPublishTaskSchedularDetails(data: any): any {

        data.attr("href", "#");

        var schedulerName: string = data.attr("data-parameter").split('&')[0].split('=')[1];

        var schedularCallFor: string = data.attr("data-parameter").split('&')[1].split('=')[1];

        var footer = "<button type='button' class='popup-panel-close' onclick='ZnodeBase.prototype.CancelUpload(" + '"divCreateSchedularForContentContainer"' + ")'><i class='z-close'></i></button>";
        ZnodeBase.prototype.ShowLoader();
        var url = "/ContentContainer/CreateScheduler?ConnectorTouchPoints=" + schedulerName + "&schedulerCallFor=" + schedularCallFor;
        Endpoint.prototype.GetPartial(url, function (response) {
            var htmlContent = footer + response;
            $("#divCreateSchedularForContentContainer").html(htmlContent);
            $($("#divCreateSchedularForContentContainer").find("a.grey")).attr("href", "#");
            $($("#divCreateSchedularForContentContainer").find("a.grey")).attr("onclick", "ZnodeBase.prototype.CancelUpload('divCreateScheduler')");
            $("#divCreateSchedularForContentContainer").show(700);
            $("body").append("<div class='modal-backdrop fade in'></div>");
            ZnodeBase.prototype.HideLoader();
        });
    }

    ContentContainerCreateScheduler(): any {
        var isValid: boolean = SearchConfiguration.prototype.ValidateSchedulerData();
        var schedulerName: string = $("#SchedulerName").val();
        if (isValid) {
            var erpTaskSchedulerViewModel = {
                "ERPTaskSchedulerId": $("#ERPTaskSchedulerId").val(),
                "IndexName": $("#IndexName").val(),
                "IsEnabled": $("#divSchedulerSetting #IsActive").prop('checked'),
                "SchedulerCallFor": $("#SchedulerCallFor").val(),
                "PortalId": $("#PortalId").val(),
                "PortalIndexId": $("#PortalIndexId").val(),
                "SchedulerFrequency": $('[name=SchedulerFrequency]:checked').val(),
                "SchedulerName": $("#SchedulerName").val(),
                "StartDate": $("#StartDate").val(),
                "StartTime": $("#StartTime").val(),
                "TouchPointName": $("#TouchPointName").val(),
                "SchedulerType": $("#SchedulerType").val(),
                "CronExpression": $("#txtCronExpression").val(),
                "HangfireJobId": $("#HangfireJobId").val()
            };
            if (parseInt($("#ERPTaskSchedulerId").val(), 10) > 0) {
                Endpoint.prototype.EditSearchScheduler(erpTaskSchedulerViewModel, function (response) {
                    if (response.status) {
                        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SchedulerUpdatedSuccessfully"), response.status ? 'success' : 'error', isFadeOut, fadeOutTime);
                        ZnodeBase.prototype.CancelUpload('divCreateSchedularForContentContainer');
                    }
                    else {
                        $("#createSchedulerError").text(response.message);
                        $("#createSchedulerError").show();
                    }
                });
            }
            else {
                Endpoint.prototype.CreateSearchScheduler(erpTaskSchedulerViewModel, function (response) {
                    if (response.status) {

                        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SchedulerCreatedSuccessfully"), response.status ? 'success' : 'error', isFadeOut, fadeOutTime);
                        ZnodeBase.prototype.CancelUpload('divCreateSchedularForContentContainer');
                        $("#schedulerNameText").val(schedulerName);
                        $("#schedulerName").removeClass("hidden");
                        $(".createScheduler").html("");
                        $(".createScheduler").html("<i class='z-add-circle'></i>" + ZnodeBase.prototype.getResourceByKeyName("UpdateScheduler"));
                        $('#grid tbody tr').each(function () {
                            var target = $(this).find("td.grid-action").find('.action-ui').find("[data-managelink='Create Scheduler']");
                            var dataparameter = $(target).attr("data-parameter");
                            if (dataparameter != undefined) {
                                var schedulerQueryStringArray = $(target).attr("data-parameter").split('&');
                                if (schedulerQueryStringArray !== undefined) {
                                    var schedulerPramArray = schedulerQueryStringArray[0].split('=');
                                    if (schedulerPramArray !== undefined) {
                                        var schedulerName = schedulerPramArray[1];
                                        if (schedulerName !== undefined && schedulerName.toLowerCase() === erpTaskSchedulerViewModel.TouchPointName.toLowerCase()) {
                                            $(target).prop('title', 'Update Scheduler');
                                            $(target).data('managelink', 'Update Scheduler')
                                        }
                                    }
                                }
                            }
                        });
                    }
                    else {
                        $("#createSchedulerError").text(response.message);
                        $("#createSchedulerError").show();
                    }
                });
            }
        }
    }
}