using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Znode.Engine.WebStore.Agents;
using Znode.Engine.WebStore.ViewModels;

namespace Znode.Engine.WebStore
{
    public static class StringExtension
    {
        // Extension method; the first parameter takes the "this" keyword and specifies the type for which the method is defined
        public static string FirstCharToUpper(this string input)
        {
            if (String.IsNullOrEmpty(input))
                throw new ArgumentException("Input string is null");

            return input.First().ToString().ToUpper() + String.Join("", input.Skip(1));
        }
    }

    public static class Attributes
    {
        /// <summary>
        /// Get attribute value.
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="attributeCode"></param>
        /// <returns></returns>
        public static string Value(this List<AttributesViewModel> attributes, string attributeCode) => attributes?.FirstOrDefault(x => x.AttributeCode == attributeCode)?.AttributeValues;

        /// <summary>
        /// Get attribute code.
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="attributeCode"></param>
        /// <returns></returns>
        public static string Code(this List<AttributesViewModel> attributes, string attributeCode) => attributes?.FirstOrDefault(x => x.AttributeCode == attributeCode)?.AttributeCode;

        /// <summary>
        /// Get attribute label.
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="attributeCode"></param>
        /// <returns></returns>
        public static string Label(this List<AttributesViewModel> attributes, string attributeCode) => attributes?.FirstOrDefault(x => x.AttributeCode == attributeCode)?.AttributeName;

        /// <summary>
        /// Get list of select type attribute from main attribute.
        /// </summary>
        /// <param name="attributes">List of AttributesViewModel </param>
        /// <param name="attributeCode">attribute Code</param>
        /// <returns>List of AttributesSelectValuesViewModel</returns>
        public static List<AttributesSelectValuesViewModel> SelectAttributeList(this List<AttributesViewModel> attributes, string attributeCode)
        => attributes?.FirstOrDefault(x => x.AttributeCode == attributeCode)?.SelectValues;

        /// <summary>
        /// Get list of select type attribute from main attribute.
        /// </summary>
        /// <param name="attributes">List of AttributesViewModel </param>
        /// <param name="attributeCode">attribute Code</param>
        /// <returns>List of AttributesSelectValuesViewModel</returns>
        public static string ValueFromSelectValue(this List<AttributesViewModel> attributes, string attributeCode)
        => attributes?.FirstOrDefault(x => x.AttributeCode == attributeCode)?.SelectValues?.FirstOrDefault()?.Value;

       /// <summary>
       /// Get attribute code.
       /// </summary>
       /// <param name="attributes"></param>
       /// <param name="attributeCode"></param>
       /// <returns></returns>       
        public static string CodeFromSelectValue(this List<AttributesViewModel> attributes, string attributeCode)
      => attributes?.FirstOrDefault(x => x.AttributeCode == attributeCode)?.SelectValues?.FirstOrDefault()?.Code;
    }

    public static class MediaPaths
    {
        public static string MediaPath
        {
            get
            {
                return PortalAgent.CurrentPortal.MediaServerUrl;
            }
        }

        public static string MediaThumbNailPath
        {
            get
            {
                return PortalAgent.CurrentPortal.MediaServerThumbnailUrl;
            }
        }


        public static string VideoUrl { get; set; }

        public static string YouTubeVideoId
        {
            get
            {
                var youtubeMatch =
                    new Regex(@"youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)")
                    .Match(VideoUrl);
                return youtubeMatch.Success ? youtubeMatch.Groups[1].Value : string.Empty;
            }
        }

        public static string VimeoVideoId
        {
            get
            {
                var vimeoMatch =
                          new Regex(@"vimeo\.com/(?:.*#|.*/videos/)?([0-9]+)", RegexOptions.IgnoreCase | RegexOptions.Multiline)
                    .Match(VideoUrl);
                return vimeoMatch.Success ? vimeoMatch.Groups[1].Value : string.Empty;
            }
        }

        public static string GetVideoTag(string url)
     => $"<div class>{GetVideoSource(url)}</div>";


        private static string GetVideoSource(string url)
        {
            string _ext = Path.GetExtension(url);
            string tag = string.Empty;
            if (!string.IsNullOrEmpty(_ext))
            {
                _ext = _ext.Remove(0, 1);
                tag = $"<video class=\"embed-responsive-item\" controls><source src=\"{url}\"  preload=\"metadata\"></video>";
            }
            else
            {
                VideoUrl = url;
                tag = !string.IsNullOrEmpty(YouTubeVideoId) ? $"<embed width=\"420\" height=\"315\" src=\"https://www.youtube.com/embed/{YouTubeVideoId}?controls=1\" />" : VimeoTag();
            }
            return tag;
        }

        private static string VimeoTag()
        {
            return !string.IsNullOrEmpty(VimeoVideoId) ? $"<object width=\"420\" height=\"315\">" +
                    $"<param name=\"allowfullscreen\" value=\"true\" />" +
                    $"<param name=\"allowscriptaccess\" value=\"always\" />" +
                    $"<param name=\"movie\" value=\"https://vimeo.com/moogaloop.swf?clip_id={VimeoVideoId}&amp;server=vimeo.com&amp;color=00adef&amp;fullscreen=1\" />" +
                    $"<embed src=\"https://vimeo.com/moogaloop.swf?clip_id={VimeoVideoId}&amp;server=vimeo.com&amp;color=00adef&amp;fullscreen=1\"" +
                    $"type=\"application/x-shockwave-flash\" allowfullscreen=\"true\" allowscriptaccess=\"always\" width=\"420\" height=\"315\"></embed>" +
                    $"</object>" : string.Empty;
        }
    }

    public static class ImagePaths
    {
        public static string ThumbnailImage(this List<AttributesViewModel> attributes)
        {
            string fileName = attributes?.FirstOrDefault(x => x.AttributeCode == "Image")?.AttributeValues;
            return !string.IsNullOrEmpty(fileName) ? $"{PortalAgent.CurrentPortal.MediaServerThumbnailUrl}{"/"}{fileName}" : string.Empty;
        }
    }
}