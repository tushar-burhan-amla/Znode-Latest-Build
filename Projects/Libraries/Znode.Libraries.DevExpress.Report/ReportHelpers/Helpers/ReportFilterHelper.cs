using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using Parameters = DevExpress.XtraReports.Parameters;

namespace Znode.Libraries.DevExpress.Report
{

    public class ReportFilterHelper : IReportFilterHelper
    {
        public const string VisibleColumnsParamName = "VisibleColumns";
        public const string VisibleColumnsParamNameDescription = "Visible Columns";
        public const string StoreNameParam = "StoresName";
        public const string WareHousesName = "WareHousesName";
        public const string OrderStatusName = "OrderStatusName";
        public const string DiscountTypeName = "DiscountTypes";
        public const string TypePara = "Type";
        public const string StoreParamDesc = "Stores Name";
        public const string Dropdown = "dropdown";
        private const string HeaderTableName = "xrTable1";
        private const string ValueConst = "Value";
        private const string DisplayName = "DisplayMember";
        private const string StoreWithCurrency = "StoreWithCurrency";

        public Func<ReportSettingModel> ReportSettignDelegate { get; set; }
        public ReportSettingModel ReportSetting { get; set; }

        #region Constructor
        public ReportFilterHelper()
        {
        }
        #endregion Constructor

        #region public method
        public ReportSettingModel GetReportSetting(Func<ReportSettingModel> reportSetting)
        {
            ReportSetting = reportSetting.Invoke();
            ReportSetting.Filters = XmlToFilterModel();
            ReportSetting.MustHideColumns = GetHideColumns(ReportConstants.MusthideKey, ReportConstants.Yes);
            ReportSetting.MustShowColumns = GetHideColumns(ReportConstants.MustshowKey, ReportConstants.Yes);
            return ReportSetting;
        }

        //This method is used to add parameters dynamically.
        public void AddParameters(ReportModel report, ReportSettingModel reportSettingModel)
        {
            bool IsStoreWithCurrency = false;
            if (!Equals(reportSettingModel.Filters, null))
            {
                reportSettingModel.Filters.ForEach(x =>
                {
                    Parameters.Parameter param = new Parameters.Parameter
                    {
                        Name = x.ColumnName,
                        Description = x.HeaderText,
                        Type = Type.GetType(x.DataType),
                        Visible = true
                    };

                    if(!string.IsNullOrEmpty(x.Value))
                    {
                        if (param.Type.Name.Equals("Boolean"))
                            param.Value = Convert.ToBoolean(x.Value);
                        else
                            param.Value = x.Value;
                        if(x.Value.Equals(StoreWithCurrency))
                        {
                            IsStoreWithCurrency = true;
                        }
                    }   
                    if (report.ReportObject.Parameters[x.ColumnName] == null)
                    {
                        report.ReportObject.Parameters.Add(param);
                    }
                });
            }

            SetParameterDefaultValues(report.ReportObject.Parameters);

            AddExtraParameter(report.ReportObject, ReportSetting.MustHideColumns, ReportSetting.MustShowColumns, VisibleColumnsParamName);
            AddExtraParameter(report.ReportObject, ReportSetting.PortalList, ReportSetting.PortalList, StoreNameParam, IsStoreWithCurrency);
            AddExtraParameter(report.ReportObject, ReportSetting.WareHouseList, ReportSetting.WareHouseList, WareHousesName);
            AddExtraParameter(report.ReportObject, ReportSetting.OrderStatusList, GetDefaultOrderStatus(ReportSetting.OrderStatusList), OrderStatusName);
            AddExtraParameter(report.ReportObject, ReportSetting.DiscountTypeList, ReportSetting.DiscountTypeList, DiscountTypeName);
            if (report.ReportObject.Parameters[TypePara] !=null)
            {
                List<DevExpressReportParameterModel> SelectList = new List<DevExpressReportParameterModel>();
                report.ReportObject.Parameters[TypePara].ValueInfo.ToString().Split('|').ToList().ForEach(Item => SelectList.Add(new DevExpressReportParameterModel() { Value = Item }));
                AddExtraParameter(report.ReportObject, SelectList, SelectList, TypePara);
            }

        }

        private List<DevExpressReportParameterModel> GetDefaultOrderStatus(List<DevExpressReportParameterModel> OrderStatusList)
        {
            return OrderStatusList?.Where(x=>x.Value.ToUpper() != "CANCELLED" && x.Value.ToUpper() != "FAILED").ToList();
        }

        private void AddExtraParameter(XtraReport report, List<DevExpressReportParameterModel> ReportParameterDatasource, List<DevExpressReportParameterModel> ReportParameterValues, string ParameterName, bool IsStoreWithCurrency = false)
        {
            var parameter = report.Parameters[ParameterName];
            if (parameter != null)
            {
                parameter.LookUpSettings = new Parameters.DynamicListLookUpSettings
                {
                    ValueMember = IsStoreWithCurrency? DisplayName : ValueConst,
                    DisplayMember = IsStoreWithCurrency ? DisplayName : ValueConst,
                    DataSource = ReportParameterDatasource,
                };
                parameter.MultiValue = true;
                parameter.Value = IsStoreWithCurrency ? GetFirstCurrentStores(ReportParameterValues.Select(x => x.DisplayMember)) : ReportParameterValues.Select(x => x.Value);
            }
        }

        public IList<string> GetFirstCurrentStores(IEnumerable<string> stores)
        {
            var firstCurrencySymbol = string.Empty;
            List<string> sameCurrencyStore = new List<string>();
            if (stores.Count() > 0)
            {
                firstCurrencySymbol = GetSubstringByString("(", ")", stores.FirstOrDefault());
                foreach (var store in stores)
                {
                    if (!string.IsNullOrEmpty(store) && store.Contains(firstCurrencySymbol))
                        sameCurrencyStore.Add(store);
                }
            }
            return sameCurrencyStore;
        }

        public string GetSubstringByString(string a, string b, string c)
        {
            return c.Substring((c.IndexOf(a) + a.Length), (c.IndexOf(b) - c.IndexOf(a) - a.Length));
        }

        public void SetParameterDefaultValues(ParameterCollection parameters)
        {
            foreach (var parameter in parameters)
            {
                if (parameter.Name.Equals(FilterKeys.BeginDate))
                    parameter.Value = DateTime.Now.GetFirstDayofMonth();
                else if (parameter.Name.Equals(FilterKeys.EndDate))
                    parameter.Value = DateTime.Now.GetLastDayofMonth();
            }            
        }

        #endregion public method

        //Read data from xml and assign into filerModel 
        protected List<FilterModel> XmlToFilterModel()
        {
            if (!string.IsNullOrEmpty(ReportSetting.ReportSettingXML))
            {
                //parse the xml string into document
                XDocument xmlDoc = XDocument.Parse(ReportSetting.ReportSettingXML);
                List<FilterModel> model = (from xml in xmlDoc.Descendants(ReportConstants.columnKey)
                                           where (xml.Element(ReportConstants.isallowsearchKey).Value.Equals(Convert.ToString(ReportConstants.Yes)))
                                           select new FilterModel
                                           {
                                               ColumnName = xml.Element(ReportConstants.nameKey).Value,
                                               HeaderText = xml.Element(ReportConstants.headertextKey).Value,
                                               IsAllowSearch = xml.Element(ReportConstants.isallowsearchKey).Value,
                                               Id = Convert.ToInt32(xml.Element(ReportConstants.IdKey).Value),
                                               DataType = xml.Element(ReportConstants.datatypeKey).Value,
                                               Value = xml.Element(ReportConstants.Value).Value,
                                               ParameterType = xml.Element(ReportConstants.ParameterType).Value
                                           }).ToList();
                return model;
            }
            return null;
        }

        protected List<DevExpressReportParameterModel> GetHideColumns(string key, string value)
        {
            if (!string.IsNullOrEmpty(ReportSetting.ReportSettingXML))
            {
                //parse the xml string into document
                XDocument xmlDoc = XDocument.Parse(ReportSetting.ReportSettingXML);
                List<DevExpressReportParameterModel> model = (from xml in xmlDoc.Descendants(ReportConstants.columnKey)
                                                              where (xml.Element(key).Value.Equals(Convert.ToString(value)))
                                                              select new DevExpressReportParameterModel
                                                              {
                                                                  Value = xml.Element(ReportConstants.headertextKey).Value
                                                              }).ToList();
                return model;
            }
            return null;
        }

        //returns the value of the element passed
        private static string GetXmlElementValue(string elementKey, dynamic xml)
        {
            try
            {
                return xml.Element(elementKey).Value;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

    }
}
