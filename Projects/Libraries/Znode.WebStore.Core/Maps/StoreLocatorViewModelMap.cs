using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Znode.Engine.WebStore.Maps
{
    public class StoreLocatorViewModelMap
    {
        // Bind List of distances to dropdown.
        public static List<SelectListItem> GetDistances()
        {
            List<SelectListItem> statusList = new List<SelectListItem>();
            statusList = (from item in GetDistanceDictionary()
                          select new SelectListItem
                          {
                              Text = item.Value,
                              Value = item.Key,
                              Selected = Equals(item.Key, 0),
                          }).ToList();

            return statusList;
        }

        //Dropdown for Distances.
        private static Dictionary<string, string> GetDistanceDictionary()
        {
            Dictionary<string, string> Distance = new Dictionary<string, string>();
            Distance.Add("0", "Distance from Zip");
            Distance.Add("5", "5 Miles");
            Distance.Add("10", "10 Miles");
            Distance.Add("25", "25 Miles");
            Distance.Add("50", "50 Miles");
            Distance.Add("75", "75 Miles");
            Distance.Add("100", "100 Miles");
            return Distance;
        }
    }

}