using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Znode.Engine.Admin
{
    public static class AutoCompleteHelpers
    {
        public static MvcHtmlString AutocompleteFor<TModel, TProperty1, TProperty2>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty1>> valueExpression,
            Expression<Func<TModel, TProperty2>> idExpression, string actionName, string controllerName, string onSelect, bool requestFocus, object additionalViewData)
        {
            return CreateTextBoxForFromAutocompleteFor<TModel, TProperty1, TProperty2>(html, valueExpression, actionName, controllerName, onSelect, requestFocus,
                idExpression.Body.ToString(), additionalViewData);
        }

        public static MvcHtmlString AutocompleteFor<TModel, TProperty1, TProperty2>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty1>> valueExpression,
            Expression<Func<TModel, TProperty2>> idExpression, string actionName, string controllerName, bool requestFocus)
        {
            return CreateTextBoxForFromAutocompleteFor<TModel, TProperty1, TProperty2>(html, valueExpression, actionName, controllerName, string.Empty, requestFocus,
                idExpression.Body.ToString(), new object { });
        }

        public static MvcHtmlString AutocompleteFor<TModel, TProperty1, TProperty2>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty1>> valueExpression,
            Expression<Func<TModel, TProperty2>> idExpression, int index, string actionName, string controllerName, bool requestFocus)
        {
            return AutocompleteFor<TModel, TProperty1, TProperty2>(html, valueExpression, idExpression, index, actionName, controllerName, requestFocus, new object { });
        }

        public static MvcHtmlString AutocompleteFor<TModel, TProperty1, TProperty2>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty1>> valueExpression,
            Expression<Func<TModel, TProperty2>> idExpression, int index, string actionName, string controllerName, bool requestFocus, object additionalViewData)
        {
            // Get the fully qualified class name of the autocomplete id field
            string idFieldString = idExpression.Body.ToString();

            // handle if the id field is an array
            int locGetItem = idFieldString.IndexOf(".get_Item(", StringComparison.Ordinal);
            if (locGetItem > 0)
            {
                idFieldString = idFieldString.Substring(0, locGetItem);
                idFieldString += $"_{index}_";
            }

            return CreateTextBoxForFromAutocompleteFor<TModel, TProperty1, TProperty2>(html, valueExpression, actionName, controllerName, string.Empty, requestFocus, idFieldString, new object { }); ;
        }

        private static MvcHtmlString CreateTextBoxForFromAutocompleteFor<TModel, TProperty1, TProperty2>(
            HtmlHelper<TModel> html,
            Expression<Func<TModel, TProperty1>> valueExpression, string actionName, string controllerName, string onSelect,
            bool requestFocus, string idFieldString, object additionalViewData)
        {
            string autocompleteUrl = UrlHelper.GenerateUrl(null, actionName, controllerName,
                null,
                html.RouteCollection,
                html.ViewContext.RequestContext,
                includeImplicitMvcValues: true);

            // We need to strip the 'model.' from the beginning
            int loc = idFieldString.IndexOf('.');
            // Also, replace the . with _ as this is done by MVC so the field name is js friendly
            string autocompleteIdField = idFieldString.Substring(loc + 1, idFieldString.Length - loc - 1)
                .Replace('.', '_');

            dynamic htmlAttributes = new ExpandoObject();

            // handle additional view data
            if (additionalViewData != null)
            {
                var additionalViewDataPropertyNamesAndValues = additionalViewData.GetType()
                    .GetProperties()
                    .Where(pi => pi.GetGetMethod() != null)
                    .Select(pi => new
                    {
                        Name = pi.Name,
                        Value = pi.GetGetMethod().Invoke(additionalViewData, null)
                    });

                foreach (var pair in additionalViewDataPropertyNamesAndValues)
                {
                    if (pair.Name == "htmlAttributes")
                    {

                        var htmlAttributesPropertyNamesAndValues = pair.Value.GetType()
                            .GetProperties()
                            .Where(pi => pi.PropertyType == typeof(string) && pi.GetGetMethod() != null)
                            .Select(pi => new
                            {
                                Name = pi.Name,
                                Value = pi.GetGetMethod().Invoke(pair.Value, null)
                            });
                        foreach (var pair2 in htmlAttributesPropertyNamesAndValues)
                        {
                            ((IDictionary<string, object>)htmlAttributes).Add(pair2.Name, pair2.Value);
                        }
                    }
                }
            }
            // add @class if it is not there yet
            if (!((IDictionary<string, object>)htmlAttributes).ContainsKey("@class"))
            {
                htmlAttributes.@class = "";
            }

            string @class = (!((string)htmlAttributes.@class).Contains("typeahead") ? "typeahead" : "");
            ((IDictionary<string, object>)htmlAttributes).Add("data-autocomplete-url", autocompleteUrl);
            ((IDictionary<string, object>)htmlAttributes).Add("data-autocomplete-id-field", autocompleteIdField);
            ((IDictionary<string, object>)htmlAttributes).Add("data-onSelect-function", onSelect);
            htmlAttributes.data_autocomplete_url = autocompleteUrl;
            htmlAttributes.data_onselect_function = onSelect;
            htmlAttributes.@class += (!string.IsNullOrEmpty(htmlAttributes.@class) ? " " : "") + @class;

            return html.TextBoxFor(valueExpression, ((IDictionary<string, object>)htmlAttributes));
        }
        #region FastSelect 

        public static MvcHtmlString FastSelectFor<TModel, TProperty1, TProperty2>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty1>> valueExpression,
            Expression<Func<TModel, TProperty2>> idExpression, string actionName, string controllerName, string onSelect, int minQueryLength, bool loadOnce, object additionalViewData, bool isMultiple = false)
        {
            return CreateTextBoxForFromFastSelectFor<TModel, TProperty1, TProperty2>(html, valueExpression, actionName, controllerName, onSelect, minQueryLength,
                idExpression.Body.ToString(), isMultiple, loadOnce, additionalViewData);
        }

        private static MvcHtmlString CreateTextBoxForFromFastSelectFor<TModel, TProperty1, TProperty2>(
            HtmlHelper<TModel> html,
            Expression<Func<TModel, TProperty1>> valueExpression, string actionName, string controllerName, string onSelect,
            int minQueryLength, string idFieldString, bool isMultiple, bool loadOnce, object additionalViewData)
        {
            // We need to strip the 'model.' from the beginning
            int loc = idFieldString.IndexOf('.');
            // Also, replace the . with _ as this is done by MVC so the field name is js friendly
            string autocompleteIdField = idFieldString.Substring(loc + 1, idFieldString.Length - loc - 1)
                .Replace('.', '_');

            RouteValueDictionary routesvalues = new RouteValueDictionary();
            dynamic htmlAttributes = new ExpandoObject();

            // handle additional view data
            if (additionalViewData != null)
            {
                var additionalViewDataPropertyNamesAndValues = additionalViewData.GetType()
                    .GetProperties()
                    .Where(pi => pi.GetGetMethod() != null)
                    .Select(pi => new
                    {
                        Name = pi.Name,
                        Value = pi.GetGetMethod().Invoke(additionalViewData, null)
                    });

                foreach (var pair in additionalViewDataPropertyNamesAndValues)
                {
                    if (pair.Name == "htmlAttributes")
                    {

                        var htmlAttributesPropertyNamesAndValues = pair.Value.GetType()
                            .GetProperties()
                            .Where(pi => pi.PropertyType == typeof(string) && pi.GetGetMethod() != null)
                            .Select(pi => new
                            {
                                Name = pi.Name,
                                Value = pi.GetGetMethod().Invoke(pair.Value, null)
                            });
                        foreach (var pair2 in htmlAttributesPropertyNamesAndValues)
                        {
                            ((IDictionary<string, object>)htmlAttributes).Add(pair2.Name, pair2.Value);
                        }
                    }
                    else if (pair.Name == "parameters")
                    {
                        var parameterPropertyNamesAndValues = pair.Value.GetType()
                            .GetProperties()
                            .Where(pi => pi.GetGetMethod() != null)
                            .Select(pi => new
                            {
                                Name = pi.Name,
                                Value = pi.GetGetMethod().Invoke(pair.Value, null)
                            });
                        foreach (var pair2 in parameterPropertyNamesAndValues)
                        {
                            routesvalues.Add(pair2.Name, pair2.Value);
                        }
                    }
                }
            }

            string autocompleteUrl = UrlHelper.GenerateUrl(null, actionName, controllerName, routesvalues,
                html.RouteCollection, html.ViewContext.RequestContext, includeImplicitMvcValues: true);

            // Update autocompleteUrl in case of Area module. 
            string urlControllerName = autocompleteUrl.Split('/')[1];
            if (urlControllerName != controllerName)
                autocompleteUrl = autocompleteUrl.Substring(urlControllerName.Length + 1);

            // add @class if it is not there yet
            if (!((IDictionary<string, object>)htmlAttributes).ContainsKey("@class"))
            {
                htmlAttributes.@class = "";
            }

            string @class = (!((string)htmlAttributes.@class).Contains("fastSelectinput") ? "fastSelectinput" : "");
            ((IDictionary<string, object>)htmlAttributes).Add("data-url", autocompleteUrl);
            ((IDictionary<string, object>)htmlAttributes).Add("data-autocomplete-id-field", autocompleteIdField);
            ((IDictionary<string, object>)htmlAttributes).Add("data-minQueryLength", minQueryLength);
            ((IDictionary<string, object>)htmlAttributes).Add("data-onSelect-function", onSelect);
            ((IDictionary<string, object>)htmlAttributes).Add("data-load-once", loadOnce ? "true" : "false");

            if (isMultiple)
            {
                ((IDictionary<string, object>)htmlAttributes).Add("multiple", "");
            }

            htmlAttributes.@class += (!string.IsNullOrEmpty(htmlAttributes.@class) ? " " : "") + @class;

            return html.TextBoxFor(valueExpression, ((IDictionary<string, object>)htmlAttributes));
        }

        #endregion
    }
}
