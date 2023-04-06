using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Znode.Engine.Api.Models;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.WebStore.Maps
{
    public class SearchMap
    {
        public static List<FacetViewModel> ToFacetViewModel(List<SearchFacetModel> searchFacetModelList, int mappingId, string typeOfMapping, string searchTerm, string brandName = "")
        {
            List<FacetViewModel> facets = new List<FacetViewModel>();
            if (searchFacetModelList?.Count > 0)
            {
                foreach (SearchFacetModel searchFacetModel in searchFacetModelList)
                {
                    if (searchFacetModel.AttributeValues?.Count > 0)
                    {
                        FacetViewModel facet = new FacetViewModel()
                        {
                            AttributeName = !string.IsNullOrEmpty(searchFacetModel.AttributeName?.Split('|')[0]) ? (searchFacetModel.AttributeName?.Split('|')[0]).Replace("_", " ") : string.Empty,
                            AttributeValues = new List<FacetValueViewModel>(searchFacetModel.AttributeValues.Select(x => new FacetValueViewModel()
                            {
                                AttributeCode = HttpUtility.UrlDecode(searchFacetModel.AttributeCode),
                                AttributeValue = HttpUtility.UrlDecode(x.AttributeValue),
                                FacetCount = x.FacetCount,
                                Label = x.Label,
                                RangeMax = x.RangeMax,
                                RangeMin = x.RangeMin,
                                RangeEnd = x.RangeEnd,
                                RangeStart = x.RangeStart,
                                RefineByUrl = GetURL(searchFacetModel.AttributeCode.Split(new string[] { "|" }, StringSplitOptions.None)[0], x.AttributeValue, mappingId, typeOfMapping, searchTerm, brandName),
                            }).ToList()),
                            ControlTypeId = searchFacetModel.ControlTypeId,
                            ControlType = searchFacetModel.ControlType,
                        };
                        facets.Add(facet);
                    }
                }
            }
            return facets;
        }

        public static string GetURL(string key, string value, int mappingId, string typeOfMapping, string searchTerm, string brandName)
        {
            switch (typeOfMapping)
            {
                case ZnodeConstant.Brand:
                    return $"Search?FacetGroup={key}&FacetValue={HttpUtility.UrlEncode(value)}&brandId={mappingId}";
                case ZnodeConstant.SearchWidget:
                    return $"Search?FacetGroup={key}&FacetValue={HttpUtility.UrlEncode(value)}&pageId={mappingId}";
                default:
                    if (mappingId > 0)
                        return $"Search?FacetGroup={key}&FacetValue={HttpUtility.UrlEncode(value)}&categoryId={mappingId}&SearchTerm={searchTerm}";
                    else
                        return $"Search?FacetGroup={key}&FacetValue={HttpUtility.UrlEncode(value)}&SearchTerm={searchTerm}";
            }
        }

        public static string UpdateUrlQueryString(string key, string value, int categoryId, bool keepExisting = false, bool toRemove = false)
        {
            return UpdateUrlQueryString(key, value, null, categoryId, keepExisting, toRemove);
        }

        public static string UpdateUrlQueryString(string key, string value, string urlPath, int categoryId, bool keepExisting = false, bool toRemove = false)
        {
            // To run NUnit test cases the below code added
            if (HttpContext.Current == null)
                return string.Empty;


            var nameValues = HttpUtility.ParseQueryString(HttpContext.Current.Request.QueryString.ToString());
            string currentValue = value;

            if (keepExisting && !string.IsNullOrEmpty(nameValues.Get(key)))
            {
                currentValue = $"{nameValues.Get(key)},{currentValue}";
            }
            else if (toRemove && !string.IsNullOrEmpty(nameValues.Get(key)) && !string.IsNullOrEmpty(currentValue))
            {
                string[] existingValues = nameValues.Get(key).Split(',');
                var newValues = existingValues.Where(x => x != currentValue);
                currentValue = string.Join(",", newValues);
            }
            nameValues.Set("Group", key);
            nameValues.Set("SearchTerm", value);

            nameValues.Remove("IsRemove");
            string url = (!string.IsNullOrEmpty(urlPath)) ? urlPath : HttpContext.Current.Request.Url.AbsolutePath;

            if (categoryId > 0)
                nameValues.Set("categoryId", Convert.ToString(categoryId));

            string updatedQueryString = "?" + nameValues.ToString();
            return url + updatedQueryString;

        }
    }
}