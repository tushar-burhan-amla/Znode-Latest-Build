using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class GeneralSettingModel : BaseModel
    {
        public int DisplayUnitId { get; set; }
        public List<DisplayUnitModel> DisplayUnitList { get; set; }

        public int DateFormatId { get; set; }
        public List<DateFormatModel> DateFormatList { get; set; }

        public int TimeFormatId { get; set; }
        public List<TimeFormatModel> TimeFormatList { get; set; }

        public int TimeZoneId { get; set; }
        public List<TimeZoneModel> TimeZoneList { get; set; }

        public int WeightUnitId { get; set; }
        public List<WeightUnitModel> WeightUnitList { get; set; }

        public int PriceRoundOffFeatureValue { get; set; }
        public List<DefaultGlobalConfigModel> PriceRoundOffList { get; set; }

        public int InventoryRoundOffFeatureValue { get; set; }
        public List<DefaultGlobalConfigModel> InventoryRoundOffList { get; set; }

        public string CurrentEnvironmentFeatureValue { get; set; }
        public List<DefaultGlobalConfigModel> EnvironmentsList { get; set; }

        public string ServerTimeZone { get; set; }
 }
}
