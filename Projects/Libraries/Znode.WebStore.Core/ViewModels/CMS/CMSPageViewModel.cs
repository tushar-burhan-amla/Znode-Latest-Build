namespace Znode.Engine.WebStore.ViewModels
{
    public class CMSPageViewModel : BaseViewModel
    {
        public int LocaleId { get; set; }

        public int PortalId { get; set; }

        public int ContentPageId { get; set; }

        public string PageName { get; set; }

        public int[] ProfileId { get; set; }

        public bool IsActive { get; set; }

        public string RevisionType { get; set; }

        public string PageTitle { get; set; }

        public string[] Text { get; set; }

        public string SeoDescription { get; set; }

        public string SeoTitle { get; set; }

        public string SeoURL { get; set; }

        public string BlogNewsType { get; set; }

        public int BlogNewsId { get; set; }
    }
}
