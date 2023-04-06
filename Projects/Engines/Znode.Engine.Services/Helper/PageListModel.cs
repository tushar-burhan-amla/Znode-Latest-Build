using MongoDB.Driver;
using System;
using System.Collections.Specialized;
using Znode.Engine.Services.Constants;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.MongoDB.Data;

namespace Znode.Engine.Services
{
    public class PageListModel
    {
        #region private variables
        private readonly FilterCollection _filters;
        private readonly NameValueCollection _sorts;      
        #endregion

        #region constructor
        public PageListModel(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            _filters = filters;
            _sorts = sorts;          
            SetPaging(page, out PagingStart, out PagingLength);    
        }

       
        #endregion

        #region Public Properties
        public int PagingStart;
        public int PagingLength;
        public int TotalRowCount;
        public string OrderBy
        {
            get
            {
                return DynamicClauseHelper.GenerateDynamicOrderByClause(_sorts);
            }
        }

        public string SPWhereClause
        {
            get
            {
                return DynamicClauseHelper.GenerateDynamicWhereClauseForSP(_filters.ToFilterDataCollection());
            }
        }

        public EntityWhereClauseModel EntityWhereClause
        {
            get
            {
                return DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(_filters.ToFilterDataCollection());
            }
        }

        public IMongoQuery MongoWhereClause
        {
            get
            {
                return MongoQueryHelper.GenerateDynamicWhereClause(_filters.ToFilterMongoCollection());
            }
        }

        public IMongoSortBy MongoOrderBy
        {
            get
            {
                return MongoQueryHelper.GenerateDynamicOrderByClause(_sorts);
            }
        }

        public string ToDebugString()
        {
            return $"WhereClause={MongoQueryHelper.GenerateDynamicWhereClause(_filters.ToFilterMongoCollection())},PagingLength={PagingLength},PagingStart={PagingStart},TotalRowCount={TotalRowCount}";
        }

        #endregion

        #region Private Method
        private void SetPaging(NameValueCollection page, out int pagingStart, out int pagingLength)
        {
            // We use int.MaxValue for the paging length to ensure we always get total results back
            pagingStart = 1;
            pagingLength = int.MaxValue;
            if (!Equals(page, null) && page.HasKeys())
            {
                // Only do if both index and size are given
                if (!string.IsNullOrEmpty(page.Get(PageKeys.Index)) && !string.IsNullOrEmpty(page.Get(PageKeys.Size)))
                {
                    pagingStart = Convert.ToInt32(page.Get(PageKeys.Index));
                    pagingLength = Convert.ToInt32(page.Get(PageKeys.Size));
                }
            }
        }    
        #endregion
    }
}
