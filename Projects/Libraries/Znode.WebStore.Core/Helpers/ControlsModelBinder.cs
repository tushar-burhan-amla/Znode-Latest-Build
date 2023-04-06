using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;

namespace Znode.Engine.WebStore
{
    public class ControlsModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContexts)
        {
            BindDataModel controlsDataModel = new BindDataModel();
            var formCollection = controllerContext.HttpContext.Request.Form;

            foreach (var keys in formCollection)
            {
                string keyname = keys.ToString();

                var keyValue = "";
                try
                {
                    keyValue = formCollection[keyname]?.Trim();
                }
                catch (HttpRequestValidationException)
                {
                    if (keyname.StartsWith("mceEditor"))
                    {
                        NameValueCollection unValidatedForm = controllerContext.HttpContext.Request.Unvalidated.Form;
                        keyValue = unValidatedForm[keyname];
                    }
                    else
                        throw;
                }

                if (!string.IsNullOrEmpty(keyname) && !Equals(keyname, "__RequestVerificationToken"))
                    controlsDataModel.ControlsData.Add(keyname, keyValue);
            }
            return controlsDataModel;
        }
    }

    // Trims leading and trailing spaces in text values.
    public class TrimModelBinder : DefaultModelBinder
    {
        protected override void SetProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, System.ComponentModel.PropertyDescriptor propertyDescriptor, object value)
        {
            if (propertyDescriptor.PropertyType == typeof(string))
            {
                string stringValue = (string)value;
                if (!string.IsNullOrWhiteSpace(stringValue))
                    value = stringValue.Trim();
                else
                    value = null;
            }
            base.SetProperty(controllerContext, bindingContext, propertyDescriptor, value);
        }
    }
}
