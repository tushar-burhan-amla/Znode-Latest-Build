using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Znode.Engine.WebStore.Helpers;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.WebStore
{
    public static class CreateControlHelper
    {
        public static MvcHtmlString CreateControl(this HtmlHelper html, Property properties, IDictionary<string, object> htmlAttributes = null)
        {
            string atributeStr = string.Empty;
            if (HelperUtility.IsNotNull(htmlAttributes))
            {
                foreach (var attr in htmlAttributes)
                    atributeStr += $" {attr.Key} ='{attr.Value}'";
            }
            return MvcHtmlString.Create(CreateControlHtml(properties, atributeStr));
        }

        public static MvcHtmlString CreateControl(this HtmlHelper html, Property properties)
        {
            string atributeStr = string.Empty;
            if (HelperUtility.IsNotNull(properties.htmlAttributes))
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
            return Convert.ToString(value);
        }

        private static string GetTextAreaControlHtml(Property properties, string attribute)
        {
            string ControlHtml;
            StringBuilder html = new StringBuilder();

            if (properties.Name.StartsWith("mceEditor"))
            {
                if (properties.IsDetailView)
                {
                    ControlHtml = $"<div class='control-md textarea-control'><textarea rows='4' cols='50' id='{properties.Id }'  name='{properties.Name}' {attribute}>{properties.Value}</textarea></div>";
                    html.Append(ControlHtml);
                    html.Append("<a href='javascript:;' class='toggleWYSIWYG' style='display: none' onclick='tinymce.execCommand(\"mceToggleEditor\", false, \"" + properties.Id + "\"); '><span>Edit</span></a>");
                    ControlHtml = html.ToString();
                }
                else
                {
                    ControlHtml = $"<div class='control-md textarea-control'><textarea rows='4' cols='50' id='{properties.Id }' class='mceEditor' name='{properties.Name}' {attribute}>{properties.Value}</textarea></div>";
                }
            }
            else
            {
                ControlHtml = $"<div class='control-md textarea-control'><textarea rows='4' cols='50' id='{properties.Id }'  name='{properties.Name}' {attribute}>{properties.Value}</textarea></div>";
            }

            return ControlHtml;
        }

        private static string GetVideoControlHtml(Property properties)
        {
            return $"<video width='160' height='90' class='upload-video' controls>" +
                          "<source src = '" + properties.Value + "' type = 'video/mp4'></video>" +
                          "<button type='button' class='btn-text-icon blue upload-btn' style='cursor:default' onclick='EditableText.prototype.BrowseMedia(\"" + properties.Id + "\", \"" + false + "\", \"" + false + "\", \"" + true + "\")'><i class='z-upload'></i>Choose a Media...</button>" +
                          "<div class='appendMediaModel'> </div>";
        }

        private static string GetNumberControlHtml(Property properties, string attribute) =>
         $"<div class='control-md input-control'><input type='text' value='{properties.Value}' id='{properties.Id }' class='{properties.CSSClass}' name='{properties.Name}' {attribute}/></div>";

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
                    Controlstring = GetMultiUploadImageControlHtml(properties, maxFileSizeInGlobleUnit);
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
            object maxFileSize;
            properties.htmlAttributes.TryGetValue("MaxFileSize", out maxFileSize);
            string maxFileSizeInGlobleUnit = string.IsNullOrEmpty(Convert.ToString(maxFileSize)) ? string.Empty : Convert.ToString(Math.Round(Convert.ToInt64(Convert.ToString(maxFileSize)) * 1024000M, 2)).ToDisplayUnitFormat();
            maxFileSizeInGlobleUnit = string.IsNullOrEmpty(maxFileSizeInGlobleUnit) ? string.Empty : maxFileSizeInGlobleUnit.Split(' ')[0];
            return maxFileSizeInGlobleUnit;
        }

        private static string GetSimpleImageControlHtml(Property properties, bool isProductImage, string fileSize)
        {
            string ImageValue = SetImageValue(properties);
            string removeIconStr = isProductImage ? string.Empty : "<a class=\"upload-images-close\" data-toggle='tooltip' data-placement='top' title='Remove' onclick='EditableText.prototype.RemoveImage(\"" + properties.Id + "\")'><i class='z-close-circle'></i></a>";
            object extensions, pointerEvent;

            properties.htmlAttributes.TryGetValue("Extensions", out extensions);
            properties.htmlAttributes.TryGetValue("pointer-events", out pointerEvent);
            pointerEvent = pointerEvent == null ? "All" : pointerEvent;
            removeIconStr = Convert.ToString(pointerEvent) == "All" ? removeIconStr : string.Empty;

            return $"<div class='control-md'><div class=\"upload-images\"  id='div" + properties.Id + "'>" +
                            "<img id=" + properties.Id + " onclick='EditableText.prototype.BrowseMedia(\"" + properties.Id + "\", \"" + false + "\", \"" + true + "\", \"" + true + "\")'  src='" + (string.IsNullOrEmpty(properties.Value) ? "" : properties.Value) + "' class='img-responsive' style='pointer-events: " + pointerEvent + "''/>" +
                            "<input type='hidden' id=" + properties.Id + " name=" + properties.Id + " value=" + ImageValue + ">" +
                              removeIconStr +
                            "<input type='hidden' value=" + (Equals(extensions, null) ? "" : extensions.ToString()) + " Id='hdn" + properties.Id + "'/>" +
                            "<input type='hidden' value='" + fileSize + "' Id='hdnMediaSize" + properties.Id + "'/>" +
                             "</div>" +

                            "<div id='Upload" + properties.Id + "' class='appendMediaModel'> </div></div>";
        }

        private static string GetMultiUploadImageControlHtml(Property properties, string fileSize)
        {
            string MultiImageValue = SetImageValue(properties);
            object extensions, pointerEvent;
            properties.htmlAttributes.TryGetValue("Extensions", out extensions);
            properties.htmlAttributes.TryGetValue("pointer-events", out pointerEvent);
            pointerEvent = pointerEvent == null ? "All" : pointerEvent;

            return $"<div class='control-lg'><div class='multi-upload-Image' id='div" + properties.Id + "'>" +
                    "<div class='multi-upload'><h3>Upload Multiple Gallery Images</h3><button type='button' class='btn-text' style='pointer-events: " + pointerEvent + "' id ='UploadMultiple' class='multi-upload' onclick='EditableText.prototype.BrowseMedia(\"" + properties.Id + "\", \"" + true + "\", \"" + true + "\", \"" + true + "\")'>Multiple Upload </button></div>" +
                    "<img id=" + properties.Id + " src='" + properties.Value + "' style='display:none' class='img-responsive' />" +
                    "<input type='hidden' id=" + properties.Id + " name=" + properties.Id + " value=" + MultiImageValue + ">" +
                      "<input type='hidden' value=" + (Equals(extensions, null) ? "" : extensions.ToString()) + " Id='hdn" + properties.Id + "'/>" +
                      "<input type='hidden' value='" + fileSize + "' Id='hdnMediaSize" + properties.Id + "'/>"
                      + "</div>" +
                    "<div id='Upload" + properties.Id + "' class='appendMediaModel'> </div></div>";
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
            if (properties.Value == "true")
                return GetHtmlForYes(properties);
            else
                return GetHtmlForNo(properties);
        }

        private static string GetHtmlForNo(Property properties)
        {
            return $"<div class='switch-field control-yes-no'>" +
                        "<input type='radio' class='yes' id=" + properties.Id + " name=" + properties.Name + " value='true'/> <label onclick='FormBuilder.prototype.labelClick(\"yes\",\"" + properties.Id + "\")'> Yes </label>" +
                        "<input type='radio' class='no' id=" + properties.Id + " name=" + properties.Name + " value ='false' checked/> <label onclick='FormBuilder.prototype.labelClick(\"no\",\"" + properties.Id + "\")'> No </label>" +
                        "</div>";
        }

        private static string GetHtmlForYes(Property properties)
        {
            return $"<div class='switch-field control-yes-no'>" +
                       "<input type='radio' class='yes' id=" + properties.Id + " name=" + properties.Name + " value='true' checked/> <label onclick='FormBuilder.prototype.labelClick(\"yes\",\"" + properties.Id + "\")'> Yes </label>" +
                       "<input type='radio' class='no' id=" + properties.Id + " name=" + properties.Name + " value ='false'/> <label onclick='FormBuilder.prototype.labelClick(\"no\",\"" + properties.Id + "\")'> No </label>" +
                       "</div>";
        }

        private static string GetLabelControlHtml(Property properties, string attribute) =>
        $"<div class='control-md input-control'> <input type='text'  id= '{properties.Id }'  {attribute}  name='{properties.Name}' value='{properties.Value}' readonly /></div>";

        private static string GetDateControlHtml(Property properties, string attribute)
        {
            object datepicker;
            properties.htmlAttributes.TryGetValue("readonly", out datepicker);
            datepicker = !Equals(datepicker, null) ? string.Empty : "datepicker";
            string dateFormat = HelperMethods.GetDateFormat();
            return $"<div class='control-sm right-inner-icon datepicker-control'><input type='text' id='{properties.Id }' value='{properties.Value.ToDateTimeFormat()}' class='{datepicker} {properties.CSSClass}'  data-date-format ='{dateFormat}' name='{properties.Name}' {attribute}/><i class='z-calendar'></i>" +
                 "<span class='text-danger field-validation-error' id='spamDate'> </span></div>";
        }

        private static string GetMultiSelectControlHtml(Property properties, string attribute) =>
       $"<div class='control-md multiselect-control'><select multiple  id='{properties.Id }' class='MultiSelectClass multiselect-dropdown {properties.CSSClass}' name='{properties.Name}' {attribute}>{SelectOptions(properties.SelectOptions, properties.Value?.Split(','))}</select></div>";

        private static string GetSelectControlHtml(Property properties, string attribute) =>
        $"<div class='control-md select-control'><select id='{properties.Id }' class='{properties.CSSClass}' name='{properties.Name}' {attribute}>{SelectOptions(properties.SelectOptions, properties.Value)}</select></div>";

        #endregion

        #region Private Methods
        private static string GetFileControlHtml(Property properties)
        => $"<div class='file-upload'><div class='file-browse-btn'><span class='btn-text file-upload-btn' title='Upload'>Browse</span><input onChange='FormBuilder.prototype.ValidateDocument(this);' type='{properties.ControlType?.ToLower()}' data-unique='{properties.htmlAttributes?.FirstOrDefault(x => x.Key == "UniqueValue").Value}' data-val='{properties.htmlAttributes?.FirstOrDefault(x => x.Key == "Extensions").Value}' value='{System.Net.WebUtility.HtmlEncode(properties.Value)}' id='p{properties.Id }' class='{properties.CSSClass}' name='{properties.Name}'/></div></div><div class='file-upload-name'><span id=error_p{properties.Id } class='error-msg' style='display: block;'></span><span id=FileName_p{properties.Id }></span><input class='valid' name={ properties.Id } id={ properties.Id } value='' type='hidden' /></div>";

        private static string GetMultipleImageControlHtml(Property properties)
        => $"<div class='file-upload'><div class='file-browse-btn'><span class='btn-text file-upload-btn' title='Upload'>Browse</span><input multiple='multiple' onChange='FormBuilder.prototype.ValidateDocument(this);' type='file' data-unique='{properties.htmlAttributes?.FirstOrDefault(x => x.Key == "UniqueValue").Value}' data-val='{properties.htmlAttributes?.FirstOrDefault(x => x.Key == "Extensions").Value}' value='{System.Net.WebUtility.HtmlEncode(properties.Value)}' id='p{properties.Id }' class='{properties.CSSClass}' name='{properties.Name}'/></div></div><div class='file-upload-name'><span id=error_p{properties.Id } class='error-msg' style='display: block;'></span><span id=FileName_p{properties.Id }></span><input class='valid' name={ properties.Id } id={ properties.Id } value='' type='hidden' /></div>";

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
                    ControlHtml = GetMultipleImageControlHtml(properties);
                    break;
                case "Number":
                    ControlHtml = GetNumberControlHtml(properties, attribute);
                    break;
                case "Video":
                    ControlHtml = GetVideoControlHtml(properties);
                    break;
                case "File":
                    ControlHtml = GetFileControlHtml(properties);
                    break;
                default:
                    ControlHtml = $"<div class='control-md'><input type='{properties.ControlType?.ToLower()}' data-unique='{properties.htmlAttributes?.FirstOrDefault(x => x.Key == "UniqueValue").Value}' value='{System.Net.WebUtility.HtmlEncode(properties.Value)}' id='{properties.Id }' class='{properties.CSSClass}' name='{properties.Name}' {attribute}/></div>";
                    break;
            }
            ControlHtml += string.IsNullOrEmpty(properties.HelpText) ? string.Empty : CreateHelpDescriptionHtml(properties);
            return ControlHtml;
        }


        private static string CreateErrorSpanHtml(Property properties)
        {
            return $"<div class='dynamic-msg'><span class='error-msg' data-valmsg-for='{properties.Name}' data-valmsg-replace='true' id='errorSpam{properties.Name}'></span></div>";
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
                options.ForEach(x => { _options += Array.IndexOf(values, x.Value) >= 0 ? $"<option selected='selected' value ='{x.Value?.Replace("'", "&#39;")}'>{x.Text}</option>" : $"<option value ='{x.Value?.Replace("'", "&#39;")}'>{x.Text}</option>"; });

            return _options;
        }

        private static string CreateHelpDescriptionHtml(Property properties)
        => $"<div class='help-text'><i class='zf-help-circle' data-toggle='tooltip' data-placement='right' title='" + properties.HelpText + "'></i> </div>";

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
        Video,
        Audio,
        File
    }
}