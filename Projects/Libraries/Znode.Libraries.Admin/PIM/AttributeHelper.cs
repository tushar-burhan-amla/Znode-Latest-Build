using System.Linq;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
namespace Znode.Libraries.Admin
{
    public class AttributeHelper
    {
        #region Private Variables

        private readonly IZnodeRepository<ZnodePimAttribute> _attributeRepository;

        #endregion

        #region Public Constructor
        public AttributeHelper()
        {
            _attributeRepository = new ZnodeRepository<ZnodePimAttribute>();
        }
        #endregion

        #region Public Method

        /// <summary>
        /// Get Attribute ID by code
        /// </summary>
        /// <param name="attributeCode">Attribute Code</param>
        /// <returns>Return attribute ID </returns>
        public virtual int GetAttributeIdByCode(string attributeCode) =>
            _attributeRepository.Table.FirstOrDefault(x => x.AttributeCode == attributeCode).PimAttributeId;

        #endregion
    }
}
