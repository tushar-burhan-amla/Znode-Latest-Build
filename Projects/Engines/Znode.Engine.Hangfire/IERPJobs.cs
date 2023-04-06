using System.Collections.Generic;

using Znode.Engine.Api.Models;

namespace Znode.Engine.Hangfire
{
    public interface IERPJobs
    {
        /// <summary>
        /// Schedule jobs in Hangfire
        /// </summary>
        /// <param name="model">Details of the job to be scheduled</param>
        /// <param name="hangfireJobId">Hangfire Job Id</param>
        /// <returns></returns>
        bool ConfigureJobs(ERPTaskSchedulerModel model, out string hangfireJobId);

        /// <summary>
        /// Removes a Hangfire job.
        /// </summary>
        /// <param name="schedulerModel">Model with details to delete the job</param>
        /// <returns>Boolean result indicating the success/failure of the job removal.</returns>
        bool RemoveJob(ERPTaskSchedulerModel schedulerModel);

        /// <summary>
        /// Removes multiple Hangfire Recurring jobs.
        /// </summary>
        /// <param name="schedulerNamesList">List of job names to be deleted.</param>
        /// <returns>Boolean result indicating the success/failure of the job removal.</returns>
        bool RemoveJobs(List<string> schedulerNamesList);

        /// <summary>
        /// Invoke the scheduled method.
        /// </summary>
        /// <param name="SchedulerName">Scheduler name</param>
        /// <param name="param">Scheduler parameters</param>
        void Invoke(string SchedulerName, string param);
    }
}
