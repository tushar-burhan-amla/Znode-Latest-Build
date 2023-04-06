using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

/// <summary>
/// Summary description for TopSpendingCustomers
/// </summary>
public class TopSpendingCustomers : DevExpress.XtraReports.UI.XtraReport
{
    private DevExpress.XtraReports.UI.DetailBand Detail;
    private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
    private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
    private XRTable xrTable2;
    private XRTableRow xrTableRow2;
    private XRTableCell xrTableCell7;
    private XRTableCell xrTableCell8;
    private XRTableCell xrTableCell9;
    private XRTableCell xrTableCell10;
    private XRTableCell xrTableCell11;
    private XRTableCell xrTableCell12;
    private XRPageInfo xrPageInfo1;
    private XRPageInfo xrPageInfo2;
    private DevExpress.DataAccess.ObjectBinding.ObjectDataSource objectDataSource1;
    private ReportHeaderBand reportHeaderBand1;
    private XRLabel xrLabel1;
    private GroupHeaderBand groupHeaderBand1;
    private XRPanel xrPanel1;
    private XRTable xrTable1;
    private XRTableRow xrTableRow1;
    private XRTableCell xrTableCell1;
    private XRTableCell xrTableCell2;
    private XRTableCell xrTableCell3;
    private XRTableCell xrTableCell4;
    private XRTableCell xrTableCell5;
    private XRTableCell xrTableCell6;
    private XRLabel xrLabel3;
    private SubBand SubBand1;
    private XRChart xrChart1;
    private GroupFooterBand GroupFooter1;
    private XRTable xrTable3;
    private XRTableRow xrTableRow3;
    private XRTableCell xrTableCell13;
    private XRTableCell xrTableCell14;
    private XRTableCell xrTableCell15;
    private XRTableCell xrTableCell16;
    private XRTableCell xrTableCell17;
    private XRTableCell xrTableCell18;
    private XRTable xrTable4;
    private XRTableRow xrTableRow5;
    private XRTableCell xrTableCell53;
    private XRTableRow xrTableRow4;
    private XRTableCell xrTableCell52;
    private XRTable xrTable5;
    private XRTableRow xrTableRow6;
    private XRTableCell xrTableCell54;
    private XRTableRow xrTableRow7;
    private XRTableCell xrTableCell55;
    private XRControlStyle TableHeader;
    private XRControlStyle TableRow;
    private XRControlStyle EvenStyle;
    private XRControlStyle OddStyle;
    private XRControlStyle ReportTitle;
    private XRControlStyle ReportFooterPaging;
    private XRControlStyle ReportFooterDateTime;
    private XRControlStyle ReportNameText;
    private XRControlStyle ReportHeaderTitleText;
    private XRControlStyle ReportHeaderText;
    private XRControlStyle ReportHeaderTable1;
    private XRControlStyle ReportHeaderTableRow;
    private XRControlStyle RightAlignCell;
    private XRControlStyle TableHeaderRightAlignCell;
    private XRTableCell xrTableCell20;
    private XRTableCell xrTableCell23;
    private XRTableCell xrTableCell26;
    private XRTableCell xrTableCell29;
    private XRTableCell xrTableCell19;
    private XRTableCell xrTableCell22;
    private XRTableCell xrTableCell25;
    private XRTableCell xrTableCell28;
    private XRTableCell xrTableCell21;
    private XRTableCell xrTableCell24;
    private XRTableCell xrTableCell27;
    private XRTableCell xrTableCell30;

    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    public TopSpendingCustomers()
    {
        InitializeComponent();
        //
        // TODO: Add constructor logic here
        //
    }

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            this.components = new System.ComponentModel.Container();
            DevExpress.XtraCharts.XYDiagram xyDiagram1 = new DevExpress.XtraCharts.XYDiagram();
            DevExpress.XtraCharts.Series series1 = new DevExpress.XtraCharts.Series();
            DevExpress.XtraCharts.SideBySideBarSeriesLabel sideBySideBarSeriesLabel1 = new DevExpress.XtraCharts.SideBySideBarSeriesLabel();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.xrTable2 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow2 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell7 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell8 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell9 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell10 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell20 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell23 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell26 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell29 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell11 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell12 = new DevExpress.XtraReports.UI.XRTableCell();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.xrPageInfo1 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.xrPageInfo2 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.reportHeaderBand1 = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.xrTable5 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow6 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell54 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow7 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell55 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTable4 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow5 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell53 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow4 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell52 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.SubBand1 = new DevExpress.XtraReports.UI.SubBand();
            this.xrChart1 = new DevExpress.XtraReports.UI.XRChart();
            this.groupHeaderBand1 = new DevExpress.XtraReports.UI.GroupHeaderBand();
            this.xrPanel1 = new DevExpress.XtraReports.UI.XRPanel();
            this.xrTable1 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow1 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell1 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell2 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell3 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell4 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell19 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell22 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell25 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell28 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell5 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell6 = new DevExpress.XtraReports.UI.XRTableCell();
            this.GroupFooter1 = new DevExpress.XtraReports.UI.GroupFooterBand();
            this.xrTable3 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow3 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell13 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell14 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell15 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell16 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell21 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell24 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell27 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell30 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell18 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell17 = new DevExpress.XtraReports.UI.XRTableCell();
            this.TableHeader = new DevExpress.XtraReports.UI.XRControlStyle();
            this.TableRow = new DevExpress.XtraReports.UI.XRControlStyle();
            this.EvenStyle = new DevExpress.XtraReports.UI.XRControlStyle();
            this.OddStyle = new DevExpress.XtraReports.UI.XRControlStyle();
            this.ReportTitle = new DevExpress.XtraReports.UI.XRControlStyle();
            this.ReportFooterPaging = new DevExpress.XtraReports.UI.XRControlStyle();
            this.ReportFooterDateTime = new DevExpress.XtraReports.UI.XRControlStyle();
            this.ReportNameText = new DevExpress.XtraReports.UI.XRControlStyle();
            this.ReportHeaderTitleText = new DevExpress.XtraReports.UI.XRControlStyle();
            this.ReportHeaderText = new DevExpress.XtraReports.UI.XRControlStyle();
            this.ReportHeaderTable1 = new DevExpress.XtraReports.UI.XRControlStyle();
            this.ReportHeaderTableRow = new DevExpress.XtraReports.UI.XRControlStyle();
            this.RightAlignCell = new DevExpress.XtraReports.UI.XRControlStyle();
            this.TableHeaderRightAlignCell = new DevExpress.XtraReports.UI.XRControlStyle();
            this.objectDataSource1 = new DevExpress.DataAccess.ObjectBinding.ObjectDataSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.xrTable2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrChart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(xyDiagram1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(series1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(sideBySideBarSeriesLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable2});
            this.Detail.HeightF = 25F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrTable2
            // 
            this.xrTable2.EvenStyleName = "EvenStyle";
            this.xrTable2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrTable2.Name = "xrTable2";
            this.xrTable2.OddStyleName = "OddStyle";
            this.xrTable2.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow2});
            this.xrTable2.SizeF = new System.Drawing.SizeF(1448F, 25F);
            // 
            // xrTableRow2
            // 
            this.xrTableRow2.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell7,
            this.xrTableCell8,
            this.xrTableCell9,
            this.xrTableCell10,
            this.xrTableCell20,
            this.xrTableCell23,
            this.xrTableCell26,
            this.xrTableCell29,
            this.xrTableCell11,
            this.xrTableCell12});
            this.xrTableRow2.Name = "xrTableRow2";
            this.xrTableRow2.Weight = 11.5D;
            // 
            // xrTableCell7
            // 
            this.xrTableCell7.CanShrink = true;
            this.xrTableCell7.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[StoreName]")});
            this.xrTableCell7.Name = "xrTableCell7";
            this.xrTableCell7.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell7.StyleName = "TableRow";
            this.xrTableCell7.Text = "xrTableCell7";
            this.xrTableCell7.Weight = 0.1115789560171274D;
            // 
            // xrTableCell8
            // 
            this.xrTableCell8.CanShrink = true;
            this.xrTableCell8.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[CustomerName]")});
            this.xrTableCell8.Name = "xrTableCell8";
            this.xrTableCell8.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell8.StyleName = "TableRow";
            this.xrTableCell8.Text = "xrTableCell8";
            this.xrTableCell8.Weight = 0.23578948974609376D;
            // 
            // xrTableCell9
            // 
            this.xrTableCell9.CanShrink = true;
            this.xrTableCell9.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[CustomerType]")});
            this.xrTableCell9.Name = "xrTableCell9";
            this.xrTableCell9.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell9.StyleName = "TableRow";
            this.xrTableCell9.Text = "xrTableCell9";
            this.xrTableCell9.Weight = 0.2315789794921875D;
            // 
            // xrTableCell10
            // 
            this.xrTableCell10.CanShrink = true;
            this.xrTableCell10.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Email]")});
            this.xrTableCell10.Name = "xrTableCell10";
            this.xrTableCell10.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell10.StyleName = "TableRow";
            this.xrTableCell10.Text = "xrTableCell10";
            this.xrTableCell10.Weight = 0.16421054546649638D;
            // 
            // xrTableCell20
            // 
            this.xrTableCell20.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[SubTotalAmount]")});
            this.xrTableCell20.Name = "xrTableCell20";
            this.xrTableCell20.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell20.StyleName = "TableRow";
            this.xrTableCell20.Text = "xrTableCell20";
            this.xrTableCell20.TextFormatString = "{0:$0.00}";
            this.xrTableCell20.Weight = 0.11153085589507192D;
            // 
            // xrTableCell23
            // 
            this.xrTableCell23.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[TotalShippingCost]")});
            this.xrTableCell23.Name = "xrTableCell23";
            this.xrTableCell23.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell23.StyleName = "TableRow";
            this.xrTableCell23.Text = "xrTableCell23";
            this.xrTableCell23.TextFormatString = "{0:$0.00}";
            this.xrTableCell23.Weight = 0.11153085589507192D;
            // 
            // xrTableCell26
            // 
            this.xrTableCell26.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[TotalTaxCost]")});
            this.xrTableCell26.Name = "xrTableCell26";
            this.xrTableCell26.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell26.StyleName = "TableRow";
            this.xrTableCell26.Text = "xrTableCell26";
            this.xrTableCell26.TextFormatString = "{0:$0.00}";
            this.xrTableCell26.Weight = 0.11153085589507192D;
            // 
            // xrTableCell29
            // 
            this.xrTableCell29.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[TotalDiscount]")});
            this.xrTableCell29.Name = "xrTableCell29";
            this.xrTableCell29.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell29.StyleName = "TableRow";
            this.xrTableCell29.Text = "xrTableCell29";
            this.xrTableCell29.TextFormatString = "{0:$0.00}";
            this.xrTableCell29.Weight = 0.11153085589507192D;
            // 
            // xrTableCell11
            // 
            this.xrTableCell11.CanShrink = true;
            this.xrTableCell11.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[AverageTotalAmount]")});
            this.xrTableCell11.Name = "xrTableCell11";
            this.xrTableCell11.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell11.StyleName = "TableRow";
            this.xrTableCell11.Text = "xrTableCell11";
            this.xrTableCell11.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.xrTableCell11.TextFormatString = "{0:$0.00}";
            this.xrTableCell11.Weight = 0.22320602389384034D;
            // 
            // xrTableCell12
            // 
            this.xrTableCell12.CanShrink = true;
            this.xrTableCell12.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[TotalAmount]")});
            this.xrTableCell12.Name = "xrTableCell12";
            this.xrTableCell12.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell12.StyleName = "RightAlignCell";
            this.xrTableCell12.Text = "xrTableCell12";
            this.xrTableCell12.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.xrTableCell12.TextFormatString = "{0:$0.00}";
            this.xrTableCell12.Weight = 0.12814617103834256D;
            // 
            // TopMargin
            // 
            this.TopMargin.HeightF = 29F;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // BottomMargin
            // 
            this.BottomMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPageInfo1,
            this.xrPageInfo2});
            this.BottomMargin.HeightF = 42.00001F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrPageInfo1
            // 
            this.xrPageInfo1.CanPublish = false;
            this.xrPageInfo1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 6.00001F);
            this.xrPageInfo1.Name = "xrPageInfo1";
            this.xrPageInfo1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrPageInfo1.PageInfo = DevExpress.XtraPrinting.PageInfo.DateTime;
            this.xrPageInfo1.SizeF = new System.Drawing.SizeF(698.4742F, 23F);
            this.xrPageInfo1.StyleName = "ReportFooterDateTime";
            // 
            // xrPageInfo2
            // 
            this.xrPageInfo2.CanPublish = false;
            this.xrPageInfo2.LocationFloat = new DevExpress.Utils.PointFloat(698.4778F, 6.00001F);
            this.xrPageInfo2.Name = "xrPageInfo2";
            this.xrPageInfo2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrPageInfo2.SizeF = new System.Drawing.SizeF(749.5222F, 23F);
            this.xrPageInfo2.StyleName = "ReportFooterPaging";
            this.xrPageInfo2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrPageInfo2.TextFormatString = "Page {0} of {1}";
            // 
            // reportHeaderBand1
            // 
            this.reportHeaderBand1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable5,
            this.xrTable4,
            this.xrLabel3,
            this.xrLabel1});
            this.reportHeaderBand1.HeightF = 94.38F;
            this.reportHeaderBand1.Name = "reportHeaderBand1";
            this.reportHeaderBand1.SubBands.AddRange(new DevExpress.XtraReports.UI.SubBand[] {
            this.SubBand1});
            // 
            // xrTable5
            // 
            this.xrTable5.LocationFloat = new DevExpress.Utils.PointFloat(544.1371F, 64.38F);
            this.xrTable5.Name = "xrTable5";
            this.xrTable5.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow6,
            this.xrTableRow7});
            this.xrTable5.SizeF = new System.Drawing.SizeF(903.8635F, 30F);
            this.xrTable5.StyleName = "ReportHeaderTable1";
            // 
            // xrTableRow6
            // 
            this.xrTableRow6.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell54});
            this.xrTableRow6.Name = "xrTableRow6";
            this.xrTableRow6.Weight = 0.8D;
            // 
            // xrTableCell54
            // 
            this.xrTableCell54.Font = new System.Drawing.Font("Verdana", 6.75F);
            this.xrTableCell54.Name = "xrTableCell54";
            this.xrTableCell54.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell54.StyleName = "ReportHeaderTable1";
            this.xrTableCell54.Text = "Store Name Equals:";
            this.xrTableCell54.Weight = 1D;
            // 
            // xrTableRow7
            // 
            this.xrTableRow7.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell55});
            this.xrTableRow7.Name = "xrTableRow7";
            this.xrTableRow7.Weight = 0.79999999999999993D;
            // 
            // xrTableCell55
            // 
            this.xrTableCell55.Name = "xrTableCell55";
            this.xrTableCell55.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell55.StyleName = "ReportHeaderTableRow";
            this.xrTableCell55.Text = "xrTableCell52";
            this.xrTableCell55.Weight = 1D;
            // 
            // xrTable4
            // 
            this.xrTable4.LocationFloat = new DevExpress.Utils.PointFloat(0F, 64.38F);
            this.xrTable4.Name = "xrTable4";
            this.xrTable4.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow5,
            this.xrTableRow4});
            this.xrTable4.SizeF = new System.Drawing.SizeF(544.1371F, 29.99997F);
            this.xrTable4.StyleName = "ReportHeaderTable1";
            // 
            // xrTableRow5
            // 
            this.xrTableRow5.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell53});
            this.xrTableRow5.Name = "xrTableRow5";
            this.xrTableRow5.Weight = 0.8D;
            // 
            // xrTableCell53
            // 
            this.xrTableCell53.Font = new System.Drawing.Font("Verdana", 6.75F);
            this.xrTableCell53.Name = "xrTableCell53";
            this.xrTableCell53.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell53.StyleName = "ReportHeaderTable1";
            this.xrTableCell53.Text = "Order Date is Between:";
            this.xrTableCell53.Weight = 1D;
            // 
            // xrTableRow4
            // 
            this.xrTableRow4.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell52});
            this.xrTableRow4.Name = "xrTableRow4";
            this.xrTableRow4.Weight = 0.79999999999999993D;
            // 
            // xrTableCell52
            // 
            this.xrTableCell52.Font = new System.Drawing.Font("Verdana", 6.75F);
            this.xrTableCell52.Name = "xrTableCell52";
            this.xrTableCell52.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell52.StyleName = "ReportHeaderTableRow";
            this.xrTableCell52.Text = "xrTableCell52";
            this.xrTableCell52.Weight = 1D;
            // 
            // xrLabel3
            // 
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(0F, 10.00001F);
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(10, 2, 2, 2, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(544.1399F, 29.24998F);
            this.xrLabel3.StyleName = "ReportHeaderText";
            this.xrLabel3.StylePriority.UsePadding = false;
            this.xrLabel3.StylePriority.UseTextAlignment = false;
            this.xrLabel3.Text = "Top Spending Customers";
            this.xrLabel3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel1
            // 
            this.xrLabel1.CanPublish = false;
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(1448F, 50F);
            this.xrLabel1.StyleName = "ReportTitle";
            // 
            // SubBand1
            // 
            this.SubBand1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrChart1});
            this.SubBand1.HeightF = 320.8333F;
            this.SubBand1.Name = "SubBand1";
            // 
            // xrChart1
            // 
            this.xrChart1.BorderColor = System.Drawing.Color.Black;
            this.xrChart1.Borders = DevExpress.XtraPrinting.BorderSide.None;
            xyDiagram1.AxisX.VisibleInPanesSerializable = "-1";
            xyDiagram1.AxisY.VisibleInPanesSerializable = "-1";
            xyDiagram1.DefaultPane.EnableAxisXScrolling = DevExpress.Utils.DefaultBoolean.False;
            xyDiagram1.DefaultPane.EnableAxisXZooming = DevExpress.Utils.DefaultBoolean.False;
            xyDiagram1.DefaultPane.EnableAxisYScrolling = DevExpress.Utils.DefaultBoolean.False;
            xyDiagram1.DefaultPane.EnableAxisYZooming = DevExpress.Utils.DefaultBoolean.False;
            this.xrChart1.Diagram = xyDiagram1;
            this.xrChart1.Legend.Name = "Default Legend";
            this.xrChart1.Legend.Visibility = DevExpress.Utils.DefaultBoolean.False;
            this.xrChart1.LocationFloat = new DevExpress.Utils.PointFloat(10.00001F, 0F);
            this.xrChart1.Name = "xrChart1";
            series1.ArgumentDataMember = "XAxis";
            sideBySideBarSeriesLabel1.Position = DevExpress.XtraCharts.BarSeriesLabelPosition.Top;
            series1.Label = sideBySideBarSeriesLabel1;
            series1.Name = "Series 1";
            series1.ValueDataMembersSerializable = "YAxis";
            this.xrChart1.SeriesSerializable = new DevExpress.XtraCharts.Series[] {
        series1};
            this.xrChart1.SizeF = new System.Drawing.SizeF(1438F, 310.8334F);
            // 
            // groupHeaderBand1
            // 
            this.groupHeaderBand1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPanel1});
            this.groupHeaderBand1.GroupUnion = DevExpress.XtraReports.UI.GroupUnion.WithFirstDetail;
            this.groupHeaderBand1.HeightF = 48F;
            this.groupHeaderBand1.Name = "groupHeaderBand1";
            this.groupHeaderBand1.RepeatEveryPage = true;
            // 
            // xrPanel1
            // 
            this.xrPanel1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable1});
            this.xrPanel1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrPanel1.Name = "xrPanel1";
            this.xrPanel1.SizeF = new System.Drawing.SizeF(1448F, 48F);
            // 
            // xrTable1
            // 
            this.xrTable1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 20.00001F);
            this.xrTable1.Name = "xrTable1";
            this.xrTable1.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow1});
            this.xrTable1.SizeF = new System.Drawing.SizeF(1448F, 28F);
            // 
            // xrTableRow1
            // 
            this.xrTableRow1.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell1,
            this.xrTableCell2,
            this.xrTableCell3,
            this.xrTableCell4,
            this.xrTableCell19,
            this.xrTableCell22,
            this.xrTableCell25,
            this.xrTableCell28,
            this.xrTableCell5,
            this.xrTableCell6});
            this.xrTableRow1.Name = "xrTableRow1";
            this.xrTableRow1.Weight = 1D;
            // 
            // xrTableCell1
            // 
            this.xrTableCell1.Name = "xrTableCell1";
            this.xrTableCell1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell1.StyleName = "TableHeader";
            this.xrTableCell1.Text = "Store Name";
            this.xrTableCell1.Weight = 0.11157894427959736D;
            // 
            // xrTableCell2
            // 
            this.xrTableCell2.Name = "xrTableCell2";
            this.xrTableCell2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell2.StyleName = "TableHeader";
            this.xrTableCell2.Text = "Customer Name";
            this.xrTableCell2.Weight = 0.23578946627103364D;
            // 
            // xrTableCell3
            // 
            this.xrTableCell3.Name = "xrTableCell3";
            this.xrTableCell3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell3.StyleName = "TableHeader";
            this.xrTableCell3.Text = "Customer Type";
            this.xrTableCell3.Weight = 0.23157895601712741D;
            // 
            // xrTableCell4
            // 
            this.xrTableCell4.Name = "xrTableCell4";
            this.xrTableCell4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell4.StyleName = "TableHeader";
            this.xrTableCell4.Text = "Email ID";
            this.xrTableCell4.Weight = 0.16421053372896635D;
            // 
            // xrTableCell19
            // 
            this.xrTableCell19.Name = "xrTableCell19";
            this.xrTableCell19.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell19.StyleName = "TableHeader";
            this.xrTableCell19.Text = "Sub Total Amount";
            this.xrTableCell19.Weight = 0.11153084999980438D;
            // 
            // xrTableCell22
            // 
            this.xrTableCell22.Name = "xrTableCell22";
            this.xrTableCell22.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell22.StyleName = "TableHeader";
            this.xrTableCell22.Text = "Total Shipping Cost";
            this.xrTableCell22.Weight = 0.11153084999980438D;
            // 
            // xrTableCell25
            // 
            this.xrTableCell25.Name = "xrTableCell25";
            this.xrTableCell25.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell25.StyleName = "TableHeader";
            this.xrTableCell25.Text = "Total Tax Cost";
            this.xrTableCell25.Weight = 0.11153084999980438D;
            // 
            // xrTableCell28
            // 
            this.xrTableCell28.Name = "xrTableCell28";
            this.xrTableCell28.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell28.StyleName = "TableHeader";
            this.xrTableCell28.Text = "Total Discount";
            this.xrTableCell28.Weight = 0.11153084999980438D;
            // 
            // xrTableCell5
            // 
            this.xrTableCell5.Name = "xrTableCell5";
            this.xrTableCell5.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell5.StyleName = "TableHeader";
            this.xrTableCell5.Text = "Average Total Amount";
            this.xrTableCell5.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.xrTableCell5.Weight = 0.1813755695410112D;
            // 
            // xrTableCell6
            // 
            this.xrTableCell6.Name = "xrTableCell6";
            this.xrTableCell6.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell6.StyleName = "TableHeaderRightAlignCell";
            this.xrTableCell6.Text = "Total Amount";
            this.xrTableCell6.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.xrTableCell6.Weight = 0.1699765685017523D;
            // 
            // GroupFooter1
            // 
            this.GroupFooter1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable3});
            this.GroupFooter1.HeightF = 30.20833F;
            this.GroupFooter1.Name = "GroupFooter1";
            // 
            // xrTable3
            // 
            this.xrTable3.LocationFloat = new DevExpress.Utils.PointFloat(0F, 1.619975F);
            this.xrTable3.Name = "xrTable3";
            this.xrTable3.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow3});
            this.xrTable3.SizeF = new System.Drawing.SizeF(1448F, 28F);
            // 
            // xrTableRow3
            // 
            this.xrTableRow3.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell13,
            this.xrTableCell14,
            this.xrTableCell15,
            this.xrTableCell16,
            this.xrTableCell21,
            this.xrTableCell24,
            this.xrTableCell27,
            this.xrTableCell30,
            this.xrTableCell18,
            this.xrTableCell17});
            this.xrTableRow3.Name = "xrTableRow3";
            this.xrTableRow3.Weight = 1D;
            // 
            // xrTableCell13
            // 
            this.xrTableCell13.Name = "xrTableCell13";
            this.xrTableCell13.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell13.StyleName = "TableHeader";
            this.xrTableCell13.Text = "Total";
            this.xrTableCell13.Weight = 0.096574710497347491D;
            // 
            // xrTableCell14
            // 
            this.xrTableCell14.Name = "xrTableCell14";
            this.xrTableCell14.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell14.StyleName = "TableHeader";
            this.xrTableCell14.Weight = 0.20408241330665197D;
            // 
            // xrTableCell15
            // 
            this.xrTableCell15.Name = "xrTableCell15";
            this.xrTableCell15.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell15.StyleName = "TableHeader";
            this.xrTableCell15.Weight = 0.2004381029874231D;
            // 
            // xrTableCell16
            // 
            this.xrTableCell16.Name = "xrTableCell16";
            this.xrTableCell16.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell16.StyleName = "TableHeader";
            this.xrTableCell16.Weight = 0.14212885917359014D;
            // 
            // xrTableCell21
            // 
            this.xrTableCell21.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sum([SubTotalAmount])")});
            this.xrTableCell21.Name = "xrTableCell21";
            this.xrTableCell21.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell21.StyleName = "TableHeader";
            this.xrTableCell21.TextFormatString = "{0:$0.00}";
            this.xrTableCell21.Weight = 0.0965285131324356D;
            // 
            // xrTableCell24
            // 
            this.xrTableCell24.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "SUM([TotalShippingCost])")});
            this.xrTableCell24.Name = "xrTableCell24";
            this.xrTableCell24.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell24.StyleName = "TableHeader";
            this.xrTableCell24.TextFormatString = "{0:$0.00}";
            this.xrTableCell24.Weight = 0.0965285131324356D;
            // 
            // xrTableCell27
            // 
            this.xrTableCell27.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sum([TotalTaxCost])")});
            this.xrTableCell27.Name = "xrTableCell27";
            this.xrTableCell27.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell27.StyleName = "TableHeader";
            this.xrTableCell27.TextFormatString = "{0:$0.00}";
            this.xrTableCell27.Weight = 0.0965285131324356D;
            // 
            // xrTableCell30
            // 
            this.xrTableCell30.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "SUM([TotalDiscount])")});
            this.xrTableCell30.Name = "xrTableCell30";
            this.xrTableCell30.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell30.StyleName = "TableHeader";
            this.xrTableCell30.TextFormatString = "{0:$0.00}";
            this.xrTableCell30.Weight = 0.0965285131324356D;
            // 
            // xrTableCell18
            // 
            this.xrTableCell18.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "SUM([AverageTotalAmount])")});
            this.xrTableCell18.Name = "xrTableCell18";
            this.xrTableCell18.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell18.StyleName = "TableHeader";
            this.xrTableCell18.StylePriority.UsePadding = false;
            this.xrTableCell18.TextFormatString = "{0:$0.00}";
            this.xrTableCell18.Weight = 0.19319573880300639D;
            // 
            // xrTableCell17
            // 
            this.xrTableCell17.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Sum([TotalAmount])")});
            this.xrTableCell17.Name = "xrTableCell17";
            this.xrTableCell17.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell17.StyleName = "TableHeaderRightAlignCell";
            this.xrTableCell17.TextFormatString = "{0:$0.00}";
            this.xrTableCell17.Weight = 0.11092073234144614D;
            // 
            // TableHeader
            // 
            this.TableHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.TableHeader.BorderColor = System.Drawing.Color.White;
            this.TableHeader.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TableHeader.ForeColor = System.Drawing.Color.White;
            this.TableHeader.Name = "TableHeader";
            this.TableHeader.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // TableRow
            // 
            this.TableRow.Name = "TableRow";
            this.TableRow.Padding = new DevExpress.XtraPrinting.PaddingInfo(1, 1, 1, 1, 100F);
            this.TableRow.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // EvenStyle
            // 
            this.EvenStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.EvenStyle.BorderColor = System.Drawing.Color.Gray;
            this.EvenStyle.Borders = ((DevExpress.XtraPrinting.BorderSide)((DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.EvenStyle.BorderWidth = 1F;
            this.EvenStyle.Name = "EvenStyle";
            this.EvenStyle.Padding = new DevExpress.XtraPrinting.PaddingInfo(1, 1, 1, 1, 100F);
            // 
            // OddStyle
            // 
            this.OddStyle.BorderColor = System.Drawing.Color.Gray;
            this.OddStyle.Borders = ((DevExpress.XtraPrinting.BorderSide)((DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Right)));
            this.OddStyle.BorderWidth = 1F;
            this.OddStyle.Name = "OddStyle";
            this.OddStyle.Padding = new DevExpress.XtraPrinting.PaddingInfo(1, 1, 1, 1, 100F);
            // 
            // ReportTitle
            // 
            this.ReportTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ReportTitle.BorderColor = System.Drawing.Color.White;
            this.ReportTitle.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.ReportTitle.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ReportTitle.ForeColor = System.Drawing.Color.White;
            this.ReportTitle.Name = "ReportTitle";
            // 
            // ReportFooterPaging
            // 
            this.ReportFooterPaging.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ReportFooterPaging.ForeColor = System.Drawing.Color.White;
            this.ReportFooterPaging.Name = "ReportFooterPaging";
            this.ReportFooterPaging.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 3, 3, 3, 100F);
            // 
            // ReportFooterDateTime
            // 
            this.ReportFooterDateTime.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ReportFooterDateTime.ForeColor = System.Drawing.Color.White;
            this.ReportFooterDateTime.Name = "ReportFooterDateTime";
            this.ReportFooterDateTime.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 3, 3, 3, 100F);
            // 
            // ReportNameText
            // 
            this.ReportNameText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ReportNameText.Font = new System.Drawing.Font("Candara", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ReportNameText.ForeColor = System.Drawing.Color.White;
            this.ReportNameText.Name = "ReportNameText";
            this.ReportNameText.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            // 
            // ReportHeaderTitleText
            // 
            this.ReportHeaderTitleText.Font = new System.Drawing.Font("Segoe UI", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ReportHeaderTitleText.Name = "ReportHeaderTitleText";
            this.ReportHeaderTitleText.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 2, 2, 100F);
            // 
            // ReportHeaderText
            // 
            this.ReportHeaderText.Font = new System.Drawing.Font("Segoe UI", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ReportHeaderText.ForeColor = System.Drawing.Color.White;
            this.ReportHeaderText.Name = "ReportHeaderText";
            this.ReportHeaderText.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 2, 2, 100F);
            this.ReportHeaderText.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            // 
            // ReportHeaderTable1
            // 
            this.ReportHeaderTable1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(180)))), ((int)(((byte)(86)))));
            this.ReportHeaderTable1.Borders = DevExpress.XtraPrinting.BorderSide.Left;
            this.ReportHeaderTable1.BorderWidth = 4F;
            this.ReportHeaderTable1.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ReportHeaderTable1.Name = "ReportHeaderTable1";
            this.ReportHeaderTable1.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 1, 1, 1, 100F);
            // 
            // ReportHeaderTableRow
            // 
            this.ReportHeaderTableRow.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(180)))), ((int)(((byte)(86)))));
            this.ReportHeaderTableRow.Borders = DevExpress.XtraPrinting.BorderSide.Left;
            this.ReportHeaderTableRow.BorderWidth = 4F;
            this.ReportHeaderTableRow.Font = new System.Drawing.Font("Verdana", 6.75F);
            this.ReportHeaderTableRow.Name = "ReportHeaderTableRow";
            this.ReportHeaderTableRow.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 1, 1, 1, 100F);
            // 
            // RightAlignCell
            // 
            this.RightAlignCell.Name = "RightAlignCell";
            this.RightAlignCell.Padding = new DevExpress.XtraPrinting.PaddingInfo(1, 4, 1, 1, 100F);
            this.RightAlignCell.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            // 
            // TableHeaderRightAlignCell
            // 
            this.TableHeaderRightAlignCell.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.TableHeaderRightAlignCell.BorderColor = System.Drawing.Color.White;
            this.TableHeaderRightAlignCell.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TableHeaderRightAlignCell.ForeColor = System.Drawing.Color.White;
            this.TableHeaderRightAlignCell.Name = "TableHeaderRightAlignCell";
            this.TableHeaderRightAlignCell.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            // 
            // objectDataSource1
            // 
            this.objectDataSource1.DataSource = typeof(Znode.Libraries.DevExpress.Report.TopSpendingCustomersModel);
            this.objectDataSource1.Name = "objectDataSource1";
            // 
            // TopSpendingCustomers
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.reportHeaderBand1,
            this.groupHeaderBand1,
            this.GroupFooter1});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.objectDataSource1});
            this.DataSource = this.objectDataSource1;
            this.Landscape = true;
            this.Margins = new System.Drawing.Printing.Margins(25, 27, 29, 42);
            this.PageHeight = 827;
            this.PageWidth = 1500;
            this.PaperKind = System.Drawing.Printing.PaperKind.Custom;
            this.StyleSheet.AddRange(new DevExpress.XtraReports.UI.XRControlStyle[] {
            this.TableHeader,
            this.TableRow,
            this.EvenStyle,
            this.OddStyle,
            this.ReportTitle,
            this.ReportFooterPaging,
            this.ReportFooterDateTime,
            this.ReportNameText,
            this.ReportHeaderTitleText,
            this.ReportHeaderText,
            this.ReportHeaderTable1,
            this.ReportHeaderTableRow,
            this.RightAlignCell,
            this.TableHeaderRightAlignCell});
            this.Version = "17.2";
            ((System.ComponentModel.ISupportInitialize)(this.xrTable2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(xyDiagram1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(sideBySideBarSeriesLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(series1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrChart1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

    }

    #endregion
}
