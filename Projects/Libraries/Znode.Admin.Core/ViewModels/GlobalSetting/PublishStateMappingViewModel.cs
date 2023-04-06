﻿namespace Znode.Engine.Admin.ViewModels
{
    public class PublishStateMappingViewModel : BaseViewModel
    {
        public int PublishStateMappingId { get; set; }
        public byte PublishStateId { get; set; }
        public string PublishState { get; set; }
        public string PublishStateCode { get; set; }
        public string ApplicationType { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }
        public bool IsEnabled { get; set; }
        public int DisplayOrder { get; set; }
    }
}
