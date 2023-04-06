using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Znode.Engine.Api.Models;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public class TouchPointConfigurationService : BaseService, ITouchPointConfigurationService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeERPConfigurator> _eRPConfiguratorRepository;
        private readonly IZnodeRepository<ZnodeERPTaskScheduler> _erpTaskSchedulerRepository;
        private readonly IEmailTemplateSharedService _emailTemplateSharedService;
        #endregion

        #region Constructor
        public TouchPointConfigurationService()
        {
            _eRPConfiguratorRepository = new ZnodeRepository<ZnodeERPConfigurator>();
            _erpTaskSchedulerRepository = new ZnodeRepository<ZnodeERPTaskScheduler>();
            _emailTemplateSharedService = GetService<IEmailTemplateSharedService>();
        }
        #endregion

        #region Public Methods
        // Method for Get TouchPoint Configuration List
        public virtual TouchPointConfigurationListModel GetTouchPointConfigurationList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);

            ReplaceSortKeys(ref sorts);
            bool isAssigned = Convert.ToBoolean(filters.Where(x => x.FilterName.Equals(FilterKeys.IsAssigned.ToLower()))?.Select(y => y.FilterValue.ToLower())?.FirstOrDefault());
            filters.RemoveAll(x => x.FilterName == FilterKeys.IsAssigned.ToLower());
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameters", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            //maps the entity list to model
            TouchPointConfigurationListModel listModel = new TouchPointConfigurationListModel();

            //Get Active ERPConfiguratorId 
            int erpConfiguratorId = _eRPConfiguratorRepository.Table.Where(x => x.IsActive).Select(g => g.ERPConfiguratorId).FirstOrDefault();
            ZnodeLogging.LogMessage("Active ERPConfiguratorId", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, erpConfiguratorId);

            if (!isAssigned)
                GetAssignTouchPointList(listModel, erpConfiguratorId);
            else
                GetUnassignTouchPointList(listModel, erpConfiguratorId);

            string whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseForSP(filters.ToFilterDataCollection());
            IZnodeViewRepository<TouchPointConfigurationModel> objStoredProc = new ZnodeViewRepository<TouchPointConfigurationModel>();
            var xmlData = HelperUtility.ToXML(listModel.TouchPointConfigurationList);
            //SP parameters
            objStoredProc.SetParameter("@TouchPointConfigurationXML", xmlData, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@WhereClause", whereClauseModel, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@IsAssigned", isAssigned, ParameterDirection.Input, DbType.Boolean);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            var list = objStoredProc.ExecuteStoredProcedureList("Znode_GetTouchPointConfigurationList @TouchPointConfigurationXML,@WhereClause,@Rows,@PageNo,@Order_By,@IsAssigned,@RowCount OUT", 6, out pageListModel.TotalRowCount);
            foreach (var item in list)
            {
                item.Status = item.IsEnabled;
                item.IsEnabled = !item.IsEnabled;
            }
            listModel = new TouchPointConfigurationListModel { TouchPointConfigurationList = list.ToList() };
            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Method Get Trigger Task Scheduler
        public virtual bool TriggerTaskScheduler(string connectorTouchPoints)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);

            //Get current active class name
            IERPConfiguratorService eRPConfiguratorService = GetService<IERPConfiguratorService>();
            Assembly assembly = Assembly.Load("Znode.Engine.ERPConnector");
            Type className = assembly.GetTypes().FirstOrDefault(g => g.Name == eRPConfiguratorService.GetActiveERPClassName());

            //Create Instance of active class
            object instance = Activator.CreateInstance(className);

            //Get Method Information from class
            MethodInfo info = className.GetMethod(connectorTouchPoints);

            //Calling method with null parameter
            string invokeStatus = Convert.ToString(info.Invoke(instance, null));

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            return !string.IsNullOrEmpty(invokeStatus) && invokeStatus == "True";
        }

        //Method Get Scheduler Log List
        public virtual TouchPointConfigurationListModel SchedulerLogList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            //maps the entity list to model
            TouchPointConfigurationListModel listModel;
            
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            string whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseForSP(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("pageListModel and whereClauseModel to set SP parameters", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString(), whereClauseModel });
            IZnodeViewRepository<TouchPointConfigurationModel> objStoredProc = new ZnodeViewRepository<TouchPointConfigurationModel>();
            //SP parameters
            objStoredProc.SetParameter("@WhereClause", whereClauseModel, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowsCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            var list = objStoredProc.ExecuteStoredProcedureList("Znode_GetImportERPConnectorLogs @WhereClause,@Rows,@PageNo,@Order_By,@RowsCount OUT", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("SchedulerLogList count", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, new object[] { list?.Count });
            listModel = new TouchPointConfigurationListModel { SchedulerLogList = list.ToList() };
            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Method Send Scheduler ActivityLog
        public virtual bool SendSchedulerActivityLog(ERPSchedulerLogActivityModel erpSchedulerLogActivityModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);

            //Method to get Email Template Details by Code.
            EmailTemplateMapperModel emailTemplateMapperModel = GetEmailTemplateByCode(ZnodeConstant.ERPTaskSchedulerStatus, erpSchedulerLogActivityModel.PortalId);
            if (IsNotNull(emailTemplateMapperModel) && IsNotNull(erpSchedulerLogActivityModel))
            {
                string messageText;
                string statusMessage;

                ZnodeERPTaskScheduler eRPTaskScheduler = _erpTaskSchedulerRepository.Table.FirstOrDefault(x => x.ERPTaskSchedulerId == erpSchedulerLogActivityModel.ERPTaskSchedulerId);

                ZnodeLogging.LogMessage("erpConfiguratorId", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, new object[] { eRPTaskScheduler?.ERPConfiguratorId });

                string storeLogo = GetCustomPortalDetails(erpSchedulerLogActivityModel.PortalId)?.StoreLogo;
                ZnodeLogging.LogMessage("storeLogo returned from GetCustomPortalDetails", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, new object[] { storeLogo });

                //Set error statusMessage 
                if (erpSchedulerLogActivityModel.SchedulerStatus)
                    statusMessage = ERP_Resources.SuccessSchedulerStatus;
                else
                    statusMessage = ERP_Resources.FailedSchedulerStatus;

                //Generate Email Message Content Based on the Email Template.
                GenerateSchedulerActivityLogEmailText(storeLogo, eRPTaskScheduler, statusMessage, emailTemplateMapperModel?.Descriptions, erpSchedulerLogActivityModel?.ErrorMessage, out messageText);
                try
                {
                    //Method for Send Email 
                    string email = _eRPConfiguratorRepository.Table.FirstOrDefault(x => x.ERPConfiguratorId == eRPTaskScheduler.ERPConfiguratorId).Email;
                    if (IsNotNull(emailTemplateMapperModel))
                        ZnodeEmail.SendEmail(email, ZnodeConfigManager.SiteConfig.AdminEmail, ZnodeEmail.GetBccEmail(emailTemplateMapperModel.IsEnableBcc, erpSchedulerLogActivityModel.PortalId, string.Empty), emailTemplateMapperModel.Subject, messageText, true);
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogActivity((int)ZnodeLogging.ErrorNum.GeneralError, null, string.Empty, null, ex.Message, null);
                    return false;
                }
                return true;
            }
            return false;
        }
        #endregion

        #region Private
        //Method Get Scheduler Result Message
        private static string GetSchedulerResultMessage(string lastRunResult)
        {
            switch (lastRunResult)
            {
                case ZnodeConstant.Zero:
                case ZnodeConstant.ZeroByZero:
                    lastRunResult = ERP_Resources.SchedulerLastRunREsultCode0;
                    break;
                case ZnodeConstant.One:
                case ZnodeConstant.ZeroByOne:
                    lastRunResult = string.Empty;
                    break;
                case ZnodeConstant.Two:
                case ZnodeConstant.ZeroByTwo:
                    lastRunResult = ERP_Resources.SchedulerLastRunREsultCode2;
                    break;
                case ZnodeConstant.Ten:
                case ZnodeConstant.ZeroByA:
                    lastRunResult = ERP_Resources.SchedulerLastRunREsultCode10;
                    break;
                case ZnodeConstant.ZeroBy41300:
                    lastRunResult = ERP_Resources.SchedulerLastRunREsultCode41300;
                    break;
                case ZnodeConstant.ZeroBy41301:
                    lastRunResult = ERP_Resources.SchedulerLastRunREsultCode41301;
                    break;
                case ZnodeConstant.ZeroBy41302:
                    lastRunResult = ERP_Resources.SchedulerLastRunREsultCode41302;
                    break;
                case ZnodeConstant.ZeroBy41303:
                    lastRunResult = ERP_Resources.SchedulerLastRunREsultCode41303;
                    break;
                case ZnodeConstant.ZeroBy41304:
                    lastRunResult = ERP_Resources.SchedulerLastRunREsultCode41304;
                    break;
                case ZnodeConstant.ZeroBy41306:
                    lastRunResult = ERP_Resources.SchedulerLastRunREsultCode41306;
                    break;
                case ZnodeConstant.ZeroBy8004130F:
                    lastRunResult = ERP_Resources.SchedulerLastRunREsultCode8004130F;
                    break;
                case ZnodeConstant.ZeroBy8004131F:
                    lastRunResult = ERP_Resources.SchedulerLastRunREsultCode8004131F;
                    break;
                case ZnodeConstant.ZeroBy80070002:
                    lastRunResult = ERP_Resources.SchedulerLastRunREsultCode80070002;
                    break;
                case ZnodeConstant.ZeroBy8007010B:
                    lastRunResult = ERP_Resources.SchedulerLastRunREsultCode8007010B;
                    break;
                case ZnodeConstant.ZeroBy800704DD:
                    lastRunResult = ERP_Resources.SchedulerLastRunREsultCode800704DD;
                    break;
                case ZnodeConstant.ZeroByC000013A:
                    lastRunResult = ERP_Resources.SchedulerLastRunREsultCodeC000013A;
                    break;
                case ZnodeConstant.ZeroByC06D007E:
                    lastRunResult = ERP_Resources.SchedulerLastRunREsultCodeC06D007E;
                    break;
                default:
                    lastRunResult = string.Empty;
                    break;
            }
            return lastRunResult;
        }


        //Check whether the Content Page Name already exists.
        [Obsolete("This method is deprecated and will be discontinued in upcoming versions." +
       " Please use GenerateSchedulerActivityLogEmailText method for the generation of new mail text which contains more details.")]
        //Method Generate Scheduler Activity Log Email
        private static void GenerateSchedulerActivityLogEmail(string errorMessage, string storeLogo, string schedulerStatus, string templateContent, out string messageText)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);

            //Set Parameters for the Email Templates to be get replaced.
            Dictionary<string, string> setDictionary = new Dictionary<string, string>
            {
                {"#ErrorMessage#", errorMessage},
                {"#SchedulerStatus#", schedulerStatus},
                {"#StoreLogo#", storeLogo}
            };


            //Replace the Email Template Keys, based on the passed email template parameters.
            messageText = GetService<IEmailTemplateSharedService>().ReplaceTemplateTokens(templateContent, setDictionary);//EmailTemplateHelper.ReplaceTemplateTokens(templateContent);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);

        }
        //Method Generate Scheduler Activity Log Email
        public virtual void GenerateSchedulerActivityLogEmailText(string storeLogo, ZnodeERPTaskScheduler eRPTaskScheduler, string schedulerStatus, string templateContent, string errorMessage, out string messageText)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            //Set Parameters for the Email Templates to be get replaced.
            Dictionary<string, string> setDictionary = new Dictionary<string, string>
            {
                {"#SchedulerName#", eRPTaskScheduler?.SchedulerName},
                {"#TouchPointName#", eRPTaskScheduler?.TouchPointName},
                {"#SchedulerStatus#", schedulerStatus},
                {"#ErrorMessage#", errorMessage},
                {"#StoreLogo#", storeLogo}
            };
            //Replace the Email Template Keys, based on the passed email template parameters.
            messageText = _emailTemplateSharedService.ReplaceTemplateTokens(templateContent, setDictionary);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
        }

        //Method Get last Task Result
        private static void GetlastTaskResult(ZnodeERPTaskScheduler taskSchedulerList, out Task task, out string lastTaskResult)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);

            task = null;
            lastTaskResult = string.Empty;
            if (!string.IsNullOrEmpty(taskSchedulerList?.SchedulerName))
            {
                using (TaskService ts = new TaskService())
                {
                    task = ts.GetTask(taskSchedulerList?.SchedulerName);
                    lastTaskResult = GetSchedulerResultMessage(task?.LastTaskResult.ToString());
                    ZnodeLogging.LogMessage("lastTaskResult returned from GetSchedulerResultMessage", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, new object[] { lastTaskResult });

                }
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);

        }

        //Method Add touchPointConfiguration items
        private static TouchPointConfigurationModel AddtouchPointConfiguration(string touchPointName, ZnodeERPTaskScheduler taskSchedulerList)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);

            Task task;
            string lastTaskResult;
            GetlastTaskResult(taskSchedulerList, out task, out lastTaskResult);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);

            return new TouchPointConfigurationModel
            {
                ConnectorTouchPoints = touchPointName,
                Interface = touchPointName,
                SchedulerName = taskSchedulerList?.SchedulerName,
                SchedulerType = taskSchedulerList?.SchedulerType,
                ERPTaskSchedulerId = !string.Equals(taskSchedulerList, null) ? taskSchedulerList.ERPTaskSchedulerId : 0,
                Triggers = task?.Definition.Triggers.ToString(),
                LastRunResult = lastTaskResult,
                IsEnabled = Convert.ToBoolean(taskSchedulerList?.IsEnabled),
                NextRunTime = (Convert.ToBoolean(taskSchedulerList?.IsEnabled) && task?.NextRunTime != DateTime.MinValue) ? task?.NextRunTime.ToString() : string.Empty,
                SchedulerCreatedDate = Convert.ToString(taskSchedulerList?.CreatedDate),
                LastRunTime = Convert.ToString(task?.LastRunTime),
                SchedulerCallFor = taskSchedulerList?.SchedulerCallFor ?? string.Empty
            };
        }

        //Method for Replace SortKeys
        private void ReplaceSortKeys(ref NameValueCollection sorts)
        {
            foreach (string key in sorts.Keys)
                if (string.Equals(key, FilterKeys.Status, StringComparison.OrdinalIgnoreCase)) { ReplaceSortKeyName(ref sorts, FilterKeys.Status.ToLower(), FilterKeys.IsEnabled); }
        }
        
        //Method for Get Unassign TouchPoint List
        private static void GetUnassignTouchPointList(TouchPointConfigurationListModel listModel, int erpConfiguratorId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);

            //Get current active class name           
            IERPConfiguratorService eRPConfiguratorService = GetService<IERPConfiguratorService>();
            Assembly assembly = Assembly.Load("Znode.Engine.ERPConnector");
            Type className = assembly.GetTypes().FirstOrDefault(g => g.Name == eRPConfiguratorService.GetActiveERPClassName());
            // Get the methods from class.
            listModel.ZnodeArrayMethodInfo = className?.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            if (IsNotNull(listModel?.ZnodeArrayMethodInfo))
                foreach (var item in listModel?.ZnodeArrayMethodInfo)
                {
                    //Add touchPointConfiguration items
                    TouchPointConfigurationModel touchPointConfigurations = new TouchPointConfigurationModel();
                    touchPointConfigurations.ConnectorTouchPoints = item.Name;
                    touchPointConfigurations.ERPConfiguratorId = erpConfiguratorId;
                    listModel.TouchPointConfigurationList.Add(touchPointConfigurations);
                }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);

        }

        //Method for Get Assign TouchPoint List
        private void GetAssignTouchPointList(TouchPointConfigurationListModel listModel, int erpConfiguratorId)
        {
            ZnodeLogging.LogMessage("Input parameters erpConfiguratorId", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, erpConfiguratorId);

            var taskSchedulerList = _erpTaskSchedulerRepository.Table.Where(x => x.ERPConfiguratorId == erpConfiguratorId && x.SchedulerCallFor == ZnodeConstant.ERP).ToList();
            foreach (var item in taskSchedulerList)
            {
                // Add touchPointConfiguration items
                TouchPointConfigurationModel touchPointConfigurations = AddtouchPointConfiguration(item.TouchPointName, item);
                listModel.TouchPointConfigurationList.Add(touchPointConfigurations);
            }
        }

        #endregion
    }
}
