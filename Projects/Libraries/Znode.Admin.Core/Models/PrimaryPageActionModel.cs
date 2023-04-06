using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Znode.Engine.Admin.ViewModels
{
    public class PrimaryPageActionModel : BaseViewModel
    {
        private List<PageAction> _pageAction;
        public List<PageAction> PageActions
        {
            get
            {
                if (_pageAction == null)
                    _pageAction = new List<PageAction>();

                return _pageAction;
            }
            set
            {
                this._pageAction = value;
            }
        }
    }
    public class PageAction
    {
        private string _callback;
        public LinkType LinkType { get; set; }
        public string LinkDisplayName { get; set; }
        public string Action { get; set; }
        public string HrefLink { get; set; }
        public string Controller { get; set; }
        public string ControlId { get; set; }
        public string Callback
        {
            get
            {
                return _callback;
            }
            set
            {
                _callback = value;
            }
        }
        public string DataTestSelector { get; set; }
        public string OnClick { get; set; }
        public object RouteParameters { get; set; }
    }

    public enum LinkType
    {
        AuthorizedPrototypeRawActionLink,
        UrlAction,
        OnClick,
        AuthorizedRawActionLink,
        Url,
    }
}
