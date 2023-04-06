using System.Collections.Generic;
using Znode.Libraries.ECommerce.Entities;

namespace Znode.Engine.Promotions
{
    /// <summary>
    /// This is the root interface for all promotion types.
    /// </summary>
    public interface IZnodePromotionsType : IZnodeProviderType
	{
        /// <summary>
        /// Order of precedence of a promotion.
        /// </summary>
		int Precedence { get; set; }

        /// <summary>
        /// Is promotion available for franchise.
        /// </summary>
		bool AvailableForFranchise { get; set; }

		List<ZnodePromotionControl> Controls { get; }
	}
}
