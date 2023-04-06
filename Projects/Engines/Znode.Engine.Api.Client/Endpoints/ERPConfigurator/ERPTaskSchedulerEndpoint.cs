namespace Znode.Engine.Api.Client.Endpoints
{
    public class ERPTaskSchedulerEndpoint : BaseEndpoint
    {
        #region ERPTaskScheduler
        //Get ERPTaskScheduler List Endpoint
        public static string List() => $"{ApiRoot}/erptaskscheduler/list";

        //Create erptaskscheduler Endpoint.
        public static string Create() => $"{ApiRoot}/erptaskscheduler/create";

        //Get erpTaskScheduler on the basis of erpTaskScheduler id Endpoint.
        public static string GetERPTaskScheduler(int erpTaskSchedulerId) => $"{ApiRoot}/erpTaskScheduler/{erpTaskSchedulerId}";

        //Delete erpTaskScheduler Endpoint.
        public static string Delete() => $"{ApiRoot}/erptaskscheduler/delete";

        // Enable disable eRPConfigurator on basis of isActive status.
        public static string EnableDisableTaskScheduler(int connectorTouchPoints, bool isActive) => $"{ApiRoot}/erptaskscheduler/enabledisabletaskscheduler/{connectorTouchPoints}/{isActive}";
        #endregion

        #region TouchPointConfiguration
        //Get TouchPointConfiguration List Endpoint
        public static string TouchPointConfigurationList() => $"{ApiRoot}/touchpointconfiguration/list";

        //Trigger task scheduler for connectorTouchPoints
        public static string TriggerTaskScheduler(string connectorTouchPoints) => $"{ApiRoot}/touchpointconfiguration/triggertaskscheduler/{connectorTouchPoints}";

        //Get Scheduler Log List Endpoint
        public static string GetSchedulerLogList() => $"{ApiRoot}/touchpointconfiguration/getschedulerloglist";
        #endregion
    }
}
