using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using Znode.Libraries.Framework.Business;
using System.Diagnostics;

namespace Znode.Engine.Services
{
    public class TagManagerService : BaseService, ITagManagerService
    {
        #region Private Variable
        private readonly IZnodeRepository<ZnodePortal> _portalRepository;
        private readonly IZnodeRepository<ZnodeGoogleTagManager> _googleTagManagerRepository;
        #endregion

        #region Public Constructor.
        public TagManagerService()
        {
            _portalRepository = new ZnodeRepository<ZnodePortal>();
            _googleTagManagerRepository = new ZnodeRepository<ZnodeGoogleTagManager>();
        }
        #endregion

        #region Public Methods
        //Get tag manager data for store.
        public virtual TagManagerModel GetTagManager(int portalId, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            List<string> navigationProperties = GetExpands(expands);

            EntityWhereClauseModel whereClauseModel = GetPortalIdWhereClause(portalId);

            ZnodeLogging.LogMessage("WhereClause to get ZnodeGoogleTagManager: ", string.Empty, TraceLevel.Verbose, whereClauseModel.WhereClause);
            ZnodeGoogleTagManager entity = _googleTagManagerRepository.GetEntity(whereClauseModel.WhereClause, navigationProperties);

            TagManagerModel tagManagerModel = entity?.ToModel<TagManagerModel>();

            if (IsNull(tagManagerModel))
                return new TagManagerModel { PortalId = portalId, PortalName = _portalRepository.Table.FirstOrDefault(x => x.PortalId == portalId)?.StoreName };

            ZnodeLogging.LogMessage("TagManagerModel with Id: ", string.Empty, TraceLevel.Verbose, tagManagerModel?.GoogleTagManagerId);
            tagManagerModel.PortalName = entity?.ZnodePortal?.StoreName;
            ZnodeLogging.LogMessage("Execution done.", string.Empty, TraceLevel.Info);
            return tagManagerModel;
        }

        //Save tag manager data for store.
        public virtual bool SaveTagManager(TagManagerModel tagManagerModel)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            if (IsNull(tagManagerModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorTagManagerModelNull);

            bool isUpdateStatus = true;

            //Update or insert tag manager data on the basis of GoogleTagManagerId.
            ZnodeLogging.LogMessage("TagManagerModel with Id: ", string.Empty, TraceLevel.Verbose, tagManagerModel?.GoogleTagManagerId);
            if (tagManagerModel.GoogleTagManagerId < 1)
                _googleTagManagerRepository.Insert(tagManagerModel.ToEntity<ZnodeGoogleTagManager>());
            else
                isUpdateStatus = _googleTagManagerRepository.Update(tagManagerModel.ToEntity<ZnodeGoogleTagManager>());

            //If tag manager details are updated successfully then return true else return false.
            ZnodeLogging.LogMessage("Execution done.", string.Empty, TraceLevel.Info);
            return isUpdateStatus;
        }
        #endregion

        #region Private Methods
        //Get whereclause for portal id.
        private static EntityWhereClauseModel GetPortalIdWhereClause(int portalId)
        {
            FilterCollection filter = new FilterCollection();
            filter.Add(ZnodeGoogleTagManagerEnum.PortalId.ToString(), FilterOperators.Equals, portalId.ToString());

            return DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection());
        }

        //Get expands and add them to navigation properties.
        private List<string> GetExpands(NameValueCollection expands)
        {
            List<string> navigationProperties = new List<string>();
            if (IsNotNull(expands) && expands.HasKeys())
            {
                foreach (string key in expands.Keys)
                    if (Equals(key, ZnodeGoogleTagManagerEnum.ZnodePortal.ToString().ToLower())) SetExpands(ZnodeGoogleTagManagerEnum.ZnodePortal.ToString(), navigationProperties);
            }
            return navigationProperties;
        }
        #endregion
    }
}
