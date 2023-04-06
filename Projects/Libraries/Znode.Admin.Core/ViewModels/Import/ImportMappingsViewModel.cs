namespace Znode.Engine.Admin.ViewModels
{
    public class ImportMappingsViewModel : BaseViewModel
    {
        public int MappingId { get; set; }
        public string MapCsvColumn { get; set; }
        public string MapTargetColumn { get; set; }
    }
}