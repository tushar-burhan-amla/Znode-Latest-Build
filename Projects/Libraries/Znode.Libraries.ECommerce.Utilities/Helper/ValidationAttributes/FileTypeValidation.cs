using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Znode.Libraries.ECommerce.Utilities
{
    public class FileTypeValidation : ValidationAttribute
    {
        private readonly List<string> types;

        public FileTypeValidation(string types)
        {
            this.types = types.Split(',').ToList();
        }

        public override bool IsValid(object value)
        {
            if (value == null) return true;
            bool isValid = true;

            HttpPostedFileBase file = value as HttpPostedFileBase;

            if (file != null)
            {
                string fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
                isValid = types.Contains(fileExt, StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                HttpPostedFileBase[] files = value as HttpPostedFileBase[];
                if (files == null) return true;

                foreach (var item in files)
                {
                    if (item == null) return true;

                    string fileExt = System.IO.Path.GetExtension(item.FileName).Substring(1);
                    isValid = types.Contains(fileExt, StringComparer.OrdinalIgnoreCase);

                    if (!isValid) break;
                }
            }
            return isValid;
        }
    }
}
