using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Znode.Engine.WebStore.ViewModels;

namespace Znode.Engine.WebStore.Maps
{
    public static class GlobalAttributeModelMap
    {
        // This method maps PIMFamilyDetailsModel to PIMFamilyDetailsViewModel.
        public static FormBuilderAttributeGroupViewModel ToGlobalAttributeEntityDetailViewModel(FormBuilderAttributeGroupModel model)
        {
            if (HelperUtility.IsNotNull(model) && HelperUtility.IsNotNull(model.Attributes) && HelperUtility.IsNotNull(model.Groups))
            {
                FormBuilderAttributeGroupViewModel entityDetailsView = model.ToViewModel<FormBuilderAttributeGroupViewModel>();

                List<string> distinctAttributeCodes = entityDetailsView.Attributes.Where(x => x != null).Select(e => e.AttributeCode).Distinct().ToList();
                entityDetailsView.Attributes = GetAttributeControls(entityDetailsView.Attributes, distinctAttributeCodes);

                return entityDetailsView;
            }
            return null;
        }

        public static List<GlobalAttributeValuesViewModel> GetAttributeControls(List<GlobalAttributeValuesViewModel> attributeValueList, List<string> distinctAttributeCodes)
        {
            int indexValue = 0;
            List<GlobalAttributeValuesViewModel> finalAttributeList = new List<GlobalAttributeValuesViewModel>();
            if (HelperUtility.IsNotNull(attributeValueList) && HelperUtility.IsNotNull(distinctAttributeCodes))
            {
                //Get final attribute list
                finalAttributeList = attributeValueList.Where(x => x != null).GroupBy(x => x.AttributeCode).Select(g => g.First()).ToList();

                foreach (string item in distinctAttributeCodes)
                {
                    //Select attribute list.
                    List<GlobalAttributeValuesViewModel> attributesList = attributeValueList.Where(x => x != null && x.AttributeCode == item?.ToString()).ToList();

                    //Check Control name
                    string controlName = $"{attributesList[0].AttributeCode}_{attributesList[0].GlobalAttributeId}_{attributesList[0].GlobalAttributeDefaultValueId.GetValueOrDefault()}_{attributesList[0].GlobalAttributeValueId.GetValueOrDefault()}_{attributesList[0].GlobalEntityId}";
                    if (attributesList[0].SubValidationName == WebStoreConstants.Email)
                    {
                        controlName += $"_{attributesList[0].SubValidationName}";
                    }

                    SetAttributeValueForSimpleSelectMultiMediaUpload(attributesList);

                    finalAttributeList[indexValue].ControlProperty.Id = $"{controlName}_attr";
                    finalAttributeList[indexValue].ControlProperty.ControlType = attributesList[0].AttributeTypeName;
                    finalAttributeList[indexValue].ControlProperty.Name = $"{controlName}_attr";
                    finalAttributeList[indexValue].ControlProperty.ControlLabel = attributesList[0].AttributeName;
                    finalAttributeList[indexValue].ControlProperty.Value = string.IsNullOrEmpty(attributesList[0].AttributeValue) ? attributesList[0].AttributeDefaultValue : attributesList[0].AttributeValue;
                    finalAttributeList[indexValue].ControlProperty.HelpText = attributesList[0].HelpDescription;
                    //Check attribute type name and control type.
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
                    // If attribute is set isRequired
                    if (attributesList[0].IsRequired)
                    {
                        if (IsKeyNotPresent(WebStoreConstants.IsRequired, finalAttributeList[indexValue].ControlProperty.htmlAttributes))
                            finalAttributeList[indexValue].ControlProperty.htmlAttributes.Add(WebStoreConstants.IsRequired, attributesList[0].IsRequired);
                    }
                    // Attribute Type Name is empty
                    if (Equals(Regex.Replace(attributesList[0].AttributeTypeName, @"\s", ""), ControlTypes.Label.ToString()))
                        finalAttributeList[indexValue].ControlProperty.Value = attributesList[0].AttributeDefaultValue;
                    //Foreach for attribute list. 
                    foreach (var dataItem in attributesList)
                    {
                        if (!Equals(dataItem.ValidationName, null) && !Equals(dataItem.ValidationName, WebStoreConstants.Extensions))
                        {
                            //
                            if (Equals(dataItem.ControlName, WebStoreConstants.Select) || Equals(dataItem.ControlName, WebStoreConstants.MultiSelect) || Equals(dataItem.ControlName, ControlTypes.SimpleSelect.ToString()))
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
                        // For validation.
                        else if ((attributesList.Select(x => x.ValidationName == WebStoreConstants.Extensions).ToList()).Any(m => m))
                        {
                            if (finalAttributeList[indexValue].ControlProperty.htmlAttributes.ContainsKey(WebStoreConstants.Extensions) == false)
                            {
                                if (IsKeyNotPresent(WebStoreConstants.Extensions, finalAttributeList[indexValue].ControlProperty.htmlAttributes))
                                {
                                    string result = string.Join(",", attributesList.Where(x => x.ValidationName == WebStoreConstants.Extensions).Select(k => k.SubValidationName).ToArray());
                                    finalAttributeList[indexValue].ControlProperty.htmlAttributes.Add(WebStoreConstants.Extensions, result);
                                }
                            }
                        }
                    }
                    indexValue++;
                }
            }
            return finalAttributeList;
        }

        public static GlobalAttributeEntityDetailsViewModel ToGlobalAttributeEntityDetailViewModel(GlobalAttributeEntityDetailsModel model)
        {
            if (HelperUtility.IsNotNull(model) && HelperUtility.IsNotNull(model.Attributes) && HelperUtility.IsNotNull(model.Groups))
            {
                GlobalAttributeEntityDetailsViewModel entityDetailsView = model.ToViewModel<GlobalAttributeEntityDetailsViewModel>();

                List<string> distinctAttributeCodes = entityDetailsView.Attributes.Where(x => x != null).Select(e => e.AttributeCode).Distinct().ToList();
                entityDetailsView.Attributes = GetAttributeControls(entityDetailsView.Attributes, distinctAttributeCodes);
                return entityDetailsView;
            }
            return null;
        }

        //This method returns true if key is not present in dictionary else return false.
        public static bool IsKeyNotPresent(string key, IDictionary<string, object> source)
        {
            if (!Equals(source, null) && !string.IsNullOrEmpty(key))
                return !source.ContainsKey(key);
            return false;
        }

        //This method set the attribute value for simpleselect multiselect &  media upload like file, image & multi upload .
        public static void SetAttributeValueForSimpleSelectMultiMediaUpload(List<GlobalAttributeValuesViewModel> attributesList)
        {
            //Checking The control type. 
            if (attributesList?.Count > 0 && (Equals(Regex.Replace(attributesList[0].AttributeTypeName, @"\s", ""), ControlTypes.MultiSelect.ToString()) ||
                                    Equals(Regex.Replace(attributesList[0].AttributeTypeName, @"\s", ""), ControlTypes.File.ToString())) ||
                                    Equals(Regex.Replace(attributesList[0].AttributeTypeName, @"\s", ""), ControlTypes.SimpleSelect.ToString()) ||
                                    Equals(Regex.Replace(attributesList[0].AttributeTypeName, @"\s", ""), ControlTypes.Image.ToString()))
            {
                string attributeValue = string.Empty;
                // For attributeName is not null check.
                if (Equals(Regex.Replace(attributesList[0].AttributeTypeName, @"\s", ""), ControlTypes.File.ToString()) || Equals(Regex.Replace(attributesList[0].AttributeTypeName, @"\s", ""), ControlTypes.Image.ToString()))
                {
                    var files = attributesList.Where(x => x.AttributeValue != null && x.ValidationName == WebStoreConstants.IsAllowMultiUpload).Select(x => x).ToList();
                    if (files?.Count > 0)
                    {
                        files = files.Where(x => x != null).GroupBy(x => x.AttributeValue).Select(g => g.First()).ToList();
                        attributeValue = string.Join(",", files.Where(x => x.AttributeValue != null).Select(k => k.AttributeValue).ToArray());
                        attributeValue += "~" + string.Join(",", files.Where(x => x.AttributeValue != null).Select(k => k.MediaId).ToArray());
                    }
                }
                else
                    attributeValue = string.Join(",", attributesList.Where(x => x.AttributeValue != null).Select(k => k.AttributeValue).ToArray());

                attributesList.ToList().ForEach(c => c.AttributeValue = attributeValue);
            }
        }
    }
}
