using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Admin.Maps
{
    public class PortalProfileViewModelMap
    {
        //Bind Profile to Drop Down List
        public static List<SelectListItem> ToListItems(IEnumerable<ProfileModel> profileModel)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            if (!Equals(profileModel, null))
            {
                listItems = (from item in profileModel
                             orderby item.ProfileName ascending
                             where item.ParentProfileId == null
                             select new SelectListItem
                             {
                                 Text = item.ProfileName,
                                 Value = item.ProfileId.ToString()
                             }).ToList();
            }
            return listItems;
        }
    }
}