using System;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Admin.Extensions;
using System.Linq;

namespace Znode.Engine.Admin.Maps
{
    public class MenuViewModelMap
    {
        //Converts MenuListModel to MenuListViewModel
        public static MenuListViewModel ToListViewModel(MenuListModel models)
        {
            MenuListViewModel listViewModel = new MenuListViewModel();

            //checks for model not null and  menu list not null
            if (!Equals(models, null) && !Equals(models.Menus, null))
            {
                //Iterate through  menu model
                foreach (MenuModel model in models.Menus)
                {
                    if (HelperUtility.IsNotNull(model))
                    {
                        //Maps the  menu list model with  menulistview model.
                        listViewModel.Menus.Add(model.ToViewModel<MenuViewModel>());
                        listViewModel.ParentMenus.Add(model.ToViewModel<MenuViewModel>());

                    }
                }
                //Set for pagination
                listViewModel.Permissions = models.Permissions?.Permissions?.ToViewModel<PermissionsViewModel>().ToList();
                listViewModel.Page = Convert.ToInt32(models.PageIndex);
                listViewModel.RecordPerPage = Convert.ToInt32(models.PageSize);
                listViewModel.TotalPages = Convert.ToInt32(models.TotalPages);
                listViewModel.TotalResults = Convert.ToInt32(models.TotalResults);
            }
            return listViewModel;
        }
    }
}