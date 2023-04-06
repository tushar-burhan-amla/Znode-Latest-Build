var controlContext;
import MultiSelectModel = Znode.Core.MultiSelectDDlModel;
class MultiSelectDDL extends ZnodeBase {
    _endpoint: Endpoint;
    _MultiSelectModel: MultiSelectModel;
    _Itemdata: Array<string>;

    constructor(doc: HTMLDocument) {
        super();
        this._endpoint = new Endpoint();
        MultiSelectDDL.prototype._Itemdata = new Array<string>();
    }

    BindSearch() {
        $('.ms-search input').on('keyup', function () {
            // ignore keystrokes that don't make a difference
            if ($(this).data('lastsearch') == $(this).val()) {
                return true;
            }
            // search non optgroup li's
            var instance = $('.dropdown');
            var optionsList = instance.find('ul')
            var search = $(this).val();

            optionsList.find('li:not(.optgroup)').each(function () {
                var optText = $(this).text();

                // show option if string exists
                if (optText.toLowerCase().indexOf(search.toLowerCase()) > -1) {
                    $(this).show();
                }
                // don't hide selected items
                else if (!$(this).hasClass('selected')) {
                    $(this).hide();
                }
            });
        });

        $(document).off("click", "div.dropdown");
        $(document).on("click", "div.dropdown", function () {
            $(this).addClass("open");
        })
    }

    GetRecord(IsAjax, Controller, Action, ParentMenuIds, SuccessCallBack, flag,target): void {
        if (IsAjax) {
            $.ajax({
                url: "/" + Controller + "/" + Action,
                data: { id: ParentMenuIds.toString(), flag: flag },
                method: "GET",
                dataType: "json",
                success: function (data) {
                    ZnodeBase.prototype.executeFunctionByName(SuccessCallBack, window, data, target);
                },
                error: function (data) {
                    console.log(data);
                }
            });
        }
    }

    CheckBoxChecked(control: any, hiddenItem: string): void {
        controlContext = control;
        var _IsSubmit = $(control).data("issubmit");
        var _IsSuboption = $(control).data("issuboption")
        if (_IsSubmit.toLowerCase() === "false") {
            MultiSelectDDL.prototype._Itemdata = [];
            $(control).parent().parent().parent().parent().find('input[type=checkbox]').each(function () {
                if ($(this).is(':checked')) {
                    MultiSelectDDL.prototype._Itemdata.push($(this).val());
                }
                else {
                    var index = MultiSelectDDL.prototype._Itemdata.indexOf($(this).val());
                    if (index != -1) {
                        MultiSelectDDL.prototype._Itemdata.splice(index, 1);
                    }
                }

            });
            var _Value = $(control).val();
            var _Controller = $(control).data("controller");
            var _Action = $(control).data("action");
            var _SuccessCallBack = $(control).data("sucess");
            var _IsMultiple = $(control).data("ismultiple");
            var _flag = true;

            if ($(control).is(':checked')) {
                if (_IsMultiple == "True") {
                    if ($.inArray(_Value, MultiSelectDDL.prototype._Itemdata) == -1) {
                        MultiSelectDDL.prototype._Itemdata.push(_Value);
                    }
                }
                else {
                    MultiSelectDDL.prototype._Itemdata = [];
                    MultiSelectDDL.prototype._Itemdata.push(_Value);
                }
            }
            else {
                if (_IsMultiple == "True") {
                    MultiSelectDDL.prototype._Itemdata = jQuery.grep(MultiSelectDDL.prototype._Itemdata, function (value) {
                        return value != _Value;
                    });
                }
                else {
                    MultiSelectDDL.prototype._Itemdata = [];
                    MultiSelectDDL.prototype._Itemdata.push(_Value);
                }
                _flag = false;
            }

            this.GetRecord(true, _Controller, _Action, MultiSelectDDL.prototype._Itemdata, _SuccessCallBack, _flag,control);
        }
        else if (typeof _IsSuboption != 'undefined' && _IsSuboption.toLowerCase() == "true") {
            var _ulId = $(control).val();
            var a = $("#" + _ulId).is(':checked');
            if ($("#" + _ulId).is(':checked')) {
                $(control).parent().parent().parent().parent().find('#optgroup-' + _ulId + ' input[type=checkbox]').each(function () {
                    this.checked = true;
                });
            }
            else {
                $(control).parent().parent().parent().parent().find('#optgroup-' + _ulId + ' input[type=checkbox]').each(function () {
                    this.checked = false;
                });
            }
        }

        if (hiddenItem) {
            $("#" + hiddenItem).val("");
            $("#" + hiddenItem).val(MultiSelectDDL.prototype._Itemdata);
        }

    }

    CheckAll(control: any, hiddenItem: string): void {
        if ($("#selectall").is(':checked')) {
            $(control).parent().parent().parent().parent().find('input[type=checkbox]').each(function () {
                this.checked = true;
                $('#selectall').prop('checked')
            });

            MultiSelectDDL.prototype._Itemdata = [];
            $(control).parent().parent().parent().parent().find('input[type=checkbox]:checked').each(function () {
                MultiSelectDDL.prototype._Itemdata.push($(this).val());
            });
        }
        else {
            $(control).parent().parent().parent().parent().find('input[type=checkbox]').each(function () {
                this.checked = false;
            });
        }
        if (hiddenItem) {
            $("#" + hiddenItem).val("");
            $("#" + hiddenItem).val(MultiSelectDDL.prototype._Itemdata);
        }
    }

    SubmitData(control: any): void {
        var _Submitdata = new Array();
        var _Id = $(control).data("ddlid");
        var tabId = $("#tabs .ui-tabs-active").attr("aria-labelledby");
        $(control).parent().parent().parent().parent().find('#' + _Id + ' input[type=checkbox]').each(function () {
            if ($(this).is(':checked')) {
                _Submitdata.push($(this).val());
            }
        });
        var _Controller = $(control).data("controller");
        var _Action = $(control).data("action");

        var ids = _Submitdata.join(", ");
        $.ajax({
            url: "/" + _Controller + "/" + _Action,
            data: { selectedIds: ids },
            method: "GET",
            dataType: "json",
            success: function (data) {
                var tabName = $("#tabs .ui-tabs-active").attr("aria-controls");
                if (typeof tabId != 'undefined') {
                    if (tabName.indexOf("ui-id") > -1) {
                        var current_index = $("#tabs").tabs("option", "active");
                        $("#tabs").tabs('load', current_index);
                    }
                    else {
                        var controller = $("#" + tabName).attr("data-controller");
                        var action = $("#" + tabName).attr("data-method");
                        var paramname = $("#" + tabName).attr("data-parameter");
                        var paramvalue = $("#" + tabName).attr("data-paramvalue");
                        var values = {};
                        values[paramname] = paramvalue;
                        $.ajax({
                            url: "/" + controller + "/" + action,
                            data: values,
                            method: "GET",
                            success: function (response) {
                                $("#" + tabName).html('');
                                $("#" + tabName).html(response);
                            }
                        });
                    }
                    //if (data.HasNoError)
                    //    Notification.prototype.DisplayNotificationMessagesHelper(data.Message, 'success', true, fadeOutTime);
                    //else
                    //    Notification.prototype.DisplayNotificationMessagesHelper(data.Message, 'error', true, fadeOutTime);
                }
                else {
                    location.reload();
                }

            },
            error: function (data) {
                console.log(data);
            }
        });
    }

    SortSuccesssCallback() {
        GridPager.prototype.SelectedPageSize(controlContext);
    }

    SortColumn(id, enable, controller, Action) {
        $("#" + id).sortable({
            // enable: enable,
            axis: "y",
            cursor: "move",
            containment: "#" + id,
            stop: function (event, ui) {
                controlContext = event.target;
                var ids = [];
                $("#" + id).find("li").each(function () {
                    ids.push(parseInt($(this).find(".btncheckbox").data("value")));
                });
                if (enable) {
                    $.ajax({
                        url: "/" + controller + "/" + Action,
                        method: "GET",
                        data: { id: ids.toString() },
                        dataType: "json",
                        success: function (data) {
                            ZnodeBase.prototype.executeFunctionByName("MultiSelectDDL.prototype.SortSuccesssCallback", window, data);
                        },
                        error: function (data) {
                            console.log(data);
                        }
                    });
                }
            }
        }).disableSelection();
    }
}


