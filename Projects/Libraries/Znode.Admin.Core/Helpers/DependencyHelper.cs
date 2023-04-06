using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Models;
using System.Collections.Generic;
using System.Web;

namespace Znode.Admin.Core.Helpers
{
    public class DependencyHelper : BaseAgent, IDependencyHelper
    {
        public StoreViewModel GetCurrentPortal()
        {
            return new StoreAgent(GetClient<PortalClient>(), GetClient<EcommerceCatalogClient>(), GetClient<ThemeClient>(), GetClient<DomainClient>(), GetClient<PriceClient>(), GetClient<OrderStateClient>(),
                GetClient<ProductReviewStateClient>(), GetClient<PortalProfileClient>(), GetClient<WarehouseClient>(), GetClient<CSSClient>(), GetClient<ManageMessageClient>(), GetClient<ContentPageClient>(), GetClient<TaxClassClient>(),
                GetClient<PaymentClient>(), GetClient<ShippingClient>(), GetClient<PortalCountryClient>(),GetClient<TagManagerClient>(), GetClient<GeneralSettingClient>()).GetCurrentPortal();
        }

        public MediaAttributeValuesListViewModel GetMediaAttributeValues(int media)
        {
            return new MediaManagerAgent(GetClient<MediaManagerClient>(), GetClient<MediaConfigurationClient>()).GetMediaAttributeValues(media);
        }

        public MediaConfigurationViewModel GetDefaultMediaConfiguration()
        {
            return new MediaConfigurationAgent(GetClient<MediaConfigurationClient>()).GetDefaultMediaConfiguration();
        }

        public Engine.Admin.Models.ViewModel SaveListView(string viewName, int viewId, string xml, string xmlSettingName, FilterCollection filters, string sortColumn,string sortType,bool isPublic,bool isDefault,int userId )
        {
            IApplicationSettingsAgent applicationSettingAgent = new ApplicationSettingsAgent(GetClient<ApplicationSettingsClient>());
            return applicationSettingAgent.CreateNewView(new Engine.Admin.Models.ViewModel { ViewName = viewName, ViewId = viewId, Filters = filters, XmlSetting = xml, XmlSettingName = xmlSettingName, SortColumn = sortColumn, SortType = sortType,IsPublic=isPublic,IsDefault=isDefault,UserId=userId
        });
        }

        //Get filter configuration XML.
        public ApplicationSettingDataModel GetFilterConfigurationXML(string listName, int? userId=null)
        => new ApplicationSettingsAgent(GetClient<ApplicationSettingsClient>())?.GetFilterConfigurationXML(listName, userId);

        public IEnumerable<RolePermissionViewModel> GetPermissions()
        {
            IRoleAgent roleAgent = new RoleAgent(GetClient<RoleClient>());
            return roleAgent.GetRolePermission(HttpContext.Current.User.Identity.Name);
        }

        public bool IsUserInRole(bool isSalesRep = false)
        {
            //Check for the User Role is Admin or not.
            RoleAgent roleAgent = new RoleAgent(GetClient<RoleClient>());
            if (isSalesRep)
            {
                return roleAgent.IsUserInRole(HttpContext.Current.User.Identity.Name, Engine.Admin.Helpers.AdminConstants.SalesRepRole);
            }
            else
            {
                return roleAgent.IsUserInRole(HttpContext.Current.User.Identity.Name, ZnodeRoleEnum.Admin.ToString());
            }
        }

        //Get filter XML.
        public Engine.Admin.Models.ViewModel GetFilterXML(int viewId = 0)
        => new ApplicationSettingsAgent(GetClient<ApplicationSettingsClient>())?.GetView(viewId);

    }
}
