using System.ComponentModel.DataAnnotations;

namespace Znode.Libraries.ECommerce.Utilities
{
    public enum ApplicationTypesEnum
    {
        Admin,
        API,
        [Display(Name = "Webstore")]
        WebStore,
        [Display(Name = "Webstore Preview")]
        WebstorePreview
    }
}
