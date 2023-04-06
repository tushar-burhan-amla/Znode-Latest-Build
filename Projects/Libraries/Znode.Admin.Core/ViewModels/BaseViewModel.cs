using System;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using System.Collections.Generic;


namespace Znode.Engine.Admin.ViewModels
{
    public abstract class BaseViewModel
    {

        public string ErrorMessage { get; set; }

        public bool HasError { get; set; }

        public string SuccessMessage { get; set; }

        public string Title { get; set; }

        public int Page { get; set; }

        public int TotalResults { get; set; }

        public int TotalPages { get; set; }

        public int RecordPerPage { get; set; }

        public SortCollection SortCollection { get; set; }

        public FilterCollection Filters { get; set; }

        public ExpandCollection Expands { get; set; }

        public string Edit { get; set; }

        public string Delete { get; set; }


        public string Manage { get; set; }

        public string Image { get; set; }

        public string View { get; set; }

        public string Copy { get; set; }

        public string Checkbox { get; set; }

        public string Disable { get; set; }

        public string Control { get; set; }

        public string EditActionUrl { get; set; }

        public string DeleteActionUrl { get; set; }

        public string IsLink { get; set; }

        public string ManageActionUrl { get; set; }

        public string ImageActionUrl { get; set; }

        public string ViewActionUrl { get; set; }

        public string CopyActionUrl { get; set; }

        public string CheckboxActionUrl { get; set; }

        public string IsLinkActionUrl { get; set; }

        public string IsLinkFieldName { get; set; }

        public string EditParamField { get; set; }

        public string DeleteParamField { get; set; }

        public string EnableDisableActionUrl { get; set; }

        public string EnableDisableParamField { get; set; }

        public string ManageParamField { get; set; }

        public string ImageParamField { get; set; }

        public string ViewParamField { get; set; }

        public string CopyParamField { get; set; }

        public string CheckboxParamField { get; set; }

        public string ControlParamField { get; set; }

        public string IsLinkParamField { get; set; }

        public string DisplayTextEdit { get; set; }

        public string DisplayTextDelete { get; set; }

        public string DisplayTextView { get; set; }

        public string DisplayTextCopy { get; set; }

        public string DisplayTextManage { get; set; }

        public string ActionMode { get; set; }

        public List<SelectListItem> Locale { get; set; }

        public string Custom1 { get; set; }
        public string Custom2 { get; set; }
        public string Custom3 { get; set; }
        public string Custom4 { get; set; }
        public string Custom5 { get; set; }

        public string ReturnUrl
        {
            get
            {
                UrlHelper urlhelper = new UrlHelper();
                string returnUrl = string.Empty;
                if (!string.Equals(HttpContext.Current.Request.Url.ToString(), Convert.ToString(HttpContext.Current.Request.UrlReferrer), StringComparison.OrdinalIgnoreCase))
                {
                    SessionHelper.SaveDataInSession<string>("returnUrl", Convert.ToString(HttpContext.Current.Request.UrlReferrer));
                }
                return urlhelper.Encode(SessionHelper.GetDataFromSession<string>("returnUrl") ?? HttpContext.Current.Request.UrlReferrer.ToString());
            }
        }

        //public string DatePickDateFormat
        //{
        //    get
        //    {
        //        return Helpers.HelperMethods.DatePickDateFormat();
        //    }
        //}

        public object Clone()
        {
            var clonedObject = this.MemberwiseClone();
            return clonedObject;
        }

        public int CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public int ModifiedBy { get; set; }
        public string ModifiedDate { get; set; }
        public int? ErrorCode { get; set; }
    }

}

