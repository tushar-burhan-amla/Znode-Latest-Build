namespace Znode.Engine.WebStore.ViewModels
{
    public class SwatchImageViewModel : BaseViewModel
    {
        public string ImagePath { get; set; }
        public string AttributeCode { get; set; }
        public string AttributeValues { get; set; }
        public bool IsSelected { get; set; }
        public bool IsDisabled { get; set; }
    }
}