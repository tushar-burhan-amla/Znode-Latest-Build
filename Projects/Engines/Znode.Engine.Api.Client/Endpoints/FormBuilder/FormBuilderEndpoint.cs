namespace Znode.Engine.Api.Client.Endpoints
{
    public class FormBuilderEndpoint : BaseEndpoint
    {
        //Create Form Endpoint.
        public static string CreateForm() => $"{ApiRoot}/formbuilder/create";

        // Get form builder list endpoint.
        public static string GetFormBuilderList() => $"{ApiRoot}/formbuilder/list";

        public static string GetForm(int id) => $"{ApiRoot}/formbuilder/get/{id}";

        // Get unassigned attribute
        public static string GetUnAssignedAttributes() => $"{ApiRoot}/formbuilder/unassignedattributes";

        //Delete form builder Endpoint.
        public static string DeleteFormBuilder() => $"{ApiRoot}/formbuilder/delete";

        //Check form code already exist or not.
        public static string IsFormCodeExist(string formCode) => $"{ApiRoot}/formbuilder/isformcodeexist/{formCode}";

        // Get unassigned attribute groups.
        public static string GetUnAssignedGroups() => $"{ApiRoot}/formbuilder/getunassignedgroups";

        //Get default values by attribute id.
        public static string GetFormAttributeGroup(int formBuilderId, int localeId, int mappingId) => $"{ApiRoot}/formbuilder/getformattributegroup/{formBuilderId}/{localeId}/{mappingId}";

        //Check form code already exist or not.
        public static string GetFormAttributeDetails(int formBuilderId, int localeId) => $"{ApiRoot}/formbuilder/getformattributegroup/{formBuilderId}/{localeId}";

        //Assign groups to form.
        public static string AssociateGroups() => $"{ApiRoot}/formbuilder/associategroups";

        //Assign groups to form.
        public static string AssociateAttributes() => $"{ApiRoot}/formbuilder/associateattributes";

        // Update form builder 
        public static string UpdateFormBuilder() => $"{ApiRoot}/formbuilder/update";

        //Update  Order
        public static string UpdateAttributeDisplayOrder()=> $"{ApiRoot}/formbuilder/updateattributedisplayorder";

        //Update form builder Group Order
        public static string UpdateGroupDisplayOrder() => $"{ApiRoot}/formbuilder/updategroupdisplayorder";

        //Un associate group from form builder on the basis of formBuilder Id and group Id.
        public static string UnAssociateFormBuilderGroups(int formBuilderId, int groupId) => $"{ApiRoot}/formbuilder/unAssociateformbuildergroups/{formBuilderId}/{groupId}";

        //Un associate group from form builder on the basis of formBuilder Id and group Id.
        public static string UnAssociateFormBuilderAttributes(int formBuilderId, int attributeId) => $"{ApiRoot}/formbuilder/unassociateformbuilderattributes/{formBuilderId}/{attributeId}";

        //Create form template.
        public static string CreateFormTemplate() => $"{ApiRoot}/formbuilder/createformtemplate";

        //Is Form Attribute Value Unique Template.
        public static string FormAttributeValueUnique() => $"{ApiRoot}/formbuilder/formattributevalueunique";
    }
}
