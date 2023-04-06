using System;
using System.Linq;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Admin.Maps
{
    public class IssuedGiftCardViewModelMap
    {
        // Convert issued gift card model to view model.
        public static IssuedGiftCardListViewModel ToListViewModel(IssuedGiftCardListModel models)
        {
            IssuedGiftCardListViewModel viewModel = new IssuedGiftCardListViewModel();
            if (!Equals(models.IssuedGiftCardModels, null))
            {
                viewModel.IssuedGiftCardModels = models.IssuedGiftCardModels.Select(
                x => new IssuedGiftCardViewModel()
                {
                    CardNumber = x.CardNumber,
                    Amount = x.Amount,
                    ExpirationDate = Convert.ToString(x.ExpirationDate)
                }).ToList();
            }
            return viewModel;
        }
    }
}