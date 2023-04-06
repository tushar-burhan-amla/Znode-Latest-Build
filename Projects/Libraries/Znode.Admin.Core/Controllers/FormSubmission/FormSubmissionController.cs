using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;

namespace Znode.Engine.Admin.Controllers
{
    public class FormSubmissionController : BaseController
    {
        #region Private Readonly members
        private readonly IFormSubmissionAgent _formSubmissionAgent;
        #endregion

        #region Constructor

        public FormSubmissionController(IFormSubmissionAgent formSubmissionAgent)
        {
            _formSubmissionAgent = formSubmissionAgent;
        }

        #endregion

        #region Public Methods

        // Get list of form submission.
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeFormSubmissionList.ToString(), model);
            FormSubmissionListViewModel formSubmissionList = _formSubmissionAgent.GetFormSubmissionList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            formSubmissionList.GridModel = FilterHelpers.GetDynamicGridModel(model, formSubmissionList.FormSubmissionList, GridListType.ZnodeFormSubmissionList.ToString(), string.Empty, null, true, true, formSubmissionList?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            formSubmissionList.GridModel.TotalRecordCount = formSubmissionList.TotalResults;

            return ActionView(formSubmissionList);
        }

        //Method to view form submit details.
        public virtual ActionResult View(int formBuilderSubmitId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            return ActionView(_formSubmissionAgent.GetFormSubmitDetails(formBuilderSubmitId));
        }
        #endregion
    }
}
