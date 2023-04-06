
var XmlGenerator = {
    FormatXml: function (xml) {
        var formatted = '';
        var reg = /(>)(<)(\/*)/g;
        xml = xml.replace(reg, '$1\r\n$2$3');
        var pad = 0;
        jQuery.each(xml.split('\r\n'), function (index, node) {
            var indent = 0;
            if (node.match(/.+<\/\w[^>]*>$/)) {
                indent = 0;
            } else if (node.match(/^<\/\w/)) {
                if (pad != 0) {
                    pad -= 1;
                }
            } else if (node.match(/^<\w[^>]*[^\/]>.*$/)) {
                indent = 1;
            } else {
                indent = 0;
            }

            var padding = '';
            for (var i = 0; i < pad; i++) {
                padding += '  ';
            }

            formatted += padding + node + '\r\n';
            pad += indent;
        });
        return formatted;
    },
    ViewXml: function (id) {
        var iscompressed = $(this).data("iscompressed");
        Endpoint.prototype.GetXml(id, function (response) {
            var mydiv = document.createElement('div');
            xmlstring = XmlGenerator.FormatXml(response);
            var d = document.createElement('div');
            var t = document.createTextNode(xmlstring);
            d.appendChild(t);
            $('#xmlTextDiv').empty().append('<pre>' + d.innerHTML + '</pre>');
            $("#vieXmlDialog").dialog({
                title: "View XML",
                resizable: false,
                modal: true,
                create: function () { $(this).closest(".ui-dialog").addClass("ui-lg-popup add-new-customer"); }
            });
            return false;
        });
    },

    GetColumnList: function () {
        var entityName = $("#txtEntityName").val().trim();
        if (entityName != "") {
            var entityType = $("#entityTypeList").val();
            Endpoint.prototype.GetColumnsList(entityType, entityName, columnListJson, function (data) {
                $("#columnListDiv").empty().html(data);
                XmlGenerator.EditModeSettings();
                $("#columnListDiv").show();
            });
        }
        else {
            $('#entityNameErrMsg').text(entiryNameRequiredMsg);
        }
    },
    EditModeSettings: function () {
        if (viewMode.trim() != '') {
            if (viewMode == "Edit") {
                $("#txtEntityName").attr("disabled", "disabled");
                $("#entityTypeList").attr("disabled", "disabled");
                $("#btnGridColumn").html('Update Grid Column');
            }
            else {
                $("#txtEntityName").prop("disabled", false);
                $("#entityTypeList").prop("disabled", false);
                $("#btnGridColumn").html('Generate Grid Column');
            }
        }
    },
    CrateAutoComplete: function () {
        $("#txtEntityName").autocomplete({
            source: function (request, response) {
                try {
                    Endpoint.prototype.GetEntityName(request.term, $("#entityTypeList").val(), function (data) {
                        response($.map(data, function (item) {
                            return { label: item.Text, value: item.Text };
                        }));
                    });
                } catch (err) {
                }
            },
            select: function (event, ui) {
                $('#txtEntityName').val(ui.item.label);
                overlayforwaitnone();
                return false;
            },
            search: function () {

            },
            messages: {
                noResults: "", results: ""
            }
        });
    },
    SaveValidation: function () {
        if ($('#txtViewOptions').val().trim() != '' && $('#txtPageName').val().trim() != '' && $('#txtObjectName').val().trim() != '' && $("#txtEntityName").val().trim() != '') {
            return true;
        }
        else {
            if ($('#txtViewOptions').val().trim() == '') {
                $('#viewOprionErrMsg').text(displayOtionReqMsg);
            }
            if ($('#txtPageName').val().trim() == '') {
                $('#viewPageErrMsg').text(pageNameReqMsg);
            }
            if ($('#txtObjectName').val().trim() == '') {
                $('#viewobjectErrMsg').text(objectNameReqMsg);
            }
            if ($('#txtEntityName').val().trim() == '') {
                $('#entityNameErrMsg').text(entiryNameRequiredMsg);
            }
            return false;
        }
    },
    SaveXML: function (url) {
        var _griddata = XmlGenerator.GridTojson();
        var txtviewOptions = $('#txtViewOptions').val().trim();
        var txtfrontPageName = $('#txtPageName').val().trim();
        var txtfrontObjectName = $('#txtObjectName').val().trim();
        var entityType = $("#entityTypeList").val();
        var entityName = $('#txtEntityName').val().trim();

        if (XmlGenerator.SaveValidation(txtviewOptions, txtfrontPageName, txtfrontObjectName)) {
            Endpoint.prototype.SaveXmlData(url, _griddata, txtviewOptions, entityType, entityName, txtfrontPageName, txtfrontObjectName, id, function (response) {
                if (response != "") {
                    if (response.IsSuccess == true) {
                        window.location.href = "/XMLGenerator/List";

                        var notification = new Notification(); notification.DisplayNotificationMessagesHelper(data.message, "success", false, 0);

                    }
                    else {
                        var notification = new Notification(); notification.DisplayNotificationMessagesHelper(data.message, "error", false, 0);

                    }
                }
            });
        }
    },
    GridTojson: function () {
        var json = '';
        var otArr = [];
        var id = 0;

        var tbl2 = $('#gridXML tbody tr').each(function (i) {
            if ($(this)[0].rowIndex != 0) {
                x = $(this).children();
                var itArr = [];
                var dumy = [];

                var obj = {};
                id = id + 1;
                obj['Id'] = id;

                var name = x.find("#lblName").text();
                obj['Name'] = name;

                var header = x.find("#headerText").val();
                obj['HeaderText'] = header;

                var width = x.find("#widthText").val();
                obj['Width'] = width;

                var dataTypeText = x.find("#dataTypeText option:selected").text();
                obj['Datatype'] = dataTypeText;

                var columnType = x.find("#columnType option:selected").text();
                obj['Columntype'] = columnType;

                var isSort = x.find("#chkallowsorting").is(":checked");
                obj['Allowsorting'] = isSort;

                var isPage = x.find("#chkAllowpaging").is(":checked");
                obj['Allowpaging'] = isPage;

                var formatText = x.find("#formatText").val();
                obj['Format'] = formatText;

                var chkVisible = x.find("#chkVisible option:selected").text();

                obj['Isvisible'] = chkVisible;

                var chkMustshow = x.find("#chkMustshow option:selected").text();
                obj['Mustshow'] = chkMustshow;

                var chkMustHide = x.find("#chkMustHide option:selected").text();
                obj['Musthide'] = chkMustHide;

                var maxLength = x.find("#maxLength").val();
                obj['Maxlength'] = maxLength;

                var chkIsallowsearch = x.find("#chkIsallowsearch option:selected").text();
                obj['Isallowsearch'] = chkIsallowsearch;

                var chkIsconditional = x.find("#chkIsconditional option:selected").text();
                obj['Isconditional'] = chkIsconditional;

                var chkIsallowlink = x.find("#chkIsallowlink option:selected").text();
                obj['Isallowlink'] = chkIsallowlink;

                var chkIslinkactionurl = x.find("#chkIslinkactionurl").val();
                obj['Islinkactionurl'] = chkIslinkactionurl;

                var chkIslinkparamfield = x.find("#chkIslinkparamfield").val();
                obj['Islinkparamfield'] = chkIslinkparamfield;

                var chkIscheckbox = x.find("#chkIscheckbox option:selected").text();
                obj['Ischeckbox'] = chkIscheckbox;

                var chkCheckboxparamfield = x.find("#chkCheckboxparamfield").val();
                obj['Checkboxparamfield'] = chkCheckboxparamfield;

                var chkIsControl = x.find("#chkIsControl option:selected").text();
                obj['Iscontrol'] = chkIsControl;

                var controlType = x.find("#controlType option:selected").val();
                obj['Controltype'] = controlType;

                var controlParamField = x.find("#controlParamField").val();
                obj['Controlparamfield'] = controlParamField;

                var displayText = x.find("#displayText").val();
                obj['Displaytext'] = displayText;

                var editActionUrl = x.find("#editActionUrl").val();
                obj['Editactionurl'] = editActionUrl;

                var editParamField = x.find("#editParamField").val();//

                obj['Editparamfield'] = editParamField;

                var deleteActionUrl = x.find("#deleteActionUrl").val();
                obj['Deleteactionurl'] = deleteActionUrl;

                var deleteParamField = x.find("#deleteParamField").val();
                obj['Deleteparamfield'] = deleteParamField;

                var manageActionUrl = x.find("#manageActionUrl").val();
                obj['Manageactionurl'] = manageActionUrl;

                var manageParamField = x.find("#manageParamField").val();
                obj['Manageparamfield'] = manageParamField;

                var viewActionUrl = x.find("#viewActionUrl").val();
                obj['Viewactionurl'] = viewActionUrl;

                var viewParamField = x.find("#viewParamField").val();
                obj['Viewparamfield'] = viewParamField;

                var imageActionUrl = x.find("#imageActionUrl").val();
                obj['Imageactionurl'] = imageActionUrl;

                var imageParamField = x.find("#imageParamField").val();
                obj['Imageparamfield'] = imageParamField;

                var copyActionUrl = x.find("#copyActionUrl").val();
                obj['Copyactionurl'] = copyActionUrl;

                var copyParamField = x.find("#copyParamField").val();
                obj['Copyparamfield'] = copyParamField;

                var xAxis = x.find("#xAxis option:selected").text();
                obj['XAxis'] = xAxis;

                var yAxis = x.find("#yAxis option:selected").text();
                obj['YAxis'] = yAxis;

                var yAxis = x.find("#IsAdvanceSearch option:selected").text();
                obj['IsAdvanceSearch'] = yAxis;

                var classText = x.find("#ClassText").val();
                obj['Class'] = classText;

                var dbParamField = x.find("#dbParamField").val();
                obj['DbParamField'] = dbParamField;

                var SearchControlType = x.find("#SearchControlType option:selected").text();
                obj['SearchControlType'] = SearchControlType;

                var SearchControlParameters = x.find("#SearchControlParameters").val();
                obj['SearchControlParameters'] = SearchControlParameters;

                var IsGraph = x.find("#ddlIsGraph").val();
                obj['IsGraph'] = IsGraph;

                var useMode = x.find("#useMode option:selected").val();
                obj['UseMode'] = useMode;

                var chkIsallowDetailView = x.find("#chkIsallowDetailView option:selected").val();
                obj['AllowDetailView'] = chkIsallowDetailView;

                otArr.push(obj);
            }
        })
        json = JSON.stringify(otArr);
        return json;
    }
}


function isNumberWithDotKey(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode != 46 && charCode > 31
      && (charCode < 48 || charCode > 57))
        return false;
    var parts = $(evt.target).val().split('.');
    if (parts.length > 1 && charCode == 46) return false;
    return true;
}

function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode == 46) return false;
    if (charCode > 31
      && (charCode < 48 || charCode > 57))
        return false;
    return true;
}

$(document).on("click", ".zf-view", function (e) {
    e.preventDefault();
    var id = $(this).attr("href").split('/')[2];
    XmlGenerator.ViewXml(id);
})