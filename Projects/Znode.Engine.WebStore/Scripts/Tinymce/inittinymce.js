
tinymce.init({
    selector: ".mceEditor",
    theme: "silver",
    setup: function (editor) {
        editor.on('change', function () {
            editor.save();
        });
        if ($('#' + editor.id).attr("disabled"))
            editor.settings.readonly = true;
    },
    menubar: 'file edit view insert format tools table help',
    plugins: "advlist code anchor autolink autosave link image lists charmap preview anchor pagebreak searchreplace wordcount visualblocks visualchars codesample fullscreen insertdatetime media nonbreaking quickbars table directionality template help save",
    content_css: [
        "/Scripts/tinymce/skins/content/default/content.css",
    ],
    toolbar: "insertfile undo redo | fullscreen | blocks | styleselect formatselect fontselect fontsizeselect | bold italic underline strikethrough  | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent blockquote | link unlink anchor image cleanup code | print preview media fullpage| insertdate inserttime | forecolor backcolor | tablecontrols | hr removeformat | sub sup | charmap advhr | ltr rtl",
    toolbar_mode: 'wrap',
    style_formats: [
        { title: 'Bold text', inline: 'b' },
        { title: 'Red text', inline: 'span', styles: { color: '#ff0000' } },
        { title: 'Red header', block: 'h1', styles: { color: '#ff0000' } },
        { title: 'Example 1', inline: 'span', classes: 'example1' },
        { title: 'Example 2', inline: 'span', classes: 'example2' },
        { title: 'Table styles' },
        { title: 'Table row 1', selector: 'tr', classes: 'tablerow1' }
    ],
    file_picker_callback: showUploadPopup,
    valid_elements: '*[*]'
});

function showUploadPopup() {
    var div = $("#divMediaUploaderPopup");
    var str = '<div class="col-sm-12 nopadding"><div id= "appendDiv" style="z-index=9999999 !important" class="media-upload-panel"> </div> </div>';
    var close = '<button type="button" class="media-upload-close" onclick="EditableText.prototype.CancelUpload()"><i class="z-close"></i></button>';
    div.html(str);

    var popupViewModel = { "FolderId": "1", "DisplayMode": "Tile", "IsMultiSelect": false, "IsPopup": true };

    $.ajax({
        url: "/MediaManager/MediaManager/List?displayMode=Tile",
        data: JSON.stringify(popupViewModel),
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            isMediaPopup = true;
            $("#appendDiv").html('');
            $("#appendDiv").html(data);
            $("#appendDiv").append(close);
            $("#FileUploader").removeAttr("style");
            $("#divMediaUploaderPopup").find("#FileUploader").show();
            $("#appendDiv").show(700);
            $("body").css('overflow', 'hidden');
            $.getScript("/Scripts/References/MediaUpload/script.js");
            $.getScript("/Scripts/Core/Controls/TreeView.js");
            $.getScript("/Scripts/Core/Znode/MediaManagerTools.js");
            GridPager.prototype.Init();
            $("#btnSubmitModel").hide();
            $(document).off("click", ".img");
            $(document).on("click", ".img", UploadSingleImage);
            TreeView.prototype.ReloadTree();

            var treeData = $('#Main_Tree').attr('data-tree');
            var obj = eval(treeData);
            selectedFolderId = obj[0].id;
        }
    });
    isUploadPopup = true;
}

function UploadSingleImage() {
    var selectedId = $(this).data("rowcheckid");
    var selectedUrl = $("#" + selectedId).parent().parent().find("#hiddensrc").val();
    if (selectedUrl != undefined && selectedUrl.length == 0)
        selectedUrl = uploadedMediaPath;
    $(".mce-container").find(".mce-combobox").find(".mce-textbox").val(selectedUrl);
    $('#divRichtextboxModel').modal('hide');
    $(".img").off('click');
    $("#appendDiv").hide(700);
}

function UploadImages() {
    var arrayURL = [];
    $("#grid").find("tr").each(function () {
        if ($(this).find("input[type=checkbox]").length > 0) {
            if ($(this).find("input[type=checkbox]").is(":checked")) {
                var URL = $(this).find("input[type=checkbox]").parent().parent().find("img").attr("src");
                //URL = URL.replace('Thumbnail/', '');
                arrayURL.push(URL);
            }
        }
    });
    if (arrayURL.length == 0)
        arrayURL.push(uploadedMediaPath);
    $(".mce-textbox.mce-placeholder").val(arrayURL);
}


function myFileBrowser(field_name, url, type, win) {

    var cmsURL = "/Scripts/tinymce/UploadFiles.html";    // script URL - use an absolute path!
    if (cmsURL.indexOf("?") < 0) {
        //add the type as the only query parameter
        cmsURL = cmsURL + "?type=" + type;
    }
    else {
        //add the type as an additional query parameter
        cmsURL = cmsURL + "&type=" + type;
    }

    //Set valid Size & Extensions for image(add/Edit Image) & Media(Add/Edit Video).
    var _validSize = '5242880';
    var _validExtensions = '.jpg, .jpeg, .bmp, .gif, .png, .mp4, .avi';
    if (type == "image") {
        _validSize = '3145728';
        _validExtensions = '.jpg, .jpeg, .bmp, .gif, .png';
    }
    else if (type == "media") {
        _validSize = '5242880';
        _validExtensions = '.mp4, .avi';
    }


    //tinymce.activeEditor.uploadImages(function (success) { $.post('ajax/post.php', tinymce.activeEditor.getContent()).done(function () { console.log("Uploaded images and posted content as an ajax request."); }); });

    tinyMCE.activeEditor.windowManager.open({
        file: cmsURL,
        title: 'File Browser',
        width: 420,  // Your dimensions may differ - toy around with them!
        height: 500,
        resizable: "yes",
        inline: "yes",  // This parameter only has an effect if you use the inlinepopups plugin!
        close_previous: "no"
    }, {
        window: win,
        input: field_name,
        type: type,
        validSize: _validSize,
        validExtensions: _validExtensions
    });
    return false;
};

function reInitializationMce() {
    $("textarea[wysiwygenabledproperty=true]").each(function () {
        var _id = $(this).attr("id");
        tinymce.execCommand('mceRemoveEditor', true, _id);
        tinymce.execCommand('mceAddControl', true, _id);
        tinymce.init({
            selector: '#' + _id,
            theme: "silver",
            setup: function (editor) {
                editor.on('change', function () {
                    editor.save();
                });
                if ($('#' + editor.id).attr("disabled"))
                    editor.settings.readonly = true;
            },
            plugins: "advlist anchor autolink autosave link image lists charmap preview anchor pagebreak searchreplace wordcount visualblocks visualchars code codesample fullscreen insertdatetime media nonbreaking quickbars table directionality template help save",
            content_css: [
                "/Scripts/tinymce/skins/content/default/content.css",
            ],
            menubar: 'file edit view insert format tools table help',
            toolbar: "insertfile undo redo | fullscreen | blocks | styleselect formatselect fontselect fontsizeselect | bold italic underline strikethrough  | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent blockquote | link unlink anchor image cleanup code | print preview media fullpage| insertdate inserttime | forecolor backcolor | tablecontrols | hr removeformat | sub sup | charmap advhr | ltr rtl",
            toolbar_mode: 'wrap',
            style_formats: [
                { title: 'Bold text', inline: 'b' },
                { title: 'Red text', inline: 'span', styles: { color: '#ff0000' } },
                { title: 'Red header', block: 'h1', styles: { color: '#ff0000' } },
                { title: 'Example 1', inline: 'span', classes: 'example1' },
                { title: 'Example 2', inline: 'span', classes: 'example2' },
                { title: 'Table styles' },
                { title: 'Table row 1', selector: 'tr', classes: 'tablerow1' }
            ],
            file_picker_callback: showUploadPopup
        });
    });
}

function InitializeDirtyForm() {
    $.getScript("/Content/bootstrap/js/jquery.dirtyforms.js", function () {
        $.getScript("/Content/bootstrap/js/jquery.dirtyforms.dialogs.bootstrap.js", function () {
            var exclamationGlyphicon = '<span class="glyphicon glyphicon-exclamation-sign" aria-hidden="true"></span> ';

            $(document).bind('bind.dirtyforms', function (ev, events) {
                var originalBind = events.bind;

                events.bind = function (window, document, data) {
                    originalBind(window, document, data);
                    $('#ddlCultureSpan').bind('DOMSubtreeModified', events.onAnchorClick);
                };
            });

            $('form').dirtyForms({
                dialog: GetDirtyFormDialogBox(exclamationGlyphicon),
                message: 'You forgot to save your details. If you leave now, they will be lost forever.'
            });
        });
    });
}

function CheckDirty() {
    if ($('form').hasClass($.DirtyForms.dirtyClass))
        return true;
    else
        return false;
}

function GetDirtyFormDialogBox(exclamationGlyphicon) {
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