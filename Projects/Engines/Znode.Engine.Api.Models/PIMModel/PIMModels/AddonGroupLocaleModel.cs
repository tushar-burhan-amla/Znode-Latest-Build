namespace Znode.Engine.Api.Models
{
    public class AddonGroupLocaleModel:BaseModel
    {
        public int PimAddonGroupLocaleId { get; set; }
        public int? PimAddonGroupId { get; set; }
        public string AddonGroupName { get; set; }
        public int? LocaleId { get; set; }

        public AddonGroupModel ZnodePimAddonGroup { get; set; }

    }
}
