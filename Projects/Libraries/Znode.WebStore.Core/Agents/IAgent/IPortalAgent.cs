using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.WebStore.ViewModels;

namespace Znode.Engine.WebStore.Agents
{
    public interface IPortalAgent
    {
        /// <summary>
        /// Change locale for current portal.
        /// </summary>
        /// <param name="localeId">New locale id to be set for current portal.</param>
        /// <returns>Returns true if locale changed successfully else return false.</returns>
        bool ChangeLocale(string localeId);

        /// <summary>
        /// Get Robots.Txt data.
        /// </summary>
        /// <returns>Returns model with data.</returns>
        RobotsTxtViewModel GetRobotsTxt();
       
        /// <summary>
        /// Get current portal.
        /// </summary>
        /// <returns>PortalViewModel</returns>
        PortalViewModel GetCurrentPortal();

        /// <summary>
        /// Set Logging Configuration Settings.
        /// </summary>
        void SetGlobalLoggingSetting();

        /// <summary>
        /// Get barcode scanner details
        /// </summary>        
        BarcodeReaderViewModel GetBarcodeScannerDetail();

#if DEBUG
        /// <summary>
        /// Gets list of selectable portals
        /// </summary>
        /// <returns>List of portal for the selection</returns>
        List<SelectListItem> GetDevPortalSelectList();
#endif

    }
}
