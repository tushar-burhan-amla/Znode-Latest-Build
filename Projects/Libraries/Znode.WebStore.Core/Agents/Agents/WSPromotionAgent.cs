using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.WebStore;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;


namespace Znode.WebStore.Core.Agents
{
    public class WSPromotionAgent : BaseAgent, IWSPromotionAgent
    {
        #region Private Variables
        private readonly IPromotionClient _promotionClient;

        #endregion

        #region Constructor
        public WSPromotionAgent(IPromotionClient promotionClient)
        {
            _promotionClient = GetClient<IPromotionClient>(promotionClient);
        }
        #endregion

        #region Promotion

        //Get list of promotions.
        public virtual PromotionListViewModel GetPromotionListByPortalId(int portalId)
        {
            //set default sort of display order if not present.
            ExpandCollection expands = new ExpandCollection();
            expands = new ExpandCollection();
            expands.Add(ZnodePromotionEnum.ZnodePromotionType.ToString());
            //Set filter to get the promotion by portal Id.
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePortalEnum.PortalId.ToString(), FilterOperators.In, portalId.ToString()));

            PromotionListModel promotionListModel = _promotionClient.GetPromotionList(expands, filters, null, null, null);
            PromotionListViewModel listViewModel = null;
            DateTime currentdate = Convert.ToDateTime(DateTime.Now);
            listViewModel = new PromotionListViewModel { PromotionList = promotionListModel?.PromotionList?.ToViewModel<PromotionViewModel>().Where(x => x.StartDate <= currentdate && x.EndDate >= currentdate).OrderBy(x => x.OrderMinimum).ToList() };
            return promotionListModel?.PromotionList?.Count > 0 ? listViewModel : new PromotionListViewModel() { PromotionList = new List<PromotionViewModel>() };
        }


        #endregion



    }
}
