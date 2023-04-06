namespace Znode.Engine.Api.Attributes
{
    public class PageSizeAttribute : PageAttribute
    {
        public override string Name { get { return "size"; } }
        public override string Description { get { return "The size of the page when retrieving paged results."; } }
    }
}