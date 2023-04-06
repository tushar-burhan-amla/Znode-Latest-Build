using System.Collections.Generic;
using Znode.Engine.WebStore.ViewModels;

namespace Znode.Engine.WebStore.Agents
{
    public interface IAttributeAgent
    {
        /// <summary>
        /// Get attribute validation by attribute code.
        /// </summary>
        /// <param name="model">attribute codes.</param>
        /// <returns>list of attribute validations.</returns>
        List<AttributeValidationViewModel> GetAttributeValidationByCodes(int ProductId, Dictionary<string, string> PersonliseValues);
    }
}