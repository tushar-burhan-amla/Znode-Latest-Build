using DevExpress.XtraReports.UI;

/// <summary>
/// Summary description for Orders
/// </summary>
public class Orders : DevExpress.XtraReports.UI.XtraReport
{
    private DevExpress.XtraReports.UI.DetailBand Detail;
    protected TopMarginBand TopMargin;
    private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
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
    private XRTableCell xrTableCell7;
    private XRTableCell xrTableCell8;
    private XRTableCell xrTableCell10;
    private XRTableCell xrTableCell12;
    private XRTableCell xrTableCell13;
    private XRTableCell xrTableCell14;
    private XRTableCell xrTableCell15;
    private XRTableCell xrTableCell16;
    private XRTableCell xrTableCell17;
    private XRTableCell xrTableCell5;
    private XRTableCell xrTableCell9;
    private XRTable xrTable2;
    private XRTableRow xrTableRow2;
    private XRTableCell xrTableCell18;
    private XRTableCell xrTableCell19;
    private XRTableCell xrTableCell20;
    private XRTableCell xrTableCell21;
    private XRTableCell xrTableCell24;
    private XRTableCell xrTableCell25;
    private XRTableCell xrTableCell27;
    private XRTableCell xrTableCell29;
    private XRTableCell xrTableCell6;
    private XRTableCell xrTableCell30;
    private XRTableCell xrTableCell31;
    private XRTableCell xrTableCell32;
    private XRTableCell xrTableCell33;
    private XRTableCell xrTableCell34;
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
    private XRLabel xrLabel3;
    private SubBand SubBand1;
    public XRChart xrChart1;
    private XRTableCell xrTableCell26;
    private XRTableCell xrTableCell23;
    private GroupFooterBand GroupFooter1;
    private XRTable xrTable3;
    private XRTableRow xrTableRow3;
    private XRTableCell xrTableCell35;
    private XRTableCell xrTableCell36;
    private XRTableCell xrTableCell37;
    private XRTableCell xrTableCell38;
    private XRTableCell xrTableCell39;
    private XRTableCell xrTableCell40;
    private XRTableCell xrTableCell41;
    private XRTableCell xrTableCell42;
    private XRTableCell xrTableCell44;
    private XRTableCell xrTableCell45;
    private XRTableCell xrTableCell46;
    private XRTableCell xrTableCell47;
    private XRTableCell xrTableCell48;
    private XRTableCell xrTableCell49;
    private XRTableCell xrTableCell50;
    private XRTableCell xrTableCell51;
    private XRTableCell xrTableCell22;
    private XRTable xrTable5;
    private XRTableRow xrTableRow6;
    private XRTableCell xrTableCell54;
    private XRTableRow xrTableRow7;
    private XRTableCell xrTableCell55;
    private XRTable xrTable4;
    private XRTableRow xrTableRow5;
    private XRTableCell xrTableCell53;
    private XRTableRow xrTableRow4;
    private XRTableCell xrTableCell52;
    private XRControlStyle ReportHeaderTable1;
    private XRControlStyle ReportHeaderTableRow;
    private XRControlStyle RightAlignCell;
    private XRControlStyle TableHeaderRightAlignCell;

    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    public Orders()
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
            DevExpress.XtraCharts.PieSeriesView pieSeriesView1 = new DevExpress.XtraCharts.PieSeriesView();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.xrTable2 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow2 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell26 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell18 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell19 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell20 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell21 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell24 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell25 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell27 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell29 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell6 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell22 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell30 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell31 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell32 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell33 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell34 = new DevExpress.XtraReports.UI.XRTableCell();
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
            this.xrTableCell23 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell1 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell2 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell3 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell4 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell7 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell8 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell10 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell12 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell5 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell9 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell13 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell14 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell15 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell16 = new DevExpress.XtraReports.UI.XRTableCell();
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
            this.GroupFooter1 = new DevExpress.XtraReports.UI.GroupFooterBand();
            this.xrTable3 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow3 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell35 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell36 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell37 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell38 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell39 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell40 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell41 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell42 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell44 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell45 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell46 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell47 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell48 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell49 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell50 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell51 = new DevExpress.XtraReports.UI.XRTableCell();
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
            ((System.ComponentModel.ISupportInitialize)(pieSeriesView1)).BeginInit();
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
            this.xrTableCell26,
            this.xrTableCell18,
            this.xrTableCell19,
            this.xrTableCell20,
            this.xrTableCell21,
            this.xrTableCell24,
            this.xrTableCell25,
            this.xrTableCell27,
            this.xrTableCell29,
            this.xrTableCell6,
            this.xrTableCell22,
            this.xrTableCell30,
            this.xrTableCell31,
            this.xrTableCell32,
            this.xrTableCell33,
            this.xrTableCell34});
            this.xrTableRow2.Name = "xrTableRow2";
            this.xrTableRow2.Weight = 11.5D;
            // 
            // xrTableCell26
            // 
            this.xrTableCell26.CanShrink = true;
            this.xrTableCell26.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[StoreName]")});
            this.xrTableCell26.Font = new System.Drawing.Font("Verdana", 6.75F);
            this.xrTableCell26.Name = "xrTableCell26";
            this.xrTableCell26.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell26.StyleName = "TableRow";
            this.xrTableCell26.Text = "xrTableCell26";
            this.xrTableCell26.Weight = 0.068246096482670215D;
            // 
            // xrTableCell18
            // 
            this.xrTableCell18.CanShrink = true;
            this.xrTableCell18.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[OrderNumber]")});
            this.xrTableCell18.Font = new System.Drawing.Font("Verdana", 6.75F);
            this.xrTableCell18.Name = "xrTableCell18";
            this.xrTableCell18.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell18.StyleName = "TableRow";
            this.xrTableCell18.StylePriority.UseFont = false;
            this.xrTableCell18.Text = "xrTableCell18";
            this.xrTableCell18.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell18.Weight = 0.056427902627327151D;
            // 
            // xrTableCell19
            // 
            this.xrTableCell19.CanShrink = true;
            this.xrTableCell19.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[OrderDate]")});
            this.xrTableCell19.Font = new System.Drawing.Font("Verdana", 6.75F);
            this.xrTableCell19.Name = "xrTableCell19";
            this.xrTableCell19.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell19.StyleName = "TableRow";
            this.xrTableCell19.StylePriority.UseFont = false;
            this.xrTableCell19.Text = "xrTableCell19";
            this.xrTableCell19.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell19.Weight = 0.061983704302449351D;
            // 
            // xrTableCell20
            // 
            this.xrTableCell20.CanShrink = true;
            this.xrTableCell20.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[OrderStatus]")});
            this.xrTableCell20.Font = new System.Drawing.Font("Verdana", 6.75F);
            this.xrTableCell20.Name = "xrTableCell20";
            this.xrTableCell20.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell20.StyleName = "TableRow";
            this.xrTableCell20.StylePriority.UseFont = false;
            this.xrTableCell20.Text = "xrTableCell20";
            this.xrTableCell20.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell20.Weight = 0.06060738129186833D;
            // 
            // xrTableCell21
            // 
            this.xrTableCell21.CanShrink = true;
            this.xrTableCell21.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[BillingFirstName] +\' \'+ [BillingLastName]")});
            this.xrTableCell21.Font = new System.Drawing.Font("Verdana", 6.75F);
            this.xrTableCell21.Name = "xrTableCell21";
            this.xrTableCell21.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell21.StyleName = "TableRow";
            this.xrTableCell21.StylePriority.UseFont = false;
            this.xrTableCell21.Text = "xrTableCell21";
            this.xrTableCell21.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell21.Weight = 0.07332425447814632D;
            // 
            // xrTableCell24
            // 
            this.xrTableCell24.CanShrink = true;
            this.xrTableCell24.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ShippingCity]")});
            this.xrTableCell24.Font = new System.Drawing.Font("Verdana", 6.75F);
            this.xrTableCell24.Name = "xrTableCell24";
            this.xrTableCell24.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell24.StyleName = "TableRow";
            this.xrTableCell24.StylePriority.UseFont = false;
            this.xrTableCell24.Text = "xrTableCell24";
            this.xrTableCell24.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell24.Weight = 0.044430920527531552D;
            // 
            // xrTableCell25
            // 
            this.xrTableCell25.CanShrink = true;
            this.xrTableCell25.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ShippingStateCode]")});
            this.xrTableCell25.Font = new System.Drawing.Font("Verdana", 6.75F);
            this.xrTableCell25.Name = "xrTableCell25";
            this.xrTableCell25.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell25.StyleName = "TableRow";
            this.xrTableCell25.StylePriority.UseFont = false;
            this.xrTableCell25.Text = "xrTableCell25";
            this.xrTableCell25.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell25.Weight = 0.038953133068130807D;
            // 
            // xrTableCell27
            // 
            this.xrTableCell27.CanShrink = true;
            this.xrTableCell27.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ShippingCountryCode]")});
            this.xrTableCell27.Font = new System.Drawing.Font("Verdana", 6.75F);
            this.xrTableCell27.Name = "xrTableCell27";
            this.xrTableCell27.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell27.StyleName = "TableRow";
            this.xrTableCell27.StylePriority.UseFont = false;
            this.xrTableCell27.Text = "xrTableCell27";
            this.xrTableCell27.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell27.Weight = 0.061946451723105783D;
            // 
            // xrTableCell29
            // 
            this.xrTableCell29.CanShrink = true;
            this.xrTableCell29.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[BillingEmailId]")});
            this.xrTableCell29.Font = new System.Drawing.Font("Verdana", 6.75F);
            this.xrTableCell29.Name = "xrTableCell29";
            this.xrTableCell29.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell29.StyleName = "TableRow";
            this.xrTableCell29.StylePriority.UseFont = false;
            this.xrTableCell29.Text = "xrTableCell29";
            this.xrTableCell29.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell29.Weight = 0.074221364001946219D;
            // 
            // xrTableCell6
            // 
            this.xrTableCell6.CanShrink = true;
            this.xrTableCell6.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ShippingTypeName]")});
            this.xrTableCell6.Font = new System.Drawing.Font("Verdana", 6.75F);
            this.xrTableCell6.Name = "xrTableCell6";
            this.xrTableCell6.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell6.StyleName = "TableRow";
            this.xrTableCell6.StylePriority.UseFont = false;
            this.xrTableCell6.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrTableCell6.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell6.Weight = 0.062831401313047278D;
            // 
            // xrTableCell22
            // 
            this.xrTableCell22.CanShrink = true;
            this.xrTableCell22.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[PaymentTypeName]")});
            this.xrTableCell22.Font = new System.Drawing.Font("Verdana", 6.75F);
            this.xrTableCell22.Name = "xrTableCell22";
            this.xrTableCell22.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell22.StyleName = "TableRow";
            this.xrTableCell22.StylePriority.UseFont = false;
            this.xrTableCell22.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrTableCell22.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell22.Weight = 0.060383592656282244D;
            // 
            // xrTableCell30
            // 
            this.xrTableCell30.CanShrink = true;
            this.xrTableCell30.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[SubTotal]")});
            this.xrTableCell30.Font = new System.Drawing.Font("Verdana", 6.75F);
            this.xrTableCell30.Name = "xrTableCell30";
            this.xrTableCell30.Padding = new DevExpress.XtraPrinting.PaddingInfo(1, 1, 1, 1, 100F);
            this.xrTableCell30.StyleName = "RightAlignCell";
            this.xrTableCell30.StylePriority.UseFont = false;
            this.xrTableCell30.StylePriority.UsePadding = false;
            this.xrTableCell30.Text = "xrTableCell30";
            this.xrTableCell30.TextFormatString = "{0:$0.00}";
            this.xrTableCell30.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell30.Weight = 0.056358764891089728D;
            // 
            // xrTableCell31
            // 
            this.xrTableCell31.CanShrink = true;
            this.xrTableCell31.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ShippingCost]")});
            this.xrTableCell31.Font = new System.Drawing.Font("Verdana", 6.75F);
            this.xrTableCell31.Name = "xrTableCell31";
            this.xrTableCell31.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell31.StyleName = "RightAlignCell";
            this.xrTableCell31.StylePriority.UseFont = false;
            this.xrTableCell31.Text = "xrTableCell31";
            this.xrTableCell31.TextFormatString = "{0:$0.00}";
            this.xrTableCell31.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell31.Weight = 0.064935194096351539D;
            // 
            // xrTableCell32
            // 
            this.xrTableCell32.CanShrink = true;
            this.xrTableCell32.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[TaxCost]")});
            this.xrTableCell32.Font = new System.Drawing.Font("Verdana", 6.75F);
            this.xrTableCell32.Name = "xrTableCell32";
            this.xrTableCell32.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell32.StyleName = "RightAlignCell";
            this.xrTableCell32.StylePriority.UseFont = false;
            this.xrTableCell32.Text = "xrTableCell32";
            this.xrTableCell32.TextFormatString = "{0:$0.00}";
            this.xrTableCell32.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell32.Weight = 0.058897048605883351D;
            // 
            // xrTableCell33
            // 
            this.xrTableCell33.CanShrink = true;
            this.xrTableCell33.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[DiscountAmount]")});
            this.xrTableCell33.Font = new System.Drawing.Font("Verdana", 6.75F);
            this.xrTableCell33.Name = "xrTableCell33";
            this.xrTableCell33.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell33.StyleName = "RightAlignCell";
            this.xrTableCell33.StylePriority.UseFont = false;
            this.xrTableCell33.Text = "xrTableCell33";
            this.xrTableCell33.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell33.Weight = 0.063411221826810277D;
            // 
            // xrTableCell34
            // 
            this.xrTableCell34.CanShrink = true;
            this.xrTableCell34.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[TotalAmount]")});
            this.xrTableCell34.Font = new System.Drawing.Font("Verdana", 6.75F);
            this.xrTableCell34.Name = "xrTableCell34";
            this.xrTableCell34.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell34.StyleName = "RightAlignCell";
            this.xrTableCell34.StylePriority.UseFont = false;
            this.xrTableCell34.Text = "xrTableCell34";
            this.xrTableCell34.TextFormatString = "{0:$0.00}";
            this.xrTableCell34.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell34.Weight = 0.072790928700972118D;
            // 
            // TopMargin
            // 
            this.TopMargin.HeightF = 29.16667F;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // BottomMargin
            // 
            this.BottomMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPageInfo1,
            this.xrPageInfo2});
            this.BottomMargin.HeightF = 52.37503F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrPageInfo1
            // 
            this.xrPageInfo1.CanPublish = false;
            this.xrPageInfo1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 23.00002F);
            this.xrPageInfo1.Name = "xrPageInfo1";
            this.xrPageInfo1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrPageInfo1.PageInfo = DevExpress.XtraPrinting.PageInfo.DateTime;
            this.xrPageInfo1.SizeF = new System.Drawing.SizeF(473.8083F, 23F);
            this.xrPageInfo1.StyleName = "ReportFooterDateTime";
            // 
            // xrPageInfo2
            // 
            this.xrPageInfo2.CanPublish = false;
            this.xrPageInfo2.LocationFloat = new DevExpress.Utils.PointFloat(473.8083F, 23.00002F);
            this.xrPageInfo2.Name = "xrPageInfo2";
            this.xrPageInfo2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrPageInfo2.SizeF = new System.Drawing.SizeF(974.1901F, 23F);
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
            this.xrTable5.LocationFloat = new DevExpress.Utils.PointFloat(275.87F, 64.38F);
            this.xrTable5.Name = "xrTable5";
            this.xrTable5.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow6,
            this.xrTableRow7});
            this.xrTable5.SizeF = new System.Drawing.SizeF(970.8361F, 30.00001F);
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
            this.xrTable4.LocationFloat = new DevExpress.Utils.PointFloat(3.178914E-05F, 64.37505F);
            this.xrTable4.Name = "xrTable4";
            this.xrTable4.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow5,
            this.xrTableRow4});
            this.xrTable4.SizeF = new System.Drawing.SizeF(275.8668F, 29.99998F);
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
            this.xrLabel3.CanPublish = false;
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(3.178914E-05F, 10.00001F);
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(10, 2, 2, 2, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(221.2345F, 29.24998F);
            this.xrLabel3.StyleName = "ReportHeaderText";
            this.xrLabel3.StylePriority.UsePadding = false;
            this.xrLabel3.StylePriority.UseTextAlignment = false;
            this.xrLabel3.Text = "ORDERS";
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
            this.SubBand1.HeightF = 290.625F;
            this.SubBand1.Name = "SubBand1";
            // 
            // xrChart1
            // 
            this.xrChart1.AutoLayout = true;
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
            this.xrChart1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrChart1.Name = "xrChart1";
            this.xrChart1.PaletteName = "Flow";
            series1.ArgumentDataMember = "XAxis";
            sideBySideBarSeriesLabel1.BackColor = System.Drawing.SystemColors.Window;
            sideBySideBarSeriesLabel1.Border.Visibility = DevExpress.Utils.DefaultBoolean.False;
            sideBySideBarSeriesLabel1.LineVisibility = DevExpress.Utils.DefaultBoolean.True;
            sideBySideBarSeriesLabel1.Position = DevExpress.XtraCharts.BarSeriesLabelPosition.Top;
            sideBySideBarSeriesLabel1.TextPattern = "{V:$0.00}";
            series1.Label = sideBySideBarSeriesLabel1;
            series1.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
            series1.Name = "Series 1";
            series1.ValueDataMembersSerializable = "YAxis";
            this.xrChart1.SeriesSerializable = new DevExpress.XtraCharts.Series[] {
        series1};
            this.xrChart1.SeriesTemplate.ToolTipSeriesPattern = "{S:$0.00}";
            pieSeriesView1.ExplodeMode = DevExpress.XtraCharts.PieExplodeMode.All;
            pieSeriesView1.TotalLabel.Visible = true;
            this.xrChart1.SeriesTemplate.View = pieSeriesView1;
            this.xrChart1.SizeF = new System.Drawing.SizeF(1448F, 290.625F);
            // 
            // groupHeaderBand1
            // 
            this.groupHeaderBand1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPanel1});
            this.groupHeaderBand1.GroupUnion = DevExpress.XtraReports.UI.GroupUnion.WithFirstDetail;
            this.groupHeaderBand1.HeightF = 48.00002F;
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
            this.xrTable1.Font = new System.Drawing.Font("Verdana", 6.75F);
            this.xrTable1.LocationFloat = new DevExpress.Utils.PointFloat(7.947286E-06F, 20.00001F);
            this.xrTable1.Name = "xrTable1";
            this.xrTable1.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow1});
            this.xrTable1.SizeF = new System.Drawing.SizeF(1448F, 28F);
            this.xrTable1.StylePriority.UseFont = false;
            // 
            // xrTableRow1
            // 
            this.xrTableRow1.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell23,
            this.xrTableCell1,
            this.xrTableCell2,
            this.xrTableCell3,
            this.xrTableCell4,
            this.xrTableCell7,
            this.xrTableCell8,
            this.xrTableCell10,
            this.xrTableCell12,
            this.xrTableCell5,
            this.xrTableCell9,
            this.xrTableCell13,
            this.xrTableCell14,
            this.xrTableCell15,
            this.xrTableCell16,
            this.xrTableCell17});
            this.xrTableRow1.Name = "xrTableRow1";
            this.xrTableRow1.Weight = 1D;
            // 
            // xrTableCell23
            // 
            this.xrTableCell23.CanShrink = true;
            this.xrTableCell23.Name = "xrTableCell23";
            this.xrTableCell23.StyleName = "TableHeader";
            this.xrTableCell23.Text = "Store Name";
            this.xrTableCell23.Weight = 0.075828990991525846D;
            // 
            // xrTableCell1
            // 
            this.xrTableCell1.BackColor = System.Drawing.Color.Transparent;
            this.xrTableCell1.CanShrink = true;
            this.xrTableCell1.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold);
            this.xrTableCell1.ForeColor = System.Drawing.Color.Transparent;
            this.xrTableCell1.Name = "xrTableCell1";
            this.xrTableCell1.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell1.StyleName = "TableHeader";
            this.xrTableCell1.StylePriority.UseFont = false;
            this.xrTableCell1.Text = "Order No.";
            this.xrTableCell1.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell1.Weight = 0.062697712896730956D;
            // 
            // xrTableCell2
            // 
            this.xrTableCell2.BackColor = System.Drawing.Color.Transparent;
            this.xrTableCell2.CanShrink = true;
            this.xrTableCell2.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold);
            this.xrTableCell2.ForeColor = System.Drawing.Color.Transparent;
            this.xrTableCell2.Name = "xrTableCell2";
            this.xrTableCell2.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell2.StyleName = "TableHeader";
            this.xrTableCell2.StylePriority.UseFont = false;
            this.xrTableCell2.Text = "Date and Time";
            this.xrTableCell2.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell2.Weight = 0.068870757355785031D;
            // 
            // xrTableCell3
            // 
            this.xrTableCell3.BackColor = System.Drawing.Color.Transparent;
            this.xrTableCell3.CanShrink = true;
            this.xrTableCell3.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold);
            this.xrTableCell3.ForeColor = System.Drawing.Color.Transparent;
            this.xrTableCell3.Name = "xrTableCell3";
            this.xrTableCell3.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell3.StyleName = "TableHeader";
            this.xrTableCell3.StylePriority.UseFont = false;
            this.xrTableCell3.Text = "Order Status";
            this.xrTableCell3.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell3.Weight = 0.067341551226841975D;
            // 
            // xrTableCell4
            // 
            this.xrTableCell4.BackColor = System.Drawing.Color.Transparent;
            this.xrTableCell4.CanShrink = true;
            this.xrTableCell4.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold);
            this.xrTableCell4.ForeColor = System.Drawing.Color.Transparent;
            this.xrTableCell4.Name = "xrTableCell4";
            this.xrTableCell4.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell4.StyleName = "TableHeader";
            this.xrTableCell4.StylePriority.UseFont = false;
            this.xrTableCell4.Text = "Billing Name";
            this.xrTableCell4.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell4.Weight = 0.081471399904093328D;
            // 
            // xrTableCell7
            // 
            this.xrTableCell7.BackColor = System.Drawing.Color.Transparent;
            this.xrTableCell7.CanShrink = true;
            this.xrTableCell7.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold);
            this.xrTableCell7.ForeColor = System.Drawing.Color.Transparent;
            this.xrTableCell7.Name = "xrTableCell7";
            this.xrTableCell7.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell7.StyleName = "TableHeader";
            this.xrTableCell7.StylePriority.UseFont = false;
            this.xrTableCell7.Text = "Shipping City";
            this.xrTableCell7.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell7.Weight = 0.049367678877754291D;
            // 
            // xrTableCell8
            // 
            this.xrTableCell8.BackColor = System.Drawing.Color.Transparent;
            this.xrTableCell8.CanShrink = true;
            this.xrTableCell8.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold);
            this.xrTableCell8.ForeColor = System.Drawing.Color.Transparent;
            this.xrTableCell8.Name = "xrTableCell8";
            this.xrTableCell8.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell8.StyleName = "TableHeader";
            this.xrTableCell8.StylePriority.UseFont = false;
            this.xrTableCell8.Text = "Shipping State";
            this.xrTableCell8.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell8.Weight = 0.043281224224828573D;
            // 
            // xrTableCell10
            // 
            this.xrTableCell10.BackColor = System.Drawing.Color.Transparent;
            this.xrTableCell10.CanShrink = true;
            this.xrTableCell10.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold);
            this.xrTableCell10.ForeColor = System.Drawing.Color.Transparent;
            this.xrTableCell10.Name = "xrTableCell10";
            this.xrTableCell10.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell10.StyleName = "TableHeader";
            this.xrTableCell10.StylePriority.UseFont = false;
            this.xrTableCell10.Text = "Shipping Country";
            this.xrTableCell10.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell10.Weight = 0.068829398553931082D;
            // 
            // xrTableCell12
            // 
            this.xrTableCell12.BackColor = System.Drawing.Color.Transparent;
            this.xrTableCell12.CanShrink = true;
            this.xrTableCell12.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold);
            this.xrTableCell12.ForeColor = System.Drawing.Color.Transparent;
            this.xrTableCell12.Name = "xrTableCell12";
            this.xrTableCell12.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell12.StyleName = "TableHeader";
            this.xrTableCell12.StylePriority.UseFont = false;
            this.xrTableCell12.Text = "Billing Email ID";
            this.xrTableCell12.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell12.Weight = 0.082468141097089123D;
            // 
            // xrTableCell5
            // 
            this.xrTableCell5.BackColor = System.Drawing.Color.Transparent;
            this.xrTableCell5.BorderWidth = 0F;
            this.xrTableCell5.CanShrink = true;
            this.xrTableCell5.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold);
            this.xrTableCell5.ForeColor = System.Drawing.Color.Transparent;
            this.xrTableCell5.Name = "xrTableCell5";
            this.xrTableCell5.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell5.StyleName = "TableHeader";
            this.xrTableCell5.StylePriority.UseFont = false;
            this.xrTableCell5.Text = "Shipping Type";
            this.xrTableCell5.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrTableCell5.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell5.Weight = 0.069812749685619929D;
            // 
            // xrTableCell9
            // 
            this.xrTableCell9.BackColor = System.Drawing.Color.Transparent;
            this.xrTableCell9.BorderWidth = 0F;
            this.xrTableCell9.CanShrink = true;
            this.xrTableCell9.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold);
            this.xrTableCell9.ForeColor = System.Drawing.Color.Transparent;
            this.xrTableCell9.Name = "xrTableCell9";
            this.xrTableCell9.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell9.StyleName = "TableHeader";
            this.xrTableCell9.StylePriority.UseFont = false;
            this.xrTableCell9.Text = "Payment Type";
            this.xrTableCell9.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrTableCell9.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell9.Weight = 0.0670928480258978D;
            // 
            // xrTableCell13
            // 
            this.xrTableCell13.BackColor = System.Drawing.Color.Transparent;
            this.xrTableCell13.CanShrink = true;
            this.xrTableCell13.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold);
            this.xrTableCell13.ForeColor = System.Drawing.Color.Transparent;
            this.xrTableCell13.Name = "xrTableCell13";
            this.xrTableCell13.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell13.StyleName = "TableHeaderRightAlignCell";
            this.xrTableCell13.StylePriority.UseFont = false;
            this.xrTableCell13.Text = "Sub Total";
            this.xrTableCell13.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell13.Weight = 0.0626208948088319D;
            // 
            // xrTableCell14
            // 
            this.xrTableCell14.BackColor = System.Drawing.Color.Transparent;
            this.xrTableCell14.CanShrink = true;
            this.xrTableCell14.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold);
            this.xrTableCell14.ForeColor = System.Drawing.Color.Transparent;
            this.xrTableCell14.Name = "xrTableCell14";
            this.xrTableCell14.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell14.StyleName = "TableHeaderRightAlignCell";
            this.xrTableCell14.StylePriority.UseFont = false;
            this.xrTableCell14.Text = "Shipping Cost";
            this.xrTableCell14.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell14.Weight = 0.072150218037083561D;
            // 
            // xrTableCell15
            // 
            this.xrTableCell15.BackColor = System.Drawing.Color.Transparent;
            this.xrTableCell15.CanShrink = true;
            this.xrTableCell15.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold);
            this.xrTableCell15.ForeColor = System.Drawing.Color.Transparent;
            this.xrTableCell15.Name = "xrTableCell15";
            this.xrTableCell15.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell15.StyleName = "TableHeaderRightAlignCell";
            this.xrTableCell15.StylePriority.UseFont = false;
            this.xrTableCell15.Text = "Tax";
            this.xrTableCell15.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell15.Weight = 0.065441166705656251D;
            // 
            // xrTableCell16
            // 
            this.xrTableCell16.BackColor = System.Drawing.Color.Transparent;
            this.xrTableCell16.CanShrink = true;
            this.xrTableCell16.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold);
            this.xrTableCell16.ForeColor = System.Drawing.Color.Transparent;
            this.xrTableCell16.Name = "xrTableCell16";
            this.xrTableCell16.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell16.StyleName = "TableHeaderRightAlignCell";
            this.xrTableCell16.StylePriority.UseFont = false;
            this.xrTableCell16.Text = "Discount";
            this.xrTableCell16.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell16.Weight = 0.070456918953480663D;
            // 
            // xrTableCell17
            // 
            this.xrTableCell17.BackColor = System.Drawing.Color.Transparent;
            this.xrTableCell17.CanShrink = true;
            this.xrTableCell17.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold);
            this.xrTableCell17.ForeColor = System.Drawing.Color.Transparent;
            this.xrTableCell17.Multiline = true;
            this.xrTableCell17.Name = "xrTableCell17";
            this.xrTableCell17.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell17.StyleName = "TableHeaderRightAlignCell";
            this.xrTableCell17.StylePriority.UseFont = false;
            this.xrTableCell17.Text = "Total Amount";
            this.xrTableCell17.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell17.Weight = 0.080878807742879977D;
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
            // GroupFooter1
            // 
            this.GroupFooter1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable3});
            this.GroupFooter1.HeightF = 28F;
            this.GroupFooter1.Name = "GroupFooter1";
            // 
            // xrTable3
            // 
            this.xrTable3.Font = new System.Drawing.Font("Verdana", 6.75F);
            this.xrTable3.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrTable3.Name = "xrTable3";
            this.xrTable3.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow3});
            this.xrTable3.SizeF = new System.Drawing.SizeF(1448F, 28F);
            this.xrTable3.StylePriority.UseFont = false;
            // 
            // xrTableRow3
            // 
            this.xrTableRow3.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell35,
            this.xrTableCell36,
            this.xrTableCell37,
            this.xrTableCell38,
            this.xrTableCell39,
            this.xrTableCell40,
            this.xrTableCell41,
            this.xrTableCell42,
            this.xrTableCell44,
            this.xrTableCell45,
            this.xrTableCell46,
            this.xrTableCell47,
            this.xrTableCell48,
            this.xrTableCell49,
            this.xrTableCell50,
            this.xrTableCell51});
            this.xrTableRow3.Name = "xrTableRow3";
            this.xrTableRow3.Weight = 1D;
            // 
            // xrTableCell35
            // 
            this.xrTableCell35.CanShrink = true;
            this.xrTableCell35.Name = "xrTableCell35";
            this.xrTableCell35.StyleName = "TableHeader";
            this.xrTableCell35.Text = "Total";
            this.xrTableCell35.Weight = 0.075828990991525846D;
            // 
            // xrTableCell36
            // 
            this.xrTableCell36.BackColor = System.Drawing.Color.Transparent;
            this.xrTableCell36.CanShrink = true;
            this.xrTableCell36.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold);
            this.xrTableCell36.ForeColor = System.Drawing.Color.Transparent;
            this.xrTableCell36.Name = "xrTableCell36";
            this.xrTableCell36.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell36.StyleName = "TableHeader";
            this.xrTableCell36.StylePriority.UseFont = false;
            this.xrTableCell36.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell36.Weight = 0.062697712896730956D;
            // 
            // xrTableCell37
            // 
            this.xrTableCell37.BackColor = System.Drawing.Color.Transparent;
            this.xrTableCell37.CanShrink = true;
            this.xrTableCell37.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold);
            this.xrTableCell37.ForeColor = System.Drawing.Color.Transparent;
            this.xrTableCell37.Name = "xrTableCell37";
            this.xrTableCell37.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell37.StyleName = "TableHeader";
            this.xrTableCell37.StylePriority.UseFont = false;
            this.xrTableCell37.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell37.Weight = 0.068870746676420208D;
            // 
            // xrTableCell38
            // 
            this.xrTableCell38.BackColor = System.Drawing.Color.Transparent;
            this.xrTableCell38.CanShrink = true;
            this.xrTableCell38.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold);
            this.xrTableCell38.ForeColor = System.Drawing.Color.Transparent;
            this.xrTableCell38.Name = "xrTableCell38";
            this.xrTableCell38.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell38.StyleName = "TableHeader";
            this.xrTableCell38.StylePriority.UseFont = false;
            this.xrTableCell38.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell38.Weight = 0.067341557226725648D;
            // 
            // xrTableCell39
            // 
            this.xrTableCell39.BackColor = System.Drawing.Color.Transparent;
            this.xrTableCell39.CanShrink = true;
            this.xrTableCell39.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold);
            this.xrTableCell39.ForeColor = System.Drawing.Color.Transparent;
            this.xrTableCell39.Name = "xrTableCell39";
            this.xrTableCell39.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell39.StyleName = "TableHeader";
            this.xrTableCell39.StylePriority.UseFont = false;
            this.xrTableCell39.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell39.Weight = 0.081471404583574478D;
            // 
            // xrTableCell40
            // 
            this.xrTableCell40.BackColor = System.Drawing.Color.Transparent;
            this.xrTableCell40.CanShrink = true;
            this.xrTableCell40.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold);
            this.xrTableCell40.ForeColor = System.Drawing.Color.Transparent;
            this.xrTableCell40.Name = "xrTableCell40";
            this.xrTableCell40.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell40.StyleName = "TableHeader";
            this.xrTableCell40.StylePriority.UseFont = false;
            this.xrTableCell40.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell40.Weight = 0.049367678877754291D;
            // 
            // xrTableCell41
            // 
            this.xrTableCell41.BackColor = System.Drawing.Color.Transparent;
            this.xrTableCell41.CanShrink = true;
            this.xrTableCell41.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold);
            this.xrTableCell41.ForeColor = System.Drawing.Color.Transparent;
            this.xrTableCell41.Name = "xrTableCell41";
            this.xrTableCell41.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell41.StyleName = "TableHeader";
            this.xrTableCell41.StylePriority.UseFont = false;
            this.xrTableCell41.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell41.Weight = 0.043281224224828573D;
            // 
            // xrTableCell42
            // 
            this.xrTableCell42.BackColor = System.Drawing.Color.Transparent;
            this.xrTableCell42.CanShrink = true;
            this.xrTableCell42.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold);
            this.xrTableCell42.ForeColor = System.Drawing.Color.Transparent;
            this.xrTableCell42.Name = "xrTableCell42";
            this.xrTableCell42.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell42.StyleName = "TableHeader";
            this.xrTableCell42.StylePriority.UseFont = false;
            this.xrTableCell42.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell42.Weight = 0.068829433694955047D;
            // 
            // xrTableCell44
            // 
            this.xrTableCell44.BackColor = System.Drawing.Color.Transparent;
            this.xrTableCell44.CanShrink = true;
            this.xrTableCell44.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold);
            this.xrTableCell44.ForeColor = System.Drawing.Color.Transparent;
            this.xrTableCell44.Name = "xrTableCell44";
            this.xrTableCell44.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell44.StyleName = "TableHeader";
            this.xrTableCell44.StylePriority.UseFont = false;
            this.xrTableCell44.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell44.Weight = 0.082468105956065185D;
            // 
            // xrTableCell45
            // 
            this.xrTableCell45.BackColor = System.Drawing.Color.Transparent;
            this.xrTableCell45.BorderWidth = 0F;
            this.xrTableCell45.CanShrink = true;
            this.xrTableCell45.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold);
            this.xrTableCell45.ForeColor = System.Drawing.Color.Transparent;
            this.xrTableCell45.Name = "xrTableCell45";
            this.xrTableCell45.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell45.StyleName = "TableHeader";
            this.xrTableCell45.StylePriority.UseFont = false;
            this.xrTableCell45.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrTableCell45.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell45.Weight = 0.069812711614798281D;
            // 
            // xrTableCell46
            // 
            this.xrTableCell46.BackColor = System.Drawing.Color.Transparent;
            this.xrTableCell46.BorderWidth = 0F;
            this.xrTableCell46.CanShrink = true;
            this.xrTableCell46.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold);
            this.xrTableCell46.ForeColor = System.Drawing.Color.Transparent;
            this.xrTableCell46.Name = "xrTableCell46";
            this.xrTableCell46.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell46.StyleName = "TableHeader";
            this.xrTableCell46.StylePriority.UseFont = false;
            this.xrTableCell46.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrTableCell46.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell46.Weight = 0.067092927303642191D;
            // 
            // xrTableCell47
            // 
            this.xrTableCell47.BackColor = System.Drawing.Color.Transparent;
            this.xrTableCell47.CanShrink = true;
            this.xrTableCell47.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Sum([SubTotal])")});
            this.xrTableCell47.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold);
            this.xrTableCell47.ForeColor = System.Drawing.Color.Transparent;
            this.xrTableCell47.Name = "xrTableCell47";
            this.xrTableCell47.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell47.StyleName = "TableHeaderRightAlignCell";
            this.xrTableCell47.StylePriority.UseFont = false;
            this.xrTableCell47.Text = "Sub Total";
            this.xrTableCell47.TextFormatString = "{0:$0.00}";
            this.xrTableCell47.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell47.Weight = 0.062620946298987581D;
            // 
            // xrTableCell48
            // 
            this.xrTableCell48.BackColor = System.Drawing.Color.Transparent;
            this.xrTableCell48.CanShrink = true;
            this.xrTableCell48.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Sum([ShippingCost])")});
            this.xrTableCell48.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold);
            this.xrTableCell48.ForeColor = System.Drawing.Color.Transparent;
            this.xrTableCell48.Name = "xrTableCell48";
            this.xrTableCell48.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell48.StyleName = "TableHeaderRightAlignCell";
            this.xrTableCell48.StylePriority.UseFont = false;
            this.xrTableCell48.Text = "Shipping Cost";
            this.xrTableCell48.TextFormatString = "{0:$0.00}";
            this.xrTableCell48.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell48.Weight = 0.072150041085525529D;
            // 
            // xrTableCell49
            // 
            this.xrTableCell49.BackColor = System.Drawing.Color.Transparent;
            this.xrTableCell49.CanShrink = true;
            this.xrTableCell49.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Sum([TaxCost])")});
            this.xrTableCell49.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold);
            this.xrTableCell49.ForeColor = System.Drawing.Color.Transparent;
            this.xrTableCell49.Name = "xrTableCell49";
            this.xrTableCell49.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell49.StyleName = "TableHeaderRightAlignCell";
            this.xrTableCell49.StylePriority.UseFont = false;
            this.xrTableCell49.Text = "Tax";
            this.xrTableCell49.TextFormatString = "{0:$0.00}";
            this.xrTableCell49.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell49.Weight = 0.065441357241460524D;
            // 
            // xrTableCell50
            // 
            this.xrTableCell50.BackColor = System.Drawing.Color.Transparent;
            this.xrTableCell50.CanShrink = true;
            this.xrTableCell50.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Sum([DiscountAmount])")});
            this.xrTableCell50.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold);
            this.xrTableCell50.ForeColor = System.Drawing.Color.Transparent;
            this.xrTableCell50.Name = "xrTableCell50";
            this.xrTableCell50.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell50.StyleName = "TableHeaderRightAlignCell";
            this.xrTableCell50.StylePriority.UseFont = false;
            this.xrTableCell50.Text = "Disc";
            this.xrTableCell50.TextFormatString = "{0:$0.00}";
            this.xrTableCell50.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell50.Weight = 0.070456833972785168D;
            // 
            // xrTableCell51
            // 
            this.xrTableCell51.BackColor = System.Drawing.Color.Transparent;
            this.xrTableCell51.CanShrink = true;
            this.xrTableCell51.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Sum([TotalAmount])")});
            this.xrTableCell51.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold);
            this.xrTableCell51.ForeColor = System.Drawing.Color.Transparent;
            this.xrTableCell51.Name = "xrTableCell51";
            this.xrTableCell51.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);
            this.xrTableCell51.StyleName = "TableHeaderRightAlignCell";
            this.xrTableCell51.StylePriority.UseFont = false;
            this.xrTableCell51.Text = "Total Amount";
            this.xrTableCell51.TextFormatString = "{0:$0.00}";
            this.xrTableCell51.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell51.Weight = 0.080878878215053948D;
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
            this.objectDataSource1.DataSource = typeof(Znode.Libraries.DevExpress.Report.OrderModel);
            this.objectDataSource1.Name = "objectDataSource1";
            // 
            // Orders
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
            this.Font = new System.Drawing.Font("Verdana", 6.75F);
            this.Landscape = true;
            this.Margins = new System.Drawing.Printing.Margins(25, 27, 29, 52);
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
            ((System.ComponentModel.ISupportInitialize)(pieSeriesView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrChart1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

    }

    #endregion
}
