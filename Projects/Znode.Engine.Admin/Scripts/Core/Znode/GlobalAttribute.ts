class GlobalAttribute extends ZnodeBase {
    constructor() {
        super();
    }

    Init() {
        GlobalAttribute.prototype.ShowHideDefaultView();
        GlobalAttribute.prototype.ShowHideFieldForAttributeTypeLink();
        GlobalAttribute.prototype.ShowHideIsAllowMultiUploadforImage();
        GlobalAttribute.prototype.ShowHidedefaultValueField();
        GlobalAttribute.prototype.ShowHideIsRequired();
        GlobalAttribute.prototype.ShowHideSwatchDivAccordingToSwatchType();
        GlobalAttribute.prototype.AutoplayRequiredOnClick();
        ZnodeBase.prototype.ResetTabDetails();
        Attributes.prototype.AddRequiredValidation();
        Attributes.prototype.ShowhideMaxCharacterforTextArea();
        Attributes.prototype.ParseForm();
        GlobalAttributeEntity.prototype.GetAttributeGroupEntity();
        GlobalAttribute.prototype.ShowHideAttributes();
    }

    //Set Input validation rules according to attribute type.
    ValidationView(): void {
        GlobalAttribute.prototype.ShowHideDefaultView();

        var url = "/GlobalAttribute/ValidationRule";

        //Change input validation rule view.
        Endpoint.prototype.ValidationView(url, $("#attributeTypeList").val(), function (res) {
            $("#validation-container").html(res);
            $.getScript("/Scripts/References/DynamicValidation.js");

            Attributes.prototype.ParseForm();
        });

        //Set front-end and attribute property according to attribute type.
        GlobalAttribute.prototype.ShowHideFieldForAttributeTypeLink();
        GlobalAttribute.prototype.ShowHideIsAllowMultiUploadforImage();
        GlobalAttribute.prototype.ShowHidedefaultValueField();
        GlobalAttribute.prototype.ShowHideIsRequired();
    }

    //Check unique attribute validation.
    IsGlobalAttributeCodeExist(): boolean {
        //check for other validations
        var result = GlobalAttribute.prototype.Validate();

        if (($("#AttributeId").val() === undefined || $("#AttributeId").val() < 1) && ($('#AttributeCode').val() !== undefined && $('#AttributeCode').val() !== "")) {
            Endpoint.prototype.IsGlobalAttributeCodeExist($('#AttributeCode').val(), function (res) {
                if (res.data) {
                    $("#errorSpanGlobalAttributeCode").addClass("error-msg");
                    $("#errorSpanGlobalAttributeCode").text(ZnodeBase.prototype.getResourceByKeyName("ErrorAttributeCode"));
                    $("#errorSpanGlobalAttributeCode").show();
                    result = false;
                }
            });
        }

        return result;
    }

    //Show default value section for select, multiselect,text and label type attribute.
    ShowHideDefaultView(): void {
        var attributetype = $("#attributeTypeList option:selected").text();

        if (attributetype == "Multi Select" || attributetype == "Simple Select" || attributetype == "Text" || attributetype == "Label") {
            $("#divDefaultAtrributeValue").show();
            attributetype == "Text" || attributetype == "Label" ? $("#defaultValue-add-new-row").hide() : $("#defaultValue-add-new-row").show();
        }

        else
            $("#divDefaultAtrributeValue").hide();
    }

    //Show hide field for link type attribute.
    ShowHideFieldForAttributeTypeLink(): void {
        var attributetype = $("#attributeTypeList option:selected").text();

        if (attributetype == "Link") {
            var div = ["IsRequired", "IsConfigurable", "IsPersonalizable", "IsUseInSearch", "IsComparable", "IsLocalizable"];

            for (var index = 0; index < div.length; index++)
                GlobalAttribute.prototype.SetYesNoControlValueFalse(div[index]);

            $("#attributeTypeGroupList option:selected").val("0");
            $("#divIsRequired").hide();
            $("#divAttributLocalizable").hide();
            //$("#divAttributGroup").hide();
            $("#divHelpDescription").hide();
        }

        else {
            $("#divIsRequired").show();
            //$("#divAttributGroup").show();
            $("#divAttributLocalizable").show();
            $("#divHelpDescription").show();
        }
    }

    //Set Yes/No control value false.
    SetYesNoControlValueFalse(controlId: string) {
        $("#" + controlId + "_left").prop("checked", "false");
        $("#" + controlId + "_right").prop("checked", "true");
    }

    //Hide isAllowmultiple option when attribute is system define for image type attribute.
    ShowHideIsAllowMultiUploadforImage(): void {
        if ($("#attributeTypeList option:selected").text() == 'Image' && $('#IsSystemDefined').val() == 'True')
            $('#divIsAllowMultiUpload').hide();

        else
            $('#divIsAllowMultiUpload').show();
    }

    //Show/Hide field when attribute type change for number,date and Yes/No type attribute.
    ShowHidedefaultValueField(): void {
        var attributeType = $("#attributeTypeList option:selected").text();

        switch (attributeType) {
            case 'Number':
                {
                    $('#divDefaultValueNumber').show();
                    $("#divDefaultValueNumber").find("input").attr("disabled", false);
                    $('#divDefaultValueDate').hide();
                    $('#divDefaultValueYESNO').hide();
                    $("#divDefaultValueDate").find("input").attr("disabled", true);
                    $("#divDefaultValueYESNO").find("input").attr("disabled", true);
                    break;
                }
            case 'Date':
                {
                    $('#divDefaultValueYESNO').hide();
                    $('#divDefaultValueNumber').hide();
                    $('#divDefaultValueDate').show();
                    $("#divDefaultValueDate").find("input").attr("disabled", false);
                    $("#divDefaultValueYESNO").find("input").attr("disabled", true);
                    $("#divDefaultValueNumber").find("input").attr("disabled", true);
                    break;
                }
            case 'Yes/No':
                {
                    $('#divDefaultValueYESNO').show();
                    $("#divDefaultValueYESNO").find("input").attr("disabled", false);
                    $('#divDefaultValueNumber').hide();
                    $('#divDefaultValueDate').hide();
                    $("#divDefaultValueNumber").find("input").attr("disabled", true);
                    $("#divDefaultValueDate").find("input").attr("disabled", true);
                    break;
                }
            default: {
                $('#divDefaultValueNumber').hide();
                $('#divDefaultValueDate').hide();
                $('#divDefaultValueYESNO').hide();
                $("#divDefaultValueYESNO").find("input").attr("disabled", true);
                $("#divDefaultValueNumber").find("input").attr("disabled", true);
                $("#divDefaultValueDate").find("input").attr("disabled", true);
            }
        }
    }

    //Hide IsRequired property for label,Link,Image type attribute.
    ShowHideIsRequired(): void {
        var attributetype = $("#attributeTypeList option:selected").text();

        if (attributetype == "Label" || attributetype == "Link" || attributetype == "Image") {
            GlobalAttribute.prototype.SetYesNoControlValueFalse("IsRequired");
            $("#divIsRequired").hide();
        }
        else
            $("#divIsRequired").show();
    }

    //Validate default date value.
    ValidateNumberDefaultValue(): boolean {
        var defaultValue = $("#AttributeDefaultValue").val();

        if (!(defaultValue == undefined || defaultValue.length == 0 || /\s/g.test(defaultValue))) {
            if (!/^[+-]?[0-9]{1,13}(?:\.[0-9]{1,6})?$/i.test(defaultValue)) {
                $("#errorAttributeDefaultValue").text(ZnodeBase.prototype.getResourceByKeyName("OnlyNumericValueallow"));
                $("#errorAttributeDefaultValue").show();
                return true;
            }
            else if ($("input[name = AllowDecimals]:checked").val() == "false" && Attributes.prototype.IsDecimalExist(defaultValue)) {
                $("#errorAttributeDefaultValue").text(ZnodeBase.prototype.getResourceByKeyName("DeciamlValueNotAllowed"));
                return true;
            }
            else if ($("input[name = AllowNegative]:checked").val() == "false" && parseInt(defaultValue) < 0) {
                $("#errorAttributeDefaultValue").text(ZnodeBase.prototype.getResourceByKeyName("NegitaveValueNotAllowed"));
                return true;
            }
            else if (GlobalAttribute.prototype.BetweenNumber($("#MinNumber").val(), $("#MaxNumber").val())) {
                $("#errorAttributeDefaultValue").text(ZnodeBase.prototype.getResourceByKeyName("InvalidNumber"));
                return true;
            }
            else {
                $("#errorAttributeDefaultValue").text("");
                return false;
            }
        }

        else {
            $("#errorAttributeDefaultValue").text("");
            return false;
        }
    }

    //Validate min and max number rang for number type default value.
    BetweenNumber(first, last): boolean {
        var defaultValue = $("#AttributeDefaultValue").val();

        if (isNaN(first) || first == undefined || first == "")
            return false;

        else if (isNaN(last) || last == undefined || last == "")
            return false;

        else if (defaultValue.length == 0 || /\s/g.test(defaultValue))
            return false;

        else
            return !(parseFloat(defaultValue) >= first && parseFloat(defaultValue) <= last);
    }

    //Vaidate attribute input validation and default values.
    Validate(): boolean {
        var Locales = [];
        var mindate = new Date($("#MinDate").val());
        var maxdate = new Date($("#MaxDate").val());
        var minNumber = $("#MinNumber").val();
        var maxNumber = $("#MaxNumber").val();
        var isAllowedNegative = $("input[name = AllowNegative]:checked").val();
        var isAllowedDecimals = $("input[name = AllowDecimals]:checked").val();
        var attributeType = $("#attributeTypeList option:selected").text();
        var flag = true;

        $(".LocaleLabel").each(function () {
            Locales.push($(this).attr('localename'));
        });

        //Validate attribute locale
        for (var i = 0; i < Locales.length; i++) {
            var value = $("#Locale" + Locales[i]).val();
            if (value.length > 100) {
                $("#error" + Locales[i]).html(ZnodeBase.prototype.getResourceByKeyName("LocaleError"));
                flag = false;
            }
            else if (value.length > 0 && value.indexOf(',') > -1) {
                $("#error" + Locales[i]).html(ZnodeBase.prototype.getResourceByKeyName("ErrorCommaNotAllowed"));
                flag = false;
            }
        }

        if (isNaN(minNumber) && minNumber != undefined) {
            $("#errorSpamMinNumber").text(ZnodeBase.prototype.getResourceByKeyName("OnlyNumericallowforMinNumber"));
            $("#errorSpamMinNumber").show();
            flag = false;
        }

        if (Attributes.prototype.IsValidateNumberLength(minNumber)) {
            $("#errorSpamMinNumber").text(ZnodeBase.prototype.getResourceByKeyName("OnlyNumericallowforMinNumber"));
            $("#errorSpamMinNumber").show();
            flag = false;
        }

        if (isNaN(maxNumber) && maxNumber != undefined) {
            $("#errorSpamMaxNumber").text(ZnodeBase.prototype.getResourceByKeyName("OnlyNumericallowforMaxNumber"));
            $("#errorSpamMaxNumber").show();
            flag = false;
        }

        if (Attributes.prototype.IsValidateNumberLength(maxNumber)) {
            $("#errorSpamMaxNumber").text(ZnodeBase.prototype.getResourceByKeyName("OnlyNumericallowforMaxNumber"));
            $("#errorSpamMaxNumber").show();
            flag = false;
        }

        if (isAllowedNegative == "false") {
            if (isNaN(minNumber)) {
                $("#errorSpamMinNumber").text(ZnodeBase.prototype.getResourceByKeyName("OnlyNumericallowforMinNumber"));
                $("#errorSpamMinNumber").show();
                flag = false;
            }
            if (!isNaN(minNumber) && parseInt(minNumber) < 0) {
                $("#errorSpamMinNumber").text(ZnodeBase.prototype.getResourceByKeyName("NegitaveValueNotAllowed"));
                $("#errorSpamMinNumber").show();
                flag = false;
            }
        }

        if (isAllowedNegative == "false") {
            if (isNaN(maxNumber)) {
                $("#errorSpamMaxNumber").text(ZnodeBase.prototype.getResourceByKeyName("OnlyNumericallowforMaxNumber"));
                $("#errorSpamMaxNumber").show();
                flag = false;
            }
            if (!isNaN(maxNumber) && parseInt(maxNumber) < 0) {
                $("#errorSpamMaxNumber").text(ZnodeBase.prototype.getResourceByKeyName("NegitaveValueNotAllowed"));
                $("#errorSpamMaxNumber").show();
                flag = false;
            }
        }

        if (isAllowedDecimals == "false") {
            if (isNaN(minNumber)) {
                $("#errorSpamMinNumber").text(ZnodeBase.prototype.getResourceByKeyName("OnlyNumericallowforMinNumber"));
                $("#errorSpamMinNumber").show();
                flag = false;
            }
            if (!isNaN(minNumber) && Attributes.prototype.IsDecimalExist(minNumber)) {
                $("#errorSpamMinNumber").text(ZnodeBase.prototype.getResourceByKeyName("DeciamlValueNotAllowed"));
                $("#errorSpamMinNumber").show();
                flag = false;
            }
        }

        if (isAllowedDecimals == "false") {
            if (isNaN(maxNumber)) {
                $("#errorSpamMaxNumber").text(ZnodeBase.prototype.getResourceByKeyName("OnlyNumericallowforMaxNumber"));
                $("#errorSpamMaxNumber").show();
                flag = false;
            }
            if (!isNaN(maxNumber) && Attributes.prototype.IsDecimalExist(maxNumber)) {
                $("#errorSpamMaxNumber").text(ZnodeBase.prototype.getResourceByKeyName("DeciamlValueNotAllowed"));
                $("#errorSpamMaxNumber").show();
                flag = false;
            }
        }

        if (parseFloat(minNumber) > parseFloat(maxNumber)) {
            $("#errorSpamMaxNumber").text(ZnodeBase.prototype.getResourceByKeyName("MaxNumberAlwaysGreaterThanMinNumber"));
            $("#errorSpamMaxNumber").show();
            flag = false;
        }

        if (Attributes.prototype.IsValidateNumberLength($("#MaxCharacters").val())) {
            $("#errorSpamMaxCharacters").text(ZnodeBase.prototype.getResourceByKeyName("NumericNumberOutofRang"));
            $("#errorSpamMaxCharacters").show();
            flag = false;
        }

        if (attributeType == "Number" && GlobalAttribute.prototype.ValidateNumberDefaultValue()) {
            $("#tabs").tabs({ active: $('#tabs a[href="#' + $("#divDefaultValueNumber").parent().parent().attr("id") + '"]').parent().index() });
            flag = false;
        }

        if (attributeType == "Date" && GlobalAttribute.prototype.ValidateDateDefaultValue()) {
            $("#tabs").tabs({ active: $('#tabs a[href="#' + $("#divDefaultValueDate").parent().parent().attr("id") + '"]').parent().index() });
            flag = false;
        }
        if (mindate > maxdate) {
            $("#spamDate").html(ZnodeBase.prototype.getResourceByKeyName("ErrorMindate"));
            flag = false;
        }
        return flag;
    }

    //Validate default date value.
    ValidateDateDefaultValue(): boolean {
        var defaultValue = $("#AttributeDefaultDateValue").val();

        if (!(defaultValue == undefined || defaultValue.length == 0 || /\s/g.test(defaultValue))) {
            if (GlobalAttribute.prototype.BetweenDate($("#MinDate").val(), $("#MaxDate").val(), defaultValue)) {
                $("#errorAttributeDateDefaultValue").text(ZnodeBase.prototype.getResourceByKeyName("InvalidDate"));
                return true;
            }
            else {
                $("#errorAttributeDateDefaultValue").text("");
                return false;
            }
        }
        else {
            $("#errorAttributeDateDefaultValue").text("");
            return false;
        }
    }

    //Validate min and max date rang for date type default value.
    BetweenDate(first: Date, last: Date, defaultValue: Date): boolean {
        if (first.toString() != "" && last.toString() != "") {
            var srcDate = first.toString().replace(/-/g, ' ');
            var startDate = new Date(srcDate);
            startDate.setDate(startDate.getDate());
            srcDate = last.toString().replace(/-/g, ' ');
            var endDate = new Date(srcDate);
            endDate.setDate(endDate.getDate());
            srcDate = defaultValue.toString().replace(/-/g, ' ');
            var defaultDate = new Date(srcDate);
            defaultDate.setDate(defaultDate.getDate());
            return !((defaultDate >= startDate) && (defaultDate <= endDate));
        }
        else
            return false;
    }

    //Validate attribute locales max character length.
    ValidationLocale(LocaleName): boolean {
        var value = $("#Locale" + LocaleName).val();

        if (value.length > 100) {
            $("#error" + LocaleName).html(ZnodeBase.prototype.getResourceByKeyName("LocaleError"));
            return false;
        }
        else if (value.length > 0 && value.indexOf(',') > -1) {
            $("#error" + LocaleName).html(ZnodeBase.prototype.getResourceByKeyName("ErrorCommaNotAllowed"));
            return false;
        }
        else {
            $("#error" + LocaleName).html("");
            return true;
        }
    }

    //Delete global attribute.
    DeleteGlobalAttribute(control): void {
        var attributeIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (attributeIds.length > 0) {
            Endpoint.prototype.DeleteGlobalAttribute(attributeIds, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    //Check default value code exiest or not.
    IsDefaultValueCodeExist(defaultvalueCode, defaultvalueId): boolean {
        var isExist = false;
        Endpoint.prototype.IsGlobalAttributeDefaultValueCodeExist($("#AttributeId").val(), defaultvalueCode, defaultvalueId, function (res) {

            if (res.data) {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("DefaultValueCodeAlreadyExist"), 'error', isFadeOut, fadeOutTime);
                isExist = true;
            }
        });
        return isExist;
    }

    //Add new row for add new default value in default value section.
    AddNewRow(jsonArray): void {
        var html = '<tr id="dynamic-row" class="dynamic-row">';
        var _localCount = $("#LocaleCount").val();

        if (_defaultValueCount == undefined || _defaultValueCount == "") {
            if (parseInt($("#DefaultValueCount").val()) > 0)
                _defaultValueCount = parseInt($("#DefaultValueCount").val()) - 1;
            else
                _defaultValueCount = parseInt($("#DefaultValueCount").val());
        }
        _defaultValueCount++;
        var ImageOrText = '';
        if ($('#swatchType').length == 1) {
            if ($("input[id = IsSwatchImage_yes]:checked").prop('checked') == true)
                ImageOrText = '<td id="divswatch" style="" class="imageicon displayswatch"><div class="divSwatchImage swatch-image" style=""><div class="control-md" id="swatch-' + _defaultValueCount + '"><div class="upload-images" id="divMediaId' + _defaultValueCount + '"><img id="SwatchImage_' + _defaultValueCount + '" onclick="GlobalAttribute.prototype.CheckMediaBrowse(' + _defaultValueCount + ');" src="/MediaFolder/no-image.png" class="img-responsive" style="pointer-events: All"><span onclick="EditableText.prototype.BrowseMedia(&quot;SwatchImage_' + _defaultValueCount + '&quot;, &quot;False&quot;, &quot;True&quot;, &quot;True&quot;)" class="change-image" style="pointer-events: All">Change</span><input type="hidden"  class="abc' + _defaultValueCount + '" id="SwatchImage_' + _defaultValueCount + '" name="SwatchImage_' + _defaultValueCount + '" value=""><input type="hidden" value=".gif,.jpeg,.jpg,.png,.webp" id="hdnSwatchImage_' + _defaultValueCount + '"><input type="hidden" value="1000" id="hdnMediaSizeSwatchImage_' + _defaultValueCount + '"></div></div></div><div class="divSwatchText swatch-text swatchTxt_' + _defaultValueCount + '" style="display: none;"><div class="control-md" id="swatchText_' + _defaultValueCount + '"><input class="jscolor"id="DefaultValueSwatchText-' + _defaultValueCount + '"name="DefaultValueSwatchText-' + _defaultValueCount + '" type="text" value=""><label disabled="disabled"></label></div></div><div id="UploadMediaId" class="appendMediaModel uploadMedia_' + _defaultValueCount + '"></div></td>';
            else if ($("input[id = IsSwatchImage_no]:checked").prop('checked') == true)
                ImageOrText = '<td id="divswatch" style="" class="imageicon displayswatch"><div class="divSwatchImage swatch-image" style="display: none;"><div class="control-md" id="swatch-' + _defaultValueCount + '"><div class="upload-images" id="divMediaId' + _defaultValueCount + '"><img id="SwatchImage_' + _defaultValueCount + '" src="/MediaFolder/no-image.png" class="img-responsive" style="pointer-events: All" disabled="disabled" onclick="GlobalAttribute.prototype.CheckMediaBrowse(' + _defaultValueCount + ');"><span onclick="EditableText.prototype.BrowseMedia(&quot;SwatchImage_' + _defaultValueCount + '&quot;, &quot;False&quot;, &quot;True&quot;, &quot;True&quot;)" class="change-image" style="pointer-events: All">Change</span><input type="hidden"  class="abc' + _defaultValueCount + '" id="SwatchImage_' + _defaultValueCount + '" name="SwatchImage_' + _defaultValueCount + '" value=""><input type="hidden" value=".gif,.jpeg,.jpg,.png,.webp" id="hdnSwatchImage_' + _defaultValueCount + '"><input type="hidden" value="1000" id="hdnMediaSizeSwatchImage_' + _defaultValueCount + '"></div></div></div><div class="divSwatchText  swatch-text swatchTxt_' + _defaultValueCount + '" style="display: block;"><div class="control-md" id="swatchText_' + _defaultValueCount + '"><input class="txtSwatch jscolor" id="DefaultValueSwatchText-' + _defaultValueCount + '"name="DefaultValueSwatchText-' + _defaultValueCount + '" type="text"><label disabled="disabled"></label></div></div><div id="UploadMediaId" class="appendMediaModel uploadMedia_' + _defaultValueCount + '"></div></td>';
            else if ($("#IsText").prop("checked") == true)
                ImageOrText = '<td id="divswatch" style="display:none" class="imageicon displayswatch"><div class="divSwatchImage swatch-image" style=""><div class="control-md" id="swatch-' + _defaultValueCount + '"><div class="upload-images" id="divMediaId' + _defaultValueCount + '"><img id="SwatchImage_' + _defaultValueCount + '" src="/MediaFolder/no-image.png" class="img-responsive" style="pointer-events: All" disabled="disabled" onclick="GlobalAttribute.prototype.CheckMediaBrowse(' + _defaultValueCount + ');"><span onclick="EditableText.prototype.BrowseMedia(&quot;SwatchImage_' + _defaultValueCount + '&quot;, &quot;False&quot;, &quot;True&quot;, &quot;True&quot;)" class="change-image" style="pointer-events: All">Change</span><input type="hidden"  class="abc' + _defaultValueCount + '" id="SwatchImage_' + _defaultValueCount + '" name="SwatchImage_' + _defaultValueCount + '" value=""><input type="hidden" value=".gif,.jpeg,.jpg,.png,.webp" id="hdnSwatchImage_' + _defaultValueCount + '"><input type="hidden" value="1000" id="hdnMediaSizeSwatchImage_' + _defaultValueCount + '"></div></div></div><div class="divSwatchText swatch-text swatchTxt_' + _defaultValueCount + '" style="display: block;"><div class="control-md" id="swatchText_' + _defaultValueCount + '"><input class="txtSwatch jscolor " id="DefaultValueSwatchText-' + _defaultValueCount + '"name="DefaultValueSwatchText-' + _defaultValueCount + '" type="text"><label disabled="disabled"></label></div></div><div id="UploadMediaId" class="appendMediaModel uploadMedia_' + _defaultValueCount + '"></div></td>';
        }
        html += '<td><div id="code_' + _defaultValueCount + '"><input  name="NewAdded"  id="DefaultValueCode-' + _defaultValueCount + '" type="text" value=""><label style="display: none;"></label> <input type="hidden" id="hdnDefaultvalue_' + _defaultValueCount + '"></div></td>';
        html += '<td><div id="displayOrder_' + _defaultValueCount + '"><input  name="NewAdded"  id="DefaultValueDisplayOrder-' + _defaultValueCount + '" type="text" value="" maxlength="3"><label style="display: none;"></label></div></td>';
        html += '<td class="is-default"><div id="isdefault' + _defaultValueCount + '"><input id="IsDefault_' + _defaultValueCount + '" name="IsDefault" type="radio" value="False" class="dirty"><span class="lbl padding-8"></span></div></td>';
        html += ImageOrText;
        for (var i = 0; i < _localCount; i++) {
            html += '<td><div id="rowIndex_' + _defaultValueCount + '"><input localeid=' + jsonArray[i] + ' name="NewAdded"  id="DefaultValue" type="text" value=""><label style="display: none;"></label></div></td>';
        }
        html += '<td><div id="action_' + _defaultValueCount + '" class="dirtyignore"><a href="#" id="action_' + _defaultValueCount + '" class="action-save btn-narrow-icon" title="Save" onclick="GlobalAttribute.prototype.SaveEditButtonClick(this);"><i class="z-ok"></i></a> <a href= "#" id="delete_' + _defaultValueCount + '" data-toggle="modal" data-target="#GlobalAttributeDeleteDefaultValuePopup" class="action-delete btn-narrow-icon" title= "Delete"  onclick="GlobalAttribute.prototype.DeleteAttributeButtonClick(this);"> <i class="z-close" > </i></a></div></td>';
        html += '</tr>';
        $("#DefaultValueCount").val(_defaultValueCount);
        $('#table-default-Values tbody:eq(0) tr:last').after(html);
        $.getScript("/Scripts/References/jscolor.js");

    }

    CheckMediaBrowse(rowSwatchIndex: number): any {
        if ($("div#action_" + rowSwatchIndex).children("a:first").attr("title") === "Edit") {
            return false;
        } else {
            EditableText.prototype.BrowseMedia("SwatchImage_" + rowSwatchIndex, 'False', 'True', 'True');
        }
    }
    // save/edit click event for attribute
    SaveEditButtonClick(element): void {     
        var indexid = $(element).attr('id').split('_')[1];
        var buttonObj = element;
        if ($(element).attr('title') === "Edit")
            GlobalAttribute.prototype.ShowDefaultValueEditMode(indexid, buttonObj);
        else
            GlobalAttribute.prototype.ValidateAndSaveDefaultAttributeValue(indexid, buttonObj);
    }

    //Show default value row in edit mode.
    ShowDefaultValueEditMode(indexId, buttonObj): void {
        $('#table-default-Values div[id="rowIndex_' + indexId + '"]').find('input').show();
        $('#table-default-Values div[id="rowIndex_' + indexId + '"]').find('label').hide();
        $('#table-default-Values div[id="displayOrder_' + indexId + '"]').find('label').hide();
        $('#table-default-Values div[id="displayOrder_' + indexId + '"]').find('input').show();
        $('#table-default-Values div[id="swatchText_' + indexId + '"]').find('label').hide();
        $('#table-default-Values div[id="swatchText_' + indexId + '"]').find('input').show();
        $('#table-default-Values div[id="swatchText_' + indexId + '"]').find('input').prop('disabled', false);
        $('#table-default-Values div[id="isdefault' + indexId + '"]').find('input').prop('disabled', false);
        $('#table-default-Values div[id="divMediaId' + indexId + '"]').find('span').show();
        $('#span_' + indexId).show();
        $('#table-default-Values div[id="swatch' + indexId + '"]').find('span').show();

        $(buttonObj).replaceWith('<a href="#" id="save_' + indexId + '" class="action-save btn-narrow-icon" title= "Save" onclick="GlobalAttribute.prototype.SaveEditButtonClick(this);" > <i class=z-ok > </i></a>');
    }
    
    DeleteAttributeButtonClick(element): void {   
        $('#hdnDeleteDiv').val($(element).attr("id"));
    }
    //Delete attribute default value.
    DeleteDefaultAttributeValue(element): void {
        var indexid = element.split('_')[1];
        if ($("#hdnDefaultvalue_" + indexid).val() == 0) {

            var textboxes = $('#table-default-Values div[id="rowIndex_' + indexid + '"]').find(('input[type=text]'));
            var emptytextboxes = textboxes.filter(function () { return this.value == ""; });
            var attributetype = $("#attributeTypeList option:selected").text();
            if (textboxes.length == emptytextboxes.length && attributetype == "Text")
                return;
            if (attributetype == "Text") {
                $('#table-default-Values div[id="rowIndex_' + "0" + '"]').find("#DefaultValue").val("");
                $('#table-default-Values div[id="code_' + indexid + '"]').find("#DefaultValueCode-" + indexid).val("");
            }
            else {
                $('#table-default-Values div[id="rowIndex_' + indexid + '"]').closest("#dynamic-row").remove();
                $('#table-default-Values div[id="rowIndex_' + indexid + '"]').remove();
                $('#table-default-Values div[id="action_' + indexid + '"]').remove();
                $('#table-default-Values div[id="displayOrder_' + indexid + '"]').remove();
                $('#table-default-Values div[id="code_' + indexid + '"]').remove();
            }
            $('#table-default-Values div[id="isdefault' + indexid + '"]').remove();
        }
        else {
            //Check atleast one locale default value present in a row. 
            Endpoint.prototype.DeleteGlobalAttributeDefaultValues($("#hdnDefaultvalue_" + indexid).val(), function (res) {
                if (res != "") {
                    if (res.success == true) {
                        $('#table-default-Values div[id="rowIndex_' + indexid + '"]').closest("#dynamic-row").remove();
                        $('#table-default-Values div[id="rowIndex_' + indexid + '"]').remove();
                        $('#table-default-Values div[id="action_' + indexid + '"]').remove();
                        $('#table-default-Values div[id="displayOrder_' + indexid + '"]').remove();
                        $('#table-default-Values div[id="code_' + indexid + '"]').remove();
                        $('#table-default-Values div[id="swatch-' + indexid + '"]').remove();
                        $('#table-default-Values div[id="swatchText_' + indexid + '"]').remove();
                        $('#table-default-Values div[id="isdefault' + indexid + '"]').remove();
                        $('.swatchTxt_' + indexid).remove();
                        $('.uploadMedia_' + indexid).remove();
                        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.statusMessage, "success", false, fadeOutTime);
                    }
                    else
                        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.statusMessage, "error", false, fadeOutTime);
                }
            });
        }
    }

    AutoplayRequiredOnClick(): any {
        var _autoplayTimeOut = $('#IsSwatchImage').val();
        $("#swatchType").find("label").on("click", function () {
            if ($(this).attr('id') == "IsSwatchImage_yes") {
                $('#table-default-Values').addClass('table-swatch').removeClass("table-text");
                $('.divSwatchText').hide();
                $('.divSwatchImage').show();
                $("#isdefault").show();
                $("#divswatch").show();
                $("#lblIsDefault").show();
                $("#headerSwatch").show();
                $(".is-default").show();
                $(".displayswatch").show();
                IsSwatch = true;
            }
            else if ($(this).attr('id') == "IsSwatchImage_no") {
                $('#table-default-Values').removeClass('table-swatch').addClass("table-text");
                $('.divSwatchImage').hide();
                $('.divSwatchText').show();
                $("#isdefault").show();
                $("#divswatch").show();
                $("#lblIsDefault").show();
                $("#headerSwatch").show();
                $(".is-default").show();
                $(".displayswatch").show();
                IsSwatch = false;
            }
            else if ($(this).attr('id') == "IsText") {
                $("#divswatch").hide();
                $("#headerSwatch").hide();
                $(".displayswatch").hide();
                IsSwatch = false;
            }
        });
    }

    ShowHideSwatchDivAccordingToSwatchType() {
        $("#divswatch").find("label,textarea,select,img,span").attr("disabled", "disabled");
        if ($('#swatchType').length == 1) {
            $('#headerSwatch').show();
            if ($("input[id = IsSwatchImage_yes]:checked").val() == "true") {
                $('.divSwatchImage').show();
                if (!($('#divswatch').hasClass('divswatch')))
                    $('#divswatch').addClass('divswatch');

                $('#headerSwatch').addClass('divswatch');
                $('#table-default-Values').addClass('table-swatch').removeClass("table-text");
                $('#divswatch').show();
                IsSwatch = true;
            }
            else if ($("input[id = IsSwatchImage_no]:checked").prop("checked") == true || $("input[id = IsSwatchImage_yes]:checked").val() == "false") {
                $('.divSwatchText').show();
                $('#divswatch').removeClass('divswatch');
                $('#headerSwatch').removeClass('divswatch');
                $('#table-default-Values').removeClass('table-swatch').addClass("table-text");
                $('.divSwatchImage').hide();
                $('#divswatch').show();
                IsSwatch = false;
            }
            else {
                $("#divswatch").hide();
                $("#headerSwatch").hide();
            }
        }
    }

    //Check default value validation and save validated value.
    ValidateAndSaveDefaultAttributeValue(indexid, buttonObj): void {
        var defaultvalueCode = $('#table-default-Values div[id="code_' + indexid + '"]').find('#DefaultValueCode-' + indexid).val();
        var displayOrder = $('#table-default-Values div[id="displayOrder_' + indexid + '"]').find('#DefaultValueDisplayOrder-' + indexid).val();
        var swatchText = $('#table-default-Values div[id="swatchText_' + indexid + '"]').find('#DefaultValueSwatchText-' + indexid).val();
        var mediaId = $('#table-default-Values div[id="divMediaId' + indexid + '"]').find($('.abc' + indexid)).val();
        //Validate default value code and locales.
        if (!GlobalAttribute.prototype.ValidateDefaultValue(indexid, defaultvalueCode, swatchText, mediaId))
            return;
        //Set label text for non editable mode.
        $('#table-default-Values div[id="code_' + indexid + '"]').find('#DefaultValueCode-' + indexid).next().text(defaultvalueCode);
        $('#table-default-Values div[id="displayOrder_' + indexid + '"]').find('#DefaultValueDisplayOrder-' + indexid).next().text(displayOrder);
        $('#table-default-Values div[id="swatchText_' + indexid + '"]').find('#DefaultValueSwatchText-' + indexid).next().text(swatchText);
        //Set url for global attribute.
        var url = "/GlobalAttribute/SaveDefaultValues";
        var IsDefault = $("#IsDefault_" + indexid).prop('checked') === true ? true : false;
        var swatch = $("input[id = IsSwatchImage_yes]:checked").val() == "true" ? mediaId : swatchText;
        var isSwatch = $("input[id = IsSwatchImage_yes]:checked").val() == "true" ? true : false;
        //call end point.       
        Endpoint.prototype.SaveGlobalAttributeDefaultValues(url, GlobalAttribute.prototype.GetDefaultValueLocaleValueArray(indexid), $("#AttributeId").val(), defaultvalueCode, $("#hdnDefaultvalue_" + indexid).val(), displayOrder, IsDefault, isSwatch, swatch, function (res) {
            if (res.defaultvalueId > 0) {
                $('#hdnDefaultvalue_' + indexid).val(res.defaultvalueId);
                $('#table-default-Values div[id="rowIndex_' + indexid + '"]').find('input').hide();
                $('#table-default-Values div[id="rowIndex_' + indexid + '"]').find('label').show();
                $('#table-default-Values div[id="code_' + indexid + '"]').find('input').hide();
                $('#table-default-Values div[id="code_' + indexid + '"]').find('label').show();
                $('#table-default-Values div[id="displayOrder_' + indexid + '"]').find('input').hide();
                $('#table-default-Values div[id="displayOrder_' + indexid + '"]').find('label').show();
                $('#table-default-Values div[id="swatchText_' + indexid + '"]').find('label').show();
                $('#table-default-Values div[id="swatchText_' + indexid + '"]').find('input').hide();
                $('#table-default-Values div[id="swatchText_' + indexid + '"]').find('input').prop('disabled', true);
                $('#table-default-Values div[id="isdefault' + indexid + '"]').find('input').prop('disabled', true);
                $('#table-default-Values div[id="divMediaId' + indexid + '"]').find('img').prop('disabled', 'disabled');
                $('#table-default-Values div[id="divMediaId' + indexid + '"]').find('img').prop('disabled', true);
                $('#table-default-Values div[id="divMediaId' + indexid + '"]').find('span').hide();
                $('#span_' + indexid).hide();
                $('#table-default-Values div[id="swatch' + indexid + '"]').find('span').hide();
                $(buttonObj).replaceWith('<a href="#" id="save_' + indexid + '" class="action-save btn-narrow-icon dirtyignore" title= "Edit" onclick="GlobalAttribute.prototype.SaveEditButtonClick(this);"> <i class=z-edit > </i></a>');
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.mode == "Create"
                    ? ZnodeBase.prototype.getResourceByKeyName("RecordCreatedSuccessfully")
                    : ZnodeBase.prototype.getResourceByKeyName("RecordUpdatededSuccessfully"), "success", false, fadeOutTime);
            }
            else
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorMessage"), "error", false, fadeOutTime);
        });

    }

    //Validate attribute default value before save and update.
    ValidateDefaultValue(indexid, defaultvalueCode, swatchText, mediaId): boolean {
        var attributetype = $("#attributeTypeList option:selected").text();
        var textboxes = $('#table-default-Values div[id="rowIndex_' + indexid + '"]').find(('input[type=text]'));
        var displayOrder = $('#table-default-Values div[id="displayOrder_' + indexid + '"]').find('#DefaultValueDisplayOrder-' + indexid).val();
        swatchText = $("input[id = IsSwatchImage_yes]:checked").val() == "true" ? mediaId : swatchText;
        if ($("#IsText").prop("checked") == true)
            swatchText = null;

        var emptytextboxes = textboxes.filter(function () { return this.value == ""; });
        if (textboxes.length == emptytextboxes.length) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("AtleasEnterOnLocale"), "error", false, fadeOutTime);
            return false;
        }
        else if (($("#AttributeId").val() == undefined) || $("#AttributeId").val() == "0") {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("FirstSaveAttribute"), (!($("#AttributeId").val() == undefined)) ? "success" : "error", false, fadeOutTime);
            return false;
        }
        else if (defaultvalueCode.length == 0) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("DefaultvalueCodeCannotBlank"), "error", false, fadeOutTime);
            return false;
        }
        else if (!/^[a-zA-Z0-9_]*$/i.test(defaultvalueCode)) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("OnlyAlphanumericareAllowed"), "error", false, fadeOutTime);
            return false;
        }
        else if (!/^[0-9]{1,3}$/.test(displayOrder) || displayOrder <= 0) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("InValidDisplayOrderRange"), "error", false, fadeOutTime);
            return false;
        }
        else if ((attributetype == "Multi Select" || attributetype == "Simple Select") && (swatchText == "" || swatchText == undefined) && ($("#IsText").prop("checked") != true)) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("EnterSwatchText"), "error", false, fadeOutTime);
            return false;
        }
        else if (GlobalAttribute.prototype.IsDefaultValueCodeExist(defaultvalueCode, $("#hdnDefaultvalue_" + indexid).val())) {
            return false;
        }
        else
            return true;
    }

    //Get default value locale array with property id and value.
    GetDefaultValueLocaleValueArray(indexid): any {
        var _defaultValues = [];
        $('#table-default-Values div[id="rowIndex_' + indexid + '"]').find(('input[type=text]')).each(function () {
            var value = $(this).val();
            var id = $(this).attr("localeid");
            $(this).next().text(value);

            var item = {};
            item["Value"] = value;
            item["Id"] = id;
            _defaultValues.push(item);
        });
        return _defaultValues;
    }

    //Method for Save Product
    SaveEntityAttribute(backURL: string) {
        ZnodeBase.prototype.ShowLoader();
        $("#globalAttributeAsidePannel li.active-tab-validation").each(function () {
            $(this).removeClass('active-tab-validation');
        });

        GlobalAttribute.prototype.ValidateFileTypeControl();

        if (!$("#frmGlobalAttribute").valid()) {
            if ($(".input-validation-error").closest("div.tab-pane").length > 0) {
                $(".input-validation-error").closest("div.tab-pane").each(function () {
                    var groupId = $(this).attr('id');
                    GlobalAttribute.prototype.SetActiveGroup(groupId);
                    ZnodeBase.prototype.HideLoader();
                });
            }
            else {
                ZnodeBase.prototype.HideLoader();
            }
        } else if (!GlobalAttribute.prototype.IsAttributeValueUnique()) {
            ZnodeBase.prototype.HideLoader();
            return false;
        }
        else {
            if (GlobalAttribute.prototype.ValidateFileTypeControl()) {
                return;
            }
            var url = decodeURIComponent(window.location.href);
            var orignalUrl = url.split(/[?#]/)[0];

            if (typeof (backURL) != "undefined")
                $.cookie("_backURL", backURL, { path: '/' });

            $("#frmGlobalAttribute").submit();
        }
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
                var groupId = $(this).closest("div.tab-pane").attr('id');
                GlobalAttribute.prototype.SetActiveGroup(groupId);
                return flag = true;
            }
            else {
                $(this).parent().find('span[id="fileerrormsg"]').html("");
                $(this).parent().find('span[id="fileerrormsg"]').hide();
            }
        });
        return flag;
    }

    SetActiveGroup(group: string) {
        $("#globalAttributeAsidePannel li").each(function () {
            var grpName = $('a', this).attr('href').replace('#', '');
            if (grpName == group) {
                $(this).addClass('active-tab-validation');
            }
        });
    }

    ShowHideAttributes(): any {
        $("#globalAttributeAsidePannel li").each(function () {
            var groupId = $('a', this).attr('href').replace('#', '');

            if ($("div[id='" + groupId + "']").find('.multi-upload-Image').length > 0)
                $("div[id='" + groupId + "']").find('.multi-upload-Image').each(function () {
                    Products.prototype.GetMultipleUploadImages($(this).attr('id'));
                });

            if ($("div[id='" + groupId + "']").find('.multi-upload-Files').length > 0)
                $("div[id='" + groupId + "']").find('.multi-upload-Files').each(function () {
                    EditableText.prototype.GetMultipleUploadFiles($(this).attr('id'));
                });
        });
    }

    //Method for return true if value is unique else false
    IsAttributeValueUnique(): boolean {
        //check for other validations        
        let result: boolean = GlobalAttribute.prototype.Validate();
        let attributeCodeValues: string = "";
        $("input[type='text']").each(function () {
            if ($(this).attr("data-unique") != undefined && $(this).attr("data-unique") != "" && $(this).attr("data-unique") != "false") {
                attributeCodeValues = attributeCodeValues + $(this).attr("id").split('_')[0] + '#' + $(this).val() + '~';
            }
        });

        let id: number = parseInt(ZnodeBase.prototype.GetParameterValues('entityId'), 10);
        let entityType: string = ZnodeBase.prototype.GetParameterValues('entityType');
        attributeCodeValues = attributeCodeValues.substr(0, attributeCodeValues.length - 1);

        Endpoint.prototype.IsGlobalAttributeValueUnique(attributeCodeValues, id, entityType, function (res) {
            if (res.data != null && res.data != "") {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.data, 'error', isFadeOut, fadeOutTime);
                result = false;
            }
        });
        return result;
    }

    SetActiveTabOnLoad(): any {
        $('a[data-toggle="tab"]').on('show.bs.tab', function (e) {
            localStorage.setItem('activeTab', $(e.target).attr('href'));
        });

        var activeTab = localStorage.getItem('activeTab');
        if (activeTab) {
            $('#globalAttributeAsidePannel a[href="' + activeTab + '"]').tab('show');
        }
    }

    ResetTabs(): any {
        localStorage.setItem('activeTab', '0');
    }

    OnSelectEntityAutocompleteDataBind(item: any): any {
        if (item != undefined) {
            let entityType: string = item.text;
            let entityId: number = item.Id;
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.GetAttributeList(entityId, entityType, function (response) {
                $("#ZnodeGlobalAttribute").html("");
                $("#ZnodeGlobalAttribute").html(response);                
                ZnodeBase.prototype.HideLoader();
            });
        }
    }
}