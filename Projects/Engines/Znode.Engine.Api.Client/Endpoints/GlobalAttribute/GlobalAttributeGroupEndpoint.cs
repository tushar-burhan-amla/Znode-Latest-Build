namespace Znode.Engine.Api.Client.Endpoints
{
    public class GlobalAttributeGroupEndpoint : BaseEndpoint
    {
        public static string GetAttributeGroups() => $"{ApiRoot}/globalattributegroup/list";

        public static string CreateAttributeGroup() => $"{ApiRoot}/globalattributegroup/create";

        public static string GetAttributeGroup(int id) => $"{ApiRoot}/globalattributegroup/get/{id}";

        public static string UpdateAttributeGroup() => $"{ApiRoot}/globalattributegroup/update";

        public static string GetGlobalAttributeGroupLocales(int globalAttributeGroupId) => $"{ApiRoot}/globalattributegroup/attributegrouplocales/{globalAttributeGroupId}";

        public static string DeleteAttributeGroup() => $"{ApiRoot}/globalattributegroup/delete";

        public static string GetAssignedAttributes() => $"{ApiRoot}/globalattributegroup/assignedattributes";

        public static string GetUnAssignedAttributes() => $"{ApiRoot}/globalattributegroup/unassignedattributes";

        public static string AssociateAttributes() => $"{ApiRoot}/globalattributegroup/associateattributes";

        public static string RemoveAssociatedAttributes() => $"{ApiRoot}/globalattributegroup/removeassociatedattributes";

        public static string UpdateAttributeDisplayOrder() => $"{ApiRoot}/globalattributegroup/updateattributedisplayorder";
    }
}
