using System;
using System.IO;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Controllers;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Areas.MediaManager.Controllers
{
    public class MediaConfigurationController : BaseController
    {
        #region Private variables
        private IMediaConfigurationAgent _mediaConfigurationAgent;
        #endregion

        public MediaConfigurationController(IMediaConfigurationAgent mediaConfigurationAgent)
        {
            _mediaConfigurationAgent = mediaConfigurationAgent;
        }

        //Media configurations.
        public virtual ActionResult MediaConfiguration(int ServerId = 0)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;
            MediaConfigurationViewModel mediaSettingViewModel = new MediaConfigurationViewModel();
            if (ServerId <= 0)
                mediaSettingViewModel  = _mediaConfigurationAgent.GetDefaultMediaConfiguration();
            else
                mediaSettingViewModel = _mediaConfigurationAgent.GetMediaConfiguration(ServerId);
            mediaSettingViewModel.IsLocalServer = string.Equals(mediaSettingViewModel.Server, AdminConstants.DefaultMediaServerName, StringComparison.OrdinalIgnoreCase);
            mediaSettingViewModel.IsNetworkDrive = string.Equals(mediaSettingViewModel.Server, AdminConstants.NetworkDrive, StringComparison.OrdinalIgnoreCase);
            mediaSettingViewModel.OptionsList = _mediaConfigurationAgent.GetOptionsForMediaServer(mediaSettingViewModel.MediaServerMasterId);
            return View(mediaSettingViewModel);
        }

        //Save media settings.
        [HttpPost]
        public virtual ActionResult SaveMediaSettings(MediaConfigurationViewModel model, string ServerId)
        {
            //If IsLocalServer is true then have to skip the ModelState.IsValid condition.
            if ((model.IsLocalServer || model.IsNetworkDrive) || ModelState.IsValid)
            {
                if (model.IsLocalServer || model.IsNetworkDrive)
                    _mediaConfigurationAgent.SetLocalConfiguration(model);
                else
                    model.Server = ServerId;
                if (!string.IsNullOrEmpty(model.NetworkUrl))
                {
                    string message = checkDirectoryPath(model.NetworkUrl);
                    if (!string.IsNullOrEmpty(message))
                    {
                        SetNotificationMessage(GetErrorNotificationMessage(message));
                        return RedirectToAction<MediaConfigurationController>(x => x.MediaConfiguration(model.MediaServerMasterId));
                    }
                }
                _mediaConfigurationAgent.UpdateMediaConfiguration(model);
                SetNotificationMessage(model.HasError ? GetErrorNotificationMessage(model.ErrorMessage) : GetSuccessNotificationMessage(Admin_Resources.UpdateMessageOfMediaSetting));
                if (model.HasError)
                {
                    model.OptionsList = _mediaConfigurationAgent.GetOptionsForMediaServer(model.MediaServerMasterId);
                    return View("MediaConfiguration", model);
                }
            }
            else
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.Error));
            //-1 as folder id to get data of root folder.
            return RedirectToAction<MediaConfigurationController>(x => x.MediaConfiguration(model.MediaServerMasterId));
        }

        //Check wheather network drive is accessible or not
        public string checkDirectoryPath(string path)
        {
            string message = string.Empty;
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception ex)
            {
                return message = ex.Message;
            }
            return message;
        }

        //Get media setting.
        public virtual ActionResult GetMediaSetting(string partialViewName, int serverId)
            => (serverId > 0) ? PartialView(partialViewName, _mediaConfigurationAgent.GetMediaConfiguration(serverId) ?? new MediaConfigurationViewModel()) : null;

        //Get Locale server url.
        public virtual JsonResult GetLocalServerURL()
            => Json(new { URL = _mediaConfigurationAgent.GetLocalServerURL() }, JsonRequestBehavior.AllowGet);

        //Get Network Drive url.
        public virtual JsonResult GetNetworkDriveURL()
            => Json(new { URL = _mediaConfigurationAgent.GetNetworkDriveURL() }, JsonRequestBehavior.AllowGet);

        //GetNetworkDriveURL

        #region Sync Media
        [HttpPost]
        public virtual JsonResult syncMedia(string folderName)
            => Json(_mediaConfigurationAgent.SyncMedia(folderName), JsonRequestBehavior.AllowGet);
        #endregion

        #region Generate Images

        //Generate images based on the display settings.
        [HttpGet]
        public virtual ActionResult GenerateImages()
        {
            bool result = _mediaConfigurationAgent.GenerateImages();
            return Json(new { status = true, message = Admin_Resources.ProcessingStarted }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}