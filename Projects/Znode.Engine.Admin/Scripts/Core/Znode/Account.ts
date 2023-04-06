class Account extends ZnodeBase {
    _endPoint: Endpoint;
    isCatalogValid: boolean;
    catalogId: number;
    isTemplateValid: boolean;
    constructor() {
        super();
        this._endPoint = new Endpoint();
    }

    Init() {
        $("#ddlUserType").off("change");
        $("#ddlUserType").on("change", function () {
            $("#hdnRoleName").val($("#ddlUserType option:selected").text());
            Account.prototype.ShowHidePermissionDiv();
        });
        $("#ddlUserType").change();
        Account.prototype.ShowHidePermissionDiv();
        Account.prototype.ValidateAccountsCustomer();
        Order.prototype.Init();
        GiftCard.prototype.GetActiveCurrencyToStore("");
        Account.prototype.EditAddress();
        Account.prototype.EditChildAccount();
        Account.prototype.EditAccountCustomerList();
        Account.prototype.BindStates();
        Account.prototype.AutoCompleteApprovalUsers();
        //Account.prototype.AutocompleteCatalog();
    }

    ValidateAccountsCustomer(): any {
        $("#frmCreateEditCustomerAccount").submit(function () {
            return Account.prototype.ValidationForUser();
        });
    }

    ValidateAccountPermissionName(): any {
        $("#AccountPermissionName").on("blur", function () {
            $("#loading-div-background").show();
            Account.prototype.ValidateExistAccountPermissionName();
            $("#loading-div-background").hide();
        });
    }

    ValidateExistAccountPermissionName(): boolean {
        var isValid = true;
        if ($("#AccountPermissionName").val() != '') {
            Endpoint.prototype.IsAccountPermissionExist($("#AccountPermissionName").val(), $("#AccountId").val(), $("#AccountPermissionId").val(), function (response) {
                if (!response) {
                    $("#AccountPermissionName").addClass("input-validation-error");
                    $("#errorSpanAccountPermissionName").addClass("error-msg");
                    $("#errorSpanAccountPermissionName").text(ZnodeBase.prototype.getResourceByKeyName("AlreadyExistAccountPermissionName"));
                    $("#errorSpanAccountPermissionName").show();
                    isValid = false;
                    $("#loading-div-background").hide();
                }
            });
        }
        return isValid;
    }

    SubmitCustomerCreateEditForm() {
            return Account.prototype.ValidationForUser();
    }

    DeleteMultipleDepartments(control): any {
        var departmentId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (departmentId.length > 0) {
            Endpoint.prototype.DeleteMultipleDepartments(departmentId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    DeleteMultipleNotes(control): any {
        var noteId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (noteId.length > 0) {
            Endpoint.prototype.DeleteMultipleNotes(noteId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    DeleteMultipleAccount(control): any {
        var accountId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (accountId.length > 0) {
            Endpoint.prototype.DeleteMultipleAccount(accountId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    ShowHidePermissionDiv(): any {
        $("#permissionsToHide").show();
    }

    DeleteMultipleAccountCustomer(control): any {
        var accountIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (accountIds.length > 0) {
            Endpoint.prototype.DeleteAccountCustomers(accountIds, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    UnAssociatePriceList(control): any {
        var priceListId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (priceListId.length > 0) {
            Endpoint.prototype.UnAssociatePriceListFromAccount(priceListId, $('#AccountId').val(), function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    EditAssociatedPriceListPrecedence(): any {
        $("#grid tbody tr td").find(".z-edit").on("click", function (e) {
            e.preventDefault();
            var priceListId = parseInt(decodeURIComponent($(this).attr("data-parameter")).split('&')[0].split('=')[1]);
            var listName = decodeURIComponent($(this).attr("data-parameter")).split('&')[1].split('=')[1];
            Endpoint.prototype.EditAssociatedPriceListPrecedenceForAccount(priceListId, $('#AccountId').val(), listName, function (res) {
                $("#priceListPrecedence").modal("show");
                $("#priceListPrecedence").html(res);
            });
        });
    }

    EditAssociatedPriceListPrecedenceResult(data: any) {
        $("#priceListPrecedence").modal("hide");
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(data.Message, "success", isFadeOut, fadeOutTime);
        ZnodeBase.prototype.RemovePopupOverlay();

        if ($('#AccountId').val() > 0)
            Account.prototype.AssociatedPriceList();
    }

    AssociatedPriceList() {
        Endpoint.prototype.GetAssociatedPriceListForAccount($("#AccountId").val(), function (response) {
            $("#AssociatedPriceListToAccount").html('');
            $("#AssociatedPriceListToAccount").html(response);
            GridPager.prototype.UpdateHandler();
        });
    }

    AssociatePriceListToAccount() {
        var priceListId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (priceListId.length > 0) {
            Endpoint.prototype.AssociatePriceListForAccount($("#AccountId").val(), priceListId, function (res) {
                $("#DivGetUnAssociatedPriceListForAccount").hide(700);
                window.location.href = window.location.protocol + "//" + window.location.host + "/Account/GetAssociatedPriceListForAccount?accountId=" + $("#AccountId").val();
            });
        }
        else {
            $('#associatedPriceListId').show();
        }
    }

    ValidateForStore(): boolean {
        let isValid: boolean = true;
        if ($("#txtPortalName").is(':visible')) {
            if ($("#txtPortalName").val() == "") {
                $("#errorRequiredStore").text('').text(ZnodeBase.prototype.getResourceByKeyName("ErrorSelectPortal")).addClass("field-validation-error").show();
                $("#txtPortalName").parent("div").addClass('input-validation-error');
                isValid= false;
            }
        }
        if ($("#RadioSpecific").is(':checked') == true) {
            if ($("#txtCatalogName").val() == "") {
                $("#errorRequiredCatalog").text('').text(ZnodeBase.prototype.getResourceByKeyName("ErrorSelectCatalog")).addClass("field-validation-error").show();
                $("#txtCatalogName").parent("div").addClass('input-validation-error');
                isValid= false;
            }
        }

        return isValid;
    }

    ValidationForUser() {
        var flag: boolean = true;
        var _AllowGlobalLevelUserCreation = $("#AllowGlobalLevelUserCreation").val();
        if (_AllowGlobalLevelUserCreation == "False" && $("#AccountName").val() == "" && $("#hdnPortalId").val() == "" && $("#StoreName").val() == "") {
            $("#errorRequiredStore").text('').text(ZnodeBase.prototype.getResourceByKeyName("SelectPortal")).addClass("field-validation-error").show();
            $("#txtPortalName").parent("div").addClass('input-validation-error');
            flag = false;
        }
        if ($("#ddlUserType").is(":visible") && $("#ddlUserType").val() == "") {
            $("#valRoleName").text('').text(ZnodeBase.prototype.getResourceByKeyName("ErrorRoleNameRequired")).addClass("field-validation-error").show();
            $("#ddlUserType").addClass('input-validation-error');
            flag = false;
        }

        if ($("#hdnRoleName").val() == "User") {
            if ($("#BudgetAmount").is(':visible')) {
                if ($("#BudgetAmount").val() == null || $("#BudgetAmount").val() == "") {
                    $("#errorRequiredAccountPermissionAccessId").text('').text(ZnodeBase.prototype.getResourceByKeyName("ErrorBudgetAmount")).addClass("field-validation-error").show();
                    $("#BudgetAmount").addClass('input-validation-error');
                    flag = false;
                }
            }
            if ($("#ddlApproverList").is(':visible')) {
                if ($("#ddlApproverList").val() == null || $("#ddlApproverList").val() == "") {
                    $("#errorRequiredApprovalUserId").html("<span>" + ZnodeBase.prototype.getResourceByKeyName("SelectApprovalUserId") + "</span>");
                    $("#ddlApproverList").addClass('input-validation-error');
                    flag = false;
                }
            }
        }
        if (!$("#BudgetAmount").is(':visible')) {
            $("#BudgetAmount").val("");
        }
        if ($("#Email").is(':visible') && $("#Email").val() == '') {
            $("#errorRequiredEmail").text('').text(ZnodeBase.prototype.getResourceByKeyName("EmailAddressIsRequired")).removeClass('field-validation-valid').addClass("field-validation-error").show();
            $("#Email").removeClass('valid').addClass('input-validation-error');
            flag = false;
        }
        if ($("#Email").is(':visible') && $("#Email").val().trim() != '' && $("#Email").val().indexOf(",") > -1) {
            $("#errorRequiredEmail").text('').text(ZnodeBase.prototype.getResourceByKeyName("ErrorEmailAddress")).removeClass('field-validation-valid').addClass("field-validation-error").show();
            $("#Email").removeClass('valid').addClass('input-validation-error');
            flag = false;
        }

        if ($("#UserName").val() != undefined && $("#UserName").val().trim() != '' && $("#UserName").val().indexOf(",") > -1) {            
            $("#errorSingleEmail").text('').text(ZnodeBase.prototype.getResourceByKeyName("ErrorEmailAddress")).removeClass('field-validation-valid').addClass("field-validation-error").show();
            $("#UserName").removeClass('valid').addClass('input-validation-error');
            
            flag = false;
        }

        return flag;
    }

    ParentAccountChange() {
        var parentAccountId = $("#CompanyAccount_ParentAccountId").val();
        if (parentAccountId > 0) {
            Endpoint.prototype.GetAccountsPortal(parentAccountId, function (res) {
                $("#hdnPortalId").val(res.portalId);
                $("#txtPortalName").val(res.storeName);
                $("#hdnStoreName").val(res.storeName);
            });
        }
    }

    EnableCustomerAccount(): any {
        var accountIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (accountIds.length > 0) {
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.CustomerEnableDisableAccount($("#AccountId").val(), accountIds, true, function (res) {
                ZnodeBase.prototype.HideLoader();
                DynamicGrid.prototype.RefreshGridOndelete($("#ZnodeAccountsCustomer").find("#refreshGrid"), res);
                if (res.status == true)
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("UnlockMessage"), 'success', isFadeOut, fadeOutTime);
                else
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, 'error', isFadeOut, fadeOutTime);
            });
        }
        else {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectAtleastOneRecord"), 'error', isFadeOut, fadeOutTime);
        }
    }

    DisableCustomerAccount(): any {
        var accountIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (accountIds.length > 0) {
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.CustomerEnableDisableAccount($("#AccountId").val(), accountIds, false, function (res) {
                ZnodeBase.prototype.HideLoader();
                DynamicGrid.prototype.RefreshGridOndelete($("#ZnodeAccountsCustomer").find("#refreshGrid"), res);
                if (res.status == true)
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("LockMessage"), 'success', isFadeOut, fadeOutTime);
                else
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, 'error', isFadeOut, fadeOutTime);
            });
        }
        else {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectAtleastOneRecord"), 'error', isFadeOut, fadeOutTime);
        }
    }

    EnableDisableAccount(): any {
        var accountIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (accountIds.length > 0) {
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.CustomerEnableDisableAccount($("#AccountId").val(), accountIds, false, function (res) {
                ZnodeBase.prototype.HideLoader();
                DynamicGrid.prototype.RefreshGridOndelete($("#ZnodeAccountsCustomer").find("#refreshGrid"), res);
                if (res.status == true)
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("DisableMessage"), 'success', isFadeOut, fadeOutTime);
                else
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, 'error', isFadeOut, fadeOutTime);
            });
        }
        else {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectAtleastOneRecord"), 'error', isFadeOut, fadeOutTime);
        }
    }

    CustomerResetPassword(): any {
        var accountIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (accountIds.length > 0) {
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.CustomerAccountResetPassword($("#AccountId").val(), accountIds, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete($("#ZnodeAccountsCustomer").find("#refreshGrid"), res);
                ZnodeBase.prototype.HideLoader();
                if (res.status == true)
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SuccessResetPassword"), 'success', isFadeOut, fadeOutTime);
                else
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName(res.message), 'error', isFadeOut, fadeOutTime);
            });
        }
        else {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectAtleastOneRecord"), 'error', isFadeOut, fadeOutTime);
        }
    }

    UpdateQuoteStatus(control, statusId): any {
        var quoteIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (quoteIds.length > 0 && statusId > 0) {
            Endpoint.prototype.UpdateQuoteStatus(quoteIds, statusId, false, function (res) {
                DynamicGrid.prototype.RefreshGrid(control, res);
                DynamicGrid.prototype.ClearCheckboxArray();
            });
        }
        else {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectAtleastOneRecord"), 'error', isFadeOut, fadeOutTime);
        }
    }

    SetUpdateQuoteUrl() {
        var updatePageType: string = $("#UpdatePageType").val();

        $("#grid tbody tr").each(function () {
            var newhref: string;
            var quoteIdLink: string;
            var orderStatus: string = $(this).find('.z-view').attr("data-parameter").split('&')[1].split('=')[1];

            if ((orderStatus.toUpperCase() == "ORDERED")) {
                var omsQuoteId: number = parseInt($(this).find('.z-view').attr("data-parameter").split('&')[0].split('=')[1]);
                var accountId = $('#UserAccountId').val();

                //New href for z-view.
                newhref = "/Order/Manage?OmsOrderId=" + omsQuoteId + "&accountId=" + accountId + "&updatePageType=" + updatePageType;

                //New href for quoteId link.
                quoteIdLink = "/Order/Manage?OmsOrderId=" + omsQuoteId + "&accountId=" + accountId + "&updatePageType=" + updatePageType;
            }
            else {
                //Append updatePageType to existing href of z-view.
                newhref = $(this).find('.z-view').attr('href') + "&updatePageType=" + updatePageType;

                //Append updatePageType to existing href of quoteId link.
                quoteIdLink = $(this).find('td').eq(1).find('a').attr('href') + "&updatePageType=" + updatePageType;
            }
            $(this).find('.z-view').attr('href', newhref);
            $(this).find('td').eq(1).find('a').attr('href', quoteIdLink);
        });
    }

    DeleteAssociatedProfiles(accountId, control): any {
        var accountProfileIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (accountProfileIds.length > 0) {
            Endpoint.prototype.DeleteAssociatedProfilesForAccounts(accountProfileIds, accountId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    AssociateProfileToAccount(accountId) {
        var profileId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (profileId.length > 0) {
            Endpoint.prototype.AssociateProfileForAccount(accountId, profileId, function (res) {
                $("#DivGetUnAssociatedProfileForAccount").hide(700);
                window.location.href = window.location.protocol + "//" + window.location.host + "/Account/GetAssociatedProfileForAccount?accountId=" + accountId;
            });
        }
        else {
            $('#associatedProfileId').show();
        }
    }

    ValidateOrderStatus(href) {
        if (href.toLowerCase().indexOf("ordered") == -1) {
            $("#hdnOrderURL").val(href);
            $("#btnConvertToOrder").click();
        }
        else
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("OrderAlreadyPlacedForQuote"), 'error', isFadeOut, fadeOutTime);
    }

    ValidateDepartmentNameField(object): any {
        var isValid = true;
        if ($(object).val() == '') {
            $(object).addClass("input-validation-error");
            if ($(object).val() == '')
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("DepartmentNameIsRequired"), 'error', isFadeOut, fadeOutTime);

            isValid = false;
        }
        else {
            $(object).remove("input-validation-error");
            $(object).removeClass("input-validation-error");
            isValid = true;
        }
        return isValid;
    }

    ValidatePrecedanceField(object): any {
        var regex = new RegExp('^\\d{0,}?$');
        var isValid = true;
        if (isNaN($(object).val())) {
            $(object).addClass("input-validation-error");
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("RequiredNumericValue"), 'error', isFadeOut, fadeOutTime);
            isValid = false;
        }
        else if (!regex.test($(object).val()) || $(object).val() == 0) {
            $(object).addClass("input-validation-error");
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("DisplayOrderRange"), 'error', isFadeOut, fadeOutTime);
            isValid = false;
        }
        else if ($(object).val() == '') {
            $(object).addClass("input-validation-error");
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("PrecedenceIsRequired"), 'error', isFadeOut, fadeOutTime);
            isValid = false;
        }
        else {
            $(object).remove("input-validation-error");
            $(object).removeClass("input-validation-error");
            isValid = true;
        }
        return isValid;
    }

    SetDefaultProfile(): any {
        var accountProfileId: string[] = MediaManagerTools.prototype.unique();
        var accountId: number = $('#HdnAccountId').val();
        var profileId: number = parseInt(Store.prototype.GetMultipleValuesOfGridColumn("Profile ID"));

        if (accountProfileId.length == 0)
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectAtleastOneRecord"), 'error', isFadeOut, fadeOutTime);
        else if (accountProfileId.length > 1)
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectAnyOneToSetAsDefault"), 'error', isFadeOut, fadeOutTime);
        else {
            Endpoint.prototype.SetAccountDefaultProfile(accountId, parseInt(accountProfileId.toString()), profileId, function (res) {
                window.location.href = window.location.protocol + "//" + window.location.host + "/Account/GetAssociatedProfileForAccount?accountId=" + accountId;
            });
        }
    }

    RemoveOrderGridIcon(): any {
        Order.prototype.HideGridColumn('IsInRMA');
        $('#grid tbody tr').each(function () {
            if ($(this).find('td.paymentStatus').html().trim().toLocaleLowerCase() == 'cc_captured' || $(this).find('td.paymentStatus').html().trim().toLocaleLowerCase() == 'cc_voided' || $(this).find('td.IsInRMA').find('i').hasClass('z-active')) {
                $(this).find('.z-edit').parents('li').remove();
            }
        });
        $("#listcontainerId").show();
    }

    //This method is used to get portal list on aside panel
    GetPortalList(): any {
        if ($('#ZnodeUserAccountList').length == 1)
            $('#ZnodeUserAccountList').html("");
        ZnodeBase.prototype.BrowseAsidePoupPanel('/Account/GetPortalList', 'divStoreListAsidePanel');
    }

    //To Do: To bind portal information
    OnSelectPortalResult(item: any): any {
        if (item != undefined) {                     
            $('#ddlPortal').val(item.Id);           
            Store.prototype.OnSelectStoreAutocompleteDataBind(item);
            Account.prototype.BindParentAccountDetailsBasedOnPortalId();
            Account.prototype.BindCountryBasedOnPortalId();
        }
    }

    //This method is used to select portal from list and show it on textbox
    GetPortalDetail(): void {
        $("#grid").find("tr").on("click", function () {
            let portalName: string = $(this).find("td[class='storecolumn']").text();
            let portalId: string = $(this).find("td")[0].innerHTML;
            $('#txtPortalName').val(portalName);
            $('#hdnPortalId').val(portalId);
            $('#PortalId').val(portalId);
            $('#ddlPortal').val(portalId);
            $("#errorRequiredStore").text("").removeClass("field-validation-error").hide();
            $("#txtPortalName").removeClass('input-validation-error');
            Account.prototype.BindParentAccountBasedOnPortalId();
            Account.prototype.BindCountryBasedOnPortalId();
            $('#divStoreListAsidePanel').hide(700);
            ZnodeBase.prototype.RemovePopupOverlay();
        });
    }

    BindParentAccountBasedOnPortalId(): any {
        $("span[data-valmsg-for='PortalId']").html("");
        var portalId = $("#hdnPortalId").val();
        if (portalId == 0 && portalId == "") {
            $('#CompanyAccount_ParentAccountId').children('option').remove();
            return false;
        }
        Endpoint.prototype.GetParentAccountList(portalId, function (response) {
            $('#CompanyAccount_ParentAccountId').children('option').remove();
            for (var i = 0; i < response.length; i++) {
                var opt = new Option(response[i].Text, response[i].Value);
                $('#CompanyAccount_ParentAccountId').append(opt);
            }
        });
    }

    BindCountryBasedOnPortalId(): any {
        $("span[data-valmsg-for='PortalId']").html("");
        var portalId = $("#hdnPortalId").val();
        if (portalId == 0 && portalId == "") {
            $('#CompanyAccount_Address_CountryName').children('option').remove();
            return false;
        }
        var accountAccess = $("#IsAccessAccount").val();
        if (accountAccess == "true") {
            Endpoint.prototype.GetCountriesByPortalId(portalId, function (response) {
                Account.prototype.GetCountries(response)
            });
        }
        else {
            Endpoint.prototype.GetCountriesByPortalIdWithountAccountAccess(portalId, function (response) {
                Account.prototype.GetCountries(response)
            });
        }
    }

    GetCountries(response): void {
        $('#CompanyAccount_Address_CountryName').children('option').remove();
        for (var i = 0; i < response.length; i++) {
            var opt = new Option(response[i].Text, response[i].Value);
            $('#CompanyAccount_Address_CountryName').append(opt);
        }

        if ($('#CompanyAccount_Address_CountryName').val() != undefined) {
            Account.prototype.BindStates();
        }
    }

    EditAddress(): void {
        $("#AccountAddressList #grid tbody tr td").find(".z-edit").on("click", function (e) {
            e.preventDefault();
            $("#divAddCustomerAddress").html("");
            var href = $(this).attr('href');
            ZnodeBase.prototype.BrowseAsidePoupPanel(href, 'divAddCustomerAddress');
        });
    }

    EditChildAccount(): void {
        $("#subAccountListId #grid tbody tr td").find(".z-edit").on("click", function (e) {
            e.preventDefault();
            $("#divAddCustomerAddress").html("");
            $("#divAddSubAccountPanel").html("");
            var href = $(this).attr('href');
            ZnodeBase.prototype.BrowseAsidePoupPanel(href, 'divAddSubAccountPanel');
        });
    }

    EditAccountCustomerList(): void {
        $("#ZnodeAccountsCustomer #grid tbody tr td").find(".z-edit").on("click", function (e) {
            e.preventDefault();
            $("#divAddCustomerAddress").html("");
            $("#divAddSubAccountPanel").html("");
            $("#divAddCustomerAsidePanel").html("");
            var href = $(this).attr('href');
            ZnodeBase.prototype.BrowseAsidePoupPanel(href, 'divAddCustomerAsidePanel');
        });
    }

    CreateChildAccount(): any {
        $("#divAddSubAccountPanel").html("");
        ZnodeBase.prototype.BrowseAsidePoupPanel('/Account/CreateSubAccount?parentAccountId=' + $('#AccountId').val(), 'divAddSubAccountPanel');
    }

    ValidateUserNameExists(): boolean {
        var isSubmit: boolean = true;
        if ($("#divAddCustomerAsidePanel #UserName").val() != '') {
            Endpoint.prototype.IsUserNameExist($("#divAddCustomerAsidePanel #UserName").val(), $("#PortalId").val(), function (response) {
                if (!response) {
                    $("#UserName").addClass("input-validation-error");
                    $("#errorUserName").addClass("error-msg");
                    $("#errorUserName").text(ZnodeBase.prototype.getResourceByKeyName("AlreadyExistUserName"));
                    $("#errorUserName").show();
                    $("#loading-div-background").hide();
                    isSubmit = false;
                }
            });
        }
        else
        {
            $("#divAddCustomerAsidePanel #UserName").addClass("input-validation-error");
            $("#errorUserName").addClass("error-msg");
            $("#errorUserName").text(ZnodeBase.prototype.getResourceByKeyName("ErrorUsernameRequired"));
            $("#errorUserName").show();
        }
        
        if ($("#divAddCustomerAsidePanel #FirstName").val() == '' || $("#divAddCustomerAsidePanel #FirstName").val() == undefined)
        {
            $("#divAddCustomerAsidePanel #FirstName").addClass("input-validation-error");
            $("#errorFirstName").addClass("error-msg");
            $("#errorFirstName").text(ZnodeBase.prototype.getResourceByKeyName("ErrorFirstNameRequired"));
            $("#errorFirstName").show();
        }

        if ($("#divAddCustomerAsidePanel #LastName").val() == '' || $("#divAddCustomerAsidePanel #LastName").val() == undefined)
        {
            $("#divAddCustomerAsidePanel #LastName").addClass("input-validation-error");
            $("#errorLastName").addClass("error-msg");
            $("#errorLastName").text(ZnodeBase.prototype.getResourceByKeyName("ErrorLastNameRequired"));
            $("#errorLastName").show();
        }

        if ($("#ddlUserType").is(":visible") && $("#ddlUserType").val() == "") {
            $("#valRoleName").text('').text(ZnodeBase.prototype.getResourceByKeyName("ErrorRoleNameRequired")).addClass("field-validation-error").show();
            $("#ddlUserType").addClass('input-validation-error');
            return false;
        }
        if (Account.prototype.ValidateBudgetAmount() && isSubmit) $("#frmCreateEditCustomerAccount").submit(); else return isSubmit;
    }

    ValidateBudgetAmount(): boolean {
        if ($("#BudgetAmount").is(':visible')) {
            if ($("#BudgetAmount").val() == null || $("#BudgetAmount").val() == "") {
                $("#errorRequiredAccountPermissionAccessId").text('').text(ZnodeBase.prototype.getResourceByKeyName("ErrorBudgetAmount")).addClass("field-validation-error").show();
                $("#BudgetAmount").addClass('input-validation-error');
                return false;
            }
        }
        return true;
    }

    ValidateChildAccountExists(): boolean {
        var isValid = true;
        if ($("#divAddSubAccountPanel #CompanyAccount_Name").val() != '') {
            Endpoint.prototype.IsAccountNameExist($("#divAddSubAccountPanel #CompanyAccount_Name").val(), $("#divAddSubAccountPanel #CompanyAccount_AccountId").val(), $("#CompanyAccount_PortalId").val(), function (response) {
                if (!response) {
                    $("#CompanyAccount_Name").addClass("input-validation-error");
                    $("#accountNameErrorId").addClass("error-msg");
                    $("#accountNameErrorId").text(ZnodeBase.prototype.getResourceByKeyName("AlreadyExistAccountName"));
                    $("#accountNameErrorId").show();
                    isValid = false;
                    $("#loading-div-background").hide();
                }
            });
        }
        if ($("#RadioSpecific").is(':checked') == true) {
            if ($("#txtCatalogName").val().trim() == "" || $("#hdnPublishCatalogId").val().trim() === '') {
                $("#errorRequiredCatalog").text('').text(ZnodeBase.prototype.getResourceByKeyName("ErrorSelectCatalog")).addClass("field-validation-error").show();
                $("#txtCatalogName").parent("div").addClass('input-validation-error');
                isValid = false;
            }
        }
        return isValid;
    }

    ValidateAccountPermissionNameField(object): any {
        var isValid = true;
        if ($(object).val() == '') {
            $(object).addClass("input-validation-error");
            if ($(object).val() == '')
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("PermissionNameIsRequired"), 'error', isFadeOut, fadeOutTime);

            isValid = false;
        }
        else {
            $(object).remove("input-validation-error");
            $(object).removeClass("input-validation-error");
            isValid = true;
        }
        return isValid;
    }

    GetPublishCatalogList(control): any {
        $("#divStoreListAsidePanel").html("");
        ZnodeBase.prototype.BrowseAsidePoupPanel('/Account/GetCatalogList', 'divCataloglistPopup');
    }

    RadioChangeEvent(control): any {
        if ($(control).attr("value") == "IsDefault") {
            $("#txtCatalogName").val("");
            $("#hdnPublishCatalogId").val("");
            $("#catalogField").hide();
            $("#errorRequiredCatalog").text('').removeClass("field-validation-error").hide();
            $("#txtCatalogName").parent("div").removeClass('input-validation-error');
        }
        else {
            $("#catalogField").show();
        }
    }

    ValidateCatalog(): boolean {
        var flag = true;
        if (Account.prototype.isCatalogValid != undefined && !Account.prototype.isCatalogValid) {
            Account.prototype.ShowErrorMessage(ZnodeBase.prototype.getResourceByKeyName("InvalidCatalogName"));
            return flag = false;
        }
        else {
            Account.prototype.HideErrorMessage();
        }
        return flag;
    }

    ShowErrorMessage(errorMessage: string = "") {
        $("#txtCatalogName").parent("div").removeClass("input-validation-valid").addClass("input-validation-error");
        $("#errorRequiredCatalog").text('').text(errorMessage).addClass("field-validation-error").show();
        $("#hdnPublishCatalogId").val(0);
    }

    HideErrorMessage() {
        $("#txtCatalogName").parent("div").removeClass("input-validation-error").addClass("input-validation-valid");
        $("#errorRequiredCatalog").removeClass("field-validation-error").addClass(" field-validation-valid").hide();

    }

    AutocompleteCatalog() {
        Account.prototype.isCatalogValid = false;
        $("#txtCatalogName").autocomplete({
            source: function (request, response) {
                try {
                    Endpoint.prototype.GetCatalogList(request.term, function (res) {
                        if (res.PortalCatalogs != null) {
                            if (res.PortalCatalogs.length > 0) {
                                var catalogNames = new Array();
                                res.PortalCatalogs.forEach(function (catalogName) {
                                    if (catalogName.CatalogName != undefined)
                                        catalogNames.push(catalogName.CatalogName);
                                });
                                if ($.inArray(request.term, catalogNames) == -1)
                                    Account.prototype.isCatalogValid = false;
                                else
                                    Account.prototype.isCatalogValid = true;
                                response($.map(res.PortalCatalogs, function (item) {
                                    return {
                                        label: item.CatalogName,
                                        catalogId: item.PublishCatalogId,
                                    };
                                }));
                            }
                            else {
                                Account.prototype.isCatalogValid = false;
                                $(".ui-autocomplete").hide();
                                ZnodeBase.prototype.HideLoader();
                            }
                        } else {
                            $("#hdnPublishCatalogId").val('');
                            Account.prototype.isCatalogValid = false;
                        }
                    });
                } catch (err) {
                }
            },
            search: function () {
                $("#hdnPublishCatalogId").val('');
            },
            select: function (event, ui) {
                $("#hdnPublishCatalogId").val(ui.item.catalogId);
                Account.prototype.isCatalogValid = true;
            },
            focus: function (event, ui) {
                $("#hdnPublishCatalogId").val(ui.item.catalogId);
            }

        }).focusout(function () {
            ZnodeBase.prototype.HideLoader();
            let isValid: boolean = Account.prototype.ValidateCatalog();
            if (!isValid || $("#txtCatalogName").val().trim() == '')
                $("#hdnPublishCatalogId").val('');

            return isValid;
        });
    }

    public BindStates(action: string = ""): void {
        let countryCode: string = $('select[name="CountryName"]').val();

        if (countryCode == "" || countryCode == null || typeof countryCode == 'undefined') {
            countryCode = $('select[name="CompanyAccount.Address.CountryName"]').val();
            $('#SelectStateName').attr("name", "CompanyAccount.Address.StateName");
        }
        if (countryCode == "" || countryCode == null || typeof countryCode == 'undefined') {
            countryCode = $('select[name="Address.CountryName"]').val();
            $('#SelectStateName').attr("name", "Address.StateName");
        }

        if (countryCode != undefined && (countryCode.toLowerCase() == 'us' || countryCode.toLowerCase() == 'ca')) {
            Endpoint.prototype.GetStates(countryCode, function (response) {
                var stateName = $('#SelectStateName');
                stateName.empty();

                $("#dev-statecode-textbox  #StateName").val('');
                $("#dev-statecode-textbox  #StateName").attr("disabled", "disabled");
                $("#dev-statecode-textbox").hide();
                $("#dev-statecode-select").show();

                $.each(response.states, function (key, value) {
                    stateName.append('<option value="' + value.Value + '">' + value.Text + '</option>');
                });

                let code: string = $("#hdn_StateCode").val();
                $("#SelectStateName option").filter(function () {
                    return ($(this).val() == code);
                }).attr('selected', true);
            });
        }
        else {
            $("#dev-statecode-textbox #StateName").prop("disabled", false);
            $("#dev-statecode-textbox").show();
            $("#dev-statecode-select").hide();
        }
    }

    EditAreaMapping(data: any) {
        Account.prototype.SetEditAreaMappingAttributes(data);
    }

    IsLimitSelected(control: any) {
        $('input[name="IsNoLimitSelected"]').each(function () {
            var selectedUserApprovalId = $(this).attr('data-user-approverid');
            if ($(this).prop('checked')) {
                $("#toBudgetAmount" + selectedUserApprovalId).val("");
            }
            $("#hdnIsNoLimit_" + selectedUserApprovalId).val($(this).prop('checked'));
        });
    }

    AddNewArea(data: any) {
        data = 0;
        Account.prototype.GetLevelList(data);
        $(".MessageBox").remove();
        $("#partial").show();
        $('.thead-div').show();
    }

    GetLevelList(data: any) {
        data = 0;
        Endpoint.prototype.GetLevelList(function (response) {
            if (response.status) {
                $("#partial").html(response.html);
                Account.prototype.SetAddAreaMappingAttributes(data);
            }
            else {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, 'info', isFadeOut, fadeOutTime);
            }
        });
    }

    AreaMapperAddResult(data: any, control: any) {
        var id = $(control).closest("form").attr("id").split('_')[1];
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(data.message, data.status ? 'success' : 'error', isFadeOut, fadeOutTime);
        if (data.userApprovalId > 0 && data.status == true) {
            if (parseInt(id) == 0) {
                $("#partial").after(data.html);
                $("#partial").html("");
            }
            Account.prototype.GetApproverLevelList(data);
        }
        else {
            Account.prototype.GetApproverLevelList(data);
        }
    }

    GetApproverLevelList(data: any) {
        Endpoint.prototype.GetApproverLevelList($("#userId").val(), function (res) {
            $("#content-to-dispaly-in-approval-table").html(res);
            Account.prototype.AccountShowHideFormAttributes(data.userApprovalId);
        });
    }

    AccountShowHideFormAttributes(data) {
        $("#saveAreaMapping_" + data + "").attr("style", "display: none !important");
        $("#CancelAreaMapping_" + data + "").attr("style", "display: none !important");
        $("#EditAreaMapping_" + data + "").show();
        $("#deleteAreaMapping_" + data + "").show();
        $("#approverOrder_" + data + "").attr('disabled', 'disabled');
        $("#approverUser_" + data + "").attr('disabled', 'disabled');
        $("#ApproverUserId" + data + "").attr('disabled', 'disabled');
        $("#levelName_" + data + "").attr('disabled', 'disabled');
        $("#areaIsNoLimit_" + data + "").attr('disabled', 'disabled');
        $("#toBudgetAmount" + data + "").attr('disabled', 'disabled');
        $("#fromBudgetAmount" + data + "").attr('disabled', 'disabled');
    }

    CancelNewAddAreaMapping(data: any, control: any) {
        var UserApproverId = data.split('_')[1];
        if (UserApproverId <= 0) {
            $(control).closest("form").remove();
        }
        else {
            Endpoint.prototype.GetApproverLevelList($("#userId").val(), function (res) {
                $("#content-to-dispaly-in-approval-table").html(res);
            });
        }
    }

    SetAddAreaMappingAttributes(data) {
        $("#saveAreaMapping_" + data + "").show();
        $("#CancelAreaMapping_" + data + "").show();
        $("#EditAreaMapping_" + data + "").attr("style", "display: none !important");
        $("#deleteAreaMapping_" + data + "").attr("style", "display: none !important");
        $("#areaIsNoLimit_" + data + "").attr('disabled', false);
        $("#approverUser_" + data + "").attr('disabled', false);
        $("#ApproverUserId" + data + "").attr('disabled', false);
    }

    SetEditAreaMappingAttributes(data) {
        $("#saveAreaMapping_" + data + "").show();
        $("#CancelAreaMapping_" + data + "").show();
        $("#EditAreaMapping_" + data + "").attr("style", "display: none !important");
        $("#deleteAreaMapping_" + data + "").attr("style", "display: none !important");
        $("#areaIsNoLimit_" + data + "").attr('disabled', 'disabled');
        var checkboxValue = $("#areaIsNoLimit_" + data + "").is(":checked");
        $("#hdnIsNoLimit_" + data + "").val("" + checkboxValue + "");
        $("#approverUser_" + data + "").attr('disabled', false);
        $("#ApproverUserId" + data + "").attr('disabled', false);
    }

    DeleteAreaMapping(data: any, control: any) {
        Endpoint.prototype.DeleteAreaMapping(data, function (response) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, response.status ? 'success' : 'error', isFadeOut, fadeOutTime);
            if (response.status) {
                $(control).closest("form").remove();
            }
            WebSite.prototype.DisplayNoRecordFoundMessage();
        });
    }

    AutoCompleteApprovalUsers() {
        var portalId = $("#portalId").val();
        var accountId = $("#accountId").val();
        $(".txtApproverUser").autocomplete({
            source: function (request, response) {
                try {
                    var approvalUserIds: string = Account.prototype.GetApproverUserIds();
                    Endpoint.prototype.GetApproverUsersByName(request.term, portalId, accountId, approvalUserIds, function (res) {
                       if (res.length > 0) {
                            response($.map(res, function (item) {
                                return {
                                    label: item.UserName,
                                    userid: item.UserId,
                                };
                            }));
                        }
                    });
                } catch (err) {
                }
            },
            select: function (event, ui) {
                var id = $(this).attr("id").split('_')[1];
                $("#frmApprovalArea_" + id + " #ApproverUserId").val(ui.item.userid);
                $("input[name='ApproverUserId']").val(ui.item.userid)
            },
            focus: function (event, ui) {
                var id = $(this).attr("id").split('_')[1];
                $("#frmApprovalArea_" + id + " #ApproverUserId").val(ui.item.userid);
            }
        })
    }

    GetApproverUserIds(): string {
        var approvalUserIds = "";
        $(".txtApproverUserClass").each(function () {
            if (jQuery(this).attr("data_userid") == "True") {
                approvalUserIds += this.value + ",";
            }
        });
        return approvalUserIds.substring(0, approvalUserIds.length - 1);
    }

    CheckApproverOrderValue(): boolean {
        var selectedUserApproverId = $("#UserApproverId").val();
        var selectedFromAmount = $("#fromBudgetAmount" + selectedUserApproverId).val();
        var selectedToAmount = $("#toBudgetAmount" + selectedUserApproverId).val();
        var selectedApproverOrdeValue = $("#approverOrder_0").val();

        var isLimit = $('#areaIsNoLimit_0').is(':checked');

        if (selectedApproverOrdeValue == 0) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectApproverOrder"), "error", isFadeOut, fadeOutTime);
            return false;
        }
        if (selectedFromAmount == "") {
            $("#spanFromBudgetAmount" + selectedUserApproverId).html('<span class="field-validation-error" data-valmsg-for="FromBudgetAmount" data-valmsg-replace="true"><span id="fromBudgetAmount0-error" class="">From Amount is required.</span></span>')
            $("#fromBudgetAmount" + selectedUserApproverId).addClass("input-validation-error");
            return false;
        } else if (selectedToAmount == "" && isLimit == false) {
            $("#spanToBudgetAmount" + selectedUserApproverId).html('<span class="field-validation-error" data-valmsg-for="ToBudgetAmount" data-valmsg-replace="true"><span id="toBudgetAmount0-error" class="toAmount">To Amount is required.</span></span>')
            $("#toBudgetAmount" + selectedUserApproverId).addClass("input-validation-error");
            return false;
        }

        if (parseFloat(selectedToAmount) <= parseFloat(selectedFromAmount)) {
            $("#spanToBudgetAmount" + selectedUserApproverId).html('<span class="field-validation-error" data-valmsg-for="ToBudgetAmount" data-valmsg-replace="true">To Amount should be greater than From Amount.</span>')
            $("#toBudgetAmount" + selectedUserApproverId).addClass("input-validation-error");
            return false;
        }
    }

    OnFormAmountChange(): void {
        var selectedUserApproverId = $("#UserApproverId").val();
        var selectedFromAmount = $("#fromBudgetAmount" + selectedUserApproverId).val();
        var selectedToAmount = $("#toBudgetAmount" + selectedUserApproverId).val();
        if (selectedFromAmount != "") {
            $("#spanFromBudgetAmount" + selectedUserApproverId).html('');
        }
        if (selectedToAmount != "") {
            $("#spanToBudgetAmount" + selectedUserApproverId).html('');
        }
    }

    OnSelectPermissionCode(control: any) {
        var selectedValue = $(control).find("select option:selected").attr('data-permissioncode')
        if (selectedValue == 'DNRA')
            $("#content-to-dispaly-in-approval-table,.budget-error-msg,#add_partial_button").hide();
        else
            $("#content-to-dispaly-in-approval-table,.budget-error-msg,#add_partial_button").show();
    }

    ModifyConvetToOrderEvent(href): any {
        $("a.z-orders").attr('href', '#');
        var omsQuoteId = href.split('=')[1];
        if (omsQuoteId > 0) {
            $.ajax({
                url: "/Quote/ConvertToOrder",
                data: { "omsQuoteId": omsQuoteId },
                type: 'POST',
                success: function (data) {
                    if (!data.HasError) {
                        var form = $('<form action="CheckoutReceipt" method="post">' +
                            '<input type="hidden" name="orderId" value="' + data.OmsOrderId + '" />' +
                            '<input type="text" name= "ReceiptHtml" value= "' + data.ReceiptHtml + '" />' +
                            '<input type="hidden" name= "IsEmailSend" value= ' + data.IsEmailSend + ' />' +
                            '</form>');
                        var action = "/Quote/CheckoutReceipt";
                        form.attr("action", action);
                        $('body').append(form);
                        $(form).submit();
                    }
                    else {
                        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(data.ErrorMessage, "error", isFadeOut, fadeOutTime);
                    }
                }
            });
        }
    }

    SelectAndApplyLevelData(control): any {
        var selectedValue = $("#" + control.id).val();
        var numberofRows = $(".approver-rows").length;
        if (numberofRows > 0) {
            for (var iCount = 0; iCount < numberofRows; iCount++) {
                var valueTocheck = $($($($($(".approver-rows")[iCount]).closest("div"))[0]).find("div")[0]).find("input.txtApproverOrder").val();
                if (selectedValue == valueTocheck) {
                    var fromBudgetAmout = $($($($($(".approver-rows")[iCount]).closest("div"))[0]).find("div")[2]).find("input").val();
                    var toBudgetAmout = $($($($($(".approver-rows")[iCount]).closest("div"))[0]).find("div")[3]).find("input").val();
                    var IsNoLimit = $($($($($(".approver-rows")[iCount]).closest("div"))[0]).find("div")[4]).find("input").prop('checked');

                    $($("#" + control.id).parent().parent().find("div")[2]).find("input").val(fromBudgetAmout);
                    $($("#" + control.id).parent().parent().find("div")[3]).find("input").val(toBudgetAmout);
                    $($("#" + control.id).parent().parent().find("div")[4]).find("input").attr('checked', IsNoLimit)

                    $($("#" + control.id).parent().parent().find("div")[2]).find("input").attr('readonly', 'readonly');
                    $($("#" + control.id).parent().parent().find("div")[3]).find("input").attr('readonly', 'readonly');
                    $($("#" + control.id).parent().parent().find("div")[4]).find("input").attr('disabled', 'disabled');

                    $("#ApproverLevelId").val(1);
                    $("#fromBudgetAmount").val(fromBudgetAmout);
                    $("#toBudgetAmount").val(toBudgetAmout);
                    $("#areaIsNoLimit_0").attr('checked', IsNoLimit);
                    $("#IsNoLimit").val(IsNoLimit);
                    break;
                } else {
                    $($("#" + control.id).parent().parent().find("div")[2]).find("input").val('0');
                    $($("#" + control.id).parent().parent().find("div")[3]).find("input").val('0');
                    $($("#" + control.id).parent().parent().find("div")[4]).find("input").attr('checked', false);

                    $($("#" + control.id).parent().parent().find("div")[2]).find("input").removeAttr('readonly');
                    $($("#" + control.id).parent().parent().find("div")[3]).find("input").removeAttr('readonly');
                    $($("#" + control.id).parent().parent().find("div")[4]).find("input").removeAttr('disabled');

                    $("#ApproverLevelId").val(1);
                    $("#fromBudgetAmount").val(0);
                    $("#toBudgetAmount").val(0);
                    $("#areaIsNoLimit_0").attr('checked', false);
                    $("#IsNoLimit").val(0);
                }
            }
        }
    }

    DeleteAddress(control): any {
        var accountId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (accountId.length > 0) {
            Endpoint.prototype.DeleteAddress(accountId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete($("#AccountAddressList").find("#refreshGrid"), res);
            });
        }
    }

    //Call this method from page as this code was load before page load.
    PermissionCodeValue() {
        var selectedValue = $("#ddlPermission option:selected").attr('data-permissioncode');
        if (selectedValue == 'DNRA')
            $("#content-to-dispaly-in-approval-table,.budget-error-msg,#add_partial_button").hide();
    }

    // Associate users with Account
    AssociateUsersWithAccount(accountId: number): void {
        var userIds: string = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (userIds.length > 0)
            Endpoint.prototype.AssociateUsersWithAccount(userIds, accountId, function (res) {
                Endpoint.prototype.GetAssociateUsers(accountId, function (response) {
                    $("#ZnodeAccountsCustomer").html('');
                    $("#ZnodeAccountsCustomer").html(response);
                    DynamicGrid.prototype.ClearCheckboxArray();
                });
                $("#divAddCustomerAsidePanel").hide(700);
                ZnodeBase.prototype.RemovePopupOverlay();
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, res.status ? "success" : "error", isFadeOut, fadeOutTime);
                ZnodeBase.prototype.RemoveAsidePopupPanel();
            });
        else {
            $('#associatedUsersClass').show();
        }
    }

    //Display hand on mouse over on grid rows
    DisplayHandOnGridRows(gridName) {
        $("[data-swhgcontainer=" + gridName +"]").find("tr").addClass('preview-link');
    }    

    //This method is used to get account list on aside panel.
    GetParentAccountList(): any {
        var selectedPortal = $("#hdnPortalId").val();
        $("#maxBudgetDiv").hide();

        if (!selectedPortal)
            selectedPortal = -1

        ZnodeBase.prototype.BrowseAsidePoupPanel('/Account/GetParentAccountsList?portalId=' + selectedPortal, 'divParentAccountListPopup');
    }

    //To bind portal id to hidden feild
    BindParentAccountDetailsBasedOnPortalId(): any {
        $("span[data-valmsg-for='PortalId']").html("");
        Account.prototype.ClearParentAccountDetails();
        return true;
    }

    //Clear selected account details
    ClearParentAccountDetails(): boolean {
        $('#parentAccountName').val("");
        $('#hdnParentAccountId').val("0");
        $('#hdnParentAccountName').val(""); 
        return true;
    }

    //To bind parent account detail to field 
    BindSelectedParentAccountDetails(): any {
        $("#grid").find("tr").click(function () {

            var accountName: string = $(this).find("td[class='accountnamecolumn']").text();
            var accountCode: string = $(this).find("td[class='accountcodecolumn']").text();
            var accountId: string = $(this).find("td[class='accountidcolumn']").text();
        
            if (accountId && accountName) {
                $('#parentAccountName').val((!accountCode) ? accountName : accountName + " | " + accountCode);
                $('#hdnParentAccountName').val(accountName);
                $('#hdnParentAccountId').val(accountId);
                $('#divParentAccountListPopup').hide(700);               
                $("#ZnodeUserAccountList").html("");
                ZnodeBase.prototype.RemovePopupOverlay();
                return true;
            }
            else {
                ZnodeBase.prototype.RemovePopupOverlay();
                return false;
            }
        });
    }
}
