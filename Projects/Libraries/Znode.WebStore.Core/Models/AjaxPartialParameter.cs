using System;
using System.Collections.Generic;
using System.Web;
namespace Znode.Engine.WebStore
{
    public class AjaxPartialParameter : IDisposable
    {
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string Parameters { get; set; }
        public string Identifier { get; set; }
		public string ReplaceTargetSelector { get; set; }
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}