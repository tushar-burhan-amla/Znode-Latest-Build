using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Api.Models;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Maps
{
    public class ERPConfiguratorViewModelMap
    {
        #region ERP Configurator

        //Convert IEnumerable type list of ERP Configurator Model to List<SelectListItem>
        public static List<SelectListItem> ToERPConfiguratorClassesListItems(IEnumerable<ERPConfiguratorModel> eRPConfiguratorModel)
        {
            List<SelectListItem> eRPConfiguratorClassList = new List<SelectListItem>();
            if (!Equals(eRPConfiguratorModel, null))
            {
                eRPConfiguratorClassList = (from eRPConfigurator in eRPConfiguratorModel
                                   select new SelectListItem
                                   {
                                       Text = eRPConfigurator.ClassName,
                                       Value = eRPConfigurator.ClassName
                                   }).ToList();
            }
            eRPConfiguratorClassList.Insert(0, new SelectListItem() { Value = "", Text = ERP_Resources.LabelSelectPackageName });

            return eRPConfiguratorClassList;
        }
        #endregion


    }
}