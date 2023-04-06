namespace Znode.Engine.Api.Client.Endpoints
{
    public class ERPConfiguratorEndpoint : BaseEndpoint
    {
        //Get ERPConfigurator List Endpoint
        public static string List() => $"{ApiRoot}/erpconfigurator/list";

        //Get All ERP Configurator Classes List which are Not In Database Endpoint
        public static string GetAllERPConfiguratorClassesNotInDatabase() => $"{ApiRoot}/erpconfigurator/getallerpconfiguratorclassesnotindatabase";

        //Create erpconfigurator Endpoint.
        public static string Create() => $"{ApiRoot}/erpconfigurator/create";

        //Get eRPConfigurator on the basis of eRPConfigurator id Endpoint.
        public static string GetERPConfigurator(int eRPConfiguratorId) => $"{ApiRoot}/eRPConfigurator/{eRPConfiguratorId}";

        //Update eRPConfigurator Endpoint.
        public static string Update() => $"{ApiRoot}/erpconfigurator/update";

        //Delete eRPConfigurator Endpoint.
        public static string Delete() => $"{ApiRoot}/erpconfigurator/delete";

        // Enable disable eRPConfigurator on basis of isActive status.
        public static string EnableDisableERPConfigurator(string eRPConfiguratorId,bool isActive) => $"{ApiRoot}/erpconfigurator/enabledisableerpconfigurator/{eRPConfiguratorId}/{isActive}";

        //Get ERPConfigurator Active Class Name.
        public static string GetActiveERPClassName() => $"{ApiRoot}/getactiveerpclassname";

        //Get ERPConfigurator ERP Class Name.
        public static string GetERPClassName() => $"{ApiRoot}/geterpclassname";

        //Get ERPTaskSchedulerId  from ERP TouchPointName.
        public static string GetSchedulerIdByTouchPointName() => $"{ApiRoot}/erptaskscheduler/GetSchedulerIdByTouchPointName";

    }
}
