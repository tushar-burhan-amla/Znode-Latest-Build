using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Znode.Engine.WebStore.Agents;
using Znode.Engine.WebStore.Helpers;
using Znode.Engine.WebStore.ViewModels;

namespace Znode.Engine.WebStore.Controllers
{
    public class ECertController : BaseController
    {
        #region Private Read-only members
        private readonly IUserAgent _userAgent;
        private readonly IECertAgent _eCertAgent;

        #endregion

        #region Public Constructor        
        public ECertController(IUserAgent userAgent, IECertAgent eCertAgent)
        {
            _userAgent = userAgent;
            _eCertAgent = eCertAgent;
        }
        #endregion

        #region Public Methods
        [Authorize]
        public virtual ActionResult AvailableECertList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //If session expires redirect to login page.
            if (string.IsNullOrEmpty(_userAgent.GetUserViewModelFromSession()?.RoleName))
                return RedirectToAction<UserController>(x => x.Login(string.Empty));

            ECertificateListViewModel list = _eCertAgent.GetAvailableECertList(model, model.SortCollection, model.Page, model.RecordPerPage);
            string cultureCode = PortalAgent.CurrentPortal.CultureCode;

            list.List?.ForEach(o =>
                        {
                            o.BalanceWithCurrency = Helper.FormatPriceWithCurrency(o.Balance, cultureCode);
                        });

            list.GridModel = FilterHelpers.GetDynamicGridModel(model, list?.List, WebStoreConstants.ZnodeMyECertificateList, string.Empty, null, true, true, list?.GridModel?.FilterColumn?.ToolMenuList);
            list.GridModel.TotalRecordCount = list.TotalResults;
            return ActionView("AvailableECertList", list);
        }


        [Authorize]
        public virtual ActionResult AddECertToBalance()
        {
            //If session expires redirect to login page.
            var loggedInUser = _userAgent.GetUserViewModelFromSession();
            if (string.IsNullOrEmpty(loggedInUser?.RoleName))
                return RedirectToAction<UserController>(x => x.Login(string.Empty));

            return ActionView("AddECertToBalance", new ECertificateViewModel());
        }


        [Authorize]
        [HttpPost]
        [OutputCache(Duration = 0)]
        public virtual ActionResult AddECertToBalance(ECertificateViewModel model)
        {
            //If session expires redirect to login page.
            var loggedInUser = _userAgent.GetUserViewModelFromSession();
            if (string.IsNullOrEmpty(loggedInUser?.RoleName))
                return RedirectToAction<UserController>(x => x.Login(string.Empty));

            if (ModelState.IsValid)
            {
                model = _eCertAgent.AddECertToBalance(model);

                if (!model.HasError)
                    SetNotificationMessage(GetSuccessNotificationMessage(model.SuccessMessage));
                else
                    SetNotificationMessage(GetErrorNotificationMessage(model.ErrorMessage));
            }

            return RedirectToAction("AvailableECertList");
        }
        #endregion
    }
}
