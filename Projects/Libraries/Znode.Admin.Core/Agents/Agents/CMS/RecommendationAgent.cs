using Znode.Engine.Api.Client;
using Znode.Engine.Api.Models;
using System;
using System.Diagnostics;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using Znode.Libraries.Resources;
using Znode.Engine.Exceptions;

namespace Znode.Engine.Admin.Agents
{
    public class RecommendationAgent : BaseAgent, IRecommendationAgent
    {
        #region Private Variables
        private readonly IRecommendationClient _recommendationClient;
        #endregion

        #region Constructor
        public RecommendationAgent(IRecommendationClient recommendationClient)
        {
            _recommendationClient = GetClient<IRecommendationClient>(recommendationClient);
        }
        #endregion

        #region Public Methods
        //To get the product recommendation setting against the portal Id
        public virtual RecommendationSettingViewModel GetRecommendationSetting(int portalId, string touchPointName)
        {
            RecommendationSettingViewModel recommendationSetting = _recommendationClient.GetRecommendationSetting(portalId, touchPointName)?.ToViewModel<RecommendationSettingViewModel>();
            return IsNotNull(recommendationSetting) ? recommendationSetting : new RecommendationSettingViewModel();
        }

        //To save the product recommendation setting.
        public virtual RecommendationSettingViewModel SaveRecommendationSetting(RecommendationSettingViewModel recommendationSettingViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("RecommendationSettingViewModel with Id", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, recommendationSettingViewModel?.PortalRecommendationSettingId);                
                RecommendationSettingModel recommendationSettingModel = _recommendationClient.SaveRecommendationSetting(recommendationSettingViewModel.ToModel<RecommendationSettingModel>());
                return recommendationSettingModel.ToViewModel<RecommendationSettingViewModel>();
            }
            catch(ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                return (RecommendationSettingViewModel)GetViewModelWithErrorMessage(recommendationSettingViewModel, Admin_Resources.ErrorFailedToCreate);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return (RecommendationSettingViewModel)GetViewModelWithErrorMessage(recommendationSettingViewModel, Admin_Resources.ErrorFailedToCreate);
            }
        }

        //To generate the recommendation engine data.
        public virtual RecommendationGeneratedDataViewModel GenerateRecommendationData(RecommendationDataGenerateViewModel recommendationDataGenerateViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("RecommendationDataGenerateViewModel with portal Id: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, recommendationDataGenerateViewModel?.PortalId);
                RecommendationGeneratedDataModel recommendationGeneratedDataModel = _recommendationClient.GenerateRecommendationData(recommendationDataGenerateViewModel?.ToModel<RecommendationDataGenerateModel>());
                return recommendationGeneratedDataModel?.ToViewModel<RecommendationGeneratedDataViewModel>();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                return (RecommendationGeneratedDataViewModel)GetViewModelWithErrorMessage(new RecommendationGeneratedDataViewModel(), GetErrorMessage(ex));
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return (RecommendationGeneratedDataViewModel)GetViewModelWithErrorMessage(new RecommendationGeneratedDataViewModel(), Admin_Resources.RecommendationDataGenerationFailed);
            }
        }

        //Get the error message on the basis of error codes.
        private static string GetErrorMessage(ZnodeException exception)
        {
            switch (exception.ErrorCode)
            {
                case ErrorCodes.AlreadyExist:
                    return Admin_Resources.RecommendationDataGenerationInProgress;

                case ErrorCodes.NotFound:
                    return Admin_Resources.RecommendationsOrderDetailsNotPresent;

                case ErrorCodes.ProcessingFailed:
                    return Admin_Resources.RecommendationDataProcessingFailed;
                    
                case ErrorCodes.CreationFailed:
                    return Admin_Resources.RecommendationProcessLogCreationFailed;
                    
                default:
                    return Admin_Resources.RecommendationDataGenerationFailed;                    
            }
        }

        #endregion
    }
}
