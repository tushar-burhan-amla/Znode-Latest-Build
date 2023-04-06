using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Services
{
    public class ECertService : BaseService, IECertService
    {
        #region Private Variables

        private readonly IZnodeRepository<ZnodeECertificate> _eCertificateRepository;
        private readonly IZnodeRepository<ZnodePimDownloadableProductKey> _pimDownloadableProductKeyRepository;
        private readonly IZnodeRepository<ZnodeOmsDownloadableProductKey> _omsDownloadableProductKeyRepository;
        private readonly IZnodeRepository<ZnodeOmsOrderLineItem> _omsOrderLineItemRepository;
        private readonly IZnodeRepository<ZnodeOmsOrderDetail> _omsOrderDetailRepository;

        #endregion

        #region Constructor
        public ECertService()
        {
            _eCertificateRepository = new ZnodeRepository<ZnodeECertificate>();
            _pimDownloadableProductKeyRepository = new ZnodeRepository<ZnodePimDownloadableProductKey>();
            _omsDownloadableProductKeyRepository = new ZnodeRepository<ZnodeOmsDownloadableProductKey>();
            _omsOrderLineItemRepository = new ZnodeRepository<ZnodeOmsOrderLineItem>();
            _omsOrderDetailRepository = new ZnodeRepository<ZnodeOmsOrderDetail>();

        }
        #endregion

        #region Public Methods 
        /// <summary>
        /// Get available eCertificate balance.
        /// </summary>
        /// <param name="eCertTotalModel">Object of ECertTotalModel to get the total</param>
        /// <returns></returns>
        public virtual ECertTotalBalanceModel GetAvailableECertBalance(ECertTotalModel eCertTotalModel)
        {
            ECertTotalBalanceModel eCertTotalBalanceModel;
            ZnodeLogging.LogMessage("PortalId to get ECertTotalBalanceModel: ", string.Empty, TraceLevel.Verbose, eCertTotalModel?.WidgetParameter?.PortalId);
            eCertTotalBalanceModel = GetECertBalanceData(eCertTotalModel?.WidgetParameter?.PortalId ?? 0);
            ZnodeLogging.LogMessage("ECertTotalBalanceModel: ", string.Empty, TraceLevel.Verbose, eCertTotalBalanceModel);
            return eCertTotalBalanceModel;
        }


        /// <summary>
        /// Get list of certificates associated to logged in user.
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="sorts"></param>
        /// <param name="page"></param>
        /// <returns>ECertificateListModel</returns>
        public virtual ECertificateListModel GetWalletECertificatesList(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            int portalId = Convert.ToInt32(filters?.FirstOrDefault(o => o.Item1?.ToLower() == ZnodePortalEnum.PortalId.ToString().ToLower() &&
                                                                        o.Item2?.ToLower() == ProcedureFilterOperators.Equals.ToLower())
                                                  ?.Item3 ?? "0");

            ZnodeLogging.LogMessage("portalId to get availableCertificates: ", string.Empty, TraceLevel.Verbose, portalId);
            List<ECertificateModel> availableCertificates = GetWalletECertificates(portalId, out pageListModel.TotalRowCount, filters, sorts, page)?.Where(o => o.Balance > 0)?.ToList() ?? new List<ECertificateModel>();

            ECertificateListModel eCertificateListModel = new ECertificateListModel
            {
                ECertificates = availableCertificates
            };
            ZnodeLogging.LogMessage("ECertificates list count: ", string.Empty, TraceLevel.Verbose, eCertificateListModel?.ECertificates?.Count);
            pageListModel.PagingStart = pageListModel.PagingStart > 0 ? pageListModel.PagingStart : 1;
            eCertificateListModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", string.Empty, TraceLevel.Info);
            return eCertificateListModel;
        }

        /// <summary>
        /// Add eCertificate balance to the wallet.
        /// </summary>
        /// <param name="eCertificateModel">eCertificate that needs to be inserted</param>
        /// <returns>Inserted eCertificate</returns>
        public virtual ECertificateModel AddECertificateToWallet(ECertificateModel eCertificateModel)
        => AddECertificateInWallet(eCertificateModel);

        #endregion

        #region Protected Methods


        /// <summary>
        /// Gets the list of ECertificateModel associated to the user.
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="rowsCount"></param>
        /// <param name="filters"></param>
        /// <param name="sorts"></param>
        /// <param name="page"></param>
        /// <returns>List of certificates associated to logged in user.</returns>
        protected virtual List<ECertificateModel> GetWalletECertificates(int portalId, out int rowsCount, FilterCollection filters = null, NameValueCollection sorts = null, NameValueCollection page = null)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            rowsCount = 0;
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = null;

            if (HelperUtility.IsNull(filters))
            {
                filters = new FilterCollection();
                filters.Add(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, $"{portalId}");
            }
            pageListModel = new PageListModel(filters, sorts, page);


            ExecuteSpHelper objStoredProc = new ExecuteSpHelper();

            //SP parameters
            objStoredProc.GetParameter("@WhereClause", pageListModel?.SPWhereClause ?? string.Empty, ParameterDirection.Input, SqlDbType.NVarChar);
            objStoredProc.GetParameter("@Rows", pageListModel?.PagingLength ?? Int32.MaxValue, ParameterDirection.Input, SqlDbType.Int);
            objStoredProc.GetParameter("@PageNo", pageListModel?.PagingStart ?? 1, ParameterDirection.Input, SqlDbType.Int);
            objStoredProc.GetParameter("@Order_By", pageListModel?.OrderBy ?? string.Empty, ParameterDirection.Input, SqlDbType.VarChar);
            objStoredProc.GetParameter("@RowsCount", pageListModel?.TotalRowCount ?? Int32.MaxValue, ParameterDirection.Output, SqlDbType.Int);
            objStoredProc.GetParameter("@UserId", GetLoginUserId(), ParameterDirection.Input, SqlDbType.Int);

            ZnodeLogging.LogMessage("pageListModel to set SP parameters: ", string.Empty, TraceLevel.Verbose, pageListModel?.ToDebugString());
            DataSet outDataSet = objStoredProc.GetSPResultInDataSet("Znode_ECertificateWalletDetails");
            List<ECertificateModel> ECertificateModelList = outDataSet?.Tables[0]
                                                            ?.AsEnumerable()
                                                            ?.Select(row => new ECertificateModel
                                                            {
                                                                CertificateKey = row.Field<string>("CertificateKey"),
                                                                IssuedByName = row.Field<string>("IssuedByName"),
                                                                Balance = row.Field<decimal>("BalanceAmount"),
                                                                ECertificateId = row.Field<int>("ECertificateId"),
                                                                IssuedAmt = row.Field<decimal>("IssuedAmount"),
                                                                IssuedCYMD = (row.Field<DateTime>("IssuedDate")).ToString("yyyyMMdd"),
                                                                LastUsedCYMD = (row.Field<DateTime>("LastUsedCYMD")).ToString("yyyyMMdd"),
                                                                Custom1 = row.Field<string>("Custom1"),
                                                                CertificateType = row.Field<string>("CertificateType"),
                                                                RedemptionApplied = row.Field<decimal>("RedemptionApplied")
                                                            })?.ToList();

            ZnodeLogging.LogMessage("ECertificateModelList count: ", string.Empty, TraceLevel.Verbose, ECertificateModelList?.Count);
            rowsCount = outDataSet?.Tables[0]
                                  ?.AsEnumerable()
                                  ?.Select(row => row.Field<int>("CountId"))?.FirstOrDefault() ?? 0;
            ZnodeLogging.LogMessage("Execution done.", string.Empty, TraceLevel.Info);
            return ECertificateModelList;
        }

        /// <summary>
        /// Gets the total balance e-certificate available for the logged in user.
        /// </summary>
        /// <param name="portalId">Portal id to be used</param>
        /// <returns>ECertTotalBalanceModel object having information about total available e-Certificate balance.</returns>
        public virtual ECertTotalBalanceModel GetECertBalanceData(int portalId)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            int totalRows = 0;
            List<ECertificateModel> availableCertificates = GetWalletECertificates(portalId, out totalRows);

            ZnodeLogging.LogMessage("Execution done.", string.Empty, TraceLevel.Info);
            return new ECertTotalBalanceModel
            {
                AvailableTotal = availableCertificates?.Sum(o => o.Balance) ?? 0,
                TraceMessage = $"Only znode DB being used in call."
            };
        }

        /// <summary>
        /// Check if key available in inventory. If 
        /// </summary>
        /// <param name="eCertificateModel">eCertificateModel whose inventory needs to be verified.</param>
        /// <returns>bool value of key availability.</returns>
        public virtual bool IsKeyAvailableInInventory(ECertificateModel eCertificateModel)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            bool successResult = true;

            //Check in Znode
            successResult = (_pimDownloadableProductKeyRepository.Table.Any(o => o.DownloadableProductKey == eCertificateModel.CertificateKey));
            ZnodeLogging.LogMessage("Execution done.", string.Empty, TraceLevel.Info);
            return successResult;
        }

        /// <summary>
        /// Bind the mandatory data required for inserting eCertificate.
        /// </summary>
        /// <param name="eCertificateModel">ECertificateModel being inserted</param>
        /// <param name="loggedInUserId">logged in user id</param>
        /// <param name="eCertificate">ZnodeECertificate entity being inserted</param>
        public virtual void BindCertificateData(ECertificateModel eCertificateModel, int loggedInUserId, ZnodeECertificate eCertificate)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            //Bind with znode data
            ZnodeLogging.LogMessage("ECertificateModel with Id to get OMS line item list: ", string.Empty, TraceLevel.Verbose, eCertificateModel?.ECertificateId);
            var omsLineItem = GetKeyOmsLineItems(eCertificateModel);
            if ((loggedInUserId > 0) && (omsLineItem?.Any() ?? false))
            {
                int issuedById = GetAssociatedUserId(eCertificateModel, omsLineItem);
                ZnodeLogging.LogMessage("issuedById value: ", string.Empty, TraceLevel.Verbose, issuedById);
                eCertificate.ZnodeECertificateWallets?.ToList()?.ForEach(o =>
                {
                    o.IssuedByUserId = issuedById;
                    o.BalanceAmount = omsLineItem?.FirstOrDefault().Price ?? 0;
                });
                eCertificate.IssuedAmount = omsLineItem?.FirstOrDefault().Price ?? 0;
            }
            ZnodeLogging.LogMessage("Execution done.", string.Empty, TraceLevel.Info);
        }


        /// <summary>
        /// Insert ECertificateModel into database. 
        /// </summary>
        /// <param name="eCertificateModel">ECertificateModel to be inserted.</param>
        /// <returns>inserted ECertificateModel.</returns>
        protected virtual ECertificateModel AddECertificateInWallet(ECertificateModel eCertificateModel)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            if (ValidateECertificateModel(eCertificateModel))
            {
                int loggedInUserId = GetLoginUserId();
                ZnodeLogging.LogMessage("ECertificateId and loggedInUserId: ", string.Empty, TraceLevel.Verbose, new object[] { eCertificateModel?.ECertificateId, loggedInUserId });
                ZnodeECertificate eCertificate = new ZnodeECertificate()
                {
                    CertificateKey = eCertificateModel.CertificateKey,
                    CertificateType = eCertificateModel.CertificateType,
                    Custom1 = eCertificateModel.Custom1,
                    Custom2 = eCertificateModel.Custom2,
                    Custom3 = eCertificateModel.Custom3,
                    Custom4 = eCertificateModel.Custom4,
                    IssuedAmount = eCertificateModel.IssuedAmt,
                    IssuedDate = DateTime.Now,
                    ZnodeECertificateWallets = new List<ZnodeECertificateWallet>() {
                              new ZnodeECertificateWallet() {
                                  IssuedToUserId = loggedInUserId,
                                  CreatedBy = loggedInUserId,
                                  BalanceAmount =  eCertificateModel.Balance
                              }
                          }

                };
                BindCertificateData(eCertificateModel, loggedInUserId, eCertificate);
                var returnModel = _eCertificateRepository.Insert(eCertificate).ToModel<ECertificateModel>();
                returnModel.Balance = eCertificateModel.Balance;
            }
            ZnodeLogging.LogMessage("Execution done.", string.Empty, TraceLevel.Info);
            return eCertificateModel;
        }

        /// <summary>
        /// Checks if given ECertificateModel data is valid or not.
        /// </summary>
        /// <param name="eCertificateModel">ECertificateModel being validated</param>
        /// <returns>bool result specifying ECertificateModel data is validation passed/failed.</returns>
        protected virtual bool ValidateECertificateModel(ECertificateModel eCertificateModel)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            bool successResult = true;
            ZnodeLogging.LogMessage("ECertificateModel with Id to check is key available in wallet and inventory: ", string.Empty, TraceLevel.Verbose, eCertificateModel?.ECertificateId);
            successResult = successResult && IsKeyAvailableInWallet(eCertificateModel);
            if (!successResult)
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.ErroreCertificateAddedToAccountBalance);

            successResult = successResult && IsKeyAvailableInInventory(eCertificateModel);
            if (!successResult)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidCertificateNumber);

            ZnodeLogging.LogMessage("Execution done.", string.Empty, TraceLevel.Info);
            return successResult;
        }

        /// <summary>
        /// Check if key available wallets
        /// </summary>
        /// <param name="eCertificateModel">ECertificateModel which needs to be checked.</param>
        /// <returns>bool success value specifying availability of ECertificate in the wallets. </returns>
        protected virtual bool IsKeyAvailableInWallet(ECertificateModel eCertificateModel)
        => !(_eCertificateRepository.Table.Any(o => o.CertificateKey == eCertificateModel.CertificateKey));


        /// <summary>
        /// Gets IssuedByUserId using ECertificate key.
        /// </summary>
        /// <param name="eCertificateModel">ECertificateModel whose key needs to be traversed.</param>
        /// <param name="omsLineItems">List of ZnodeOmsOrderLineItem in which given key has appeared.</param>
        /// <returns>IssuedByUserId</returns>
        protected virtual int GetAssociatedUserId(ECertificateModel eCertificateModel, IEnumerable<ZnodeOmsOrderLineItem> omsLineItems = null)
        => (omsLineItems ?? GetKeyOmsLineItems(eCertificateModel)).Join(_omsOrderDetailRepository.Table, l => l.OmsOrderDetailsId, r => r.OmsOrderDetailsId, (l, r) => r)
           ?.FirstOrDefault()?.UserId
           ?? 0;

        /// <summary>
        /// Gets List of ZnodeOmsOrderLineItem in which given key has appeared.
        /// </summary>
        /// <param name="eCertificateModel">ECertificateModel whose details are needed.</param>
        /// <returns>List of ZnodeOmsOrderLineItem</returns>
        protected virtual IEnumerable<ZnodeOmsOrderLineItem> GetKeyOmsLineItems(ECertificateModel eCertificateModel)
        => _pimDownloadableProductKeyRepository.Table.Where(o => o.DownloadableProductKey == eCertificateModel.CertificateKey)
           ?.Join(_omsDownloadableProductKeyRepository.Table, l => l.PimDownloadableProductKeyId, r => r.PimDownloadableProductKeyId, (l, r) => r)
           ?.Join(_omsOrderLineItemRepository.Table, l => l.OmsOrderLineItemsId, r => r.OmsOrderLineItemsId, (l, r) => r);
        #endregion
    }
}
