using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Resources;
using Znode.Engine.Admin.Extensions;
using System;

namespace Znode.Engine.Admin.Maps
{
    public class ProviderEngineViewModelMap
    {
        #region TaxRule Type
        //Map TaxRuleType ViewModel model to TaxRuleType Model. 
        public static TaxRuleTypeModel ToTaxRuleModel(ProviderEngineViewModel taxRuleTypeViewModel)
        {
            if (!Equals(taxRuleTypeViewModel, null))
            {
                return new TaxRuleTypeModel()
                {
                    TaxRuleTypeId = taxRuleTypeViewModel.Id,
                    IsActive = taxRuleTypeViewModel.IsActive,
                    Name = taxRuleTypeViewModel.Name,
                    ClassName = taxRuleTypeViewModel.ClassName,
                    Description = taxRuleTypeViewModel.Description,
                    PortalId = taxRuleTypeViewModel.PortalId,
                };
            }
            else
                return new TaxRuleTypeModel();
        }

        //Map TaxRuleType Model to TaxRuleType View Model.
        public static ProviderEngineViewModel ToTaxRuleViewModel(TaxRuleTypeModel taxRuleTypeModel)
        {
            if (Equals(taxRuleTypeModel, null))
                return null;

            return new ProviderEngineViewModel()
            {
                Id = taxRuleTypeModel.TaxRuleTypeId,
                IsActive = taxRuleTypeModel.IsActive,
                Name = taxRuleTypeModel.Name,
                ClassName = taxRuleTypeModel.ClassName,
                Description = taxRuleTypeModel.Description,
                PortalId = taxRuleTypeModel.PortalId,
            };
        }

        //Map TaxRuleTypeListModel to TaxRuleTypeListViewModel
        public static ProviderEngineListViewModel ToTaxRuleListViewModel(TaxRuleTypeListModel taxRuleTypeListModel)
        {
            if (taxRuleTypeListModel?.TaxRuleTypes?.Count > 0)
            {
                ProviderEngineListViewModel taxRuleTypeListViewModel = new ProviderEngineListViewModel()
                {
                    ProvideEngineTypes = taxRuleTypeListModel.TaxRuleTypes.Select(
                        taxRuleTypeModel => new ProviderEngineViewModel()
                        {
                            Id = taxRuleTypeModel.TaxRuleTypeId,
                            IsActive = taxRuleTypeModel.IsActive,
                            Name = taxRuleTypeModel.Name,
                            ClassName = taxRuleTypeModel.ClassName,
                            Description = taxRuleTypeModel.Description,
                            PortalId = taxRuleTypeModel.PortalId,
                        }).ToList()
                };  
                taxRuleTypeListViewModel.Page = Convert.ToInt32(taxRuleTypeListModel.PageIndex);
                taxRuleTypeListViewModel.RecordPerPage = Convert.ToInt32(taxRuleTypeListModel.PageSize);
                taxRuleTypeListViewModel.TotalPages = Convert.ToInt32(taxRuleTypeListModel.TotalPages);
                taxRuleTypeListViewModel.TotalResults = Convert.ToInt32(taxRuleTypeListModel.TotalResults);
                return taxRuleTypeListViewModel;
            }
            else
                return new ProviderEngineListViewModel();
        }

        //Convert IEnumerable type list of TaxRuleTypeModel to List<SelectListItem>
        public static List<SelectListItem> ToTaxRuleListItems(IEnumerable<TaxRuleTypeModel> taxRuleTypeModel)
        {
            List<SelectListItem> taxRuleTypeList = new List<SelectListItem>();
            if (!Equals(taxRuleTypeModel, null))
            {
                taxRuleTypeList = (from taxRuleType in taxRuleTypeModel
                                   select new SelectListItem
                                   {
                                       Text = taxRuleType.Name,
                                       Value = taxRuleType.Name
                                   }).ToList();
            }
            taxRuleTypeList.Insert(0, new SelectListItem() { Value = "0", Text = Admin_Resources.LabelSelectTaxRuleType });

            return taxRuleTypeList;
        }
        #endregion

        #region Shipping Type
        //Map ShippingType ViewModel model to ShippingType Model. 
        public static ShippingTypeModel ToShippingTypeModel(ProviderEngineViewModel shippingTypeViewModel)
        {
            if (!Equals(shippingTypeViewModel, null))
            {
                return new ShippingTypeModel()
                {
                    ShippingTypeId = shippingTypeViewModel.Id,
                    IsActive = shippingTypeViewModel.IsActive,
                    Name = shippingTypeViewModel.Name,
                    ClassName = shippingTypeViewModel.ClassName,
                    Description = shippingTypeViewModel.Description,
                };
            }
            else
                return new ShippingTypeModel();
        }

        //Map ShippingType Model to ShippingType View Model.
        public static ProviderEngineViewModel ToShippingTypeViewModel(ShippingTypeModel shippingTypeModel)
        {
            if (Equals(shippingTypeModel, null))
                return null;

            return new ProviderEngineViewModel()
            {
                Id = shippingTypeModel.ShippingTypeId,
                IsActive = shippingTypeModel.IsActive,
                Name = shippingTypeModel.Name,
                ClassName = shippingTypeModel.ClassName,
                Description = shippingTypeModel.Description,
            };
        }

        //Map ShippingTypeListModel to ShippingTypeListViewModel
        public static ProviderEngineListViewModel ToShippingListViewModel(ShippingTypeListModel shippingTypeListModel)
        {
            if (shippingTypeListModel?.ShippingTypeList?.Count > 0)
            {
                ProviderEngineListViewModel shippingTypeListViewModel = new ProviderEngineListViewModel()
                {
                    ProvideEngineTypes = shippingTypeListModel.ShippingTypeList.Select(
                        shippingTypeModel => new ProviderEngineViewModel()
                        {
                            Id = shippingTypeModel.ShippingTypeId,
                            IsActive = shippingTypeModel.IsActive,
                            Name = shippingTypeModel.Name,
                            ClassName = shippingTypeModel.ClassName,
                            Description = shippingTypeModel.Description,
                        }).ToList()
                };

                shippingTypeListViewModel.Page = Convert.ToInt32(shippingTypeListModel.PageIndex);
                shippingTypeListViewModel.RecordPerPage = Convert.ToInt32(shippingTypeListModel.PageSize);
                shippingTypeListViewModel.TotalPages = Convert.ToInt32(shippingTypeListModel.TotalPages);
                shippingTypeListViewModel.TotalResults = Convert.ToInt32(shippingTypeListModel.TotalResults);

                return shippingTypeListViewModel;
            }
            else
                return new ProviderEngineListViewModel();
        }

        //Convert IEnumerable type list of ShippingTypeModel to List<SelectListItem>
        public static List<SelectListItem> ToShippingListItems(IEnumerable<ShippingTypeModel> shippingTypeModel)
        {
            List<SelectListItem> shippingTypeList = new List<SelectListItem>();
            if (!Equals(shippingTypeModel, null))
            {
                shippingTypeList = (from taxRuleType in shippingTypeModel
                                   select new SelectListItem
                                   {
                                       Text = taxRuleType.Name,
                                       Value = taxRuleType.Name
                                   }).ToList();
            }
            shippingTypeList.Insert(0, new SelectListItem() { Value = "0", Text = Admin_Resources.TitleSelectShippingType });
            return shippingTypeList;
        }
        #endregion

        #region Promotion Type
        //Map PromotionType ViewModel model to PromotionType Model. 
        public static PromotionTypeModel ToPromotionTypeModel(ProviderEngineViewModel promotionTypeViewModel)
        {
            if (!Equals(promotionTypeViewModel, null))
            {
                return new PromotionTypeModel()
                {
                    PromotionTypeId = promotionTypeViewModel.Id,
                    IsActive = promotionTypeViewModel.IsActive,
                    Name = promotionTypeViewModel.Name,
                    ClassName = promotionTypeViewModel.ClassName,
                    ClassType = promotionTypeViewModel.ClassType,
                    Description = promotionTypeViewModel.Description,

                };
            }
            else
                return new PromotionTypeModel();
        }

        //Map PromotionType Model to PromotionType View Model.
        public static ProviderEngineViewModel ToPromotionTypeViewModel(PromotionTypeModel promotionTypeModel)
        {
            if (Equals(promotionTypeModel, null))
                return null;

            return new ProviderEngineViewModel()
            {
                Id = promotionTypeModel.PromotionTypeId,
                IsActive = promotionTypeModel.IsActive,
                Name = promotionTypeModel.Name,
                ClassName = promotionTypeModel.ClassName,
                ClassType = promotionTypeModel.ClassType,
                Description = promotionTypeModel.Description,
            };
        }

        //Map PromotionTypeListModel to PromotionTypeListViewModel
        public static ProviderEngineListViewModel ToPromotionListViewModel(PromotionTypeListModel promotionTypeListModel)
        {
            if (promotionTypeListModel?.PromotionTypes?.Count > 0)
            {
                ProviderEngineListViewModel promotionTypeListViewModel = new ProviderEngineListViewModel()
                {
                    ProvideEngineTypes = promotionTypeListModel.PromotionTypes.Select(
                        promotionTypeModel => new ProviderEngineViewModel()
                        {
                            Id = promotionTypeModel.PromotionTypeId,
                            IsActive = promotionTypeModel.IsActive,
                            Name = promotionTypeModel.Name,
                            ClassName = promotionTypeModel.ClassName,
                            ClassType = promotionTypeModel.ClassType,
                            Description = promotionTypeModel.Description,
                        }).ToList()
                };

                promotionTypeListViewModel.Page = Convert.ToInt32(promotionTypeListModel.PageIndex);
                promotionTypeListViewModel.RecordPerPage = Convert.ToInt32(promotionTypeListModel.PageSize);
                promotionTypeListViewModel.TotalPages = Convert.ToInt32(promotionTypeListModel.TotalPages);
                promotionTypeListViewModel.TotalResults = Convert.ToInt32(promotionTypeListModel.TotalResults);

                return promotionTypeListViewModel;
            }
            else
                return new ProviderEngineListViewModel();
        }

        //Convert IEnumerable type list of PromotionTypeModel to List<SelectListItem>
        public static List<SelectListItem> ToPromotionListItems(IEnumerable<PromotionTypeModel> promotionTypeModel)
        {
            List<SelectListItem> promotionTypeList = new List<SelectListItem>();
            if (!Equals(promotionTypeModel, null))
            {
                promotionTypeList = (from taxRuleType in promotionTypeModel
                                    select new SelectListItem
                                    {
                                        Text = taxRuleType.Name,
                                        Value = taxRuleType.Name
                                    }).ToList();
            }
            promotionTypeList.Insert(0, new SelectListItem() { Value = "0", Text = Admin_Resources.TitleSelectPromotionType });
            return promotionTypeList;
        }

        // This method maps PIMFamilyDetailsModel to PIMFamilyDetailsViewModel for promotion attribute.
        public static PIMFamilyDetailsViewModel ToPIMFamilyDetailsViewModel(PIMFamilyDetailsModel model)
        {
            if (!Equals(model?.Attributes, null))
            {
                PIMFamilyDetailsViewModel pimFamilyDetailsViewModel = new PIMFamilyDetailsViewModel
                {
                    Attributes = model.Attributes.ToViewModel<PIMProductAttributeValuesViewModel>().ToList(),
                };

                List<string> distinctAttributeCodes = pimFamilyDetailsViewModel.Attributes.Where(x => x != null).Select(e => e.AttributeCode + e.PimAttributeFamilyId).Distinct().ToList();
                pimFamilyDetailsViewModel.Attributes = PIMAttributeFamilyViewModelMap.GetAttributeControls(pimFamilyDetailsViewModel.Attributes, distinctAttributeCodes);
                return pimFamilyDetailsViewModel;
            }
            return null;
        }
        #endregion
    }
}