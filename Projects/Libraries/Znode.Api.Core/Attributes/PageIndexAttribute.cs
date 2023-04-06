namespace Znode.Engine.Api.Attributes
{
    public class PageIndexAttribute : PageAttribute
    {
        public override string Name { get { return "index"; } }
        public override string Description { get { return "The index to retrieve when requesting paged results."; } }
    }
}