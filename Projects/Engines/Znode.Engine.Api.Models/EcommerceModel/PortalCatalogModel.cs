using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models
{
    public class PortalCatalogModel : BaseModel
    {
        [Required]
        public int PortalCatalogId { get; set; }
        [Required]
        public int PortalId { get; set; }
        [Required]
        public int PublishCatalogId { get; set; }
        public int? ThemeId { get; set; }
        public int? CssId { get; set; }
        public string CatalogName { get; set; }

        //This property contains active locale of portal.
        public int LocaleId { get; set; }

        //This property contains active server for portal to get media.
        public string MediaServerUrl { get; set; }

        //This property contains ThumbnailUrl from active server for portal to get media.
        public string MediaServerThumbnailUrl { get; set; }

        public List<PublishCatalogModel> PublishCatalogs { get; set; }
    }
}
