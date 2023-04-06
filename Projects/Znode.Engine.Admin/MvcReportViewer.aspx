    <%@ Page Language="C#" AutoEventWireup="true" Inherits="MvcReportViewer.MvcReportViewer, MvcReportViewer" Culture="en-US" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <script src="Scripts/References/jquery-3.3.1.min.js"></script>
    <style type="text/css">
        .report-viewer-table .report-parameter-table {background:#f4f4f4 !important; border:2px solid #c3c3c3 !important; border-top:0 !important;} 
        .report-viewer-table #ParametersRowReportViewer {display:block;}
        .report-viewer-table input[type="text"] {background:#fff !important; color:#606d7a; border:1px solid #c3c3c3; border-radius:2px; height:20px; line-height:20px; padding:0 5px;}
        .report-viewer-table select{color:#606d7a;font-size:12px; height:24px;min-width:100%; border:1px solid #c3c3c3; border-radius:2px; outline:none; background:#fff; cursor:pointer;}
        .report-viewer-table td.report-viewer-toggle {display:none;}
        .report-viewer-table input[type="submit"] {background-color:#891e17; color:#fff; border:0; cursor:pointer; padding:0 15px; height:24px; line-height:23px; min-width:90px; text-align:center; font-size:12px !important; font-family:segoeui, HelveticaNeueLTStd Lt, Arial, sans-serif !important; border-radius:2px;}
        .report-viewer-table input[type="submit"]:hover {background-color:#5b6770;}
        .report-viewer-table .report-viewer-control {background:#fff !important; padding:7px 0 5px; margin:0 0 10px;}
        .report-viewer-table .report-viewer-control-btn, .report-viewer-table .report-viewer-control-btn:hover, .report-viewer-table .report-viewer-refresh-btn:hover, .report-viewer-table .report-viewer-print-btn:hover {background:#f5f5f5 !important; border-color:#c3c3c3 !important;}
        .report-viewer-control-menu {background:#fff !important; border:1px solid #c3c3c3 !important;}
        .report-viewer-control-menu > div {border:0 !important;}
        .report-viewer-control-menu a {background:#fff; padding:4px 8px !important; color:#606d7a !important; outline:0;}
        .report-viewer-control-menu > div:hover {border:0 !important;}
        .report-viewer-control-menu > div a:hover {color:#262626 !important; background:#f5f5f5;}
        .report-viewer-asyncwait {background-color:#fff !important; border:1px solid #f1f1f1 !important; color:#5b6770;}
        .report-viewer-asyncwait a {color:#5b6770 !important; text-decoration:none;}
    </style>
</head>
<body>
    <form id="reportForm" runat="server">
        <div>
            <asp:ScriptManager runat="server" ID="ScriptManager"></asp:ScriptManager>
            <rsweb:ReportViewer ID="ReportViewer" runat="server" PageCountMode="Actual" ShowExportControls="true" ShowRefreshButton="true"></rsweb:ReportViewer>
        </div>
    </form>
    <script type="text/html" id="non-ie-print-button">
        <div class="" style="font-family: Verdana; font-size: 8pt; vertical-align: top; display: inline-block; width: 28px; margin-left: 6px;">
            <table style="display: inline;" cellspacing="0" cellpadding="0">
                <tbody>
                    <tr>
                        <td height="28">
                            <div>
                                <div id="mvcreportviewer-btn-print" style="border: 1px solid transparent; border-image: none; cursor: default; background-color: transparent;">
                                    <table title="Print">
                                        <tbody>
                                            <tr>
                                                <td>
                                                    <input
                                                        id="PrintButton"
                                                        title="Print"
                                                        style="width: 16px; height: 16px;"
                                                        type="image"
                                                        alt="Print"
                                                        runat="server"
                                                        src="~/Reserved.ReportViewerWebControl.axd?OpType=Resource&amp;Version=11.0.3442.2&amp;Name=Microsoft.Reporting.WebForms.Icons.Print.gif" />
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </script>
</body>
</html>

<script language="javascript" type="text/javascript">
       try {
   
        Sys.Application.add_load(function () {
            $find("ReportViewer").add_propertyChanged(viewerPropertyChanged);

        });
           function viewerPropertyChanged(sender, e) {
            if (e.get_propertyName() == "isLoading") {
                if ($find("ReportViewer").get_isLoading()) {
                    /* Do something when loading starts*/

                }
                else {
                    if ($("#ReportViewer").height() > 500) {
                        $("#reportIframe", window.parent.document.body).height($("#ReportViewer").height() + 150);
                        $("#reportIframe", window.parent.document.body).width($("#ReportViewer_fixedTable").width() + 20);
                    }
                    else {
                        $("#reportIframe", window.parent.document.body).height(600);
                    }
                    /* Do something when loading stops*/
                    $("#ReportViewer").find("#ReportViewer_fixedTable").attr("style", "width:100%");
                    $('#ReportViewer_fixedTable').addClass('report-viewer-table');
                    $('#ReportViewer_fixedTable').find('#ParameterTable_ReportViewer_ctl04').addClass('report-parameter-table');
                    $('#ReportViewer_fixedTable').find('#ReportViewer_ToggleParam').parent().addClass('report-viewer-toggle');
                    $('#ReportViewer_ctl05').addClass('report-viewer-control');
                    $('#ReportViewer_ctl05_ctl04_ctl00_Menu').addClass('report-viewer-control-menu');
                    $('#ReportViewer_ctl05_ctl04_ctl00').addClass('report-viewer-control-btn');
                    $('#ReportViewer_ctl05_ctl05_ctl00_ctl00').addClass('report-viewer-refresh-btn');
                    $('#ReportViewer_ctl05_ctl06_ctl00_ctl00').addClass('report-viewer-print-btn');
                    $('#ReportViewer_AsyncWait_Wait').addClass('report-viewer-asyncwait');
                    var reportControl = $("#ReportViewer").find("#ReportViewer_fixedTable").find("tr:eq(2)").find("div:eq(1)");
                    $(reportControl).children("div:eq(3)").attr("style", "display:inline-block;");
                    $(reportControl).children("div:eq(4)").attr("style", "display:inline-block");
                    $(reportControl).children("div:eq(5)").attr("style", "display:inline-block");
                    $(reportControl).children("div:eq(6)").attr("style", "display:inline-block");
                }
            }
        }
        function adjustIframeSize() {
            /* you can play around with these figures until your report is perfect*/
            var extraHeightToAvoidCuttingOffPartOfReport = 100;
            var extraWidthToAvoidCuttingOffPartOfReport = 10;

            /* '#ReportViewer_fixedTable' is a portion of the report viewer that contains the actual report, minus the parameters etc*/
            var reportPage = $('#ReportViewer_fixedTable');

            /* get the height of the report. '#ParametersRowReportViewer' is that top part that contains parameters etc */
            var newHeight = reportPage.height() + $('#ParametersRowReportViewer').height() + extraHeightToAvoidCuttingOffPartOfReport;
         
            /* same for width */
            var newWidth = reportPage.width() + extraWidthToAvoidCuttingOffPartOfReport;

            /* get iframe from parent document, the rest of this function only works if both the iframe and the parent page are on the same domain  */
            var reportIframe = $('#ReportViewerFrame', parent.document);

            /* just make sure that nothing went wrong with the calculations, other wise the entire report could be given a very small value for height and width, thereby hiding the report*/
            if (newHeight > extraHeightToAvoidCuttingOffPartOfReport)
                reportIframe.height(newHeight);
            if (newWidth > extraWidthToAvoidCuttingOffPartOfReport)
                reportIframe.width(newWidth);
        }
    } catch (e) {
    }      
</script>