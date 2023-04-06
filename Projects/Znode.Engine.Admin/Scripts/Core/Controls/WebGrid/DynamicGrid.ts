var _gridContainerName;
var deleteActionlink;
var isSelectCalender = false;
var selectedImages = [];
var popoverHtml;
var popoverHtmlForPanel;
var dyanmic_grid_loaded = "DynamicGridLoaded";


//This condition is checked to stop the re-initialization of filterColumnList.
if (!filterColumnList)
    var filterColumnList = [];
class DynamicGrid extends ZnodeBase {

    constructor(doc: HTMLDocument) {
        super();
        $(document).trigger(dyanmic_grid_loaded)
    }

    SetColumnList(columnList: any) {
        filterColumnList = JSON.parse(JSON.stringify(columnList));
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

        $("#btnClearSearch").off("click");

        $("#btnClearSearch").on("click", function () {
            $(this).closest("form").find("input[type=text]").val('');
            $(this).closest("form").find("select").val('');
            $(this).closest("form").submit();
        });

        var bootstrapjs = document.createElement('script');
        bootstrapjs.setAttribute('src', window.location.protocol + "//" + window.location.host + "/Content/bootstrap-3.3.7/js/tooltip.min.js");
        document.body.appendChild(bootstrapjs);

        if ($('.datepicker').length) {
            var s = document.createElement('script');
            s.setAttribute('src', window.location.protocol + "//" + window.location.host + "/Content/bootstrap-3.3.7/js/datepicker.js");
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
        $('#grid:visible tbody tr:eq(0) td').each(function () {
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
                    if ($.inArray($(this).attr("id"), CheckBoxCollection) == -1) CheckBoxCollection.push($(this).attr("id"))
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
        $(".grid-row-checkbox").off("change");
        $(".grid-row-checkbox").on("change", function () {
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
                    $(this).html("<a class='z-" + orgText.split('$')[0].toLowerCase() + " actiov-icon' href='#' title='" + orgText.split('$')[0] + "' onclick=CommonHelper.BindDeleteConfirmDialog('Confirm&nbspDelete?','Are&nbspyou&nbspsure,&nbspyou&nbspwant&nbspto&nbspdelete&nbspthis&nbsprecord?','" + orgText.split('$')[2] + "') ></a>");
                }
                else {
                    $(this).html("<a class='z-" + orgText.split('$')[0].toLowerCase() + " actiov-icon' title='" + orgText.split('$')[0] + "' href='" + orgText.split('$')[2] + "'></a>");
                }
            }
            else {
                $(this).html("<a class='z-" + orgText.split('$')[0].toLowerCase() + " actiov-icon' href='#' title='" + orgText.split('$')[0] + "' onclick=CommonHelper.BindDeleteConfirmDialog('Confirm&nbspDelete?','Are&nbspyou&nbspsure,&nbspyou&nbspwant&nbspto&nbspdelete&nbspthis&nbsprecord?','" + orgText.split('$')[1] + "') ></a>");
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
        if (datatype == "Int32" || datatype == "Decimal") {
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
        }
    }
    OnFilterColumnChange(e) {
        var currentTarget = $(e).find('option:selected');
        var inputText = "";
        var dateFormat = currentTarget.attr('data-column-dateformat');
        var columnId = parseInt(currentTarget.attr('data-columnId'));
        var filterColumnObj = this.GetFilterColumnList(columnId);
        $(e).parent().find('.filtervalueinput') ? $(e).parent().find('.filtervalueinput').empty() : "";
        inputText = this.GetInputField(filterColumnObj, dateFormat);
        $(e).parent().find('.filtervalueinput').append(inputText);
        if ($('.datepicker').length) {
            var s = document.createElement('script');
            s.setAttribute('src', window.location.protocol + "//" + window.location.host + "/Content/bootstrap-3.3.7/js/datepicker.js");
            document.body.appendChild(s);
        }

        var optionList = $(e).parent().find('.optionList');
        optionList.html("");
        optionList.html(filterColumnObj['selectedListOfDatatype']);
    }

    private GetFilterColumnList(columnId: number) {
        var filterColumnObj = {};
        for (var i = 0; i < filterColumnList.length; i++) {
            if (filterColumnList[i] && filterColumnList[i].Id === columnId) {
                filterColumnObj['selectedListOfDatatype'] = filterColumnList[i].SelectListOfDatatype;
                filterColumnObj['dataType'] = filterColumnList[i].DataType;
                filterColumnObj['columnName'] = filterColumnList[i].ColumnName;
                filterColumnObj['textValue'] = filterColumnList[i].TextValue;
            }
        }
        return filterColumnObj;
    }

    private GetInputField(filterColumnObj: {}, dateFormat: string) {
        var mxlen = (filterColumnObj['dataType'] === "Int32") ? "maxlength=20" : "";
        if (filterColumnObj['dataType']) {
            if (filterColumnObj['dataType'].toLowerCase() == "boolean") {
                var isSelectedTrue = "";
                if (filterColumnObj['textValue'] == "False") {
                    isSelectedTrue = "selected";
                }
                return "<select name=" + filterColumnObj['columnName'] + " style='float:left;width:80px;'>" +
                    "<option value='True'>True</option><option value='False' " + isSelectedTrue + ">False</option ></select>";
            }
            else if (filterColumnObj['dataType'].toLowerCase() == "date" || filterColumnObj['dataType'].toLowerCase() == "datetime") {
                return (
                    '<input id="filtercolumn" class="filtercolumn datepicker" type="text" style="float:left;width:150px;"' +
                    ' data-datype="' + filterColumnObj['dataType'] + '" ' + mxlen +
                    ' name="' + filterColumnObj['columnName'] +
                    '" data-columnname="' + filterColumnObj['columnName'] +
                    '" value=""" " data-date-format="' + dateFormat + '"  maxlength=' + '"50" />');
            }

        }
        return '<input id="filtercolumn" type="text" style="float:left;width:150px;"' +
            'data-datype="' + filterColumnObj['dataType'] + '" ' + mxlen +
            ' name="' + filterColumnObj['columnName'] +
            '" data-columnname="' + filterColumnObj['columnName'] +
            '" value=""" " maxlength=' + '"130" />';

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
        //App.Api.GetSubGridPartial(id, type, method, function (response) {
        //    callback_fun(response);
        //});
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
        //else {
        //    return "";
        //}
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

        $('.popovercontent').popover({
            html: true,
            content: function () {
                $(".z-manage-filter").addClass("active-manage-filter");
                var length = $('.parent-content-popover').length;
                var htmlContent = "";
                var filterHtml = $(this).parentsUntil('#filterComponent').find('.parent-content-popover')[0];
                if (!$('#aside-popup-panel')[0]) {
                    htmlContent = popoverHtml = filterHtml && filterHtml.innerHTML ? filterHtml.innerHTML : popoverHtml;
                } else
                    htmlContent = popoverHtmlForPanel = filterHtml && filterHtml.innerHTML ? filterHtml.innerHTML : popoverHtmlForPanel;
                filterHtml.innerHTML = "";
                return htmlContent;
            },
            placement: "bottom"
        });
        if ($('.datepicker').length) {
            var s = document.createElement('script');
            s.setAttribute('src', window.location.protocol + "//" + window.location.host + "/Content/bootstrap-3.3.7/js/datepicker.js");
            document.body.appendChild(s);
        }
    }

    FilterButtonPress(control) {   
        $(control).closest("form").submit();
        UpdateContainerId = $(control).closest('form').attr('data-ajax-update').replace("#", "");
    }


    HidePopover() {
        $('body').off('click');
        $('body').on('click', function (e) {
            if (!isSelectCalender) {
                $('[data-toggle="popover"]').each(function () {
                    if (!$(this).is(e.target) && $(this).has(e.target).length === 0 && $('.popover').has(e.target).length === 0) {
                        if (e.target.id == "close") {
                            return;
                        }
                        $(this).popover('hide');
                        $(".z-manage-filter").removeClass("active-manage-filter");
                    }
                });

            }
            isSelectCalender = false;

            if (!$('div.dropdown').is(e.target) && $('div.dropdown').has(e.target).length === 0 && $('.open').has(e.target).length === 0) {
                $('div.dropdown').removeClass('open');
            }
        });

        $('body').on('hidden.bs.popover', function (e) {
            $(e.target).data("bs.popover").inState.click = false;
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
            $(target).closest("#searchform").find("#filter-control-" + _val).remove();

            if ($($(target).closest("#searchform").attr("data-ajax-update")).find("section").find("#refreshGrid")[0] !== undefined)
                $($(target).closest("#searchform").attr("data-ajax-update")).find("section").find("#refreshGrid")[0].click();
        }
        else {
            $(target).closest("#searchform").find("#filter-content-main").append(data);
            this.GetPopoverForFilter();
        }
    }
    RemoveFilterButton(control: any, filterName: string) {
        if (filterName != null) {
            $(control).parentsUntil('#filterComponent').find('[name="' + filterName + '"]').val("");

            $(control).closest("form").submit();
        }
        $(control).parent()[0].outerHTML = "";

    }
    RemoveFilterText(control: any, filterName: string) {
        if (filterName != null) {
            $(control).parentsUntil('#filterComponent').find('[name="' + filterName + '"]').val("");
        }
        var addControl = this.GetCurrentContext(control).next().find("#addfilter");
        if (addControl.attr('disabled'))
            this.EnableDisableAddFilterButton(addControl, false);
        $(control).parent().find('.columnList').empty();
        $(control).parent().hide();
    }
    //Get current Active Filter element 
    GetCurrentContext(selector): any {
        return $(selector).parentsUntil('#filterComponent');
    }

    AddFilterButton(control) {

        var div = document.createElement('div');
        var htmlText = this.GetCurrentContext(control).find('.add-filter-template').html();

        var length = $('.columnList:visible').find('option:selected').length;
        for (var index = 0; index <= length - 1; index++) {
            var optionList = $('.columnList:visible').find('option:selected')[index].outerHTML;
            if (optionList != "<option>Select Column</option>") {
                htmlText = htmlText.replace(optionList, "");
            }
        }
        $(div).append(htmlText);
        this.GetCurrentContext(control).find('.containerClass').append(div);
        //Disable Add Filter button when option list contain 1 or less options
        if (($('.columnList:visible:last option').length - 1) <= 1)
            this.EnableDisableAddFilterButton(control, true);

    }
    //Enable/Disable Add filter button
    EnableDisableAddFilterButton(selector, isDisabled): void {
        isDisabled ? $(selector).attr('disabled', true) : $(selector).attr('disabled', false);
    }

    ClearFilter(control, filterId, ColumnName) {
        if ($(control).closest("form").find('input[name="' + ColumnName + '"]').length <= 0) {
            $("<input type='hidden' name='" + ColumnName + "' id='" + ColumnName + "' value=''/>").appendTo($(control).closest("form").find("#filter-control-" + filterId));
        }
        $(control).closest("form").submit();
        ZnodeBase.prototype.activeAsidePannel();
        UpdateContainerId = $(control).closest('form').attr('data-ajax-update').replace("#", "");
        GridPager.prototype.SetSelectedCheckboxChecked();
    }

    Init() {
        this.DataValidattionOnKeyDown();
        $("#btnClearSearch").off("click");
        $("#btnClearSearch").on("click", function () {
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
        if ($(obj).is(":checked") && IsValidImage) {

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

    RedirectToDelete(control) {
        control = deleteActionlink !== undefined ? deleteActionlink : control;
        ZnodeBase.prototype.ShowLoader();
        $.ajax({
            type: "GET",
            url: $("#hdnDeleteActionURL").val(),
            success: (function (response) {
                DynamicGrid.prototype.RefreshGridOndelete(control, response);
            })
        });
    }

    RefreshGridNoNotification(control) {
        $(control).closest('section').find("#refreshGrid").click();
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
        this.RemoveModalOverlay();
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, response.status ? 'success' : 'error', isFadeOut, fadeOutTime);
        if (response.status) {
            $(document).trigger("GRID_ON_DELETE");
        }
    }

    RefreshGrid(control, response) {
        $(control).closest('section').find("#refreshGrid").click();
        this.RemoveModalOverlay();
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, response.status ? 'success' : 'error', isFadeOut, fadeOutTime);
    }

    RefreshGridOnNewView(control, response) {
        $(control).closest('section').find("#refreshGrid").click();
        if ($('.panel-container').length > 0)
            ZnodeNotification.prototype.DisplayNotificationMessagesHelperForAsidePanel(response.message, response.status ? 'success' : 'error', isFadeOut, fadeOutTime);
        else
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, response.status ? 'success' : 'error', isFadeOut, fadeOutTime);
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
            if ($.inArray(e.slice(e.indexOf("_") + 1), result) == -1) result.push(e.slice(e.indexOf("_") + 1));
           
        });
        return result.join();
    }

    LoadDatepickerScript() {
        $('.popovercontent').on("click", function () {
            if ($('.datepicker').length) {
                var s = document.createElement('script');
                s.setAttribute('src', window.location.protocol + "//" + window.location.host + "/Content/bootstrap-3.3.7/js/datepicker.js");
                document.body.appendChild(s);
            }
        });
    }


    CreateNewView(control) {
        var _isPublic;
        var _isDefault = false;
        var _targetid = $(control).data('targetid');
        var _itemId = $(control).data('id');
        var _newText = $(control).parent().find("#view_item_" + _itemId).val();
        if (_itemId != 0) {
            _isPublic = $(control).attr('data-isPublic') == 'True' ? true : false;
            _isDefault = $(control).attr('data-isDefault') == 'True' ? true : false;
        }
        else
            _isPublic = $(control).parent().parent().find('input[type="checkbox"]').is(":checked");
        Endpoint.prototype.CreateNewView(_itemId, _newText, _isPublic, _isDefault, function (responce) {
            DynamicGrid.prototype.RefreshGridOnNewView(control, responce);
        });
    }
    SetDefaultView(control) {
        var _isDefault = ($(control).attr('data-isDefault') == 'True' ? true : false);
        if (!_isDefault) {
            var _isPublic = $(control).attr('data-isPublic') == 'True' ? true : false;
            var _isDefault = !($(control).attr('data-isDefault') == 'True' ? true : false);
            this.EditView(control, _isDefault, _isPublic);
        }

    }

    ChangeViewVisibility(control) {
        var _isDefault = ($(control).attr('data-isDefault') == 'True' ? true : false);
        var _isPublic = !($(control).attr('data-isPublic') == 'True' ? true : false);
        this.EditView(control, _isDefault, _isPublic);
    }

    EditView(control, _isDefault, _isPublic) {
        var _targetid = $(control).data('targetid');
        var _itemId = $(control).data('id');
        var _text = $(control).data('text');
        Endpoint.prototype.CreateNewView(_itemId, _text, _isPublic, _isDefault, function (responce) {
            DynamicGrid.prototype.RefreshGridOnNewView(control, responce);
        });
    }


    DeleteView(control) {
        var _targetid = $(control).data('targetid');
        var _itemId = $(control).data('id');

        Endpoint.prototype.DeleteView(_itemId, function (responce) {
            DynamicGrid.prototype.RefreshGridOnNewView(control, responce);
        });
    }

    GetView(control) {
        var _itemId = $(control).data('id');
        var _viewName = $("#grid").attr('data-swhgcontainer')

        Endpoint.prototype.GetView(_itemId, _viewName, function (responce) {
            $(control).closest('section').find("#refreshGrid").click();
        });
    }


    GetMultipleValuesOfGridColumn(columnName): string {
        var column: any = [];
        var index: number = 0;
        $("#grid").find("tr.grid-header").find("th").each(function () {
            column.push($(this));
        });

        index = this.GetIndexOfColumn(column, columnName);

        return this.GetColumnValue(index);
    }

    //Get the index of column in grid.
    GetIndexOfColumn(column: any, columnName: string): number {
        for (var index = 0; index < column.length; index++) {
            if (column[index].text().trim() == columnName) {
                return index;
            }
        }
    }

    //Get the value of column in grid.
    GetColumnValue(index: number): string {
        var value: string = "";
        $("#grid").find("tr").each(function () {
            if ($(this).find(".grid-row-checkbox").length > 0) {
                if ($(this).find(".grid-row-checkbox").is(":checked")) {
                    value = value + $(this).find("td")[index].innerText + ",";
                }
            }
        });
        return value.substr(0, value.length - 1);
    }
    //Clear all the assigned filter
    ClearAllFilter(control) {
        var length = $('.filtervalueinput').find('[name]').length;
        for (var index = 0; index < length; index++) {
            var elements = $('.filtervalueinput').find('[name]')[index];
            elements['value'] = "";
        }

        $(control).closest("form").submit();
    }

    //Clear the global filter.
    ClearGlobalFilter(control, ColumnName) {
        if ($(control).closest("form").find('input[name="' + ColumnName + '"]').length > 0) {
            $(control).closest("form").find('input[name="' + ColumnName + '"]').val('');
        }
        else {
            $("#globalfiltercolumn").val('');
        }
        $(control).closest("form").submit();
        ZnodeBase.prototype.activeAsidePannel();
        UpdateContainerId = $(control).closest('form').attr('data-ajax-update').replace("#", "");
        GridPager.prototype.SetSelectedCheckboxChecked();
    }

    //Setting the value of check box while creating view.
    SetCheckboxValue(control) {
        var element = $(control).parent().find('input[type="checkbox"]');
        if (element && element.is(":checked"))
            element.prop('checked', false)
        else
            element.prop('checked', true)

    }

    RemoveModalOverlay() {
        if ($('.modal-backdrop :last-child').length >= 1)
            $('.modal-backdrop :last-child').remove();
        else
            $('.modal-backdrop').remove();
    }
}
$(document).off("click", ".view-item-edit");
$(document).on("click", ".view-item-edit", function () {
    $(this).parent().closest('ul').parent().addClass('open');
    var _targetid = $(this).data('targetid');
    var _itemId = $(this).data('id');
    $("#view-item-name-" + _itemId).hide();
    $(this).hide();
    $(_targetid).show();
});

$(document).off("click", ".dropdown-menu");
$(document).on("click", ".dropdown-menu", function (event) { event.stopPropagation(); });

$(document).off("click", "#popoverclose");
$(document).on("click", "#popoverclose", function () { $(".z-manage-filter").removeClass("active-manage-filter"); $(".popovercontent").popover('hide'); });

$(document).off("click", ".view-item-save");
$(document).on("click", ".view-item-save", function () {
    var _itemId = $(this).data('id');
    var _newText = $(this).parent().find("#view_item_" + _itemId).val();
    if ($.trim(_newText) !== "") {
        DynamicGrid.prototype.CreateNewView(this);
        var _targetid = $(this).data('targetid');
        if (_itemId != "0")
            $(_targetid).hide();
        $("#view-item-name-" + _itemId).text(_newText);
        $("#view-display-selected").text(_newText);
        $("#view-item-edit-" + _itemId).show();
        $("#view-item-name-" + _itemId).show();
    }
    else {
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper("Please enter valid view name.", 'error', isFadeOut, fadeOutTime);
    }
});
$(document).off("click", ".view-item-option");
$(document).on("click", ".view-item-option", function () {
    var _text = $(this).data('text');
    var _itemId = $(this).data('id');
    $("#view-display-selected").text(_text);
    DynamicGrid.prototype.GetView(this);
});

$(document).ready(function () {
    var _dynamicGrid: DynamicGrid;
    _dynamicGrid = new DynamicGrid(window.document);
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

$(document).off("click", ".view-item-delete");
$(document).on("click", ".view-item-delete", function () {
    DynamicGrid.prototype.DeleteView(this);
});

$(document).off("click", ".default-item-option");
$(document).on("click", ".default-item-option", function () {
    DynamicGrid.prototype.SetDefaultView(this);
});
$(document).off("click", ".public-checkbox");
$(document).on("click", ".public-checkbox", function () {
    DynamicGrid.prototype.SetCheckboxValue(this);
});


$(document).off("click", "#grid tbody tr");
$(document).on("click", "#grid tbody tr", function (event) {
    var _node = event.target.className;
    if (_node !== "z-publish" && _node !== "z-edit" && _node !== "z-delete" && _node !== "z-preview" && _node !== "z-copy" && _node !== "z-manage" && _node !== "grid-row-checkbox " && _node !== "grid-row-checkbox disabled-checkbox" && _node !== "z-enable" && _node !== "z-disable")
        $(this).find("input[type=checkbox]").click();
});
$("html").on("click", function (e) {
    var _target = e.target;
    if ($(_target).closest('.dropdown-tool')[0] == undefined || $(_target).closest('a').hasClass('btn-dropdown')) {
        if ($(".dropdown-tool").hasClass('open')) {
            $('[data-btn-edit]').hide();
            $('.view-item-edit').show();
            $('.save-view-link').show();
        }
    }
});

$(document).off("keypress", "#searchform")
$(document).on("keypress", "#searchform", function (evt) {  
    var _target = evt.target;
    if (evt.keyCode == 13) {
        evt.preventDefault();
        if ($(event.target).val().length > 0) {
            DynamicGrid.prototype.FilterButtonPress(_target);
        }
    }
});
