using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Admin.Extensions;
using Znode.Libraries.ECommerce.Utilities;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Znode.Engine.Admin.Maps
{
    public static class GlobalAttributeModelMap
    {
        // This method maps PIMFamilyDetailsModel to PIMFamilyDetailsViewModel.
        public static GlobalAttributeEntityDetailsViewModel ToGlobalAttributeEntityDetailViewModel(GlobalAttributeEntityDetailsModel model)
        {
            if (!Equals(model, null) && !Equals(model.Attributes, null) && !Equals(model.Groups, null))
            {


                GlobalAttributeEntityDetailsViewModel entityDetailsView = model.ToViewModel<GlobalAttributeEntityDetailsViewModel>();

                List<string> distinctAttributeCodes = entityDetailsView.Attributes.Where(x => x != null).Select(e => e.AttributeCode).Distinct().ToList();
                entityDetailsView.Attributes = GetAttributeControls(entityDetailsView.Attributes, distinctAttributeCodes);


                return entityDetailsView;
            }
            return new GlobalAttributeEntityDetailsViewModel();
        }

        public static FormBuilderAttributeGroupViewModel ToFormBuilderAttributeGroupViewModel(FormBuilderAttributeGroupModel model)
        {
            if (!Equals(model, null) && !Equals(model.Attributes, null) && !Equals(model.Groups, null))
            {
                FormBuilderAttributeGroupViewModel viewModel = model.ToViewModel<FormBuilderAttributeGroupViewModel>();

                List<string> distinctAttributeCodes = viewModel.Attributes.Where(x => x != null).Select(e => e.AttributeCode).Distinct().ToList();
                viewModel.Attributes = GetAttributeControls(viewModel.Attributes, distinctAttributeCodes);
                
                return viewModel;
            }
            return null;
        }

        public static List<GlobalAttributeValuesViewModel> GetAttributeControls(List<GlobalAttributeValuesViewModel> attributeValueList, List<string> distinctAttributeCodes)
        {
            int indexValue = 0;
            List<GlobalAttributeValuesViewModel> finalAttributeList = new List<GlobalAttributeValuesViewModel>();
            if (HelperUtility.IsNotNull(attributeValueList) && HelperUtility.IsNotNull(distinctAttributeCodes))
            {
                finalAttributeList = attributeValueList.Where(x => x != null).GroupBy(x => x.AttributeCode).Select(g => g.First()).ToList();

                foreach (string item in distinctAttributeCodes)
                {
                    List<GlobalAttributeValuesViewModel> attributesList = attributeValueList.Where(x => x != null && x.AttributeCode == item?.ToString()).ToList();

                    //Appended keys with property name {AttributeCode}[0]_{PimAttributeId}[1]_{PimAttributeDefaultValueId}[2]_{PimAttributeValueId}[3]_{PimAttributeFamilyId}[4].
                    string controlName = $"{attributesList[0].AttributeCode}_{attributesList[0].GlobalAttributeId}_{attributesList[0].GlobalAttributeDefaultValueId.GetValueOrDefault()}_{attributesList[0].GlobalAttributeValueId.GetValueOrDefault()}_{attributesList[0].GlobalEntityId}";

                    SetAttributeValueForSimpleSelectMultiMediaUpload(attributesList);

                    finalAttributeList[indexValue].ControlProperty.Id = $"{controlName}_attr";
                    finalAttributeList[indexValue].ControlProperty.ControlType = attributesList[0].AttributeTypeName;
                    finalAttributeList[indexValue].ControlProperty.Name = $"{controlName}_attr";
                    finalAttributeList[indexValue].ControlProperty.ControlLabel = attributesList[0].AttributeName;
                    finalAttributeList[indexValue].ControlProperty.Value = string.IsNullOrEmpty(attributesList[0].AttributeValue) ? attributesList[0].AttributeDefaultValue : attributesList[0].AttributeValue;
                    finalAttributeList[indexValue].ControlProperty.HelpText = attributesList[0].HelpDescription;
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
                        if (IsKeyNotPresent(AdminConstants.IsRequired, finalAttributeList[indexValue].ControlProperty.htmlAttributes))
                            finalAttributeList[indexValue].ControlProperty.htmlAttributes.Add(AdminConstants.IsRequired, attributesList[0].IsRequired);
                    }

                    if (Equals(Regex.Replace(attributesList[0].AttributeTypeName, @"\s", ""), ControlTypes.Label.ToString()))
                        finalAttributeList[indexValue].ControlProperty.Value = attributesList[0].AttributeDefaultValue;

                    foreach (var dataItem in attributesList)
                    {
                        if (!Equals(dataItem.ValidationName, null) && !Equals(dataItem.ValidationName, AdminConstants.Extensions))
                        {
                            if (Equals(dataItem.ControlName, AdminConstants.Select) || Equals(dataItem.ControlName, AdminConstants.MultiSelect) || Equals(dataItem.ControlName, ControlTypes.SimpleSelect.ToString()))
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
                        else if ((attributesList.Select(x => x.ValidationName == AdminConstants.Extensions).ToList()).Any(m => m))
                        {
                            if (finalAttributeList[indexValue].ControlProperty.htmlAttributes.ContainsKey(AdminConstants.Extensions) == false)
                            {
                                if (IsKeyNotPresent(AdminConstants.Extensions, finalAttributeList[indexValue].ControlProperty.htmlAttributes))
                                {
                                    string result = string.Join(",", attributesList.Where(x => x.ValidationName == AdminConstants.Extensions).Select(k => k.SubValidationName).ToArray());
                                    finalAttributeList[indexValue].ControlProperty.htmlAttributes.Add(AdminConstants.Extensions, result);
                                }
                            }
                        }
                    }
                    indexValue++;
                }
            }

            return finalAttributeList;
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
            if (attributesList?.Count > 0 && (Equals(Regex.Replace(attributesList[0].AttributeTypeName, @"\s", ""), ControlTypes.MultiSelect.ToString()) ||
                                    Equals(Regex.Replace(attributesList[0].AttributeTypeName, @"\s", ""), ControlTypes.File.ToString())) ||
                                    Equals(Regex.Replace(attributesList[0].AttributeTypeName, @"\s", ""), ControlTypes.SimpleSelect.ToString()) ||
                                    Equals(Regex.Replace(attributesList[0].AttributeTypeName, @"\s", ""), ControlTypes.Image.ToString()))
            {
                string attributeValue = string.Empty;
                bool IsImage = false;
                if (Equals(Regex.Replace(attributesList[0].AttributeTypeName, @"\s", ""), ControlTypes.File.ToString()) || Equals(Regex.Replace(attributesList[0].AttributeTypeName, @"\s", ""), ControlTypes.Image.ToString()))
                {
                    var files = attributesList.Where(x => x.AttributeValue != null && x.ValidationName == AdminConstants.IsAllowMultiUpload).Select(x => x).ToList();
                    if (files?.Count > 0)
                    {
                        files = files.Where(x => x != null).GroupBy(x => x.AttributeValue).Select(g => g.First()).ToList();
                        attributeValue = string.Join(",", files.Where(x => x.AttributeValue != null).Select(k => k.AttributeValue).ToArray());
                        attributeValue += "~" + string.Join(",", files.Where(x => x.AttributeValue != null).Select(k => k.MediaId).ToArray());
                        IsImage = true;
                    }
                }
                else if (Equals(Regex.Replace(attributesList[0].AttributeTypeName, @"\s", ""), ControlTypes.SimpleSelect.ToString()))
                {
                    attributeValue = string.Join(",", attributesList.Where(x => x.AttributeValue != null).Select(k => k.AttributeValue).ToArray());
                    if (attributeValue == "")
                        attributeValue = string.Join(",", attributesList.Where(x => x.IsDefault == true)?.Select(k => k.AttributeDefaultValueCode)?.ToArray());
                }
                else
                {
                    if (!IsImage)
                    {
                       attributeValue = string.Join(",", attributesList.Where(x => x.AttributeValue != null).Select(k => k.AttributeValue).ToArray());
                    }
                }
                attributesList.ToList().ForEach(c => c.AttributeValue = attributeValue);
            }
        }
    }
}
