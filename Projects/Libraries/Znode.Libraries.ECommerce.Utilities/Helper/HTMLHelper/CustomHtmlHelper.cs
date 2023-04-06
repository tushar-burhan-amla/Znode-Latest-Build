using System.Web.Mvc;

namespace Znode.Libraries.ECommerce.Utilities
{
    public static class CustomHtmlHelper
    {
        public static MvcHtmlString SearchTextBox(string name, string placeHolder, string id, bool isReadOnly, string value = "", string type = "", string data_test_selector = "")
        {
            //declare the html helper 
            TagBuilder builder = new TagBuilder("input");
            //hook the properties and add any required logic
            builder.MergeAttribute("name", name);
            builder.MergeAttribute("placeHolder", placeHolder);
            builder.MergeAttribute("id", id);
            builder.MergeAttribute("value", value);
            builder.MergeAttribute("data-test-selector", data_test_selector);
            if (!string.IsNullOrEmpty(type))
                builder.MergeAttribute("type", type);
            if (isReadOnly)
                builder.MergeAttribute("readonly", "readonly");

            //create the helper with a self closing capability
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.SelfClosing));
        }

        public static MvcHtmlString HiddenField(string name, string id, string type = "", string value = "")
        {
            //declare the html helper 
            TagBuilder builder = new TagBuilder("input");
            //hook the properties and add any required logic
            builder.MergeAttribute("name", name);
            builder.MergeAttribute("id", id);
            builder.MergeAttribute("value", value);
            if (!string.IsNullOrEmpty(type))
                builder.MergeAttribute("type", type);

            //create the helper with a self closing capability
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.SelfClosing));
        }

        public static MvcHtmlString SearchButton(string id, string clickElement = "", string title = "", string data_test_selector = "")
        {
            // Build A tag
            TagBuilder aTagBuilder = new TagBuilder("a");

            aTagBuilder.MergeAttribute("id", id);
            aTagBuilder.MergeAttribute("onclick", clickElement);
            aTagBuilder.MergeAttribute("class", "btn-narrow-icon");
            aTagBuilder.MergeAttribute("data-placement", "top");
            aTagBuilder.MergeAttribute("data-toggle", "tooltip");
            aTagBuilder.MergeAttribute("data-original-title", title);
            aTagBuilder.MergeAttribute("data-test-selector", data_test_selector);
            // Build I tag
            TagBuilder iTagBuilder = new TagBuilder("i");

            iTagBuilder.MergeAttribute("class", "z-search");

            // Render the end tag
            iTagBuilder.ToString(TagRenderMode.EndTag);

            // Add the I tag to the A tag
            aTagBuilder.InnerHtml += iTagBuilder.ToString();

            return new MvcHtmlString(aTagBuilder.ToString());
        }
    }
}
