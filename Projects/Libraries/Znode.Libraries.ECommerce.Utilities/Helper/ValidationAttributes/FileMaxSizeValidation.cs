using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Znode.Libraries.ECommerce.Utilities
{
    public class FileMaxSizeValidation : ValidationAttribute
    {
        private readonly int maxFileSize;

        //
        public FileMaxSizeValidation(int maxFileSize)
        {
            this.maxFileSize = maxFileSize;
        }

        //Check if value is valid or not.
        public override bool IsValid(object value)
        {
            HttpPostedFileBase file = value as HttpPostedFileBase;
            bool isValid = true;

            //check if file is null.
            if (HelperUtility.IsNotNull(file))
                isValid = file.ContentLength <= maxFileSize;
            else
            {
                HttpPostedFileBase[] files = value as HttpPostedFileBase[];
                if (HelperUtility.IsNull(files)) return true;

                foreach (var item in files)
                {
                    if (HelperUtility.IsNull(item)) return true;

                    isValid = item.ContentLength <= maxFileSize;

                    if (!isValid) break;
                }
            }
            return isValid;
        }
    }
}