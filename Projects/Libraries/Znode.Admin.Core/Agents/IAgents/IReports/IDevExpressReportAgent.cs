using System.Collections.Generic;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.DevExpress.Report;

namespace Znode.Engine.Admin.Agents
{
    public interface IDevExpressReportAgent
    {
        /// <summary>
        /// Get the report setting by report code.
        /// </summary>
        /// <param name="reportCode"></param>
        /// <returns>Report setting model.</returns>
        ReportSettingModel GetReportSetting(string reportCode);

        /// <summary>
        /// Get Discount type list
        /// </summary>
        /// <returns></returns>
        List<Api.Models.DevExpressReportParameterModel> GetDiscountTypeList();

        /// <summary>
        /// Get report data source by report code.
        /// </summary>
        /// <param name="reportCode"></param>
        /// <param name="reportFilterModel"></param>
        /// <returns></returns>
        dynamic GetReportDataSource(string reportCode, dynamic reportFilterModel);

        /// <summary>
        /// Get all Portal(Store) names for import.
        /// </summary>
        /// <returns></returns>
        List<Api.Models.DevExpressReportParameterModel> GetPortalList();

        /// <summary>
        /// Get List of warehouse.
        /// </summary>
        /// <returns></returns>
        List<Api.Models.DevExpressReportParameterModel> GetWarehouseList();

        /// <summary>
        /// Get List of order status.
        /// </summary>
        /// <returns></returns>
        List<Api.Models.DevExpressReportParameterModel> GetOrderStatusList();

        /// <summary>
        /// Get all report categories by report categoryId.
        /// </summary>
        /// <param name="reportCategoryId"></param>
        /// <returns></returns>
        ReportCategoryListViewModel GetReportCategories(int? reportCategoryId);

        /// <summary>
        /// This method is used to save report layout into database.
        /// </summary>
        /// <param name="reportModel"></param>
        Api.Models.ReportViewModel SaveReportIntoDatabase(Api.Models.ReportViewModel reportModel);

        /// <summary>
        /// This method is used to get saved layout from the database according to user id and report code.
        /// </summary>
        /// <param name="reportModel"></param>
        /// <returns></returns>
        List<Api.Models.ReportViewModel> LoadSavedReportLayout(Api.Models.ReportViewModel reportModel);

        /// <summary>
        /// Get report by report code and name.
        /// </summary>
        /// <param name="reportCode"></param>
        /// <returns></returns>
        ReportModel GenerateReport(string reportCode, string reportName);

        /// <summary>
        /// This method is used to delete the save report layout view from the database.
        /// </summary>
        /// <param name="reportViewId"></param>
        bool DeleteSavedReportLayout(int reportViewId);
    }
}
