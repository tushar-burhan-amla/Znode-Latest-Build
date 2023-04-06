using System;

namespace Znode.Engine.ERPConnector
{
    /// <summary>
    /// This data is used for checking web-connector connector connection.
    /// </summary>
    public static class DefaultData
    {
        #region Static data for testing purpose

        /// <summary>
        /// Default object for adding new address reference
        /// </summary>
        public static Address Address { get; } = new Address()
        {
            Addr1 = "Alpha Block, B-Wing",
            Addr2 = "SEZ Mihan",
            Addr3 = "W-Building, Nagpur",
            City = "Nagpur",
            State = "Maharashtra",
            Country = "India",
            PostalCode = "440023"
        };

        /// <summary>
        /// Default static data for new customer
        /// </summary>
        public static CustomerAdd CustomerAdd { get; } = new CustomerAdd()
        {
            Name = "Znode Customer",
            FirstName = "Znode",
            LastName = "Customer",
            MiddleName = string.Empty,
            Phone = string.Empty,
            Email = "Znode.yopmail.com",
            Contact = string.Empty,
            BillAddress = Address
        };

        /// <summary>
        /// Static data for adding new customer QB XML object
        /// </summary>
        public static CustomerAddRq CustomerAddRq { get; } = new CustomerAddRq()
        {
            CustomerAdd = CustomerAdd
        };

        /// <summary>
        /// Static data for adding new inventory item
        /// </summary>
        public static ItemInventoryAdd ItemInventoryAdd { get; } = new ItemInventoryAdd()
        {
            AssetAccountRef = new Ref()
            {
                FullName = QuickBooksConstants.AssetAccountType
            },
            COGSAccountRef = new Ref()
            {
                FullName = QuickBooksConstants.COGSAccountType
            },
            IncomeAccountRef = new Ref()
            {
                FullName = QuickBooksConstants.IncomeAccountType
            },
            Name = "SKURP2345",
            SalesPrice = "6.00",
            SalesDesc = "Raw Peanuts"
        };

        /// <summary>
        /// Add request object for inserting new inventory with static data
        /// </summary>
        public static ItemInventoryAddRq ItemInventoryAddRq { get; } = new ItemInventoryAddRq()
        {
            ItemInventoryAdd = ItemInventoryAdd
        };

        /// <summary>
        /// Static data for new sales order insertion
        /// </summary>
        public static SalesOrderAdd SalesOrderAdd { get; } = new SalesOrderAdd()
        {
            RefNumber = "1111",
            BillAddress = Address,
            CustomerRef = new Ref()
            {
                FullName = CustomerAdd.Name
            },
            SalesOrderLineAdd = new SalesOrderLineAdd[] {
                         new SalesOrderLineAdd() {
                              Amount = ItemInventoryAdd.SalesPrice,
                              Desc = ItemInventoryAdd.SalesDesc,
                              ItemRef = new Ref()
                              {
                                   FullName = ItemInventoryAdd.Name
                              },
                              Quantity = "1"
                         },
                         new SalesOrderLineAdd() {
                              Amount = ItemInventoryAdd.SalesPrice,
                              Desc = ItemInventoryAdd.SalesDesc,
                              ItemRef = new Ref()
                              {
                                      FullName = ItemInventoryAdd.Name
                              },
                              Quantity = "2"
                         }
                     },
            ShipAddress = Address,
            TxnDate = DateTime.Now.ToString(QuickBooksConstants.DateFormat),
            Memo = "Znode demo Sales Order"
        };

        /// <summary>
        /// Static data of add request for inserting new sales order
        /// </summary>
        public static SalesOrderAddRq SalesOrderAddRq { get; } = new SalesOrderAddRq()
        {
            SalesOrderAdd = SalesOrderAdd
        };

        #endregion Static data for testing purpose
    }
}