namespace Znode.Engine.Shipping.FedEx
{
    public class FedExEnum
    {
        public enum ServiceType
        {

            /// <remarks/>
            EUROPE_FIRST_INTERNATIONAL_PRIORITY,

            /// <remarks/>
            FEDEX_1_DAY_FREIGHT,

            /// <remarks/>
            FEDEX_2_DAY,

            /// <remarks/>
            FEDEX_2_DAY_AM,

            /// <remarks/>
            FEDEX_2_DAY_FREIGHT,

            /// <remarks/>
            FEDEX_3_DAY_FREIGHT,

            /// <remarks/>
            FEDEX_DISTANCE_DEFERRED,

            /// <remarks/>
            FEDEX_EXPRESS_SAVER,

            /// <remarks/>
            FEDEX_FIRST_FREIGHT,

            /// <remarks/>
            FEDEX_FREIGHT_ECONOMY,

            /// <remarks/>
            FEDEX_FREIGHT_PRIORITY,

            /// <remarks/>
            FEDEX_GROUND,

            /// <remarks/>
            FEDEX_NEXT_DAY_AFTERNOON,

            /// <remarks/>
            FEDEX_NEXT_DAY_EARLY_MORNING,

            /// <remarks/>
            FEDEX_NEXT_DAY_END_OF_DAY,

            /// <remarks/>
            FEDEX_NEXT_DAY_FREIGHT,

            /// <remarks/>
            FEDEX_NEXT_DAY_MID_MORNING,

            /// <remarks/>
            FIRST_OVERNIGHT,

            /// <remarks/>
            GROUND_HOME_DELIVERY,

            /// <remarks/>
            INTERNATIONAL_ECONOMY,

            /// <remarks/>
            INTERNATIONAL_ECONOMY_FREIGHT,

            /// <remarks/>
            INTERNATIONAL_FIRST,

            /// <remarks/>
            INTERNATIONAL_PRIORITY,

            /// <remarks/>
            INTERNATIONAL_PRIORITY_FREIGHT,

            /// <remarks/>
            PRIORITY_OVERNIGHT,

            /// <remarks/>
            SAME_DAY,

            /// <remarks/>
            SAME_DAY_CITY,

            /// <remarks/>
            SMART_POST,

            /// <remarks/>
            STANDARD_OVERNIGHT,
        }

        public enum ExpressRegionCode
        {

            /// <remarks/>
            APAC,

            /// <remarks/>
            CA,

            /// <remarks/>
            EMEA,

            /// <remarks/>
            LAC,

            /// <remarks/>
            US,
        }

        public enum ConsolidationType
        {

            /// <remarks/>
            INTERNATIONAL_DISTRIBUTION_FREIGHT,

            /// <remarks/>
            INTERNATIONAL_ECONOMY_DISTRIBUTION,

            /// <remarks/>
            INTERNATIONAL_GROUND_DIRECT_DISTRIBUTION,

            /// <remarks/>
            INTERNATIONAL_GROUND_DISTRIBUTION,

            /// <remarks/>
            INTERNATIONAL_PRIORITY_DISTRIBUTION,

            /// <remarks/>
            TRANSBORDER_DISTRIBUTION,
        }

        public enum ShipmentOnlyFieldsType
        {

            /// <remarks/>
            DIMENSIONS,

            /// <remarks/>
            INSURED_VALUE,

            /// <remarks/>
            WEIGHT,
        }

        public enum EdtRequestType
        {

            /// <remarks/>
            ALL,

            /// <remarks/>
            NONE,
        }

        public enum FlatbedTrailerOption
        {

            /// <remarks/>
            OVER_DIMENSION,

            /// <remarks/>
            TARP,
        }

        public enum FedExLocationType
        {

            /// <remarks/>
            FEDEX_EXPRESS_STATION,

            /// <remarks/>
            FEDEX_FACILITY,

            /// <remarks/>
            FEDEX_FREIGHT_SERVICE_CENTER,

            /// <remarks/>
            FEDEX_GROUND_TERMINAL,

            /// <remarks/>
            FEDEX_HOME_DELIVERY_STATION,

            /// <remarks/>
            FEDEX_OFFICE,

            /// <remarks/>
            FEDEX_SHIPSITE,

            /// <remarks/>
            FEDEX_SMART_POST_HUB,
        }

        public enum ShipmentNotificationAggregationType
        {

            /// <remarks/>
            PER_PACKAGE,

            /// <remarks/>
            PER_SHIPMENT,
        }

        public enum NotificationEventType
        {

            /// <remarks/>
            ON_DELIVERY,

            /// <remarks/>
            ON_ESTIMATED_DELIVERY,

            /// <remarks/>
            ON_EXCEPTION,

            /// <remarks/>
            ON_SHIPMENT,

            /// <remarks/>
            ON_TENDER,
        }

        public enum ShipmentNotificationRoleType
        {

            /// <remarks/>
            BROKER,

            /// <remarks/>
            OTHER,

            /// <remarks/>
            RECIPIENT,

            /// <remarks/>
            SHIPPER,

            /// <remarks/>
            THIRD_PARTY,
        }

        public enum NotificationType
        {

            /// <remarks/>
            EMAIL,
        }

        public enum NotificationFormatType
        {

            /// <remarks/>
            HTML,

            /// <remarks/>
            TEXT,
        }

        public enum PackagingType
        {

            /// <remarks/>
            FEDEX_10KG_BOX,

            /// <remarks/>
            FEDEX_25KG_BOX,

            /// <remarks/>
            FEDEX_BOX,

            /// <remarks/>
            FEDEX_ENVELOPE,

            /// <remarks/>
            FEDEX_EXTRA_LARGE_BOX,

            /// <remarks/>
            FEDEX_LARGE_BOX,

            /// <remarks/>
            FEDEX_MEDIUM_BOX,

            /// <remarks/>
            FEDEX_PAK,

            /// <remarks/>
            FEDEX_SMALL_BOX,

            /// <remarks/>
            FEDEX_TUBE,

            /// <remarks/>
            YOUR_PACKAGING,
        }

    }
}
