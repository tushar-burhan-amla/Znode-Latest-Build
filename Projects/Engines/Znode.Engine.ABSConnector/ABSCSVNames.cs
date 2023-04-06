namespace Znode.Engine.ABSConnector
{
    public static class ABSCSVNames
    {
        //Products
        public const string ProductParentLevelDetail = "PRDH";
        public const string ProductParenLevelAttributes = "PRDHA";
        public const string ProductSKULevelDetail = "PRDD";
        public const string ProductSKULevelAttributes = "PRDDA";

        //Orders
        public const string OrderHeaderFile = "MAGOH";
        public const string OrderDetailFile = "MAGOD";
        public const string OrderShipFile = "MAGOS";

        //Inventory
        public const string InventoryBySKU = "MAGINV";

        //Order Status
        public const string LineDetailStatusInformation = "MAGSTAT";

        //Customers
        public const string CustomerSoldTo = "MAGSOLD";
        public const string CustomerShipTo = "MAGSHIP";

        //Tier pricing
        public const string SystemsSpecialPricing = "MAGGRP";

        //Special Promotional Pricing
        public const string SpecialPromotionalPricingSetup = "MAGPRO";

        //Attributes
        public const string Attributes = "AttributeRawData";

        //Pricing
        public const string Pricing = "TPRICE";

    }
}
