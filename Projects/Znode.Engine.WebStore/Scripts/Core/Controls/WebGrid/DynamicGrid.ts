var _gridContainerName;
var deleteActionlink;
var isSelectCalender = false;
var selectedImages = [];
class DynamicGrid extends ZnodeBase {

    constructor(doc: HTMLDocument) {
        super();
    }

    SetSortOrder() {
        DynamicGrid.prototype.getActionLink();
        $("#grid .grid-header th a").each(function () {
            var ahref = $(this).attr('href');
            if (ahref.indexOf('_swhg') >= 0) {
                var cutStr = ahref.substr(ahref.indexOf('_swhg'), ahref.length);
                ahref = ahref.replace(cutStr.split('&')[0], '')
                ahref = ahref.replace('_&', '');
                $(this).attr('href', ahref);
                //$(this).attr('data-swhglnk', false);
                $(this).attr('data-swhglnk', 'false');
            }
        });

        GridPager.prototype.UpdateHandler();

        $("#btnClearSearch").unbind("click");

        $("#btnClearSearch").on("click", function () {
            $(this).closest("form").find("input[type=text]").val('');
            $(this).closest("form").find("select").val('');
            $(this).closest("form").submit();
        });

        var currentThemeName = $("#currentThemePath").val();
        var tooltipPath = "/Views/Themes/" + currentThemeName + "/Content/bootstrap-3.3.7/js/tooltip.min.js";
        var bootstrapjs = document.createElement('script');
        bootstrapjs.setAttribute('src', window.location.protocol + "//" + window.location.host + tooltipPath);
        document.body.appendChild(bootstrapjs);

        if ($('.datepicker').length) {
            var s = document.createElement('script');
            s.setAttribute('src', window.location.protocol + "//" + window.location.host + "/scripts/lib/datepicker.js");
            document.body.appendChild(s);
        }

        DynamicGrid.prototype.CreateNestedGridNode();
        if (!navigator.userAgent.match(/Trident\/7\./)) {
            $('.table-responsive').addClass('scroll-default');
        }
    }

    getActionLink() {
        var index = 0;
        $("#grid th").each(function () {
            var hdrText = $.trim($(this).text());
            hdrText = hdrText.replace(/\s/g, "");
            if (hdrText === "Checkbox") {
                DynamicGrid.prototype.setCheckboxHeader(this);
                DynamicGrid.prototype.checkAllChange();
            }
            else if (hdrText.toLocaleLowerCase() === "select") {
                DynamicGrid.prototype.rowCheckChange();
            }
            index++;
        });

        //Set header class
        $('#grid tbody tr:eq(0) td').each(function () {
            var className = $(this).attr('class');
            index = $(this).index();
            if (className !== "") {
                $(this).closest("#grid").find('th:eq(' + index + ')').attr("class", className);
            }
        });
    }

    DynamicPartialLoad(url) {
        Endpoint.prototype.getView(url, function (res) {
            if (res !== null) {
                var element = document.createElement("div");
                element.innerHTML = res;
                $('#Resultpartial').html(element.innerHTML);
            }
        });
    }

    setCheckboxHeader(header) {
        $(header).closest("th").html("<input type='Checkbox' name='check-all' class='header-check-all' id='check-all'/><span class='lbl padding-8'></span>");
        this.rowCheckChange();
    }

    checkAllChange() {
        $(document).off("change", ".header-check-all");
        $(document).on("change", ".header-check-all", function () {
            var index = $(this).closest('th').index();
            if (this.checked) {
                $(this).closest('#grid').find('tr').find('td:eq(' + index + ') input[type=checkbox]:enabled').prop('checked', true)

                $(this).closest('#grid').find('tr').find('td:eq(' + index + ') input[type=checkbox]:enabled').each(function () {
                    CheckBoxCollection.push($(this).attr("id"))
                });

            }
            else {
                $(this).closest('#grid').find('tr').find('td:eq(' + index + ') input[type=checkbox]').prop('checked', false);


                $(this).closest('#grid').find('tr').find('td:eq(' + index + ') input[type=checkbox]:enabled').each(function () {
                    var removeItem = $(this).attr("id");
                    CheckBoxCollection = jQuery.grep(CheckBoxCollection, function (value) {
                        return value != removeItem;
                    });
                });
            }
        });
    }

    rowCheckChange() {
        $(".grid-row-checkbox").unbind("change");
        $(".grid-row-checkbox").change(function () {
            if (this.checked) {
                var checkBoxCount = $(this).closest('#grid').find(".grid-row-checkbox").length;
                var checkBoxCheckedCount = $(this).closest('#grid').find(".grid-row-checkbox:checked").length;

                if (checkBoxCount === checkBoxCheckedCount)
                    $(this).closest('#grid').find("#check-all").prop('checked', true);
                else
                    $(this).closest('#grid').find("#check-all").prop('checked', false);

                CheckBoxCollection.push($(this).attr("id"))
                var result = [];
                $.each(CheckBoxCollection, function (i, e) {
                    if ($.inArray(e, result) == -1) result.push(e);
                });

                CheckBoxCollection = result;
            }
            else {
                $(this).closest('#grid').find("#check-all").prop('checked', false);
                var removeItem = $(this).attr("id");
                CheckBoxCollection = jQuery.grep(CheckBoxCollection, function (value) {
                    return value != removeItem;
                });
            }
        });
    }

    SaveSelectedCheckboxItems(isSelected, selectedValue) {
        var selectedIds = new Array();

        if (localStorage.getItem("selectedchkboxItems") != "") {
            selectedIds = JSON.parse(localStorage.getItem("selectedchkboxItems"));
        }
        if (isSelected) {
            selectedIds.push(selectedValue);
        }
        else {
            //selectedIds.pop(selectedValue);
            selectedIds.splice(selectedValue);
        }
        this.SetDistinctItemsInArray(selectedIds);
    }

    CheckUncheckAllSelectedCheckboxItems(isSelected) {
        var selectedIds = new Array();

        if (localStorage.getItem("selectedchkboxItems") != "") {
            selectedIds = JSON.parse(localStorage.getItem("selectedchkboxItems"));
        }

        $(".grid-row-checkbox").each(function () {
            if (isSelected) {
                selectedIds.push($(this).attr('id'));
            }
            else {
                //selectedIds.pop($(this).attr('id'));
                selectedIds.pop();
            }
        });

        this.SetDistinctItemsInArray(selectedIds);
    }

    UncheckAllSelectedCheckboxItems() {
        localStorage.setItem("selectedchkboxItems", "");
    }

    SetDistinctItemsInArray(arrayObj) {
        var selectedIds = [];
        arrayObj.forEach(function (value) {
            if (selectedIds.indexOf(value) == -1) {
                selectedIds.push(value);
            }
        });
        if (selectedIds.length > 0) {
            localStorage.setItem("selectedchkboxItems", JSON.stringify(selectedIds));
        }
    }

    selectedRow(fun_success) {
        var ids = new Array();
        $(".grid-row-checkbox:checked").each(function () {
            ids.push({
                values: $.trim($(this).attr('id').split('_')[1])
            });
        });
        fun_success(ids);
    }

    setEnabledImage(index) {
        $("#grid tr").find("td:eq(" + index + ")").each(function () {
            var orgText = $(this).text();
            orgText = $.trim(orgText)
            $(this).text('');

            if (orgText === "True") {
                $(this).html("<i class='z-ok'></i>");
            }
            else {
                $(this).html("<i class='z-close'></i>");
            }
        });
    }

    setDeleteConfirm(index) {
        $("#grid tr").find("td:eq(" + index + ")").each(function () {

            var orgText = $(this).text();
            orgText = $.trim(orgText)
            $(this).text('');

            if (orgText.indexOf("isConfirm") >= 0) {
                if ((orgText.split('$')[1]).split('=')[1] === "true") {
                    $(this).html("<a class='zf-" + orgText.split('$')[0].toLowerCase() + " actiov-icon' href='#' title='" + orgText.split('$')[0] + "' onclick=CommonHelper.BindDeleteConfirmDialog('Confirm&nbspDelete?','Are&nbspyou&nbspsure,&nbspyou&nbspwant&nbspto&nbspdelete&nbspthis&nbsprecord?','" + orgText.split('$')[2] + "') ></a>");
                }
                else {
                    $(this).html("<a class='zf-" + orgText.split('$')[0].toLowerCase() + " actiov-icon' title='" + orgText.split('$')[0] + "' href='" + orgText.split('$')[2] + "'></a>");
                }
            }
            else {
                $(this).html("<a class='zf-" + orgText.split('$')[0].toLowerCase() + " actiov-icon' href='#' title='" + orgText.split('$')[0] + "' onclick=CommonHelper.BindDeleteConfirmDialog('Confirm&nbspDelete?','Are&nbspyou&nbspsure,&nbspyou&nbspwant&nbspto&nbspdelete&nbspthis&nbsprecord?','" + orgText.split('$')[1] + "') ></a>");
            }
        });
    }

    selectedRowByIndex(index, fun_success) {
        var ids = new Array();
        $("#grid tbody tr").find("td:eq(" + index + ") input[type=checkbox]:checked").each(function () {
            ids.push({
                values: $.trim($(this).attr('id').split('_')[1])
            });
        });
        fun_success(ids);
    }

    clickonGridClear(event, control) {
        event.preventDefault();
        var domain = $(control).attr('href');
        var location = window.location.href;
        location = location.replace((domain.split('?')[1]).split('=')[1], "");

        if (location.indexOf("FranchiseAdmin") >= 0 || location.indexOf("MallAdmin") >= 0) {
            window.location.href = (domain.split('?')[0]) + "?returnurl=../../" + location;
        }
        else {
            window.location.href = (domain.split('?')[0]) + "?returnurl=../" + location;
        }
    }

    DataValidattion(e, control) {
        var datatype = $(control).parent().parent().prev().find('select option:selected').attr('data-datype');
        switch (datatype) {
            case "Int32":
                // Allow: backspace, delete, tab, escape, enter and .
                if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
                    // Allow: Ctrl+A
                    (e.keyCode == 65 && e.ctrlKey === true) ||
                    // Allow: home, end, left, right, down, up
                    (e.keyCode >= 35 && e.keyCode <= 40)) {
                    // let it happen, don't do anything
                    return;
                }
                // Ensure that it is a number and stop the keypress
                if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
                    e.preventDefault();
                }
                break;
            case "String":
                // Allow: backspace, delete, tab, escape, enter and .
                if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
                    // Allow: Ctrl+A
                    (e.keyCode == 65 && e.ctrlKey === true) ||
                    // Allow: home, end, left, right, down, up
                    (e.keyCode >= 35 && e.keyCode <= 40)) {
                    // let it happen, don't do anything
                    return;
                }

                var str = String.fromCharCode(e.keyCode)
                if (!/^[a-zA-Z0-9\s]+$/.test(str)) {
                    e.preventDefault();
                }
                else {
                    return;
                }
                break;
        }
    }

    DataValidattionOnFilters(e, control) {
        var datatype = $(control).attr('data-datype');
        switch (datatype) {
            case "Int32":
                // Allow: backspace, delete, tab, escape, enter and .
                if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
                    // Allow: Ctrl+A
                    (e.keyCode == 65 && e.ctrlKey === true) ||
                    // Allow: home, end, left, right, down, up
                    (e.keyCode >= 35 && e.keyCode <= 40)) {
                    // let it happen, don't do anything
                    return;
                }
                // Ensure that it is a number and stop the keypress
                if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
                    e.preventDefault();
                }
                break;
            case "Decimal":
                // Allow: backspace, delete, tab, escape, enter and .
                if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
                    // Allow: Ctrl+A
                    (e.keyCode == 65 && e.ctrlKey === true) ||
                    // Allow: home, end, left, right, down, up
                    (e.keyCode >= 35 && e.keyCode <= 40)) {
                    // let it happen, don't do anything
                    return;
                }
                // Ensure that it is a number and stop the keypress
                if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
                    e.preventDefault();
                }
                break;
        }
    }

    CreateNestedGridNode() {
        if ($("#subT").length > 0) {
            //var size = $("#grid > thead > tr >th").size(); // get total column
            var size = $("#grid > thead > tr >th").length; // get total column
            $("#grid > thead > tr >th").last().remove(); // remove last column
            $("#grid > thead > tr").prepend("<th style='padding:0 10px;'></th>"); // add one column at first for collapsible column
            $("#grid > tbody > tr").each(function (i, el) {
                $(this).prepend(
                    $("<td></td>")
                        .addClass("expand-grid")
                        .addClass("hoverEff")
                        .attr('title', "click for show/hide")
                );

                //Now get sub table from last column and add this to the next new added row
                var table = $("table", this).parent().html();
                //add new row with this subtable
                $(this).after("<tr><td style='padding-left:20px;' colspan='" + (size) + "'>" + table + "</td></tr>");
                $("table", this).parent().remove();
                // ADD CLICK EVENT FOR MAKE COLLAPSIBLE
                $(".hoverEff", this).on("click", function () {
                    if ($(this).hasClass("collapse-grid")) {
                        var id = $(this).parent().closest("tr").next().find('table:eq(0) tbody tr:eq(0) td').find("#recored-id").val();
                        var type = $(this).parent().closest("tr").next().find('table:eq(0) tbody tr:eq(0) td').find("#type-name").val();
                        var method = $(this).parent().closest("tr").next().find('table:eq(0) tbody tr:eq(0) td').find("#method-name").val();
                        var control = this;
                        this.GetSubGrid(id, type, method, function (response) {

                            $(control).parent().closest("tr").next().find('table:eq(0) thead').remove();
                            $(control).parent().closest("tr").next().find('table:eq(0) tbody tr:eq(0)').css("display", "none");
                            if ($(control).parent().closest("tr").next().find('table:eq(0) tbody tr').length == 1) {
                                $(control).parent().closest("tr").next().find('table:eq(0) tbody').append('<tr><td></td></tr>')
                            }

                            $(control).parent().closest("tr").next().find('table:eq(0) tbody tr:eq(1) td').html(response);

                            if ($("#report-title").text() === "Order Pick List") {
                                $("#subT table tbody tr").find('td:eq(3)').each(function () {
                                    var txt = $(this).text(); $(this).html(txt);
                                })
                            }

                            $(control).parent().closest("tr").next().find('table:eq(0) tbody tr:eq(1) td').find('th').each(function () {
                                var header = $(this).text();
                                $(this).text(header.replace('_', ' '));
                            });

                            $(control).parent().closest("tr").next().slideToggle(100);
                            $(control).toggleClass("expand-grid collapse-grid");

                        });
                    }
                    else {
                        $(this).parent().closest("tr").next().slideToggle(100);
                        $(this).toggleClass("expand-grid collapse-grid");
                    }
                });

            });

            //by default make all subgrid in collapse mode
            $("#grid > tbody > tr td.expand-grid").each(function (i, el) {
                $(this).toggleClass("collapse-grid expand-grid");
                $(this).parent().closest("tr").next().slideToggle(100);
            });
        }
    }

    GetSubGrid(id, type, method, callback_fun) {
    }

    GetSelectedCheckBoxValue() {
        var selectedIds = new Array();

        if (localStorage.getItem("selectedchkboxItems") != undefined && localStorage.getItem("selectedchkboxItems") != "") {
            selectedIds = JSON.parse(localStorage.getItem("selectedchkboxItems"));
            for (var item in selectedIds) {
                selectedIds[item] = selectedIds[item].replace("rowcheck_", "");
            }
            return selectedIds;
        }
    }

    ShowHideGrid() {
        $("#grid-list-content").animate({
            opacity: 'toggle'
        }, 'slow');
        var text = $('#hide-grid-link').text();
        $('#hide-grid-link').text(
            text == "Hide Grid" ? "Show Grid" : "Hide Grid");
    }

    IsDataPresentInList(control, value) {
        $(control).parent().parent().next().find('select option').each(function () {
            if (this.value == value) {
                return false;
            }
        });
    }

    GetPopoverForFilter() {
        var popoverContent = $('.popovercontent');
        if (popoverContent.length > 0) {
            popoverContent.popover({
                html: true,
                content: function () {
                    var input_text = "";
                    var options_list = "<div class='parent-content-popover'> <select name=\"DataOperatorId\" style='float:left;width:80px;'>" +
                        $(this).attr("data-options-list") + "</select>";
                    if ($(this).attr('data-datype').toLowerCase() == "boolean") {
                        var isSelectedTrue = "";
                        if ($(this).attr("data-text-value") == "False") {
                            isSelectedTrue = "selected";
                        }
                        input_text = "<select name=" + $(this).attr('data-columnname') + " style='float:left;width:80px;'>" +
                            "<option value='True'>True</option><option value='False' " + isSelectedTrue + ">False</option ></select>";
                    }
                    if ($(this).attr('data-datype').toLowerCase() == "date" || $(this).attr('data-datype').toLowerCase() == "datetime") {
                        input_text = '<div class="" id="filter-componant-control-content" style="float:left;">' +
                            '<input id="filtercolumn" type="text" style="float:left;width:150px;"' +
                            ' data-datype="' + $(this).attr('data-datype') +
                            '" ' + $(this).attr('data-max-length') +
                            ' name="' + $(this).attr('data-columnname') +
                            '" data-columnname="' + $(this).attr('data-columnname') +
                            '" value="' + $(this).attr("data-text-value") + '"class="datepicker" data-date-format="' + $(this).attr("data-column-dateformat") + '"  maxlength=' + '"50" />' +
                            '</div>';
                    }
                    else {
                        input_text = '<div class="" id="filter-componant-control-content" style="float:left;">' +
                            '<input id="filtercolumn" type="text" style="float:left;width:150px;"' +
                            ' data-datype="' + $(this).attr('data-datype') +
                            '" ' + $(this).attr('data-max-length') +
                            ' name="' + $(this).attr('data-columnname') +
                            '" data-columnname="' + $(this).attr('data-columnname') +
                            '" value="' + $(this).attr("data-text-value") + '" maxlength=' + '"130" />' +
                            '</div>';
                    }
                    var buttonHtml = '<div class="pull-left"><button title="Search" class="filterButton filter-search-btn" onclick="DynamicGrid.prototype.FilterButtonPress(this);return false;"><i class="icon-search zf-search"></i></button></div></div>';

                    return options_list + input_text + buttonHtml;
                },
                placement: "bottom",
                container: "#" + $('.popovercontent').first().parent().attr('id')
            });
        }
        if ($('.datepicker').length) {
            var s = document.createElement('script');
            s.setAttribute('src', window.location.protocol + "//" + window.location.host + "/scripts/lib/datepicker.js");
            document.body.appendChild(s);
        }
    }

    FilterButtonPress(control) {
        $(control).closest("form").submit();
    }

    HidePopover() {
        $('body').off('click');
        $('body').on('click', function (e) {
            if (!isSelectCalender) {
                $('[data-toggle="popover"]').each(function () {
                    if (!$(this).is(e.target) && $(this).has(e.target).length === 0 && $('.popover').has(e.target).length === 0) {
                        $(this).popover('hide');
                    }
                });

            }
            isSelectCalender = false;

            if (!$('div.dropdown').is(e.target) && $('div.dropdown').has(e.target).length === 0 && $('.open').has(e.target).length === 0) {
                $('div.dropdown').removeClass('open');
            }
        });
    }

    DataValidattionOnKeyDown() {
        $(document).on("keydown", "#filter-componant-control-content input[type=text]", function (e) {
            DynamicGrid.prototype.DataValidattionOnFilters(e, this)
        });
    }

    ShowHidecolumn(res) {
        GridPager.prototype.SelectedPageSize(controlContext);
    }

    GenerateFilter(data, target) {
        var _val = parseInt(data);
        if (!isNaN(_val)) {
            $($(target).closest("#searchform").attr("data-ajax-update")).find("section").find("#refreshGrid")[0].click();
            $(target).closest("#searchform").find("#filter-control-" + _val).remove();
        }
        else {
            $(target).closest("#searchform").find("#filter-content-main").append(data);
            this.GetPopoverForFilter();
        }
    }

    ClearFilter(control, filterId, ColumnName) {
        if ($(control).closest("form").find('input[name="' + ColumnName + '"]').length <= 0) {
            $("<input type='hidden' name='" + ColumnName + "' id='" + ColumnName + "' value=''/>").appendTo($(control).closest("form").find("#filter-control-" + filterId));
        }
        $(control).closest("form").submit();
        /*ZnodeBase.prototype.activeAsidePannel()*/;
    }

    Init() {
        this.DataValidattionOnKeyDown();
        $("#btnClearSearch").unbind("click");
        $("#btnClearSearch").click(function () {
            $(this).closest("form").find("input[type=text]").val('');
            $(this).closest("form").find("select").val('');
            $(this).closest("form").submit();
        });
        this.GetPopoverForFilter();
        this.getActionLink();
        this.HidePopover();
        localStorage.setItem("selectedchkboxItems", "");
    }

    GetNextPreviousRecords(areaName, controller, action, id) {
        var url = "";
        if (areaName != null && areaName != "") {
            url = "/" + areaName + "/" + controller + "/" + action + "?" + id;
        }
        else {
            url = "/" + controller + "/" + action + "?" + id;
        }
        this.DynamicPartialLoad(url);
    }

    ShowHideTileContext(id, isShow) {
        if (isShow) {
            $(id).hide();
        }
        else {
            $(id).show();
        }
    }

    ShowHideTileOverlay(obj) {
        if ($(obj).is(":checked")) {

            selectedImages.push({
                values: $.trim($(obj).attr("id").split('_')[1]),
                source: $(obj).parent().parent().find('img').attr('src'),
                text: $(obj).parent().parent().parent().find(".title").text()
            });
            $(obj).parent().parent().parent().addClass("img-checked");
        } else {
            var id = $.trim($(obj).attr("id").split('_')[1]);
            for (var i = 0; i < selectedImages.length; i++) {
                if (selectedImages[i].values === id)
                    selectedImages.splice(i, 1);
            }
            $(obj).parent().parent().parent().removeClass("img-checked");
        }
    }

    ConfirmDelete(url, control) {
        deleteActionlink = control;
        $("#hdnDeleteActionURL").val(url)
    }

    RefreshGrid(control, response) {
        $(control).closest('section').find("#refreshGrid").click();
        $('.modal-backdrop').remove();
        // Notification.prototype.DisplayNotificationMessagesHelper(response.message, response.status ? 'success' : 'error', isFadeOut, fadeOutTime);
    }

    ClearCheckboxArray() {
        CheckBoxCollection = new Array();
    }

    ConfirmEnableDisable(url) {
        $("#hdnEnableDisableActionURL").val(url)
    }

    RedirectToEnableDisable() {
        window.location.href = window.location.protocol + "//" + window.location.host + $("#hdnEnableDisableActionURL").val();
    }

    ConfirmResetPassword(url) {
        $("#hdnResetPasswordURL").val(url)
    }

    RedirectToResetPassword() {
        window.location.href = window.location.protocol + "//" + window.location.host + $("#hdnResetPasswordURL").val();
    }

    GetChildMenus(data) {
        $('#mainDiv').html(data);
        var bootstrapjs = document.createElement('script');
        bootstrapjs.setAttribute('src', window.location.protocol + "//" + window.location.host + "/Scripts/Core/Znode/RoleAndAccessRight.js");
        document.body.appendChild(bootstrapjs);
    }

    GetMultipleSelectedIds(target = undefined) {
        var ids = [];

        if (target !== undefined) {
            _gridContainerName = "#" + $(target).closest("section").attr('update-container-id');
        }
        if (CheckBoxCollection === undefined || CheckBoxCollection.length === 0) {
            $(_gridContainerName + " #grid").find("tr").each(function () {
                if ($(this).find(".grid-row-checkbox").length > 0) {
                    if ($(this).find(".grid-row-checkbox").is(":checked")) {
                        var id = $(this).find(".grid-row-checkbox").attr("id");
                        CheckBoxCollection.push(id);
                    }
                }
            });
        }

        var result = [];
        $.each(CheckBoxCollection, function (i, e) {
            if ($.inArray(e, result) == -1) result.push(e.split("_")[1]);
        });
        return result.join();
    }

    LoadDatepickerScript() {
        $('.popovercontent').on("click", function () {
            if ($('.datepicker').length) {
                var s = document.createElement('script');
                s.setAttribute('src', window.location.protocol + "//" + window.location.host + "/scripts/lib/datepicker.js");
                document.body.appendChild(s);
            }
        });
    }

    RefreshGridOndelete(control, response) {
        if (response.status) {
            if (($(control).closest('section').find('#grid tbody tr').length) === $(control).closest('section').find('#grid tbody tr').find("input[type=checkbox]:checked").length || ($(control).closest('section').find('#grid tbody tr').length - 1) === 0) {
                if (PageIndex > 0) {
                    PageIndex = PageIndex - 1;
                }
            }
            if (($(control).closest('section').find('#grid tbody tr').length) !== $(control).closest('section').find('#grid tbody tr').find("input[type=checkbox]:checked").length && $("#pagerTxt").val() == $("#pagerTxt").next('span').text().replace("/ ", "").trim()) {
                PageIndex = 0;
            }
        }
        $(control).closest("section").find("#pagerTxt").val(PageIndex + 1);
        $(control).closest('section').find("#refreshGrid").click();
        this.ClearCheckboxArray();
        $('.modal-backdrop').remove();
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, response.status ? 'success' : 'error', isFadeOut, fadeOutTime);
    }

    RedirectToDelete(control) {
        control = deleteActionlink !== undefined ? deleteActionlink : control;
        $("#loading-div-background").show();
        $.ajax({
            type: "GET",
            url: $("#hdnDeleteActionURL").val(),
            success: (function (response) {
                DynamicGrid.prototype.RefreshGridOndelete(control, response);
            })
        });
    }
}

$(window).on("load", function () {
    var _dynamicGrid = new DynamicGrid(window.document);
    _dynamicGrid.Init();
    DynamicGrid.prototype.LoadDatepickerScript();
});
$(document).ajaxComplete(function () {
    var _dynamicGridAjax: DynamicGrid;
    _dynamicGridAjax = new DynamicGrid(window.document);
    _dynamicGridAjax.GetPopoverForFilter();
    _dynamicGridAjax.getActionLink();
    DynamicGrid.prototype.LoadDatepickerScript();
});