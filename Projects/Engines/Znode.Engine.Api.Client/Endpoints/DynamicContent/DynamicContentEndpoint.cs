namespace Znode.Engine.Api.Client.Endpoints
{
    public class DynamicContentEndpoint : BaseEndpoint
    {
        //To get WYSIWYG editor formats.
        public static string GetEditorFormats(int portalId) => $"{ApiRoot}/geteditorformats/{portalId}";
    }
}
