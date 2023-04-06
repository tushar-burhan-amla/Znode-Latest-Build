using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public static class PublishHelper
    {
                
        public static string GetIndexName(string catalogName)
        {
            if (!string.IsNullOrEmpty(catalogName))
            {
                catalogName = RegexQuery(catalogName);
                return $"{catalogName.ToLower()}index";
            }
            return string.Empty;
        }

        private static string RegexQuery(string catalogName)
        {
            catalogName = Regex.Replace(catalogName, @"\s+", ""); // Regex to remove spaces 
            catalogName = Regex.Replace(catalogName, @"[^a-zA-Z0-9_]{1,255}\+", string.Empty); //Regex to have alphabet and numbers up to limit 255.
            catalogName = Regex.Replace(catalogName, @"[?!^#\/*?<>..|.]", string.Empty);//Regex to remove the special character that are mentioned .
            catalogName = Regex.Replace(catalogName, @"(?i)([?!^(+_)])", string.Empty); //Regex to remove the special character if they are at starting postion .
            catalogName = catalogName.StartsWith("-") ? catalogName.Replace('-', ' ') : catalogName; //To remove '-'.
            return catalogName;
        }

        public static string GetIndexName(int publishCatalogId)
        {
            if (publishCatalogId > 0)
            {
                ISearchService searchService = GetService<ISearchService>();
                PortalIndexModel portalIndex = searchService.GetCatalogIndexData(null, new FilterCollection() { new FilterTuple(ZnodeCatalogIndexEnum.PublishCatalogId.ToString(), FilterOperators.Equals, publishCatalogId.ToString()) });
                if (portalIndex?.CatalogIndexId > 0)
                    return portalIndex.IndexName;
                else
                {
                    return GetIndexName(GetService<IPublishedCatalogDataService>().GetPublishCatalogById(publishCatalogId)?.CatalogName);
                }
            }
            return string.Empty;
        }
        
        //Converts Searchable Attributes List to Data Table
        public static DataTable ConvertKeywordListToDataTable(List<int> publishProductIds)
        {
            DataTable table = new DataTable("@PimProductId");
            table.Columns.Add("Id", typeof(int));

            foreach (int model in publishProductIds)
                table.Rows.Add(model);
            return table;
        }
    }
}