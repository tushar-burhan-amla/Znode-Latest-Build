namespace Znode.Libraries.Search
{
    public class SearchCMSPages
    {
        public int versionid { get; set; }
        public int localeid { get; set; }
        public int portalid { get; set; }
        public int contentpageid { get; set; }
        public string pagename { get; set; }
        public int[] profileid { get; set; }
        public string indexid { get; set; }
        public bool isactive { get; set; }
        public string revisionType { get; set; }
        public string pagetitle { get; set; }
        public string[] text { get; set; }
        public string seodescription { get; set; }
        public string seotitle { get; set; }
        public string seourl { get; set; }
        public long timestamp { get; set; }
        public string didyoumean { get; set; }
        public string blognewstype { get; set; }
        public int blognewsid { get; set; }     
        public string pagecode { get; set; }
    }
}
