using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Admin;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Observer;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public class GiftCardService : BaseService, IGiftCardService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeGiftCard> _giftCardRepository;
        private readonly IZnodeRepository<ZnodeRmaConfiguration> _rmaConfigurationRepository;
        private readonly IZnodeRepository<ZnodePortal> _portalRepository;
        private readonly IZnodeRepository<ZnodeGiftCardHistory> _giftCardHistoryRepository;
        #endregion

        #region Constructor
        public GiftCardService()
        {
            _giftCardRepository = new ZnodeRepository<ZnodeGiftCard>();
            _rmaConfigurationRepository = new ZnodeRepository<ZnodeRmaConfiguration>();
            _portalRepository = new ZnodeRepository<ZnodePortal>();
            _giftCardHistoryRepository = new ZnodeRepository<ZnodeGiftCardHistory>();


        }
        #endregion

        #region Public Methods
        //Create new GiftCard.
        public virtual GiftCardModel CreateGiftCard(GiftCardModel giftCardModel)
        {
            ZnodeLogging.LogMessage("CreateGiftCard method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNull(giftCardModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.GiftCardModelNotNull);

            ZnodeGiftCard giftCard = _giftCardRepository.Insert(giftCardModel.ToEntity<ZnodeGiftCard>());
            ZnodeLogging.LogMessage("Inserted giftCard with id ", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Verbose, giftCard?.GiftCardId);

            //Check gift card to be created is for Referral Commission Payment or not.
            if (giftCardModel.IsReferralCommission)
                //Check if amount paying to customer is greater than amount to be paid throws an exception.
                CheckAmount(giftCardModel);
            else if (giftCardModel.UserId > 0 && giftCardModel.SendMail)
            {
                RMAConfigurationModel rmaConfigurationModel = _rmaConfigurationRepository.GetEntity(string.Empty)?.ToModel<RMAConfigurationModel>();
                giftCardModel.NotificationSentToCustomer = rmaConfigurationModel?.GcNotification;
                //If gift card is created for customer and is not referral, then mail will be sent to customer.
                SendMailToCustomer(giftCardModel);
                var onVoucherIssueInit = new ZnodeEventNotifier<GiftCardModel>(giftCardModel, EventConstant.OnIssueVoucher);
            }

            ZnodeLogging.LogMessage(IsNotNull(giftCard) ? String.Format(Admin_Resources.SuccessCreateGiftCard, giftCardModel.CardNumber) : Admin_Resources.ErrorCreateGiftCard, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            if (IsNotNull(giftCard))
                return giftCard.ToModel<GiftCardModel>();
            ZnodeLogging.LogMessage("CreateGiftCard method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return giftCardModel;
        }

        //Update Gift Card.
        public virtual bool UpdateGiftCard(GiftCardModel giftCardModel)
        {
            ZnodeLogging.LogMessage("UpdateGiftCard method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNull(giftCardModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.GiftCardModelNotNull);

            if (giftCardModel.GiftCardId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.IdCanNotBeLessThanOne);
            decimal remainingAmount = _giftCardRepository?.Table?.FirstOrDefault(x => x.CardNumber.ToLower().Equals(giftCardModel.CardNumber.ToLower()))?.RemainingAmount.GetValueOrDefault()?? 0 ;
            //Update gift card.
            bool isGiftCardUpdated = _giftCardRepository.Update(giftCardModel.ToEntity<ZnodeGiftCard>());

            if (isGiftCardUpdated )
            {
                AddVoucherHistoryNotes(giftCardModel, remainingAmount);
                if (giftCardModel.UserId > 0)
                {
                    RMAConfigurationModel rmaConfigurationModel = _rmaConfigurationRepository.GetEntity(string.Empty)?.ToModel<RMAConfigurationModel>();
                    giftCardModel.NotificationSentToCustomer = rmaConfigurationModel?.GcNotification;

                    if (giftCardModel.SendMail && giftCardModel.IsActive)
                    {
                        //If gift card is created for customer and is not referral, then mail will be sent to customer.
                        SendMailToCustomer(giftCardModel);
                        var onIssueVoucherInit = new ZnodeEventNotifier<GiftCardModel>(giftCardModel, EventConstant.OnIssueVoucher);

                    }
                }

            }

            ZnodeLogging.LogMessage(isGiftCardUpdated ? string.Format(Admin_Resources.SuccessUpdateGiftCard, giftCardModel.CardNumber) : string.Format(Admin_Resources.ErrorUpdateWarehouse, giftCardModel.CardNumber), ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("UpdateGiftCard method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return isGiftCardUpdated;
        }

        //Add voucher history notes for updated remaining Amount.
        protected virtual void AddVoucherHistoryNotes(GiftCardModel giftCardModel, decimal remainingAmount)
        {
            if (remainingAmount != giftCardModel.RemainingAmount)
            {
                ZnodeGiftCardHistory history = new ZnodeGiftCardHistory();
                history.TransactionDate = GetDate();
                history.OmsOrderDetailsId = null;
                history.GiftCardId = giftCardModel.GiftCardId;
                history.RemainingAmount = giftCardModel.RemainingAmount;
                history.Notes = String.Format(Admin_Resources.VoucherHistoryAmountUpdatedNotes.ToString(), ZnodeCurrencyManager.FormatPriceWithCurrency(remainingAmount, giftCardModel.CultureCode), ZnodeCurrencyManager.FormatPriceWithCurrency(Convert.ToDecimal(giftCardModel.RemainingAmount), giftCardModel.CultureCode));
                _giftCardHistoryRepository.Insert(history);
            }
        }

        //Get GiftCard by gift card id.
        public virtual GiftCardModel GetGiftCard(int giftCardId)
        {
            ZnodeLogging.LogMessage("GetGiftCard method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (giftCardId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.IdCanNotBeLessThanOne);
            //This method check the access of manage screen for sales rep user
            ValidateAccessForSalesRepUser(ZnodeConstant.OrderVoucher, giftCardId);
            ZnodeGiftCard giftCardEntity = _giftCardRepository.GetEntity(GetWhereClause(giftCardId), GetNavigationProperties());
            GiftCardModel giftCard = giftCardEntity.ToModel<GiftCardModel>();
            ZnodeRepository<ZnodeAccount> userAccount = new ZnodeRepository<ZnodeAccount>();
            if (IsNotNull(giftCard))
            {
                if (IsNotNull(giftCardEntity.ZnodeUser))
                {
                    giftCard.CustomerName = GetCustomerName(giftCardEntity.ZnodeUser.FirstName, giftCardEntity.ZnodeUser.LastName, giftCardEntity.ZnodeUser.Email);                  
                    giftCard.AccountName = userAccount.Table.FirstOrDefault(x => x.AccountId == giftCardEntity.ZnodeUser.AccountId)?.Name;
                    giftCard.AccountCode = userAccount.Table.FirstOrDefault(x => x.AccountId == giftCardEntity.ZnodeUser.AccountId)?.AccountCode;
                }
                giftCard.StoreName = _portalRepository.Table.FirstOrDefault(x => x.PortalId == giftCard.PortalId)?.StoreName;
            }
            ZnodeLogging.LogMessage("GetGiftCard method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return giftCard;
        }

        //Get Voucher by voucher Code.
        public virtual GiftCardModel GetVoucher(string voucherCode)
        {
            ZnodeLogging.LogMessage("GetVoucher method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (string.IsNullOrEmpty(voucherCode))
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Api_Resources.VoucherCodeCanNotBeEmpty);

            int? giftCardId  = _giftCardRepository?.Table?.FirstOrDefault(x => x.CardNumber.ToLower().Equals(voucherCode.ToLower()))?.GiftCardId;

            if (giftCardId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Api_Resources.VoucherCodeIsInValidMessage);
            
            return GetGiftCard(giftCardId.GetValueOrDefault()); 
        }

        //Get Customer Name
        protected virtual string GetCustomerName(string firstName, string lastName, string username)
        {
            if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
                return $"{username} | {firstName} {lastName}";
            else
                return username;
        }

        //Build WhereClause with giftCardId
        protected virtual string GetWhereClause(int giftCardId)
        {
            FilterCollection filters = new FilterCollection() { new FilterTuple(ZnodeGiftCardEnum.GiftCardId.ToString(), ProcedureFilterOperators.Equals, giftCardId.ToString()) };
            return DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause;
        }

        //Set navigation properties
        protected virtual List<string> GetNavigationProperties()
        {
            List<string> navigationProperty = new List<string>();
            SetExpands(ZnodeGiftCardEnum.ZnodeUser.ToString(), navigationProperty);
            return navigationProperty;
        }

        //Get paged Gift Card list.
        public virtual GiftCardListModel GetGiftCardList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("GetGiftCardList method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            //Get Authorized Portal Access for login User.
            int userId = 0; string portalAccess = string.Empty;
            userId = Convert.ToInt32(filters?.Find(x => string.Equals(x.FilterName, ZnodeGiftCardEnum.UserId.ToString(), StringComparison.CurrentCultureIgnoreCase))?.Item3);
            portalAccess = GetAvailablePortals(userId);

            string expirationDate = filters?.Find(x => string.Equals(x.FilterName, ZnodeGiftCardEnum.ExpirationDate.ToString(), StringComparison.CurrentCultureIgnoreCase))?.Item3;
            filters?.RemoveAll(x => string.Equals(x.FilterName, ZnodeGiftCardEnum.ExpirationDate.ToString(), StringComparison.CurrentCultureIgnoreCase));

            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("SP parameter values: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            GiftCardListModel listModel = new GiftCardListModel();
            IZnodeViewRepository<GiftCardModel> objStoredProc = new ZnodeViewRepository<GiftCardModel>();

            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowsCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@PortalId", portalAccess, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@ExpirationDate", string.IsNullOrEmpty(expirationDate) ? string.Empty : expirationDate, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@SalesRepUserId", HelperMethods.GetSalesRepUserId(), ParameterDirection.Input, DbType.Int32);

            IList<GiftCardModel> giftCardList = objStoredProc.ExecuteStoredProcedureList("Znode_GetGiftCardList  @WhereClause,@Rows,@PageNo,@Order_By,@RowsCount OUT,@PortalId,@ExpirationDate,@SalesRepUserId", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("Gift card list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, giftCardList?.Count());
            listModel.GiftCardList = giftCardList?.Count > 0 ? giftCardList.ToList() : new List<GiftCardModel>();

            //If filter contains IsReferralCommission, Set true flag for ReferralCommissionCount, if referral commission count is greater than zero. 
            if (filters.Any(x => string.Equals(x.FilterName, ZnodeGiftCardEnum.IsReferralCommission.ToString(), StringComparison.CurrentCultureIgnoreCase)))
                SetReferralCommissionCountFlag(listModel, filters);

            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("GetGiftCardList method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Delete GiftCard  by giftCardId.
        public virtual bool DeleteGiftCard(ParameterModel giftCardId)
        {
            ZnodeLogging.LogMessage("DeleteGiftCard method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            bool status = false;
            if (giftCardId.Ids.Count() < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.IdCanNotBeLessThanOne);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeGiftCardEnum.GiftCardId.ToString(), ProcedureFilterOperators.In, giftCardId.Ids.ToString()));

            IZnodeRepository<ZnodeRmaRequestItem> _rmaRequestItem = new ZnodeRepository<ZnodeRmaRequestItem>();
            _rmaRequestItem.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);
            ZnodeLogging.LogMessage("GenerateDynamicWhereClause method call with parameter : filters.ToFilterDataCollection() ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, filters.ToFilterDataCollection());
            status = _giftCardRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);
            ZnodeLogging.LogMessage(status ? String.Format(Admin_Resources.SuccessDeleteGiftCard, giftCardId.Ids) : String.Format(Admin_Resources.ErrorDeleteWarehouse, giftCardId.Ids), ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("DeleteGiftCard method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return status;
        }

        //Delete Voucher by voucherCode.
        public virtual bool DeleteVoucher(ParameterModel voucherCodes)
        {
            ZnodeLogging.LogMessage("DeleteVoucher method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (string.IsNullOrEmpty(voucherCodes.Ids))
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Api_Resources.VoucherCodeCanNotBeEmpty);

            ParameterModel giftCardId = new ParameterModel();

            var giftCardIds = _giftCardRepository?.Table?.Where(x => voucherCodes.Ids.ToLower().Contains(x.CardNumber.ToLower()))?.Select(x => x.GiftCardId.ToString())?.ToList();

            if (giftCardIds.Count > 0)
                giftCardId.Ids = String.Join(",", giftCardIds);

            if (IsNull(giftCardId.Ids) || giftCardId.Ids?.Count() < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Api_Resources.VoucherCodeIsInValidMessage);

            return DeleteGiftCard(giftCardId);
        }

        //Create new random card number.
        public virtual string GetRandomCardNumber() => GenerateRandomNumber.GetNextGiftCardNumber();


        // Activate Deactivate Vouchers
        public virtual bool ActivateDeactivateVouchers(ParameterModel voucherId, bool isActive)
        {
            ZnodeLogging.LogMessage(" Activate DiActivate Voucher method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            bool status = false;
            if (IsNotNull(voucherId) && voucherId.Ids?.Count() < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.IdCanNotBeLessThanOne);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeGiftCardEnum.GiftCardId.ToString(), ProcedureFilterOperators.In, voucherId.Ids.ToString()));


            List<ZnodeGiftCard> znodeGiftCards = _giftCardRepository.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause).ToList();
            ZnodeLogging.LogMessage("VoucherList count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, znodeGiftCards?.Count);

            if (znodeGiftCards?.Count > 0)
            {
                znodeGiftCards.ForEach(c => c.IsActive = isActive);
                //Update multiple records.
                status = _giftCardRepository.BatchUpdate(znodeGiftCards);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return status;

        }

        // Activate Deactivate Vouchers
        public virtual bool ActivateDeactivateVouchersByVoucherCode(ParameterModel voucherCodes, bool isActive)
        {
            ZnodeLogging.LogMessage("ActivateDeactivateVouchersByVoucherCode method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (string.IsNullOrEmpty(voucherCodes.Ids))
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Api_Resources.VoucherCodeCanNotBeEmpty);

            ParameterModel giftCardId = new ParameterModel();
            
            var giftCardIds = _giftCardRepository?.Table?.Where(x => voucherCodes.Ids.ToLower().Contains(x.CardNumber.ToLower()))?.Select(x => x.GiftCardId.ToString())?.ToList();

            if(giftCardIds.Count > 0)
               giftCardId.Ids = String.Join(",", giftCardIds);

            if (IsNull(giftCardId.Ids) || giftCardId.Ids?.Count() < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Api_Resources.VoucherCodeIsInValidMessage);

            return ActivateDeactivateVouchers(giftCardId, isActive);
        }
        
        //Get paged Gift Card history list.
        public virtual GiftCardHistoryListModel GetGiftCardHistoryList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            if (IsNull(filters) || filters?.Count() < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorFiltersEmpty);

            ZnodeLogging.LogMessage("GetGiftCardHistoryList method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            int giftCardId = 0; int userId = 0;
            giftCardId = Convert.ToInt32(filters?.Find(x => string.Equals(x.FilterName, ZnodeGiftCardEnum.GiftCardId.ToString(), StringComparison.CurrentCultureIgnoreCase))?.Item3);
            userId = Convert.ToInt32(filters?.Find(x => string.Equals(x.FilterName, ZnodeGiftCardEnum.UserId.ToString(), StringComparison.CurrentCultureIgnoreCase))?.Item3);

            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("SP parameter values: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            GiftCardHistoryListModel listModel = new GiftCardHistoryListModel();
            IZnodeViewRepository<GiftCardHistoryModel> objStoredProc = new ZnodeViewRepository<GiftCardHistoryModel>();

            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowsCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            IList<GiftCardHistoryModel> giftCardList = objStoredProc.ExecuteStoredProcedureList("Znode_GetGiftCardHistoryList  @WhereClause,@Rows,@PageNo,@Order_By,@RowsCount OUT", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("Gift card history list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, giftCardList?.Count());
            listModel.GiftCardHistoryList = giftCardList?.Count > 0 ? giftCardList.ToList() : new List<GiftCardHistoryModel>();

            filters.Clear();
            filters.Add(ZnodeGiftCardEnum.GiftCardId.ToString(), FilterOperators.Equals, giftCardId.ToString());
            if (userId > 0)
                filters.Add(ZnodeUserEnum.UserId.ToString(), FilterOperators.Equals, userId.ToString());

            listModel.GiftCard = _giftCardRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection())?.WhereClause, GetNavigationProperties())?.ToModel<GiftCardModel>();
            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("GetGiftCardHistoryList method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return listModel;

        }

        // Send the voucher expiration reminder email.
        public virtual bool SendVoucherExpirationReminderEmail()
        {
            ZnodeLogging.LogMessage(" Send voucher expiration reminder email method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            bool status = true;
            List<GiftCardModel> znodeGiftCards = _giftCardRepository.Table.Where(x => x.IsActive == true && x.RemainingAmount > 0 && x.UserId > 0 && x.ExpirationDate >= DateTime.Today.Date)?.ToModel<GiftCardModel>().ToList();
            if (IsNull(znodeGiftCards))
                return false;

            var portalsAttribute = GetVoucherPortalWithAttributeValue(znodeGiftCards);

            IZnodeRepository<ZnodeUser> _userRepository = new ZnodeRepository<ZnodeUser>();

            IEnumerable<int?> userIdsIds = znodeGiftCards.Select(x => x.UserId)?.Distinct();

            List<ZnodeUser> znodeUsers = _userRepository.Table.Where(x => userIdsIds.Contains(x.UserId))?.ToList();

            List<int?> accountIds = znodeUsers.Select(x => x.AccountId)?.Distinct().ToList();

            List<GlobalAttributeValuesModel> accountAttributevalueModel = GetvoucherAccountWithAttributeValue(accountIds);

            if (portalsAttribute.Count > 0)
            {
                foreach (var portalsAttributeValue in portalsAttribute)
                {
                    EmailTemplateMapperModel emailTemplateMapperModel = GetEmailTemplateByCode(ZnodeConstant.VoucherExpirationReminder, portalsAttributeValue.PortalId);

                    List<GiftCardModel> giftCardPortal = znodeGiftCards.Where(x => x.PortalId == portalsAttributeValue.PortalId)?.ToList();

                    if (giftCardPortal?.Count > 0)
                    {
                        foreach (GiftCardModel znodeGiftCard in giftCardPortal)
                        {
                            int accountId = 0;
                            ZnodeUser user = znodeUsers.FirstOrDefault(x => x.UserId == znodeGiftCard.UserId);
                            if (IsNotNull(user))
                                accountId = user.AccountId.GetValueOrDefault();

                            string attributeValueforStore = !string.IsNullOrEmpty(portalsAttributeValue.AttributeValue) ? portalsAttributeValue.AttributeValue : portalsAttributeValue.AttributeDefaultValueCode;
                            if (accountId > 0)
                            {
                                string attributeValueforAccount = (accountAttributevalueModel?.FirstOrDefault(x => x.GlobalEntityId == accountId)?.AttributeValue);

                                if (!string.IsNullOrEmpty(attributeValueforAccount))
                                {
                                    CheckAndSendExpirationReminderEmail(znodeGiftCard.ExpirationDate, GetVoucherExpirationDate(Convert.ToInt32(attributeValueforAccount)), znodeGiftCard, emailTemplateMapperModel, portalsAttributeValue.StoreName, user);
                                }
                                else
                                {
                                    CheckAndSendExpirationReminderEmail(znodeGiftCard.ExpirationDate, GetVoucherExpirationDate(Convert.ToInt32(attributeValueforStore)), znodeGiftCard, emailTemplateMapperModel, portalsAttributeValue.StoreName, user);
                                }

                            }
                            else
                            {
                                CheckAndSendExpirationReminderEmail(znodeGiftCard.ExpirationDate, GetVoucherExpirationDate(Convert.ToInt32(attributeValueforStore)), znodeGiftCard, emailTemplateMapperModel, portalsAttributeValue.StoreName, user);
                            }
                        }
                    }
                    ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                }
            }
            return status;
        }

        #endregion

        #region Private Method

        // Check expiration date with global setting expiration date and send reminder email.
        protected virtual void CheckAndSendExpirationReminderEmail(DateTime? voucherExpirationDate, DateTime globalSettingExpirationDate, GiftCardModel giftCardModel, EmailTemplateMapperModel emailTemplateMapperModel, string storeName, ZnodeUser user)
        {
            if (voucherExpirationDate == globalSettingExpirationDate)
            {
                SendVoucherExpirationReminderEmail(giftCardModel, emailTemplateMapperModel, storeName, user);
                var onVoucherExpirationReminderInit = new ZnodeEventNotifier<GiftCardModel>(giftCardModel, EventConstant.OnVoucherExpirationReminder);
            }
        }

        //Set true flag for ReferralCommissionCount, if referral commission count is greater than zero. 
        protected virtual void SetReferralCommissionCountFlag(GiftCardListModel listModel, FilterCollection filters)
        {
            string userId = filters.Find(x => string.Equals(x.FilterName, ZnodeUserEnum.UserId.ToString(), StringComparison.CurrentCultureIgnoreCase))?.FilterValue;

            FilterCollection referralCommissionFilter = new FilterCollection();
            referralCommissionFilter.Add(ZnodeUserEnum.UserId.ToString(), FilterOperators.Equals, userId);

            IZnodeRepository<ZnodeOmsReferralCommission> _referralCommission = new ZnodeRepository<ZnodeOmsReferralCommission>();
            listModel.ReferralCommissionCount = _referralCommission.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(referralCommissionFilter.ToFilterDataCollection()).WhereClause).Any();
        }

        //Check if amount paying to customer is greater than amount to be paid throws an exception.
        protected virtual void CheckAmount(GiftCardModel giftCardModel)
        {
            IZnodeRepository<View_CustomerReferralCommissionDetail> _viewCustomerReferralCommissionDetail = new ZnodeRepository<View_CustomerReferralCommissionDetail>();

            //Get list of owed amount and calculate its total.
            giftCardModel.OwedAmount = _giftCardRepository.Table.Where(x => x.UserId == giftCardModel.UserId && x.IsReferralCommission == true)?.Select(x => x.Amount)?.ToList()?.Sum();
            ZnodeLogging.LogMessage("Gift Card Owed Amount:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, giftCardModel.OwedAmount);
            //Get list of order commission amount and calculate its total.
            decimal? totalCommission = _viewCustomerReferralCommissionDetail.Table.Where(x => x.UserId == giftCardModel.UserId)?.Select(x => x.OrderCommission)?.ToList()?.Sum();
            ZnodeLogging.LogMessage("Gift Card Total Commission:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, totalCommission);
            //Check if sum of owned amount and newly entered amount is greater than total commission. 
            if ((giftCardModel.OwedAmount + giftCardModel.Amount) > totalCommission)
            {
                //Left amount is amount left to pay to customer.
                giftCardModel.LeftAmount = totalCommission - giftCardModel.OwedAmount;

                //If sum of owned amount and newly entered amount is greater than amount to be paid,throws an exception.
                throw new ZnodeException(ErrorCodes.InvalidData, $"{Admin_Resources.PaymentGreaterErrorMessage} { Math.Round(giftCardModel.LeftAmount.GetValueOrDefault(), Convert.ToInt32(DefaultGlobalConfigSettingHelper.GetDefaultGlobalConfigSettings()?.DefaultGlobalConfigs.Where(x => x.FeatureName == GlobalSettingEnum.PriceRoundOff.ToString())?.Select(x => x.FeatureValues)?.FirstOrDefault()))}");
            }
        }

        //This method will send gift card mail to customer.        
        protected virtual void SendMailToCustomer(GiftCardModel giftCardModel)
        {
            if (IsNotNull(giftCardModel))
            {
                EmailTemplateMapperModel emailTemplateMapperModel = GetEmailTemplateByCode(ZnodeConstant.IssueVoucher, giftCardModel.PortalId);

                if (IsNotNull(emailTemplateMapperModel))
                {
                    //Get default global config list.
                    List<DefaultGlobalConfigModel> defaultGlobalSettingData = GetDefaultGlobalSettingData();

                    string storeName = string.Empty;
                    //Get user details.
                    ZnodeUser userDetails = GetUser(giftCardModel.UserId);

                    if (giftCardModel.PortalId > 0)
                        storeName = _portalRepository.Table.Where(w => w.PortalId == giftCardModel.PortalId)?.FirstOrDefault()?.StoreName;
                    ZnodeLogging.LogMessage("Store Name:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, storeName);
                    string messageText = emailTemplateMapperModel.Descriptions;
                    string subject = $"{emailTemplateMapperModel?.Subject} - {giftCardModel?.Name}";
                    if (giftCardModel.TransactionAmount > 0)
                        giftCardModel.RemainingAmount = giftCardModel?.RemainingAmount + giftCardModel.TransactionAmount;

                    messageText = ReplaceTokenWithMessageText(ZnodeConstant.CustomerName, $" {userDetails?.FirstName} {userDetails?.LastName}", messageText);
                    messageText = ReplaceTokenWithMessageText(ZnodeConstant.GiftCardAmount, $"{FormatPriceWithCurrency(giftCardModel?.RemainingAmount, string.IsNullOrEmpty(giftCardModel.CultureCode) ? GetDefaultCulture(defaultGlobalSettingData) : giftCardModel.CultureCode, GetDefaultPriceRoundOff(defaultGlobalSettingData)).Replace("$", "$$").ToString()}", messageText);
                    messageText = ReplaceTokenWithMessageText(ZnodeConstant.GiftCardNumber, giftCardModel?.CardNumber.ToString(), messageText);
                    messageText = ReplaceTokenWithMessageText(ZnodeConstant.StoreLogo, GetCustomPortalDetails(giftCardModel.PortalId)?.StoreLogo, messageText);
                    messageText = ReplaceTokenWithMessageText(ZnodeConstant.FirstName, $" {userDetails?.FirstName}", messageText);
                    messageText = ReplaceTokenWithMessageText(ZnodeConstant.StoreName, $" {storeName}", messageText);
                    messageText = ReplaceTokenWithMessageText(ZnodeConstant.VoucherName, $" {giftCardModel?.Name.ToString()}", messageText);
                    messageText = ReplaceTokenWithMessageText(ZnodeConstant.StartDate, $" {Convert.ToDateTime(giftCardModel?.StartDate).ToTimeZoneDateFormat()}", messageText);

                    if (IsNotNull(giftCardModel?.ExpirationDate))
                        messageText = ReplaceTokenWithMessageText(ZnodeConstant.ExpirationDate, Convert.ToDateTime(giftCardModel?.ExpirationDate).ToTimeZoneDateFormat(), messageText);
                    else
                        messageText = ReplaceTokenWithMessageText(ZnodeConstant.ExpirationDateMessage, string.Empty, messageText);

                    ZnodeLogging.LogMessage("Parameter for SendEmail", ZnodeLogging.Components.OMS.ToString(),
                    TraceLevel.Verbose, new object[] { userDetails?.Email, subject, messageText, giftCardModel.PortalId });
                    //Send mail to customer.
                    SendEmail(userDetails?.Email, subject, messageText, giftCardModel.PortalId);
                }
            }
        }

        //Send Email.
        protected virtual void SendEmail(string email, string subject, string messageText, int portalId = 0)
        {
            ZnodeEmail.SendEmail(portalId, email, ZnodeConfigManager.SiteConfig.AdminEmail, string.Empty, subject, messageText, true);
        }

        //TO DO - in progress

        //Get default global config list.
        protected virtual List<DefaultGlobalConfigModel> GetDefaultGlobalSettingData()
        {
            IDefaultGlobalConfigService defaultGlobalConfigService = ZnodeDependencyResolver.GetService<IDefaultGlobalConfigService>();
            return defaultGlobalConfigService.GetDefaultGlobalConfigList()?.DefaultGlobalConfigs;
        }

        //Get default currency code.
        protected virtual string GetDefaultCulture(List<DefaultGlobalConfigModel> defaultGlobalSettingData)
           => defaultGlobalSettingData?.Where(x => string.Equals(x.FeatureName, GlobalSettingEnum.Culture.ToString(), StringComparison.CurrentCultureIgnoreCase))?.Select(x => x.FeatureValues)?.FirstOrDefault();

        //Get default price round off.
        protected virtual string GetDefaultPriceRoundOff(List<DefaultGlobalConfigModel> defaultGlobalSettingData)
            => defaultGlobalSettingData?.Where(x => string.Equals(x.FeatureName, GlobalSettingEnum.PriceRoundOff.ToString(), StringComparison.CurrentCultureIgnoreCase))?.Select(x => x.FeatureValues)?.FirstOrDefault();

        //Get default date format.
        protected virtual string GetDefaultDateFormat(List<DefaultGlobalConfigModel> defaultGlobalSettingData)
            => defaultGlobalSettingData?.Where(x => string.Equals(x.FeatureName, GlobalSettingEnum.DateFormat.ToString(), StringComparison.CurrentCultureIgnoreCase))?.Select(x => x.FeatureValues)?.FirstOrDefault();

        //For Price according to currency.
        protected virtual string FormatPriceWithCurrency(decimal? price, string CultureName, string defaultPriceRoundOff)
        {
            string currencyValue;
            if (IsNotNull(CultureName))
            {
                CultureInfo info = new CultureInfo(CultureName);
                info.NumberFormat.CurrencyDecimalDigits = Convert.ToInt32(defaultPriceRoundOff);
                currencyValue = $"{price.GetValueOrDefault().ToString("c", info.NumberFormat)}";
            }
            else
                currencyValue = Convert.ToString(price);

            return currencyValue;
        }

        //This method will send gift card mail to customer.        
        protected virtual void SendVoucherExpirationReminderEmail(GiftCardModel giftCardModel, EmailTemplateMapperModel emailTemplateMapperModel, string storeName, ZnodeUser userDetails)
        {
            if (IsNotNull(giftCardModel))
            {
                if (IsNotNull(emailTemplateMapperModel))
                {
                    //Get default global config list.
                    List<DefaultGlobalConfigModel> defaultGlobalSettingData = GetDefaultGlobalSettingData();
                    string messageText = emailTemplateMapperModel.Descriptions;
                    string subject = emailTemplateMapperModel?.Subject;
                    subject = ReplaceTokenWithMessageText(ZnodeConstant.VoucherName, $" {giftCardModel?.Name.ToString()}", subject);
                    subject = ReplaceTokenWithMessageText(ZnodeConstant.ExpirationDate, giftCardModel?.ExpirationDate?.ToString(GetDefaultDateFormat(defaultGlobalSettingData)), subject);

                    messageText = ReplaceTokenWithMessageText(ZnodeConstant.CustomerName, $" {userDetails?.FirstName} {userDetails?.LastName}", messageText);
                    messageText = ReplaceTokenWithMessageText(ZnodeConstant.StoreName, $" {storeName}", messageText);
                    messageText = ReplaceTokenWithMessageText(ZnodeConstant.VoucherName, $" {giftCardModel?.Name.ToString()}", messageText);
                    messageText = ReplaceTokenWithMessageText(ZnodeConstant.GiftCardNumber, giftCardModel?.CardNumber.ToString(), messageText);

                    if (IsNotNull(giftCardModel?.ExpirationDate))
                        messageText = ReplaceTokenWithMessageText(ZnodeConstant.ExpirationDate, giftCardModel?.ExpirationDate?.ToString(GetDefaultDateFormat(defaultGlobalSettingData)), messageText);
                    else
                        messageText = ReplaceTokenWithMessageText(ZnodeConstant.ExpirationDateMessage, string.Empty, messageText);

                    ZnodeLogging.LogMessage("Parameter for SendEmail", ZnodeLogging.Components.OMS.ToString(),
                    TraceLevel.Verbose, new object[] { userDetails?.Email, subject, messageText, giftCardModel.PortalId });
                    //Send mail to customer.
                    SendEmail(userDetails?.Email, subject, messageText, giftCardModel.PortalId);
                }
            }
        }

        protected virtual ZnodeUser GetUser(int? userId)
        {
            IZnodeRepository<ZnodeUser> _userRepository = new ZnodeRepository<ZnodeUser>();
            return _userRepository?.Table.FirstOrDefault(x => x.UserId == userId);
        }

        protected virtual DateTime GetVoucherExpirationDate(int voucherExpiredInDays)
        {
            return DateTime.Today.Date.AddDays(voucherExpiredInDays);
        }

        //Get Voucher portal with attribute value
        protected virtual dynamic GetVoucherPortalWithAttributeValue(List<GiftCardModel> znodeGiftCards)
        {          
            IEnumerable<int> portalIds = znodeGiftCards.Select(x => x.PortalId)?.Distinct();
            IZnodeRepository<ZnodePortalGlobalAttributeValue> znodeGAttributeValue = new ZnodeRepository<ZnodePortalGlobalAttributeValue>();
            IZnodeRepository<ZnodePortalGlobalAttributeValueLocale> znodeGAttributeValueLocale = new ZnodeRepository<ZnodePortalGlobalAttributeValueLocale>();
            IZnodeRepository<ZnodeGlobalAttribute> znodeGAttribute = new ZnodeRepository<ZnodeGlobalAttribute>();
            IZnodeRepository <ZnodeGlobalAttributeDefaultValue> znodeGAttributeDefaultValue = new ZnodeRepository<ZnodeGlobalAttributeDefaultValue>();
            IZnodeRepository<ZnodePortal> znodePortal = new ZnodeRepository<ZnodePortal>();
            return (from portal in znodePortal.Table
                                       join attributeValue in znodeGAttributeValue.Table on portal.PortalId equals attributeValue.PortalId
                                       join attributeValueLocale in znodeGAttributeValueLocale.Table on attributeValue.PortalGlobalAttributeValueId equals attributeValueLocale.PortalGlobalAttributeValueId
                                       join attribute in znodeGAttribute.Table on attributeValue.GlobalAttributeId equals attribute.GlobalAttributeId
                                       join defaultAttributeValue in znodeGAttributeDefaultValue.Table on attribute.GlobalAttributeId equals defaultAttributeValue.GlobalAttributeId
                    where attribute.AttributeCode == "VoucherExpirationReminderEmailInDays" && portalIds.Contains(portal.PortalId)
                                       select new
                                       {
                                           PortalId = portal.PortalId,
                                           AttributeValue = attributeValueLocale.AttributeValue,
                                           AttributeCode = attribute.AttributeCode,
                                           AttributeDefaultValueCode = defaultAttributeValue.AttributeDefaultValueCode,
                                           StoreName = portal.StoreName
                                       })?.ToList();
        }

        // Get Account portal with attribute value
        protected virtual List<GlobalAttributeValuesModel> GetvoucherAccountWithAttributeValue( List<int?> accountIds)
        {
            List<GlobalAttributeValuesModel> accountAttributevalueModel = new List<GlobalAttributeValuesModel>();
            if (accountIds?.Count > 0)
            {
                foreach (int? accountId in accountIds)
                {
                    if (accountId > 0)
                    {
                        List<GlobalAttributeValuesModel> globalLevelAttributeAccount = GetGlobalLevelAttributeList(accountId.Value, ZnodeConstant.Account);
                        string attributeValue = globalLevelAttributeAccount?.FirstOrDefault(x => string.Equals(x.AttributeCode, "AccountVoucherExpirationReminderEmailInDays", StringComparison.InvariantCultureIgnoreCase))?.AttributeValue;
                        GlobalAttributeValuesModel view_ReturnBoolean = new GlobalAttributeValuesModel()
                        {
                            AttributeValue = attributeValue,
                            GlobalEntityId = accountId.Value
                        };
                        accountAttributevalueModel.Add(view_ReturnBoolean);
                    }
                }
            }
            return accountAttributevalueModel;
        }

        #endregion
    }
}
