using System;
using System.Collections.Generic;
using System.Web;
namespace Znode.Engine.WebStore
{
    public class WidgetParameter : IDisposable
    {
        public int CMSMappingId { get; set; }
        public int LocaleId { get; set; }
        public string WidgetCode { get; set; }
        public string WidgetKey { get; set; }
        public string TypeOfMapping { get; set; }
        public string DisplayName { get; set; }
        public Dictionary<string, object> properties { get; set; }
        public string PartialViewName { get; set; }
        bool disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //dispose managed resources
                }
            }
            //dispose unmanaged resources
            disposed = true;
        }

        public bool IsEmpty { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool CMSMode
        {
            get
            {
                string _cmsMode = HttpContext.Current.Request.QueryString["cmsmode"];

                return string.IsNullOrEmpty(_cmsMode) ? false : true;
            }
        }
    }
}