namespace Znode.Engine.Api.Client.Endpoints
{
    public class PublishHistoryEndpoint : BaseEndpoint
    {
        public static string List()
            => $"{ApiRoot}/publishhistory/list";

        public static string Delete(int versionId)
            => $"{ApiRoot}/publishhistory/delete/{versionId}";

    }
}
