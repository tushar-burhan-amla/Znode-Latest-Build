using System.Linq;

namespace Znode.Libraries.ECommerce.Utilities
{
    public static class GlobalSettingHelper
    {
        //Set Global Setting Sub Feature Values from ZnodeGlobalSetting Table
        public static GlobalSettingValues SetFeatureValue(string subFeatureValues)
        {
            string[] tupleValues = new string[3];
            if (!string.IsNullOrEmpty(subFeatureValues))
            {
                string[] subFeatureValuesArray = subFeatureValues.Split('|');
                for (int cnt = 0; cnt < subFeatureValuesArray.Count(); cnt++)
                {
                    if (cnt + 1 <= tupleValues.Count())
                        tupleValues[cnt] = subFeatureValuesArray.ElementAtOrDefault(cnt) != null ? subFeatureValuesArray[cnt] : string.Empty;
                }
            }
            return new GlobalSettingValues() { Value1 = tupleValues[0], Value2 = tupleValues[1], Value3 = tupleValues[2] };
        }

    }
}
