using Znode.Engine.Api.Models;

namespace Znode.Engine.Services
{
    public interface IServerValidationService
    {
        ValidateServerModel CompairValidation(ValidateServerModel model);

        /// <summary>
        /// Checks server validation
        /// </summary>     
        /// <param name="attributeCodes">Attribute codes.</param>
        /// <param name="value">Attribute value.</param>
        /// <returns>Returns boolean</returns>
        bool CheckServerValidation(string attributeCodes, object value);
    }
}
