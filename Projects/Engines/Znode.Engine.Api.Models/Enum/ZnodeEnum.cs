namespace Znode.Engine.Api.Models.Enum
{

    public enum EntityType
    {
        Table = 1,
        StoredProcedure = 2,
        View = 3
    }
    public enum ViewOptions
    {
        Grid = 1,
        Tile = 2,
        Graph = 3,
        Report = 4
    }
    public enum EntityName
    {
        R_ApplicationSettingEntity = 1
    }

    public enum GetObjectColumnListParameter
    {
        V = 1,//for View
        U = 2, //for Table
        P = 3 //for Procedure
    }
    public enum ViewMode
    {
        Create = 1,
        Edit = 2,
        Delete = 3
    }

    public enum UploadStatusCode
    {
        ExtensionNotAllow = 10,
        FileAlreadyExist = 20,
        MaxFileSize = 30,
        Corrupt = 40,
        Error = 50,
        Done = 60,
        Removed = 70,
        SelectSingleFile = 80,
        SelectFile = 90,
        UnSupportedFile = 100
    }
    public enum ZnodePaymentStatus
    {
        /// <summary>
        /// Credit card Authorized
        /// </summary>
        AUTHORIZED,

        /// <summary>
        /// Credit Card Captured.
        /// </summary>
        CAPTURED,

        /// <summary>
        /// Credit Card Declined
        /// </summary>
        DECLINED,

        /// <summary>
        /// Credit Card Refunded
        /// </summary>
        REFUNDED,

        /// <summary>
        /// Credit Card Payment was Voided
        /// </summary>
        VOIDED,

        /// <summary>
        /// Credit Card Payment Pending
        /// </summary>
        PENDING,

        /// <summary>
        /// Paypal Payment Pending for Review
        /// </summary>
        PENDINGFORREVIEW
    }

    public enum ZnodeEntityTypeEnum
    {
        Store,
        Catalog,
    }

    public enum XMLSiteMap
    {
        XmlSiteMap,
        Google,
        Bing,
        XmlProductFeed,
    }
}

