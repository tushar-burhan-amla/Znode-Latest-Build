
interface HTMLAnchorElement {
    download: string;
}

class MediaManagerTools {
    selectedFolderId;
    isMoveFolder;

    constructor(doc: HTMLDocument) {
        this.isMoveFolder = false;
        this.selectedFolderId = null;
    }

    SortSuccesssCallback() {
        $("#btnsearch").closest("form").submit();
    }

    Download() {
        EditableText.prototype.DialogDelete("DownloadMedia");
        var arrayURL = [];
        var arrayFileNames = [];
        $("#grid").find("tr").each(function () {
            if ($(this).find(".grid-row-checkbox").length > 0) {
                if ($(this).find(".grid-row-checkbox").is(":checked")) {
                    var viewModeType = $('#viewmode').attr("value");
                    var URL = viewModeType == "Tile"
                        ? $(this).find(".grid-row-checkbox").parent().parent().parent().parent().parent().find(".imageicon").find("img,i").attr("src")
                        : $(this).find(".grid-row-checkbox").parent().parent().find(".imageicon").find("img,i").attr("src");
                    var FileName = viewModeType == "Tile"
                        ? $(this).find(".grid-row-checkbox").parent().parent().parent().parent().parent().find(".filename").find("a").text()
                        : $(this).find(".grid-row-checkbox").parent().parent().find(".filename").find("a").text();
                    arrayURL.push(URL);
                    arrayFileNames.push(FileName);
                }
            }
        });
        MediaManagerTools.prototype.multiDownload(arrayURL, arrayFileNames);
    }

    multiDownload(urls,fileNames) {
        if (!urls || urls.length < 1) {
            throw new Error('`urls` required');
        }
        if (!fileNames ) {
            throw new Error('`fileNames` required');
        }
        var delay: number = 0;
        for (var i = 0; i < urls.length; i++) {
            setTimeout(this.SaveToDisk.bind(null, urls[i], fileNames[i]), 100 * ++delay);
        }
    }

    SaveToDisk(url, fileName) {
        fetch(url)
            .then(response => response.blob())
            .then(blob => {
                const link = document.createElement("a");
                link.href = URL.createObjectURL(blob);
                link.download = fileName;
                link.click();
            })
            .catch(console.error);
    }

    DownloadMedia() {
        MediaManagerTools.prototype.SaveToDisk($("#MediaPath").val(), $("#FileName").val());
    }

    SetCanvas(image) {
        var canvas = document.createElement('canvas');
        canvas.width = image.width;
        canvas.height = image.height;
        canvas.getContext('2d').drawImage(image, 0, 0);
        var blob = canvas.toDataURL("image");
        return blob;
    }
    isFirefox() {
        return /Firefox\//i.test(navigator.userAgent);
    }

    sameDomain(url) {
        var a = document.createElement('a');
        a.href = url;
        return location.hostname === a.hostname && location.protocol === a.protocol;
    }

    unique() {
        var ids = [];
        $("#grid").find("tr").each(function () {
            if ($(this).find(".grid-row-checkbox").length > 0) {
                if ($(this).find(".grid-row-checkbox").is(":checked")) {
                    var id = $(this).find(".grid-row-checkbox").attr("id").split("_")[1];
                    ids.push(id);
                }
            }
        });
        var result = [];
        $.each(ids, function (i, e) {
            if ($.inArray(e, result) == -1) result.push(e);
        });
        return result;
    }

    DeleteMediaPermanent(isFolderDelete) {
        var ref = $('#Main_Tree').jstree(true),
            selectedNode = ref.get_selected();
        if (selectedNode == null) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorSelectFolderToDelete"), 'error', isFadeOut, fadeOutTime);
        }
        else {
            if (isFolderDelete) {
                ZnodeBase.prototype.ShowLoader();
                Endpoint.prototype.FolderDelete(selectedNode[0].toString(), function (data) {
                    ZnodeBase.prototype.HideLoader();
                    TreeView.prototype.RebindMediaStructureTreeData(data.FolderJsonTree);
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(data.Message, data.HasNoError ? 'success' : 'error', isFadeOut, fadeOutTime);
                    var parentId = ref.get_parent(selectedNode);
                    ref.delete_node(selectedNode);
                    $('#Main_Tree' + ' #' + parentId + '_anchor').click();
                });
            }
            else {
                var ids = DynamicGrid.prototype.GetMultipleSelectedIds();
                ZnodeBase.prototype.ShowLoader();
                Endpoint.prototype.MediaDelete(ids, function (data) {
                    ZnodeBase.prototype.HideLoader();
                    DynamicGrid.prototype.RefreshGridOndelete($("#View_GetMediaPathDetail").find("#refreshGrid"), data);
                });
            }
            }   
    }

    GetSelectednodeIdOfMainTree() {
        var selectedNode = new Array();
        var ref = $('#Main_Tree').jstree(true),
            selectedNode = ref.get_selected();

        //If not a single folder id is selected
        if (selectedNode.length == 0) {
            var treeData = $('#Main_Tree').attr('data-tree');
            var obj = eval(treeData);
            var folderId = $('#hdnMediaFolderId').val();
            if (folderId == "0" || folderId == "-1") {
                folderId = obj[0].id;
            }
            return folderId;
        }

        if ($('#isUploadPopup') != undefined && $('#isUploadPopup').val() == "true") {
            var treeData = $('#Main_Tree').attr('data-tree');
            var obj = eval(treeData);
            if (obj[0].id == 1)
                selectedNode[0] = -1;
            return selectedNode = obj[0].id;
        }
        else {
            if (selectedNode[0] == 1)
                selectedNode[0] = -1;
            return selectedNode[0];
        }

    }

    GetSelectednodeId() {
        var selectedNode = new Array();
        var ref = $('#Popup_Tree').jstree(true),
            selectedNode = ref.get_selected();
        return selectedNode[0];
    }

    SeIsMoveFolderValue(modelPopup) {
        this.isMoveFolder = true;
        EditableText.prototype.DialogDelete(modelPopup);
    }

    MoveCopyMedia() {
        var ids = this.unique().toString();
        var selectedNode = this.GetSelectednodeId();
        if (selectedNode == null || selectedNode == "") {
            $("#TreePopupError").show();
            return false;
        }
        $("#TreePopupError").hide();
        ZnodeBase.prototype.ShowLoader();
        Endpoint.prototype.MoveMedia(selectedNode, ids, function (data) {
            ZnodeBase.prototype.HideLoader();
            $("#TreePopupCancel").click();
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(data.Message, data.HasNoError ? 'success' : 'error', isFadeOut, fadeOutTime);
            if (MediaManagerTools.prototype.isMoveFolder) {
                $("#searchform").submit();
            }
        });
        DynamicGrid.prototype.ClearCheckboxArray();
    }

    Delete(isFolderDelete) {
        var ids = this.unique();
        if (ids.length == 0) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorSelectMediaToDelete"), 'error', isFadeOut, fadeOutTime);
        }
        else {
            MediaManagerTools.prototype.DeleteMediaPermanent(isFolderDelete);
        }
    }

    DeleteFolder(isFolderDelete) {
        var ref = $('#Main_Tree').jstree(true),
            selectedNode = ref.get_selected();

        if (selectedNode == null) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorSelectFolderToDelete"), 'error', isFadeOut, fadeOutTime);
        }
        else {
            MediaManagerTools.prototype.DeleteMediaPermanent(isFolderDelete);
        }
    }

    ShowMainPage() {
        $("#backtolist").off("click");
        $("#backtolist").on('click', function (e) {
            $("#MainPage").show();
            $("#FileUploader").hide();
            var _newUrl = MediaManagerTools.prototype.UpdateQueryString("displayMode", "List", window.location.href);
            var _newUrl = MediaManagerTools.prototype.UpdateQueryString("mode", "List", _newUrl);
            window.history.pushState({ path: _newUrl }, '', _newUrl);
            e.preventDefault();
            var selectedFolderId = MediaManagerTools.prototype.GetSelectednodeIdOfMainTree();
            if (selectedFolderId == null || selectedFolderId == "") {
                return false;
            }
            GridPager.prototype.SetUpdateContainerId($("#hdnFrontObjectName").val());
            MediaManagerTools.prototype.RedirectToMediaManagerPageWithFolderId("List", selectedFolderId);
        });
    }

    ShowUploder() {
        $("#AddMediabtn").unbind("click");
        $("#AddMediabtn").on('click', function (e) {
            e.preventDefault();
            $("#MainPage").hide();
            $("#FileUploader").show();

            var _newUrl = MediaManagerTools.prototype.UpdateQueryString("displayMode", "Tile", window.location.href);

            _newUrl = MediaManagerTools.prototype.UpdateQueryString("mode", "add", _newUrl);
            window.history.pushState({ path: _newUrl }, '', _newUrl);

            var selectedFolderId = MediaManagerTools.prototype.GetSelectednodeIdOfMainTree();
            if (selectedFolderId == null || selectedFolderId == "") {
                return false;
            }
            GridPager.prototype.SetUpdateContainerId($("#hdnFrontObjectName").val());
            MediaManagerTools.prototype.RedirectToMediaManagerPageWithFolderId("Tile", selectedFolderId);
        });
    }

    UpdateQueryString(key, value, url) {
        if (!url) url = window.location.href;
        var re = new RegExp("([?&])" + key + "=.*?(&|#|$)(.*)", "gi"),
            hash;

        if (re.test(url)) {
            if (typeof value !== 'undefined' && value !== null)
                return url.replace(re, '$1' + key + "=" + value + '$2$3');
            else {
                hash = url.split('#');
                url = hash[0].replace(re, '$1$3').replace(/(&|\?)$/, '');
                if (typeof hash[1] !== 'undefined' && hash[1] !== null)
                    url += '#' + hash[1];
                return url;
            }
        }
        else {
            if (typeof value !== 'undefined' && value !== null) {
                var separator = url.indexOf('?') !== -1 ? '&' : '?';
                hash = url.split('#');
                url = hash[0] + separator + key + '=' + value;
                if (typeof hash[1] !== 'undefined' && hash[1] !== null)
                    url += '#' + hash[1];
                return url;
            }
            else
                return url;
        }
    }
    RedirectToMediaManagerPageWithFolderId(viewMode, selectedFolderId) {
        var currentPageNumber = 0;
        if (typeof PageIndex != 'undefined') {
            currentPageNumber = parseInt(PageIndex.toString()) + 1;
        }
        else { currentPageNumber = 1; }
        var requestedPage = currentPageNumber;
        var url = GridPager.prototype.GetRedirectUrl();
        if (url.indexOf("?") > -1) {
            url = window.location.href;
        }
        if (requestedPage <= 0) {
            requestedPage = currentPageNumber;
        }
        MediaManagerTools.prototype.SetCustomJurlWithFolderId(requestedPage, viewMode, url, selectedFolderId);
    }

    SetCustomJurlWithFolderId(requestedPage, _viewMode, url, selectedFolderId) {
        var _customUri = new CustomJurl();
        var newUrlParameter = _customUri.setQueryParameter(RecordPerPageFieldName, PageSize);
        newUrlParameter = _customUri.setQueryParameter(PageFieldName, requestedPage);
        newUrlParameter = _customUri.setQueryParameter("ViewMode", _viewMode);
        newUrlParameter = _customUri.setQueryParameter("folderId", selectedFolderId);
        if (Sort != null) {
            newUrlParameter = _customUri.setQueryParameter(SortFieldName, Sort);
            newUrlParameter = _customUri.setQueryParameter(SortDirFieldName, SortDir);
        }
        var newUrl = _customUri.build(url, newUrlParameter);
        GridPager.prototype.pagingUrlHandler(newUrl);
    }

    HideStatusGrid() {
        $('#fileuploadstatus table tbody').html("");
        $("#fileuploadstatus").hide();
    }

    ClickCheckGrid() {
        $(".img input[name=MediaId]").on("click", function () {
            $(this).parents('.imageicon').trigger("click");
        });
    }
}


class MediaManager {
    Init() {
        var _ready: MediaManagerTools;
        _ready = new MediaManagerTools(window.document);

        var id = $("#DropDownId").val();
        var enable = $("#IsDraggable").val();
        $("#" + id).sortable({
            //enable: enable,
            axis: "y",
            cursor: "move",
            containment: "#" + id,
            stop: function (event, ui) {
                var ids = [];
                $("#" + id).find(".Main_Dropdown").find("li").each(function () {
                    ids.push(parseInt($(this).find(".btncheckbox").data("value")));
                });
                if (enable) {
                    var controller = '@Model.Controller';
                    var Action = "@Model.SortAction";
                    console.log(ids.toString());
                    $.ajax({
                        url: "/" + controller + "/" + Action,
                        data: { id: ids.toString() },
                        method: "GET",
                        dataType: "json",
                        success: function (data) {
                            ZnodeBase.prototype.executeFunctionByName("MediaManagerTools.prototype.SortSuccesssCallback", window, data);
                        },
                        error: function (data) {
                            console.log(data);
                        }
                    });
                }
            }

        }).disableSelection();

        $('#Main_Tree').on('changed.jstree', function (e, data) {
            var i, lengthOftree, folderIds = [];
            for (i = 0, lengthOftree = data.selected.length; i < lengthOftree; i++) {
                folderIds.push(data.instance.get_node(data.selected[i]).id);
            }

            _ready.selectedFolderId = folderIds[0];
        });

        MediaManagerTools.prototype.ShowMainPage();

        MediaManagerTools.prototype.ShowUploder();

        // Preview Media Edit Img.
        var $preview = $("<div id='Media-Preview' class='preview-overlay' ><p class='media-preview'><img src='' alt='Image preview' class='preview-img' /><button data-dismiss='modal' class='close' type='button'><i class='z-close-circle'></i></button></p></div>");
        $("body").append($preview);
        $preview.hide();
        $("#Media-Preview button").click(function (e) {
            $("#Media-Preview").fadeOut();
            $(".preview-overlay").fadeOut();
        });
        $(".preview-link").click(function (e) {
            e.preventDefault();
            var href = $(this).attr('src');
            $("#Media-Preview").fadeIn();
            $("#Media-Preview img").attr('src', href);
        });

        $(document).off("click", ".z-edit");
        $(document).on("click", ".z-edit", function () {            
            var href = $(this).attr('href');
            $(this).attr('href', href + '&selectedfolder=' + _ready.selectedFolderId + '');
            return true;
        });

        $(document).off("click", ".filename");
        $(document).on("click", ".filename", function () {
            var href = $(this).children().attr('href');
            $(this).children().attr('href', href + '&selectedfolder=' + _ready.selectedFolderId + '');
        });

        MediaManager.prototype.BindTreeView();
    }

    BindTreeView(): void {       
        var _ready: TreeView;
        _ready = new TreeView(window.document);
        _ready.LoadTree();
        _ready.BindEvent();
        _ready.PopupTree();

        $(document).off("keyup", ".treesearch");
        $(document).on("keyup", ".treesearch", function (e) {
            if (e.keyCode === 13) {
                var searchText = $('.treesearch').val();
                var result = $('#Main_Tree').jstree('search', searchText);
                if ($(result).find('.jstree-search').length == 0 && searchText != "")
                    $('#searchresult').html(ZnodeBase.prototype.getResourceByKeyName("NoResult"));
                else
                    $('#searchresult').html("");
            }
            else {
                e.preventDefault();
                return false;
            }
        });
    }
}


