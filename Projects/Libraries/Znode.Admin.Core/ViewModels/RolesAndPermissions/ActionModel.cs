namespace Znode.Engine.Admin.ViewModels
{
    public class ActionModel : BaseViewModel
    {
        private string _callback = "";
        private bool _isCancelIsShow = true;
        private bool _isSaveIsShow = true;
        private bool _isSaveCloseIsShow = true;
        public string Action { get; set; }
        public string Controller { get; set; }
        public string FormId { get; set; }
        public string SaveHeader { get; set; }
        public string CancelHeader { get; set; }
        public string SaveId { get; set; }
        public string CancelId { get; set; }
        public string CancelUrl { get; set; }
        public bool IsSaveCloseEnable { get; set; }
        public bool IsCancelIsShow
        {
            get { return _isCancelIsShow; }
            set { _isCancelIsShow = value; }
        }
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

        public bool IsSaveIsShow
        {
            get
            {
                return _isSaveIsShow;
            }

            set
            {
                _isSaveIsShow = value;
            }
        }

        public bool IsSaveCloseIsShow
        {
            get
            {
                return _isSaveCloseIsShow;
            }

            set
            {
                _isSaveCloseIsShow = value;
            }
        }
    }
}