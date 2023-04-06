using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public class AddressService : BaseService, IAddressService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeAddress> _addressRepository;
        #endregion

        #region Constructor
        //Constructor
        public AddressService()
        {
            _addressRepository = new ZnodeRepository<ZnodeAddress>();
        }
        #endregion

        #region Public Methods
        public virtual AddressListModel GetAddressList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            SetFilterData(filters);
            List<ZnodeAddress> addressList = _addressRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount).ToList();

            AddressListModel addresslistModel = AddressMap.ToListModel(addressList);
            //Set one time address availability flag
            addresslistModel?.AddressList?.ForEach(o => o.DontAddUpdateAddress = true);

            return addresslistModel;
        }
        #endregion

        #region Private Methods
        private void SetFilterData(FilterCollection filters)
        {
            filters = HelperUtility.IsNull(filters) ? new FilterCollection() : filters;
            string filterValue = filters.Where(x => x.Item1.ToLower() == ZnodeAddressEnum.AddressId.ToString().ToLower())?.Select(y => y.Item3)?.FirstOrDefault();

            if (!string.IsNullOrEmpty(filterValue))
            {
                filters.RemoveAll(x => x.FilterName == ZnodeAddressEnum.AddressId.ToString().ToLower());
                filterValue = filterValue.Replace('_', ',');
                filters.Add(ZnodeAddressEnum.AddressId.ToString(), FilterOperators.In, filterValue);
            }
        }

        #endregion
    }
}