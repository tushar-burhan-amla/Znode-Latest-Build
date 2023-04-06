function showUploadPopup() {
    var div = $("#divMediaUploaderPopup")
        , str = '<div class="col-sm-12 nopadding"><div id= "appendDiv" style="z-index=9999999 !important" class="media-upload-panel"> </div> </div>';
    div.html(str);
    var header = '<div class="col-sm-12 title-container">' +
        ' <h1>MEDIA UPLOAD </h1> ' +
        '<button type="button" style="color:white;" class="btn-text-icon pull-right" onclick="EditableText.prototype.CancelUpload(); RestoreImagePopupIndex();"><i class="z-back"></i>BACK</button>' +
        ' </div> ';
    var popupViewModel = {
        FolderId: "1",
        DisplayMode: "Tile",
        IsMultiSelect: !1,
        IsPopup: !0
    };
    $("body .tox-tinymce-aux").css("z-index", "0");
    $.ajax({
        url: "/MediaManager/MediaManager/List?displayMode=Tile",
        data: JSON.stringify(popupViewModel),
        type: "POST",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            isMediaPopup = !0,
                $("#appendDiv").html(""),
                $("#appendDiv").html(data),
                $("#FileUploader").removeAttr("style"),
                $("#appendDiv").prepend(header),
                $("#divMediaUploaderPopup").find("#FileUploader").show(),
                $("#appendDiv").slideDown(),
                $(".mce-floatpanel").css("z-index", "0"),
                $("#mce-modal-block").css("z-index", "0"),
                $.getScript("/Scripts/References/MediaUpload/script.js"),
                $.getScript("/Scripts/Core/Controls/TreeView.js"),
                $.getScript("/Scripts/Core/Znode/MediaManagerTools.js"),
                GridPager.prototype.Init(),
                $("#btnSubmitModel").hide(),
                $(".aside-popup-panel.modal-dialog.modal-xl.modal-xxl").append("<div class='modal-backdrop fade in'></div>"),
                $(document).off("click", ".img"),
                $(document).on("click", ".img", UploadSingleImage),
                TreeView.prototype.ReloadTree();
            var treeData = $("#Main_Tree").attr("data-tree")
                , obj = eval(treeData);
            selectedFolderId = obj[0].id
        }
    }),
        isUploadPopup = !0
}
function UploadSingleImage() {
    var e = $(this).data("rowcheckid");
    t = $("#" + e).parent().parent().find("#hiddensrc").val(),
        null != t && 0 == t.length && (t = uploadedMediaPath),
        $(".tox-dialog__body-content").find(".tox-control-wrap").find(".tox-textfield").val(t),
        $("#divRichtextboxModel").modal("hide"),
        $(".img").off("click"),
        $("#appendDiv").hide().slideUp(),
        $(".mce-floatpanel").css("z-index", "65536"),
        $("#mce-modal-block").css("z-index", "65535"),
        $("body .tox-tinymce-aux").css("z-index", "9999"),
        $("body").css("overflow", "hidden")
}
function UploadImages() {
    var t = [];
    $("#grid").find("tr").each(function () {
        if (0 < $(this).find("input[type=checkbox]").length && $(this).find("input[type=checkbox]").is(":checked")) {
            var e = $(this).find("input[type=checkbox]").parent().parent().find("img").attr("src");
            t.push(e)
        }
    }),
        0 == t.length && t.push(uploadedMediaPath),
        $(".mce-textbox.mce-placeholder").val(t)
}
function myFileBrowser(e, t, i, a) {
    var n = "/Scripts/tinymce/UploadFiles.html";
    n = n.indexOf("?") < 0 ? n + "?type=" + i : n + "&type=" + i;
    var o = "5242880"
        , l = ".jpg, .jpeg, .bmp, .gif, .png, .mp4, .avi";
    return "image" == i ? (o = "3145728",
        l = ".jpg, .jpeg, .bmp, .gif, .png") : "media" == i && (o = "5242880",
            l = ".mp4, .avi"),
        tinyMCE.activeEditor.windowManager.open({
            file: n,
            title: "File Browser",
            width: 420,
            height: 500,
            resizable: "yes",
            inline: "yes",
            close_previous: "no"
        }, {
            window: a,
            input: e,
            type: i,
            validSize: o,
            validExtensions: l
        }),
        !1
}

function GetFormatForEditor(callbackMethod) {
    var currentPortalId = parseInt($('#PortalId').val());
    if (isNaN(currentPortalId) == false)
        Endpoint.prototype.GetEditorFormats(currentPortalId, callbackMethod);
    else
        Endpoint.prototype.GetEditorFormats(0, callbackMethod);
}

function reInitializationMce() {
    $("textarea[wysiwygenabledproperty=true]").each(function () {
        var e = $(this).attr("id");
        var tinyMceObject = {
            selector: "#" + e,
            theme: "silver",
            browser_spellcheck: !0,
            setup: function (e) {
                e.on("change", function () {
                    e.save()
                }),
                    $("#" + e.id).attr("disabled") && (e.settings.readonly = !0)
            },
            plugins: "advlist anchor autolink autosave link image lists charmap preview anchor pagebreak searchreplace wordcount visualblocks visualchars code codesample fullscreen insertdatetime media nonbreaking quickbars table directionality template help save",
            content_css: [
                "/Scripts/tinymce/skins/content/default/content.css",
            ],
            menubar: 'file edit view insert format tools table help',
            toolbar: "insertfile undo redo | fullscreen | blocks | styleselect formatselect fontselect fontsizeselect | bold italic underline strikethrough  | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent blockquote | link unlink anchor image cleanup code | print preview media fullpage| insertdate inserttime | forecolor backcolor | tablecontrols | hr removeformat | sub sup | charmap advhr | ltr rtl",
            toolbar_mode: 'wrap',
            contextmenu: "cut copy paste | link image inserttable | cell row column deletetable",
            formats: {
            },
            style_formats: [
                {
                    title: "Bold text",
                    inline: "b"
                }, {
                    title: "Red text",
                    inline: "span",
                    styles: {
                        color: "#ff0000"
                    }
                }, {
                    title: "Red header",
                    block: "h1",
                    styles: {
                        color: "#ff0000"
                    }
                }, {
                    title: "Example 1",
                    inline: "span",
                    classes: "example1"
                }, {
                    title: "Example 2",
                    inline: "span",
                    classes: "example2"
                }, {
                    title: "Table styles"
                }, {
                    title: "Table row 1",
                    selector: "tr",
                    classes: "tablerow1"
                }],
            valid_elements: '*[*]',
            file_picker_callback: showUploadPopup
        };

        GetFormatForEditor(function (data) {
            getFormats(data);
            getStyleFormats(data);

            function getFormats(para) {
                if (para.FormatList != null) {
                    para.FormatList.forEach(function (item) {
                        var name = item.Name.replace(/ /g, '');
                        var formatString = item.Format.replace(/'/g, '"');
                        var formatJson = JSON.parse(formatString);
                        tinyMceObject.formats = Object.assign(tinyMceObject.formats, { [name]: formatJson });
                    });
                }
            }

            function getStyleFormats(para) {
                if (para.FormatList != null) {
                    para.FormatList.forEach(function (item) {
                        var title = item.Name;
                        var format = item.Name.replace(/ /g, '');
                        tinyMceObject.style_formats.push({ title: title, format: format });
                    });
                }
            }
        })
        tinymce.execCommand("mceRemoveEditor", !0, e),
            tinymce.execCommand("mceAddControl", !0, e),
            tinymce.init(tinyMceObject)
    })
}

function InitializeDirtyForm() {
    $.getScript("/Content/bootstrap-3.3.7/js/jquery.dirtyforms.js", function () {
        $.getScript("/Content/bootstrap-3.3.7/js/jquery.dirtyforms.dialogs.bootstrap.js", function () {
            var exclamationGlyphicon = '<span class="glyphicon glyphicon-exclamation-sign" aria-hidden="true"></span> ';

            $(document).bind('bind.dirtyforms', function (ev, events) {
                var originalBind = events.bind;

                events.bind = function (window, document, data) {
                    originalBind(window, document, data);
                    $('#ddlCultureSpan').bind('DOMSubtreeModified', events.onAnchorClick);
                };
            });

            $('form').dirtyForms({
                dialog: GetDirtyFormDialog(exclamationGlyphicon),
                message: 'You forgot to save your details. If you leave now, they will be lost forever.'
            });
        });
    });
}
function CheckDirty() {
    return !!$("form").hasClass($.DirtyForms.dirtyClass)
}
function ValidateUploadedImage(e) {
    var t = top.tinymce.activeEditor.windowManager.getParams()
        , i = t.validSize
        , a = t.validExtensions.split(",");
    if ("file" == e.type) {
        var n = e.value;
        if (0 < n.length) {
            for (var o = !1, l = 0; l < a.length; l++) {
                var r = a[l];
                if (n.substr(n.length - r.length, r.length).toLowerCase() == r.toLowerCase()) {
                    o = !0;
                    break
                }
            }
            if (!o)
                return alert("Sorry, " + n + " is invalid, allowed extensions are: " + a.join(", ")),
                    e.value = "",
                    !1
        }
        if (e.files[0].size > i)
            return alert("Sorry, " + n + " is invalid, Maximum file size should be less than 3MB. "),
                e.value = "",
                !1
    }
    return !0
}
function GetRichTextBoxEditorValue() {
    return tinyMCE.activeEditor.getContent({
        format: "raw"
    })
}
tinymce.init({
    editor_selector: ".mceEditor",
    theme: "silver",
    browser_spellcheck: !0,
    setup: function (e) {
        e.on("change", function () {
            e.save()
        }),
            $("#" + e.id).attr("disabled") && (e.settings.readonly = !0)
    },
    menubar: 'file edit view insert format tools table help',
    plugins: "advlist anchor autolink autosave link image lists charmap preview anchor pagebreak searchreplace wordcount visualblocks visualchars code codesample fullscreen insertdatetime media nonbreaking quickbars table directionality template help save",
    content_css: [
        "/Scripts/tinymce/skins/content/default/content.css",
    ],
    toolbar: "insertfile undo redo | fullscreen | blocks | styleselect formatselect fontselect fontsizeselect | bold italic underline strikethrough  | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent blockquote | link unlink anchor image cleanup code | print preview media fullpage| insertdate inserttime | forecolor backcolor | tablecontrols | hr removeformat | sub sup | charmap advhr | ltr rtl",
    toolbar_mode: 'wrap',
    spellchecker_language: "en",
    contextmenu: "cut copy paste | link image inserttable | cell row column deletetable",
    height: "480",
    convert_urls: false,
    style_formats: [{
        title: "Bold text",
        inline: "b"
    }, {
        title: "Red text",
        inline: "span",
        styles: {
            color: "#ff0000"
        }
    }, {
        title: "Red header",
        block: "h1",
        styles: {
            color: "#ff0000"
        }
    }, {
        title: "Example 1",
        inline: "span",
        classes: "example1"
    }, {
        title: "Example 2",
        inline: "span",
        classes: "example2"
    }, {
        title: "Table styles"
    }, {
        title: "Table row 1",
        selector: "tr",
        classes: "tablerow1"
    }],
    valid_elements: '*[*]',
    file_picker_callback: showUploadPopup
});
var FileBrowserDialogue = {
    init: function () { },
    mySubmit: function () {
        var a = top.tinymce.activeEditor.windowManager.getParams()
            , e = (a.type,
                new FormData)
            , n = []
            , t = document.getElementById("FileUpload").files.length;
        $("#grid").find("tr").each(function () {
            if (0 < $(this).find("input[type=checkbox]").length && $(this).find("input[type=checkbox]").is(":checked")) {
                var e = $(this).find("input[type=checkbox]").parent().parent().find("img").attr("src");
                e = e.replace("Thumbnail", ""),
                    n.push(e)
            }
        });
        for (var i = 0; i < t; i++) {
            var o = document.getElementById("FileUpload").files[i];
            e.append("FileUpload", o),
                e.append("files", o),
                e.append("folderId", 1),
                e.append("isOverrideFile", o.name)
        }
        $.ajax({
            type: "POST",
            url: "/MediaManager/MediaManager/UploadFiles",
            data: e,
            processData: !1,
            contentType: !1,
            success: function (e) {
                var t = n
                    , i = a.window;
                i.document.getElementById(a.input).value = t,
                    void 0 !== i.ImageDialog && (i.ImageDialog.getImageData && i.ImageDialog.getImageData(),
                        i.ImageDialog.showPreviewImage && i.ImageDialog.showPreviewImage(t)),
                    top.tinymce.activeEditor.windowManager.close()
            },
            error: function (e) { }
        })
    }
};

function GetDirtyFormDialog(exclamationGlyphicon) {
    return {
        title: exclamationGlyphicon + 'Are you sure you want to leave this page?',
        proceedButtonClass: 'dirty-proceed',
        proceedButtonText: 'Leave This Page',
        stayButtonClass: 'dirty-stay',
        stayButtonText: 'Stay Here',
        dialogID: 'dirty-dialog',
        titleID: 'dirty-title',
        messageClass: 'dirty-message',
        preMessageText: '',
        postMessageText: '',
        replaceText: true,
    };
}

function RestoreImagePopupIndex() {
    $("body .tox-tinymce-aux").css("z-index", "9999");
}

$(document).ready(function () {
    $(".tox-silver-sink").css("z-index", "1001");
});