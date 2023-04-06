using System;
using System.Text.RegularExpressions;
using System.Web;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Observer;

namespace Znode.Engine.Connector
{
    /// <summary>
    /// This class is use for adding terminals for connecting external application. 
    /// Using this class user can connect and insert data like external ERP, etc.
    /// User need to write specific terminal code and implemantation in it.
    /// </summary>
    public class GlobalConnector
    {
        #region Private Variables
        protected readonly EventAggregator eventAggregator;
        protected string _operationType;
        // Event listeners for the different entity
        protected Connector<ZnodeAddress> addressMessageToken;
        protected Connector<ZnodeOmsOrder> orderMessageToken;
        protected Connector<ZnodeUser> userMessageToken;
        protected Connector<string> deleteMessageToken;
        private const string defaultEvent = "DefaultEvent";
        #endregion

        #region Constructor
        public GlobalConnector(EventAggregator eve)
        {
            eventAggregator = eve;
            AttachEvents(eve, _operationType);
        }
        public GlobalConnector(EventAggregator eve, string operationType = defaultEvent) : this(eve)
        {
            _operationType = operationType;
        }
        private void AttachEvents(EventAggregator eve, string operationType)
        {
            addressMessageToken = eve.Attach<ZnodeAddress>(this.OnAddressUpdate, _operationType);
            orderMessageToken = eve.Attach<ZnodeOmsOrder>(this.OnPlaceOrder, _operationType);
            userMessageToken = eve.Attach<ZnodeUser>(this.OnProfileUpdate, _operationType);
            deleteMessageToken = eve.Attach<string>(this.OnDeleteRecord, _operationType);
        }
        #endregion

        #region  Methods

        /// <summary>
        /// This is method to Log activity for address insert 
        /// </summary>
        /// <param name="ZnodeAddress"> ZnodeAddress  entity </param>
        protected virtual void OnAddressUpdate(ZnodeAddress model)
        {
            LogImpersonationActivity(model.ModifiedBy, model.AddressId, "Address");
            eventAggregator.Detach(addressMessageToken);
        }

        /// <summary>
        /// This is method to Log activity for user profile data
        /// </summary>
        /// <param name="model"> ZnodeUser  entity</param>
        protected virtual void OnProfileUpdate(ZnodeUser model)
        {
            LogImpersonationActivity(model.UserId, model.UserId, "Profile");
            eventAggregator.Detach(userMessageToken);
        }

        /// <summary>
        /// This is sample method to understand implemantation of connector.
        /// </summary>
        /// <param name="model">ZnodeOmsOrder entity</param>
        protected virtual void OnPlaceOrder(ZnodeOmsOrder model)
        {
            LogImpersonationActivity(model.CreatedBy, model.OmsOrderId, "Order");
            eventAggregator.Detach(orderMessageToken);
        }

        /// <summary>
        /// This is sample method to understand implemantation of connector.
        /// </summary>
        /// <param name="model">ZnodeOmsOrder entity</param>
        protected virtual void OnDeleteRecord(string condition)
        {
            if (!string.IsNullOrEmpty(condition) && GetImpersonationId() > 0)
            {

                string[] conditionArray = Regex.Split(condition, "and");
                if (conditionArray[0].Split('=')[0].ToString().Trim() == "AddressId" && conditionArray.Length > 1)
                {
                    LogImpersonationActivity(Convert.ToInt32(conditionArray[1].Split('=')[1].ToString()), Convert.ToInt32(conditionArray[0].Split('=')[1].ToString()), "Address");
                }
            }
            eventAggregator.Detach(orderMessageToken);
        }
        #endregion

        protected void LogImpersonationActivity(int webstoreUserId, int activityId, string activityType)
        {
            int csrId = GetImpersonationId();
            if (csrId > 0)
            {
                int portalId= GetImpersonationPortalId();
                IZnodeRepository<ZnodeImpersonationLog> _impersonationLogRepository = new ZnodeRepository<ZnodeImpersonationLog>();
                _impersonationLogRepository.Insert(new ZnodeImpersonationLog { PortalId = portalId, CSRId = csrId, WebstoreuserId = webstoreUserId, ActivityId = activityId, ActivityType = activityType, OperationType = _operationType });
            }
        }

        #region private method
        //Get Impersonation CSR Id from Header
        protected int GetImpersonationId()
        {
            const string headerImpersonationCSRId = "Znode-ImpersonationCSRId";
            int impersonationCSRId = 0;
            var headers = HttpContext.Current.Request.Headers;
            int.TryParse(headers[headerImpersonationCSRId], out impersonationCSRId);
            return impersonationCSRId;
        }

        //Get Impersonation CSR Id from Header
        protected int GetImpersonationPortalId()
        {
            const string headerImpersonationCSRId = "Znode-ImpersonationPortalId";
            int portalId = 0;
            var headers = HttpContext.Current.Request.Headers;
            int.TryParse(headers[headerImpersonationCSRId], out portalId);
            return portalId;
        }
        #endregion
    }
}
