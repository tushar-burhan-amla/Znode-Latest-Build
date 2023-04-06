using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Services
{
    public class CMSWidgetsService : BaseService, ICMSWidgetsService
    {
        #region Private Variable
        private readonly IZnodeRepository<ZnodeCMSWidget> _cmsWidgetsRepository;
        #endregion

        #region Constructor
        public CMSWidgetsService()
        {
            _cmsWidgetsRepository = new ZnodeRepository<ZnodeCMSWidget>();
        }
        #endregion

        #region Public Methods
        //Get CMS Widgets list.
        public virtual CMSWidgetsListModel List(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to get CMSWidgetsList: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            //Get the Widget List.
            CMSWidgetsListModel listModel = new CMSWidgetsListModel();
            listModel.CMSWidgetsList = _cmsWidgetsRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount)?.ToModel<CMSWidgetsModel>()?.ToList();
            ZnodeLogging.LogMessage("CMSWidgetsList count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, listModel.CMSWidgetsList?.Count());

            //Set for pagination
            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return listModel;
        }

        // Get Widget by codes.
        public virtual CMSWidgetsListModel GetWidgetByCodes(ParameterModel widgetCodes)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (HelperUtility.IsNull(widgetCodes) || string.IsNullOrEmpty(widgetCodes.Ids))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorCodesNull);
            ZnodeLogging.LogMessage("Input parameter widgetCodes value: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, widgetCodes);

            //Split the Widget Codes based on ",", to get the widget details.
            CMSWidgetsListModel cmsWidgetsListModel = new CMSWidgetsListModel();
            List<string> widgetsCodes = widgetCodes.Ids.Split(',').ToList();
            cmsWidgetsListModel.CMSWidgetsList = _cmsWidgetsRepository.Table.Where(x => widgetsCodes.Contains(x.Code))?.ToModel<CMSWidgetsModel>()?.ToList();
            ZnodeLogging.LogMessage("CMSWidgetsList count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, cmsWidgetsListModel.CMSWidgetsList?.Count());
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return cmsWidgetsListModel;
        }
        #endregion
    }
}
