using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public class PublishProductDataService : BaseService, IPublishProductDataService, IPublishProcessValidationService
    {
        #region Private Variables

        private readonly IZnodeRepository<ZnodePublishCatalogLog> _publishCatalogLogRepository;
        private readonly IZnodeRepository<ZnodeExportProcessLog> _znodeExportProcessLog;

        #endregion Private Variables

        #region Publish Catalog Constructor

        public PublishProductDataService()
        {
            _publishCatalogLogRepository = new ZnodeRepository<ZnodePublishCatalogLog>();
            _znodeExportProcessLog = new ZnodeRepository<ZnodeExportProcessLog>();
        }

        #endregion Publish Catalog Constructor

        #region Public Methods

        //Check whether any other catalog is in publish state or not
        public virtual bool IsCatalogPublishInProgress()
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            byte publishStateId = Convert.ToByte(ZnodePublishStatesEnum.PROCESSING);

            bool isCatalogPublishInProgress = _publishCatalogLogRepository.Table.Any(x => x.IsCatalogPublished == null || x.PublishStateId == publishStateId);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return isCatalogPublishInProgress;
        }

        //Perform single product publish operation by calling master store procedure
        public virtual DataSet ProcessSingleProductPublish(int pimProductId, string revisionType, out bool status)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            try
            {
                List<int> pimProductIds = new List<int>();
                status = false;
                pimProductIds.Add(pimProductId);

                /*Call master sp and wait to finish sp operation to perform single product publish operation such as 
                adding data into publish table depending on revision type and pimProductId.
                NOTE : if Revisiontype : Null - only production, Preview - only preview, Production - preview & production */

                DataSet resultDataSet = null;
                ExecuteSpHelper executeSpHelper = new ExecuteSpHelper();
                DataTable pimProductIdTable = PublishHelper.ConvertKeywordListToDataTable(pimProductIds);

                executeSpHelper.SetTableValueParameter("@PimProductId", pimProductIdTable, ParameterDirection.Input, SqlDbType.Structured, "dbo.TransferId");
                executeSpHelper.GetParameter("@RevisionType", revisionType, ParameterDirection.Input, SqlDbType.NVarChar);
                executeSpHelper.GetParameter("@UserId", GetLoginUserId(), ParameterDirection.Input, SqlDbType.Int);

                //These parameters IsAutoPublish & ImportGUID is used in bulk import process publish.
                executeSpHelper.GetParameter("@IsAutoPublish", null , ParameterDirection.Input, SqlDbType.Bit);
                executeSpHelper.GetParameter("@ImportGUID", string.Empty, ParameterDirection.Input, SqlDbType.NVarChar);

                resultDataSet = executeSpHelper.GetSPResultInDataSet("Znode_PublishSingleProductEntity");

                if (resultDataSet?.Tables?.Count > 1 && resultDataSet?.Tables[0]?.Rows?.Count > 0)
                {
                    status = resultDataSet.Tables[2].AsEnumerable().Select(dataRow => (bool)dataRow.Field<object>("Status")).FirstOrDefault();
                }

                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return resultDataSet;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                ZnodeLogging.LogMessage("Single product master sp is failed to excute", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                throw ex;
            }
        }

        //Check requested product for publish is associated with publish category or not.
        public virtual bool IsProductAssociateWithPublishCategory(int pimProductId)
        {
            ZnodeRepository<ZnodePimCategoryProduct> _pimCategoryProductRepository = new ZnodeRepository<ZnodePimCategoryProduct>();
            ZnodeRepository<ZnodePimCategoryHierarchy> _pimCategoryHierarchyRepository = new ZnodeRepository<ZnodePimCategoryHierarchy>();
            return (from _pimCategoryProduct in _pimCategoryProductRepository.Table
                    join _pimCategoryHierarchy in _pimCategoryHierarchyRepository.Table on _pimCategoryProduct.PimCategoryId equals _pimCategoryHierarchy.PimCategoryId
                    where _pimCategoryProduct.PimProductId == pimProductId
                    select _pimCategoryProduct.PimProductId).Any();
        }


        //Create product(s) elastic index and delete old index of product(s)
        public virtual void CreateProductElasticIndex(List<ZnodePublishProductEntity> productEntities, string revisionType)
        {
            if (productEntities?.Count > 0)
            {
                ISearchService searchService = GetService<ISearchService>();

                //Performed this operation to get catalogid and localeid so that can loop through this combination.
                List<ZnodePublishProductEntity> productEntity = productEntities.GroupBy(x => new { x.ZnodeCatalogId, x.LocaleId }).Select(x => x.FirstOrDefault()).ToList();
                string indexName = string.Empty;
                foreach (var item in productEntity)
                {
                    IEnumerable<object> productIds = productEntities.Where(x => x.ZnodeCatalogId == item.ZnodeCatalogId && x.ZnodeCategoryIds > 0
                                                                          && x.LocaleId == item.LocaleId)
                                                                    .Select(x => x.ZnodeProductId).Distinct().Cast<object>();
                    indexName = PublishHelper.GetIndexName(item.ZnodeCatalogId);

                    if (productIds?.AsEnumerable().Count() > 0)
                    {
                        searchService.DeleteProduct(indexName, productIds, revisionType, Convert.ToString(item.VersionId));
                    }

                    List<ZnodePublishProductEntity> productsForCreateIndex = productEntities.FindAll(x => x.ZnodeCatalogId == item.ZnodeCatalogId
                                                                                 && x.ZnodeCategoryIds > 0 && x.LocaleId == item.LocaleId && x.ElasticSearchEvent != 2);
                    if (productsForCreateIndex?.Count > 0)
                        searchService.CreateProduct(indexName, productsForCreateIndex);
                }
            }
        }

        //Fetch appropriate revision type data for elastic search based on given revision type
        public virtual List<string> GetRevisionTypesForElasticIndex(string revisionType)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (revisionType == ZnodePublishStatesEnum.PRODUCTION.ToString())
            {
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return new List<string> { ZnodePublishStatesEnum.PREVIEW.ToString(), ZnodePublishStatesEnum.PRODUCTION.ToString() };
            }
            else if (revisionType == ZnodePublishStatesEnum.PREVIEW.ToString())
            {
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return new List<string> { ZnodePublishStatesEnum.PREVIEW.ToString() };
            }
            else
            {
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return new List<string> { ZnodePublishStatesEnum.PRODUCTION.ToString() };
            }
        }

        public virtual bool IsExportPublishInProgress()
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            byte publishStateId = Convert.ToByte(ZnodePublishStatesEnum.PROCESSING);

            bool isExportPublishInProgress = _znodeExportProcessLog.Table.Any(x => x.Status == ZnodeConstant.ExportStatusInprogress || x.Status == ZnodeConstant.SearchIndexStartedStatus);


            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return isExportPublishInProgress;
        }

        #endregion Public Methods       
    }
}
