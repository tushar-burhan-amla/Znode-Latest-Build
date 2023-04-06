using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models
{
    public class MediaManagerModel : BaseModel
    {
        public int MediaId { get; set; }
        public int? MediaConfigurationId { get; set; }
        public int? AttributeFamilyId { get; set; }
        public string Path { get; set; }
        public string Size { get; set; }
        public string Height { get; set; }
        public string Width { get; set; }
        public string Length { get; set; }
        public string MediaType { get; set; }
        public int MediaPathId { get; set; }
        [Required]
        public string FileName { get; set; }
        public string ShortDescription { get; set; }
        public int MediaCategoryId { get; set; }
        public string Folder { get; set; }
        public bool IsImage { get; set; }
        public string FamilyCode { get; set; }
        public string MediaServerPath { get; set; }
        public string MediaServerThumbnailPath { get; set; }
        public string DisplayName { get; set; }

        public string OldMediaPath { get; set; }

        public bool IsSVGImage { get; set; }
		public int Version { get; set; }
	}
}
