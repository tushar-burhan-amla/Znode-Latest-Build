using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Libraries.Admin
{
    public class ReportGenerator
    {
        #region Private Variables
        private readonly string DataSetName = "DynamicDataSet";
        private readonly string ReportGroupName = "Table1_Details_Group";
        private readonly string ReportHeaderImage = "Image1";
        private readonly string ReportFooterPage = "PageNumber";
        private readonly string DataSourceNameWithPath = $"/Data Sources/{ConfigurationManager.AppSettings["ReportDataSetName"].ToString()}";
        private readonly string DataSourceName = ConfigurationManager.AppSettings["ReportDataSetName"].ToString();
        #endregion

        #region Public Methods

        /// <summary>
        /// Generate the RDL report
        /// </summary>
        /// <param name="doc">XmlDocument</param>
        /// <param name="parameterList">ArrayList</param>
        /// <param name="model">DynamicReportModel</param>
        public virtual void GenerateRDLReport(XmlDocument doc, Dictionary<string, string> parameterList, ArrayList columnList, DynamicReportModel model)
        {
            if (model.ReportType.ToLower().Contains("product") || model.ReportType.ToLower().Contains("category"))
                GeneratePivotReport(doc, parameterList, columnList, model);
            else
                GenerateNormalReport(doc, parameterList, columnList, model);
        }

        #endregion

        #region Private Methods

        protected virtual void GenerateNormalReport(XmlDocument doc, Dictionary<string, string> parameterList, ArrayList columnList, DynamicReportModel model)
        {
            XmlElement tablix;
            XmlElement report = AddReportElement(doc);
            AddDataSourceElements(doc, report);
            XmlElement dataset = AddDataSetElements(doc, report, parameterList, model.StoredProcedureName);
            CreateFieldElements(dataset, doc, columnList);
            XmlElement reportItems = AddReportSectionElement(report, doc);
            XmlElement tablixBody = AddTablixElements(reportItems, doc, out tablix);
            CreateTablixColumns(tablixBody, model.Columns);
            XmlElement tablixRows = AddElement(tablixBody, "TablixRows", null);

            CreateTablixElements(tablixRows, doc, model.Columns);
            AddTablixRowElements(tablixRows, doc, model.Columns);
            CreateTablixHierarchy(tablix, doc, model.Columns);
            CreateReportParamSection(report, doc, parameterList);
        }

        protected virtual void GeneratePivotReport(XmlDocument doc, Dictionary<string, string> parameterList, ArrayList columnList, DynamicReportModel model)
        {
            XmlElement report = AddReportElement(doc);
            AddDataSourceElements(doc, report);
            XmlElement dataset = AddDataSetElements(doc, report, parameterList, model.StoredProcedureName);
            CreateFieldElements(dataset, doc, columnList);
            AddBodyElementForPivot(doc, report, columnList, model);
        }

        #region General Functions for Reports

        protected virtual XmlElement AddReportElement(XmlDocument doc)
        {
            string xmlData = "<Report " +
            "xmlns=\"http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition\">" +
                "</Report>";
            doc.Load(new StringReader(xmlData));

            XmlElement report = (XmlElement)doc.FirstChild;
            AddElement(report, "AutoRefresh", "0");
            AddElement(report, "ConsumeContainerWhitespace", "true");
            return report;
        }

        protected virtual void AddDataSourceElements(XmlDocument doc, XmlElement report)
        {
            XmlElement dataSources = AddElement(report, "DataSources", null);
            XmlElement dataSource = AddElement(dataSources, "DataSource", null);
            XmlAttribute attr = dataSource.Attributes.Append(doc.CreateAttribute("Name"));
            attr.Value = DataSourceName;

            AddElement(dataSource, "DataSourceReference", DataSourceNameWithPath);
        }

        protected virtual XmlElement AddDataSetElements(XmlDocument doc, XmlElement report, Dictionary<string, string> parameterList, string spName)
        {
            XmlElement dataSets = AddElement(report, "DataSets", null);
            XmlElement dataSet = AddElement(dataSets, "DataSet", null);
            XmlAttribute attr = dataSet.Attributes.Append(doc.CreateAttribute("Name"));
            attr.Value = DataSetName;

            //Query element
            XmlElement query = AddElement(dataSet, "Query", null);
            CreateQueryParam(query, doc, parameterList);
            AddElement(query, "DataSourceName", DataSourceName);
            AddElement(query, "CommandText", spName);
            AddElement(query, "CommandType", "StoredProcedure");
            AddElement(query, "Timeout", "60");

            return dataSet;
        }

        protected virtual void CreateQueryParam(XmlElement query, XmlDocument doc, Dictionary<string, string> parameterList)
        {
            if (parameterList.Count > 0)
            {
                XmlElement QueryParameters = AddElement(query, "QueryParameters", null);
                XmlElement QueryParameter = null;
                XmlAttribute attr = null;

                foreach (var item in parameterList)
                {
                    QueryParameter = AddElement(QueryParameters, "QueryParameter", null);
                    attr = QueryParameter.Attributes.Append(doc.CreateAttribute("Name"));
                    attr.Value = string.Concat("@", item.Key.ToString().Trim());
                    AddElement(QueryParameter, "Value", item.Value.ToString().Trim());                    
                }
            }
        }

        protected virtual void CreateFieldElements(XmlElement dataset, XmlDocument doc, ArrayList columns)
        {
            XmlElement fields = AddElement(dataset, "Fields", null);
            XmlElement field = null;
            XmlAttribute attr = null;

            foreach (var column in columns)
            {
                field = AddElement(fields, "Field", null);
                attr = field.Attributes.Append(doc.CreateAttribute("Name"));
                attr.Value = column.ToString().Trim();
                AddElement(field, "DataField", column.ToString().Trim());
            }
        }

        protected virtual XmlElement AddElement(XmlElement parent, string name, string value)
        {
            XmlElement newelement = parent.OwnerDocument.CreateElement(name,
                "http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition");
            parent.AppendChild(newelement);
            if (value != null) newelement.InnerText = value;
            return newelement;
        }
        #endregion

        #region Functions for Pivot Reports

        protected virtual void AddBodyElementForPivot(XmlDocument doc, XmlElement report, ArrayList columnList, DynamicReportModel model)
        {
            XmlElement reportSections = AddElement(report, "ReportSections", null);
            XmlElement reportSection = AddElement(reportSections, "ReportSection", null);

            XmlElement body = AddElement(reportSection, "Body", null);
            XmlElement reportItems = AddElement(body, "ReportItems", null);
            AddHeaderTextbox(doc, reportItems, model);
            CreateTablixElementsForPivot(doc, reportItems, model, columnList);
            AddElement(body, "Height", "0.78in");
            AddElement(reportSection, "Width", "5in");

            //Page elements
            XmlElement page =  AddElement(reportSection, "Page", null);
            AddElement(page, "LeftMargin", "1in");
            AddElement(page, "RightMargin", "1in");
            AddElement(page, "TopMargin", "1in");
            AddElement(page, "BottomMargin", "1in");

        }

        protected virtual void CreateTablixElementsForPivot(XmlDocument doc, XmlElement reportItems, DynamicReportModel model, ArrayList columnList)
        {
            XmlElement reportTablix = AddElement(reportItems, "Tablix", null);
            XmlAttribute attr = reportTablix.Attributes.Append(doc.CreateAttribute("Name"));
            attr.Value = "Matrix1";
            CreateTablixCorner(doc, reportTablix);
            CreateTablixBody(doc, reportTablix, columnList);
            //Create tablix column hierarchy
            CreateTablixHierarchy(doc, reportTablix, columnList[1].ToString(), "Column");
            //Create tablix row hierarchy
            CreateTablixHierarchy(doc, reportTablix, columnList[2].ToString(), "Row");
            
            AddElement(reportTablix, "RepeatColumnHeaders", "true");
            AddElement(reportTablix, "RepeatRowHeaders", "true");
            AddElement(reportTablix, "DataSetName", DataSetName);
            AddElement(reportTablix, "Top", "0.36in");
            AddElement(reportTablix, "Height", "0.42in");
            AddElement(reportTablix, "Width", "2in");
            AddElement(reportTablix, "ZIndex", "1");

        }

        protected virtual void CreateTablixHierarchy(XmlDocument doc, XmlElement reportTablix, string columnValue, string tablixType)
        {
            XmlElement reportTablixColumnHierarchy = AddElement(reportTablix, $"Tablix{tablixType}Hierarchy", null);
            XmlElement reportTablixMembers = AddElement(reportTablixColumnHierarchy, "TablixMembers", null);
            XmlElement reportTablixMember = AddElement(reportTablixMembers, "TablixMember", null);
            XmlElement group = AddElement(reportTablixMember, "Group", null);
            XmlAttribute attr = group.Attributes.Append(doc.CreateAttribute("Name"));
            attr.Value = $"Matrix1_{columnValue.Trim()}";
            XmlElement groupExpressions = AddElement(group, "GroupExpressions", null);
            XmlElement groupExpression = AddElement(groupExpressions, "GroupExpression", $"=Fields!{columnValue.Trim()}.Value");

            XmlElement sortExpressions = AddElement(reportTablixMember, "SortExpressions", null);
            XmlElement sortExpression = AddElement(sortExpressions, "SortExpression", null);
            AddElement(sortExpression, "Value", null);

            XmlElement tablixHeader = AddElement(reportTablixMember, "TablixHeader", null);
            AddElement(tablixHeader, "Size", "0.21in");
            XmlElement tablixCellContents = AddElement(tablixHeader, "CellContents", null);
            XmlElement tablixCellTextbox = AddElement(tablixCellContents, "Textbox", null);
            attr = tablixCellTextbox.Attributes.Append(doc.CreateAttribute("Name"));
            attr.Value = columnValue.Trim();
            AddElement(tablixCellTextbox, "CanGrow", "true");
            XmlElement tablixCellTextboxParagraphs = AddElement(tablixCellTextbox, "Paragraphs", null);
            XmlElement tablixCellTextboxParagraph = AddElement(tablixCellTextboxParagraphs, "Paragraph", null);
            XmlElement cellRuns = AddElement(tablixCellTextboxParagraph, "TextRuns", null);
            XmlElement cellRun = AddElement(cellRuns, "TextRun", null);
            XmlElement cellValue = AddElement(cellRun, "Value", $"=First(Fields!{columnValue.Trim()}.Value)");
            XmlElement cellStyle = AddElement(cellRun, "Style", null);
            AddElement(cellStyle, "FontFamily", "Segoe UI");
            AddElement(cellStyle, "FontWeight", "Bold");
            AddElement(cellStyle, "Color", "#ffffff");

            XmlElement cellParagraphStyle = AddElement(tablixCellTextbox, "Style", null);
            XmlElement cellStyleBorder = AddElement(cellParagraphStyle, "Border", null);
            AddElement(cellStyleBorder, "Color", "#dddddd");
            AddElement(cellStyleBorder, "Style", "Solid");

            AddElement(cellParagraphStyle, "BackgroundColor", "#29343b");
            AddElement(cellParagraphStyle, "PaddingLeft", "5pt");
            AddElement(cellParagraphStyle, "PaddingRight", "2pt");
            AddElement(cellParagraphStyle, "PaddingTop", "1pt");
            AddElement(cellParagraphStyle, "PaddingBottom", "1pt");

            AddElement(reportTablixMember, "DataElementOutput", "Output");
        }

        protected virtual void CreateTablixBody(XmlDocument doc, XmlElement reportTablix, ArrayList columnList)
        {
            XmlElement reportTablixBody = AddElement(reportTablix, "TablixBody", null);
            XmlElement reportTablixColumns = AddElement(reportTablixBody, "TablixColumns", null);
            XmlElement reportTablixColumn = AddElement(reportTablixColumns, "TablixColumn", null);
            AddElement(reportTablixColumn, "Width", "1in");
            XmlElement reportTablixRows = AddElement(reportTablixBody, "TablixRows", null);
            XmlElement reportTablixRow = AddElement(reportTablixRows, "TablixRow", null);
            AddElement(reportTablixRow, "Height", "0.21in");
            XmlElement reportTablixCells = AddElement(reportTablixRow, "TablixCells", null);
            XmlElement reportTablixCell = AddElement(reportTablixCells, "TablixCell", null);
            XmlElement reportTablixCellContents = AddElement(reportTablixCell, "CellContents", null);
            XmlElement reportTablixCellTextbox = AddElement(reportTablixCellContents, "Textbox", null);
            XmlAttribute attr = reportTablixCellTextbox.Attributes.Append(doc.CreateAttribute("Name"));
            attr.Value = "CellTextbox1";
            AddElement(reportTablixCellTextbox, "CanGrow", "true");
            XmlElement cellParagraphs = AddElement(reportTablixCellTextbox, "Paragraphs", null);
            XmlElement cellParagraph = AddElement(cellParagraphs, "Paragraph", null);
            XmlElement cellRuns = AddElement(cellParagraph, "TextRuns", null);
            XmlElement cellRun = AddElement(cellRuns, "TextRun", null);
            XmlElement cellValue = AddElement(cellRun, "Value", $"=First(Fields!{columnList[0].ToString()}.Value)");
            XmlElement cellStyle = AddElement(cellRun, "Style", null);
            AddElement(cellStyle, "FontFamily", "Segoe UI");

            XmlElement cellParagraphStyle = AddElement(reportTablixCellTextbox, "Style", null);
            XmlElement cellStyleBorder = AddElement(cellParagraphStyle, "Border", null);
            AddElement(cellStyleBorder, "Color", "#dddddd");
            AddElement(cellStyleBorder, "Style", "Solid");

            AddElement(cellParagraphStyle, "PaddingLeft", "5pt");
            AddElement(cellParagraphStyle, "PaddingRight", "2pt");
            AddElement(cellParagraphStyle, "PaddingTop", "1pt");
            AddElement(cellParagraphStyle, "PaddingBottom", "1pt");

            AddElement(reportTablixCell, "DataElementOutput", "Output");
        }

        protected virtual void CreateTablixCorner(XmlDocument doc, XmlElement reportTablix)
        {
            XmlElement tablixCorner = AddElement(reportTablix, "TablixCorner", null);
            XmlElement tablixCornerRows = AddElement(tablixCorner, "TablixCornerRows", null);
            XmlElement tablixCornerRow = AddElement(tablixCornerRows, "TablixCornerRow", null);
            XmlElement tablixCornerCell = AddElement(tablixCornerRow, "TablixCornerCell", null);
            XmlElement cellContents = AddElement(tablixCornerCell, "CellContents", null);
            XmlElement cellTextbox = AddElement(cellContents, "Textbox", null);
            XmlAttribute attr = cellTextbox.Attributes.Append(doc.CreateAttribute("Name"));
            attr.Value = "CornerCell1";
            XmlElement cellParagraphs = AddElement(cellTextbox, "Paragraphs", null);
            XmlElement cellParagraph = AddElement(cellParagraphs, "Paragraph", null);
            XmlElement cellRuns = AddElement(cellParagraph, "TextRuns", null);
            XmlElement cellRun = AddElement(cellRuns, "TextRun", null);
            XmlElement cellValue = AddElement(cellRun, "Value", null);
            XmlElement cellStyle = AddElement(cellRun, "Style", null);
            AddElement(cellStyle, "FontFamily", "Segoe UI");

            XmlElement cellParagraphStyle = AddElement(cellTextbox, "Style", null);
            XmlElement cellStyleBorder = AddElement(cellParagraphStyle, "Border", null);
            AddElement(cellStyleBorder, "Color", "#dddddd");
            AddElement(cellStyleBorder, "Style", "Solid");

            AddElement(cellParagraphStyle, "PaddingLeft", "2pt");
            AddElement(cellParagraphStyle, "PaddingRight", "2pt");
            AddElement(cellParagraphStyle, "PaddingTop", "1pt");
            AddElement(cellParagraphStyle, "PaddingBottom", "1pt");
        }

        protected virtual void AddHeaderTextbox(XmlDocument doc, XmlElement reportItems, DynamicReportModel model)
        {
            XmlElement textBox = AddElement(reportItems, "Textbox", null);
            XmlAttribute attr = textBox.Attributes.Append(doc.CreateAttribute("Name"));
            attr.Value = "ReportHeader";

            AddElement(textBox, "CanGrow", "true");
            AddElement(textBox, "KeepTogether", "true");

            XmlElement paragraphs =  AddElement(textBox, "Paragraphs", null);
            XmlElement paragraph = AddElement(paragraphs, "Paragraph", null);
            XmlElement textRuns = AddElement(paragraph, "TextRuns", null);
            XmlElement textRun = AddElement(textRuns, "TextRun", null);
            XmlElement value = AddElement(textRun, "Value", model.ReportType);
            XmlElement style = AddElement(textRun, "Style", null);

            AddElement(style, "FontFamily", "Segoe UI");
            AddElement(style, "FontSize", "9pt");
            AddElement(style, "FontWeight", "Bold");
            AddElement(style, "Color", "#29343b");

            AddElement(textBox, "Height", "0.35167in");
            AddElement(textBox, "Width", "5in");
            style = AddElement(textBox, "Style", null);

            AddElement(style, "PaddingLeft", "5pt");
            AddElement(style, "PaddingRight", "5pt");
            AddElement(style, "PaddingTop", "1pt");
            AddElement(style, "PaddingBottom", "1pt");
        }

        #endregion

        #region Functions for Normal Reports

        protected virtual XmlElement AddReportSectionElement(XmlElement report, XmlDocument doc)
        {
            XmlElement reportSections = AddElement(report, "ReportSections", null);
            XmlElement reportSection = AddElement(reportSections, "ReportSection", null);
            AddElement(reportSection, "Width", "5in");
            XmlElement reportPage = AddElement(reportSection, "Page", null);
            CreateReportHeaderFooter(reportPage, doc);
            XmlElement body = AddElement(reportSection, "Body", null);
            AddElement(body, "Height", "1.5in");
            XmlElement reportItems = AddElement(body, "ReportItems", null);

            return reportItems;
        }

        protected virtual void CreateReportHeaderFooter(XmlElement reportPage, XmlDocument doc)
        {
            CreateReportPageFooter(reportPage, doc);
        }

        protected virtual void CreateReportPageFooter(XmlElement reportPage, XmlDocument doc)
        {
            XmlElement reportPageFooter = AddElement(reportPage, "PageFooter", null);
            AddElement(reportPageFooter, "Height", "0.4in");
            AddElement(reportPageFooter, "PrintOnFirstPage", "true");
            AddElement(reportPageFooter, "PrintOnLastPage", "true");

            XmlElement reportItems = AddElement(reportPageFooter, "ReportItems", null);
            XmlElement reportFooterTextbox = AddElement(reportItems, "Textbox", null);
            XmlAttribute attr = reportFooterTextbox.Attributes.Append(doc.CreateAttribute("Name"));
            attr.Value = ReportFooterPage;
            AddElement(reportFooterTextbox, "CanGrow", "true");
            AddElement(reportFooterTextbox, "KeepTogether", "true");
            XmlElement reportFooterParagraphs = AddElement(reportFooterTextbox, "Paragraphs", null);
            XmlElement reportFooterParagraph = AddElement(reportFooterParagraphs, "Paragraph", null);
            XmlElement reportFooterTextRuns = AddElement(reportFooterParagraph, "TextRuns", null);
            XmlElement reportFooterTextRun = AddElement(reportFooterTextRuns, "TextRun", null);
            AddElement(reportFooterTextRun, "Value", "Page");
            XmlElement reportFooterTextRunStyle = AddElement(reportFooterTextRun, "Style", null);
            AddElement(reportFooterTextRunStyle, "FontSize", "8pt");
            AddElement(reportFooterTextRunStyle, "FontWeight", "Normal");
            AddElement(reportFooterTextRunStyle, "Color", "#ffffff");
        }

        protected virtual void CreateReportPageHeader(XmlElement reportPage, XmlDocument doc)
        {
            XmlElement reportPageHeader = AddElement(reportPage, "PageHeader", null);
            AddElement(reportPageHeader, "Height", "0.4in");
            AddElement(reportPageHeader, "PrintOnFirstPage", "true");
            AddElement(reportPageHeader, "PrintOnLastPage", "true");
            XmlElement reportItems = AddElement(reportPageHeader, "ReportItems", null);
            XmlElement reportImage = AddElement(reportItems, "Image", null);
            XmlAttribute attr = reportImage.Attributes.Append(doc.CreateAttribute("Name"));
            attr.Value = ReportHeaderImage;
            AddElement(reportImage, "Source", "External");
            AddElement(reportImage, "Sizing", "FitProportional");
            AddElement(reportImage, "Top", "0.12861in");
            AddElement(reportImage, "Left", "0.19271in");
            AddElement(reportImage, "Height", "0.17361in");
            AddElement(reportImage, "Width", "1.31706in");
            AddElement(reportImage, "ZIndex", "1");

            XmlElement reportStyle = AddElement(reportImage, "Style", null);
            XmlElement reportBorder = AddElement(reportStyle, "Border", null);
            AddElement(reportBorder, "Style", "None");

            XmlElement reportItemStyle = AddElement(reportPageHeader, "Style", null);
            reportBorder = AddElement(reportItemStyle, "Border", null);
            AddElement(reportBorder, "Style", "None");

            AddElement(reportItemStyle, "BackgroundColor", "#29343b");
        }

        protected virtual XmlElement AddTablixElements(XmlElement reportItems, XmlDocument doc, out XmlElement tablix)
        {
            tablix = AddElement(reportItems, "Tablix", null);
            XmlAttribute attr = tablix.Attributes.Append(doc.CreateAttribute("Name"));
            attr.Value = "Tablix1";
            AddElement(tablix, "DataSetName", DataSetName);
            AddElement(tablix, "Top", "0.5in");
            AddElement(tablix, "Left", "0.5in");
            AddElement(tablix, "Height", "0.5in");
            AddElement(tablix, "Width", "3in");
            XmlElement tablixBody = AddElement(tablix, "TablixBody", null);

            return tablixBody;
        }

        protected virtual void CreateTablixColumns(XmlElement tablixBody, ReportColumnsListModel columns)
        {
            XmlElement tablixColumns = AddElement(tablixBody, "TablixColumns", null);
            XmlElement tablixColumn = null;
            
            foreach (ReportColumnModel column in columns.ColumnList)
            {
                tablixColumn = AddElement(tablixColumns, "TablixColumn", null);
                AddElement(tablixColumn, "Width", "1.5in");
            }
        }

        protected virtual void CreateTablixElements(XmlElement tablixRows, XmlDocument doc, ReportColumnsListModel columns)
        {
            XmlElement tablixCell = null;
            XmlElement cellContents = null;
            XmlElement textbox = null;
            XmlAttribute attr = null;

            XmlElement tablixRow = AddElement(tablixRows, "TablixRow", null);
            AddElement(tablixRow, "Height", "0.5in");
            XmlElement tablixCells = AddElement(tablixRow, "TablixCells", null);

            foreach (ReportColumnModel column in columns.ColumnList)
            {
                tablixCell = AddElement(tablixCells, "TablixCell", null);
                cellContents = AddElement(tablixCell, "CellContents", null);
                textbox = AddElement(cellContents, "Textbox", null);
                attr = textbox.Attributes.Append(doc.CreateAttribute("Name"));
                attr.Value = string.Concat("Header", column.ColumnName.ToString().Trim());
                AddElement(textbox, "KeepTogether", "true");
                XmlElement paragraphs = AddElement(textbox, "Paragraphs", null);
                XmlElement paragraph = AddElement(paragraphs, "Paragraph", null);
                XmlElement textRuns = AddElement(paragraph, "TextRuns", null);
                XmlElement textRun = AddElement(textRuns, "TextRun", null);
                AddElement(textRun, "Value", column.ColumnName.ToString().Trim());
                XmlElement style = AddElement(textRun, "Style", null);
                AddElement(style, "Color", "#ffffff");                
                AddElement(style, "FontFamily", "Segoe UI");
                AddElement(style, "FontSize", "9pt");
                AddElement(style, "FontWeight", "Bold");

                XmlElement paragraphStyle = AddElement(textbox, "Style", null);
                AddElement(paragraphStyle, "BackgroundColor", "#29343b");
                AddElement(paragraphStyle, "VerticalAlign", "Middle");


                XmlElement styleBorder = AddElement(paragraphStyle, "Border", null);
                AddElement(styleBorder, "Color", "#dddddd");
                AddElement(styleBorder, "Style", "Solid");
                AddElement(styleBorder, "Width", "0.25pt");

                AddElement(paragraphStyle, "PaddingLeft", "5pt");
                AddElement(paragraphStyle, "PaddingRight", "5pt");
                AddElement(paragraphStyle, "PaddingTop", "1pt");
                AddElement(paragraphStyle, "PaddingBottom", "1pt");
            }
        }

        protected virtual void AddTablixRowElements(XmlElement tablixRows, XmlDocument doc, ReportColumnsListModel columns)
        {
            XmlElement tablixRow = AddElement(tablixRows, "TablixRow", null);
            AddElement(tablixRow, "Height", "0.5in");
            XmlElement tablixCells = AddElement(tablixRow, "TablixCells", null);

            CreateRowElements(tablixCells, doc, columns);
        }

        protected virtual void CreateRowElements(XmlElement tablixCells, XmlDocument doc, ReportColumnsListModel columns)
        {
            XmlElement tablixCell = null;
            XmlElement cellContents = null;
            XmlElement textbox = null;
            XmlAttribute attr = null;

            XmlElement paragraphs = null;
            XmlElement paragraph = null;
            XmlElement textRuns = null;
            XmlElement textRun = null;
            XmlElement style = null;

            foreach (ReportColumnModel column in columns.ColumnList)
            {
                tablixCell = AddElement(tablixCells, "TablixCell", null);
                cellContents = AddElement(tablixCell, "CellContents", null);
                textbox = AddElement(cellContents, "Textbox", null);
                attr = textbox.Attributes.Append(doc.CreateAttribute("Name"));
                attr.Value = column.ColumnName.ToString().Trim();

                AddElement(textbox, "CanGrow", "true");
                AddElement(textbox, "KeepTogether", "true");
                paragraphs = AddElement(textbox, "Paragraphs", null);
                paragraph = AddElement(paragraphs, "Paragraph", null);
                textRuns = AddElement(paragraph, "TextRuns", null);
                textRun = AddElement(textRuns, "TextRun", null);
                AddElement(textRun, "Value", string.Concat("=Fields!", column.ColumnName.ToString().Trim(), ".Value"));
                style = AddElement(textRun, "Style", null);

                AddElement(style, "Color", "#333333");
                AddElement(style, "FontFamily", "Segoe UI");
                AddElement(style, "FontSize", "9pt");

                XmlElement textRunsStyle = AddElement(paragraph, "Style", null);
                AddElement(textRunsStyle, "TextAlign", "Left");

                XmlElement paragraphStyle = AddElement(textbox, "Style", null);
                AddElement(paragraphStyle, "VerticalAlign", "Middle");

                XmlElement styleBorder = AddElement(paragraphStyle, "Border", null);
                AddElement(styleBorder, "Color", "#dddddd");
                AddElement(styleBorder, "Style", "Solid");
                AddElement(styleBorder, "Width", "0.25pt");

                AddElement(paragraphStyle, "PaddingLeft", "1pt");
                AddElement(paragraphStyle, "PaddingRight", "1pt");
                AddElement(paragraphStyle, "PaddingTop", "1pt");
                AddElement(paragraphStyle, "PaddingBottom", "1pt");
            }
        }

        protected virtual void CreateTablixHierarchy(XmlElement tablix, XmlDocument doc, ReportColumnsListModel columns)
        {
            //TablixColumHierarchy element
            XmlElement tablixColumnHierarchy = AddElement(tablix, "TablixColumnHierarchy", null);
            XmlElement tablixMembers = AddElement(tablixColumnHierarchy, "TablixMembers", null);
            
            foreach (ReportColumnModel column in columns.ColumnList)
                AddElement(tablixMembers, "TablixMember", null);

            CreateTablixRowHierarchy(tablix, doc, tablixMembers);
        }

        protected virtual void CreateTablixRowHierarchy(XmlElement tablix, XmlDocument doc, XmlElement tablixMembers)
        {
            //TablixRowHierarchy elements
            XmlElement tablixRowHierarchy = AddElement(tablix, "TablixRowHierarchy", null);
            tablixMembers = AddElement(tablixRowHierarchy, "TablixMembers", null);
            XmlElement tablixMember = AddElement(tablixMembers, "TablixMember", null);
            AddElement(tablixMember, "KeepWithGroup", "After");
            AddElement(tablixMember, "KeepTogether", "true");
            AddElement(tablixMember, "RepeatOnNewPage", "true");
            AddElement(tablixMember, "FixedData", "true");
            tablixMember = AddElement(tablixMembers, "TablixMember", null);
            AddElement(tablixMember, "DataElementName", "Detail_Collection");
            AddElement(tablixMember, "DataElementOutput", "Output");
            AddElement(tablixMember, "KeepTogether", "true");
            XmlElement group = AddElement(tablixMember, "Group", null);
            XmlAttribute attr = group.Attributes.Append(doc.CreateAttribute("Name"));
            attr.Value = ReportGroupName;
            AddElement(group, "DataElementName", "Detail");
            XmlElement tablixMembersNested = AddElement(tablixMember, "TablixMembers", null);
            AddElement(tablixMembersNested, "TablixMember", null);
        }

        protected virtual void CreateReportParamSection(XmlElement report, XmlDocument doc, Dictionary<string, string> parameters)
        {
            if (IsNotNull(parameters) && parameters.Count > 0)
            {
                XmlElement ReportParameters = AddElement(report, "ReportParameters", null);
                XmlElement ReportParameter = null;
                XmlAttribute attr = null;
                foreach (var item in parameters)
                {
                    ReportParameter = AddElement(ReportParameters, "ReportParameter", null);
                    attr = ReportParameter.Attributes.Append(doc.CreateAttribute("Name"));
                    attr.Value = item.Key.ToString().Trim();
                    XmlElement defaultValue = AddElement(ReportParameter, "DefaultValue", null);
                    XmlElement valueList = AddElement(defaultValue, "Values", null);
                    AddElement(valueList, "Value", item.Value.ToString().Trim());
                    AddElement(ReportParameter, "DataType", "String");
                    AddElement(ReportParameter, "Prompt", item.Key.ToString().Trim());
                    AddElement(ReportParameter, "MultiValue", "false");
                    AddElement(ReportParameter, "AllowBlank", "true");
                    AddElement(ReportParameter, "Hidden", "true");
                }
            }
        }

        #endregion

        #endregion
    }
}
