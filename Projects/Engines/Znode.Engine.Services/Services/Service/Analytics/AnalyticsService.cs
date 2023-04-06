using Newtonsoft.Json;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Analytics;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Services
{
    public class AnalyticsService : BaseService, IAnalyticsService
    {
        #region Constructor
        public AnalyticsService()
        {
        }
        #endregion

        // Method to get analytics dashboard data
        public virtual AnalyticsModel GetAnalyticsDashboardData()
        {
            ZnodeLogging.LogMessage("Execution started", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            IAnalyticsHelper _analyticsHelper = ZnodeDependencyResolver.GetService<IAnalyticsHelper>();
            return _analyticsHelper.GetAccessToken(DefaultGlobalConfigSettingHelper.AnalyticsJSONKey);
        }

        // Method to get analytics JSON key
        public virtual string GetAnalyticsJSONKey() => DefaultGlobalConfigSettingHelper.AnalyticsJSONKey;

        // Method to update the analytics details
        public virtual bool UpdateAnalyticsDetails(AnalyticsModel analyticsDetailsModel)
        {
            bool isUpdated = true;
            ZnodeLogging.LogMessage("Execution started", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);

            if (HelperUtility.IsNull(analyticsDetailsModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            IZnodeRepository<ZnodeGlobalSetting> _globalSettingRepository = new ZnodeRepository<ZnodeGlobalSetting>();

            ZnodeGlobalSetting globalSetting = _globalSettingRepository.Table.FirstOrDefault(x => x.FeatureName == GlobalSettingEnum.AnalyticsJSONKey.ToString());

            //Serialize the JSON to save it in database
            analyticsDetailsModel.AnalyticsJSONKey = JsonConvert.SerializeObject(analyticsDetailsModel.AnalyticsJSONKey);

            if (HelperUtility.IsNull(globalSetting))
            {
                ZnodeGlobalSetting globalSettingDetails = _globalSettingRepository.Insert(new ZnodeGlobalSetting() { FeatureName = GlobalSettingEnum.AnalyticsJSONKey.ToString(), FeatureValues = analyticsDetailsModel.AnalyticsJSONKey });
                if (HelperUtility.IsNull(globalSettingDetails?.ZNodeGlobalSettingId))
                    isUpdated = false;
            }
            else if (!Equals(globalSetting?.FeatureValues, analyticsDetailsModel.AnalyticsJSONKey))
            {
                globalSetting.FeatureValues = analyticsDetailsModel.AnalyticsJSONKey;
                isUpdated = _globalSettingRepository.Update(globalSetting);
            }
            return isUpdated;
        }
    }
}
