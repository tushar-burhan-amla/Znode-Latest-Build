
class TreeView {
    _endpoints: Endpoint;
    _callBackCount = 0;

    constructor(doc: HTMLDocument) {
        this._endpoints = new Endpoint();
        this._callBackCount = 0;
    }

    create() {
        var mediaStructureTree = $('#Main_Tree').jstree(true),
            selectedNode = mediaStructureTree.get_selected();
        if (!selectedNode.length) { return false; }
        selectedNode = selectedNode[0];
        var createdNode = mediaStructureTree.create_node(selectedNode, { "type": "file" });

        if (createdNode.length > 0) {
            mediaStructureTree.edit(createdNode, "New Folder", this.editCurrentNode.bind(this));
            $('.jstree-rename-input').attr('maxLength', 100);
        }
    }

    public editCurrentNode(obj: any): any {
        if (this.IsFolderNameValid(obj) && obj.parent != null && obj.parent != "") {
            if (obj.text.length <= 100) {
                ZnodeBase.prototype.ShowLoader();
                Endpoint.prototype.MediaAddFolder(obj.parent, obj.text, function (data) {
                    $('#Main_Tree').jstree(true).set_id(obj, data.Id);
                    ZnodeBase.prototype.HideLoader();
                    TreeView.prototype.RebindMediaStructureTreeData(data.FolderJsonTree);
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(data.Message, data.HasNoError ? 'success' : 'error', isFadeOut, fadeOutTime);
                });
            }
            else {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorFolderName"), 'error', isFadeOut, fadeOutTime);
                return false;
            }
        }
        else {
            this.ReloadTree();
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorSameNameFolder"), 'error', isFadeOut, fadeOutTime);
            return false;
        }
    }

    rename() {
        var mediaStructureTree = $('#Main_Tree').jstree(true),
            selectedNode = mediaStructureTree.get_selected();
        if ($('.jstree-clicked').length == 1) {
            var selectedNodeText = $('.jstree-clicked').text();
        }
        if (!selectedNode.length) { return false; }
        selectedNode = selectedNode[0];
        if (selectedNode) {
            mediaStructureTree.edit(selectedNode, selectedNodeText, this.MediaRenameFolder.bind(this));
            $('.jstree-rename-input').attr('maxLength', 100);
        }
    }

    public MediaRenameFolder(obj: any): any {
        if (this.IsFolderNameValid(obj) && obj.id != null && obj.id != "") {
            if (obj.text.length <= 100) {
                ZnodeBase.prototype.ShowLoader();
                Endpoint.prototype.MediaRenameFolder(obj.id, obj.text, function (data) {
                    ZnodeBase.prototype.HideLoader();
                    TreeView.prototype.RebindMediaStructureTreeData(data.FolderJsonTree);
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(data.Message, data.HasNoError ? 'success' : 'error', isFadeOut, fadeOutTime);
                });
            }
            else {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorFolderName"), 'error', isFadeOut, fadeOutTime);
                return false;
            }
        }
        else {
            this.ReloadTree();
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorSameNameFolder"), 'error', isFadeOut, fadeOutTime);
            return false;
        }
    }

    share() {
        var mediaStructureTree = $('#Main_Tree').jstree(true),
            selectedNode = mediaStructureTree.get_selected();
        if (!selectedNode.length) { return false; }
        selectedNode = selectedNode[0];
        if (selectedNode) {
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.MediaUserList(selectedNode.toString(), function (response) {
                if (response != null && response != "") {
                    ZnodeBase.prototype.HideLoader();
                    $("#share-btn").html('<button type="button" id="sharebtnId" data-toggle="modal" data-target="#useraccountlist" title="Share">Share</button>')
                    $("#sharebtnId").click();
                    $("#useraccountlist").html(response);
                    $("#useraccountlist").show();
                }
            });
        }
    }

    remove() {
        TreeView.prototype.BindEvent();
        var isRoot = $("#IsRootFolder").val();
        if (isRoot != undefined && isRoot != "" && isRoot == "false") {
            $("#share-btn").html('<button type="button" id="btnDeleteFolder" data-toggle="modal" data-target="#MediaFolderDeletePopup" title="Share">Share</button>')
            $("#btnDeleteFolder").click();
        }
        else {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorRootFolderCanNotDelete"), 'error', isFadeOut, fadeOutTime);
        }
    }

    RebindMediaStructureTreeData(updatedData: string): void {
        $('#Popup_Tree').attr('data-tree', updatedData);
        $('#Main_Tree').attr('data-tree', updatedData);
        $("#Popup_Tree").jstree('destroy');
        TreeView.prototype.PopupTree();
    }


    LoadTree() {
        var treeData = $('#Main_Tree').attr('data-tree');
        var obj = eval(treeData);
        $('#Main_Tree').jstree({
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
    }

    PopupTree() {
        var treeData = $('#Popup_Tree').attr('data-tree');
        var obj = eval(treeData);
        $("#TreePopupError").hide();
        $('#Popup_Tree').jstree({
            'core': {
                "animation": 0,
                "check_callback": true,
                data: obj
            },
            "search": {
                "case_insensitive": true,
                "show_only_matches": true
            },
            "plugins": ["contextmenu", "dnd", "search", "state", "wholerow"],
        });
    }

    BindEvent() {
        $("#Main_Tree").off('ready.jstree');
        $("#Main_Tree").on('ready.jstree', function (e, data) {
            var treeData = $('#Main_Tree').attr('data-tree');
            var obj = eval(treeData);
            var folderId = $('#hdnMediaFolderId').val();
            if (folderId === undefined || folderId == "0" || folderId == "-1" || data.instance.get_parent(folderId) == "#") {
                folderId = obj[0].id;
                $(".jstree-icon").click();
            }
            $('#Main_Tree').jstree(true).deselect_all();
            $('#Main_Tree').jstree('select_node', folderId);
        })

        $('#Main_Tree').off("move_node.jstree");
        $("#Main_Tree").on('move_node.jstree', this.setCurrentNode.bind(this));

        $('#Main_Tree').off("select_node.jstree");
        $("#Main_Tree").on('select_node.jstree', this.bindCurrentNode.bind(this))
    }

    public bindCurrentNode(e: any, data: any): any {  
        if (this._callBackCount === 0) {
            this._callBackCount++;
            TreeView.prototype.SetRootFolder(data);
        }
        else {
            var id = TreeView.prototype.SetRootFolder(data);
            ZnodeBase.prototype.ShowLoader();
            var displayMode = $("#ViewMode").val();
            if ($('#isUploadPopup') != undefined && $('#isUploadPopup').val() == "true")
                displayMode = "Tile";
            var model: any = { "FolderId": id, "IsMultiSelect": true };

            var _newUrl;
            if (!isMediaPopup) {
                _newUrl = MediaManagerTools.prototype.UpdateQueryString("folderId", id, window.location.href);
                _newUrl = MediaManagerTools.prototype.UpdateQueryString("displayMode", displayMode, _newUrl);
                window.history.pushState({ path: _newUrl }, '', _newUrl);
            }
            else {
                _newUrl = MediaManagerTools.prototype.UpdateQueryString("folderId", id, "/MediaManager/MediaManager/List");
                _newUrl = MediaManagerTools.prototype.UpdateQueryString("displayMode", displayMode, _newUrl);
            }

            $.ajax({
                url: _newUrl,
                data: JSON.stringify(model),
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                success: function (result) {
                    ZnodeBase.prototype.HideLoader();
                    $("#" + $("#hdnFrontObjectName").val()).html(result);
                }
            });
        }
    }

    public setCurrentNode(e: any, data: any): any {

        if (data.parent != "#" && this.IsFolderNameValid(data.node)) {
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.MediaMoveFolder(data.parent, data.node.id, function (data) {
                ZnodeBase.prototype.HideLoader();
                TreeView.prototype.RebindMediaStructureTreeData(data.FolderJsonTree);
                if (data.HasNoError) {
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(data.Message, 'success', isFadeOut, fadeOutTime);
                } else {
                    TreeView.prototype.ReloadTree();
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(data.Message, 'error', isFadeOut, fadeOutTime);
                }
            });
        }
        else {
            this.ReloadTree();
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorSameNameFolder"), data.HasNoError ? 'success' : 'error', isFadeOut, fadeOutTime);
        }
    }

    ShareFolderWithUsers() {
        var accountIds = "";
        var folderId = $("#hdnFolderId").val();
        var accountIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (accountIds.length > 0) {
            this._endpoints.MediaShareFolder(folderId, accountIds, function (res) {
                $("#useraccountlist").hide();
                ZnodeBase.prototype.RemovePopupOverlay();
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.Message, res.HasNoError ? 'success' : 'error', isFadeOut, fadeOutTime);
            });
        }
        else {
            $("#useraccountlist").hide();
            ZnodeBase.prototype.RemovePopupOverlay();
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper("Please select at least one user.", 'error', isFadeOut, fadeOutTime);
        }
    }

    IsFolderNameValid(selectedNode: any): boolean {
        var mediaStructureTree = $('#Main_Tree').jstree(true)
        var siblings = mediaStructureTree.get_children_dom(selectedNode.parent);

        var newNodeId = selectedNode.id;

        var siblingFolderNames = [];
        siblings.find("a.jstree-anchor").each(function () {
            if (this.parentElement.id != newNodeId) {
                siblingFolderNames.push($(this).text());
            }
        })

        if (selectedNode.text != null && selectedNode.text != "") {
            return ($.inArray(selectedNode.text, siblingFolderNames) == -1);
        } else { return false; }
    }

    ReloadTree(): void {
        $('#Main_Tree').jstree("destroy");
        TreeView.prototype.LoadTree();
        TreeView.prototype.BindEvent();
    }

    SetRootFolder(data: any): any {
        var i, j, r = [];
        for (i = 0, j = data.selected.length; i < j; i++) {
            r.push(data.instance.get_node(data.selected[i]).id);
        }
        var id = r[0];
        if (data.instance.get_parent(id) == "#") {
            $("#IsRootFolder").val("true");
        }
        else {
            $("#IsRootFolder").val("false");
        }
        return id;
    }

}
