namespace Znode.Engine.Api.Client.Endpoints
{
    public class PIMAttributeGroupEndpoint:BaseEndpoint
    {
        public static string GetAttributeGroups() => $"{ApiRoot}/pimattributegroup/list";

        public static string GetAttributeGroup(int id) => $"{ApiRoot}/pimattributegroup/{id}";

        public static string CreateAttributeGroup() => $"{ApiRoot}/pimattributegroup/create";

        public static string UpdateAttributeGroup() => $"{ApiRoot}/pimattributegroup/update";

        public static string DeleteAttributeGroup() => $"{ApiRoot}/pimattributegroup/delete";

        public static string GetAssignedAttributes() => $"{ApiRoot}/pimattributegroup/assignedattributes";

        public static string GetUnAssignedAttributes() => $"{ApiRoot}/pimattributegroup/unassignedattributes";

        public static string GetPIMAttributeGroupLocales(int pimAttributeGroupId) => $"{ApiRoot}/pimattributegroup/attributegrouplocales/{pimAttributeGroupId}";

        public static string SaveAttributeGroupLocales() => $"{ApiRoot}/pimattributegroup/saveattributegrouplocales";

        public static string AssociateAttributes() => $"{ApiRoot}/pimattributegroup/associateattributes";

        public static string RemoveAssociatedAttributes() => $"{ApiRoot}/pimattributegroup/removeassociatedattributes";

        public static string UpdateAttributeDisplayOrder() => $"{ApiRoot}/pimattributegroup/updateattributedisplayorder";
    }
}
