using System.Collections.Generic;

namespace Znode.Engine.Promotions
{
    /// <summary>
    /// This is the base class for all promotion types.
    /// </summary>
    public class ZnodePromotionsType : IZnodePromotionsType
    {
        #region Private variables
        private string _className;
        private List<ZnodePromotionControl> _controls;
        #endregion

        #region Public Properties
        public string ClassName
        {
            get
            {
                if (string.IsNullOrEmpty(_className))
                    _className = GetType().Name;

                return _className;
            }
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public int Precedence { get; set; }
        public bool AvailableForFranchise { get; set; }

        public List<ZnodePromotionControl> Controls
        {
            get { return _controls ?? (_controls = new List<ZnodePromotionControl>()); }
        }
        #endregion
    }
}
