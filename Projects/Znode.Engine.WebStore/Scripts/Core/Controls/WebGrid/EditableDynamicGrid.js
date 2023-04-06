function AddNewRowManage() {
    $("#Dynamic_Grid").show()
}

function isNumberKey(t) {
    var e = t.which ? t.which : event.keyCode;
    return 46 == e ? !1 : e > 31 && (48 > e || e > 57) ? !1 : !0
}

function DgUpdateString(t) {
    var e = "",
        i = [],
        n = {},
        a = !0,
        d = $(t).data("parameter"),
        o = $(t).closest("td").parent("tr");
    d = d.substring(1, d.length);
    d = d.split("&");
    for (var arrindex = 0; arrindex < d.length; arrindex++) {
        var _item = d[arrindex].split("=");
        n[_item[0]] = _item[1];
    }
    $(o).find("[data-dgview='edit']").each(function (t, e) {
        $(this).parents("td").find("[data-dgview='edit']").removeAttr("style"), !$(this).parents("td").hasClass("IsRequired") || "" != $(this).parent("td").find(".input-text").val() && 0 != $(this).find("option:selected").val() ? $(this).parents("td").hasClass("Password") && "" != $(this).val() && $(this).val().length < 6 ? ($(this).parents("td").find("[data-dgview='edit']").attr("style", "border-color:red"), $(this).parents("td").find("[data-dgview='edit']").attr("title", PasswordInstructionMessage), a = !1) : (columnName = $(this).data("columnname"), n[columnName] = $.trim($(this).val())) : ($(this).parents("td").find("[data-dgview='edit']").attr("style", "border-color:red"), a = !1)
    });
    var s = null;
    $(o).find(".chk-btn").each(function (t, e) {
        s = $(e), n[s.attr("name")] = s.is(":checked")
    }), s = null, -1 != window.location.href.indexOf("filter=MasterUserDetails") && $(o).find("[data-columnname='ShortCode']").length > 0 && $(o).find("[data-columnname='ShortCode']").each(function (t, e) {
        s = $(e), n[s.attr("data-columnname")] = s.html()
    }), i.push(n), e = JSON.stringify(i);
    var r = jQuery.Event(ListConstants.ROW_EDIT);
    return r.detail = {
        data: e,
        linkId: t
    }, a ? ($(document).trigger(r), DgCallAjax(e, t, function (t) {
        Notification.prototype.DisplayNotificationMessagesHelper(t.message, t.status ? "success" : "error", isFadeOut, fadeOutTime)
    }), e) : a
}

function DgCallAjax(t, e, i) {
    var n = $(e).attr("href");
    ZnodeBase.prototype.ajaxRequest(n, "get", {
        data: t
    }, i, "json")
}

function DgUpdateSuccess(t) {
    var e = $(t).closest("td").parent("tr");
    $(e).find("[data-dgview='edit']").each(function (t, e) {
        "dropDown" == $(this).attr("class") ? $(this).prev("[data-dgview='show']").text($(this).children("option:selected").text()) : $(this).prev("[data-dgview='show']").text($(this).val())
    }), e.find("[data-dgview='show']").show(), e.find("[data-dgview='edit']").hide(), e.find("[data-disable='checkbox']").attr("disabled", "disabled"), e.find("[data-managelink='Cancel']").parent("li").remove(), e.find("[data-managelink='Update']").parent("li").remove(), e.find("[data-managelink='Edit']").parent("li").show()
}

function DgUpdateAllSuccess() {
    var t = $("#dynamicGrid");
    $(t).find("[data-dgview='edit']").each(function (t, e) {
        "dropDown" == $(this).attr("class") ? $(this).prev("[data-dgview='show']").text($(this).children("option:selected").text()) : $(this).prev("[data-dgview='show']").text($(this).val())
    }), t.find("[data-dgview='show']").show(), t.find("[data-dgview='edit']").hide(), t.find(".fileUploader").removeClass("display-block").addClass("display-none"), $("#btnEditAll").show(), $("#btnCancelAll").hide(), $("#btnUpdateAll").hide()
}

function DgDeleteRow(t) {
    var e = "",
        i = [],
        n = {},
        a = $(t).data("parameter").split("="),
        d = $(t).parent("li").parent("ul").parent("div").parent("div").parent("td").parent("tr");
    n.Id = a[1], $(d).find("[data-dgview='edit']").each(function (t, e) {
        columnName = $(this).data("columnname"), n[columnName] = $.trim($(this).val())
    });
    var o = null;
    $(d).find(".chk-btn").each(function (t, e) {
        o = $(e), n[o.attr("name")] = o.is(":checked")
    }), i.push(n), e = JSON.stringify(i);
    var s = jQuery.Event(ListConstants.ROW_DELETE);
    return s.detail = {
        data: e,
        linkId: t
    }, $(document).trigger(s), e
}
var rowCount = 0,
    EditableGridEvent = {
        Init: function () {
            $(document).on("click", "#btnAddRow", function () {
                $("#txtRowCount").css("display", "block"), $("#lblRowCount").css("display", "block"), $("#btnSaveRowData").css("display", "block"), $("#gridAddNewRowDynamicDiv").css("display", "block"), $("[data-backbutton='back']").is(":visible") || $("#CancelRowAdd").css("display", "block"), $("#Dynamic_Grid").hide(), $("#SearchComponent").hide(), $(".HideShowComponent").show(), $("#editAllDiv").hide();
                var t = $("#gridAddNewRow tbody tr").html();
                gridHtml = void 0 == t ? gridHtml : "<tr>" + t + "</tr>";
                var e = rowCount;
                for (0 == e && (e = 1, $("#gridAddNewRow tbody tr").each(function () {
                        $(this).closest("tr").remove()
                }), rowCount = e), i = 0; i < e; i++) $("#gridAddNewRow").append(gridHtml), $("#gridAddNewRow").find(".fileUploader").removeClass("display-none").addClass("display-block");
                $("#gridAddNewRowDynamicDiv").find("[data-deletenewrow='newrow']").parents("td").removeClass("display-none"), $("#gridAddNewRowDynamicDiv").find("[data-columnname='Delete']").removeClass("display-none").parents("th").removeClass("display-none"), -1 != window.location.href.indexOf("filter=City") && $("[data-city='city']").hide(), -1 != window.location.href.indexOf("filter=MasterUserDetails") && ($("[data-regorg='regorg']").show(), $("#gridAddNewRow tbody tr").find("select[data-columnname='ApprovalId']").attr("disabled", "disabled"))
            }), $(document).on("click", "#CancelRowAdd", function () {
                $("#gridAddNewRow").closest("tr").remove(), $("#gridAddNewRow tbody tr").each(function () {
                    $(this).closest("tr").remove()
                }), $("#txtRowCount").val(""), $("#txtRowCount").css("display", "none"), $("#lblRowCount").css("display", "none"), $("#btnSaveRowData").css("display", "none"), $("#gridAddNewRowDynamicDiv").css("display", "none"), $("#CancelRowAdd").css("display", "none"), $("#Dynamic_Grid").show(), $("#SearchComponent").show(), $(".HideShowComponent").hide(), $("#editAllDiv").show(), -1 != window.location.href.indexOf("filter=MasterUserDetails") && ($("[data-regorg='regorg']").hide(), $("#grid-container").show());
                try {
                    $("#AddNewGridAlignment").addClass("pull-right"), $("#ChildAddNewGridAlignment").removeAttr("style")
                } catch (t) { }
            }), $(document).on("click", "#btnSaveRowData", function () {
                return EditableGrid.GetNewRowJson(this)
            }), $(document).on("click", "#btnUpdateAll", function () {
                return EditableGrid.GetAllRowJson(ListConstants.ROW_EDITALL)
            }), $(document).on("click", "#btnDeleteAll", function () {
                var t = void 0 == DeleteAllRowConfirmationMessage ? "Are you sure you want to delete all rows?" : DeleteAllRowConfirmationMessage;
                bootbox.confirm(t, function (t) {
                    return t ? EditableGrid.GetAllRowJson(ListConstants.ROW_DELETEALL) : void 0
                })
            }), $(document).on("click", "[data-deletenewrow='newrow']", function () {
                $("#gridAddNewRowDynamicDiv tbody tr").length > 1 && $(this).parent("td").parent("tr").remove()
            }), $(document).off("click", "[data-managelink='Edit']"), $(document).on("click", "[data-managelink='Edit']", function (t) {
                t.preventDefault();
                var e = $(this).closest("td").parent("tr");
                e.find("[data-dgview='show']").hide(), e.find("[data-dgview='edit']").show(), e.find("[data-disable='checkbox']").prop("disabled", false);
                var i = $(this).data("parameter"),
                    n = $(this).attr("href"),
                    a = '<li><a class="zf-update zf-ok" href="' + n + '" data-parameter="' + i + '" data-managelink="Update" title="Update"></a></li><li><a class="z-cancel z-close" href="' + n + '" data-parameter="' + i + '" data-managelink="Cancel" title="Cancel"></a></li>';
                $(this).closest("ul").prepend(a), e.find("[data-managelink='Edit']").parent("li").hide();
                var d = jQuery.Event(ListConstants.ROW_PrevEDIT);
                return d.detail = {
                    linkId: $(this)
                }, $(document).trigger(d), !1
            }), $(document).off("click", "[data-managelink='Cancel'],[data-managelink='Clear']"), $(document).on("click", "[data-managelink='Cancel'],[data-managelink='Clear']", function (t) {
                t.preventDefault();
                var e = $(this).closest("td").parent("tr");
                e.find("[data-dgview='show']").show(), e.find("[data-dgview='edit']").hide(), e.find("[data-disable='checkbox']").attr("disabled", "disabled"), e.find(".fileUploader").removeClass("display-block").addClass("display-none"), e.find("[data-managelink='Cancel']").parent("li").remove(), e.find("[data-managelink='Update']").parent("li").remove(), e.find("[data-managelink='Edit']").parent("li").show();
                var i = jQuery.Event(ListConstants.ROW_Cancel);
                return i.detail = {
                    linkId: $(this)
                }, $(document).trigger(i), !1
            }), $(document).off("click", "[data-managelink='Update']"), $(document).on("click", "[data-managelink='Update']", function (t) {
                t.preventDefault();
                var e = DgUpdateString(this);
                return e ? (DgUpdateSuccess(this), !1) : !1
            })
        }
    },
    EditableGrid = {
        GetNewRowJson: function (t) {
            var e = "",
                i = [],
                n = 1;
            $("#gridAddNewRow tbody tr").each(function (t) {
                if (0 != $(this)[0].rowIndex) {
                    x = $(this).children();
                    var e = {};
                    e.Id = n, $("td", this).each(function (t) {
                        var i = "";
                        $(this).find(".radio-btn").length > 0 && (i = $(this).find(".radio-btn").data("columnname"), e[i] = $(this).find(".radio-btn").is(":checked")), $(this).find(".chk-btn").length > 0 && (i = $(this).find(".chk-btn").data("columnname"), e[i] = $(this).find(".chk-btn").is(":checked")), $(this).find(".input-text").length > 0 && (i = $(this).find(".input-text").data("columnname"), e[i] = $.trim($(this).find(".input-text").val()), $(this).find(".input-text").parent("td").hasClass("IsRequired") && "" == $(this).find(".input-text").val() ? $(this).find(".input-text").attr("style", "border-color:red") : $(this).find(".input-text").removeAttr("style"), $(this).find(".input-text").parent("td").hasClass("Password") && $(this).find(".input-text").val().length < 6 && ($(this).find(".input-text").attr("title", PasswordInstructionMessage), $(this).find(".input-text").attr("style", "border-color:red"))), $(this).find("[type='hidden']").length > 0 && (i = $(this).find("[type='hidden']").data("columnname"), e[i] = $.trim($(this).find("[type='hidden']").val())), $(this).find(".dropDown").length > 0 && (i = $(this).find(".dropDown").data("columnname"), e[i] = $.trim($(this).find(".dropDown").val()), $(this).find(".dropDown").parent("td").hasClass("IsRequired") && "0" == $(this).find(".dropDown").val() ? $(this).find(".dropDown").attr("style", "border-color:red") : $(this).find(".dropDown").removeAttr("style"))
                    }), i.push(e), n = parseInt(n) + 1
                }
            }), e = JSON.stringify(i);
            var a = jQuery.Event(ListConstants.ROW_ADDED);
            return a.detail = {
                data: e,
                linkId: t
            }, $(document).trigger(a), e
        },
        GetAllRowJson: function (t) {
            var n = "",
        i = [];
            $("#dynamicGrid tbody tr").each(function (t) {
                if (0 != $(this)[0].rowIndex) {
                    x = $(this).children();
                    var n = {},
                        d = x.find("[data-managelink='Update']");
                    d = d.substring(1, d.length);
                    for (var a, e = $(d).data("parameter").split("&"), h = 0; h < e.length; h++) {
                        e[h];
                        a = h.split("="), n[a[0]] = a[1]
                    }
                    $("td", this).each(function (t) {
                        var i;
                        $(this).find(".radio-btn").length > 0 && (i = $(this).find(".radio-btn").data("columnname"), n[i] = $(this).find(".radio-btn").is(":checked")), $(this).find(".chk-btn").length > 0 && (i = $(this).find(".chk-btn").data("columnname"), n[i] = $(this).find(".chk-btn").is(":checked")), $(this).find(".input-text").length > 0 && (i = $(this).find(".input-text").data("columnname"), n[i] = $(this).find(".input-text").val()), $(this).find("[type='hidden']").length > 0 && (i = $(this).find("[type='hidden']").data("columnname"), n[i] = $(this).find("[type='hidden']").val()), $(this).find(".dropDown").length > 0 && (i = $(this).find(".dropDown").data("columnname"), n[i] = $(this).find(".dropDown").val())
                    }), i.push(n)
                }
            }), n = JSON.stringify(i);
            var d = jQuery.Event(t);
            return d.detail = {
                data: n,
                linkId: t
            }, $(document).trigger(d), n
        }
    };
ListConstants = {
    ROW_ADDED: "ROW_ADDED",
    ROW_EDIT: "ROW_EDIT",
    ROW_Cancel: "ROW_Cancel",
    ROW_PrevEDIT: "ROW_PrevEDIT",
    ROW_EDITALL: "ROW_EDITALL",
    ROW_DELETE: "ROW_DELETE",
    ROW_DELETEALL: "ROW_DELETEALL"
};