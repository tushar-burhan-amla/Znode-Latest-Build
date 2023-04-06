using System;
using System.Collections.Generic;
using Znode.Engine.Api.Models;

namespace Znode.Libraries.ECommerce.ShoppingCart
{
    /// <summary>
    /// Represents a product 
    /// </summary>    
    [Serializable()]
    public class ZnodeProduct : ZnodeProductBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ZNodeProduct class
        /// </summary>
        public ZnodeProduct()
        {
        }
        #endregion

        #region Public Properties        

        // Gets or sets the MaxQuantity       
        public override decimal? MaxQty
        {
            get
            {
                return _MaxQty;
            }

            set { _MaxQty = value; }
        }      

        public List<WebStoreAddOnModel> AddOns { get; set; }

        public string UOM { get; set; }

        #endregion      
    }
}
