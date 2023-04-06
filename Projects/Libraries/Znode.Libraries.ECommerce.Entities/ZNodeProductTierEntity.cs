using System;
using System.Xml.Serialization;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.ECommerce.Entities
{
    // Represents a product tier price
    [Serializable()]
    [XmlRoot("ZNodeProductTier")]
    public class ZnodeProductTierEntity : ZnodeBusinessBase
    {

        #region Public Properties  
        // Gets or sets the product tier Id.
        [XmlElement()]
        public int ProductTierID { get; set; }

        // Gets or sets the product Id.
        [XmlElement()]
        public int ProductID { get; set; }

        // Gets or sets the profile Id.
        [XmlElement()]
        public int ProfileID { get; set; }

        // Gets or sets the tier start.
        [XmlElement()]
        public int TierQuantity { get; set; }

        // Gets or sets the price.
        [XmlElement()]
        public decimal Price { get; set; }

        // Gets or sets the tier start.
        [XmlElement()]
        public decimal MinQuantity { get; set; }

        // Gets or sets the tier start.
        [XmlElement()]
        public decimal MaxQuantity { get; set; }
        #endregion
    }
}
