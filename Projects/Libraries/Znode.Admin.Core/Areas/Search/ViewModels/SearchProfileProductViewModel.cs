using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Znode.Engine.Admin.ViewModels;

namespace Znode.Admin.Core.Areas.Search.ViewModels
{
    public class SearchProfileProductViewModel : BaseViewModel
    {
        public string Id { get; set; }
        public string ExternalId { get; set; }
        public int ZnodeProductId { get; set; }
        public int Version { get; set; }
        public int LocaleId { get; set; }
        public string Name { get; set; }
        public int CatalogId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ImageLargePath { get; set; }
        public string ImageMediumPath { get; set; }
        public string ImageThumbNailPath { get; set; }
        public string ImageSmallPath { get; set; }
        public string ImageSmallThumbnailPath { get; set; }
        public List<AttributesViewModel> Attributes { get; set; }
    }
}
