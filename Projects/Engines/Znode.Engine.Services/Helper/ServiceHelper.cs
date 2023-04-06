using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Znode.Engine.Api.Models;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using System.Reflection;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Framework.Business;
using System.Diagnostics;

using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
namespace Znode.Engine.Services
{
    public static class ServiceHelper
    {
        #region Constants
        public const string DateTimeRange = "DateTimeRange";
        public const string IsCatalogFilter = "IsCatalogFilter";
        #endregion

        public static void BindPageListModel(this BaseListModel baseListModel, PageListModel pageListModel)
        {
            if (IsNotNull(pageListModel))
            {
                baseListModel.TotalResults = pageListModel.TotalRowCount;
                baseListModel.PageIndex = pageListModel.PagingStart;
                baseListModel.PageSize = pageListModel.PagingLength;
            }
        }

        //Format price according to global setting.
        public static string FormatPriceWithCurrency(decimal? price, string CultureName)
        {
            string currencyValue;
            if (IsNotNull(CultureName))
            {
                CultureInfo info = new CultureInfo(CultureName);
                info.NumberFormat.CurrencyDecimalDigits = Convert.ToInt32(DefaultGlobalConfigSettingHelper.DefaultPriceRoundOff);
                currencyValue = $"{price.GetValueOrDefault().ToString("c", info.NumberFormat)}";
            }
            else
                currencyValue = Convert.ToString(price);

            return currencyValue;
        }
        /// <summary>
        /// This method will invoke IsCodeExists method based on service name
        /// </summary>
        /// <param name="parameterModel">Parameter Model</param>
        /// <param name="className">Service name</param>
        /// <returns>boolean value</returns>
        public static bool ExecuteFunctionByName(HelperParameterModel parameterModel, string className, string methodName, string namespaceName = "", string projectName = "")
        {
            namespaceName = string.IsNullOrEmpty(namespaceName) ? "Znode.Engine.Services" : namespaceName;
            projectName = string.IsNullOrEmpty(projectName) ? "Znode.Engine.Services" : projectName;
            //Get Class Reference
            Type connector = Type.GetType(string.Concat(namespaceName, ".", className, ",", projectName));

            //Get Method of that class
            MethodInfo methodInfo = connector.GetMethod(methodName);
            object result = null;
            if (methodInfo != null)
            {
                //Create instance of that class
                object classInstance = Activator.CreateInstance(connector, null);
                //set parameter for method
                object[] parametersArray = new object[] { parameterModel };
                //Invoke method of that class
                result = methodInfo.Invoke(classInstance, parametersArray);

            }
            return (bool)result;
        }
        /// <summary>
        /// This method will round off inventory quantity based on Global Setting
        /// </summary>
        /// <param name="Quantity">Quantity</param>
        /// <returns></returns>
        public static string ToInventoryRoundOff(decimal Quantity)
          => GetInventoryRoundOff(Convert.ToString(Quantity));
        /// <summary>
        /// Get Inventory Round off by quantity
        /// </summary>
        /// <param name="Quantity"></param>
        /// <returns></returns>
        public static string GetInventoryRoundOff(string Quantity)
        {
            string roundOff = DefaultGlobalConfigSettingHelper.DefaultInventoryRoundOff;
            decimal size = Convert.ToDecimal(Quantity);
            return Convert.ToString(Math.Round((size), Convert.ToInt32(roundOff), MidpointRounding.AwayFromZero));
        }

        /// <summary>
        /// Adds date time value in filter collection against created date column.
        /// </summary>
        /// <param name="filters">FilterCollection</param>
        /// <returns>returns filters</returns>
        public static FilterCollection AddDateTimeValueInFilter(FilterCollection filters)
        {
            if(filters?.Count > 0)
            {
                string _dateTimeRange = filters.FirstOrDefault(x => string.Equals(x.FilterName, DateTimeRange, StringComparison.InvariantCultureIgnoreCase))?.FilterValue;
                //Update filters if _dateTimeRange is not null.
                if (_dateTimeRange != null && _dateTimeRange != string.Empty)
                {
                    filters.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.CreatedDate, StringComparison.InvariantCultureIgnoreCase));
                    filters.Add(new FilterTuple(FilterKeys.CreatedDate, FilterOperators.Between, _dateTimeRange));
                    filters.RemoveAll(x => string.Equals(x.FilterName, DateTimeRange, StringComparison.InvariantCultureIgnoreCase));
                }
            }
            return filters;
        }

        /// <summary>
        /// Adds date time value in filter collection against created date column.
        /// </summary>
        /// <param name="filters">FilterCollection</param>
        /// <param name="filterName">Filter Name</param>
        /// <returns>returns filters</returns>
        public static FilterCollection AddDateTimeValueInFilterByName(FilterCollection filters, string filterName)
        {
            if (filters?.Count > 0)
            {
                string dateTimeRange = filters.FirstOrDefault(x => string.Equals(x.FilterName, DateTimeRange, StringComparison.InvariantCultureIgnoreCase))?.FilterValue;
                //Update filters if _dateTimeRange is not null.
                if (!string.IsNullOrEmpty( dateTimeRange))
                {
                    filters.RemoveAll(x => string.Equals(x.FilterName, filterName, StringComparison.InvariantCultureIgnoreCase));
                    filters.Add(new FilterTuple(filterName, FilterOperators.Between, dateTimeRange));
                    filters.RemoveAll(x => string.Equals(x.FilterName, DateTimeRange, StringComparison.InvariantCultureIgnoreCase));
                }
            }
            return filters;
        }

        //Get catalog filter values from FilterCollection.
        public static int GetCatalogFilterValues(FilterCollection filters, ref bool isCatalogFilter)
        {
            int pimCatalogId = 0;
            if (filters.Any(x => string.Equals(x.FilterName, FilterKeys.CatalogId, StringComparison.CurrentCultureIgnoreCase)))
            {
                Int32.TryParse(filters.FirstOrDefault(x => string.Equals(x.FilterName, FilterKeys.CatalogId, StringComparison.CurrentCultureIgnoreCase))?.FilterValue, out pimCatalogId);
                filters.RemoveAll(x => x.FilterName.Equals(FilterKeys.CatalogId, StringComparison.InvariantCultureIgnoreCase));
            }
            if (filters.Any(x => string.Equals(x.FilterName, IsCatalogFilter, StringComparison.CurrentCultureIgnoreCase)))
            {
                isCatalogFilter = Convert.ToBoolean(filters.FirstOrDefault(x => string.Equals(x.FilterName, IsCatalogFilter, StringComparison.InvariantCultureIgnoreCase))?.FilterValue);
                filters.RemoveAll(x => x.FilterName.Equals(IsCatalogFilter, StringComparison.InvariantCultureIgnoreCase));
            }
            return pimCatalogId;
        }

        //to create the scheduler for voucher reminder email.
        public static void CreateVoucherReminderEmailScheduler()
        {
            ZnodeLogging.LogMessage("Create voucher email scheduler execution started", ZnodeLogging.Components.API.ToString(), TraceLevel.Info);
            try
            {
                bool isVoucherEmailSchedulerEnabled = ZnodeApiSettings.EnableSchedulerForVoucherReminderEmail;
                if (!isVoucherEmailSchedulerEnabled)
                    return;

                ERPTaskSchedulerModel eRPTaskSchedulerModel = new ERPTaskSchedulerModel
                {
                    TouchPointName = ZnodeConstant.VoucherReminderEmailTouchpoint,
                    SchedulerName = ZnodeConstant.VoucherReminderEmailSchedulerName,
                    SchedulerFrequency = ZnodeApiSettings.SchedulerFrequencyForVoucherReminderEmail,
                    StartDate = Convert.ToDateTime(Convert.ToString(DateTime.Today.ToShortDateString()) + " " + ZnodeApiSettings.VoucherEmailSchedulerTriggerTime),
                    SchedulerCallFor = "VoucherEmailHelper",
                    SchedulerType = ZnodeConstant.Scheduled,
                    IsAssignTouchPoint = true,
                    IsEnabled = isVoucherEmailSchedulerEnabled,
                    DomainName = ZnodeAdminSettings.ZnodeApiRootUri,
                    CronExpression = ZnodeApiSettings.VoucherEmailSchedulerCronExpression
                };

                IERPTaskSchedulerService eRPTaskSchedulerService = new ERPTaskSchedulerService();
                IList<View_ReturnBoolean> createResult = new List<View_ReturnBoolean>();
                View_ReturnBoolean view_ReturnBoolean = new View_ReturnBoolean()
                {
                    Id = 1,
                    Status = true
                };
                createResult.Add(view_ReturnBoolean);

                eRPTaskSchedulerService.CreateScheduler(eRPTaskSchedulerModel, createResult);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(string.Format("Error in voucher reminder Email Schedular", ex.StackTrace), ZnodeLogging.Components.API.ToString(), TraceLevel.Error, ex);
            }
            ZnodeLogging.LogMessage("Create voucher email scheduler execution done", ZnodeLogging.Components.API.ToString(), TraceLevel.Info);
        }

        //to create the scheduler for clear user registration attempts.
        public static void CreateClearUserRegistrationAttemptScheduler()
        {
            ZnodeLogging.LogMessage("Create clear user registration attempt scheduler execution started", ZnodeLogging.Components.API.ToString(), TraceLevel.Info);
            try
            {
                bool isUserRegistrationAttemptSchedulerEnabled = ZnodeApiSettings.EnableSchedulerForUserRegistrationAttempt;
                if (!isUserRegistrationAttemptSchedulerEnabled)
                    return;

                ERPTaskSchedulerModel eRPTaskSchedulerModel = new ERPTaskSchedulerModel
                {
                    TouchPointName = ZnodeConstant.ClearUserRegistrationAttemptEmailTouchpoint,
                    SchedulerName = ZnodeConstant.ClearUserRegistrationAttemptSchedulerName,
                    SchedulerFrequency = ZnodeApiSettings.UserRegistrationAttemptSchedulerType,
                    StartDate = Convert.ToDateTime(Convert.ToString(DateTime.Today.ToShortDateString()) + " " + ZnodeApiSettings.UserRegistrationAttemptSchedulerTriggerTime),
                    SchedulerCallFor = ZnodeConstant.UserRegistrationAttemptHelper,
                    SchedulerType = ZnodeConstant.Scheduled,
                    IsAssignTouchPoint = true,
                    IsEnabled = isUserRegistrationAttemptSchedulerEnabled,
                    DomainName = ZnodeAdminSettings.ZnodeApiRootUri,
                    CronExpression = ZnodeApiSettings.UserRegistrationAttemptSchedulerCronExpression
                };

                IERPTaskSchedulerService eRPTaskSchedulerService = GetService<IERPTaskSchedulerService>();
                IList<View_ReturnBoolean> createResult = new List<View_ReturnBoolean>();
                createResult.Add( new View_ReturnBoolean()
                {
                    Id = 2,
                    Status = true
                });

                eRPTaskSchedulerService.CreateScheduler(eRPTaskSchedulerModel, createResult);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(string.Format("Error in clear user registration attempt Schedular", ex.StackTrace), ZnodeLogging.Components.API.ToString(), TraceLevel.Error, ex);
            }
            ZnodeLogging.LogMessage("Create clear user registration attempt scheduler execution done", ZnodeLogging.Components.API.ToString(), TraceLevel.Info);
        }

        //to create the scheduler for delete expired payment token.
        public static void CreateDeletePaymentAuthTokenScheduler()
        {
            ZnodeLogging.LogMessage("Create delete expired payment token scheduler execution started", ZnodeLogging.Components.API.ToString(), TraceLevel.Info);
            try
            {
                bool isPaymentTokenSchedulerEnabled = ZnodeApiSettings.EnableSchedulerForDeletePaymentAuthToken;
                if (!isPaymentTokenSchedulerEnabled)
                    return;

                ERPTaskSchedulerModel eRPTaskSchedulerModel = new ERPTaskSchedulerModel
                {
                    TouchPointName = ZnodeConstant.DeletePaymentAuthTokenTouchpoint,
                    SchedulerName = ZnodeConstant.DeletePaymentAuthTokenSchedulerName,
                    SchedulerFrequency = ZnodeApiSettings.DeletePaymentAuthTokenSchedulerType,
                    StartDate = Convert.ToDateTime(Convert.ToString(DateTime.Today.ToShortDateString()) + " " + ZnodeApiSettings.PaymentTokenSchedularTriggerTime),
                    SchedulerCallFor = ZnodeConstant.PaymentAuthTokenSchedulerCallFor,
                    SchedulerType = ZnodeConstant.Scheduled,
                    IsAssignTouchPoint = true,
                    IsEnabled = isPaymentTokenSchedulerEnabled,
                    DomainName = ZnodeAdminSettings.ZnodeApiRootUri,
                    CronExpression = ZnodeApiSettings.DeletePaymentAuthTokenSchedulerCronExpression
                };

                IERPTaskSchedulerService eRPTaskSchedulerService = GetService<IERPTaskSchedulerService>();
                IList<View_ReturnBoolean> createResult = new List<View_ReturnBoolean>();
                createResult.Add(new View_ReturnBoolean()
                {
                    Id = 3,
                    Status = true
                });

                eRPTaskSchedulerService.CreateScheduler(eRPTaskSchedulerModel, createResult);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(string.Format("Error in delete expired payment token schedular", ex.StackTrace), ZnodeLogging.Components.API.ToString(), TraceLevel.Error, ex);
            }
            ZnodeLogging.LogMessage("delete expired payment token schedular execution done", ZnodeLogging.Components.API.ToString(), TraceLevel.Info);
        }

        // Scheduler for stock notification.
        public static void StockNoticeNotification()
        {
            try
            {
                bool isStockNoticeNotificationEnabled = ZnodeApiSettings.EnableStockNotice;
                if (!isStockNoticeNotificationEnabled)
                    return;

                ERPTaskSchedulerModel eRPTaskSchedulerModel = new ERPTaskSchedulerModel
                {
                    TouchPointName = ZnodeConstant.StockNotification,
                    SchedulerName = ZnodeConstant.StockNotification,
                    SchedulerFrequency = ZnodeApiSettings.StockNoticeSchedulerType,
                    StartDate = Convert.ToDateTime(Convert.ToString(DateTime.Today.ToShortDateString()) + " " + ZnodeApiSettings.PaymentTokenSchedularTriggerTime),
                    SchedulerCallFor = ZnodeConstant.StockNotification,
                    SchedulerType = ZnodeConstant.Scheduled,
                    IsAssignTouchPoint = true,
                    IsEnabled = isStockNoticeNotificationEnabled,
                    DomainName = ZnodeAdminSettings.ZnodeApiRootUri,
                    CronExpression = ZnodeApiSettings.StockNoticeCronExpression
                };

                IERPTaskSchedulerService eRPTaskSchedulerService = GetService<IERPTaskSchedulerService>();
                IList<View_ReturnBoolean> createResult = new List<View_ReturnBoolean>();
                createResult.Add(new View_ReturnBoolean()
                {
                    Id = 4,
                    Status = true
                });

                eRPTaskSchedulerService.CreateScheduler(eRPTaskSchedulerModel, createResult);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage($"Error in stock notification scheduler {ex.Message}", ZnodeLogging.Components.API.ToString(), TraceLevel.Error);
            }
            ZnodeLogging.LogMessage("Created scheduler for stock notice", ZnodeLogging.Components.API.ToString(), TraceLevel.Info);
        }

        //to create the scheduler for delete outdated export files.
        public static void CreateDeleteExportScheduler()
        {
            ZnodeLogging.LogMessage("Create delete outdated export files scheduler execution started", ZnodeLogging.Components.API.ToString(), TraceLevel.Info);
            try
            {
                bool isExportSchedulerEnabled = ZnodeApiSettings.EnableSchedulerForDeleteExportFiles;
                if (!isExportSchedulerEnabled)
                    return;

                ERPTaskSchedulerModel eRPTaskSchedulerModel = new ERPTaskSchedulerModel
                {
                    TouchPointName = ZnodeConstant.DeleteExportTouchpoint,
                    SchedulerName = ZnodeConstant.DeleteExportSchedulerName,
                    SchedulerFrequency = ZnodeApiSettings.DeleteExportSchedulerType,
                    StartDate = Convert.ToDateTime(Convert.ToString(DateTime.Today.ToShortDateString()) + " " + ZnodeApiSettings.ExportSchedulerTriggerTime),
                    SchedulerCallFor = ZnodeConstant.ExportSchedulerCallFor,
                    SchedulerType = ZnodeConstant.Scheduled,
                    IsAssignTouchPoint = true,
                    IsEnabled = isExportSchedulerEnabled,
                    DomainName = ZnodeAdminSettings.ZnodeApiRootUri,
                    CronExpression = ZnodeApiSettings.DeleteExportSchedulerCronExpression
                };

                IERPTaskSchedulerService eRPTaskSchedulerService = GetService<IERPTaskSchedulerService>();
                IList<View_ReturnBoolean> createResult = new List<View_ReturnBoolean>();
                createResult.Add(new View_ReturnBoolean()
                {
                    Id = 5,
                    Status = true
                });

                eRPTaskSchedulerService.CreateScheduler(eRPTaskSchedulerModel, createResult);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(string.Format("Error in delete outdated export files schedular", ex.StackTrace), ZnodeLogging.Components.API.ToString(), TraceLevel.Error, ex);
            }
            ZnodeLogging.LogMessage("delete outdated export files schedular execution done", ZnodeLogging.Components.API.ToString(), TraceLevel.Info);
        }

    }
}
