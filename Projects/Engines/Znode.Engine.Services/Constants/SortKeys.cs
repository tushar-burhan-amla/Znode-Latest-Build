namespace Znode.Engine.Services.Constants
{
    public static class SortKeys
    {
        //Dynamic grid constants.
        public const string sortKey = "sort";
        public const string sortDirKey = "sortdir";

        //Sort direction constants.
        public static string Ascending { get; } = "asc";
        public static string Descending { get; } = "desc";

        //Sort key constants.
        public static string AttributeFamilyId { get; } = "AttributeFamilyId";
        public static string AttributeGroupId { get; } = "AttributeGroupId";
    }
}
