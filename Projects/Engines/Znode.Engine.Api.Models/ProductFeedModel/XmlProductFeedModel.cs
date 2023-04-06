using System.Web;
using System.Xml.Serialization;

namespace Znode.Engine.Api.Models
{
    [XmlRoot(ElementName = "item")]
    public class XmlProductFeedModel
    {
        [XmlIgnore]
        public string DomainName { get; set; }
        public string Title { get; set; }
        private string id;
        public string ID
        {
            get => id;
            set
            {
                if (DomainName == null || ProductID == null || value == null)
                {
                    Title = null;
                    id = null;
                    Brand = null;
                    Category = null;
                    Description = null;
                    Availability = null;
                    inventory = null;
                    Price = null;
                    link = null;
                    image_Link = null;
                }
                else
                {
                    id = value;
                }
            }
        }
        public string Brand { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        [XmlIgnore]
        public string MediaConfiguration { get; set; }
        [XmlIgnore]
        private string image_Link;
        public string Availability { get; set; }
        [XmlIgnore]
        private string inventory = "0";
        public string Inventory
        {
            get => inventory;
            set
            {
                if (value != null)
                {
                    inventory = value;
                }
            }
        }
        public string Price { get; set; }
        [XmlIgnore]
        public string ProductID { get; set; }
        [XmlIgnore]
        private string link;
        public string Image_Link
        {
            get
            {
                return image_Link;
            }
            set
            {
                if (value != null)
                {
                    image_Link = MediaConfiguration + value;
                }
            }
        }
        public string Link
        {
            get
            {
                return link;
            }
            set
            {
                {
                    if (DomainName != null || ProductID != null)
                    {
                        if(value == null)
                        link = $"{HttpContext.Current.Request.Url.Scheme}://{DomainName}{"/product/"}{ProductID}";
                        else
                        link = $"{HttpContext.Current.Request.Url.Scheme}://{DomainName}/{value}";
                    }
                }
            }
        }
    }
}

