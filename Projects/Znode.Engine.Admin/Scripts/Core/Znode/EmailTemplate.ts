declare function reInitializationMce(): any;
var RESET_PASSWORD: string = "ResetPassword";
class EmailTemplate extends ZnodeBase {
    _Model: any;
    _endPoint: Endpoint;
    isTemplateValid: boolean;
    constructor() {
        super();
    }
    Init() {
        EmailTemplate.prototype.LocaleDropDownChangeForEmailTemplate();
        EmailTemplate.prototype.AutoCompleteEmailTemplates();
        EmailTemplate.prototype.SaveTemplateMapping();
        EmailTemplate.prototype.ShowHideTemplateTokens();
    }

    LocaleDropDownChangeForEmailTemplate() {
        $("#ddl_locale_list_email_template").on("change", function () {
            Endpoint.prototype.EditEmailTemplate($("#EmailTemplateId").val(), $("#ddl_locale_list_email_template").val(), function (response) {
                $('#div_email_template_for_locale').html(response);
                $("#div_email_template_for_locale textarea").attr("wysiwygenabledproperty", "true");
                reInitializationMce();
            });
        });
    }

    DeleteEmailTemplates(control): any {
        var emailTemplateId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (emailTemplateId.length > 0) {
            Endpoint.prototype.DeleteEmailTemplates(emailTemplateId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    PreviewTemplate(): any {
        $("#grid tbody tr td").find(".z-preview").on("click", function (e) {
            e.preventDefault();
            var templateId = $(this).attr("data-parameter").split('&')[0].split('=')[1];
            ZnodeBase.prototype.BrowseAsidePoupPanel('/EmailTemplate/Preview?emailTemplateId=' + templateId + '', 'divEmailTemplatePreview');
        });
    }

    AddNewArea(data: any) {
        data = 0;
        EmailTemplate.prototype.GetAvailableTemplateArea(data);
        $(".MessageBox").remove();
        $("#partial").show();
        $('.thead-div').show();
    }

    GetAvailableTemplateArea(data: any) {
        data = 0;
        Endpoint.prototype.GetAvailableTemplateArea($("#ddl_portal_list_for_email_template").val(), function (response) {
            if (response.status) {
                $("#partial").html(response.html);
                EmailTemplate.prototype.SetAreaMappingAttributes(0, true);
            }
            else {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, 'info', isFadeOut, fadeOutTime);
            }
        });
    }

    EditAreaMapping(data: any) {
        EmailTemplate.prototype.SetAreaMappingAttributes(data, false);
    }

    DeleteAreaMapping(data: any, control: any) {
        Endpoint.prototype.DeleteEmailTemplateAreaMapping(data, function (response) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, response.status ? 'success' : 'error', isFadeOut, fadeOutTime);
            if (response.status) {
                $(control).closest("form").remove();
            }
            WebSite.prototype.DisplayNoRecordFoundMessage();
        });
    }

    CancelNewAddAreaMapping(data: any, control: any) {
        var EmailTemplateMapperId = data.split('_')[1];
        if (EmailTemplateMapperId <= 0) {
            $(control).closest("form").remove();
        }
        else {
            Endpoint.prototype.ManageEmailTemplateArea($("#ddl_portal_list_for_email_template").val(), function (responce) {
                $("#content-to-dispaly-in-table").html(responce);
            });
        }
    }

    SetAreaMappingAttributes(data, isCreate: boolean): void {
        $("#saveAreaMapping_" + data + "").show();
        $("#CancelAreaMapping_" + data + "").show();
        $("#EditAreaMapping_" + data + "").attr("style", "display: none !important");
        $("#deleteAreaMapping_" + data + "").attr("style", "display: none !important");
        $("#areaActive_" + data + "").attr('disabled', false);
        $("#areaSmsActive_" + data + "").attr('disabled', false);
        $("#emailTemplate_" + data + "").attr('disabled', false);
        var emailArea = isCreate ? $("select[name=EmailTemplateAreasId] option:selected").text() : $("#emailArea_" + data + "").val();
        if (emailArea != RESET_PASSWORD)
        $("#areaIsEnableBcc_" + data + "").attr('disabled', false);
    }

    AreaMapperAddResult(data: any, control: any) {
        var id = $(control).closest("form").attr("id").split('_')[1];
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(data.message, data.status ? 'success' : 'error', isFadeOut, fadeOutTime);
        if (data.emailTemplateMapperId > 0 && data.status == true) {
            if (parseInt(id) == 0) {
                $("#partial").after(data.html);
                $("#partial").html("");
            }
            EmailTemplate.prototype.EmailTemplateShowHideFormAttributes(data.emailTemplateMapperId);
        }
        else {
            Endpoint.prototype.ManageEmailTemplateArea($("#ddl_portal_list_for_email_template").val(), function (res) {
                $("#content-to-dispaly-in-table").html(res);
                EmailTemplate.prototype.EmailTemplateShowHideFormAttributes(data.emailTemplateMapperId);
            });
        }
    }
    
    EmailTemplateShowHideFormAttributes(data) {
        var emailArea = $("#emailArea_" + data + "").val();
        if (emailArea == RESET_PASSWORD) {
            Endpoint.prototype.ManageEmailTemplateArea($("#ddl_portal_list_for_email_template").val(), function (res) {
                $("#content-to-dispaly-in-table").html(res);
            });
        }
        $("#saveAreaMapping_" + data + "").attr("style", "display: none !important");
        $("#CancelAreaMapping_" + data + "").attr("style", "display: none !important");
        $("#EditAreaMapping_" + data + "").show();
        $("#deleteAreaMapping_" + data + "").show();
        $("#emailArea_" + data + "").attr('disabled', 'disabled');
        $("#areaSmsActive_" + data + "").attr('disabled', 'disabled');
        $("#emailTemplate_" + data + "").attr('disabled', 'disabled');
        $("#areaActive_" + data + "").attr('disabled', 'disabled');
        $("#areaIsEnableBcc_" + data + "").attr('disabled', 'disabled');
    }

    AutoCompleteEmailTemplates() {
        $(".txtEmailTemplate").autocomplete({
            source: function (request, response) {
                try {
                    Endpoint.prototype.GetEmailTemplateListByName(request.term, function (res) {
                        if (res.length > 0) {
                            var templateValues = new Array();
                            res.forEach(function (templateValue) {
                                if (templateValue != undefined)
                                    templateValues.push(templateValue.TemplateName);
                            });
                            if ($.inArray(request.term, templateValues) == -1)
                                EmailTemplate.prototype.isTemplateValid = false;
                            else
                                EmailTemplate.prototype.isTemplateValid = true;
                            response($.map(res, function (item) {
                                return {
                                    label: item.TemplateName,
                                    templateId: item.EmailTemplateId,
                                };
                            }));
                        }
                        else {
                            EmailTemplate.prototype.isTemplateValid = false;
                        }
                    });
                } catch (err) {
                }
            },
            select: function (event, ui) {
                var id = $(this).attr("id").split('_')[1];
                $("#frmEmailTemplateArea_" + id + " #EmailTemplateId").val(ui.item.templateId);
                EmailTemplate.prototype.isTemplateValid = true;
            },
            focus: function (event, ui) {
                var id = $(this).attr("id").split('_')[1];
                $("#frmEmailTemplateArea_" + id + " #EmailTemplateId").val(ui.item.templateId);
            }
        }).focusout(function () {
            return EmailTemplate.prototype.ValidateEmailTemplate($(this).attr("id"));
        });
    }

    ValidateEmailTemplate(data): boolean {
        var flag = true;
        var mapperId = data.split('_')[1];
        if (EmailTemplate.prototype.isTemplateValid != undefined && !EmailTemplate.prototype.isTemplateValid) {
            EmailTemplate.prototype.ShowErrorMessage(ZnodeBase.prototype.getResourceByKeyName("InvalidEmailTemplate"), mapperId);
            return flag = false;
        }
        else {
            EmailTemplate.prototype.HideErrorMessage(mapperId);
        }
        return flag;
    }

    ShowErrorMessage(errorMessage: string = "", mapperId: any) {
        $("#emailTemplate_" + mapperId + "").removeClass("input-validation-valid").addClass("input-validation-error");
        $("#valEmailTemplate_" + mapperId + "").removeClass("field-validation-valid").addClass("field-validation-error").html("<span>" + errorMessage + "</span>");
        $("#frmEmailTemplateArea_" + mapperId + " #EmailTemplateId").val(0);
    }

    HideErrorMessage(mapperId: any) {
        $("#emailTemplate_" + mapperId + "").removeClass("input-validation-error").addClass("input-validation-valid");
        $("#valEmailTemplate_" + mapperId + "").removeClass("field-validation-error").addClass(" field-validation-valid").html("");

    }

    SaveTemplateMapping() {
        $(".btnSaveTemplateMapping").on("click", function () {

            return EmailTemplate.prototype.ValidateEmailTemplate($(this).attr("id"));
        });
    }

    PortalDropDownChange() {
        Endpoint.prototype.ManageEmailTemplateArea($("#ddl_portal_list_for_email_template").val(), function (responce) {
            $("#content-to-dispaly-in-table").html(responce);
        });
    }

    ShowHideTemplateTokens(): void {
        $("#templateTokens").on("click", function () {
            if ($("#templateTokensData").hasClass('display-none')) {
                $(this).html("Hide");
                $("#templateTokensData").removeClass("display-none").show("slow");
            }
            else {
                $(this).html("See More");
                $("#templateTokensData").addClass("display-none").hide("slow");
            }
        });
    }

    OnEmailTemplateAreaChange(): void {
        var emailArea = $("select[name=EmailTemplateAreasId] option:selected").text();
        if (emailArea == RESET_PASSWORD) {
            $("#areaIsEnableBcc_0").attr('disabled', true);
            $("#areaIsEnableBcc_0").attr('checked', false);
        }
        else
            $("#areaIsEnableBcc_0").attr('disabled', false);

    }
}