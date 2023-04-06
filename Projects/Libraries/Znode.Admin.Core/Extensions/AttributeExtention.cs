using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Admin.ViewModels;

namespace Znode.Engine.Admin
{
    public static class AttributeExtention
    {
        /// <summary>
        /// Get attribute value.
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="attributeCode"></param>
        /// <returns></returns>
        public static string Value(this List<PublishAttributeViewModel> attributes, string attributeCode) => attributes?.FirstOrDefault(x => x.AttributeCode == attributeCode)?.AttributeValues;

        /// <summary>
        /// Get attribute code.
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="attributeCode"></param>
        /// <returns></returns>
        public static string Code(this List<PublishAttributeViewModel> attributes, string attributeCode) => attributes?.FirstOrDefault(x => x.AttributeCode == attributeCode)?.AttributeCode;

        /// <summary>
        /// Get attribute label.
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="attributeCode"></param>
        /// <returns></returns>
        public static string Label(this List<PublishAttributeViewModel> attributes, string attributeCode) => attributes?.FirstOrDefault(x => x.AttributeCode == attributeCode)?.AttributeName;

        /// <summary>
        /// Get list of select type attribute from main attribute.
        /// </summary>
        /// <param name="attributes">List of PublishAttributeViewModel </param>
        /// <param name="attributeCode">attribute Code</param>
        /// <returns>List of AttributesSelectValuesViewModel</returns>
        public static List<AttributesSelectValuesViewModel> SelectAttributeList(this List<PublishAttributeViewModel> attributes, string attributeCode)
        => attributes?.FirstOrDefault(x => x.AttributeCode == attributeCode)?.SelectValues;

        /// <summary>
        /// Get list of select type attribute from main attribute.
        /// </summary>
        /// <param name="attributes">List of AttributesViewModel </param>
        /// <param name="attributeCode">attribute Code</param>
        /// <returns>List of AttributesSelectValuesViewModel</returns>
        public static List<AttributesSelectValuesViewModel> SelectAttributeList(this List<AttributesViewModel> attributes, string attributeCode)
        => attributes?.FirstOrDefault(x => x.AttributeCode == attributeCode)?.SelectValues;


        /// <summary>
        /// Get attribute value.
        /// </summary>
        /// <param name="attributes">List of AttributesViewModel</param>
        /// <param name="attributeCode">attribute Code</param>
        /// <returns>Attribute Value</returns>
        public static string Value(this List<AttributesViewModel> attributes, string attributeCode) => attributes?.FirstOrDefault(x => x.AttributeCode == attributeCode)?.AttributeValues;
    }
}