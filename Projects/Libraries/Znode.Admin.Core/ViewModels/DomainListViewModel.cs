using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class DomainListViewModelss : BaseViewModel
    {
        #region Constructor
        /// <summary>
        /// Constructor for DomainViewModel that initializes the Domains list.
        /// </summary>
        public DomainListViewModelss()
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
        #endregion
    }
}
