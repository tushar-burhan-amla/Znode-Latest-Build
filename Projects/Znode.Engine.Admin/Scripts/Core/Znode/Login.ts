class Login extends ZnodeBase {
    _endPoint: Endpoint;

    constructor() {
        super();
    }

    ChangePassword(): any {
        let userName: string = $("#btnUsername").val();
        window.location.href = "/User/ResetAdminPassword?userName=" + userName;
    }

    IsShowChangePasswordPopup(): boolean {
        if ($("#btnUsername").val().toLowerCase() == atob(Constant.defaultAdmin)) {
            Endpoint.prototype.IsShowChangePasswordPopup(function (response) {

                if (response.status)
                    $('#divcssaddpopup').modal('show');
                else
                    $("#frmLogin").submit();
            });
        }
        else
            $("#frmLogin").submit();
        return true;
    }

    SaveInCookie(): void {
        if ($("#Dont-Show-Message").prop("checked")) {
            Endpoint.prototype.SaveInCookie(function (response) {
                $("#frmLogin").submit();
            });
        } else {
            $("#frmLogin").submit();
        }
    }
}

$(document).ready(function () {

    $("#Login").click(function () {
        let userName: string = $("#btnUsername").val();
        let password: string = $("#btnPassword").val();
        if (userName == "" || userName == null
            || typeof userName == "undefined" || password == ""
            || password == null || typeof password == "undefined") {
            if (userName == "" || userName == null || typeof userName == "undefined") {
                $("#btnUsername").addClass("input-validation-error");
                $("#valUserName").text('').text("Username is required.").addClass("field-validation-error").show();
            }
            if (password == "" || password == null || typeof password == "undefined") {
                $("#btnPassword").addClass("input-validation-error");
                $("#valPassword").text('').text("Password is required.").addClass("field-validation-error").show();
            }
            return false;
        }
        else {
            
                Login.prototype.IsShowChangePasswordPopup();
            return false;
        }

    });

    $("#Later").click(function () {
        Login.prototype.SaveInCookie();
    });

    $("#ResetAdminPassword").click(function () {
        Login.prototype.ChangePassword();
    });


});