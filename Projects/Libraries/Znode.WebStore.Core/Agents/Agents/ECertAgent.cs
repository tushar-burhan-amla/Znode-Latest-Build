using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.WebStore.Helpers;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.WebStore.Agents
{
    public class ECertAgent : BaseAgent, IECertAgent
    {
        #region Private Variables
        private readonly IECertClient _eCertClient;
        #endregion

        #region Constructor
        public ECertAgent(IECertClient eCertClient)
        {
            _eCertClient = GetClient<IECertClient>(eCertClient);
        }
        #endregion


        public virtual ECertificateListViewModel GetAvailableECertList(FilterCollectionDataModel filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
            //Set login user Id in client header to get profile, customer, account based pricing. 

            if (HelperUtility.IsNull(filters))
                filters = new FilterCollectionDataModel() { Filters = new FilterCollection() };
            filters.Filters?.Add(new FilterTuple(ZnodeUserEnum.UserId.ToString(), FilterOperators.Equals, GetUserId().ToString()));
            filters.Filters?.Add(new FilterTuple(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, PortalAgent.CurrentPortal.PortalId.ToString()));
            ZnodeLogging.LogMessage("Input parameters filters and sorts:", string.Empty, TraceLevel.Verbose, new { filters = filters, sortCollection = sortCollection });
            ECertificateListModel ecertList = _eCertClient.GetAvailableECertList(null, filters?.Filters, sortCollection, pageIndex, recordPerPage);
            ECertificateListViewModel ecertListViewModel = new ECertificateListViewModel { List = ecertList?.ECertificates?.ToViewModel<ECertificateViewModel>().ToList() };
            SetListPagingData(ecertListViewModel, ecertList);
            ZnodeLogging.LogMessage("Agent method executed.", string.Empty, TraceLevel.Info);
            return ecertListViewModel?.List?.Count > 0 ? ecertListViewModel : new ECertificateListViewModel();
        }


        //Add eCertificate to the wallet.
        public virtual ECertificateViewModel AddECertToBalance(ECertificateViewModel eCertificateViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
            ECertificateViewModel updatedECertificateModel;
            try
            {

                updatedECertificateModel = _eCertClient.AddECertToBalance(eCertificateViewModel.ToModel<ECertificateModel>())?.ToViewModel<ECertificateViewModel>() ?? new ECertificateViewModel();
                updatedECertificateModel.SuccessMessage = WebStore_Resources.SuccessECertAddedToBalance;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.InvalidData:
                    default:
                        {

                            updatedECertificateModel = (ECertificateViewModel)GetViewModelWithErrorMessage(eCertificateViewModel, WebStore_Resources.ECertInvalidDataErrorMessage);
                            break;
                        }
                    case ErrorCodes.AlreadyExist:
                        {

                            updatedECertificateModel = (ECertificateViewModel)GetViewModelWithErrorMessage(eCertificateViewModel, WebStore_Resources.ECertAlreadyExistErrorMessage);
                            break;
                        }
                    case ErrorCodes.NullModel:
                        {
                            updatedECertificateModel = eCertificateViewModel;
                            updatedECertificateModel.SuccessMessage = WebStore_Resources.SuccessECertZeroBalanceAdded;
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                updatedECertificateModel = (ECertificateViewModel)GetViewModelWithErrorMessage(eCertificateViewModel, WebStore_Resources.ECertInvalidDataErrorMessage);
            }
            ZnodeLogging.LogMessage("Agent method executed.", string.Empty, TraceLevel.Info);
            return updatedECertificateModel;
        }

    }
}
