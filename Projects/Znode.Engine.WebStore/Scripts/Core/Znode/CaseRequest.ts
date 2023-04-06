class CaseRequest extends ZnodeBase {
    constructor() {
        super();
    }

    Init() {
        CaseRequest.prototype.ValidationForContactUsForm();
        CaseRequest.prototype.ValidationForCustomerFeedbackForm();
    }

    //Set validation for inputs
    ValidationForContactUsForm(): any {
        $("#contact-us").on("click", function () {
            var flag: boolean = true;

            //Set required field for first name
            var firstName: string = $("#valFirstName").val();
            if (firstName.length < 1) {
                $("#valFirstNameErr").html(ZnodeBase.prototype.getResourceByKeyName("RequiredFirstName"));
                flag = false;
            }
            else {
                $("#valFirstNameErr").html("");
            }

            //Set required field for last name
            var lastName: string = $("#valLastName").val();
            if (lastName.length < 1) {
                $("#valLastNameErr").html(ZnodeBase.prototype.getResourceByKeyName("RequiredLastName"));
                flag = false;
            }
            else {
                $("#valLastNameErr").html("");
            }

            //Set required field for comment
            var comment: string = $("#valComment").val();
            if (comment.length < 1) {
                $("#valCommentErr").html(ZnodeBase.prototype.getResourceByKeyName("RequiredComment"));
                flag = false;
            }
            else {
                $("#valCommentErr").html("");
            }

            //Validate phone number
            var phoneNum: string = $("#valPhoneNum").val();
            if (phoneNum.length < 1) {
                $("#valPhoneNumErr").html(ZnodeBase.prototype.getResourceByKeyName("RequiredPhoneNumber"));
                flag = false;
            }

            //Validate email address
            var email: string = $("#valEmail").val();
            if (email.length < 1) {
                $("#valEmailErr").html(ZnodeBase.prototype.getResourceByKeyName("RequiredEmailId"));
                flag = false;
            }
            else {
                $("#valEmailErr").html("");
                var regex = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
                if (!regex.test(email)) {
                    $("#valEmailErr").html(ZnodeBase.prototype.getResourceByKeyName("ErrorEmailAddress"));
                    flag = false;
                }
            }
            return flag;
        });

        //Set Validate Captcha Code for ContactUs form
        $("#formCreateCaseRequest").on("submit", function () {
            var captcha = $("#CaptchaInputText").val();
            $("#valueCaptchaError").html("");
            if (typeof captcha != undefined && captcha != null && captcha != "") {
                $("#contact-us").prop("disabled", true).addClass("disabled");
            }
            $("#contact-captcha").html("");
        });
    }

    //Set validation for Customer Feedback Form
    ValidationForCustomerFeedbackForm(): any {
        $("#customer-feedback").on("click", function () {
            var flag: boolean = true;
            var FirstName: string = $("#FirstName").val();
            if (FirstName.length < 1) {
                $("#valFirstNameErr").html(ZnodeBase.prototype.getResourceByKeyName("RequiredFirstName"));
                flag = false;
            }
            var LastName: string = $("#LastName").val();
            if (LastName.length < 1) {
                $("#valLastNameErr").html(ZnodeBase.prototype.getResourceByKeyName("RequiredLastName"));
                flag = false;
            }
            //Validate email address
            var email: string = $("#valEmailAddress").val();
            if (email.length < 1) {
                $("#valEmailAddressErr").html(ZnodeBase.prototype.getResourceByKeyName("RequiredEmailId"));
                flag = false;
            }
            else {
                $("#valEmailAddressErr").html("");
                var regex = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
                if (!regex.test(email)) {
                    $("#valEmailAddressErr").html(ZnodeBase.prototype.getResourceByKeyName("ErrorEmailAddress"));
                    flag = false;
                }
            }
            return flag;
        });

        //Set Validate Captcha Code for Customer Feedback Form
        $("#formCreateCustomerFeedback").submit(function (event) {
            $("#valueCaptchaError").html("");
        });
    }
}


