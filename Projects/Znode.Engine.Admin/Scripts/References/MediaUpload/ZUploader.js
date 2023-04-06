'use strict';
(function (document, window, index) {
    // feature detection for drag&drop upload
    var isAdvancedUpload = function () {
        var div = document.createElement('div');
        return (('draggable' in div) || ('ondragstart' in div && 'ondrop' in div)) && 'FormData' in window && 'FileReader' in window;
    }();

   
    // applying the effect for every form
    var forms = document.querySelectorAll('.box');
    Array.prototype.forEach.call(forms, function (form) {
        var input = form.querySelector('input[type="file"]'),
            label = form.querySelector('label'),
            errorMsg = form.querySelector('.box__error span'),
            restart = form.querySelectorAll('.box__restart'),
            droppedFiles = false,
            IsReplace = false,

            uploadFiles = function (files) {
                var SelectedFolderId = $("#Main_Tree").find('li[aria-selected="true"]').attr('id');
                $("#divErrorMessage").hide();
                $("#messageBoxMediaManagerContainerId").html("");
                if (SelectedFolderId === undefined || SelectedFolderId === "") {
                    $("#divErrorMessage").show();
                    $("#messageBoxMediaManagerContainerId").html(ZnodeBase.prototype.getResourceByKeyName("ErrorSelectFoldertouploadfileError"));
                    return false;
                }
                if (files.length > 0) {
                    var data = new FormData();
                    for (var i = 0; i < files.length; i++) {
                        if (!isValidFileName(files[i].name)) {
                            $('#MediaInvalidFileName .modal-body').text('');
                            $('#MediaInvalidFileName .modal-body').text(ZnodeBase.prototype.getResourceByKeyName("MediaInvalidFileName") +
                                " < > # % + { } | \ ^ ~ [ ] `");
                            $('#MediaInvalidFileName').modal({ backdrop: 'static', keyboard: false });
                        }
                        else {

                            var fileType = files[i].type;
                            var tpl = '<tr class="working" id="' + files[i].name + '"><td><span class="filename">' + files[i].name + '<span></td><td>' + formatFileSize(files[i].size) + '</td><td class="status">Waiting...</td><td></td></tr>';
                            if ($('#fileuploadstatus table tbody').find('tr[id="' + files[i].name + '"]').length === 0)
                                $('#fileuploadstatus table tbody').append(tpl)

                            if (files[i].name.replace(/\.[^/.]+$/, "").length < 146)
                                data.append("file" + i, files[i]);
                            else {
                                $('#fileuploadstatus table tbody').find("tr[id='" + files[i].name + "']").find("td:eq(2)").html("File name should not contain more than 145 characters");
                                $('#fileuploadstatus table tbody').find("tr[id='" + files[i].name + "']").find("td:eq(3)").html();
                            }
                            $("#fileuploadstatus").show();
                        }
                    }

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
                            url: response.ApiUrl + "/apiupload/upload?folderid=" + SelectedFolderId + "&filetype=" + fileType + "&isreplace=" + IsReplace + "",
                            contentType: false,
                            dataType: "json",
                            processData: false,
                            data: data,
                            success: function (data) {
                                ShowStatus(data);
                                IsReplace = false;
                            },
                            error: function (error) {
                                var jsonValue = JSON.parse(error.responseText);
                            }
                        });
                    })
                }
            },
            ShowStatus = function (data) {
                $("#divErrorMessage").hide();
                $("#messageBoxMediaManagerContainerId").html("");
                for (var i = 0; i < data.length; i++) {
                    $('#fileuploadstatus table tbody').find("tr[id='" + data[i].FileName + "']").find("td:eq(2)").html(StatusCode(data[i].StatusCode));
                    $('#fileuploadstatus table tbody').find("tr[id='" + data[i].FileName + "']").find("td:eq(3)").html(StatusAction(data[i].StatusCode, data[i].MediaId));
                    if (data[i].StatusCode === 20) {
                        $("#divErrorMessage").show();
                        $("#messageBoxMediaManagerContainerId").html(ZnodeBase.prototype.getResourceByKeyName("SomeImagesAlreadyExist"));
                    }
                    if (data[i].StatusCode == 100) {
                        $("#divErrorMessage").show();
                        $("#messageBoxMediaManagerContainerId").html(ZnodeBase.prototype.getResourceByKeyName("ErrorWebPSupport"));
                    }
                }
                StatusActionEvent();

                if ($("#appendDiv").length > 0) {
                    $("#appendDiv").show();
                }
                $("#Main_Tree").find('li[aria-selected="true"] a:eq(0)').click();
            },
            StatusCode = function (statusCode) {
                switch (statusCode) {
                    case 10:
                        return "Extension Not Allowed";
                    case 20:
                        return "File Already Exist";
                    case 30:
                        return "Error- Exceeds size limit";
                    case 40:
                        return "Error-Corrupt";
                    case 50:
                        return "Error";
                    case 60:
                        return "Done";
                    case 70:
                        return "Removed";
                    case 100:
                        return "Animated .webp Images are not Supported";
                }
            },
            StatusAction = function (statusCode, mediaId) {
                switch (statusCode) {
                    case 10:
                        return "";
                    case 20:
                        return "<a href='javascript:void(0)' data-actionName='replace' data-mediaId='" + mediaId + "'>Replace</a>";
                    case 30:
                        return "";
                    case 40:
                        return "Corrupt";
                    case 50:
                        return "Error";
                    case 60:
                        return "<a href='javascript:void(0)' class='cancel' data-actionName='delete' data-mediaId='" + mediaId + "'><i class='z-delete'></i></a>";
                    case 70:
                        return "";
                    case 100:
                        return "";
                }
            },
            triggerFormSubmit = function () {
                var event = document.createEvent('HTMLEvents');
                event.initEvent('submit', true, false);
                form.dispatchEvent(event);
            },
            formatFileSize = function (bytes) {
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
            },
            StatusActionEvent = function () {
                $('#fileuploadstatus table tbody tr').find('td').find('a[data-actionname]').unbind("click");
                $('#fileuploadstatus table tbody tr').find('td').find('a[data-actionname]').click(function () {
                    var folderId = $("#Main_Tree").find('li[aria-selected="true"]').attr('id');
                    var actionName = $(this).data('actionname');
                    var mediaId = $(this).data('mediaid');
                    var fileName = $(this).closest('tr').attr('id');
                    if (actionName === "replace")
                        ReplaceFile(mediaId, fileName);
                    else
                        DeleteFile(mediaId, folderId, fileName);
                });
            },
            ReplaceFile = function (mediaId, fileName) {
                $('#fileuploadstatus table tbody').find("tr[id='" + fileName + "']").find("td:eq(2)").html("Waiting...");
                IsReplace = true;
                var files = input.files;
                var fileData = [];
                for (var i = 0; i < files.length; i++) {
                    if (files[i].name === fileName)
                        fileData.push(files[i])
                }
                uploadFiles(fileData);
            },
            DeleteFile = function (mediaId, folderid, fileName) {
                var model = { FolderId: folderid, MediaIds: mediaId };

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
                        url: response.ApiUrl + "/apiupload/remove",
                        contentType: "application/json",
                        dataType: "json",
                        data: JSON.stringify(model),
                        success: function (data) {
                            data[0].FileName = fileName;
                            ShowStatus(data);
                            $("#file").val('');
                        },
                        error: function (error) {
                            var jsonValue = JSON.parse(error.responseText);
                        }
                    });
                })
            };
        // letting the server side to know we are going to make an Ajax request
        var ajaxFlag = document.createElement('input');
        ajaxFlag.setAttribute('type', 'hidden');
        ajaxFlag.setAttribute('name', 'ajax');
        ajaxFlag.setAttribute('value', 1);
        form.appendChild(ajaxFlag);

        // automatically submit the form on file select
        input.addEventListener('change', function (e) {
            uploadFiles(e.target.files);
        });

        // drag&drop files if the feature is available
        if (isAdvancedUpload) {
            form.classList.add('has-advanced-upload'); // letting the CSS part to know drag&drop is supported by the browser

            ['drag', 'dragstart', 'dragend', 'dragover', 'dragenter', 'dragleave', 'drop'].forEach(function (event) {
                form.addEventListener(event, function (e) {
                    // preventing the unwanted behaviours
                    e.preventDefault();
                    e.stopPropagation();
                });
            });
            ['dragover', 'dragenter'].forEach(function (event) {
                form.addEventListener(event, function () {
                    form.classList.add('is-dragover');
                });
            });
            ['dragleave', 'dragend', 'drop'].forEach(function (event) {
                form.addEventListener(event, function () {
                    form.classList.remove('is-dragover');
                });
            });
            form.addEventListener('drop', function (e) {
                droppedFiles = e.dataTransfer.files; // the files that were dropped
                uploadFiles(droppedFiles);
            });
        }


        // if the form was submitted
        form.addEventListener('submit', function (e) {
            // preventing the duplicate submissions if the current one is in progress
            if (form.classList.contains('is-uploading')) return false;

            form.classList.add('is-uploading');
            form.classList.remove('is-error');

            if (isAdvancedUpload) // ajax file upload for modern browsers
            {
                e.preventDefault();

                // gathering the form data
                var ajaxData = new FormData(form);
                if (droppedFiles) {
                    Array.prototype.forEach.call(droppedFiles, function (file) {
                        ajaxData.append(input.getAttribute('name'), file);
                    });
                }

                // ajax request
                var ajax = new XMLHttpRequest();
                ajax.open(form.getAttribute('method'), form.getAttribute('action'), true);

                ajax.onload = function () {
                    form.classList.remove('is-uploading');
                    if (ajax.status >= 200 && ajax.status < 400) {
                        var data = JSON.parse(ajax.responseText);
                        form.classList.add(data.success === true ? 'is-success' : 'is-error');
                        if (!data.success) errorMsg.textContent = data.error;
                    }
                    else
                        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorContactWebMaster"), 'error', isFadeOut, fadeOutTime);
                };

                ajax.onerror = function () {
                    form.classList.remove('is-uploading');
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorTryAgain"), 'error', isFadeOut, fadeOutTime);
                };

                ajax.send(ajaxData);
            }
            else // fallback Ajax solution upload for older browsers
            {
                var iframeName = 'uploadiframe' + new Date().getTime(),
                    iframe = document.createElement('iframe');

                $iframe = $('<iframe name="' + iframeName + '" style="display: none;"></iframe>');

                iframe.setAttribute('name', iframeName);
                iframe.style.display = 'none';

                document.body.appendChild(iframe);
                form.setAttribute('target', iframeName);

                iframe.addEventListener('load', function () {
                    var data = JSON.parse(iframe.contentDocument.body.innerHTML);
                    form.classList.remove('is-uploading')
                    form.classList.add(data.success === true ? 'is-success' : 'is-error')
                    form.removeAttribute('target');
                    if (!data.success) errorMsg.textContent = data.error;
                    iframe.parentNode.removeChild(iframe);
                });
            }
        });


        // restart the form if has a state of error/success
        Array.prototype.forEach.call(restart, function (entry) {
            entry.addEventListener('click', function (e) {
                e.preventDefault();
                form.classList.remove('is-error', 'is-success');
                input.click();
            });
        });

        // Firefox focus bug fix for file input
        input.addEventListener('focus', function () { input.classList.add('has-focus'); });
        input.addEventListener('blur', function () { input.classList.remove('has-focus'); });

    });

    function isValidFileName(fileName) {
        return !/[~`#%\^+=\[\]\\';,/{}|\\"<>\?]/g.test(fileName);
    }
}(document, window, 0));
