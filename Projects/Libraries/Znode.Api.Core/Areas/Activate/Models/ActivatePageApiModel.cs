using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Web;

namespace Znode.Engine.Api.Areas.Activate.Models
{
    public class ActivatePageApiModel
    {
        /// <summary>
        /// This method reads the EULA file and assign to Eula property and set default License Type as FreeTrial
        /// </summary>
        //This method reads the EULA file and assign to Eula property and set default License Type as FreeTrial
        public ActivatePageApiModel()
        {
            Eula = GetEULA();
            LicenseType = "FreeTrial";
        }

        public string LicenseType { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        public string FullName { get; set; }

        public string SerialNumber { get; set; }

        [Required(ErrorMessage = "Email number is required")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Not a valid Email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Activation requires that you accept the software license agreement (EULA)")]
        public bool CheckEula { get; set; }

        public string Eula { get; set; }

        public string Message { get; set; }

        public string ErrorMessage { get; set; }


        /// <summary>
        /// Get the Software License Agreement
        /// </summary>
        /// <returns>Returns the Software License Agreement</returns>
        public static string GetEULA()
        {
            string path = HttpContext.Current.Server.MapPath("~/eula.txt");
            return File.ReadAllText(path);
        }
    }
}
