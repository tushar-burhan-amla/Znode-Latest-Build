using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Maps
{
    public class GeneralSettingMap
    {
        #region Public methods
        public static GlobalSettingViewModel ToViewModel(GeneralSettingModel model)
        {
            if (HelperUtility.IsNotNull(model))
            {
                GlobalSettingViewModel viewModel = new GlobalSettingViewModel();
                viewModel.DisplayUnitList = new List<SelectListItem>();
                viewModel.DateFormatList = new List<SelectListItem>();
                viewModel.TimeZoneList = new List<SelectListItem>();
                viewModel.WeightUnitList = new List<SelectListItem>();
                viewModel.TimeFormatList = new List<SelectListItem>();

                //Get details related to date format.
                GetDateFormatDetails(model, viewModel);

                //Get details related to Time Format.
                GetTimeFormatDetails(model, viewModel);

                //Get details related to display units.
                GetDisplayUnitDetails(model, viewModel);

                //Get details related to time zone.
                GetTimeZoneDetails(model, viewModel);

                //Get details related to weight unit.
                GetWeightUnitDetails(model, viewModel);

                viewModel.PriceRoundOffFeatureValue = Convert.ToInt32(model.PriceRoundOffList?.FirstOrDefault()?.FeatureValues);

                viewModel.InventoryRoundOffFeatureValue = Convert.ToInt32(model.InventoryRoundOffList?.FirstOrDefault()?.FeatureValues);

                viewModel.CurrentEnvironmentFeatureValue = model.EnvironmentsList?.FirstOrDefault()?.FeatureValues;

                viewModel.ServerTimeZone = model.ServerTimeZone;

                return viewModel;
            }
            return new GlobalSettingViewModel();
        } 
        #endregion

        #region Private Methods
        //Get details related to weight unit.
        private static void GetWeightUnitDetails(GeneralSettingModel model, GlobalSettingViewModel viewModel)
        {
            viewModel.WeightUnitId = model.WeightUnitList?.Where(x => x.IsDefault)?.FirstOrDefault()?.WeightUnitId;
            model.WeightUnitList.ForEach(item => viewModel.WeightUnitList.Add(new SelectListItem() { Text = item.WeightUnitCode, Value = item.WeightUnitId.ToString(), Selected = item.IsDefault }));
        }

        //Get details related to time zone.
        private static void GetTimeZoneDetails(GeneralSettingModel model, GlobalSettingViewModel viewModel)
        {
            viewModel.TimeZoneId = model.TimeZoneList?.Where(x => x.IsDefault)?.FirstOrDefault()?.TimeZoneId;
            model.TimeZoneList.ForEach(item => viewModel.TimeZoneList.Add(new SelectListItem() { Text = item.TimeZoneDetailsDesc, Value = item.TimeZoneId.ToString(), Selected = item.IsDefault }));
        }

        //Get details related to display units.
        private static void GetDisplayUnitDetails(GeneralSettingModel model, GlobalSettingViewModel viewModel)
        {
            viewModel.DisplayUnitId = model.DisplayUnitList?.Where(x => x.IsDefault)?.FirstOrDefault()?.DisplayUnitId;
            model.DisplayUnitList.ForEach(item => viewModel.DisplayUnitList.Add(new SelectListItem() { Text = item.DisplayUnitCode, Value = item.DisplayUnitId.ToString(), Selected = item.IsDefault }));
        }

        //Get details related to date format.
        private static void GetDateFormatDetails(GeneralSettingModel model, GlobalSettingViewModel viewModel)
        {
            viewModel.DateFormatId = model.DateFormatList?.Where(x => x.IsDefault)?.FirstOrDefault()?.DateFormatId;
            model.DateFormatList.ForEach(item => viewModel.DateFormatList.Add(new SelectListItem() { Text = item.DateFormat, Value = item.DateFormatId.ToString(), Selected = item.IsDefault }));
        }

        //Get details related to time format.
        private static void GetTimeFormatDetails(GeneralSettingModel model, GlobalSettingViewModel viewModel)
        {
            viewModel.TimeFormatId = model.TimeFormatList?.Where(x => x.IsDefault)?.FirstOrDefault()?.TimeFormatId;
            model.TimeFormatList.ForEach(item => viewModel.TimeFormatList.Add(new SelectListItem() { Text = item.TimeFormat, Value = item.TimeFormatId.ToString(), Selected = item.IsDefault }));
        }
        #endregion
    }
}