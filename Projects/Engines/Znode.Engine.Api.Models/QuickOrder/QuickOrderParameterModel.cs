using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models
{
    public class QuickOrderParameterModel
    {
        /// <summary>
        /// This helps to pass in query parameter (Comma seperated string)
        /// </summary>
        [Required]
        public string SKUs { get; set; }
    }
}
