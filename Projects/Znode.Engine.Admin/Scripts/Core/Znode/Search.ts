class Search extends ZnodeBase {
    _endpoint: Endpoint;

    constructor() {
        super();
        this._endpoint = new Endpoint();

    }



    SetQueryType(queryId: string, subQueryId: string, queryTypeName: string, e: any): void {
        $(".query-type li").removeClass("active");
        $(e).parent().addClass("active");
        queryId = queryId.trim();
        subQueryId = subQueryId.trim();
        if (subQueryId != '') {
            var _text = $(e).parents('ul').closest("li[class='open']").find('a span:eq(0)').data('text');
            $(e).parents('ul').closest("li[class='open']").find('a span:eq(0)').text(_text + "-" + queryTypeName);
        }
        $("#SearchQueryTypeId").val(queryId);
        $("#SearchSubQueryTypeId").val(subQueryId);
        $("#QueryTypeName").val(queryTypeName);
        Endpoint.prototype.SetFeatureByQueyId(queryId, function (res) {
            $("#divFeaturesList").html("");
            $("#divFeaturesList").html(res);
        });
    }

    ShowHideValidationMessage(): any {
        var isValid: boolean = true;
        var boostElement = $(".BoostValue");
        if ($("#hdnProfileName").val() == '') {
            $("#hdnProfileName").addClass("input-validation-error");
            $("#errorRequiredSearchProfileName").addClass("error-msg");
            $("#errorRequiredSearchProfileName").text(ZnodeBase.prototype.getResourceByKeyName("ErrorRequiredSearchProfileName"));
            $("#errorRequiredSearchProfileName").show();
            isValid = false;
        }
        if (boostElement.length > 0) {
            var array = [];
            var regex = new RegExp('^0?[1-9][0-9]{0,2}$|^\s*$')
            $('.BoostValue').each(function () {
                array.push([this.value, $(this).attr('number')]);
            });

            for (var i = 0; i < array.length; i++) {
                var value = parseInt(array[i][0], 10);
                var num = array[i][1];
                if (!regex.test(array[i][0])) {
                    $("#SearchableAttributesList[" + num + "].BoostValue").addClass("input-validation-error");
                    $("#errorAttributeLength" + num.toString()).addClass("error-msg");
                    $("#errorAttributeLength" + num.toString()).text("The field Boost Value must be between 1 and 999");
                    $("#errorAttributeLength" + num.toString()).show();
                    isValid = false;
                }
                else {
                    $("#SearchableAttributesList[" + num + "].BoostValue").removeClass("input-validation-error");
                    $("#errorAttributeLength" + num.toString()).removeClass("error-msg");
                    $("#errorAttributeLength" + num.toString()).hide();
                }
            }
        }
        var MinimumShouldMatch = $("#MinimumShouldMatch").val();
        if (MinimumShouldMatch != undefined && (/\D/g.test(MinimumShouldMatch))) {
            $("#MinimumShouldMatch").addClass("input-validation-error");
            $("#errorNotANumber").addClass("error-msg");
            $("#errorNotANumber").text("Please enter a number.");
            $("#errorNotANumber").show();
            isValid = false;
        }
        var searchFeatureValue = $(".SearchFeatureValue");
        var regex = new RegExp('^(0?[1-9]|[1-9][0-9])$');
        var searchFeatureValue = $(".SearchFeatureValue");
        if (searchFeatureValue.length > 0) {
            var array = [];
            $('.SearchFeatureValue').each(function () {
                array.push([this.value, $(this).attr('value')]);
            });

            for (var i = 0; i < array.length; i++) {
                if (!regex.test(array[i][0])) {
                    $("#errorSearchFeatureValue" + (i + 1)).text(ZnodeBase.prototype.getResourceByKeyName("NgramRange"));
                    isValid = false;
                }
                var value = parseInt(array[i][0]);

                var isMinGramGreaterThanMaxGram: boolean = parseInt(array[0][0]) > parseInt(array[1][0]) && i > 0;
                if (isMinGramGreaterThanMaxGram) {
                    $("#errorSearchFeatureValue" + (i + 1)).text(ZnodeBase.prototype.getResourceByKeyName("MaxGramRange"));
                    isValid = false;
                }
                else {
                    $("#errorSearchFeatureValue" + (i + 1)).removeClass("error-msg");
                    $("#errorSearchFeatureValue" + (i + 1)).addClass("field-validation-valid");
                    $("#errorSearchFeatureValue" + (i + 1)).hide();
                }
                if (!isValid) {
                    $("#errorSearchFeatureValue" + (i + 1)).addClass("error-msg");
                    $("#errorSearchFeatureValue" + (i + 1)).removeClass("field-validation-valid");
                    $("#errorSearchFeatureValue" + (i + 1)).show();
                }
            }
        }
        return isValid;
    }

    RemoveAttribute(divId: string): void {
        var index = $("#" + divId + " [name='SearchableAttributesList.Index']").val();
        $('#' + divId).parent().remove();       
    }
    
    AddAttribute(): any {
        var attributeCodes = $(".AttributeCodes");
        var attributeCodesText = $(".AttributeCodes").text();
        var publishCatalogId = $('#hdnPublishCatalogIdCreate').val();
        var array = [];
        $('.AttributeCodes').each(function () {
            array.push(this.value);
        });
        var attributeCodesString = array.join(", ");
        var isAddAttributes = true;
        var url = '/Search/Search/GetCatalogBasedAttributes?publishCatalogId=' + publishCatalogId + '&associatedAttributes=' + attributeCodesString + '&isAddAttributes=' + isAddAttributes;
        $("#addAttributesPanel").html("");
        ZnodeBase.prototype.BrowseAsidePoupPanel(url, 'addAttributesPanel');
        ZnodeBase.prototype.HideLoader();
    }

    AddAttributes(): any {
        var attributeCodes = DynamicGrid.prototype.GetMultipleSelectedIds('ZnodeUnAssociatedSearchAttributes');
        if (attributeCodes.length > 0) {
            var attributeNames = DynamicGrid.prototype.GetMultipleValuesOfGridColumn("Attribute Name");
            var attributeCodesArray = attributeCodes.split(',');
            var attributeNamesArray = attributeNames.split(',');
            var associatedAttributesLength = $(".AttributeCodes").length;
            $("#errorSearchableAttributes").html('');
            for (var i = 0; i < attributeCodesArray.length; i++) {
                var divId = 'div' + attributeCodesArray[i];
                var atributeName = attributeNamesArray[i];
                var attributeCodeAttr = 'SearchableAttributesList[' + associatedAttributesLength + '].AttributeCode';
                var attributeNameAttr = 'SearchableAttributesList[' + associatedAttributesLength + '].AttributeName';
                var isUseInSearchAttr = 'SearchableAttributesList[' + associatedAttributesLength + '].IsUseInSearch';
                var boostValueAttr = 'SearchableAttributesList[' + associatedAttributesLength + '].BoostValue';
                var ngramValueAttr = 'SearchableAttributesList[' + associatedAttributesLength + '].IsNgramEnabled';
                var onClickEvent = "Search.prototype.RemoveAttribute('" + divId + "')";
                var errorAttributeLength = "errorAttributeLength" + associatedAttributesLength;
                $("#searchableAttributes").append(
                    $('<div/>', { 'class': 'row ' }).append(
                        $('<div/>', { 'class': 'col-xs-12 attributes clearfix', 'id': divId }).append(
                            $('<div/>', { 'class': ' col-md-4 padding-right-0' }).append(
                                $('<label/>', { 'class': 'addedattributes', 'data-test-selector': 'lbl' + atributeName, text: atributeName }),
                                $('<input/>', { 'type': "hidden", 'id': attributeNameAttr, 'name': 'SearchableAttributesList.Index', 'value': associatedAttributesLength }),
                                $('<input/>', { 'type': "hidden", 'id': attributeCodeAttr, 'name': attributeCodeAttr, 'value': attributeCodesArray[i], 'class': 'AttributeCodes' }),
                                $('<input/>', { 'type': "hidden", 'id': isUseInSearchAttr, 'name': isUseInSearchAttr, 'value': 'true' })
                            ),
                            $('<div/>', { 'class': 'search-field-input col-md-3 nopadding', }).append(
                                $('<input/>', { 'name': boostValueAttr, 'id': boostValueAttr, 'class': 'BoostValue', 'data-val': 'true', 'data-val-number': 'The field Boost Value must be a number.', 'data-val-range': 'Boost value should be between 1 and 999', 'data-val-range-max': '999', 'data-val-range-min': '1', 'data-val-rege': 'Enter a whole number', 'data-val-regex-pattern': '^([0-9][0-9]*)$', 'type': 'text', 'value': '', 'data-test-selector': 'txtBoostValue' ,'number': associatedAttributesLength }),

                            ),
                            $('<div/>', { 'class': 'col-md-3 Enable N-Gram', 'id': 'divIsEventLoggingEnabled' }).append($('<div/>', { 'class': 'nopadding log-switch' }).append($('<div/>', { 'class': 'control-sm' }).append($('<div/>', { 'class': 'switch-field control-yes-no' }).append(
                                $('<input/>', {
                                    'class': 'yes',
                                    'name': ngramValueAttr, 'id': 'IsNgramEnabled' + associatedAttributesLength, 'data-test-selector': 'chkEventLoggingEnabledLeft', 'value': "true", 'data-val': 'true', 'data-val-required': 'The IsNgramEnable field is required.', 'type': 'radio'
                                }),
                                $('<label>', { 'onclick': 'EditableText.prototype.labelClick(\'yes\',\'' + 'IsNgramEnabled' + associatedAttributesLength + '\')', 'data-test-selector': 'lblEventLoggingEnabledLeft', 'for': 'IsNgramEnabled ' + associatedAttributesLength, text: 'Yes' }),
                                $('<input/>', {
                                    'class': 'no',
                                    'name': ngramValueAttr, 'id': 'IsNgramEnabled' + associatedAttributesLength, 'data-test-selector': 'chkEventLoggingEnabledRight', 'value': "false", 'data-val': 'false', 'data-val-required': 'The IsNgramEnable field is required.', 'type': 'radio', 'checked': 'checked'
                                }),
                                $('<label>', { 'onclick': 'EditableText.prototype.labelClick(\'no\',\'' + 'IsNgramEnabled' + associatedAttributesLength + '\')','data-test-selector': 'lblEventLoggingEnabledRight', 'for': 'IsNgramEnabled ' + associatedAttributesLength, text: 'No' }),
                            )))),
                            $('<div/>', { 'class': 'col-md-2 close-icon' }).append(
                                $('<a/>', { 'class': 'search-close-icon', 'onclick': onClickEvent, 'data-test-selector': 'linkCloseIcon' }).append(
                                    $('<i/>', { 'class': 'z-close-circle' })
                                )
                            ),
                            $('<span/>', { 'class': 'field-validation-valid', 'data-valmsg-for': boostValueAttr, 'data-valmsg-replace': 'true', id: errorAttributeLength })
                        )
                    )
                );
                associatedAttributesLength++;
            }
            ZnodeBase.prototype.CancelUpload("addAttributesPanel");
            ZnodeBase.prototype.HideLoader();
            return true;
        }
        else {
            Brand.prototype.DisplayNotificationMessagesForBrand("Please select atleast one attribute.", "error", isFadeOut, fadeOutTime);
            ZnodeBase.prototype.HideLoader();
            return false;
        }
    }

    CreateSearchProfile(): any {
        var catalogId = $("#hdnPublishCatalogId").val();
        window.location.href = window.location.protocol + "//" + window.location.host + "/Search/Search/CreateSearchProfile";
    }

    //Set cancel url for all cancel and back buttons
    ReturnBackToList(): void {
        localStorage['activetab'] = "";
        localStorage['CurrentUrl'] = "";
        window.location.href = window.location.protocol + "//" + window.location.host + "/Search/Search/GetSearchProfiles";
    }


    GetSerchTermProducts(control): any {

        var searchTerm: string = encodeURIComponent($("#SearchTerm").val());

        if (searchTerm == "" || searchTerm == undefined)
            return false;

        if (Search.prototype.ValidateSearchProfileData(control)) {
            var publishCatalogId = parseInt($("#hdnPublishCatalogIdCreate").val());
            if (publishCatalogId <= 0) {
                $("#errorSelectCatalog").html("Please select catalog to get search result.");
                return false;
            }
            $.ajax({
                url: '/Search/Search/GetSearchProduct?searchText=' + searchTerm,
                data: $('#frmCreateSearchProfile').serialize(),
                type: 'POST',
                success: function (data) {
                    $("#SomeDivToShowTheResult").html("");
                    $("#SomeDivToShowTheResult").html(data);
                    ZnodeBase.prototype.HideLoader();
                }
            });
        }
    }

    GetSearchProfiles(publishCatalogId: string, catalogName: string): any {
        $("#divCataloglistPopup").html("");
        Endpoint.prototype.GetSearchProfilesByCatalogId(publishCatalogId, catalogName, function (res) {
            $("#profileListGrid").html("");
            $("#profileListGrid").html(res);
        });
    }

    GetCatalogSearchRules(publishCatalogId: string, catalogName: string): any {
        $("#divCataloglistPopup").html("");
        SearchConfiguration.prototype.SetFastSelectFilter("publishcatalogid", publishCatalogId);
        SearchConfiguration.prototype.SubmitFormOnFastSelection();
    }

    GetCatalogBasedAttributes(publishCatalogId: string): any {
        $.ajax({
            url: '/Search/Search/GetCatalogBasedAttributes?publishCatalogId=' + publishCatalogId,
            data: $('#divSearchCatalogCreate').serialize(),
            type: 'POST',
            success: function (data) {
                $("#divSearchableAttributes").html(data);
                ZnodeBase.prototype.HideLoader();
            }
        });
    }

    GetCatalogBasedFieldList(publishCatalogId: string): any {
        Endpoint.prototype.GetFieldValueList(parseInt(publishCatalogId), 0, function (res) {
            $("#dyanamicSearchCatalogField").html("");
            $("#dyanamicSearchCatalogField").html(res);
        });
    }

    GetCatalogProfileList(): void {
        $("#ZnodeStoreCatalog").find("tr").click(function () {
            var url = window.location.pathname.split("/");
            var action = url[3];
            let catalogName: string = $(this).find("td[class='catalogcolumn']").text();
            let publishCatalogId: string = $(this).find("td")[0].innerHTML;
            if (action == "CreateSearchProfile" || action == "GetTabStructureForProfile") {
                $('#txtCatalogNameCreate').val(catalogName);
                $('#hdnPublishCatalogIdCreate').val(publishCatalogId);
                Search.prototype.GetCatalogBasedAttributes(publishCatalogId);
                Search.prototype.GetCatalogBasedFieldList(publishCatalogId);
                $("#errorRequiredCatalog").text("").removeClass("field-validation-error").hide();
                $("#txtCatalogNameCreate").removeClass('input-validation-error');
            }
            else if (action == "GetBoostAndBuryRules") {
                $('#txtCatalogName').val(catalogName);
                $('#hdnPublishCatalogId').val(publishCatalogId);
                Search.prototype.GetCatalogSearchRules(publishCatalogId, catalogName);
                $("#errorRequiredCatalog").text("").removeClass("field-validation-error").hide();
                $("#txtCatalogName").removeClass('input-validation-error');
            } else {
                $('#txtCatalogName').val(catalogName);
                $('#hdnPublishCatalogId').val(publishCatalogId);
                Search.prototype.GetSearchProfiles(publishCatalogId, catalogName);
                $("#errorRequiredCatalog").text("").removeClass("field-validation-error").hide();
                $("#txtCatalogName").removeClass('input-validation-error');
            }
            $('#divCataloglistPopup').hide(700);
            ZnodeBase.prototype.RemovePopupOverlay();
        });
    }
    //This method is used to select catalog from fast select and show it on textbox
    OnSelectGetCatalogProfileList(item: any): any {
        if (item != undefined && item.Id > 0) {
            var url = window.location.pathname.split("/");
            var action = url[3];
            let catalogName: string = item.text;
            let publishCatalogId: string = item.Id;

            if (action == "CreateSearchProfile" || action == "GetTabStructureForProfile") {
                $('#txtCatalogNameCreate').val(catalogName);
                $('#hdnPublishCatalogIdCreate').val(publishCatalogId);
                Search.prototype.GetCatalogBasedAttributes(publishCatalogId);
                Search.prototype.GetCatalogBasedFieldList(publishCatalogId);
                $("#errorRequiredCatalog").text("").removeClass("field-validation-error").hide();
                $("#txtCatalogNameCreate").parent("div").removeClass('input-validation-error');
            }
            else if (action == "GetBoostAndBuryRules") {
                Store.prototype.OnSelectPubCatalogAutocompleteDataBind(item);
                Search.prototype.GetCatalogSearchRules(publishCatalogId, catalogName);

            } else {
                Store.prototype.OnSelectPubCatalogAutocompleteDataBind(item);
                Search.prototype.GetSearchProfiles(publishCatalogId, catalogName);
            }
        }
    }

    GetPublishCatalogList(control): any {
        $("#divCataloglistPopup").html("");
        ZnodeBase.prototype.BrowseAsidePoupPanel('/Search/Search/GetCatalogList', 'divCataloglistPopup');
    }

    GetUnAssociatedAttributes(searchProfileId: number): void {
        $("#divSearchAttributePopup").html("");
        ZnodeBase.prototype.BrowseAsidePoupPanel('/Search/Search/GetUnAssociatedCatalogAttributes?searchProfileId=' + searchProfileId, 'divSearchAttributePopup');
    }

    GetUnAssociatedPortalList(): void {
        $("#divSearchAttributePopup").html("");
        var searchProfileId = $("#hdnSearchProfileId").val();
        ZnodeBase.prototype.BrowseAsidePoupPanel('/Search/Search/GetUnAssociatedPortalList?searchProfileId=' + searchProfileId, 'divSearchAttributePopup');
    }

    ValidateCatalog(): boolean {
        if ($("#hdnPublishCatalogId").val() == "" || $("#hdnPublishCatalogId").val() == 0 || $("#hdnPublishCatalogId").val() == "") {
            $("#errorRequiredCatalog").text('').text(ZnodeBase.prototype.getResourceByKeyName("ErrorSelectCatalog")).addClass("field-validation-error").show();
            $("#txtCatalogName").addClass('input-validation-error');
            return false;
        }
        return true;
    }

    AssociateSearchAttributesToProfile(searchProfileId: number) {
        var attributeCode: string = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (attributeCode.length > 0) {
            Endpoint.prototype.AssociateSearchAttributesToProfile(searchProfileId, attributeCode, function (res) {
                $("#divSearchAttributePopup").hide(700);
                window.location.href = window.location.protocol + "//" + window.location.host + "/Search/Search/GetTabStructureForProfile?searchProfileId=" + searchProfileId;
            });
        }
        else {
            $('#associatedAttributeId').show();
        }
    }

    AssociatePortalToProfile() {
        var searchProfileId: any = $("#hdnSearchProfileId").val();

        var portalIds: string = DynamicGrid.prototype.GetMultipleSelectedIds();

        if (portalIds.length > 0) {
            Endpoint.prototype.AssociatePortalToProfile(searchProfileId, portalIds, function (res) {
                $("#divSearchAttributePopup").hide(700);
                $('.modal-backdrop').remove();
                window.location.href = window.location.protocol + "//" + window.location.host + "/Search/Search/GetTabStructureForProfile?searchProfileId=" + searchProfileId;

                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, res.status ? 'success' : 'error', isFadeOut, fadeOutTime);
            });
        }
        else {
            $('#selectErrorDiv').show();
        }
    }

    UnAssociatePortalToSearchProfile(control) {
        var searchProfileId: any = $("#SearchProfileId").val();
        var portalSearchProfileId: string = DynamicGrid.prototype.GetMultipleSelectedIds();

        if (portalSearchProfileId.length > 0) {
            Endpoint.prototype.UnAssociatePortalToSearchProfile(searchProfileId, portalSearchProfileId, function (res) {
                $("#divSearchAttributePopup").hide(700);
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, res.status ? 'success' : 'error', isFadeOut, fadeOutTime);
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
        else {
            $('#associatedAttributeId').show();
        }
    }


    UnAssociateSearchAttributesFromProfile(control): any {
        var searchProfilesAttributeMappingId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (searchProfilesAttributeMappingId.length > 0) {
            Endpoint.prototype.UnAssociateSearchAttributesFromProfile(searchProfilesAttributeMappingId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    DeleteSearchTriggers(control): any {
        var searchProfileTriggerId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (searchProfileTriggerId.length > 0) {
            Endpoint.prototype.DeleteSearchTriggers(searchProfileTriggerId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    CreateTriggers(IsConfirmation: boolean): any {
        $("#triggerError").hide();
        var Keyword: string = $("#frmCreateEditTriggers input[name=Keyword]").val();
        var ProfileIds: string[] = $("#ProfileId").val();
        var SearchProfileId: number = $("#SearchProfileId").val();

        if (Search.prototype.ValidateTriggerForm(Keyword, ProfileIds)) {

            var SearchTriggersViewModel = {
                "SearchProfileId": SearchProfileId,
                "Keyword": $("#frmCreateEditTriggers input[name=Keyword]").val(),
                "ProfileIds": $("#ProfileId").val(),
                "IsConfirmation": IsConfirmation,
            };

            Endpoint.prototype.CreateSearchProfileTriggers(SearchTriggersViewModel, function (response) {
                if (response.status) {
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, response.status ? 'success' : 'error', isFadeOut, fadeOutTime);
                    ZnodeBase.prototype.CancelUpload('divAddTriggerPopup');
                    Endpoint.prototype.GetSearchProfilesTriggers(SearchProfileId, function (res) {
                        $("#divTriggers").html(res);
                        ZnodeBase.prototype.HideLoader();
                    });
                }
                else {
                    ZnodeBase.prototype.HideLoader();
                    $("#PopUpConfirmAddTrigger").modal('show');
                }
            });
        }
    }

    ValidateTriggerForm(Keyword: string, ProfileIds: string[]): boolean {
        if (Keyword == "" && ProfileIds.length == 0) {
            $("#triggerError").show();
            return false;
        }

        return true;
    }

    DefaultSubmit(SelectedIdArr: string[], Controller: string, Action: string, Area: string, Callback: string) {
        var action = "SetDefault";
        var ids = [];
        ids = MediaManagerTools.prototype.unique();

        if (ids.length == 0)
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectAtleastOneRecord"), 'error', isFadeOut, fadeOutTime);
        else if (ids.length > 1)
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectAnyOneToSetAsDefault"), 'error', isFadeOut, fadeOutTime);
        else {
            this.submit(ids, action, Controller, Action, Area, Callback);
            CheckBoxCollection = new Array();
        }
    }

    submit(SelectedIdArr: string[], action: string, Controller: string, Action: string, Area: string, Callback: any) {
        var searchProfileId = parseInt(SelectedIdArr.toString());
        var portalIds = Store.prototype.GetMultipleValuesOfGridColumn('Store ID');
        var gridCatalogId = Store.prototype.GetMultipleValuesOfGridColumn('Catalog ID');
        var cataLogIds = isNaN(gridCatalogId) ? $("#hdnPublishCatalogId").val() : gridCatalogId;
        var publishCatalogId: number = parseInt(cataLogIds);
        var portalId: number = parseInt(portalIds.toString());
        if (portalId > 0) {
            var url = "/" + Area + "/" + Controller + "/" + Action;
            this.GetProfileSetting(url, portalId, searchProfileId, publishCatalogId, Controller, Area, Callback);
        }
        else
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorSearchProfileCannotMarkedAsDefault"), 'error', isFadeOut, fadeOutTime);
    }

    GetProfileSetting(url: string, portalId: number, searchProfileId: number, publishCatalogId: number, controller: string, Area: string, callback: any): void {
        Endpoint.prototype.SetSearchProfileSetting(url, portalId, searchProfileId, publishCatalogId, function (response) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, response.status ? 'success' : 'error', isFadeOut, fadeOutTime);
            var catalogName = $('#txtCatalogName').val();
            var catalogId: string;
            if (publishCatalogId > 0)
                catalogId = publishCatalogId.toString();

            Search.prototype.GetSearchProfiles(catalogId, catalogName);
        });
    }

    GoBackToSearchProfile(): any {
        $("#lnkSearchProfileList").click();
    }

    ValidateSearchProfileData(control): boolean {
        $("#errorSearchableAttributes").html("");

        if ($("#txtCatalogNameCreate").val() == "") {
            $("#errorRequiredCatalog").text('').text(ZnodeBase.prototype.getResourceByKeyName("ErrorSelectCatalog")).addClass("field-validation-error").show();
            $("#txtCatalogNameCreate").parent("div").addClass('input-validation-error');
            return false;
        }
        var count: number = 0;
        $("#searchableAttributes input[type=text]").each(function () {
            count = count + 1;
        });
        if (count == 0) {
            $("#errorSearchableAttributes").html(ZnodeBase.prototype.getResourceByKeyName("ErrorAtleastOneSearchableAttributes")).show();
            $("#divSearchableAttributes").collapse('show')
            return false;
        }
        var QueryTypeName: string = $("#QueryTypeName").val();
        if (QueryTypeName.indexOf('Multi Match') != 0 && count > 1) {
            $("#errorSearchableAttributes").html(ZnodeBase.prototype.getResourceByKeyName("ErrorSearchableAttributes")).show();
            $("#divSearchableAttributes").collapse('show');
            return false;
        }
        var eventCallId = $(control).attr("id");
        var isEqual = eventCallId == "SearchTerm";
        if (eventCallId != "searchResult" || isEqual) {
            var isActive = Search.prototype.ShowHideValidationMessage();
            return isActive;
        }
        return true;
    }

    ShowHideDiv(name: string, control: any): any {
        if (name.indexOf('Multi Match') == 0) {
            $("#div" + name.replace(" ", "")).show();
            $("#QueryTypeName").val("Multi Match");
        } else {
            $("#SearchSubQueryTypeId").val("");
            $("#QueryTypeName").val("");
            $("#divMultiMatch").hide();
        }

        if (name == "Match Phrase" || name == "Match Phrase Prefix") {
            $("#divOperator").hide();
        }else {
            $("#divOperator").show();
        }

        if (control != undefined) {
            Endpoint.prototype.SetFeatureByQueyId(control.value, function (res) {
                $("#divFeaturesList").html("");
                $("#divFeaturesList").html(res);
            });
        }
    }

    AddNewRow() {
        var existingFields: number = $("#fieldValueList").find('select').length;
        var publishCatalogId = $("#hdnPublishCatalogIdCreate").val();
        if (existingFields == 0) {
            Endpoint.prototype.GetFieldValueList(publishCatalogId, 0, function (res) {
                $("#fieldValueList").html("");
                $("#fieldValueList").html(res);
                return;
            });
        }
        var divIndex = existingFields + 1;
        var divId: string = "fieldValueListDiv_" + divIndex;
        var switchIdLeft: string = "fieldValue_left_" + divIndex;
        var switchIdRight: string = "fieldValue_right_" + divIndex;

        var ListHtml = $("#fieldList").html();

        $("#fieldValueList").append(
            $('<div/>', { 'class': 'col-xs-12 nopadding rank-section', 'id': divId }).append(
                $('<div/>', { 'class': 'left-section', 'id': 'fieldList' }).append(
                    ListHtml
                ),
                $('<div/>', { 'class': 'switch-field' }).append(
                    $('<div/>', { 'class': 'control-yes-no' }).append(
                        $('<input/>', { 'name': 'fieldValues.Index', 'value': divIndex }),
                        $('<input/>', { 'type': "radio", 'checked': "checked", 'id': switchIdLeft, 'name': 'fieldValues[' + divIndex + ']', 'value': 1 }),
                        $('<label/>', { 'for': switchIdLeft, 'text': 'ASC' }),
                        $('<input/>', { 'type': "radio", 'id': switchIdRight, 'name': 'fieldValues[' + divIndex + ']', 'value': 0 }),
                        $('<label/>', { 'for': switchIdRight, 'text': 'DESC' })),
                    $('<i/>', { 'class': "z-close-circle", 'title': "Delete", 'onclick': "Search.prototype.RemoveRow('" + divId + "')" }))));
    }

    RemoveRow(divid: string) {
        $('#' + divid).remove();
    }

    PauseSearchRule(control) {
        var URL = $(control).attr("data-parameter");

        var URLParameter = URL.replace('?', '').split('&');
        var searchCatalogRuleId = URLParameter[0].split('=')[1];
        var isPause = URLParameter[1].split('=')[1] == 'True' ? true : false;
        Endpoint.prototype.PauseCatalogSearchRule(searchCatalogRuleId, isPause, function (res) {
            DynamicGrid.prototype.RefreshGridOndelete(control, res);
        });
    }

    IsGlobalRule(): void {
        if ($("#IsGlobalRule").is(":checked")) {
            $(".divSearchTrigger *").attr('readonly', true);
            $('.divSearchTrigger').css({ pointerEvents: "none" })
        }
        else {
            $(".divSearchTrigger *").attr('readonly', false);
            $('.divSearchTrigger').css({ pointerEvents: "visible" })
        }
    };
                                                                                                                                                                                                                         
    EditSearchRule(control): void {
        var URL = $(control).attr("href");
        control.attr("href", "#");
        if (URL == undefined || URL == "" || URL == "#") {
            URL = decodeURIComponent(control[0].dataset.parameter);
        }
        var URLParameter = URL.replace('?', '').split('&');
        var searchCatalogRuleId = URLParameter[0].split('=')[1];
        ZnodeBase.prototype.BrowseAsidePoupPanel('/Search/Search/UpdateBoostAndBuryRule?searchCatalogRuleId=' + searchCatalogRuleId, 'divCreateRulePopup');
    }

    DeleteCatalogSearchRule(control): any {
        var ruleId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (ruleId.length > 0) {
            Endpoint.prototype.DeleteCatalogSearchRule(ruleId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    ValidateRuleName(): boolean {
        var isIndexExists: string = $("#IsSearchIndexExists").val();

        if ($("#IsGlobalRule").is(":checked") && $("#searchItem tr").length == 0) {
            $("#boostandBuryError").text(ZnodeBase.prototype.getResourceByKeyName("ErrorAddBoostAndBuryItems"));
            $("#boostandBuryError").show();
            return false;
        } else if ($("#searchTrigger").find("tbody tr").length == 0 || $("#searchItem").find("tbody tr").length == 0) {
            if ($("#hdnRuleName").val() != '') {
                $("#boostandBuryError").text(ZnodeBase.prototype.getResourceByKeyName("ErrorAddBoostAndBuryItemsorTrigger"));
                $("#boostandBuryError").show();
            }
            return false;
        }

        if (isIndexExists == "False" || isIndexExists == "false") {
            $("#divCreateRulePopup").hide();
            $("#divCreateRulePopup").html("");
            ZnodeBase.prototype.RemovePopupOverlay();
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorSearchIndexExists"), "error", false, 10000);
            return false;
        }


        var isValid = true;
        if ($("#hdnRuleName").val() != '') {
            Endpoint.prototype.IsRuleNameExist($("#hdnRuleName").val(), $("#PublishCatalogId").val(), function (response) {
                if (!response) {
                    $("#hdnRuleName").addClass("input-validation-error");
                    $("#errorSpanRuleName").addClass("error-msg");
                    $("#errorSpanRuleName").text(ZnodeBase.prototype.getResourceByKeyName("AlreadyExistRuleName"));
                    $("#errorSpanRuleName").show();
                    isValid = false;
                }
                else
                    $("#errorSpanRuleName").hide();
            });
        }
        return isValid;
    }



    BoostBuryAddResult(data: any) {
        ZnodeBase.prototype.ShowLoader();
        $("#divCreateRulePopup").html("");
        $("#divCreateRulePopup").hide();
        ZnodeBase.prototype.RemovePopupOverlay();
        ZnodeBase.prototype.HideLoader();
        if (!data.status) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(data.message, "error", data.status, 10000);
        }
        else {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(data.message, "success", data.status, 10000);
        }

        SearchConfiguration.prototype.SetFastSelectFilter("publishcatalogid", $("#hdnPublishCatalogId").val());
        SearchConfiguration.prototype.SubmitFormOnFastSelection();
    }

    CanclePopUp(): void {
        $('#btnCancel').click(function (event) {
            event.preventDefault();
            ZnodeBase.prototype.CancelUpload("divCreateRulePopup")
        });

    }

    SetStartPauseSearchRule(isPause: any, searchCatalogRuleId: number): void {
        Endpoint.prototype.PauseCatalogSearchRule(searchCatalogRuleId.toString(), isPause, function (data) {
            if (!data.status) {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(data.message, "error", data.status, 10000);
            }
            else {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(data.message, "success", data.status, 10000);
            }
            $("#" + 'ZnodeSearchCatalogRule').find(".btn-search").click();
            ZnodeBase.prototype.CancelUpload("divCreateRulePopup")

        });
    }

    AddBoostAndBuryPopUp(publishCatalogId: number, catalogName: string): void {
        ZnodeBase.prototype.BrowseAsidePoupPanel('/Search/Search/CreateBoostAndBuryRule?publishCatalogId=' + publishCatalogId + '&catalogName=' + encodeURIComponent(catalogName), 'divCreateRulePopup');
    }

    //Change condition drop down on the basis of data type.
    ChangeFieldDropdown(id: number, attributeType: any): void {
        var datatype: string = $(attributeType).find(":selected").attr("data-type");
        $('#SearchItemRuleList_' + id + '__SearchItemCondition option').wrap('<span/>');
        var itemCondtionId = $('#SearchItemRuleList_' + id + '__SearchItemCondition');
        if (datatype == "number") {
            var numericCondition = { et: 'Equals to (=)', gt: 'Greater than (>)', lt: 'Less than (<)', gte: 'Greater than equal to (>=)', lte: 'Less than equal to (<=)' };
            $.each(numericCondition, function (val, text) {
                itemCondtionId.append(
                    $('<option></option>').val(val).html(text)
                );
            });
        }
        else {
            var textCondition = { Contains: 'Contains', Is: 'Is', StartWith: 'Start With', EndWith: 'End With' };
            $.each(textCondition, function (val, text) {
                itemCondtionId.append(
                    $('<option></option>').val(val).html(text)
                );
            });
        }
    }

    AutocompleteFieldSuggestion() {
        $(".boostAndBuryClass").autocomplete({
            source: function (request, response) {
                try {

                    var element = $(this)[0]["element"][0];
                    var fieldName = element ? $(element).closest(".update-value-condition").find("[name*='SearchItemKeyword']").val() : "";
                    var selectedValue = $(element).closest(".update-value-condition").find("[name*='SearchItemKeyword'] option:selected");
                    var dataType = selectedValue.data('type')

                    if (fieldName != undefined && fieldName != "" && dataType != "number") {
                        if (request.term.length < 3) {
                            var element = $(this)[0]["element"];
                            element.attr("data-original-title", "Type " + (3 - request.term.length) + " more character");
                            element.tooltip("show");
                            return false;
                        }
                        else {
                            var element = $(this)[0]["element"];
                            element.attr("data-original-title", "");
                            element.tooltip("hide");
                        }
                        Endpoint.prototype.GetAutoSuggestion(request.term, fieldName, $("#hdnPublishCatalogId").val(), function (res) {
                            ZnodeBase.prototype.HideLoader();
                            if (res.length > 0) {

                                response($.map(res, function (item) {
                                    return {
                                        label: item,
                                    };
                                }));
                            }
                            else {
                                Inventory.prototype.isSKUValid = false;
                                $(".ui-autocomplete").hide();
                            }
                        });
                    }
                }
                catch (err) {
                }

            },
            select: function (event, ui) {
                Inventory.prototype.isSKUValid = true;
            }
        });

        $(".boostAndBuryClass").on("input", function () {
            if (!$(this).val()) {
                $(this).attr("data-original-title", "");
                $(this).tooltip("hide");
            }
            if ($(this).val().length < 3) {
                $(".ui-widget-content").html("");
            }
        })
    }

    //This method is used to select store from fast select and show it on textbox
    OnSelectStoreAutocompleteDataBind(item: any): any {
        if (item != undefined) {
            let portalName: string = item.text;
            let portalId: number = item.Id;

            Endpoint.prototype.GetTopKeywordsReport(portalId, portalName, function (response) {
                $("#orderList").html("");
                $("#orderList").html(response);
            });
        }
    }

    DateTimePickerRangeForSearchReport(): any {
        var ranges = {
            'Last Hour': [],
            'Last Day': [],
            'Last 7 Days': [],
            'Last 30 Days': [],
        }
        return ranges;
    }

    PublishSearchProfile(control) {
        if (Search.prototype.ValidateSearchProfileData(control)) {
            $("#PublishRequired").val('True');
            $("#frmCreateSearchProfile").submit();
        }
    }

    PublishSearchProfileFromList(publishButton): any {
        var searchProfileId: any = publishButton.attr("data-parameter").split('&')[0].split('=')[1]
        publishButton.attr("href", "#");

        if (searchProfileId.length > 0) {
            Endpoint.prototype.PublishSearchProfile(searchProfileId, function (res) {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, res.status ? 'success' : 'error', isFadeOut, fadeOutTime);
            });
        }
    }

    ShowDeleteSearchProfilePopup(): any {
        $("#grid tbody tr td").find(".z-delete").removeAttr("onclick data-toggle data-target");
        $("#grid tbody tr td").find(".z-delete").on("click", function (e) {
            e.preventDefault();
            $("#DeleteSearchProfileId").val($(this).attr("data-parameter").split('&')[0].split('=')[1]);
            $("#DeleteSearchProfilePopup").modal('show');
        });
    }

    DeleteSearchProfiles(control): any {
        var searchProfileId = [];
        if (MediaManagerTools.prototype.unique().length <= 0) {
            searchProfileId.push($("#DeleteSearchProfileId").val());
            $("#DeleteSearchProfileId").val("");
        }
        else {
            searchProfileId = MediaManagerTools.prototype.unique();
        }
        if (searchProfileId.length > 0) {
            Endpoint.prototype.DeleteSearchProfile(searchProfileId.join(","), $('#isDeletePublishSearchProfile').is(":checked"), function (res) {
                $("#refreshGrid").click();
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, res.status ? 'success' : 'error', isFadeOut, fadeOutTime);
            });
        }
    }

}