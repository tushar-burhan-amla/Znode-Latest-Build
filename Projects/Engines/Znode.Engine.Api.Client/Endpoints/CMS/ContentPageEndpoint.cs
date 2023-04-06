namespace Znode.Engine.Api.Client.Endpoints
{
    public class ContentPageEndpoint : BaseEndpoint
    {
        #region Content Page
        //Create Content Page Endpoint.
        public static string CreateContentPage() => $"{ApiRoot}/contentpage/createcontentpage";

        //Get Content Page List Endpoint.
        public static string GetContentPageList() => $"{ApiRoot}/contentpage/contentpagelist";

        //Get Content Page on the basis of Content Page id Endpoint.
        public static string GetContentPage() => $"{ApiRoot}/contentpage/getcontentpage";

        //Update  Content Page Endpoint.
        public static string UpdateContentPage() => $"{ApiRoot}/contentpage/updatecontentpage";

        //Delete Content Page Endpoint.
        public static string DeleteContentPage() => $"{ApiRoot}/contentpage/deletecontentpage";

        //Publish Content Page Endpoint.
        public static string PublishContentPage() => $"{ApiRoot}/contentpage/publishcontentpages";

        //Save Content Page as preview Endpoint.
        public static string PublishContentPageWithPreview() => $"{ApiRoot}/contentpage/publishcontentpagewithpreview";

        //Get Content Page Template List Endpoint.
        public static string GetContentPageTemplateList() => $"{ApiRoot}/contentpage/contentpagetemplatelist";

        //Get Content Page Template on the basis of Content Page Template id Endpoint.
        public static string GetContentTemplatePage(int cmsContentPageTemplateId) => $"{ApiRoot}/contentpage/getcontentpageTemplate/{cmsContentPageTemplateId}";
        #endregion

        #region Content Page Tree
        public static string GetTree() => $"{ApiRoot}/contentpage/gettree";

        public static string RenameFolder() => $"{ApiRoot}/contentpage/renamefolder";

        public static string AddFolder() => $"{ApiRoot}/contentpage/addfolder";

        public static string DeleteFolder() => $"{ApiRoot}/contentpage/deletefolder";

        //Get Content page list.
        public static string GetContentPagesList() => $"{ApiRoot}/contentpage/contentpageslist";

        //Endpoint for moving folder to another folder.
        public static string MoveContentPagesFolder() => $"{ApiRoot}/contentpage/MoveContentPagesFolder";

        //Endpoint for moving page from one folder to another folder.
        public static string MovePageToFolder() => $"{ApiRoot}/contentpage/move";

        #endregion
    }
}
