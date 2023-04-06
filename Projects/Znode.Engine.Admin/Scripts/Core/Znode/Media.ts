interface HTMLInputEvent extends Event {
    target: HTMLInputElement & EventTarget;
}

interface FileReaderEventTarget extends EventTarget {
    result: string
}

interface FileReaderEvent extends ProgressEvent {
    target: FileReaderEventTarget;
    getMessage(): string;
}

class MediaEdit extends ZnodeBase {

    Init() {
    }

    ReplaceMedia() {
        document.getElementById("txtUpload").onchange = function (e?: HTMLInputEvent) {
            let totalFiles: any = e.target.files.length;
            $("#fileuploadstatus").hide();
            $("#fileName").text("");
            $('#fileuploadstatus table tbody').html("");
            $("#isMediaReplace").val("false");

            if (totalFiles > 0) {
                var file = e.target.files[0];              
                var filename = file.name.toLowerCase();
                var filetype = $("#Type").val().toLowerCase();              
                //Validate file type
                if (!filename.match(filetype)) {
                    var tpl = '<tr class="working" id="' + file.name + '"><td><span class="filename">' + file.name + '<span></td><td>' + MediaEdit.prototype.formatFileSize(file.size) + '</td><td class="status">Extension not allowed</td><td></td></tr>';
                    if ($('#fileuploadstatus table tbody').find('tr[id="' + file.name + '"]').length === 0)
                        $('#fileuploadstatus table tbody').append(tpl);
                    $("#fileuploadstatus").show();
                    return false;
                }
                else {
                    $("#isMediaReplace").val("true");
                    MediaEdit.prototype.readImageFile(file);
                    return true;
                }
            }
            return false;
        }
    }

    formatFileSize(bytes: number): any {
        if (typeof bytes !== 'number') {
            return '';
        }

        if (bytes >= 1000000000) {
            return (bytes / 1000000000).toFixed(2) + ' GB';
        }

        if (bytes >= 1000000) {
            return (bytes / 1000000).toFixed(2) + ' MB';
        }
        return (bytes / 1000).toFixed(2) + ' KB';

    }

    readImageFile(file: any): any {
        var reader = new FileReader();
        reader.onload = function (fr: ProgressEvent) {
            $('#impPrev').attr('src', fr.currentTarget["result"]);
            $("#Size").val(MediaEdit.prototype.formatFileSize(file.size));

            var img = new Image();
            img.src = fr.currentTarget["result"];
            img.addEventListener("load", function () {
                $("#Width").val(this.width + " pixels");
                $("#Height").val(this.height + " pixels");
            });
        }
        reader.readAsDataURL(file);
    }

    ValidateImageAndSaveToServer(file: any, totalFiles: number, ): any {
           
        if (($("#frmMediaEdit").valid()) && ($("#isMediaReplace").val() == "true") && (totalFiles > 0)) {
            
            var data = new FormData();
            data.append("file", file);

            var mediaId = $("#MediaId").val();
            var folderId = $("#isMediaReplace").data("val");
            var fileNameold = $("#FileName").val();
       
            CommonHelper.prototype.GetAjaxHeaders(function (response) {
                if (response.Authorization.match("^Authorization: ")) {
                    response.Authorization = response.Authorization.replace('Authorization: ', '');
                }                
                $.ajax({
                    type: "POST",
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader("Authorization", response.Authorization);
                        xhr.setRequestHeader("Znode-UserId", response.ZnodeAccountId);
                        xhr.setRequestHeader("Znode-DomainName", response.DomainName);
                        xhr.setRequestHeader("Token", response.Token);
                    },
                    url: response.ApiUrl + "/apiupload/upload?folderid=" + folderId + "&filetype=" + file.type + "&isMediaReplace=" + $("#isMediaReplace").val() + "&mediaId=" + mediaId + "&filename=" + fileNameold + "",
                    contentType: false,
                    dataType: "json",
                    processData: false,
                    data: data,
                    async: false,
                    success: function (data1) {
                    },
                    error: function (error) {
                        var jsonValue = JSON.parse(error.responseText);
                    }
                });
            })
        }
        return true;
    }
}

