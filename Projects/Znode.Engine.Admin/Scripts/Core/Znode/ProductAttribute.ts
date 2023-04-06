var IsSwatch: boolean;
var _defaultValueCount;
class ProductAttribute extends ZnodeBase {

    constructor() {
        super();
    }
    Init() {
        ProductAttribute.prototype.BindDefaultValueSaveClick();
        ProductAttribute.prototype.BindDefaultValueDeleteClick();
        ProductAttribute.prototype.ShowHideDefaultView();
        ProductAttribute.prototype.ShowhideFieldForAttributeTypeLink();
        Attributes.prototype.AddRequiredValidation();
        ProductAttribute.prototype.ShowHideAttributeGroupsSelection();
        Attributes.prototype.ShowhideMaxCharacterforTextArea();
        Attributes.prototype.ParseForm();
        ProductAttribute.prototype.ShowHideIsConfigurable();
        ProductAttribute.prototype.ShowHideIsFacets();
        ProductAttribute.prototype.ShowHideIsPersonisable();
        ProductAttribute.prototype.ShowhideIsAllowMultiUploadforImage();
        ProductAttribute.prototype.ShowHidedefaultValueField();
        ProductAttribute.prototype.ShowHideIsRequired();
        ZnodeBase.prototype.ResetTabDetails();
        ProductAttribute.prototype.ShowHideSwatchDivAccordingToSwatchType();
        ProductAttribute.prototype.AutplayRequiredOnClick();
        ProductAttribute.prototype.ShowHideSwatch();
    }

    //Bind default value save/edit click event.
    BindDefaultValueSaveClick(): void {
        $(".action-save").off("click");
        $("body").off("click", ".action-save");
        $("body").on("click", ".action-save", function () {
            var indexid = $(this).parent().attr('id').split('_')[1];
            var buttonObj = this;
            if ($(this).attr('title') === "Edit")
                ProductAttribute.prototype.ShowDefaultValueEditMode(indexid, buttonObj);
            else if ($(this).attr('title') === "Cancel")
                ProductAttribute.prototype.ShowDefaultValueCancelMode(indexid, buttonObj);
            else
                ProductAttribute.prototype.ValidateAndSaveDefaultAttributeValue(indexid, buttonObj);
        });
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

        $(buttonObj).replaceWith('<a href="#" class="action-save btn-narrow-icon" title= "Save" > <i class=z-ok > </i></a><a href="#" class="action-save btn-narrow-icon" title= "Cancel" > <i class="z-cancel z-close" > </i></a>');
    }

    //Show default value row in edit mode.
    ShowDefaultValueCancelMode(indexid, buttonObj): void {
        $('#table-default-Values div[id="rowIndex_' + indexid + '"]').find('input').hide();
        $('#table-default-Values div[id="rowIndex_' + indexid + '"]').find('label').show();
        $.each($('#table-default-Values div[id="rowIndex_' + indexid + '"]'), function (i, e) {
            if ($(e) != null) {
                $(e).find('input').val($(e).find('label').html());
            }
        });
        $('#table-default-Values div[id="code_' + indexid + '"]').find('input').hide();
        $('#table-default-Values div[id="code_' + indexid + '"]').find('label').show();
        $('#table-default-Values div[id="displayOrder_' + indexid + '"]').find('input').hide();
        $('#table-default-Values div[id="displayOrder_' + indexid + '"]').find('label').show();
        $('#table-default-Values div[id="displayOrder_' + indexid + '"]').find('input').val($('#table-default-Values div[id="displayOrder_' + indexid + '"]').find('label').html());
        $('#table-default-Values div[id="swatchText_' + indexid + '"]').find('label').show();
        $('#table-default-Values div[id="swatchText_' + indexid + '"]').find('input').hide();
        $('#table-default-Values div[id="swatchText_' + indexid + '"]').find('input').val($('#table-default-Values div[id="swatchText_' + indexid + '"]').find('label').html());
        $('#table-default-Values div[id="swatchText_' + indexid + '"]').find('input').prop('disabled', true);
        $('#table-default-Values div[id="isdefault' + indexid + '"]').find('input').prop('disabled', true);
        $('#table-default-Values div[id="divMediaId' + indexid + '"]').find('img').prop('disabled', 'disabled');
        $('#table-default-Values div[id="divMediaId' + indexid + '"]').find('img').prop('disabled', true);
        $('#table-default-Values div[id="divMediaId' + indexid + '"]').find('span').hide();
        $('#span_' + indexid).hide();
        $('#table-default-Values div[id="swatch' + indexid + '"]').find('span').hide();
        $('#table-default-Values div[id="action_' + indexid + '"]').find('a[title|=Save]').remove();
        if ($("#hdisdefaultcheck").length > 0 && $("#" + $("#hdisdefaultcheck").val()).length > 0)
            $("#" + $("#hdisdefaultcheck").val()).prop("checked", true);
        else
            $.each($('#table-default-Values td[class|=is-default]').find('input'), function (i, e) { $(e).prop("checked", false); });

        $(buttonObj).replaceWith('<a href="#" class="action-save btn-narrow-icon dirtyignore" title= "Edit" > <i class=z-edit > </i></a>');
    }

    //Check default value validation and save validated value.
    ValidateAndSaveDefaultAttributeValue(indexid, buttonObj): void {
        var defaultvalueCode = $('#table-default-Values div[id="code_' + indexid + '"]').find('#DefaultValueCode-' + indexid).val();
        var displayOrder = $('#table-default-Values div[id="displayOrder_' + indexid + '"]').find('#DefaultValueDisplayOrder-' + indexid).val();
        var swatchText = $('#table-default-Values div[id="swatchText_' + indexid + '"]').find('#DefaultValueSwatchText-' + indexid).val();
        var mediaId = $('#table-default-Values div[id="divMediaId' + indexid + '"]').find($('.abc' + indexid)).val();
        //Validate default value code and locales.
        if (!ProductAttribute.prototype.ValidateDefaultValue(indexid, defaultvalueCode, swatchText, mediaId))
            return;
        //Set label text for non editable mode.
        $('#table-default-Values div[id="code_' + indexid + '"]').find('#DefaultValueCode-' + indexid).next().next().text(defaultvalueCode);
        $('#table-default-Values div[id="displayOrder_' + indexid + '"]').find('#DefaultValueDisplayOrder-' + indexid).next().text(displayOrder);
        $('#table-default-Values div[id="swatchText_' + indexid + '"]').find('#DefaultValueSwatchText-' + indexid).next().text(swatchText);
        //Set url for category and product.
        var url = $("#IsCategory").val() === "True" ? "/PIM/CategoryAttribute/SaveDefaultValues" : "/PIM/ProductAttribute/SaveDefaultValues";
        var IsDefault = $("#IsDefault_" + indexid).prop('checked') === true ? true : false;
        var swatch = $("input[id = IsSwatchImage_yes]:checked").val() == "true" ? mediaId : swatchText;
        var isSwatch = $("input[id = IsSwatchImage_yes]:checked").val() == "true" ? true : false;
        //call end point.       
        Endpoint.prototype.SaveDefaultValues(url, ProductAttribute.prototype.GetDefaultValueLocaleValueArray(indexid), $("#AttributeId").val(), defaultvalueCode, $("#hdnDefaultvalue_" + indexid).val(), displayOrder, IsDefault, isSwatch, swatch, function (res) {
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
                $('#table-default-Values div[id="action_' + indexid + '"]').find('a[title|=Cancel]').remove();
                $("#hdisdefaultcheck").val("IsDefault_" + indexid);
                $(buttonObj).replaceWith('<a href="javascript:void(0)" class="action-save btn-narrow-icon dirtyignore" title= "Edit" > <i class=z-edit > </i></a>');
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.mode == "Create"
                    ? ZnodeBase.prototype.getResourceByKeyName("RecordCreatedSuccessfully")
                    : ZnodeBase.prototype.getResourceByKeyName("RecordUpdatededSuccessfully"), "success", false, fadeOutTime);
            }
            else
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorMessage"), "error", false, fadeOutTime);
        });

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

    //Bind default value delete click event.
    BindDefaultValueDeleteClick(): void {
        $(".action-delete").off("click");
        $("body").on("click", ".action-delete", function () {
            $('#hdnDeleteDiv').val($(this).attr("id"));
        });
    }

    //Set Input validation rules according to attribute type.
    ValidationView(): void {
        ProductAttribute.prototype.ShowHideDefaultView();
        var url = $("#IsCategory").val() === "True" ? "/PIM/CategoryAttribute/ValidationRule" : "/PIM/ProductAttribute/ValidationRule";

        //Change input validation rule view.
        Endpoint.prototype.ValidationView(url, $("#attributeTypeList").val(), function (res) {
            $("#validation-container").html(res);
            $.getScript("/Scripts/References/DynamicValidation.js");
            Attributes.prototype.ParseForm();
        });

        //Set front-end and attribute property according to attribute type.
        ProductAttribute.prototype.ShowhideFieldForAttributeTypeLink();
        ProductAttribute.prototype.ShowHideIsFacets();
        ProductAttribute.prototype.ShowHideIsPersonisable();
        ProductAttribute.prototype.ShowHideIsConfigurable();
        ProductAttribute.prototype.ShowhideIsAllowMultiUploadforImage();
        ProductAttribute.prototype.ShowHidedefaultValueField();
        ProductAttribute.prototype.ShowHideIsRequired();
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
                ImageOrText = '<td id="divswatch" style="" class="imageicon displayswatch"><div class="divSwatchImage swatch-image" style=""><div class="control-md" id="swatch-' + _defaultValueCount + '"><div class="upload-images" id="divMediaId' + _defaultValueCount + '"><img id="SwatchImage_' + _defaultValueCount + '" onclick="ProductAttribute.prototype.CheckMediaBrowse(' + _defaultValueCount + ');" src="/MediaFolder/no-image.png" class="img-responsive" style="pointer-events: All"><span onclick="EditableText.prototype.BrowseMedia(&quot;SwatchImage_' + _defaultValueCount + '&quot;, &quot;False&quot;, &quot;True&quot;, &quot;True&quot;)" class="change-image" style="pointer-events: All">Change</span><input type="hidden"  class="abc' + _defaultValueCount + '" id="SwatchImage_' + _defaultValueCount + '" name="SwatchImage_' + _defaultValueCount + '" value=""><input type="hidden" value=".gif,.jpeg,.jpg,.png,.svg,.SVG" id="hdnSwatchImage_' + _defaultValueCount + '"><input type="hidden" value="1000" id="hdnMediaSizeSwatchImage_' + _defaultValueCount + '"></div></div></div><div class="divSwatchText swatch-text swatchTxt_' + _defaultValueCount + '" style="display: none;"><div class="control-md" id="swatchText_' + _defaultValueCount + '"><input class="jscolor"id="DefaultValueSwatchText-' + _defaultValueCount + '"name="DefaultValueSwatchText-' + _defaultValueCount + '" type="text" value=""><label disabled="disabled"></label></div></div><div id="UploadMediaId" class="appendMediaModel uploadMedia_' + _defaultValueCount + '"></div></td>';
            else if ($("input[id = IsSwatchImage_no]:checked").prop('checked') == true)
                ImageOrText = '<td id="divswatch" style="" class="imageicon displayswatch"><div class="divSwatchImage swatch-image" style="display: none;"><div class="control-md" id="swatch-' + _defaultValueCount + '"><div class="upload-images" id="divMediaId' + _defaultValueCount + '"><img id="SwatchImage_' + _defaultValueCount + '" src="/MediaFolder/no-image.png" class="img-responsive" style="pointer-events: All" disabled="disabled" onclick="ProductAttribute.prototype.CheckMediaBrowse(' + _defaultValueCount + ');"><span onclick="EditableText.prototype.BrowseMedia(&quot;SwatchImage_' + _defaultValueCount + '&quot;, &quot;False&quot;, &quot;True&quot;, &quot;True&quot;)" class="change-image" style="pointer-events: All">Change</span><input type="hidden"  class="abc' + _defaultValueCount + '" id="SwatchImage_' + _defaultValueCount + '" name="SwatchImage_' + _defaultValueCount + '" value=""><input type="hidden" value=".gif,.jpeg,.jpg,.png,.svg,.SVG" id="hdnSwatchImage_' + _defaultValueCount + '"><input type="hidden" value="1000" id="hdnMediaSizeSwatchImage_' + _defaultValueCount + '"></div></div></div><div class="divSwatchText  swatch-text swatchTxt_' + _defaultValueCount + '" style="display: block;"><div class="control-md" id="swatchText_' + _defaultValueCount + '"><input class="txtSwatch jscolor" id="DefaultValueSwatchText-' + _defaultValueCount + '"name="DefaultValueSwatchText-' + _defaultValueCount + '" type="text"><label disabled="disabled"></label></div></div><div id="UploadMediaId" class="appendMediaModel uploadMedia_' + _defaultValueCount + '"></div></td>';
            else if ($("#IsText").prop("checked") == true)
                ImageOrText = '<td id="divswatch" style="display:none" class="imageicon displayswatch"><div class="divSwatchImage swatch-image" style=""><div class="control-md" id="swatch-' + _defaultValueCount + '"><div class="upload-images" id="divMediaId' + _defaultValueCount + '"><img id="SwatchImage_' + _defaultValueCount + '" src="/MediaFolder/no-image.png" class="img-responsive" style="pointer-events: All" disabled="disabled" onclick="ProductAttribute.prototype.CheckMediaBrowse(' + _defaultValueCount + ');"><span onclick="EditableText.prototype.BrowseMedia(&quot;SwatchImage_' + _defaultValueCount + '&quot;, &quot;False&quot;, &quot;True&quot;, &quot;True&quot;)" class="change-image" style="pointer-events: All">Change</span><input type="hidden"  class="abc' + _defaultValueCount + '" id="SwatchImage_' + _defaultValueCount + '" name="SwatchImage_' + _defaultValueCount + '" value=""><input type="hidden" value=".gif,.jpeg,.jpg,.png,.svg,.SVG" id="hdnSwatchImage_' + _defaultValueCount + '"><input type="hidden" value="1000" id="hdnMediaSizeSwatchImage_' + _defaultValueCount + '"></div></div></div><div class="divSwatchText swatch-text swatchTxt_' + _defaultValueCount + '" style="display: block;"><div class="control-md" id="swatchText_' + _defaultValueCount + '"><input class="txtSwatch jscolor " id="DefaultValueSwatchText-' + _defaultValueCount + '"name="DefaultValueSwatchText-' + _defaultValueCount + '" type="text"><label disabled="disabled"></label></div></div><div id="UploadMediaId" class="appendMediaModel uploadMedia_' + _defaultValueCount + '"></div></td>';
        }
        html += '<td><div id="code_' + _defaultValueCount + '"><input  name="NewAdded"  id="DefaultValueCode-' + _defaultValueCount + '" type="text" value=""><span id="defaultCodeVal-' + _defaultValueCount + '" class="field-validation-valid" data-valmsg-for="DefaultValueCode-' + _defaultValueCount + '" data-valmsg-replace="true" style="display: none;"></span><label style="display: none;"></label> <input type="hidden" id="hdnDefaultvalue_' + _defaultValueCount + '"></div></td>';
        html += '<td><div id="displayOrder_' + _defaultValueCount + '"><input  name="NewAdded"  id="DefaultValueDisplayOrder-' + _defaultValueCount + '" type="text" value="" maxlength="3"><label style="display: none;"></label></div></td>';
        html += $("#IsCategory").val() === "False" ? '<td class="is-default"><div id="isdefault' + _defaultValueCount + '"><input id="IsDefault_' + _defaultValueCount + '" name="IsDefault" type="radio" value="False" class="dirty" data-test-selector="chkIsDefault"><span class="lbl padding-8" data-test-selector="IsDefaultchecked_' + _defaultValueCount + '"></span></div></td>' : "";
        html += $("#IsCategory").val() === "False" ? ImageOrText : "";
        for (var i = 0; i < _localCount; i++) {
            html += '<td><div id="rowIndex_' + _defaultValueCount + '"><input localeid=' + jsonArray[i] + ' name="NewAdded"  id="DefaultValue" type="text" value=""><label style="display: none;"></label></div></td>';
        }
        html += '<td><div id="action_' + _defaultValueCount + '" class="dirtyignore"><a href="#" class="action-save btn-narrow-icon" title="Save"><i class="z-ok"></i></a> <a href= "#" id="delete_' + _defaultValueCount + '" data-toggle="modal" data-target="#ProductDeleteDefaultValuePopup" class="action-delete btn-narrow-icon" title= "Delete" > <i class="z-close" > </i></a></div></td>';
        html += '</tr>';
        $("#DefaultValueCount").val(_defaultValueCount);
        $('#table-default-Values tbody:eq(0) tr:last').after(html);
        ProductAttribute.prototype.ScrollToElementById("DefaultValueCode-" + _defaultValueCount, 0);
        $.getScript("/Scripts/References/jscolor.js");
        ProductAttribute.prototype.BindDefaultValueSaveClick();
        ProductAttribute.prototype.BindDefaultValueDeleteClick();
    }

    //Hide IsRequired property for label,Link,Image type attribute.
    ShowHideIsRequired(): void {
        var attributetype = $("#attributeTypeList option:selected").text();
        if (attributetype == "Label" || attributetype == "Link" || attributetype == "Image") {
            ProductAttribute.prototype.SetYesNoControlValueFalse("IsRequired");
            $("#divIsRequired").hide();
        }
        else
            $("#divIsRequired").show();
    }

    //Show hide field for link type attribute.
    ShowhideFieldForAttributeTypeLink(): void {
        var attributetype = $("#attributeTypeList option:selected").text();
        if (attributetype == "Link") {
            var div = ["IsRequired", "IsConfigurable", "IsPersonalizable", "IsUseInSearch", "IsComparable", "IsLocalizable"];
            for (var index = 0; index < div.length; index++)
                ProductAttribute.prototype.SetYesNoControlValueFalse(div[index]);

            $("#attributeTypeGroupList option:selected").val("0");
            $("#divIsRequired").hide();
            $("#divAttributConfigurable").hide();
            $("#divAttributLocalizable").hide();
            $("#divAttributshowongrid").hide();
            $("#divAttributPersonalizable").hide();
            $("#divFrontendProperty").hide();
            $("#divAttributGroup").hide();
            $("#divHelpDescription").hide();
        }
        else {
            $("#divIsRequired").show();
            $("#divAttributConfigurable").show();
            $("#divAttributshowongrid").show();
            $("#divAttributPersonalizable").show();
            $("#divFrontendProperty").show();
            $("#divAttributGroup").show();
            $("#divAttributLocalizable").show();
            $("#divHelpDescription").show();
        }
    }

    //Show IsConfigurable option for simple select type attribute only.
    ShowHideIsConfigurable(): void {
        var attributetype = $("#attributeTypeList option:selected").text();
        if (attributetype == "Simple Select")
            $("#divAttributConfigurable").show();
        else {
            $("#divAttributConfigurable").hide();
            ProductAttribute.prototype.SetYesNoControlValueFalse("IsConfigurable");
        }
    }

    //Show IsPersonalisable option for text type attribute only.
    ShowHideIsPersonisable(): void {
        var attributetype = $("#attributeTypeList option:selected").text();
        if (attributetype == "Text")
            $("#divAttributPersonalizable").show();
        else
            $("#divAttributPersonalizable").hide();
    }

    //Show Is Facets option for simple and multiselect type attribute only.
    ShowHideIsFacets(): void {
        var attributetype = $("#attributeTypeList option:selected").text();
        if (attributetype == "Multi Select" || attributetype == "Simple Select")
            $("#divIsFacets").show();
        else {
            $("#divIsFacets").hide();
            ProductAttribute.prototype.SetYesNoControlValueFalse("IsFacets");
        }
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

    //Delete PIM attribute.
    DeletePIMAttribute(control, controllerName): void {
        var attributeIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (attributeIds.length > 0) {
            Endpoint.prototype.DeletePIMAttribute(attributeIds, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    //Validate attribute default value before save and update.
    ValidateDefaultValue(indexid, defaultvalueCode, swatchText, mediaId): boolean {
        var attributetype = $("#attributeTypeList option:selected").text();
        var textboxes = $('#table-default-Values div[id="rowIndex_' + indexid + '"]').find(('input[type=text]'));
        var displayOrder = $('#table-default-Values div[id="displayOrder_' + indexid + '"]').find('#DefaultValueDisplayOrder-' + indexid).val();
        if ($("#IsCategory").val() === "False")
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
            $("#defaultCodeVal-" + indexid).text('').text(ZnodeBase.prototype.getResourceByKeyName("DefaultvalueCodeCannotBlank")).addClass("field-validation-error").show();
            $('#DefaultValueCode-' + indexid).addClass('input-validation-error');
            return false;
        }
        else if (!/^[a-zA-Z0-9_]*$/i.test(defaultvalueCode)) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("OnlyAlphanumericareAllowed"), "error", false, fadeOutTime);
            return false;
        }
        else if (!/^[0-9]{1,5}$/.test(displayOrder) || displayOrder <= 0) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("FiveDigitDisplayOrderRangeValidationMessage"), "error", false, fadeOutTime);
            return false;
        }
        else if ((attributetype == "Multi Select" || attributetype == "Simple Select") && (swatchText == "" || swatchText == undefined) && ($("#IsCategory").val() === "False") && ($("#IsText").prop("checked") != true)) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("EnterSwatchText"), "error", false, fadeOutTime);
            return false;
        }
        else if (ProductAttribute.prototype.IsDefaultValueCodeExist(defaultvalueCode, $("#hdnDefaultvalue_" + indexid).val())) {
            return false;
        }
        else
            $("#defaultCodeVal-" + indexid).text('');
        return true;
    }

    //Delete attribute default value.
    DeleteDefaultAttributeValue(element): void {
        var indexid = element.split('_')[1];
        if ($("#hdnDefaultvalue_" + indexid).val() == 0) {
            var textboxes = $('#table-default-Values div[id="rowIndex_' + indexid + '"]').find(('input[type=text]'));
            var emptytextboxes = textboxes.filter(function () { return this.value == ""; });
            var attributetype = $("#attributeTypeList option:selected").text();
            if (textboxes.length == emptytextboxes.length && attributetype == "Text") {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorEmptyDefaultValues"), 'error', isFadeOut, fadeOutTime);
                return;
            }
            if (attributetype == "Text") {
                $('#table-default-Values div[id="rowIndex_' + "0" + '"]').find("#DefaultValue").val("");
                $('#table-default-Values div[id="code_' + indexid + '"]').find("#DefaultValueCode-" + indexid).val("");
                $('#table-default-Values div[id="displayOrder_' + indexid + '"]').find("#DefaultValueDisplayOrder-" + indexid).val("");
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
            var url = $("#IsCategory").val() === "True" ? "/PIM/CategoryAttribute/DeleteDefaultValues" : "/PIM/ProductAttribute/DeleteDefaultValues";
            Endpoint.prototype.DeleteDefaultValues(url, $("#hdnDefaultvalue_" + indexid).val(), function (res) {
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

    //Hide attribute group selection for personalizable type attribute.
    ShowHideAttributeGroupsSelection(): void {
        $("#IsPersonalizable_left").off("click");
        $("#IsPersonalizable_left").on("click", function () {
            $("#divAttributGroup option:eq(0)").prop("selected", true);
            $("#divAttributGroup").hide();
            $("#divAttributConfigurable").hide();
            $("#divIsUseInSearch").hide();
            ProductAttribute.prototype.SetYesNoControlValueFalse("IsUseInSearch");
            ProductAttribute.prototype.SetYesNoControlValueFalse("IsAllowHtmlTag");

        });
        $("#IsPersonalizable_right").off("click");
        $("#IsPersonalizable_right").on("click", function () {
            $("#divAttributGroup").show();
            $("#divAttributConfigurable").hide();
            $("#divIsUseInSearch").show();
        });
        if ($("input[name = IsPersonalizable]:checked").val() == "true") {
            $("#divAttributGroup option:eq(0)").prop("selected", true);
            $("#divAttributGroup").hide();
            $("#divAttributConfigurable").hide();
            $("#divIsUseInSearch").hide();
            ProductAttribute.prototype.SetYesNoControlValueFalse("IsUseInSearch");
            ProductAttribute.prototype.SetYesNoControlValueFalse("IsAllowHtmlTag");
        }
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

        if (attributeType == "Number" && ProductAttribute.prototype.ValidateNumberDefaultValue()) {
            $("#tabs").tabs({ active: $('#tabs a[href="#' + $("#divDefaultValueNumber").parent().parent().attr("id") + '"]').parent().index() });
            flag = false;
        }

        if (attributeType == "Date" && ProductAttribute.prototype.ValidateDateDefaultValue()) {
            $("#tabs").tabs({ active: $('#tabs a[href="#' + $("#divDefaultValueDate").parent().parent().attr("id") + '"]').parent().index() });
            flag = false;
        }

        if (mindate > maxdate) {
            $("#spamDate").html(ZnodeBase.prototype.getResourceByKeyName("ErrorMindate"));
            flag = false;
        }
        return flag;
    }

    //Hide isAllowmultiple option when attribute is system define for image type attribute.
    ShowhideIsAllowMultiUploadforImage(): void {
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

    //Validate default date value.
    ValidateNumberDefaultValue(): boolean {
        var defaulyValue = $("#AttributeDefaultValue").val();

        if (!(defaulyValue == undefined || defaulyValue.length == 0 || /\s/g.test(defaulyValue))) {
            if (!/^[+-]?[0-9]{1,13}(?:\.[0-9]{1,6})?$/i.test(defaulyValue)) {

                $("#errorAttributeDefaultValue").text(ZnodeBase.prototype.getResourceByKeyName("OnlyNumericValueallow"));
                $("#errorAttributeDefaultValue").show();
                return true;
            }
            else if ($("input[name = AllowDecimals]:checked").val() == "false" && Attributes.prototype.IsDecimalExist(defaulyValue)) {
                $("#errorAttributeDefaultValue").text(ZnodeBase.prototype.getResourceByKeyName("DeciamlValueNotAllowed"));
                return true;
            }
            else if ($("input[name = AllowNegative]:checked").val() == "false" && parseInt(defaulyValue) < 0) {
                $("#errorAttributeDefaultValue").text(ZnodeBase.prototype.getResourceByKeyName("NegitaveValueNotAllowed"));
                return true;
            }
            else if (ProductAttribute.prototype.BetweenNumber($("#MinNumber").val(), $("#MaxNumber").val())) {
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

    //Validate default date value.
    ValidateDateDefaultValue(): boolean {
        var defaulyValue = $("#AttributeDefaultDateValue").val();
        if (!(defaulyValue == undefined || defaulyValue.length == 0 || /\s/g.test(defaulyValue))) {
            if (ProductAttribute.prototype.BetweenDate($("#MinDate").val(), $("#MaxDate").val(), defaulyValue)) {
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

    //Validate unique attribute value of product.
    IsProductAttributeCodeExist(): boolean {
        return ProductAttribute.prototype.IsPIMAttributeCodeExist(false);
    }

    //Validate unique attribute value of category.
    IsCategoryAttributeCodeExist(): boolean {
        return ProductAttribute.prototype.IsPIMAttributeCodeExist(true);
    }

    //Check unique attribute validation.
    IsPIMAttributeCodeExist(iscategory: boolean): boolean {
        //check for other validations
        var result = ProductAttribute.prototype.Validate();
        if (($("#AttributeId").val() === undefined || $("#AttributeId").val() < 1) && ($('#AttributeCode').val() !== undefined && $('#AttributeCode').val() !== "")) {
            Endpoint.prototype.IsPIMAttributeCodeExist($('#AttributeCode').val(), iscategory, function (res) {
                if (res.data) {
                    $("#errorSpanPIMAttributeCode").addClass("error-msg");
                    $("#errorSpanPIMAttributeCode").text(ZnodeBase.prototype.getResourceByKeyName("ErrorAttributeCode"));
                    $("#errorSpanPIMAttributeCode").show();
                    result = false;
                }
            });
        }
        return result;
    }

    //Check default value code exiest or not.
    IsDefaultValueCodeExist(defaultvalueCode, defaultvalueId): boolean {
        var isExiest = false;
        Endpoint.prototype.IsAttributeDefaultValueCodeExist($("#AttributeId").val(), defaultvalueCode, defaultvalueId, function (res) {

            if (res.data) {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("DefaultValueCodeAlreadyExist"), 'error', isFadeOut, fadeOutTime);
                isExiest = true;
            }
        });
        return isExiest;
    }

    //Set Yes/No control value false.
    SetYesNoControlValueFalse(controlId: string) {
        $("#" + controlId + "_left").prop("checked", "false");
        $("#" + controlId + "_right").prop("checked", "true");
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

    AutplayRequiredOnClick(): any {
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
                $('#table-default-Values').removeClass('table-swatch').addClass("table-text");;
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

    ShowHideSwatch(): any {
        if ($("#IsCategory").val() === "True") {
            $("#swatchType").remove();
            $("#isdefault").remove();
            $("#divswatch").remove();
            $("#lblIsDefault").remove();
            $("#headerSwatch").remove();
            $(".is-default").remove();
        }
    }

    CheckMediaBrowse(rowSwatchIndex: number): any {
        if ($("div#action_" + rowSwatchIndex).children("a:first").attr("title") === "Edit") {
            return false;
        } else {
            EditableText.prototype.BrowseMedia("SwatchImage_" + rowSwatchIndex, 'False', 'True', 'True');
        }
    }
    // Check whether search filter or grid filter is empty
    IsSearchFieldEmtpty(control): boolean {
        var searchFilter = $(control).parentsUntil('#filterComponent').find('#globalfiltercolumn')

        if (searchFilter && !$(searchFilter).val())
            return $('.filtervalueinput :input:visible').length > 0 ? !$('.filtervalueinput :input:visible').val() : true;
        return false;
    }

    // Scroll to an element by elementId, scrollOverTime is in milliseconds
    ScrollToElementById(elementId: string, scrollOverTime: number): void {
        $('html, body').animate({
            scrollTop: $("#" + elementId).offset().top
        }, scrollOverTime);
    }
}
$(document).on(dyanmic_grid_loaded, function () {
    DynamicGrid.prototype.FilterButtonPress = function (control) {
        var status;
        $('.filtervalueinput').children().each(function (index, value) {
            var element = $(value);
            var columnList = $('#container').is(':visible') ? $('#container').find('option:selected').val() : " ";
            if (columnList == '0') {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorSelectFilterColumn"), 'error', isFadeOut, fadeOutTime);
                status = true;
                return false;
            }
            if (element.attr('name') == 'Total' || element.attr('name') == 'TotalReturnAmount' || element.attr('name') == 'Discount' || element.attr('name') == 'Rating') {
                if (!$.isNumeric($(value).val())) {
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper("Please enter a positive numeric value.", 'error', isFadeOut, fadeOutTime);
                    status = true;
                    return false;
                }
            }
        });
        //restrict search on grid pages if search value is empty.
        var isEmptySearch = ProductAttribute.prototype.IsSearchFieldEmtpty(control);
        if (status || isEmptySearch) {
            return false;
        }


        var form = $(control).closest("form")
        var formData = form.serialize();
        $.ajax({
            type: 'POST',
            url: form.attr('action'),
            data: formData,
            success: function (data) {
                GridPager.prototype.GridUpdateHandler(data);
            }
        });
        UpdateContainerId
            = form.attr('data-ajax-update').replace("#", "");
    };
});
