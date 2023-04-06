using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Admin.Core.Helpers;
using Znode.Libraries.Resources;
using System.Net;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using Znode.Libraries.Framework.Business;
using System.Diagnostics;

namespace Znode.Engine.Admin
{
    public static class CreateControlHelper
    {

        public static string  AttributeCode { get; set; }
        public static MvcHtmlString CreateControl(this HtmlHelper html, Property properties, IDictionary<string, object> htmlAttributes = null)
        {
            if (html?.ViewData?.Model is ViewModels.PIMProductAttributeValuesViewModel)
                AttributeCode = ((HtmlHelper<ViewModels.PIMProductAttributeValuesViewModel>)html)?.ViewData?.Model?.AttributeCode;

            string atributeStr = string.Empty;
            if (!Equals(htmlAttributes, null))
            {
                foreach (var attr in htmlAttributes)
                {
                    atributeStr += $" {attr.Key} ='{attr.Value}'";
                }
            }
            return MvcHtmlString.Create(CreateControlHtml(properties, atributeStr));
        }

        public static MvcHtmlString CreateControl(this HtmlHelper html, Property properties)
        { 
            if(html?.ViewData?.Model is ViewModels.PIMProductAttributeValuesViewModel)
            AttributeCode = ((HtmlHelper<ViewModels.PIMProductAttributeValuesViewModel>)html)?.ViewData?.Model?.AttributeCode;
            
            string atributeStr = string.Empty;
            if (!Equals(properties.htmlAttributes, null))
            {
                atributeStr += "data-val='true'";
                foreach (var attr in properties.htmlAttributes)
                    atributeStr += $" {attr.Key} ='{CheckValueIsDate(attr.Value, properties.ControlType)}'";

            }
            return MvcHtmlString.Create(CreateControlHtml(properties, atributeStr) + CreateErrorSpanHtml(properties));
        }

        #region Private Methods For Get Html String
              
        private static string CheckValueIsDate(object value, string controlType)
        {
            if (controlType == "Date" && !Equals(value, null))
            {
                DateTime dt;
                DateTime.TryParse(Convert.ToString(value), out dt);
                if (!Equals(dt, null) && dt != DateTime.MinValue)
                    return dt.ToString(DefaultSettingHelper.DefaultDateFormat);
            }
            return WebUtility.HtmlEncode(Convert.ToString(value));
        }
        private static string GetTextAreaControlHtml(Property properties, string attribute)
        {
            string ControlHtml;
            StringBuilder html = new StringBuilder();

            if (properties.Name.StartsWith("mceEditor"))
            {
                if (properties.IsDetailView)
                {
                    ControlHtml = $"<div class='control-md'><textarea rows='4' cols='50' id='{properties.Id }' data-test-selector='txt{AttributeCode}'  name='{properties.Name}' {attribute}>{properties.Value}</textarea></div>";
                    html.Append(ControlHtml);
                    html.Append("<a href='javascript:;' class='toggleWYSIWYG' data-test-selector='link"+AttributeCode+"' style='display: none' onclick='tinymce.execCommand(\"mceToggleEditor\", false, \"" + properties.Id + "\"); '><span>Edit</span></a>");
                    ControlHtml = html.ToString();
                }
                else
                {
                    ControlHtml = $"<div class='control-md'><textarea rows='4' cols='50' id='{properties.Id }' class='mceEditor' data-test-selector='txt{AttributeCode}' name='{properties.Name}' {attribute}>{properties.Value}</textarea></div>";
                }
            }
            else
            {
                ControlHtml = $"<div class='control-md'><textarea rows='4' cols='50' id='{properties.Id }' data-test-selector='txt{AttributeCode}'  name='{properties.Name}' {attribute}>{properties.Value}</textarea></div>";
            }

            return ControlHtml;
        }

        private static string GetVideoControlHtml(Property properties)
        {
            return $"<video width='160' height='90' class='upload-video' controls>" +
                          "<source src = '" + properties.Value + "' type = 'video/mp4'></video>" +
                          "<button type='button' data-test-selector='btnUploadVideo' class='btn-text-icon blue upload-btn' style='cursor:default' onclick='EditableText.prototype.BrowseMedia(\"" + properties.Id + "\", \"" + false + "\", \"" + false + "\", \"" + true + "\")'><i class='z-upload'></i>Choose a Media...</button>" +
                          "<div class='appendMediaModel'> </div>";
        }

        private static string GetFileControlHtml(Property properties, string attribute)
        {
            if (properties.htmlAttributes.ContainsKey("IsPimFileControl") || properties.htmlAttributes.ContainsKey("IsGlobalAttributeFileControl"))
            {
                object extensions, isRequired, pointerEvent, isAllowMultiUpload;
                properties.htmlAttributes.TryGetValue("Extensions", out extensions);
                properties.htmlAttributes.TryGetValue("IsRequired", out isRequired);
                properties.htmlAttributes.TryGetValue("pointer-events", out pointerEvent);
                properties.htmlAttributes.TryGetValue("IsAllowMultiUpload", out isAllowMultiUpload);
                pointerEvent = pointerEvent == null ? "All" : pointerEvent;
                string fileSize = GetMediaImageSize(properties);
                isRequired = string.IsNullOrEmpty(Convert.ToString(isRequired)) ? null : "isrequired = '" + isRequired + "'";
                if (Convert.ToString(isAllowMultiUpload) == "false")
                {
                    string fileMediaValueIds = SetImageValue(properties);
                    string mediaName = properties.Value?.Substring(properties.Value.LastIndexOf('/') + 1)?.Split('~')[0];
                    string htmlString = GetMediaFileTypeViewByFamilyType(string.IsNullOrEmpty(fileMediaValueIds) ? "" : GetMediaFamily(Convert.ToInt32((string.IsNullOrEmpty(fileMediaValueIds) ? "0" : fileMediaValueIds))));

                    return $"<div class='file-help-text' style='display:unset;pointer-events: " + pointerEvent + "' id='div" + properties.Id + "' >" +
                    "<ul><button type='button' style='margin-top:10px;' class='btn-text-icon btn-text-color-icon blue btn-margin-left' data-test-selector='btnUploadImage' id='UploadImage'  cursor:default' onclick='EditableText.prototype.BrowseMedia(\"" + properties.Id + "\", \"" + false + "\", \"" + false + "\", \"" + true + "\")'><i class='z-upload'></i>Browse</button></ul>" +
                    "<input type='hidden' value=" + (Equals(extensions, null) ? "" : extensions.ToString()) + " Id='hdn" + properties.Id + "'/>" +
                     "<input type='hidden' value='" + fileSize + "' Id='hdnMediaSize" + properties.Id + "'/>" +
                      "<input " + isRequired + "  type='text' style='display: none'  id=" + properties.Id + " name=" + properties.Id + " value=" + fileMediaValueIds + ">" +
                      "<span style='margin-left:175px;width:85%;' class='error-msg' id='fileerrormsg'> </span>" +
                   "<div id='Upload" + properties.Id + "' class='fileuploader control-md control-non' imagevalidator='true'>" +
                    (string.IsNullOrEmpty(fileMediaValueIds) ? "" : "<div id='" + properties.Id + "'><div class='upload-video dirtyignore'>" + htmlString + " <a class='upload-images-close' data-toggle='tooltip' data-placement='top' title='Remove' onclick='EditableText.prototype.RemoveMedia(\"" + properties.Id + "\")'><i class='z-close-circle'></i></a></div>   <span> " + (string.IsNullOrEmpty(htmlString) ? null : properties.FilesName) + " </span></div>") +
                   "</div></div>";
                }
                else
                    return GetMultiUploadControlHtml(properties, fileSize, false);
            }
            else
            {
                return $"<div class='file-upload' id='div" + properties.Id + "'>" +
                                "<span title = 'Upload' class='btn-text-icon blue file-upload-btn'><i class='z-upload'></i>Browse" +
                                "<input type=" + properties.ControlType?.ToLower() + " value=" + properties.Value + " id=" + properties.Id + " name=" + properties.Name + " " + attribute + "/></span>" +
                            "</div>";
            }
        }

        private static string GetNumberControlHtml(Property properties, string attribute) =>
         $"<div class='control-md'><input type='text' data-test-selector='txt{AttributeCode}' value='{properties.Value}' id='{properties.Id }' class='{properties.CSSClass}' name='{properties.Name}' {attribute}/></div>";

        private static string GetImageControlHtml(Property properties)
        {
            string Controlstring = string.Empty;
            bool isProductImage = (string.IsNullOrEmpty(properties.Value) ? true : (properties.Value.Equals(ZnodeAdminSettings.DefaultImagePath + "~") ? true : false));
            if (string.IsNullOrEmpty(properties.Value))
                properties.Value = ZnodeAdminSettings.DefaultImagePath;

            string maxFileSizeInGlobleUnit = GetMediaImageSize(properties);

            if (properties.htmlAttributes.ContainsKey("IsAllowMultiUpload"))
            {
                object IsAllowMultiUpload;
                properties.htmlAttributes.TryGetValue("IsAllowMultiUpload", out IsAllowMultiUpload);


                if (IsAllowMultiUpload?.ToString().ToLower() == "true")
                {
                    properties.Value = isProductImage ? "" : properties.Value;
                    Controlstring = GetMultiUploadControlHtml(properties, maxFileSizeInGlobleUnit, true);
                }
                else
                {
                    Controlstring = GetSimpleImageControlHtml(properties, isProductImage, maxFileSizeInGlobleUnit);
                }
            }
            else
            {
                Controlstring = GetSimpleImageControlHtml(properties, isProductImage, maxFileSizeInGlobleUnit);
            }

            return Controlstring;
        }

        private static string GetMediaImageSize(Property properties)
        {
            string maxFileSizeInGlobalUnit = "0";
            try
            {       
                object maxFileSize;
                ZnodeLogging.LogMessage("Method:GetMediaImageSize", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                properties.htmlAttributes.TryGetValue("MaxFileSize", out maxFileSize);

                double fileSize;
                int maxSize = double.TryParse(maxFileSize?.ToString(), out fileSize) ? (int)fileSize : 0; ;
                maxFileSizeInGlobalUnit = Convert.ToString(Math.Round(maxSize * 1024000M, 2))?.ToDisplayUnitFormat();
                maxFileSizeInGlobalUnit = string.IsNullOrEmpty(maxFileSizeInGlobalUnit) ? string.Empty : maxFileSizeInGlobalUnit.Split(' ')[0];
                
                ZnodeLogging.LogMessage("Execution Done", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                return maxFileSizeInGlobalUnit;

            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                return maxFileSizeInGlobalUnit;
            }                      
        }
        
        private static string GetSimpleImageControlHtml(Property properties, bool isProductImage, string fileSize)
        {
            string ImageValue = SetImageValue(properties);
            string removeIconStr = isProductImage ? string.Empty : "<a class=\"upload-images-close dirtyignore\" data-toggle='tooltip' data-placement='top' title='Remove' onclick='EditableText.prototype.RemoveImage(\"" + properties.Id + "\")'><i class='z-close-circle'></i></a>";
            object extensions, pointerEvent;

            properties.htmlAttributes.TryGetValue("Extensions", out extensions);
            properties.htmlAttributes.TryGetValue("pointer-events", out pointerEvent);
            pointerEvent = pointerEvent == null ? "All" : pointerEvent;
            removeIconStr = Convert.ToString(pointerEvent) == "All" ? removeIconStr : string.Empty;

            return $"<div class='control-md'><div class=\"upload-images\"  id='div" + properties.Id + "'>" +
                            "<img id=" + properties.Id + " data-test-selector=img"+ AttributeCode + "   onclick='EditableText.prototype.BrowseMedia(\"" + properties.Id + "\", \"" + false + "\", \"" + true + "\", \"" + true + "\")'  src='" + (string.IsNullOrEmpty(properties.Value) ? "" : properties.Value) + "' class='img-responsive dev-upload-single' style='pointer-events: " + pointerEvent + "'/>" +
                            "<span onclick='EditableText.prototype.BrowseMedia(\"" + properties.Id + "\", \"" + false + "\", \"" + true + "\", \"" + true + "\")' class='change-image dev-upload-single'  style='pointer-events: " + pointerEvent + "'>Change</span>" +
                            "<input type='hidden' id=" + properties.Id + " name=" + properties.Id + " value=" + ImageValue + ">" +
                              removeIconStr +
                            "<input type='hidden' value=" + (Equals(extensions, null) ? "" : extensions.ToString()) + " Id='hdn" + properties.Id + "'/>" +
                            "<input type='hidden' value='" + fileSize + "' Id='hdnMediaSize" + properties.Id + "'/>" +
                             "</div>" +

                            "<div id='Upload" + properties.Id + "' class='appendMediaModel'> </div></div>";
        }

        private static string GetMultiUploadControlHtml(Property properties, string fileSize, bool IsImageTag)
        {
            string MultiUploadValues = SetImageValue(properties);
            object extensions, pointerEvent;
            properties.htmlAttributes.TryGetValue("Extensions", out extensions);
            properties.htmlAttributes.TryGetValue("pointer-events", out pointerEvent);
            pointerEvent = pointerEvent == null ? "All" : pointerEvent;
            string classname = IsImageTag == true ? "Image" : "Files";

            if(!IsImageTag)
            {
                return $"<div class='control-lg'><div class='multi-upload-" + classname + "' id='div" + properties.Id + "'>" +
                   "<div class='multi-upload'><h3>" + Admin_Resources.UploadFiles + "</h3><button type='button' data-test-selector='btn"+ AttributeCode + "' class='btn-text dev-upload-multiple' style='pointer-events: " + pointerEvent + "' id ='UploadMultiple' class='multi-upload' onclick='EditableText.prototype.BrowseMedia(\"" + properties.Id + "\", \"" + true + "\", \"" + IsImageTag + "\", \"" + true + "\")'>Multiple Upload </button></div>" +
                   "<input type='hidden' id=" + properties.Id + " src='" + properties.Value + "' name=" + properties.Id + " value=" + MultiUploadValues + ">" +
                     "<input type='hidden' value=" + (Equals(extensions, null) ? "" : extensions.ToString()) + " Id='hdn" + properties.Id + "'/>" +
                     "<input type='hidden' value='" + fileSize + "' Id='hdnMediaSize" + properties.Id + "'/>" +
                     "<input type='hidden' value='" + properties.FilesName + "' Id='hdnFileNameValue" + properties.Id + "'/>"
                     + "</div>" +
                   "<div id='Upload" + properties.Id + "' class='appendMediaModel' style='pointer-events: " + pointerEvent + "'> </div></div>";
            }
            else
            {
                string imageHTML = "";
                if(properties?.Value != null && properties?.Value != "")
                {
                    int count = 0;
                    foreach(string imagePath in properties.Value.Split(','))
                    {
                        imageHTML = imageHTML + "<div class='upload-images multi-upload-images dirtyignore'><img id=" + properties.Id + count + " src='" + imagePath + "' class='img-responsive'><a class='upload-images-close' data-toggle='tooltip' data-placement='top' title='Remove' onclick='EditableText.prototype.RemoveMultipleImage(&quot;" + properties.Id + count + "&quot;)'><i class='z-close-circle'></i></a><input type='hidden' id=" + properties.Id + count + " value='0'></div>";
                        count++;
                    }
                }
                
                return $"<div class='control-lg'><div class='multi-upload-" + classname + "' id='div" + properties.Id + "'>" +
                   "<div class='multi-upload'><h3>" + Admin_Resources.UploadFiles + "</h3><button type='button' data-test-selector='btn"+ AttributeCode + "' class='btn-text dev-upload-multiple' style='pointer-events: " + pointerEvent + "' id ='UploadMultiple' class='multi-upload' onclick='EditableText.prototype.BrowseMedia(\"" + properties.Id + "\", \"" + true + "\", \"" + IsImageTag + "\", \"" + true + "\")'>Multiple Upload </button></div>" +
                   "<input type='hidden' id=" + properties.Id + " src='" + properties.Value + "' name=" + properties.Id + " value=" + MultiUploadValues + ">" +
                     "<input type='hidden' value=" + (Equals(extensions, null) ? "" : extensions.ToString()) + " Id='hdn" + properties.Id + "'/>" +
                     "<input type='hidden' value='" + fileSize + "' Id='hdnMediaSize" + properties.Id + "'/>" +
                     "<input type='hidden' value='" + properties.FilesName + "' Id='hdnFileNameValue" + properties.Id + "'/>" +
                     imageHTML
                     + "</div>" +
                   "<div id='Upload" + properties.Id + "' class='appendMediaModel' style='pointer-events: " + pointerEvent + "'> </div></div>";
            }
            
        }

        public static string SetImageValue(Property properties)
        {
            string ImageId = string.Empty;
            if (!string.IsNullOrEmpty(properties.Value))
            {
                var splitValue = properties.Value.Split('~');
                if (splitValue.Length > 1)
                {
                    properties.Value = splitValue[0];
                    ImageId = splitValue[1];
                }
            }
            return Regex.Replace(ImageId, @"\s", "");
        }

        private static string GetYesNoControlHtml(Property properties)
        {
            string isYesNoValue = properties.Value;
            if (isYesNoValue?.ToLower() == "true")
                return GetHtmlForYes(properties);
            else
                return GetHtmlForNo(properties);
        }

        private static string GetHtmlForNo(Property properties)
        {
            object pointerEvent;
            properties.htmlAttributes.TryGetValue("pointer-events", out pointerEvent);
            return $"<div class='switch-field control-yes-no' style='pointer-events: " + pointerEvent + "'>" +
                        "<input type='radio' class='yes' id=" + (properties.Id + 1) + " data-test-selector=chkYes" + AttributeCode + " name =" + properties.Name + " value='true'/> <label onclick='EditableText.prototype.labelClick(\"yes\",\"" + (properties.Id + 1) + "\")'> Yes </label>" +
                        "<input type='radio' class='no' id=" + (properties.Id + 2) + " data-test-selector=chkNo" + AttributeCode + " name=" + properties.Name + " value ='false' checked/> <label onclick='EditableText.prototype.labelClick(\"no\",\"" + (properties.Id + 2) + "\")'> No </label>" +
                        "</div>";
        }

        private static string GetHtmlForYes(Property properties)
        {
            object pointerEvent;
            properties.htmlAttributes.TryGetValue("pointer-events", out pointerEvent);
            return $"<div class='switch-field control-yes-no' style='pointer-events: " + pointerEvent + "'>" +
                       "<input type='radio' class='yes' data-test-selector=chkYes"+ AttributeCode + " id="+ (properties.Id + 1) + " name=" + properties.Name + " value='true' checked/> <label onclick='EditableText.prototype.labelClick(\"yes\",\"" + (properties.Id + 1) + "\")'> Yes </label>" +
                       "<input type='radio' class='no' data-test-selector=chkNo"+ AttributeCode + " id=" + (properties.Id + 2) + " name=" + properties.Name + " value ='false'/> <label onclick='EditableText.prototype.labelClick(\"no\",\"" + (properties.Id + 2) + "\")'> No </label>" +
                       "</div>";
        }

        private static string GetLabelControlHtml(Property properties, string attribute) =>
        $"<div class='control-md'> <input type='text'  id= '{properties.Id }'  {attribute}  name='{properties.Name}' data-test-selector='lbl{AttributeCode}' value='{properties.Value}' readonly /></div>";

        private static string GetDateControlHtml(Property properties, string attribute)
        {
            object datepicker;
            properties.htmlAttributes.TryGetValue("readonly", out datepicker);
            datepicker = !Equals(datepicker, null) ? string.Empty : "datepicker";

            return $"<div class='control-sm right-inner-icon'><input type='text' id='{properties.Id }' data-test-selector='txt{AttributeCode}' value='{properties.Value.ToDateTimeFormat()}' class='{datepicker} {properties.CSSClass}'  data-date-format ='{HelperMethods.GetDateFormat()}' name='{properties.Name}' {attribute}/><i class='z-calendar'></i>" +
                 "<span class='text-danger field-validation-error' id='spamDate'> </span></div>";
        }

        private static string GetMultiSelectControlHtml(Property properties, string attribute) =>
       $"<div class='control-md'><select multiple  id='{properties.Id }' data-test-selector='drp{AttributeCode}' class='MultiSelectClass multiselect-dropdown {properties.CSSClass}' name='{properties.Name}' {attribute}>{SelectOptions(properties.SelectOptions, properties.Value?.Split(','))}</select></div>";

        private static string GetSelectControlHtml(Property properties, string attribute) =>
        $"<div class='control-md'><select id='{properties.Id }' data-test-selector='drp{AttributeCode}' class='{properties.CSSClass}' name='{properties.Name}' {attribute}>{SelectOptions(properties.SelectOptions, properties.Value)}</select></div>";

        #endregion

        #region Private Methods



        private static string CreateControlHtml(Property properties, string attribute)
        {
            string ControlHtml = string.Empty;
            switch (Regex.Replace(properties.ControlType, @"\s", ""))
            {
                case "SimpleSelect":
                case "Select":
                    ControlHtml = GetSelectControlHtml(properties, attribute);
                    break;
                case "MultiSelect":
                    ControlHtml = GetMultiSelectControlHtml(properties, attribute);
                    break;
                case "Date":
                    ControlHtml = GetDateControlHtml(properties, attribute);
                    break;
                case "Label":
                    ControlHtml = GetLabelControlHtml(properties, attribute);
                    break;
                case "TextArea":
                    ControlHtml = GetTextAreaControlHtml(properties, attribute);
                    break;
                case "Yes/No":
                    ControlHtml = GetYesNoControlHtml(properties);
                    break;
                case "Image":
                    ControlHtml = GetImageControlHtml(properties);
                    break;
                case "Number":
                    ControlHtml = GetNumberControlHtml(properties, attribute);
                    break;
                case "File":
                    ControlHtml = GetFileControlHtml(properties, attribute);
                    break;
                case "Video":
                    ControlHtml = GetVideoControlHtml(properties);
                    break;
                default:
                    ControlHtml = $"<div class='control-md'><input type='{properties.ControlType?.ToLower()}' data-test-selector='txt{AttributeCode}' data-unique='{properties.htmlAttributes?.FirstOrDefault(x => x.Key == "UniqueValue").Value}' value='{System.Net.WebUtility.HtmlEncode(properties.Value)}' id='{properties.Id }' class='{properties.CSSClass}' name='{properties.Name}' {attribute}/></div>";
                    break;
            }
            ControlHtml += string.IsNullOrEmpty(properties.HelpText) ? string.Empty : CreateHelpDescriptionHtml(properties);
            return ControlHtml;
        }


        private static string CreateErrorSpanHtml(Property properties)
        {
            return $"<div class='dynamic-msg validation-float'><span class='error-msg' data-valmsg-for='{properties.Name}' data-valmsg-replace='true' id='errorSpam{properties.Name}'></span></div>";
        }

        private static string SelectOptions(List<SelectListItem> options, string value)
        {
            string _options = $"<option ></option>";

            if (!Equals(options, null))
                options.ForEach(x => { _options += Equals(x.Value, value) ? $"<option selected='selected' value ='{x.Value?.Replace("'", "&#39;")}'>{x.Text}</option>" : $"<option value ='{x.Value?.Replace("'", "&#39;")}'>{x.Text}</option>"; });

            return _options;
        }

        private static string SelectOptions(List<SelectListItem> options, string[] values)
        {
            if (Equals(values, null))
                values = new string[] { string.Empty };

            string _options = string.Empty;
            if (!Equals(options, null))
                options.ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x.Value))
                        _options += Array.IndexOf(values, x.Value) >= 0 ? $"<option selected='selected' value ='{x.Value?.Replace("'", "&#39;")}'>{x.Text}</option>" : $"<option value ='{x.Value?.Replace("'", "&#39;")}'>{x.Text}</option>";
                });

            return _options;
        }

        private static string CreateHelpDescriptionHtml(Property properties)
        => $"<div class='help-text'><i class='z-help-circle' data-toggle='tooltip' data-placement='right' title='" + WebUtility.HtmlEncode(properties.HelpText) + "'></i> </div>";

        private static string GetMediaFamily(int mediaId)
          => GetService<IDependencyHelper>().GetMediaAttributeValues(mediaId)?.FamilyCode;

        private static string GetMediaFileTypeViewByFamilyType(string mediaFamily)
        {
            string mediaIcon = string.Empty;
            switch (mediaFamily?.ToLower())
            {
                case DynamicGridConstants.Video:
                    mediaIcon = "<i class='z-video'> </i>";
                    break;
                case DynamicGridConstants.Audio:
                    mediaIcon = "<i class='z-audio'> </i>";
                    break;
                case DynamicGridConstants.File:
                    mediaIcon = "<i class='z-file-text'> </i>";
                    break;
            }
            return mediaIcon;
        }
        #endregion
    }

    public class Property
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string CSSClass { get; set; }
        public string ControlType { get; set; }
        public string Value { get; set; }
        public string[] Values { get; set; }
        public string ControlLabel { get; set; }
        public bool IsDetailView { get; set; }
        public Dictionary<string, object> htmlAttributes { get; set; } = null;
        public List<SelectListItem> SelectOptions { get; set; } = null;
        public string HelpText { get; set; }
        public string FilesName { get; set; }
        public Property()
        {
            htmlAttributes = new Dictionary<string, object>();
        }

    }

    public enum ControlTypes
    {
        Date,
        Number,
        MultiSelect,
        Select,
        Text,
        TextArea,
        Label,
        YesNo,
        Image,
        SimpleSelect,
        File,
        Video,
        Audio
    }
}