using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class DomainListViewModel : BaseViewModel
    {
        #region Constructor
        /// <summary>
        /// Constructor for DomainViewModel that initialize the Domains list.
        /// </summary>
        public DomainListViewModel()
        {
            GridModel = new GridModel();
            Domains = new List<DomainViewModel>();
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets all Domain list.
        /// </summary>
        public List<DomainViewModel> Domains { get; set; }
        public GridModel GridModel { get; set; }
        public int PortalId { get; set; }
        public string PortalName { get; set; }
        #endregion
    }
}
