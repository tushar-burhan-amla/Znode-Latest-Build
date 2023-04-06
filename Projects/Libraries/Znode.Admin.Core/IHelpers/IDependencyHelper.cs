using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Models;
using System.Collections.Generic;

namespace Znode.Admin.Core.Helpers
{
    public interface IDependencyHelper
    {
        StoreViewModel GetCurrentPortal();

        MediaAttributeValuesListViewModel GetMediaAttributeValues(int media);

        MediaConfigurationViewModel GetDefaultMediaConfiguration();
        

        Engine.Admin.Models.ViewModel SaveListView(string viewName, int viewId, string xml, string xmlSettingName, FilterCollection filters, string sortColumn,string sortType,bool isPublic,bool isDefault,int userId );

        //Get filter configuration XML.
        ApplicationSettingDataModel GetFilterConfigurationXML(string listName, int? userId = null);

        IEnumerable<RolePermissionViewModel> GetPermissions();

        bool IsUserInRole(bool isSaleRep = false);

        Engine.Admin.Models.ViewModel GetFilterXML(int viewId = 0);

    }
}
