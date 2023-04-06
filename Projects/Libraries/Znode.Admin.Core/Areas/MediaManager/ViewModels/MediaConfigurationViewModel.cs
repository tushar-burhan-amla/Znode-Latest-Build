using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class MediaConfigurationViewModel : BaseViewModel
    {
        public bool IsLocalServer { get; set; }
        public bool IsNetworkDrive { get; set; }
        public int MediaServerMasterId { get; set; }

        [Required]
        public string Server { get; set; }
        public string OptionsList { get; set; }
        public int MediaConfigurationId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelMediaServerURL, ResourceType = typeof(Admin_Resources))]
        public string URL { get; set; }

        [Required(ErrorMessageResourceType = typeof(MediaManager_Resources), ErrorMessageResourceName = "RequiredInput")]
        public string AccessKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(MediaManager_Resources), ErrorMessageResourceName = "RequiredInput")]
        public string SecretKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(MediaManager_Resources), ErrorMessageResourceName = "RequiredInput")]
        public string BucketName { get; set; }

        public string ThumbnailFolderName { get; set; }
        public string NetworkUrl { get; set; }
        public string CDNUrl { get; set; }
        public GlobalMediaDisplaySettingViewModel GlobalMediaDisplaySetting { get; set; }
    }
}
