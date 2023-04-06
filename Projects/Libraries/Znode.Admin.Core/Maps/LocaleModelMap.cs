using System;
using Znode.Engine.Api.Models;
using Znode.Engine.Admin.ViewModels;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Znode.Engine.Admin.Maps
{
    class LocaleModelMap
    {
        //Convert LocaleViewModel model to  LocaleModel. 
        public static LocaleModel ToModel(LocaleViewModel localeModel)
        {
            if (!Equals(localeModel, null))
            {
                return new LocaleModel()
                {
                    LocaleId = localeModel.LocaleId,
                    Name = localeModel.Name,
                    Code = localeModel.Code,
                    IsActive = localeModel.IsActive,
                    IsDefault = localeModel.IsDefault
                };
            }
            else
            {
                return null;
            }
        }

        public static LocaleListViewModel ToListViewModel(LocaleListModel models)
        {
            //Locale list model
            LocaleListViewModel listViewModel = new LocaleListViewModel();

            //checks for model not null and  profile list not null
            if (!Equals(models, null) && !Equals(models.Locales, null))
            {
                foreach (LocaleModel model in models.Locales)
                {
                    listViewModel.Locales.Add(ToViewModel(model));
                }
                //set for pagination
                listViewModel.Page = Convert.ToInt32(models.PageIndex);
                listViewModel.RecordPerPage = Convert.ToInt32(models.PageSize);
                listViewModel.TotalPages = Convert.ToInt32(models.TotalPages);
                listViewModel.TotalResults = Convert.ToInt32(models.TotalResults);
            }
            //returns result
            return listViewModel;
        }

        public static LocaleViewModel ToViewModel(LocaleModel localeModel)
        {
            //Checks if LocaleModel is null
            if (!Equals(localeModel, null))
            {
                return new LocaleViewModel()
                {
                    LocaleId = localeModel.LocaleId,
                    Name = localeModel.Name,
                    Code = localeModel.Code,
                    IsActive = localeModel.IsActive,
                    IsDefault = localeModel.IsDefault
                };
            }
            else
                return null;
        }

        public static List<SelectListItem> ToLocaleListItem(LocaleListModel model)
        {
            List<SelectListItem> LocaleItem = new List<SelectListItem>();
            if (!Equals(model.Locales, null))
            {

                LocaleItem = (from item in model.Locales
                              select new SelectListItem
                              {
                                  Text = item.Name,
                                  Value = item.LocaleId.ToString(),
                                  Selected = item.IsDefault
                              }).ToList();
            }
            return LocaleItem;
        }

    }
}