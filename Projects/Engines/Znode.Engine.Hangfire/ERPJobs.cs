using Hangfire;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;

using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Hangfire;

namespace Znode.Engine.Hangfire
{
    public class ERPJobs : IERPJobs
    {
        public virtual bool ConfigureJobs(ERPTaskSchedulerModel model, out string hangfireJobId)
        {
            var status = false;
            hangfireJobId = string.Empty;

            try
            {
                ZnodeLogging.LogMessage("Hangfire job creation starting for " + model.SchedulerName, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);

                //In case of update, delete the current job and create a new job in Hangfire after that.
                bool removeJobResult = RemoveJob(model);

                if (model.IsInstantJob)
                {
                    hangfireJobId = BackgroundJob.Enqueue(() => this.Invoke(model.SchedulerName, JsonConvert.SerializeObject(model)));
                }
                else if (model.SchedulerFrequency == ZnodeConstant.OneTime)
                {
                    hangfireJobId = BackgroundJob.Schedule(() => this.Invoke(model.SchedulerName, JsonConvert.SerializeObject(model)), new DateTimeOffset(model.StartDate.Value));
                }
                else if (model.SchedulerFrequency == ZnodeConstant.Recurring)
                {
                    RecurringJob.AddOrUpdate(model.SchedulerName, () => this.Invoke(model.SchedulerName, JsonConvert.SerializeObject(model)), model.CronExpression, TimeZoneInfo.Local);
                }
                ZnodeLogging.LogMessage("Hangfire job created for " + model.SchedulerName, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);

                status = true;
            }
            catch (Exception ex)
            {
                status = false;
                ZnodeLogging.LogMessage($"Error in Hangfire :{ex.Message}", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error, ex);
            }
            return status;
        }

        public virtual bool RemoveJob(ERPTaskSchedulerModel schedulerModel)
        {
            bool result = true;

            try
            {
                if (schedulerModel.ERPTaskSchedulerId <= 0)
                    return result;

                //Delete one-time job
                if(!string.IsNullOrEmpty(schedulerModel.HangfireJobId))
                    result = BackgroundJob.Delete(schedulerModel.HangfireJobId);

                //Delete recurring job
                result = RemoveJob(schedulerModel.SchedulerName);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error);
                result = false; 
            }

            return result;
        }

        public virtual bool RemoveJobs(List<string> schedulerNamesList)
        {
            bool status = false;
            try
            {
                foreach (string schedulerName in schedulerNamesList)
                {
                    status = RemoveJob(schedulerName);
                }
            }
            catch (Exception ex)
            {
                status = false;
                ZnodeLogging.LogMessage($"Error in removing Hangfire job:{ex.Message}", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error, ex);
            }
            return status;
        }

        [DisplayName("{0}")]
        public virtual void Invoke(string SchedulerName, string param)
        {
            ERPTaskSchedulerModel obj = JsonConvert.DeserializeObject<ERPTaskSchedulerModel>(param);
            string type = Convert.ToString(obj.SchedulerCallFor);
            ISchedulerProviders _provider = GetSchedulerProviderObject(type);

            ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;

            try
            {
                ZnodeLogging.LogMessage("param for execution :" + param, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                ZnodeLogging.LogMessage("Start invoking Hangfire job for " + SchedulerName, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                if (!Equals(_provider, null))
                    _provider.InvokeMethod(obj);
                ZnodeLogging.LogMessage("Hangfire job invoked for " + SchedulerName, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage($"Error in invoking Hangfire job:{ex.Message}", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error, ex);
            }
        }

        protected virtual ISchedulerProviders GetSchedulerProviderObject(string schedulerType)
        {
            if (!string.IsNullOrEmpty(schedulerType))
            {
                StructureMap.Container container = new StructureMap.Container(content =>
                content.Scan(scan =>
                {
                    scan.AssemblyContainingType<ISchedulerProviders>();
                    scan.AddAllTypesOf<ISchedulerProviders>();
                }));
                string schedulerName = container.Model.AllInstances.FirstOrDefault(x => x.Description.Contains(schedulerType)).Name;
                return !string.IsNullOrEmpty(schedulerType) ? container.GetInstance<ISchedulerProviders>(schedulerName) : null;
            }
            return null;
        }

        protected virtual bool RemoveJob(string schedulerName)
        {
            bool status;
            try
            {
                RecurringJob.RemoveIfExists(schedulerName);
                ZnodeLogging.LogMessage("Hangfire job Removed for " + schedulerName, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                status = true;
            }
            catch (Exception ex)
            {
                status = false;
                ZnodeLogging.LogMessage($"Error in removing Hangfire job:{ex.Message}", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error, ex);
            }
            return status;
        }
    }
}