using System.Diagnostics;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Services
{
    public class WebStoreWishListService : BaseService, IWebStoreWishListService
    {
        #region Private Variable
        private readonly IZnodeRepository<ZnodeUserWishList> _wishListRepository;
        #endregion

        #region Public Constructor.
        public WebStoreWishListService()
        {
            _wishListRepository = new ZnodeRepository<ZnodeUserWishList>();
        }
        #endregion

        //Add product to user wishlist
        public virtual WishListModel AddToWishList(WishListModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("WishListModel with UserWishListId ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose,model?.UserWishListId);

            if (HelperUtility.IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelCanNotBeNull);

            if (UpdateWishList(model))
                return model;

            return _wishListRepository.Insert(model.ToEntity<ZnodeUserWishList>())?.ToModel<WishListModel>();
        }

        //Delete wishlist.
        public virtual bool DeleteWishlist(int wishlistId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("wishlistId to be deleted ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, wishlistId);

            if (wishlistId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorWishlistIdLessThanOne);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeUserWishListEnum.UserWishListId.ToString(), FilterOperators.Equals, wishlistId.ToString()));
            string whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause;
            ZnodeLogging.LogMessage("WhereClause for delete :", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, whereClause);

            bool isDeleted = _wishListRepository.Delete(whereClause);
            ZnodeLogging.LogMessage(isDeleted ? string.Format( Admin_Resources.SuccessWishlistDelete, wishlistId) : string.Format(Admin_Resources.ErrorWishlistDelete, wishlistId), ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return isDeleted;
        }

        //Update product to user wishlist
        public virtual bool UpdateWishList(WishListModel model)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeUserWishListEnum.SKU.ToString(), FilterOperators.Is, model.SKU));
            filters.Add(new FilterTuple(ZnodeUserWishListEnum.UserId.ToString(), FilterOperators.Equals, model.UserId.ToString()));
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeUserWishList znodeModel = _wishListRepository.GetEntity(whereClauseModel.WhereClause, whereClauseModel.FilterValues);
            if (IsNotNull(znodeModel))
            {
                model.UserWishListId = znodeModel.UserWishListId;
                if (_wishListRepository.Update(model.ToEntity<ZnodeUserWishList>()))
                    return true;
                else
                    throw new ZnodeException(ErrorCodes.UpdateCustomFieldError, Admin_Resources.UpdateError);
            }
            return false;
        }
    }
}
