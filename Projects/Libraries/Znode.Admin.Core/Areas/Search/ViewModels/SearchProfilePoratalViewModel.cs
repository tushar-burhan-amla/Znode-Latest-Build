﻿namespace Znode.Engine.Admin.ViewModels
{
    public class SearchProfilePortalViewModel : BaseViewModel
    {
        public int PortalSearchProfileId { get; set; }
        public int PortalId { get; set; }
        public int SearchProfileId { get; set; }
        public bool IsDefault { get; set; }

        public string PortalName { get; set; }

        public int PublishCatalogId { get; set; }

        public string CatalogName { get; set; }
        public string ProfileName { get; set; }

    }
}
