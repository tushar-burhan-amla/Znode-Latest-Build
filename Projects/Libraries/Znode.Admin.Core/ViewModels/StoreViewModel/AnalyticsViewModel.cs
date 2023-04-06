using System;
using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class AnalyticsViewModel : BaseViewModel
    {
        public int PortalId { get; set; }

        public TagManagerViewModel TagManager { get; set; }

        public PortalTrackingPixelViewModel TrackingPixel { get; set; }

        
    }
}
