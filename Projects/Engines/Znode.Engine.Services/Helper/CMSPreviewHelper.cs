using System;
using System.Diagnostics;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Services.Helper
{
    public static class CMSPreviewHelper
    {
        #region public Methods        

        //Publish the Saved WidgetConfiguration to Preview.
        public static bool PublishWidgetToPreview(CMSTypeMappingModel typeModel)
        {
            bool status = false;
            try
            {
                if (HelperUtility.IsNotNull(typeModel))
                {
                    bool IsGlobalPreviewEnabled = new PublishProcessor().IsWebstorePreviewEnabled();
                    //Publish to Preview after content saving
                    if (IsGlobalPreviewEnabled && typeModel.EnableCMSPreview && typeModel.TypeOfMapping.Equals(ZnodeCMSTypeofMappingEnum.ContentPageMapping.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        //publish content page details.                    
                        PublishedModel model = ZnodeDependencyResolver.GetService<IContentPageService>().PublishContentPage(typeModel.CMSMappingId, 0, typeModel.LocaleId, ZnodePublishStatesEnum.PREVIEW.ToString(), true);
                        //setting error message based on returned status
                        status = model.IsPublished;
                        if (!model.IsPublished)
                        {
                            ZnodeLogging.LogMessage(model.ErrorMessage, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                        }
                    }
                }
                else
                {
                    ZnodeLogging.LogMessage(Admin_Resources.ModelCanNotBeNull);
                }
                return status;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return status;
        }
        #endregion      
    }
}
