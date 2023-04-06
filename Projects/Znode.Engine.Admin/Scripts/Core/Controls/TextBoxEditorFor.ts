declare function GetRichTextBoxEditorValue(): string;
//Global variable for identifying the popup if anyone changing it change its all occurrences
var isMediaPopup = false;
var IsValidImage = false;
class EditableText extends ZnodeBase {
	Controller: string;
	Action: string;
	Field: string;
	Value: any;
	Primarykey: string;
	Mediadata: string[];
	Model: any;
	IsRequired: string;
	RegExPattern: string;
	RegEx_value: string;
	Validationrule: string;
	Allowdecimals: any;
	Allownegative: any;
	Maxnumber: string;
	Minnumber: string;
	Maxcharacters: string;
	MaxDate: string;
	MinDate: string;
	ControlLabel: string;

	constructor(doc: HTMLDocument) {
		super();
	}

	Init() {
		$(document).mouseup(function (e) {
			var popup = $("#appendDiv");
			if (popup.length > 0) {
                if (!popup.is(e.target) && popup.has(e.target).length == 0) {
                    EditableText.prototype.CancelUpload();
                }
			}
		});
	}

	labelClick(className: string, AttributeId: string) {
		if (className === "yes") {
			$("input#" + AttributeId + ".yes").prop("checked", true);
			$("input#" + AttributeId + ".no").prop("checked", false);
		}
		else {
			$("input#" + AttributeId + ".no").prop("checked", true);
			$("input#" + AttributeId + ".yes").prop("checked", false);
		}
	}

	EditControl(id: string): void {
		$("#divEdit_" + id).hide();
		$("#divSaveCancel_" + id).show();
		if ($("#" + id + " input").length > 0) {
			$("#" + id + " input").attr("readonly", false);
			this.Value = $("#" + id + " input").val();
		}
		else {
			$("#" + id + " textarea").attr("readonly", false);
			this.Value = $("#" + id + " textarea").val();
			var toggleWYSIWYG = $("#" + id).find("a.toggleWYSIWYG");
			if (toggleWYSIWYG) {
				toggleWYSIWYG.click();
			}
		}
		$("#divWidth span").text("");
	}

	HideControl(id: string): void {
		var spamId = "";
		$("#divSaveCancel_" + id).hide();
		$("#divEdit_" + id).show();
		if ($("#" + id + " input").length > 0) {
			$("#" + id + " input").attr("readonly", true);
			spamId = $("#" + id + " input").attr('id');
		}
		else {
			$("#" + id + " textarea").attr("readonly", true);
			spamId = $("#" + id + " textarea").attr('id');
			var toggleWYSIWYG = $("#" + id).find("a.toggleWYSIWYG");
			if (toggleWYSIWYG) {
				toggleWYSIWYG.click();
			}
		}
		$("#errorSpam" + spamId).text('');
	}

	SaveControl(obj: any): boolean {
		this.Field = $(obj).attr("data-field");
		this.ControlLabel = $(obj).attr("data-label");

		if ($("#editor" + this.Field).is("select")) {
			this.Value = $("#" + this.Field + " option:selected").text();
		} else {
			if ($("#" + this.Field + " input").length > 0) {
				this.Primarykey = $("#" + this.Field + " input").attr('id')
				this.Value = $("#" + this.Field + " input").val();
				if ($('#' + this.Field + ' input[type="radio"]').length > 0) {
					if (!$('#' + this.Primarykey).is(':checked')) { this.Value = "false"; }
				}
			}
			else {
				this.Primarykey = $("#" + this.Field + " textarea").attr('name')
				if (this.Primarykey.match("^mceEditor")) {//checks if the text area is rich text box editor.
					this.Value = GetRichTextBoxEditorValue();
				}
				else {
					this.Value = $("#" + this.Field + " textarea").val();
				}
			}
		}

		$("#errorSpam" + this.Primarykey).text("");
		this.Mediadata = this.Primarykey.split('_');

		this.Model = { "Value": this.Value, "Field": this.Field, "PrimaryKey": this.Primarykey };

		// Checking validation on Client side for required field
		this.IsRequired = $(obj).attr("data-isrequired");
		if (typeof this.IsRequired != 'undefined') {
			if (this.IsRequired.length > 0 && this.Value.length == 0) {
				$("#errorSpam" + this.Primarykey).text(this.ControlLabel + " field is required.");
				return false;
			}
		}

		// Validation for Regular Expression
		this.RegExPattern = $(obj).attr("data-val-regex-pattern");
		this.RegEx_value = $(obj).attr("data-val-regex");
		if (typeof this.RegExPattern != 'undefined') {
			var pattern = new RegExp(this.RegExPattern);
			if (!pattern.test(this.Value)) {
				$("#errorSpam" + this.Primarykey).text(this.ControlLabel + " value does not match with pattern");
				return false;
			}
		}

		this.Validationrule = $(obj).attr("data-validationrule");
		var valrule = "";
		if (typeof this.Validationrule != 'undefined') {
			if ($("#" + this.Field + " input").length > 0) {
				valrule = $('#' + this.Field + ' input').attr('data-val-regex-pattern');
			}
			else {
				valrule = $('#' + this.Field + ' textarea').attr('data-val-regex-pattern');
			}
			pattern = new RegExp(valrule);
			if (!pattern.test(this.Value)) {
				$("#errorSpam" + this.Primarykey).text(this.ControlLabel + " value does not match with validation rule specified");
				return false;
			}
		}

		//Validation for allowing decimal values
		this.Allowdecimals = $(obj).attr("data-allowdecimals");
		if (typeof this.Allowdecimals != 'undefined') {
			if (this.Allowdecimals = 'NO' && this.Value.indexOf('.') > -1) {
				$("#errorSpam" + this.Primarykey).text(this.ControlLabel + " field not allow decimal value");
				return false;
			}
		}

		//Validation for Negative Number
		this.Allownegative = $(obj).attr("data-allownegative");
		if (typeof this.Allownegative != 'undefined') {
			if (this.Allownegative = 'NO' && this.Value < 0) {
				$("#errorSpam" + this.Primarykey).text(this.ControlLabel + " field not allow negative value");
				return false;
			}
		}

		//Validation for maximum Number
		this.Maxnumber = $(obj).attr("data-maxnumber");
		if (typeof this.Maxnumber != 'undefined') {
			if (parseInt(this.Value, 10) > parseInt(this.Maxnumber, 10)) {
				$("#errorSpam" + this.Primarykey).text(this.ControlLabel + " value should not be grether than " + this.Maxnumber);
				return false;
			}
		}

		//Validation for minimum Number
		this.Minnumber = $(obj).attr("data-minnumber");
		if (typeof this.Minnumber != 'undefined') {
			if (parseInt(this.Value, 10) < parseInt(this.Minnumber, 10)) {
				$("#errorSpam" + this.Primarykey).text(this.ControlLabel + " value should not be less than " + this.Minnumber);
				return false;
			}
		}

		//Validation for max charecters
		this.Maxcharacters = $(obj).attr("data-maxcharacters");
		if (typeof this.Maxcharacters != 'undefined') {
			if (this.Value.length > parseInt(this.Maxcharacters, 10)) {
				$("#errorSpam" + this.Primarykey).text(this.ControlLabel + " exceed max charecter limit");
				return false;
			}
		}

		//Validation for date
		this.MaxDate = $(obj).attr("data-maxdate");
		this.MinDate = $(obj).attr("data-mindate");
		if (typeof this.MaxDate != 'undefined' && typeof this.MinDate != 'undefined') {
			//validate Date correct format
			pattern = /^([0-9]{2})\/([0-9]{2})\/([0-9]{4})$/;
			if (!pattern.test(this.Value)) {
				$("#errorSpam" + this.Primarykey).text("Invalid date format");
				return false;
			}
			var min = new Date(this.MinDate);
			var val = new Date(this.Value);
			var max = new Date(this.MaxDate);

			if (val < min || val > max) {
				$("#errorSpam" + this.Primarykey).text("Date is not in range");
				return false;
			}
		}

		$.ajax({
			url: "/MediaManager/MediaManager/UpdateMediaAttributeValue",
			method: "POST",
			data: this.Model,
			success: function (data) {
				if (data.errorMessage) {
					$("#errorSpam" + EditableText.prototype.Field).text(data.errorMessage)
				} else {
					if (EditableText.prototype.Value == "true" || EditableText.prototype.Value == "false") {
						var boolValue = (EditableText.prototype.Value === "true");
						EditableText.prototype.Value = boolValue ? "Yes" : "No";
					}
					$("#lbl" + EditableText.prototype.Field).text(EditableText.prototype.Value);
					if (EditableText.prototype.Mediadata[1] == "") {
						var newPrimaryKey = EditableText.prototype.Mediadata[0] + "_" + data.primaryKey + "_" + "_" + EditableText.prototype.Mediadata[3] + "_" + EditableText.prototype.Mediadata[4];
						$(obj).attr("data-primarykey", newPrimaryKey);
					}
					EditableText.prototype.HideControl(EditableText.prototype.Field);
				}
			},
			error: function () {
				$("#errorSpam" + EditableText.prototype.Primarykey).text("An error occured while updating the value.")
			}
		});
	}

    BrowseMedia(ImageId: string, IsMultiSelect: string, IsImageTag: string, IsDynamicControl: string, callback: string = '' , IsFileTypeFilterRequired : boolean = false) {
		var div = $("#divMediaUploaderPopup");
		var str: string = '<div id= "appendDiv" class="media-upload-panel"> </div>';
		div.html(str);
		var uploadButtonHtml = IsMultiSelect == "False" ? "display:none" : "display:block";
		var header = '<div class="col-sm-12 title-container">' +
			' <h1>MEDIA UPLOAD </h1> ' +
			'<div class="pull-right">' +
			'<button type="button" style="' + uploadButtonHtml + '" class="btn-text btn-text-secondary pull-right" onclick="EditableText.prototype.MultipleUpload(\'' + ImageId.trim() + '\',\'' + IsImageTag + '\');Products.prototype.UploadImages(\'' + ImageId.trim() + '\');"> Upload </button>' +
			'<button type="button" style="color:white;" class="btn-text-icon pull-right" onclick="EditableText.prototype.CancelUpload()"><i class="z-back" data-test-selector="popLinkCancel"></i> BACK </button>' +
			'</div>' +
            '  </div> ';
        var Type: string = IsFileTypeFilterRequired ? "MediaType" : "";
        var value: string 
        value = IsFileTypeFilterRequired ? $("input[id=hdn" + ImageId + "]").val() : "";
        var model: any = { "FolderId": -1, "IsMultiSelect": IsMultiSelect, "IsPopup": true, "Type": Type, "Value": value};
		$.ajax({
			url: '/MediaManager/MediaManager/List?displayMode=Tile',
			data: JSON.stringify(model),
			type: 'POST',
			contentType: 'application/json; charset=utf-8',
			success: function (response) {
				$("#appendDiv").html('');
				$("#appendDiv").html(response);
				$("#FileUploader").removeAttr("style");
				$("#appendDiv").prepend(header);
				$("#divMediaUploaderPopup").find("#FileUploader").show();
              
				isMediaPopup = true;
				if (IsMultiSelect == "False") {
                    $('.table-responsive').addClass('single-selection');
                    $(document).off("click", ".img");
                    $(document).on("click", ".img", function (e) {
                        e.stopPropagation();
                        var selectedDiv = $(this).data("rowcheckid");
                        EditableText.prototype.SingleUpload(ImageId, selectedDiv, IsImageTag, IsDynamicControl);
                        $(this).find("input[type='checkbox']").prop("checked", false);
                        if (typeof callback !== 'undefined' && callback != null && callback !== "") {
                          ZnodeBase.prototype.executeFunctionByName(callback, window, ImageId);
                        }
                    });
                    selectedImages = [];
                    IsValidImage = true;
				}
				else {
                    $(document).off("click", ".img");
                    $(document).on("click", ".img", function (e) {    
                        e.stopPropagation();
						var selectedDiv = $(this).data("rowcheckid");
						if (EditableText.prototype.ValidateImage(ImageId, selectedDiv, IsImageTag, IsDynamicControl)) {
							IsValidImage = true;
						}
						else {
                            $(this).find("input[type='checkbox']").prop("checked", false);
                            var invalidImageId = selectedDiv.split('_')[1];
                            for (var i = 0; i < selectedImages.length; i++) {
                                if (selectedImages[i].values === invalidImageId)
                                    selectedImages.splice(i, 1);
                            }
						}
					});
                    selectedImages = [];
                    IsValidImage = true;
				}
				$("#appendDiv").slideDown(700);
				$("body").css('overflow', 'hidden');
				$("body").append("<div class='modal-backdrop fade in'></div>");

				$.getScript("/Scripts/Core/Controls/TreeView.js");
                $.getScript("/Scripts/Core/Znode/MediaManagerTools.js");
				GridPager.prototype.Init();

				var sliderTree = $("#divMediaUploaderPopup").find("#Main_Tree");

                if (sliderTree) {
                    var treeData = $(sliderTree).attr('data-tree');
                    var obj = eval(treeData);
                    $(sliderTree).jstree({
                        'core': {
                            "animation": 0,
                            "check_callback": function (operation, node, parent, position, more) {
                                //Restrict drag drop in popup
                                if (isMediaPopup) {
                                    return false;
                                }
                                if (operation === "move_node") {
                                    return (position == 0 && (more.pos == "i" || more.pos == undefined));
                                }
                                return true;  //allow all other operations
                            },
                            'multiple': false,
                            data: obj,
                        },
                        "search": {
                            "case_insensitive": true,
                            "show_only_matches": true
                        },
                        "plugins": ["contextmenu", "dnd", "search", "state", "wholerow"],
                        "contextmenu": {
                            "items": function ($node) {
                                return {
                                    "Create": {
                                        "label": "Add Folder",
                                        "action": function (obj) {
                                            TreeView.prototype.create();
                                        }
                                    },
                                    "Rename": {
                                        "label": "Rename",
                                        "action": function (obj) {
                                            TreeView.prototype.rename();
                                        }
                                    },
                                    "Delete": {
                                        "label": "Delete",
                                        "action": function (obj) {
                                            TreeView.prototype.remove();
                                        }
                                    }
                                }
                            }
                        }
                    });

                    $(sliderTree).on('select_node.jstree', TreeView.prototype.bindCurrentNode.bind(this))                   
				}
				else {
					TreeView.prototype.ReloadTree();
                }
                            
            }  
		});
	}

	SingleUpload(ImageId: string, selectedDiv: string, IsImageTag: string, IsDynamicControl: string) {
		var mediafamily = $("#" + selectedDiv).parent().parent().find('#hiddenFamily').val();
		var mediaPath = $("#" + selectedDiv).parent().parent().find("#hiddensrc").val();
		mediaPath = EditableText.prototype.StrikeOutQueryString(mediaPath);

		if (!EditableText.prototype.ValidateImage(ImageId, selectedDiv, IsImageTag, IsDynamicControl))
			return false;

		if (IsDynamicControl == "True") {
			switch (mediafamily) {
				case "Video":
					EditableText.prototype.UploadVideo(ImageId, selectedDiv, mediaPath);
					break;

				case "Audio":
					EditableText.prototype.UploadAudio(ImageId, selectedDiv, mediaPath);
					break;

				case "File":
					EditableText.prototype.UploadFile(ImageId, selectedDiv, mediaPath);
					break;

				default:
					EditableText.prototype.UploadImage(ImageId, selectedDiv, mediaPath);
			}
		}
		else if (mediafamily == "Image")
			EditableText.prototype.UploadImage(ImageId, selectedDiv, mediaPath);
		else
			ZnodeNotification.prototype.DisplayNotificationMessagesHelper(mediaPath.substring((mediaPath.lastIndexOf("/") + 1)) + ZnodeBase.prototype.getResourceByKeyName("InvalidfileTypeWithoutName"), "error", isFadeOut, fadeOutTime);

		$("#appendDiv").hide(200);
		$("#appendDiv").slideUp(200, function () {
			$(this).remove();
		});
		$("body").css('overflow', 'auto');
		$("#currentGroupCode").val('');
		$(".modal-backdrop").remove();
        $('#' + ImageId + '').closest("form").addClass('dirty'); //Enable dirty form for page.
	}

	UploadVideo(ImageId: string, selectedDiv: string, mediaPath: any) {
		$("input#" + ImageId).val(selectedDiv.split('_')[1]);
        var div = $('#Upload' + ImageId);
        var selectedFileName = EditableText.prototype.GetSelectedFileName(selectedDiv);
        div.html("<div id='" + ImageId + "'><div class='upload-video'> <i class='z-video'> </i><a class=\"upload-images-close\" data-toggle=\"tooltip\" data-placement=\"top\" title=\"Remove\" onclick='EditableText.prototype.RemoveMedia(\"" + ImageId + "\")'><i class='z-close-circle'></i></a></div><span>" + selectedFileName + "</span> </div>");
	}

	UploadAudio(ImageId: string, selectedDiv: string, mediaPath: any) {
		$("input#" + ImageId).val(selectedDiv.split('_')[1]);
        var div = $('#Upload' + ImageId);
        var selectedFileName = EditableText.prototype.GetSelectedFileName(selectedDiv);
        div.html("<div id='" + ImageId + "'><div class='upload-audio'><i class='z-audio'> </i><a class=\"upload-images-close\" data-toggle=\"tooltip\" data-placement=\"top\" title=\"Remove\" onclick='EditableText.prototype.RemoveMedia(\"" + ImageId + "\")'><i class='z-close-circle'></i></a></div><span>" + selectedFileName + "</span></div>");
	}

	UploadFile(ImageId: string, selectedDiv: string, mediaPath: any) {
		$("input#" + ImageId).val(selectedDiv.split('_')[1]);
        var div = $('#Upload' + ImageId);
        var selectedFileName = EditableText.prototype.GetSelectedFileName(selectedDiv);
        div.html("<div id='" + ImageId + "'><div class='upload-file dirtyignore'><i class='z-file-text'> </i> <a class=\"upload-images-close\" data-toggle=\"tooltip\" data-placement=\"top\" title=\"Remove\" onclick='EditableText.prototype.RemoveMedia(\"" + ImageId + "\")'><i class='z-close-circle'></i></a></div><span>" + selectedFileName + "</span></div>");
	}

	UploadImage(ImageId: string, selectedDiv: string, mediaPath: any) {
		$("#" + ImageId).attr('src', mediaPath);
		$("input#" + ImageId).val(selectedDiv.split('_')[1]);
        var div = $("div[id=div" + ImageId + "]");
		div.append("<a class=\"upload-images-close dirtyignore\" data-toggle=\"tooltip\" data-placement=\"top\" title=\"Remove\" onclick='EditableText.prototype.RemoveImage(\"" + ImageId + "\")'><i class='z-close-circle'></i></a>");
	}
	ValidateImage(ImageId: string, selectedDiv: string, IsImageTag: string, IsDynamicControl: string) {
		var uploadImageId = selectedDiv.split('_')[1];
		var mediaSize = $("#" + selectedDiv).parent().parent().find("#hdnSize_" + uploadImageId).val();
		var mediaPath = $("#" + selectedDiv).parent().parent().find("#hiddensrc").val();
		mediaPath = EditableText.prototype.StrikeOutQueryString(mediaPath);

		var mediaName = $("#" + selectedDiv).parent().parent().siblings("div").html();
		if (IsImageTag === "True" && typeof mediaPath == "undefined") {
			$("#divErrorMessage").show();
			$("#messageBoxMediaManagerContainerId").html(ZnodeBase.prototype.getResourceByKeyName("ErrorInvalidFileType"));
			return false;
		}
		if (IsDynamicControl == "True" && !EditableText.prototype.ValidateExtentions(ImageId, mediaPath)) {
			$("#divErrorMessage").show();
			$("#messageBoxMediaManagerContainerId").html(ZnodeBase.prototype.getResourceByKeyName("ErrorInvalidFileType"));
			return false;
		}
		if (IsDynamicControl == "True" && EditableText.prototype.ValidateFileSize(ImageId, mediaSize)) {
			$("#divErrorMessage").show();
			$("#messageBoxMediaManagerContainerId").html(mediaName + ZnodeBase.prototype.getResourceByKeyName("InvalidFileSize"));
			return false;
        }

        if (IsDynamicControl == "True" && EditableText.prototype.IsFileAlreadyUploaded(ImageId, uploadImageId)) {
            $("#divErrorMessage").show();
            $("#messageBoxMediaManagerContainerId").html(mediaName + ' is ' + ZnodeBase.prototype.getResourceByKeyName("ErrorAlreadyUploadedFile"));
            return false;
        }

		return true;
	}
	MultipleUpload(DivId: string, IsImageTag: string) {
		var ids = new Array();
		var isAnyImageExist = false;
		if (selectedImages != undefined && selectedImages.length > 0)
			ids = selectedImages;

		if (ids.length > 0) {
			if (IsImageTag === "True")
				EditableText.prototype.UploadMultipleImage(ids, DivId, isAnyImageExist);
			else
				EditableText.prototype.UploadMultipleFiles(ids, DivId, isAnyImageExist);

			$("#appendDiv").hide(200);
			$("#appendDiv").html('');
			$("#appendDiv").slideUp(200, function () {
				$(this).remove();
				$(".modal-backdrop").remove();
				$("body").css('overflow', 'auto');
			});
			$(".modal-backdrop").remove();

		}
		else {
			$("#divErrorMessage").show();
			$("#messageBoxMediaManagerContainerId").html(ZnodeBase.prototype.getResourceByKeyName("ErrorSelectAtLeastOneItem"));
		}
		$("#currentGroupCode").val('');
		$('#' + DivId + '').closest("form").addClass('dirty'); //Enable dirty form for page.
	}

	//For uploading multiple images.
	UploadMultipleImage(ids: any, attributeName: string, isAnyFileExist: boolean) {
		var presentDiv = $("#div" + attributeName);  //Current div where file need to attached.
		var idsArray = [];     //Selected Ids array
		var SrcArray = [];     //Source array of uploded Images/files.
		var imageIndex = 0;     //Index of uploded Images/files.
		var ExistingValue = $("input[name=" + attributeName + "]").val();
		var ExistingImageSrc = $("input[name=" + attributeName + "]").attr("src");

		if (ExistingValue != undefined && ExistingValue.length > 0) {
			var SplitExistingValue = ExistingValue.split(',');
			for (var i = 0; i < SplitExistingValue.length; i++) {
				idsArray.push(SplitExistingValue[i]);
			}
			imageIndex = SplitExistingValue.length;
		}
		if (ExistingImageSrc != undefined && ExistingImageSrc.length > 0) {
			var SpliExistingSrcValue = ExistingImageSrc.split(',');
			for (i = 0; i < SpliExistingSrcValue.length; i++) {
				SrcArray.push(SpliExistingSrcValue[i]);
			}
		}

		for (var j = 0; j < ids.length; j++) {

			if (typeof ids[j].source == "undefined")
				ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("InvalidfileType"), "error", false, fadeOutTime);
			else {
				if (!EditableText.prototype.ValidateExtentions(attributeName, ids[j].source)) {
					ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ids[j].source.substring((ids[j].source.lastIndexOf("/") + 1)) + ZnodeBase.prototype.getResourceByKeyName("InvalidfileTypeWithoutName"), "error", false, fadeOutTime);
					return;
				}
				//Check image exist or not.
				if (idsArray.indexOf(ids[j].values) == -1) {
					presentDiv.append("<div class=\"upload-images multi-upload-images dirtyignore\" ><img id='" + attributeName + "_" + imageIndex + "' src= '" + ids[j].source + "' class='img-responsive'>" +
						"<a class=\"upload-images-close\" data-toggle=\"tooltip\" data-placement=\"top\" title=\"Remove\" onclick='EditableText.prototype.RemoveMultipleImage(\"" + attributeName + "_" + imageIndex + "\")'><i class='z-close-circle'></i></a>" +
						"<input type='hidden' id='" + attributeName + "_" + imageIndex + "' value=" + ids[j].values.toString() + "></div>");
					idsArray.push(ids[j].values);
					SrcArray.push(ids[j].source);
				}
				else
					isAnyFileExist = true;
			}
			imageIndex = imageIndex + 1;
		}
		$("input[name=" + attributeName + "]").val(idsArray.join(","));
		$("input[name=" + attributeName + "]").prop("src", SrcArray.join(","));
	}

	//For uploading multiple files.
	UploadMultipleFiles(ids: any, DivId: string, isAnyFilesExist: boolean) {
		var div = $("#Upload" + DivId);
		var existingValueArray = [];
		var fileIndex = 0;

		var ExistingValue = $("input[name=" + DivId + "]").val();
		if (ExistingValue != undefined && ExistingValue.length > 0) {
			var SplitExistingValue = ExistingValue.split(',');
			for (var i = 0; i < SplitExistingValue.length; i++) {
				existingValueArray.push(SplitExistingValue[i]);
			}
			fileIndex = SplitExistingValue.length;
		}

        for (var i = 0; i < ids.length; i++) {
            var mediafamily = $('#appendDiv input[id=rowcheck_' + ids[i].values + ']').parent().find("#hiddenFamily").val();

			switch (mediafamily) {
				case "Video":
					div.append("<div id='" + DivId + "_" + fileIndex + "'><div class='upload-video'> <i class='z-video'> </i><a class=\"upload-images-close\" data-toggle=\"tooltip\" data-placement=\"top\" title=\"Remove\" onclick='EditableText.prototype.RemoveMultipleFiles(\"" + DivId + "_" + fileIndex + "\")'><i class='z-close-circle'></i></a></div><span>" + ids[i].text + "</span> </div>");
					break;

				case "Audio":
					div.append("<div id='" + DivId + "_" + fileIndex + "'><div class='upload-audio'><i class='z-audio'> </i><a class=\"upload-images-close\" data-toggle=\"tooltip\" data-placement=\"top\" title=\"Remove\" onclick='EditableText.prototype.RemoveMultipleFiles(\"" + DivId + "_" + fileIndex + "\")'><i class='z-close-circle'></i></a></div><span>" + ids[i].text + "</span></div>");
					break;

				default:
                    div.append("<div id='" + DivId + "_" + fileIndex + "'><div class='upload-file dirtyignore'><i class='z-file-text'> </i> <a class=\"upload-images-close\" data-toggle=\"tooltip\" data-placement=\"top\" title=\"Remove\" onclick='EditableText.prototype.RemoveMultipleFiles(\"" + DivId + "_" + fileIndex + "\")'>" +
						"<i class='z-close-circle'> </i></a> <input type='hidden' id='" + DivId + "_" + fileIndex + "' value='" + ids[i].values + "'> </div><span>" + ids[i].text + "</span> </div>");
					break;
			}

			existingValueArray.push(ids[i].values);
			fileIndex = fileIndex + 1;
		}

		$("input[name=" + DivId + "]").val(existingValueArray.join(","));
	}

	//Method for Get Multiple Upload files
	GetMultipleUploadFiles(controlName) {
		controlName = controlName.substring(3);
		var FileSource = $("#" + controlName).attr('src');
		var MediaIds = $("#" + controlName).val()
		var MediaIdArrays = MediaIds.split(',');
        var fileIndex = 0;

        var filesName = $("#hdnFileNameValue" + controlName).val();

        var filesNameArray = [];
        if (filesName) {
            filesNameArray = filesName.split(',');
        }

		if (MediaIds.length > 0 && FileSource != undefined && FileSource != "") {
			var fileSourceCollection = FileSource.split(',');
			if (fileSourceCollection.length > 0) {
				var presentDiv = $("#Upload" + controlName);
				presentDiv.html('');
				for (var i = 0; i < fileSourceCollection.length; i++) {
                    var mediaExtention = EditableText.prototype.GetMediaExtention(fileSourceCollection[i]);
                    var fileName = filesNameArray[i];
                    var uploadedFileName = (fileName ? fileName : fileSourceCollection[i].substring(fileSourceCollection[i].lastIndexOf('/') + 1));

					switch (mediaExtention) {
						case "Video":
							presentDiv.append("<div id='" + controlName + "_" + fileIndex + "'><div class='upload-video'> <i class='z-video'> </i><a class=\"upload-images-close\" data-toggle=\"tooltip\" data-placement=\"top\" title=\"Remove\" onclick='EditableText.prototype.RemoveMultipleFiles(\"" + controlName + "_" + fileIndex + "\")'>" +
                                "<i class='z-close-circle' > </i></a> <input type='hidden' id='" + controlName + "_" + fileIndex + "' src='" + fileSourceCollection[i] + "' value='" + MediaIdArrays[i] + "'> </div><span>" + uploadedFileName + "</span> </div>");
							break;

						case "Audio":
							presentDiv.append("<div id='" + controlName + "_" + fileIndex + "'><div class='upload-audio'><i class='z-audio'> </i><a class=\"upload-images-close\" data-toggle=\"tooltip\" data-placement=\"top\" title=\"Remove\" onclick='EditableText.prototype.RemoveMultipleFiles(\"" + controlName + "_" + fileIndex + "\")'>" +
                                "<i class='z-close-circle' > </i></a> <input type='hidden' id='" + controlName + "_" + fileIndex + "' src='" + fileSourceCollection[i] + "' value='" + MediaIdArrays[i] + "'> </div><span>" + uploadedFileName + "</span> </div>");
							break;

						default:
                            presentDiv.append("<div id='" + controlName + "_" + fileIndex + "'><div class='upload-file dirtyignore'><i class='z-file-text'> </i> <a class=\"upload-images-close\" data-toggle=\"tooltip\" data-placement=\"top\" title=\"Remove\" onclick='EditableText.prototype.RemoveMultipleFiles(\"" + controlName + "_" + fileIndex + "\")'>" +
                                "<i class='z-close-circle'> </i></a> <input type='hidden' id='" + controlName + "_" + fileIndex + "' src='" + fileSourceCollection[i] + "' value='" + MediaIdArrays[i] + "'> </div><span>" + uploadedFileName + "</span> </div>");
							break;
					}

					fileIndex = fileIndex + 1;
				}
			}
		}
	}

	GetMediaExtention(fileSource: String): String {
		var uploadFileType: string;
		var mediaExtention = fileSource.substring(fileSource.lastIndexOf('.') + 1);
		var audio = "3ga, aac, aif, flac, mid, mp3, wav, wma";
		var video = "mp4, ogv, webm";

		if (audio.indexOf(mediaExtention) > -1) {
			uploadFileType = "Audio";
		}
		else if (video.indexOf(mediaExtention) > -1) {
			uploadFileType = "Video";
		}
		else
			uploadFileType = "Files";

		return uploadFileType;
	}

    CancelUpload() {
        $("#appendDiv").slideUp(200, function () {
            $("#appendDiv").hide(5);
            $(this).remove();
            $(".mce-floatpanel").css("z-index", "65536");
            $("#mce-modal-block").css("z-index", "65535");
        });
        $("body").css('overflow','auto');
        $(".modal-backdrop").remove();
    }
    RemoveImage(ImageId: string) {
        $("#" + ImageId).attr('src', '');
        $("#" + ImageId).attr('src', '/MediaFolder/no-image.png');
        $("#div" + ImageId + " .upload-images-close").remove();
        $("input#" + ImageId).val('');
        $(".tooltip-arrow").fadeOut();
        $(".tooltip-inner").fadeOut();
    }

	//Function to remove multiple uploded images.
	RemoveMultipleImage(ImageId: string) {
		var removeImageValue = $("input[id=" + ImageId + "]").val();
		var removeImageUrl = $("img[id=" + ImageId + "]").prop("src");

		$("#" + ImageId).closest('div').remove();
		var divId = ImageId.substr(0, ImageId.lastIndexOf('_'));

		var inputValueArray = $('input#' + divId).val().split(',');
		var imageSrcArray = $('input#' + divId).attr("src").split(',');

		inputValueArray = jQuery.grep(inputValueArray, function (value) {
			return value != removeImageValue;
		});

		imageSrcArray = jQuery.grep(imageSrcArray, function (value) {
			return value != decodeURIComponent(removeImageUrl);
		});

		$("input[name=" + divId + "]").val(inputValueArray.toString());
		$("input[name=" + divId + "]").attr("src", imageSrcArray.toString());
	}

	//Function to remove multiple uploded files.
	RemoveMultipleFiles(fileId: string) {
		var removeFileValue = $("input[id=" + fileId + "]").val();
		var removeFileSource = $("input[id=" + fileId + "]").prop("src");

		$("#" + fileId).closest('div').remove();
		var actualFileId = fileId.substr(0, fileId.lastIndexOf('_'));
		var inputValueArray = $('input#' + actualFileId).val().split(',');
		var fileSrcArray = $('input#' + actualFileId).attr("src").split(',');

		inputValueArray = jQuery.grep(inputValueArray, function (value) {
			return value != removeFileValue;
		});

		fileSrcArray = jQuery.grep(fileSrcArray, function (value) {
			return value != decodeURIComponent(removeFileSource);
		});

		$("input[name=" + actualFileId + "]").val(inputValueArray.toString());
		$("input[name=" + actualFileId + "]").attr("src", fileSrcArray.toString());

	}

	DialogDelete(DataTarget: string, target = undefined) {
		var selectedIds = DynamicGrid.prototype.GetMultipleSelectedIds(target);
		if (selectedIds.length > 0) {
			$('#' + DataTarget + '').modal('show');
		}
		else {
			$('#NoCheckboxSelected').modal('show');
		}
	}

	ValidateExtentions(ImageId, fileName) {
        var supportedFileExtession = $("#hdn" + ImageId).val();
        fileName = EditableText.prototype.StrikeOutQueryString(fileName);
		var extension = fileName.substring(fileName.lastIndexOf(".")).toLowerCase();
		if (supportedFileExtession != undefined && supportedFileExtession.toLowerCase().indexOf(extension) >= 0)
			return true;

		return false;
	}
	ValidateFileSize(ImageId, mediaSize) {
		var maxfileSize = $("#hdnMediaSize" + ImageId).val();
		if (mediaSize != undefined && maxfileSize != undefined && maxfileSize != "") {
			var sizearray = mediaSize.split(' ');
			if (sizearray.length > 0 && !isNaN(sizearray[0])) {
				if (Number(sizearray[0]) > Number(maxfileSize))
					return true;
			}
		}
		return false;
	}
	RemoveMedia(MediaId: string) {
		$("div#" + MediaId).html('');
		$("input#" + MediaId).val('');
	}

	StrikeOutQueryString(url: string) {
		return url.split("?")[0].split("#")[0];
	}

    IsFileAlreadyUploaded(imageId, uploadImageId) {
        if (imageId != undefined && uploadImageId != undefined) {
            var existingUploadedIds = $("input[name=" + imageId + "]").val();

            if (existingUploadedIds != undefined && existingUploadedIds.length > 0) {
                var splitExistingUploadedIds = existingUploadedIds.split(',');

                for (var i = 0; i < splitExistingUploadedIds.length; i++) {
                    if (splitExistingUploadedIds[i] == uploadImageId) {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    GetSelectedFileName(selectedDiv) {
        var selectedDivImageId = selectedDiv.split('_')[1];
        var selectedDivObject = document.getElementById(selectedDiv);
        if (selectedDivObject) {
            return $(selectedDivObject).parent().parent().parent().find('.title').text();
        }
        return selectedDivImageId;
    }
}
$(document).ready(function () {
	var _editableText: EditableText;
	_editableText = new EditableText(window.document);
	_editableText.Init();
});
