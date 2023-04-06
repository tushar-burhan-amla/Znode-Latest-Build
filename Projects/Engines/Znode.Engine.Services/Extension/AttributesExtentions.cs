using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Services
{
    public static class AttributesExtentions
    {
        /// <summary>
        /// Get attribute value.
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="attributeCode"></param>
        /// <returns></returns>
        public static string Value(this List<PublishAttributeModel> attributes, string attributeCode) => attributes?.FirstOrDefault(x => x.AttributeCode == attributeCode)?.AttributeValues;

        /// <summary>
        /// Get attribute code.
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="attributeCode"></param>
        /// <returns></returns>
        public static string Code(this List<PublishAttributeModel> attributes, string attributeCode) => attributes?.FirstOrDefault(x => x.AttributeCode == attributeCode)?.AttributeCode;

        /// <summary>
        /// Get attribute label.
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="attributeCode"></param>
        /// <returns></returns>
        public static string Label(this List<PublishAttributeModel> attributes, string attributeCode) => attributes?.FirstOrDefault(x => x.AttributeCode == attributeCode)?.AttributeName;

        /// <summary>
        /// Get list of select type attribute from main attribute.
        /// </summary>
        /// <param name="attributes">List of AttributesViewModel </param>
        /// <param name="attributeCode">attribute Code</param>
        /// <returns>List of AttributesSelectValuesViewModel</returns>
        public static List<AttributesSelectValuesModel> SelectAttributeList(this List<PublishAttributeModel> attributes, string attributeCode)
        => attributes?.FirstOrDefault(x => x.AttributeCode == attributeCode)?.SelectValues;
    }
}
