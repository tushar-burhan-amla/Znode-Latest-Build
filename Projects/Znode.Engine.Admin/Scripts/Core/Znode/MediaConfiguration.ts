class MediaConfiguration extends ZnodeBase {
    _endpoint: Endpoint;
    _selectedServer: string;
    _serverId: number;
    _partialViewName: string;
    _flag: boolean;

    constructor() {
        super();
        this._endpoint = new Endpoint();
        this._selectedServer = $("input[name=server]:checked").val();
    }

    Init() {
        MediaConfiguration.prototype.SetDivVisibility();
    }

    DivVisiblity(selectedServer: string): void {
        if (selectedServer == "0") {
            $("#hdnIsLocalServer").val(<any>true);
            $("#hdnIsNetworkDrive").val(<any>false);
            $("#otherServer").hide();
            $("#localurldiv").show();
            $("#networkserver").hide();
            $('#hdnServerName').val($('input[name=server]:checked').data("server"));
            $("#hostsiteurl").val("");
            $("#networkdrivepath").val("");
            Endpoint.prototype.GetLocalServerURL(function (res) {
                $("#localserverurl").val(res.URL);
            });
        }
        else if (selectedServer == "2") {
            $("#hdnIsNetworkDrive").val(<any>true);
            $("#hdnIsLocalServer").val(<any>false);
            $("#otherServer").hide();
            $('#hdnServerName').val($('input[name=server]:checked').data("server"));
            $("#networkserver").show();
        }
        else {
            $("#hdnIsLocalServer").val(<any>false);
            $("#otherServer").show();
            $("#localurldiv").hide();
            $("#networkserver").hide();
            $("#hostsiteurl").val("");
            $("#networkdrivepath").val("");
        }
    }

    DropdownChange(): void {
        this._serverId = $('#ddlServerName').val();
        $('#hdnIsNetworkDrive').val('False');
        $('#hdnIsLocalServer').val('False');
        this._partialViewName = $("#ddlServerName").find("option[value=" + this._serverId + "]").attr("data-partialviewname");
        if (this._partialViewName != undefined && this._partialViewName != null && this._partialViewName != " ") {
            $('#hdnServerName').val($("#ddlServerName").find("option[value=" + this._serverId + "]").text());
            $('#btnMediaConfiguration').prop('disabled', false);

            this.GetMediaSetting(this._partialViewName, this._serverId);
        }
        else {
            $('#partialPlaceHolder').html("");
        }
    };

    GetMediaSetting(partialViewName: string, serverId: number): void {
        this._endpoint.GetMediaSetting(partialViewName, serverId, function (response) {
            $('#partialPlaceHolder').html(response);
        });
    }

    ServerConfigurationValidation(): boolean {
        this._flag = true;
        if ($('#ddlServerName').is(':visible')) {
            var partialViewName: any = $("#ddlServerName option:selected").attr("data-partialviewname");
            if (partialViewName == " ") {
                $("#ddlServerErr").html(ZnodeBase.prototype.getResourceByKeyName("ErrorSelectStorageServer"));
                this._flag = false;
            }
            else {
                $("#ddlServerErr").html("");
            }
        }
        if ($('input[name="AccessKey"]').is(':visible') && $('input[name="SecretKey"]').is(':visible') && $('input[name="BucketName"]').is(':visible')) {
            var accessKey: any = $('input[name="AccessKey"]').val();
            var secretKey: any = $('input[name="SecretKey"]').val();
            var bucketName: any = $('input[name="BucketName"]').val();

            if (accessKey.length < 1) {
                $("#valDisplayAccessKeyErr").html(ZnodeBase.prototype.getResourceByKeyName("RequiredInput"));
                this._flag = false;
            }
            else {
                $("#valDisplayAccessKeyErr").html("");
            }
            if (secretKey.length < 1) {
                $("#valDisplaySecretKeyErr").html(ZnodeBase.prototype.getResourceByKeyName("RequiredInput"));
                this._flag = false;
            }
            else {
                $("#valDisplaySecretKeyErr").html("");
            }
            if (bucketName.length < 1) {
                $("#valDisplayBucketNameErr").html(ZnodeBase.prototype.getResourceByKeyName("RequiredInput"));
                this._flag = false;
            }
            else {
                $("#valDisplayBucketNameErr").html("");
            }
        }
        if ($("input[name=server]:checked").val() == "2") {
            if ($("#hostsiteurl").val() == "") {
                $("#errorRequiredURL").text('').text(ZnodeBase.prototype.getResourceByKeyName("ErrorEnterURL")).addClass("field-validation-error").show();
                $("#hostsiteurl").addClass('input-validation-error');
                this._flag = false;
            }
            if ($("#networkdrivepath").val() == "") {
                $("#errorRequiredNetworkURL").text('').text(ZnodeBase.prototype.getResourceByKeyName("ErrorEnterNetworkURL")).addClass("field-validation-error").show();
                $("#networkdrivepath").addClass('input-validation-error');
                this._flag = false;
            }
        }
        return this._flag;
    }

    startSyncProcess(folderName): void {
        this._endpoint.SyncMedia(folderName, function () { ZnodeNotification.prototype.DisplayNotificationMessagesHelper("Media Synchronized Successfully.", 'success', isFadeOut, fadeOutTime); });
    }


    SetDivVisibility(): void {
        var $radios = $('input:radio[name=server]');
        if ($radios.is(':checked') === false && $("#hdnIsLocalServer").val() === "True") {
            $radios.filter('[value=0]').prop('checked', true);
        }
        else if ($radios.is(':checked') === false && $("#hdnIsNetworkDrive").val() === "True") {
            $radios.filter('[value=2]').prop('checked', true);
        }
        else {
            $radios.filter('[value=1]').prop('checked', true);
        }

        var _serverConfig = new MediaConfiguration();

        _serverConfig.DivVisiblity(_serverConfig._selectedServer);

        if (_serverConfig._selectedServer == "1") {
            $("#ddlServerName").change();
            _serverConfig.ServerConfigurationValidation();
        }

    }

    GenerateImageOnEdit(): boolean {
        Endpoint.prototype.GenerateImageOnEdit($("#Path").val(), $("#FileName").val(), function (res) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, res.status ? "success" : "error", isFadeOut, fadeOutTime);
        });
        return false;
    }

    GenerateImages(): boolean {
        Endpoint.prototype.GenerateImages(function (res) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, res.status ? "success" : "error", isFadeOut, fadeOutTime);
        });
        return false;
    }
}
