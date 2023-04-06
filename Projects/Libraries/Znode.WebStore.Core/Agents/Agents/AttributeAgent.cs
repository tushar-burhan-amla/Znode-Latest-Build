using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Models;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.WebStore.Agents
{
    public class AttributeAgent : BaseAgent, IAttributeAgent
    {
        #region Private Variables.
        private readonly IPIMAttributeClient _attributeClient;
        #endregion

        #region Public Constructor.
        public AttributeAgent(IPIMAttributeClient attributeClient)
        {
            _attributeClient = GetClient<IPIMAttributeClient>(attributeClient);
        }
        #endregion

        //Get attribute validation by attribute code.
        public virtual List<AttributeValidationViewModel> GetAttributeValidationByCodes(int productId, Dictionary<string, string> personalizeValues)
        {
            PIMFamilyDetailsModel details = _attributeClient.GetAttributeValidationByCodes(new ParameterProductModel
            {
                HighLightsCodes = string.Join(",", personalizeValues.Select(x => x.Key)),
                LocaleId = PortalAgent.LocaleId
            });
            List<AttributeValidationViewModel> attributeValidationList = details?.Attributes?.ToViewModel<AttributeValidationViewModel>()?.ToList();
            List<string> distinctAttributeCodes = attributeValidationList?.Where(x => x != null)?.Select(e => e.AttributeCode + e.PimAttributeFamilyId)?.Distinct()?.ToList();
            return GetAttributeControls(attributeValidationList, distinctAttributeCodes, personalizeValues);
        }

        //Get dynamic attribute controls
        public virtual List<AttributeValidationViewModel> GetAttributeControls(List<AttributeValidationViewModel> attributeValueList, List<string> distinctAttributeCodes, Dictionary<string, string> personalizeValues)
        {
            int indexValue = 0;
            List<AttributeValidationViewModel> finalAttributeList = new List<AttributeValidationViewModel>();
            if (HelperUtility.IsNotNull(attributeValueList) && HelperUtility.IsNotNull(distinctAttributeCodes))
            {
                finalAttributeList = attributeValueList.Where(x => x != null).GroupBy(x => x.AttributeCode + x.PimAttributeFamilyId).Select(g => g.First()).ToList();

                foreach (string item in distinctAttributeCodes)
                {
                    List<AttributeValidationViewModel> attributesList = attributeValueList.Where(x => x != null && x.AttributeCode + x.PimAttributeFamilyId == item?.ToString()).ToList();

                    //Appended keys with property name {AttributeCode}[0]_{PimAttributeId}[1]_{PimAttributeDefaultValueId}[2]_{PimAttributeValueId}[3]_{PimAttributeFamilyId}[4].
                    string controlName = $"{attributesList[0].AttributeCode}";

                    finalAttributeList[indexValue].ControlProperty.Id = $"{controlName}";
                    finalAttributeList[indexValue].ControlProperty.ControlType = attributesList[0].AttributeTypeName;
                    finalAttributeList[indexValue].ControlProperty.Name = $"{controlName}";
                    finalAttributeList[indexValue].ControlProperty.ControlLabel = attributesList[0].AttributeName;
                    finalAttributeList[indexValue].ControlProperty.Value = string.IsNullOrEmpty(attributesList[0].AttributeValue) ? attributesList[0].AttributeDefaultValue : attributesList[0].AttributeValue;
                    if (Equals(Regex.Replace(attributesList[0].AttributeTypeName, @"\s", ""), ControlTypes.MultiSelect.ToString()) || Equals(Regex.Replace(attributesList[0].AttributeTypeName, @"\s", ""), ControlTypes.SimpleSelect.ToString()))
                    {
                        finalAttributeList[indexValue].ControlProperty.SelectOptions = new List<SelectListItem>();
                        var SelectOptionsList = attributesList.Select(x => new { x.AttributeDefaultValue, x.AttributeDefaultValueCode }).ToList();
                        finalAttributeList[indexValue].ControlProperty.Value = attributesList[0].AttributeValue;
                        foreach (var SelectOptions in SelectOptionsList)
                        {
                            if (!string.IsNullOrEmpty(SelectOptions?.AttributeDefaultValueCode))
                            {
                                finalAttributeList[indexValue].ControlProperty.SelectOptions.Add(new SelectListItem() { Text = SelectOptions.AttributeDefaultValue, Value = SelectOptions.AttributeDefaultValueCode });
                                finalAttributeList[indexValue].ControlProperty.CSSClass = finalAttributeList[indexValue].AttributeCode;
                            }
                        }
                    }
                    if (attributesList[0].IsRequired)
                    {
                        if (IsKeyNotPresent(ZnodeConstant.IsRequired, finalAttributeList[indexValue].ControlProperty.htmlAttributes))
                            finalAttributeList[indexValue].ControlProperty.htmlAttributes.Add(ZnodeConstant.IsRequired, attributesList[0].IsRequired);
                    }

                    if (Equals(Regex.Replace(attributesList[0].AttributeTypeName, @"\s", ""), ControlTypes.Label.ToString()))
                        finalAttributeList[indexValue].ControlProperty.Value = attributesList[0].AttributeDefaultValue;

                    foreach (var dataItem in attributesList)
                    {
                        if (!Equals(dataItem.ValidationName, null) && !Equals(dataItem.ValidationName, ZnodeConstant.Extensions))
                        {
                            if (Equals(dataItem.ControlName, ZnodeConstant.Select) || Equals(dataItem.ControlName, ZnodeConstant.MultiSelect) || Equals(dataItem.ControlName, ControlTypes.SimpleSelect.ToString()))
                            {
                                if (IsKeyNotPresent(dataItem.ValidationName, finalAttributeList[indexValue].ControlProperty.htmlAttributes))
                                    finalAttributeList[indexValue].ControlProperty.htmlAttributes.Add(dataItem.ValidationName, dataItem.SubValidationName);
                            }
                            else
                            {
                                if (IsKeyNotPresent(dataItem.ValidationName, finalAttributeList[indexValue].ControlProperty.htmlAttributes))
                                    finalAttributeList[indexValue].ControlProperty.htmlAttributes.Add(dataItem.ValidationName, dataItem.ValidationValue);
                            }
                        }
                        else if ((attributesList.Select(x => x.ValidationName == ZnodeConstant.Extensions).ToList()).Any(m => m))
                        {
                            if (finalAttributeList[indexValue].ControlProperty.htmlAttributes.ContainsKey(ZnodeConstant.Extensions) == false)
                            {
                                if (IsKeyNotPresent(ZnodeConstant.Extensions, finalAttributeList[indexValue].ControlProperty.htmlAttributes))
                                {
                                    string result = string.Join(",", attributesList.Where(x => x.ValidationName == ZnodeConstant.Extensions).Select(k => k.SubValidationName).ToArray());
                                    finalAttributeList[indexValue].ControlProperty.htmlAttributes.Add(ZnodeConstant.Extensions, result);
                                }
                            }
                        }
                    }
                    indexValue++;
                }
            }

            finalAttributeList.ForEach(
                      x =>
                      {
                          string value = personalizeValues.Where(y => y.Key == x.AttributeCode)?.FirstOrDefault().Value;
                          x.ControlProperty.htmlAttributes?.Add("IsPersonalizable", "True");
                          if (x.AttributeTypeName == ControlTypes.Text.ToString())
                              x.ControlProperty.htmlAttributes?.Add("placeholder", value);
                      });
            return finalAttributeList;
        }

        //This method returns true if key is not present in dictionary else return false.
        public virtual bool IsKeyNotPresent(string key, IDictionary<string, object> source)
        {
            if (HelperUtility.IsNotNull(source) && !string.IsNullOrEmpty(key))
                return !source.ContainsKey(key);
            return false;
        }
    }
}