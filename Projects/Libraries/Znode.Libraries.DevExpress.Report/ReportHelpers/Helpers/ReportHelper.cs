using DevExpress.XtraCharts;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.WebDocumentViewer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Libraries.DevExpress.Report
{
    public class ReportHelper: IReportHelper
    {
        #region constants 
        private const string ChartType = "ChartType";
        private const string ReportHeaderBand = "reportHeaderBand1";
        private const string SubBandName = "SubBand1";
        private const string FirstTitleParam = "xrTable4";
        private const string SecondTitleParam = "xrTable5";
        private const string xrTableWareHouse = "xrTableWareHouse";
        private const string XrChartName = "xrChart1";
        private const string None = "None";
        private const string SeriesName = "Series 1";
        private const string ValueInfo = "ValueInfo";
        private const string VisibleColumnName = "VisibleColumns";
        private const string StoresName = "StoresName";
        private const string WareHousesName = "WareHousesName";
        private const string DiscountTypes = "DiscountTypes";
        private const string OrderStatusName = "OrderStatusName";
        private const string BeginDateName = "BeginDate";
        private const string BeginDate = "Begin Date";
        private const string EndDateName = "EndDate";
        private const string EndDate = "End Date";
        private const string Xaxis = "XAxis";
        private const string Yaxis = "YAxis";
        private const string DetailBandName = "DetailBand";
        private const string TableCellControlType = "XRTableCell";
        private const string GroupHeaderBandName = "GroupHeaderBand";
        private const string GroupFooterBandName = "GroupFooterBand";
        private const string ControlType = "ControlType";
        private const string RegularExpressForText = @"[^A-Za-z]+";
        private const string DateTime = "DateTime";
        private const string HtmlLinesSperator = "<br/>";
        private const string DefaultCurrencyFormat = "{0:$0.00}";
        private const string DetailsTable = "xrTable2";
        private const string NoRecordFound = "We can't find any records for the selected parameters.";
        private const string ShowAllCustomerName = "ShowAllCustomers";
        private const string TopCustomerName = "TopCustomers";
        private const string Customers = "Customers";
        Dictionary<string, string> reportsParameters = null;
        #endregion constants

        #region Private variables
        private readonly IReportFilterHelper _filterHelper;
        #endregion Private variables

        #region public properties        
        public HttpContext CurrentContext { get; set; }
        public ReportSettingModel ReportSettingModel { get; set; }
        public Func<ReportSettingModel> ReportSettignDelegate { get; set; }
        public Func<dynamic, dynamic> DataSourceDelegate { get; set; }
        public Func<string, ReportViewModel> ReportViewModelDelegate { get; set; }
        public float TableWidth { get; set; }
        public string SavedReportName { get; set; }
        public string ReportAllColumns { get; set; }
        public bool IsReload { get; set; }
        public string CurrencySymbol { get; set; }
        #endregion public properties

        #region Constructor
        public ReportHelper(IReportFilterHelper filterHelper)
        {
            _filterHelper = filterHelper;
        }
        #endregion Constructor

        #region public Method

        //Generate report
        public ReportModel GenerateReport(string reportCode, string reportName, HttpContext currentContext, Func<ReportSettingModel> reportSetting, Func<dynamic, dynamic> dataSourcefun, Func<string, ReportViewModel> reportViewModel)
        {
            ReportSettignDelegate = reportSetting;
            DataSourceDelegate = dataSourcefun;
            ReportViewModelDelegate = reportViewModel;
            CurrentContext = currentContext;

            var reportObject = new ReportModel
            {
                ReportObject = (XtraReport)Assembly.GetExecutingAssembly().CreateInstance(reportCode),
                //Commented the code for next phase
                CustomButtons = GetCustomButtons()
            };

            //Configure report setting
            ConfigureReportSetting(reportObject);

            //Add report filters in report.
            GenerateReportFilters(reportObject);

            //To auto bind the report
            reportObject.ReportObject.RequestParameters = false;

            var reportExportOptionsHelper = new ReportExportOptionsHelper();
            reportExportOptionsHelper.SetDefaultExportOptionsSetting(reportObject.ReportObject);

            return reportObject;

        }
        #endregion public Method      

        #region Private Method

        //After clicking on submit button of report filter menu this event will fire. 
        private void ReportObject_DataSourceDemanded(object sender, EventArgs e)
        {
            try
            {

                //Set current context to httpcontext.
                HttpContext.Current = CurrentContext;
                var xtraReport = sender as XtraReport;

                if(HelperUtility.IsNotNull(xtraReport))
                {
                    ValidateParameterValues(xtraReport);

                    var dataSource = DataSourceDelegate.Invoke(GetParameters(xtraReport?.Parameters));

                    CurrencySymbol = (dataSource.Count > 0 && dataSource[0].GetType().GetProperty("Symbol") != null ? dataSource[0].Symbol : string.Empty);

                    xtraReport.DataSource = dataSource;

                    GetReportParametersValues(xtraReport);

                    ConfigureGraphSettingOnReport(xtraReport);

                    //Set column Visibility
                    if (HelperUtility.IsNotNull(ReportSettingModel.DefaultLayoutXML))
                    {
                        ConfirgureReportLayout(xtraReport);
                    }

                    //Load stylesheet file 
                    if (HelperUtility.IsNotNull(ReportSettingModel.StyleSheetXml))
                        xtraReport.StyleSheet.LoadFromStream(new ReportThemeHelper().ConvertStyleXmlToStream(ReportSettingModel.StyleSheetXml.ToString()));

                    //Set some of parameters default value.
                    _filterHelper.SetParameterDefaultValues(xtraReport.Parameters);
                    //Assign report parameters values to report controls
                    AssignReportParaValsToReportControls(xtraReport);

                    if (dataSource.Count == 0)
                    {
                        AddEmptyRow(xtraReport);
                    }
                    xtraReport.ApplyFiltering();
                    SaveReportHelper.CurrentReport = xtraReport;
                }
            } catch(Exception)
            {
                throw;
            }
        }

        protected void AddEmptyRow(XtraReport xtraReport)
        {
            XRTable detailsTable = xtraReport.FindControl(DetailsTable, true) as XRTable;
            if(detailsTable.Rows.Count>0)
            {
                detailsTable.Rows[0].Cells[0].Controls.Add(new XRLabel() { Text = NoRecordFound,WidthF = 500});
                detailsTable.Rows[0].Cells[0].CanGrow = true;
                detailsTable.Rows[0].Cells[0].WidthF = 550;
            }
            
        }   

        public string GetSubstringByString(string a, string b, string c)    
        {
            return c.Substring((c.LastIndexOf(a) + a.Length), (c.LastIndexOf(b) - c.LastIndexOf(a) - a.Length));
        }

        private void ValidateParameterValues(XtraReport xtraReport)
        {
            var errorSummery = string.Empty;
            foreach(var parameter in xtraReport.Parameters)
            {
                if(parameter.Name.Equals(StoresName))
                {
                    if(((string[])(xtraReport.Parameters[StoresName].Value)).Length == 0)
                    {
                        errorSummery = string.Concat(errorSummery, string.Format(Admin_Resources.ZReportStoreParaRequiredValidation + HtmlLinesSperator));
                    } else
                    {
                        var dataSource = (string[])(xtraReport.Parameters[StoresName].Value);
                        if(dataSource.FirstOrDefault().Contains("("))
                        {
                            List<string> symbols = new List<string>();
                            foreach(var item in dataSource)
                            {
                                symbols.Add(GetSubstringByString("(", ")", item));
                            }
                            if(symbols.Distinct().Count() > 1)
                            {
                                errorSummery = string.Concat(errorSummery, string.Format(Admin_Resources.ZReportStoreParaCurrencyValidation + HtmlLinesSperator)) ;
                            }
                        }
                    }
                } else if(parameter.Name.Equals(WareHousesName))
                {
                    if(((string[])xtraReport.Parameters[WareHousesName].Value).Length == 0)
                    {
                        errorSummery = string.Concat(errorSummery, string.Format(Admin_Resources.ZReportWareHouseParamValidation + HtmlLinesSperator));
                    }
                }
                else if (parameter.Name.Equals(DiscountTypes))
                {
                    if (((string[])xtraReport.Parameters[DiscountTypes].Value).Length == 0)
                    {
                        errorSummery = string.Concat(errorSummery, string.Format(Admin_Resources.ZReportDiscountTypeParamValidation + HtmlLinesSperator));
                    }
                }
                else if(parameter.Name.Equals(BeginDateName) || parameter.Name.Equals(EndDateName))
                {
                    if(((DateTime)xtraReport.Parameters[parameter.Name].Value).Year <= 1900)
                    {
                        errorSummery = string.Concat(errorSummery, string.Format(Admin_Resources.ZReportDateYearGreaterThan + HtmlLinesSperator, parameter.Description));
                    }
                } else if(parameter.Name.Equals(OrderStatusName))
                {
                    if(((string[])xtraReport.Parameters[OrderStatusName].Value).Length == 0)
                    {
                        errorSummery = string.Concat(errorSummery, string.Format(Admin_Resources.ZReportOrderStatusParamValidation + HtmlLinesSperator));
                    }
                } else if(parameter.Name.Equals(VisibleColumnName) && ((string[])xtraReport.Parameters[VisibleColumnName].Value).Length == 0)
                {
                    errorSummery = string.Concat(errorSummery, string.Format(Admin_Resources.ZReportVisibleColumnParamValidation + HtmlLinesSperator));
                }
            }
            if(xtraReport.Parameters[BeginDateName] != null && xtraReport.Parameters[EndDateName] != null && 
                ((DateTime)xtraReport.Parameters[BeginDateName].Value > (DateTime)xtraReport.Parameters[EndDateName].Value))
            {
                errorSummery = string.Concat(errorSummery, string.Format(Admin_Resources.ZReportBeginDateLessThanEndDate + HtmlLinesSperator, BeginDate, EndDate));
            }

            if (xtraReport.Parameters[ShowAllCustomerName] != null && xtraReport.Parameters[TopCustomerName] != null &&
                !(bool)xtraReport.Parameters[ShowAllCustomerName].Value && (int)xtraReport.Parameters[TopCustomerName].Value <= 0)
            {
                errorSummery = string.Concat(errorSummery, string.Format(Admin_Resources.ZReportShowAllTopCustomerValidation + HtmlLinesSperator, Customers));
            }

            if (!string.IsNullOrEmpty(errorSummery))
                throw new ArgumentException(errorSummery);
        }

        private void GetReportParametersValues(XtraReport xtraReport)
        {
            reportsParameters = new Dictionary<string, string>();

            foreach (var Parameter in xtraReport.Parameters)    
            {
                reportsParameters.Add(Parameter.Name, Parameter.Type.Name.Equals(DateTime) ? Parameter.Value.ToString() : Convert.ToString(Parameter.GetProperty(ValueInfo)));
            }
        }

        //This method is used to assign report parameters values to report components.
        private void AssignReportParaValsToReportControls(XtraReport xtraReport)
        {

            ReportHeaderBand reportHeaderBand = xtraReport.FindControl(ReportHeaderBand, true) as ReportHeaderBand;
            if(HelperUtility.IsNotNull(reportHeaderBand))
            {
                XRTable xrTable = reportHeaderBand?.FindControl(FirstTitleParam, true) as XRTable;
                if(HelperUtility.IsNotNull(xrTable))
                {
                    xrTable.Rows[xrTable.Rows.Count - 1].Cells[0].Text = string.Format("{0} - {1}", reportsParameters[BeginDateName], reportsParameters[EndDateName]);
                }
                SetParamValueToReportHeaderControl(reportHeaderBand, SecondTitleParam, StoresName);
                SetParamValueToReportHeaderControl(reportHeaderBand, xrTableWareHouse, WareHousesName);
            }
        }

        private XRTable SetParamValueToReportHeaderControl(ReportHeaderBand reportHeaderBand, string tableName, string parameterName)
        {
            XRTable xrTable = reportHeaderBand?.FindControl(tableName, true) as XRTable;
            if(HelperUtility.IsNotNull(xrTable))
                xrTable.Rows[xrTable.Rows.Count - 1].Cells[0].Text = reportsParameters[parameterName].Replace("|", ", ");

            return xrTable;
        }

        //This method is used to show/hide report columns.
        private void ConfirgureReportLayout(XtraReport xtraReport)
        {
            var parameter = xtraReport.Parameters[VisibleColumnName];

            if(parameter != null)
            {
                string valuesString = Convert.ToString(parameter.GetProperty(ValueInfo));
                if(!string.IsNullOrEmpty(valuesString))
                {
                    var values = valuesString.Split('|');
                    var allcolumns = ReportSettingModel.MustHideColumns.Select(x => x.Value).ToArray();
                    var reorderList = ReorderVisibleColumnsValues(values, allcolumns);
                    ReportSettingModel.DefaultLayoutXML = SetCurrencyFormat(ReportSettingModel.DefaultLayoutXML);
                    var doc = XDocument.Parse(ReportSettingModel.DefaultLayoutXML);
                    foreach(var value in allcolumns)
                    {
                        if(!values.Contains(value))
                        {
                            XElement selectedElement = doc.Descendants()?.FirstOrDefault(x => (string)x.Attribute("Text") == value);

                            RemoveXElement(doc, selectedElement, new string[] { DetailBandName, GroupHeaderBandName, GroupFooterBandName });
                        }
                    }
                    for(var index = 1; index <= reorderList.Count; index++)
                    {
                        string selectedElementName = doc.Descendants()?.FirstOrDefault(x => (string)x.Attribute("Text") == reorderList[index - 1])?.Name.ToString();

                        ReorderReportTableComponents(doc, xtraReport, reorderList, index, selectedElementName, new string[] { DetailBandName, GroupHeaderBandName, GroupFooterBandName });
                        
                    }
                    xtraReport.LoadLayoutFromXml(GenerateStreamFromString(doc.ToString()));

                }
            }

        }

        private string SetCurrencyFormat(string xmlString)
        {
            if(string.IsNullOrEmpty(CurrencySymbol))
                return xmlString;
            return xmlString.Replace(DefaultCurrencyFormat, "{0:" + CurrencySymbol + SetDecimalPointPosition(Convert.ToInt16(ReportSettingModel.DefaultPriceRoundOff)) + "}");
        }

        private string SetDecimalPointPosition(int roundOff)
        {
            string formatString = "0";
            if(roundOff > 0)
                formatString += ".";
            for(int i = 0; i < roundOff; i++)
            {
                formatString = string.Concat(formatString, "0");
            }
            return formatString;
        }

        //This method is used to rearrange the reports columns.
        private void ReorderReportTableComponents(XDocument doc, XtraReport xtraReport, List<string> reorderList, int index, string selectedElementName, string[] BandNames)
        {
            foreach (var BandName in BandNames)
            {
                XElement detailBandElement = doc.Descendants()?.FirstOrDefault(x => (string)x.Attribute(ControlType) == BandName);
                XElement xElement = detailBandElement?.Descendants(selectedElementName)?.FirstOrDefault(x => (string)x.Attribute(ControlType) == TableCellControlType);

                if (HelperUtility.IsNotNull(xElement))
                {
                    xElement.Name = Regex.Replace(xElement.Name.ToString(), RegularExpressForText, String.Empty) + index.ToString();
                    XAttribute attribute = new XAttribute("WidthF", xtraReport.PageWidth / reorderList.Count);
                    xElement.Add(attribute);
                }
            }               
        }

        //This method is used to remove column schema from report schema.
        private void RemoveXElement(XDocument doc, XElement selectedElement, string[] BandNames)
        {
            foreach (var BandName in BandNames)
            {
                XElement bandElement = doc.Descendants()?.FirstOrDefault(x => (string)x.Attribute(ControlType) == BandName);
                var xElement = bandElement?.Descendants(selectedElement?.Name)?.FirstOrDefault(x => (string)x.Attribute(ControlType) == TableCellControlType);
                xElement?.Remove();
            }       
        }

        //This method is used to rearrange the report columns
        private List<string> ReorderVisibleColumnsValues(string[] visibleCols, string[] AllCols)
        {
            List<string> reorderList = new List<string>();
            foreach(var Col in AllCols)
            {
                if(visibleCols.Contains(Col))

                    reorderList.Add(Col);
            }
            return reorderList;
        }

        private ParameterCollection GetParameters(ParameterCollection parameters)
        {
            ParameterCollection newparameters = new ParameterCollection();
            int index = 0;
            foreach(var parameter in parameters)
            {
                if(!parameter.Name.Equals(FilterKeys.VisibleColumns) && !parameter.Name.Equals(FilterKeys.ChartType)
                    && !parameter.Name.Equals(FilterKeys.XAxis) && !parameter.Name.Equals(FilterKeys.YAxis))
                {
                    newparameters.Insert(index, parameter);
                    index += 1;
                }
            }
            return newparameters;
        }

        //Set report settings. 
        private void ConfigureReportSetting(ReportModel report)
        {
            //Get report setting model from API.
            ReportSettingModel = GetReportSetting();

            // Set report landscape property
            report.ReportObject.Landscape = ReportSettingModel.DisplayMode;

            if(HelperUtility.IsNotNull(ReportSettingModel.DefaultLayoutXML))
                report.ReportObject.LoadLayoutFromXml(GenerateStreamFromString(ReportSettingModel.DefaultLayoutXML));


            //Set some of parameters default value.
            if(report.ReportObject.Parameters.Count > 0)
                _filterHelper.SetParameterDefaultValues(report.ReportObject.Parameters);           

            //Data source demanded event register to report.            
            report.ReportObject.DataSourceDemanded += ReportObject_DataSourceDemanded;

        }

        private Stream GenerateStreamFromString(string s)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(s));
        }

        //This is used to display graph on the report according to values, choose through parameters   
        private void ConfigureGraphSettingOnReport(XtraReport xtraReport)
        {

            var chartType = xtraReport.Parameters[ChartType];
            if(HelperUtility.IsNotNull(chartType) && HelperUtility.IsNotNull(chartType.Value))
            {
                string chartdisplayMode = Convert.ToString(chartType.Value);
                SubBand subBand = xtraReport.FindControl(SubBandName, true) as SubBand;
                if(HelperUtility.IsNotNull(subBand))
                {
                    subBand.Visible = true;
                    XRChart xrChart = xtraReport.FindControl(XrChartName, true) as XRChart;
                    if(HelperUtility.IsNotNull(xrChart))
                    {
                        if(string.IsNullOrEmpty(chartdisplayMode) || chartdisplayMode.Equals(None))
                        {
                            subBand.Visible = false;
                        } else
                        {
                            ChangeReportChartView(chartdisplayMode, xrChart);
                            AxisConfiguration(xtraReport, xrChart);

                        }
                    }
                }
            } else
            {
                SubBand subBand = xtraReport.FindControl(SubBandName, true) as SubBand;
                if(HelperUtility.IsNotNull(subBand))
                    subBand.Visible = false;
            }
        }

        //This method is used to set axis of graph in the report as per selection of parameters
        private void AxisConfiguration(XtraReport xtraReport, XRChart xrChart)
        {
            //Change the x and y axis mapping according to the parameters of report
            var xaxisParam = xtraReport.Parameters[Xaxis];
            var chartSeries = xrChart.GetSeriesByName(SeriesName);
            var yaxisParam = xtraReport.Parameters[Yaxis];
            if(HelperUtility.IsNotNull(xaxisParam) && !string.IsNullOrEmpty(Convert.ToString(xaxisParam.Value))
                && HelperUtility.IsNotNull(yaxisParam) && !string.IsNullOrEmpty(Convert.ToString(yaxisParam.Value))
                )
            {
                chartSeries.ArgumentDataMember = Xaxis;
                AssignDatasourceToChart(xtraReport, xrChart, Regex.Replace(Convert.ToString(xaxisParam.Value), @"\s+", ""), Regex.Replace(Convert.ToString(yaxisParam.Value), @"\s+", ""));
            } else

                chartSeries.SetDataMembers(chartSeries.ArgumentDataMember, Regex.Replace(Convert.ToString(yaxisParam.Value), @"\s+", ""));

        }

        public void AssignDatasourceToChart(XtraReport xtraReport, XRChart xrChart, string XColumnName, string YColumnName)
        {
            var datasource = xtraReport.DataSource as IEnumerable<object>;
            var data = datasource.GroupBy(d => d.GetProperty(XColumnName), d => d.GetProperty(YColumnName),
                    (a, b) => new {
                        XAxis = a,
                        YAxis = b.Sum(x => Math.Round(Convert.ToDecimal(x), 2))
                    }).ToList();
            xrChart.DataSource = data;
        }


        //Update the chart view type according to chart type chose through parameter window. 
        private void ChangeReportChartView(string chartdisplayMode, XRChart xrChart) =>
                xrChart.GetSeriesByName(SeriesName)?.ChangeView((ViewType)Enum.Parse(typeof(ViewType), chartdisplayMode, true));

        private List<WebDocumentViewerMenuItem> GetCustomButtons()
        {
            var buttons = new List<WebDocumentViewerMenuItem>();
            buttons.Add(new WebDocumentViewerMenuItem {
                Text = "Save",
                ImageClassName = "SaveButtonIcon",
                JSClickAction = "function() { DevExpressReport.prototype.ShowSaveLayoutPopup();}",
                HasSeparator = true
            });
            buttons.Add(new WebDocumentViewerMenuItem {
                Text = "Open",
                ImageClassName = "OpenButtonIcon",
                JSClickAction = "function() { DevExpressReport.prototype.ViewSavedHistories(); }",
            });
            return buttons;
        }
        #endregion Private Method

        #region Protected Method

        // Get report settings and filters
        protected ReportSettingModel GetReportSetting() => _filterHelper.GetReportSetting(ReportSettignDelegate);


        //Get report parameters from database and add these into report object.
        protected void GenerateReportFilters(ReportModel report)
        {
            _filterHelper.AddParameters(report, ReportSettingModel);
            report.MustHideColumns = String.Join(",", ReportSettingModel.MustHideColumns.Select(x => x.Value));
        }

        public string SaveReportIntoDatabase()
        {
            string tmpFileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".repx";
            SaveReportHelper.CurrentReport.SaveLayoutToXml(tmpFileName);
            string xmlXtraReportLayout = System.IO.File.ReadAllText(tmpFileName);
            System.IO.File.Delete(tmpFileName);
            return xmlXtraReportLayout;
        }



        #endregion Protected Method
    }


}
