using System.Collections.Generic;
namespace Znode.Engine.Api.Models
{
    public class ProductFeedModel : BaseModel
    {
        public int ProductFeedId { get; set; }
        public int PortalId { get; set; }
        public int LocaleId { get; set; }
        public string ChangeFreq { get; set; }
        public string RootTag { get; set; }
        public string RootTagValue { get; set; }
        public string XmlFileName { get; set; }
        public string SuccessXMLGenerationMessage { get; set; }
        public PortalListModel StoreList { get; set; }
        public string ErrorMessage { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public int ProductFeedSiteMapTypeId { get; set; }
        public int ProductFeedTypeId { get; set; }
        public string FileName { get; set; }
        public string ProductFeedTypeCode { get; set; }
        public string ProductFeedSiteMapTypeCode { get; set; }
        public string ProductFeedTypeName { get; set; } 
        public List<ProductFeedSiteMapTypeModel> ProductFeedSiteMapTypeList { get; set; }
        public List<ProductFeedTypeModel> ProductFeedTypeList { get; set; }
        public bool IsFromScheduler { get; set; }        
        public int UserId { get; set; }
        public string Token { get; set; }
        public string StoreName { get; set; }
        public int FileCount { get; set; }
        public string LocaleName { get; set; }
    }
}
