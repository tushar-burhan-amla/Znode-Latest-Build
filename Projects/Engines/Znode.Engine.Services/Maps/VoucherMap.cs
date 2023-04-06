using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services.Maps
{
    public static class VoucherMap
    {
        public static ZnodeVoucher ToZnodeVoucher(VoucherModel voucherModel)
       => HelperUtility.IsNull(voucherModel) ? null :
         new ZnodeVoucher
         {
             VoucherMessage = voucherModel.VoucherMessage,
             VoucherNumber = voucherModel.VoucherNumber,
             IsVoucherValid = voucherModel.IsVoucherValid,
             IsVoucherApplied = voucherModel.IsVoucherApplied,
             VoucherBalance = voucherModel.VoucherBalance,
             VoucherAmountUsed = voucherModel.VoucherAmountUsed,
             VoucherName = voucherModel.VoucherName,
             ExpirationDate = voucherModel.ExpirationDate,
             CultureCode = voucherModel.CultureCode,
             PortalId = voucherModel.PortalId,
             IsExistInOrder = voucherModel.IsExistInOrder,
             UserId = voucherModel.UserId,
             OrderVoucherAmount = voucherModel.OrderVoucherAmount
         };

        public static VoucherModel ToVoucherModel(ZnodeVoucher znodeVoucher)
            => HelperUtility.IsNull(znodeVoucher) ? new VoucherModel() :
             new VoucherModel
             {
                 VoucherMessage = znodeVoucher.VoucherMessage,
                 VoucherNumber = znodeVoucher.VoucherNumber,
                 IsVoucherValid = znodeVoucher.IsVoucherValid,
                 IsVoucherApplied = znodeVoucher.IsVoucherApplied,
                 VoucherBalance = znodeVoucher.VoucherBalance,
                 VoucherAmountUsed = znodeVoucher.VoucherAmountUsed,
                 VoucherName = znodeVoucher.VoucherName,
                 ExpirationDate = znodeVoucher.ExpirationDate,
                 CultureCode = znodeVoucher.CultureCode,
                 PortalId = znodeVoucher.PortalId,
                 IsExistInOrder = znodeVoucher.IsExistInOrder,
                 UserId = znodeVoucher.UserId,
                 OrderVoucherAmount = znodeVoucher.OrderVoucherAmount
             };
    }
}
